namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Drawing;

    using MAIG_RoboCode_Project.MonteCarlo;

    using Robocode;

    class MCTSRobot : Robot
    {
        /// <summary>
        /// Gets or sets the enemy robot.
        /// </summary>
        private RobotInfo Enemy { get; set; }

        private List<Projectile> Projectiles { get; set; }

        private bool ShouldScan { get; set; }

        // The main method of your robot containing robot logics
        public override void Run()
        {
            this.Ahead(1);
            ShouldScan = true;
            this.RadarColor = Color.Blue;
            this.ScanColor = Color.Yellow;
            var mcts = new MCTS();
            this.Projectiles = new List<Projectile>();
            Global.BF_WIDTH = this.BattleFieldWidth;
            Global.BF_HEIGHT = this.BattleFieldHeight;
            this.Enemy = new RobotInfo(Global.MAX_ROBOT_ENERGY, 0, Global.BF_WIDTH / 2.0, Global.BF_HEIGHT / 2.0, 0, 0, 0, 0, "Enemy");

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
    }
}