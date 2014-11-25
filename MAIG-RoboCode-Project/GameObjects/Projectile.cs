namespace MAIG_RoboCode_Project
{
    using System;

    class Projectile
    {
        public double Heading { get; private set; }

        public string Name { get; private set; }

        public RobotInfo Owner { get; private set; }

        public double Power { get; private set; }

        public double Velocity { get; private set; }

        public bool IsActive { get; set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Damage
        {
            get
            {
                return (4 * this.Power) + (2 * Math.Max(this.Power - 1, 0));
            }
        }

        public double EnergyReturn
        {
            get
            {
                return 3 * this.Power;
            }
        }

        public double HeatGenerated
        {
            get
            {
                return 1 + (this.Power / 5);
            }
        }

        public Projectile(double x, double y, double heading, double power, bool active)
        {
            this.X = x;
            this.Y = y;
            this.Heading = heading;
            this.Power = power;
            this.Velocity = PowerToVelocity(this.Power);
            this.IsActive = active;
        }

        public Projectile NextTick()
        {
            var newPos = this.NextPosition();
            var x = newPos.Item1;
            var y = newPos.Item2;

            return new Projectile(x, y, this.Heading, this.Power, this.IsActive);
        }

        private Tuple<double, double> NextPosition()
        {
            var displacedPosition = Global.DegreeToXY(this.Heading, this.Velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            return new Tuple<double, double>(newX, newY);
        }

        private static double PowerToVelocity(double power)
        {
            return 20 - (3 * power);
        }
    }
}
