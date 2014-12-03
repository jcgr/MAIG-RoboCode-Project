namespace MAIG_RoboCode_Project.MonteCarlo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAIG_RoboCode_Project.GameObjects;

    public class TreeNode
    {
        public int Visits { get; private set; }

        public TreeNode Parent { get; private set; }

        public List<TreeNode> Children { get; private set; }

        public double UctValue { get; private set; }

        public Gamestate Gamestate { get; private set; }

        public double PathLength { get; private set; }

        public double AverageScore { get; private set; }

        public double MaxScore { get; private set; }

        public TreeNode(Gamestate gs, TreeNode parent, double plength)
        {
            this.Gamestate = gs;
            this.Parent = parent;
            this.PathLength = plength;
            
            this.Children = new List<TreeNode>();
            this.Visits = 0;
            this.AverageScore = 0.0;
            this.MaxScore = 0.0;


        }

        public TreeNode BestChild()
        {
            // If there are any children visited less often that the threshold, visit one of them first.
            var childrenBeneathThreshold = this.Children.Where(child => child.Visits < Global.MCTS_VISIT_THRESHOLD).ToList();
            var cbt = childrenBeneathThreshold.Count;

            // Choose randomly between children with fewer visit than the threshold.
            if (cbt > 0)
            {
                return childrenBeneathThreshold[Global.Random.Next(cbt)];
            }

            TreeNode bestChild = null;
            double bestValue = double.MinValue;

            // Calculate the UCT value of all this node's children and choose the best to return.
            foreach (TreeNode child in this.Children)
            {
                var log = Math.Log(this.Visits);
                var div = log / child.Visits;
                var sqrt = Math.Sqrt(div);
                var explorationValue = Global.MCTS_EXPLORATION_CONSTANT * sqrt;
                child.UctValue = child.GetScore() + explorationValue;
                
                if (child.UctValue > bestValue)
                {
                    bestChild = child;
                    bestValue = child.UctValue;
                }
            }

            return bestChild;
        }

        public TreeNode expand()
        {
            var ourRobot = this.Gamestate.OurRobot;
            var possibleMoves = RobotInstructions.GetListOfInstructions(ourRobot.Velocity, ourRobot.CanFire);

            foreach (var ri in possibleMoves)
            {
                var newGameState = Gamestate.SimulateTurn(this.Gamestate, ri);
                var newNode = new TreeNode(newGameState, this, this.PathLength + 1.0);
                this.Children.Add(newNode);
            }

            return this.BestChild();
        }

        public void UpdateValues(double score)
        {
            this.Visits++;

            // If no children, average score over 1
            var childrenCount = this.Children.Count == 0 ? 1 : this.Children.Count;
            var scoreSum = 0.0;
            foreach (var tn in this.Children)
            {
                scoreSum += tn.AverageScore;
            }

            this.AverageScore = scoreSum / childrenCount;

            if (score > this.MaxScore)
            {
                this.MaxScore = score;
            }

        }

        public bool IsTerminalNode()
        {
            var robotDead = this.Gamestate.OurRobot.Status == RoboStatus.Destroyed
                            || this.Gamestate.EnemyRobot.Status == RoboStatus.Destroyed;

            return robotDead || this.PathLength >= Global.MCTS_MAX_PATH_TO_ROOT;
        }

        public bool IsLeafNode()
        {
            var ourRobot = this.Gamestate.OurRobot;
            return this.Children.Count != RobotInstructions.GetListOfInstructions(ourRobot.Velocity, ourRobot.CanFire).Count;
        }

        public double GetScore()
        {
            var ourWeightedScore = this.Gamestate.OurRobot.GetScore() * Global.PLAYER_SCORE_WEIGHT;
            var enemyWeightedScore = this.Gamestate.EnemyRobot.GetScore() * Global.ENEMY_SCORE_WEIGHT;
            return ourWeightedScore - enemyWeightedScore;
        }
    }
}
