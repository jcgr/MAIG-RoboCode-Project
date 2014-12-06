namespace MAIG_RoboCode_Project.MonteCarlo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAIG_RoboCode_Project.GameObjects;

    /// <summary>
    /// A node in the tree of the Monte-Carlo Tree Search algorithm.
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class.
        /// </summary>
        /// <param name="gs"> The game state for this node. </param>
        /// <param name="parent"> The parent of this node. </param>
        /// <param name="plength"> The length of the path back to the root. </param>
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

        #region Properties
        /// <summary>
        /// Gets the amount of visits to this node.
        /// </summary>
        public int Visits { get; private set; }

        /// <summary>
        /// Gets the parent of this node.
        /// </summary>
        public TreeNode Parent { get; private set; }

        /// <summary>
        /// Gets the children of this node.
        /// </summary>
        public List<TreeNode> Children { get; private set; }

        /// <summary>
        /// Gets the UCT-value of this node.
        /// </summary>
        public double UctValue { get; private set; }

        /// <summary>
        /// Gets the gamestate attached to this node.
        /// </summary>
        public Gamestate Gamestate { get; private set; }

        /// <summary>
        /// Gets the length of the path back to the root from this node.
        /// </summary>
        public double PathLength { get; private set; }

        /// <summary>
        /// Gets the average score of this node's children.
        /// </summary>
        public double AverageScore { get; private set; }

        /// <summary>
        /// Gets the highest score of any child of this node.
        /// </summary>
        public double MaxScore { get; private set; }
        #endregion

        /// <summary>
        /// Gets the best child of this node.
        /// </summary>
        /// <returns>The best child of the node</returns>
        public TreeNode BestChild()
        {
            // If there are any children visited less often that the threshold, visit one of them first.
            var childrenBeneathThreshold = this.Children.Where(child => child.Visits < Global.MCTSVisitThreshold).ToList();
            var cbt = childrenBeneathThreshold.Count;

            // Choose randomly between children with fewer visit than the threshold.
            if (cbt > 0)
            {
                return childrenBeneathThreshold[Global.Random.Next(cbt)];
            }

            TreeNode bestChild = null;
            var bestValue = double.MinValue;

            // Calculate the UCT value of all this node's children and choose the best to return.
            foreach (TreeNode child in this.Children)
            {
                var log = Math.Log(this.Visits);
                var div = log / child.Visits;
                var sqrt = Math.Sqrt(div);
                var explorationValue = Global.MCTSExplorationConstant * sqrt;
                child.UctValue = child.GetScore() + explorationValue;

                if (child.UctValue > bestValue)
                {
                    bestChild = child;
                    bestValue = child.UctValue;
                }
            }

            return bestChild;
        }

        /// <summary>
        /// Expands the node to create the possible children and then gets the best of those children.
        /// </summary>
        /// <returns>The best child of this node.</returns>
        public TreeNode Expand()
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

        /// <summary>
        /// Updates the visits and score values of this node.
        /// </summary>
        /// <param name="score"> The score to use in the update. </param>
        public void UpdateValues(double score)
        {
            this.Visits++;

            // If no children, average score over 1
            var childrenCount = this.Children.Count == 0 ? 1 : this.Children.Count;
            var scoreSum = this.Children.Sum(tn => tn.AverageScore);

            this.AverageScore = scoreSum / childrenCount;

            if (score > this.MaxScore)
            {
                this.MaxScore = score;
            }
        }

        /// <summary>
        /// Checks if this node is a terminal node.
        /// </summary>
        /// <returns>True if this is a terminal node; false if not.</returns>
        public bool IsTerminalNode()
        {
            var robotDead = this.Gamestate.OurRobot.Status == RoboStatus.Destroyed
                            || this.Gamestate.EnemyRobot.Status == RoboStatus.Destroyed;

            return robotDead || this.PathLength >= Global.MCTSMaxPathToRoot;
        }

        /// <summary>
        /// Checks if this node is a leaf node (i.e. not fully expanded)
        /// </summary>
        /// <returns>True if this node is a leaf; false if not.</returns>
        public bool IsLeafNode()
        {
            var ourRobot = this.Gamestate.OurRobot;
            return this.Children.Count != RobotInstructions.GetListOfInstructions(ourRobot.Velocity, ourRobot.CanFire).Count;
        }

        /// <summary>
        /// Gets the score of this node.
        /// </summary>
        /// <returns>The score of the node.</returns>
        public double GetScore()
        {
            var ourWeightedScore = this.Gamestate.OurRobot.GetScore() * Global.PlayerScoreWeight;
            var enemyWeightedScore = this.Gamestate.EnemyRobot.GetScore() * Global.EnemyScoreWeight;
            return ourWeightedScore - enemyWeightedScore;
        }
    }
}
