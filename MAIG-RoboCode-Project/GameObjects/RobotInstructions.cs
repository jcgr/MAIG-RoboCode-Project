namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RobotInstructions
    {
        private static List<RobotInstructions> listOfInstructions;

        public static List<RobotInstructions> ListOfInstructions
        {
            get
            {
                if (listOfInstructions != null)
                {
                    return listOfInstructions;
                }

                var list = new List<RobotInstructions>();
                for (var vc = -2; vc < 2; vc++)
                {
                    list.Add(new RobotInstructions(0, 0, 0, 0, 0, vc)); // Don't shoot
                    list.Add(new RobotInstructions(0, 0, 0, 0, 1, vc)); // Shoot
                }/*

                for (var rd = -Global.SIMULATION_MAX_ROBOT_ROTATION;
                     rd < Global.SIMULATION_MAX_ROBOT_ROTATION;
                     rd += Global.ROBOT_TURN_INTERVAL)
                {
                    list.Add(new RobotInstructions(0, rd, 0, 0, 0)); // Don't shoot
                    list.Add(new RobotInstructions(0, rd, 0, 0, 1)); // Shoot
                }
                for (var gd = -Global.MAX_GUN_ROTATION; gd < Global.MAX_GUN_ROTATION; gd += Global.GUN_TURN_INTERVAL)
                {
                    list.Add(new RobotInstructions(0, 0, gd, 0, 0)); // Don't shoot
                    list.Add(new RobotInstructions(0, 0, gd, 0, 1)); // Shoot
                }
                for (var radard = -Global.SIMULATION_MAX_RADAR_ROTATION; radard < Global.SIMULATION_MAX_RADAR_ROTATION; radard += Global.RADAR_TURN_INTERVAL)
                {
                    list.Add(new RobotInstructions(0, 0, 0, radard, 0)); // Don't shoot
                    list.Add(new RobotInstructions(0, 0, 0, radard, 1)); // Shoot
                }*/

                listOfInstructions = list;
                
                Console.WriteLine("Actions list size: " + listOfInstructions.Count);
                return listOfInstructions;

                /*
                if (listOfInstructions != null)
                {
                    return listOfInstructions;
                }

                var list = new List<RobotInstructions>();
                for (var vc = -2; vc < 2; vc++)
                {
                    for (var rd = -Global.SIMULATION_MAX_ROBOT_ROTATION; rd < Global.SIMULATION_MAX_ROBOT_ROTATION; rd += Global.ROBOT_TURN_INTERVAL)
                    {
                        for (var gd = -Global.MAX_GUN_ROTATION; gd < Global.MAX_GUN_ROTATION; gd += Global.GUN_TURN_INTERVAL)
                        {
                            for (var radard = -Global.SIMULATION_MAX_RADAR_ROTATION; radard < Global.SIMULATION_MAX_RADAR_ROTATION; radard += Global.RADAR_TURN_INTERVAL)
                            {
                                list.Add(new RobotInstructions(0, rd, gd, radard, 0, vc)); // Don't shoot
                                list.Add(new RobotInstructions(0, rd, gd, radard, 1, vc)); // Shoot
                            }
                        }
                    }
                }

                listOfInstructions = list;
                return listOfInstructions;
                 * */
            }

            set
            {
                listOfInstructions = value;
            }
        }
        public double MoveDistance { get; set; }

        public double RobotDegrees { get; private set; }

        public double GunDegrees { get; private set; }

        public double RadarDegrees { get; private set; }

        public double FirePower { get; private set; }

        public double VelocityChange { get; private set; }

        public RobotInstructions(double moveDistance, double robotDegrees, double gunDegrees, double radarDegrees, double firePower, double velocityChange = 0)
        {
            this.MoveDistance = moveDistance;
            this.RobotDegrees = robotDegrees;
            this.GunDegrees = gunDegrees;
            this.RadarDegrees = radarDegrees;
            this.FirePower = firePower;
            this.VelocityChange = velocityChange;
        }

        public static List<RobotInstructions> GetListOfInstructions(double velocity, bool canShoot)
        {
            return ListOfInstructions.Where(ri => IsInstructionPossible(velocity, canShoot, ri)).ToList();
        }

        public static bool IsInstructionPossible(double velocity, bool canShoot, RobotInstructions ri)
        {
            //return true; //TODO: BAD HACK!

            if ((velocity < 0 && ri.VelocityChange == -2.0)
                || (velocity > 0 && ri.VelocityChange == 2.0))
            {
                return false;
            }

            if ((velocity == 8.0 && ri.VelocityChange > 0.0)
                || (velocity == -8.0 && ri.VelocityChange < 0.0))
            {
                return false;
            }

            if (ri.FirePower > 0.0 && !canShoot)
            {
                return false;
            }

            var maxDegree = 10 - (0.75 * velocity);

            return !(Math.Abs(ri.RobotDegrees) > maxDegree);
        }

        public static RobotInstructions GetEnemyInstructions(Gamestate gs, List<Projectile> projectiles)
        {
            var robot = gs.EnemyRobot;
            var distance = robot.Velocity;

            var nextPos = robot.NextPosition(robot.RobotHeading, robot.Velocity);
            if (
                projectiles.Any(
                    p =>
                    (Math.Abs(p.X - nextPos.Item1) < Global.TOLERANCE)
                    && (Math.Abs(p.Y - nextPos.Item2) < Global.TOLERANCE)))
            {

                if (robot.Velocity < 0.0)
                {
                    var accelerate = robot.NextPosition(robot.RobotHeading, Math.Max(robot.Velocity - 1, -8.0));
                    var decelerate = robot.NextPosition(robot.RobotHeading, Math.Min(robot.Velocity + 2, 8.0));
                    if (
                        projectiles.Any(
                            p =>
                            (Math.Abs(p.X - accelerate.Item1) < Global.TOLERANCE)
                            && (Math.Abs(p.Y - accelerate.Item2) < Global.TOLERANCE)))
                    {
                        distance = Math.Max(robot.Velocity - 1, -8.0);
                    }
                    else if (projectiles.Any(
                            p =>
                            (Math.Abs(p.X - decelerate.Item1) < Global.TOLERANCE)
                            && (Math.Abs(p.Y - decelerate.Item2) < Global.TOLERANCE)))
                    {
                        {
                            distance = Math.Min(robot.Velocity + 2, 8.0);
                        }
                    }
                }
                else
                {
                    var accelerate = robot.NextPosition(robot.RobotHeading, Math.Min(robot.Velocity + 1, 8.0));
                    var decelerate = robot.NextPosition(robot.RobotHeading, Math.Max(robot.Velocity - 2, -8.0));
                    if (
                        projectiles.Any(
                            p =>
                            (Math.Abs(p.X - accelerate.Item1) < Global.TOLERANCE)
                            && (Math.Abs(p.Y - accelerate.Item2) < Global.TOLERANCE)))
                    {
                        distance = Math.Min(robot.Velocity + 1, 8.0);
                    }
                    else if (projectiles.Any(
                            p =>
                            (Math.Abs(p.X - decelerate.Item1) < Global.TOLERANCE)
                            && (Math.Abs(p.Y - decelerate.Item2) < Global.TOLERANCE)))
                    {
                        {
                            distance = Math.Max(robot.Velocity - 2, -8.0);
                        }
                    }
                }
            }
            else
            {
                distance = 0; //TODO: Bad hack? Was robot.Velocity before
            }

            var robotDegrees = 0; // TODO: Bad hack? Was robot.RobotHeading before

            var gunDegrees = 0.0;
            var enemyNextPos = gs.OurRobot.NextPosition(gs.OurRobot.RobotHeading, gs.OurRobot.Velocity);

            var gunAngleToEnemy = Global.XYToDegree(enemyNextPos.Item1, enemyNextPos.Item2, robot.X, robot.Y);
            
            var angle1 = gunAngleToEnemy - robot.GunHeading;
            var angle2 = robot.GunHeading - gunAngleToEnemy;

            var gunAngleDifference = angle1 < angle2 ? angle1 : angle2;
            var gunAngleDifferenceAbs = Math.Abs(gunAngleDifference);
            if (gunAngleDifferenceAbs < 5)
            {
                gunDegrees = 0;
            }
            else if (gunAngleDifferenceAbs < 20)
            {
                gunDegrees = gunAngleDifference;
            }
            else
            {
                gunDegrees = gunAngleDifference < 0.0 ? -Global.MAX_GUN_ROTATION : Global.MAX_GUN_ROTATION;
            }

            var radarDegrees = 0.0;
            var radarAngleToEnemy = Global.XYToDegree(enemyNextPos.Item1, enemyNextPos.Item2, robot.X, robot.Y);
            var radarAngleDifference = radarAngleToEnemy - robot.RadarHeading;
            var radarAngleDifferenceAbs = Math.Abs(radarAngleDifference);
            if (radarAngleDifferenceAbs < 5)
            {
                radarDegrees = 0;
            }
            else if (radarAngleDifferenceAbs < 20)
            {
                radarDegrees = radarAngleDifference;
            }
            else
            {
                radarDegrees = radarAngleDifference < 0.0 ? -Global.MAX_RADAR_ROTATION : Global.MAX_RADAR_ROTATION;
            }

            var firePower = (Math.Abs(gunDegrees) < Global.TOLERANCE && gs.EnemyRobot.CanFire) ? 1.0 : 0.0;

            return new RobotInstructions(distance, robotDegrees, gunDegrees, radarDegrees, firePower);
        }

    }
}
