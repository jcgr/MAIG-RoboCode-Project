namespace MAIG_RoboCode_Project.GameObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Instructions used for robots in the Monte-Carlo Tree Search algorithm.
    /// </summary>
    public class RobotInstructions
    {
        /// <summary>
        /// The list of possible instructions.
        /// </summary>
        private static List<RobotInstructions> listOfInstructions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RobotInstructions"/> class.
        /// </summary>
        /// <param name="moveDistance"> The distance to move. </param>
        /// <param name="robotDegrees"> The amount of degrees to turn the robot. </param>
        /// <param name="gunDegrees"> The amount of degrees to turn the gun. </param>
        /// <param name="radarDegrees"> The amount of degrees to turn the radar. </param>
        /// <param name="firePower"> The amount of power to shoot with (0.0 indicates no firing). </param>
        /// <param name="velocityChange"> The amount to change the velocity. </param>
        public RobotInstructions(double moveDistance, double robotDegrees, double gunDegrees, double radarDegrees, double firePower, double velocityChange = 0)
        {
            this.MoveDistance = moveDistance;
            this.RobotDegrees = robotDegrees;
            this.GunDegrees = gunDegrees;
            this.RadarDegrees = radarDegrees;
            this.FirePower = firePower;
            this.VelocityChange = velocityChange;
        }

        /// <summary>
        /// Gets the list of possible instructions.
        /// </summary>
        public static List<RobotInstructions> ListOfInstructions
        {
            get
            {
                // If already generated, do not do so again.
                if (listOfInstructions != null)
                {
                    return listOfInstructions;
                }

                var list = new List<RobotInstructions>();

                // Generate velocity change instructions
                for (var vc = -2; vc < 2; vc++)
                {
                    list.Add(new RobotInstructions(0, 0, 0, 0, 0, vc)); // Don't shoot
                    list.Add(new RobotInstructions(0, 0, 0, 0, 1, vc)); // Shoot
                }

                // Generate robot rotation instructions
                for (var rd = -Global.SIMULATION_MAX_ROBOT_ROTATION;
                     rd < Global.SIMULATION_MAX_ROBOT_ROTATION;
                     rd += Global.ROBOT_TURN_INTERVAL)
                {
                    list.Add(new RobotInstructions(0, rd, 0, 0, 0)); // Don't shoot
                    list.Add(new RobotInstructions(0, rd, 0, 0, 1)); // Shoot
                }

                // Generate gun rotation instructions
                for (var gd = -Global.MAX_GUN_ROTATION; gd < Global.MAX_GUN_ROTATION; gd += Global.GUN_TURN_INTERVAL)
                {
                    list.Add(new RobotInstructions(0, 0, gd, 0, 0)); // Don't shoot
                    list.Add(new RobotInstructions(0, 0, gd, 0, 1)); // Shoot
                }

                // Generate radar rotation instructions
                for (var radard = -Global.SIMULATION_MAX_RADAR_ROTATION; radard < Global.SIMULATION_MAX_RADAR_ROTATION; radard += Global.RADAR_TURN_INTERVAL)
                {
                    list.Add(new RobotInstructions(0, 0, 0, radard, 0)); // Don't shoot
                    list.Add(new RobotInstructions(0, 0, 0, radard, 1)); // Shoot
                }

                listOfInstructions = list;
                return listOfInstructions;
            }
        }

        /// <summary>
        /// Gets or sets the distance to move.
        /// </summary>
        public double MoveDistance { get; set; }

        /// <summary>
        /// Gets the amount of degrees to rotate the robot.
        /// </summary>
        public double RobotDegrees { get; private set; }

        /// <summary>
        /// Gets the amount of degrees to rotate the gun.
        /// </summary>
        public double GunDegrees { get; private set; }

        /// <summary>
        /// Gets the amount of degrees to rotate the radar.
        /// </summary>
        public double RadarDegrees { get; private set; }

        /// <summary>
        /// Gets the amount of power to shoot with (0.0 power indicates no firing).
        /// </summary>
        public double FirePower { get; private set; }

        /// <summary>
        /// Gets the amount to change the velocity.
        /// </summary>
        public double VelocityChange { get; private set; }

        /// <summary>
        /// Gets the list of possible instructions based on current velocity and if the robot can shoot.
        /// </summary>
        /// <param name="velocity">The current velocity of the robot.</param>
        /// <param name="canShoot">Whether the robot can shoot or not.</param>
        /// <returns>The list of possible instructions.</returns>
        public static List<RobotInstructions> GetListOfInstructions(double velocity, bool canShoot)
        {
            return ListOfInstructions.Where(ri => IsInstructionPossible(velocity, canShoot, ri)).ToList();
        }

        /// <summary>
        /// Checks if an instruction can be performed based on the current velocity and if the robot can shoot.
        /// </summary>
        /// <param name="velocity">The current velocity of the robot.</param>
        /// <param name="canShoot">Whether the robot can shoot or not.</param>
        /// <param name="ri">The robot instructions to check.</param>
        /// <returns></returns>
        public static bool IsInstructionPossible(double velocity, bool canShoot, RobotInstructions ri)
        {
            // return true; //TODO: BAD HACK!
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
