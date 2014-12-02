namespace MAIG_RoboCode_Project
{
    using System;

    public static class Global
    {
        private static Random rand;

        public static readonly double TOLERANCE = 1.0E-15;

        public static double COOLING_RATE = 0.1;

        public static readonly double MAX_ROBOT_ENERGY = 100; // TODO: Not sure if this is right; should look into it

        public static readonly double MAX_GUN_ROTATION = 20;

        public static readonly double MAX_RADAR_ROTATION = 45;

        public static double BF_WIDTH = 100;

        public static double BF_HEIGHT = 100;

        public static readonly double SCORE_PER_BULLET_DAMAGE = 1;

        public static readonly double SCORE_SURVIVAL_BONUS = 50;

        public static readonly double PLAYER_SCORE_WEIGHT = 1.0;
        
        public static readonly double ENEMY_SCORE_WEIGHT = 0.5;

        public static readonly double MCTS_VISIT_THRESHOLD = 3;

        public static readonly double MCTS_EXPLORATION_CONSTANT = 10;

        public static readonly double MCTS_MAX_PATH_TO_ROOT = 20;

        public static Random Random
        {
            get
            {
                return rand ?? (rand = new Random());
            }
        }

        /// <summary>
        /// 
        /// Inspired by http://www.vcskicks.com/code-snippet/degree-to-xy.php
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
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
        /// 
        /// Inspired by http://www.vcskicks.com/code-snippet/degree-to-xy.php
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="originX"></param>
        /// <param name="originY"></param>
        /// <returns></returns>
        public static double XYToDegree(double x, double y, double originX, double originY)
        {
            var deltaX = originX - x;
            var deltaY = originY - y;

            var radAngle = Math.Atan2(deltaY, deltaX);
            var degreeAngle = radAngle * 180.0 / Math.PI;

            var angle = 180.0 - degreeAngle;
            return (angle + 90) % 360;
        }
    }
}
