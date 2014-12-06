namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using MAIG_RoboCode_Project.GameObjects;
    using MAIG_RoboCode_Project.MonteCarlo;

    using Robocode;

    /// <summary>
    /// Robocode robot based on Monte-Carlo Tree Search.
    /// </summary>
    public class MCTSRobot : Robot
    {
        /// <summary>
        /// Controls whether the robot will do another iteration of its while-loop.
        /// </summary>
        private bool keepRunning = true;

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

        /// <summary>
        /// Gets or sets the counter to check if the robot should scan.
        /// </summary>
        private int ScanCounter { get; set; }

        /// <summary>
        /// The run method of the robot.
        /// </summary>
        public override void Run()
        {
            this.ScanCounter = Global.ScanCounter;
            Global.BfWidth = this.BattleFieldWidth;
            Global.BfHeight = this.BattleFieldHeight;

            this.ShouldScan = true;
            this.RadarColor = Color.Blue;
            this.ScanColor = Color.Yellow;

            var mcts = new MCTS();
            this.Projectiles = new List<Projectile>();
            this.Enemy = GenerateAssumedEnemy();

            while (this.keepRunning)
            {
                while (this.ShouldScan)
                {
                    this.TurnGunRight(20);
                }

                var gs = new Gamestate(new RobotInfo(this), this.Enemy, this.Projectiles, null);

                var tn = mcts.Search(gs);

                var i = tn.Gamestate.Instructions;

                if (Math.Abs(i.RobotDegrees) >= Global.Tolerance)
                {
                    this.TurnRight(i.RobotDegrees);

                    // Console.WriteLine("robot degrees " + i.RobotDegrees);
                }

                if (Math.Abs(i.GunDegrees) >= Global.Tolerance)
                {
                    this.TurnGunRight(i.GunDegrees);

                    // Console.WriteLine("gun degrees " + i.GunDegrees);
                }

                if (Math.Abs(i.RadarDegrees) >= Global.Tolerance)
                {
                    this.TurnRadarRight(i.RadarDegrees);

                    // Console.WriteLine("radar degress " + i.RadarDegrees);
                }

                if (Math.Abs(i.MoveDistance) >= Global.Tolerance)
                {
                    this.Ahead(i.MoveDistance);

                    // Console.WriteLine("move distance " + i.MoveDistance);
                }
                
                if (Math.Abs(i.FirePower) > 0.0 && this.GunHeat <= 0)
                {
                    this.Fire(i.FirePower);
                    this.Projectiles.Add(new Projectile(this.X, this.Y, this.GunHeading, i.FirePower, true, this.Name));

                    // Console.WriteLine("fire power" + i.FirePower);
                }

                this.Projectiles = this.Projectiles.Select(p => p.NextTick()).ToList();
                this.ScanCounter--;
            }
        }

        /// <summary> The code run by the robot whenever it scans another robot. </summary>
        /// <param name="e"> The scanned robot event. </param>
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            this.ShouldScan = false;
            this.ScanCounter = Global.ScanCounter;
            var angle = ((this.Heading + e.Bearing) + 360) % 360;

            var enemyPosition = Global.DegreeToXY(angle, e.Distance);

            this.Enemy = new RobotInfo(e.Energy, e.Velocity, enemyPosition.Item1, enemyPosition.Item2, e.Heading, e.Heading, e.Heading, 0, "Enemy");
        }

        /// <summary>
        /// The code run by the robot when the battle ends.
        /// </summary>
        /// <param name="evnt"> The event. </param>
        public override void OnBattleEnded(BattleEndedEvent evnt)
        {
            this.keepRunning = false;
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

            var x = Global.BfWidth / 2.0;
            var y = Global.BfHeight / 2.0;

            return new RobotInfo(Global.StartingRobotEnergy, 0, x, y, Zero, Zero, Zero, Zero, "Enemy");
        }
    }
}