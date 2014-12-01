namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RobotInstructions
    {

        public double MoveDistance { get; private set; }

        public double RobotDegrees { get; private set; }

        public double GunDegrees { get; private set; }

        public double RadarDegrees { get; private set; }

        public double FirePower { get; private set; }

        public RobotInstructions(double moveDistance, double robotDegrees, double gunDegrees, double radarDegrees, double firePower)
        {
            this.MoveDistance = moveDistance;
            this.RobotDegrees = robotDegrees;
            this.GunDegrees = gunDegrees;
            this.RadarDegrees = radarDegrees;
            this.FirePower = firePower;
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
                distance = robot.Velocity;
            }

            var robotDegrees = robot.RobotHeading;

            var playerRobotInSight = false;
            var gunDegrees = 0.0;
            var enemyNextPos = gs.OurRobot.NextPosition(gs.OurRobot.RobotHeading, gs.OurRobot.Velocity);
            
            var gunAngleToEnemy = Global.XYToDegree(enemyNextPos.Item1, enemyNextPos.Item2, robot.X, robot.Y);
            var gunAngleDifference = gunAngleToEnemy - robot.GunHeading;
            var gunAngleDifferenceAbs = Math.Abs(gunAngleDifference);
            if (gunAngleDifferenceAbs < 5)
            {
                gunDegrees = 0;
                playerRobotInSight = true;
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

            var firePower = (playerRobotInSight && gs.EnemyRobot.CanFire) ? 1.0 : 0.0;

            return new RobotInstructions(distance, robotDegrees, gunDegrees, radarDegrees, firePower);
        }

    }
}
