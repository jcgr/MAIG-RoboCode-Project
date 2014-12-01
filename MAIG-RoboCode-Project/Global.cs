namespace MAIG_RoboCode_Project
{
    using System;

    public static class Global
    {
        public static readonly double TOLERANCE = 1.0E-15;

        public static double COOLING_RATE = 0.1;

        public static readonly double MAX_ROBOT_ENERGY = 100; // TODO: Not sure if this is right; should look into it

        public static readonly double MAX_GUN_ROTATION = 20;

        public static readonly double MAX_RADAR_ROTATION = 45;

        public static double BF_WIDTH = 100;

        public static double BF_HEIGHT = 100;

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
    }
}
