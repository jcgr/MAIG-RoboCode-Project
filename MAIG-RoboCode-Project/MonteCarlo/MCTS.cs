﻿﻿namespace MAIG_RoboCode_Project.MonteCarlo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using MAIG_RoboCode_Project.GameObjects;

    public class MCTS
    {
        public static void Main()
        {
            // Test method
            var mcts = new MCTS();

            var us = new RobotInfo(100, 0, 25, 25, 0, 0, 0, 0, "US");
            var them = new RobotInfo(100, 0, 75, 75, 0, 0, 0, 0, "THEM");

            var gamestate = new Gamestate(us, them, new List<Projectile>(), null);

            mcts.Search(gamestate);
        }

        /// <summary>
        /// The root node of the tree.
        /// </summary>
        public TreeNode Root { get; private set; }

        /// <summary>
        /// The current iteration of the tree search.
        /// </summary>
        private int CurrIteration { get; set; }

        /// <summary>
        /// Runs a Monte-Carlo tree search starting from the current game state.
        /// </summary>
        /// <param name="game">The gamestate to start the search at.</param>
        /// <returns>The treenode that contains the best action.</returns>
        public TreeNode Search(Gamestate game)
	    {
		    this.Root = new TreeNode(game, null, 0);

            var sw = new Stopwatch();
            sw.Start();

		    this.CurrIteration = 0;
		    while (sw.ElapsedMilliseconds < Global.MCTSAllowedSearchTime
				    && this.CurrIteration < Global.MCTSMaxIterations)
		    {
			    // Selection + Expansion
			    var currNode = this.Selection();

			    // Playout / simulation
			    var tempNode = this.Playout(currNode);  

			    // Backpropagation
			    this.Backpropagate(currNode, tempNode);
			    this.CurrIteration++;
		    }

		    TreeNode bestNode = null;
		    double bestNodeScore = double.MinValue;

		    foreach (var tn in this.Root.Children)
		    {
			    if (tn.GetScore() >= bestNodeScore)
			    {
				    bestNode = tn;
				    bestNodeScore = tn.GetScore();
			    }
		    }

		    return bestNode;
	    }

        /// <summary>
        /// Selects the best child or expands the node. Selection/Expansion from MCTS.
        /// </summary>
        /// <returns>The newly expanded node or the best child.</returns>
        private TreeNode Selection()
        {
            var tempNode = this.Root;

            while (!tempNode.IsTerminalNode())
                if (!tempNode.IsLeafNode())
                    tempNode = tempNode.BestChild();
                else
                    return tempNode.Expand();

            return tempNode;
        }

        /// <summary>
        /// Simulates a play starting at the given node.
        /// </summary>
        /// <param name="node">The node to start the simulation at.</param>
        /// <returns>The final node of the simulation.</returns>
        private TreeNode Playout(TreeNode node)
        {
            var tempNode = node;

            while (!tempNode.IsTerminalNode())
            {
                var tempGame = tempNode.Gamestate;
                var ourRobot = tempGame.OurRobot;
                var possibleActions = RobotInstructions.GetListOfInstructions(ourRobot.Velocity, ourRobot.CanFire);
                var randomInstruction = possibleActions[Global.Random.Next(possibleActions.Count)];

                tempGame = Gamestate.SimulateTurn(tempGame, randomInstruction);

                tempNode = new TreeNode(tempGame, tempNode, tempNode.PathLength + 1.0);
            }

            return tempNode;
        }

        /// <summary>
        /// Backpropagates the tree, using the given node as start point.
        /// </summary>
        /// <param name="currNode">The node the simulation was started at.</param>
        /// <param name="endNode">The node the simulation ended at.</param>
        public void Backpropagate(TreeNode currNode, TreeNode endNode)
        {
            TreeNode tempNode = currNode;
            double score = endNode.GetScore();

            while (tempNode != null)
            {
                tempNode.UpdateValues(score);
                tempNode = tempNode.Parent;
            }
        }
    }
}
