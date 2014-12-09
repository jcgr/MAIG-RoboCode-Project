namespace MAIG_RoboCode_Project.GameObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A game state in the Monte-Carlo Tree Search algorithm for Robocode.
    /// </summary>
    public class Gamestate
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gamestate"/> class. 
        /// </summary>
        /// <param name="ours">The information about the MCTS-robot.</param>
        /// <param name="enemy">The information about the opponent robot.</param>
        /// <param name="projectilesInFlight">The projectiles currently in flight.</param>
        /// <param name="ri">The instructions used to arrive at this game state.</param>
        public Gamestate(RobotInfo ours, RobotInfo enemy, List<Projectile> projectilesInFlight, RobotInstructions ri)
        {
            this.OurRobot = ours;
            this.EnemyRobot = enemy;
            this.FlyingProjectiles = projectilesInFlight;
            this.Instructions = ri;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the robot belonging to the MCTS-robot.
        /// </summary>
        public RobotInfo OurRobot { get; private set; }

        /// <summary>
        /// Gets the robot of the opponent.
        /// </summary>
        public RobotInfo EnemyRobot { get; private set; }

        /// <summary>
        /// Gets the list of projectiles that are currently in flight in the simulation.
        /// </summary>
        public List<Projectile> FlyingProjectiles { get; private set; }

        /// <summary>
        /// Gets the instructions for the MCTS robot used to arrive at this game state from the previous game state.
        /// </summary>
        public RobotInstructions Instructions { get; private set; }
        #endregion
       
        /// <summary>
        /// Simulates a turn in the game.
        /// </summary>
        /// <param name="gs"> The game state to simulate from.</param>
        /// <param name="ours">Instructions for the MCTS-robot.</param>
        /// <param name="enemy">The instructions for the enemy robot.</param>
        /// <returns>The newly simulated <see cref="Gamestate"/>.</returns>
        public static Gamestate SimulateTurn(Gamestate gs, RobotInstructions ours, RobotInstructions enemy = null)
        {
            // Move all projectiles
            var movedProjectiles = gs.FlyingProjectiles.Select(p => p.NextTick()).ToList();

            // Get instructions for the enemy
            enemy = enemy ?? RobotInstructions.GetEnemyInstructions(gs, movedProjectiles);

            // Move robots according to instructions
            var ourRobot = gs.OurRobot.NextTick(ours, movedProjectiles, gs.EnemyRobot);
            var enemyRobot = gs.EnemyRobot.NextTick(enemy, movedProjectiles, gs.OurRobot);

            // Increase survival scores if a robot was destroyed
            if (enemyRobot.Status == RoboStatus.Destroyed && ourRobot.Status != RoboStatus.Destroyed)
            {
                ourRobot.ScoreList["survival"] = 1;
            }
            else if (ourRobot.Status == RoboStatus.Destroyed && enemyRobot.Status != RoboStatus.Destroyed)
            {
                enemyRobot.ScoreList["survival"] = 1;
            }
                
            // Adds change in robot heading to score list
            var diff = Math.Abs(ourRobot.RobotHeading - gs.OurRobot.RobotHeading);
            var difference = diff > 180 ? 360 - diff : diff;

            ourRobot.ScoreList["robotHeading"] = difference;

            ourRobot.ScoreList["movementScore"] = 0;

            if (ourRobot.X < 50)
            {
                ourRobot.ScoreList["movementScore"] += ourRobot.X - 50;
            }
            else if (ourRobot.X > Global.BfWidth - 50)
            {
                ourRobot.ScoreList["movementScore"] += ourRobot.X - Global.BfWidth;
            }

            if (ourRobot.Y < 50)
            {
                ourRobot.ScoreList["movementScore"] += ourRobot.Y - 50;
            }
            else if (ourRobot.Y > Global.BfHeight - 50)
            {
                ourRobot.ScoreList["movementScore"] += ourRobot.Y - Global.BfHeight;
            }

            // If robot decreased velocity, decrease score. Else increase it by how much it increased.
            if (Math.Abs(Math.Abs(ourRobot.Velocity) - Math.Abs(gs.OurRobot.Velocity)) < Global.Tolerance)
            {
                ourRobot.ScoreList["movementScore"] += -1;
            }
            else
            {
                if ((Math.Abs(ourRobot.X - gs.OurRobot.X) < Global.Tolerance && Math.Abs(ourRobot.Y - gs.OurRobot.Y) < Global.Tolerance) && Math.Abs(ourRobot.Velocity - gs.OurRobot.Velocity) > Global.Tolerance)
                {
                    ourRobot.ScoreList["movementScore"] += -1;
                }
                else
                {
                    ourRobot.ScoreList["movementScore"] += Math.Abs(ourRobot.Velocity - gs.OurRobot.Velocity);
                }
            }
            
            // Awards the robot for attempting to fire
            if (ours.FirePower > 0 && gs.OurRobot.CanFire)
            {
                ourRobot.ScoreList["shootScore"] = 1; 
            }

            return new Gamestate(ourRobot, enemyRobot, movedProjectiles, ours);
        }
    }
}