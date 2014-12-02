namespace MAIG_RoboCode_Project.MCTS
{
    using System.Collections.Generic;
    using System.Linq;

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

            return null;
        }

        public TreeNode expand()
        {
            return null;
        }

        public void UpdateValues(double score)
        {
            
        }

        public bool IsTerminalNode()
        {
            return false;
        }

        public bool IsLeafNode()
        {
            return false;
        }

        public double GetScore()
        {
            return 0.0;
        }
    }
}
