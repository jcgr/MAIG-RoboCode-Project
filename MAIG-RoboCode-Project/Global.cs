namespace MAIG_RoboCode_Project
{
    using System;

    /// <summary>
    /// The globally accessed elements of the MCTS-Robot.
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// The difference tolerated between two "equal" decimal values.
        /// </summary>
        public const double Tolerance = 1.0E-15;

        /// <summary>
        /// How many searches should be done before the robot scans for the enemy again.
        /// </summary>
        public const int ScanCounter = 30;

        #region Score Values

        /// <summary>
        /// The weight to give the score of the player when calculating score.
        /// </summary>
        public const double PlayerScoreWeight = 1.0;

        /// <summary>
        /// The weight to give the score for the enemy player when calculating score.
        /// </summary>
        public const double EnemyScoreWeight = 0.5;

        /// <summary>
        /// The amount of points granted for each point of damage dealt by a bullet.
        /// </summary>
        public const double ScorePerBulletDamage = 1.0;

        /// <summary>
        /// The amount of points granted for surviving a round.
        /// </summary>
        public const double ScoreSurvivalBonus = 50.0;

        /// <summary>
        /// The amount of game points granted for each scored movement point.
        /// </summary>
        public const double ScoreMovement = 1.5;

        /// <summary>
        /// The amount of points given for changing the heading of the robot.
        /// </summary>
        public const double ScoreRobotHeading = 0.4;

        /// <summary>
        /// The amount of points granted for having shot a bullet.
        /// </summary>
        public const double ScoreShoot = 0.1;

        /// <summary>
        /// The amount of points granted for each point of energy the robot has.
        /// </summary>
        public const double ScoreEnergy = 0.05;

        #endregion

        #region Simulation Values & Intervals

        /// <summary>
        /// The amount of energy a robot starts with.
        /// </summary>
        public const double StartingRobotEnergy = 100;

        /// <summary>
        /// The maximum amount a robot can turn its gun in a turn.
        /// </summary>
        public const double MaxGunRotation = 20;

        /// <summary>
        /// The maximum amount a robot can turn its radar in a turn.
        /// </summary>
        public const double MaxRadarRotation = 45;

        /// <summary>
        /// The maximum turning rate for the robot when generating new instructions in the simulation.
        /// </summary>
        public const double SimulationMaxRobotRotation = 10;

        /// <summary>
        /// The maximum turning rate for the radar when generating new instructions in the simulation.
        /// </summary>
        public const double SimulationMaxRadarRotation = 40;

        /// <summary>
        /// The size of the interval between different instructions for turning the robot.
        /// </summary>
        public const double RobotTurnInterval = 2;

        /// <summary>
        /// The size of the interval between different instructions for turning the gun.
        /// </summary>
        public const double GunTurnInterval = 5;

        /// <summary>
        /// The size of the interval between different instructions for turning the radar.
        /// </summary>
        public const double RadarTurnInterval = 20;

        #endregion

        #region MCTS Constants

        /// <summary>
        /// The visit threshold for children in the Monte-Carlo Tree Search algorithm.
        /// </summary>
        public const double MCTSVisitThreshold = 3;

        /// <summary>
        /// The exploration constant in the Monte-Carlo Tree Search algorithm.
        /// </summary>
        public const double MCTSExplorationConstant = 1;

        /// <summary>
        /// The maximum path to the root in the tree of the Monte-Carlo Tree Search algorithm.
        /// </summary>
        public const double MCTSMaxPathToRoot = 25;

        /// <summary>
        /// The maximum allowed search time in the Monte-Carlo Tree Search algorithm.
        /// </summary>
        public const double MCTSAllowedSearchTime = 10;

        /// <summary>
        /// The maximum number of iterations allowed in the Monte-Carlo Tree Search algorithm.
        /// </summary>
        public const double MCTSMaxIterations = 500;

        #endregion

        #region Robocode Values

        /// <summary>
        /// The cooling rate of the guns.
        /// </summary>
        public const double CoolingRate = 0.1;

        /// <summary>
        /// Gets or sets the width of the battlefield.
        /// </summary>
        public static double BfWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the battlefield.
        /// </summary>
        public static double BfHeight { get; set; }

        #endregion

        #region Privates

        /// <summary>
        /// The random object used in the Monte-Carlo Tree Search.
        /// </summary>
        private static Random rand;

        #endregion

        /// <summary>
        /// Gets the random object used globally by the algorithm.
        /// </summary>
        public static Random Random
        {
            get
            {
                return rand ?? (rand = new Random());
            }
        }

        #region Math Methods

        /// <summary>
        /// Calculates a point based on input degrees and radius.
        /// Inspired by http://www.vcskicks.com/code-snippet/degree-to-xy.php
        /// </summary>
        /// <param name="degrees">The degree from the origin.</param>
        /// <param name="radius">The distance from the origin.</param>
        /// <returns>The calculated point.</returns>
        public static Tuple<double, double> DegreeToXY(double degrees, double radius)
        {
            // Change from compass degrees (Robocode) to x/y degrees
            // If degrees less than 90, add 360 to make degrees not negative
            var modifiedDegrees = degrees < 90 ? degrees + 360 : degrees;
            modifiedDegrees = (modifiedDegrees - 90) % 360;

            var radians = modifiedDegrees * Math.PI / 180.0;

            var x = Math.Cos(radians) * radius;
            var y = Math.Sin(-radians) * radius;

            x = Math.Abs(x) < Tolerance ? 0 : x;
            y = Math.Abs(y) < Tolerance ? 0 : y;

            return new Tuple<double, double>(x, y);
        }

        /// <summary>
        /// Calculates a degree based on two points.
        /// Inspired by http://www.vcskicks.com/code-snippet/degree-to-xy.php
        /// </summary>
        /// <param name="x">x-value of target.</param>
        /// <param name="y">y-value of target.</param>
        /// <param name="originX">x-value of origin.</param>
        /// <param name="originY">y-value of origin.</param>
        /// <returns>The angle between the two points.</returns>
        public static double XYToDegree(double x, double y, double originX, double originY)
        {
            var deltaX = originX - x;
            var deltaY = originY - y;

            var radAngle = Math.Atan2(deltaY, deltaX);
            var degreeAngle = radAngle * 180.0 / Math.PI;

            var angle = 180.0 - degreeAngle;
            return (angle + 90) % 360;
        }

        #endregion
    }
}
