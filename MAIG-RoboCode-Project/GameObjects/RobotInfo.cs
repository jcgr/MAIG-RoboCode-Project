namespace MAIG_RoboCode_Project
{
    using System;

    class RobotInfo
    {
        public double Energy { get; private set; }

        public double Velocity { get; private set; }

        public Tuple<double, double> Position { get; private set; }

        public double RadarHeading { get; private set; }

        public double GunHeat { get; private set; }

        public double RobotHeading { get; private set; }

        public double GunHeading { get; private set; }
    }
}
