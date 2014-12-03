namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using MAIG_RoboCode_Project.GameObjects;
    using MAIG_RoboCode_Project.MonteCarlo;

    using Robocode;

    /// <summary>
    /// Robocode robot based on Monte-Carlo Tree Search.
    /// </summary>
    public class MCTSRobot : Robot
    {
        /// <summary>
        /// Gets or sets the enemy robot.
        /// </summary>
        private RobotInfo Enemy { get; set; }

        /// <summary>
        /// Gets or sets the projectiles that are currently assumed to be in the air.
        /// </summary>
        private List<Projectile> Projectiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the robot should scan.
        /// </summary>
        private bool ShouldScan { get; set; }

        // The main method of the robot containing robot logics
        public override void Run()
        {
            Global.BF_WIDTH = this.BattleFieldWidth;
            Global.BF_HEIGHT = this.BattleFieldHeight;

            ShouldScan = true;
            this.RadarColor = Color.Blue;
            this.ScanColor = Color.Yellow;

            var mcts = new MCTS();
            this.Projectiles = new List<Projectile>();
            this.Enemy = GenerateAssumedEnemy();

            while (true)
            {
                while (ShouldScan)
                {
                    this.TurnGunRight(20);
                }

                var gs = new Gamestate(new RobotInfo(this), Enemy, Projectiles, null);

                var tn = mcts.Search(gs);

                var i = tn.Gamestate.Instructions;

                if (i.RobotDegrees != 0)
                {
                    this.TurnRight(i.RobotDegrees);
                    Console.WriteLine("robot degrees " + i.RobotDegrees);
                }

                if (i.GunDegrees != 0)
                {
                    this.TurnGunRight(i.GunDegrees);
                    Console.WriteLine("gun degrees " + i.GunDegrees);
                }

                if (i.RadarDegrees != 0)
                {
                    this.TurnRadarRight(i.RadarDegrees);
                    Console.WriteLine("radar degress " + i.RadarDegrees);
                }

                if (i.MoveDistance != 0)
                {
                    this.Ahead(i.MoveDistance);
                    Console.WriteLine("move distance " + i.MoveDistance);
                }

                if (i.FirePower != 0 && this.GunHeat >= 0)
                {
                    this.Fire(i.FirePower);
                    this.Projectiles.Add(new Projectile(this.X, this.Y, this.GunHeading, i.FirePower, true, this.Name));
                    Console.WriteLine("fire power" + i.FirePower);
                }
            }
        }

        // Robot event handler, when the robot sees another robot
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            this.ShouldScan = false;
            var angle = ((this.Heading + e.Bearing) + 360) % 360;

            var enemyPosition = Global.DegreeToXY(angle, e.Distance);

            this.Enemy = new RobotInfo(e.Energy, e.Velocity, enemyPosition.Item1, enemyPosition.Item2, e.Heading, e.Heading, e.Heading, 0, "Enemy");
        }

        /// <summary>
        /// Generates info about an enemy based on initial conditions.
        /// </summary>
        /// <returns>
        /// The <see cref="RobotInfo"/> of the enemy.
        /// </returns>
        private static RobotInfo GenerateAssumedEnemy()
        {
            const double Zero = 0.0;

            var x = Global.BF_WIDTH / 2.0;
            var y = Global.BF_HEIGHT / 2.0;

            return new RobotInfo(Global.STARTING_ROBOT_ENERGY, 0, x, y, Zero, Zero, Zero, Zero, "Enemy");
        }
    }
}