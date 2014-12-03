namespace MAIG_RoboCode_Project
{
    using System;

    public static class Global
    {
        #region Score Values

        public const double PLAYER_SCORE_WEIGHT = 1.0;

        public const double ENEMY_SCORE_WEIGHT = 0.5;

        public const double SCORE_PER_BULLET_DAMAGE = 1;

        public const double SCORE_SURVIVAL_BONUS = 50;

        public const double SCORE_MOVEMENT = 1;

        public const double SCORE_GUN_DIRECTION = 0.1;

        public const double SCORE_SHOOT = 0.3;

        #endregion

        #region Simulation Values & Intervals

        public const double STARTING_ROBOT_ENERGY = 100;

        public const double MAX_GUN_ROTATION = 20;

        public const double GUN_TURN_INTERVAL = 10;

        public const double ROBOT_TURN_INTERVAL = 20;

        public const double SIMULATION_MAX_ROBOT_ROTATION = 40;

        public const double RADAR_TURN_INTERVAL = 20;

        public const double SIMULATION_MAX_RADAR_ROTATION = 40;

        public const double MAX_RADAR_ROTATION = 45;

        #endregion

        #region Robocode Values

        public const double COOLING_RATE = 0.1;

        public static double BF_WIDTH = 100;

        public static double BF_HEIGHT = 100;

        #endregion

        #region Privates

        private static Random rand;

        #endregion

        #region MCTS Constants

        public const double MCTS_VISIT_THRESHOLD = 2;

        public const double MCTS_EXPLORATION_CONSTANT = 10;

        public const double MCTS_MAX_PATH_TO_ROOT = 10;

        public const double MCTS_ALLOWED_SEARCH_TIME = 1000;

        public const double MCTS_MAX_ITERATIONS = 500;

        #endregion

        public const double TOLERANCE = 1.0E-15;

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

            x = Math.Abs(x) < TOLERANCE ? 0 : x;
            y = Math.Abs(y) < TOLERANCE ? 0 : y;

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
