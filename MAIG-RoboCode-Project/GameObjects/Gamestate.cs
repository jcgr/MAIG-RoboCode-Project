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

            // Calculate angle to enemy
            var gunAngleToEnemy = Global.XYToDegree(enemyRobot.X, enemyRobot.Y, ourRobot.X, ourRobot.Y);
            var angle1 = Math.Abs(gunAngleToEnemy - ourRobot.GunHeading);
            var angle2 = Math.Abs(ourRobot.GunHeading - gunAngleToEnemy);
                
            // Adds angle difference to score list
            ourRobot.ScoreList["gunDirection"] = angle1 < angle2 ? -angle1 : -angle2;

            // If robot decreased velocity, decrease score. Else increase it by how much it increased.
            if (Math.Abs(ourRobot.Velocity) <= Math.Abs(gs.OurRobot.Velocity))
            {
                ourRobot.ScoreList["movementScore"] = -1;
            }
            else
            {
                if (Math.Abs(ourRobot.X - gs.OurRobot.X) < Global.Tolerance && Math.Abs(ourRobot.Y - gs.OurRobot.Y) < Global.Tolerance)
                {
                    ourRobot.ScoreList["movementScore"] = Math.Abs(ourRobot.Velocity - gs.OurRobot.Velocity);
                }
                else
                {
                    ourRobot.ScoreList["movementScore"] = -1;
                }
            }
            
            // Awards the robot for attempting to fire
            if (ours.FirePower > 0 && gs.OurRobot.CanFire)
            {
                ourRobot.ScoreList["shootScore"] = 1; // TODO: Should it total the scores down the tree or only score per gamestate?
            }

            return new Gamestate(ourRobot, enemyRobot, movedProjectiles, ours);
        }
    }
}
