namespace MAIG_RoboCode_Project
{
    using System;

    class Projectile
    {
        public double Heading { get; private set; }

        public string Name { get; private set; }

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

        public Projectile(double x, double y, double heading, double power, double velocity, bool active)
        {
            this.X = x;
            this.Y = y;
            this.Heading = heading;
            this.Power = power;
            this.Velocity = velocity;
            this.IsActive = active;
        }

        public Projectile nextTick()
        {
            var newPos = this.nextPosition();
            var x = newPos.Item1;
            var y = newPos.Item2;

            return new Projectile(x, y, this.Heading, this.Power, this.Velocity, this.IsActive);
        }

        private Tuple<double, double> nextPosition()
        {
            var displacedPosition = this.DegreeToXY(this.Heading, this.Velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            return new Tuple<double, double>(newX, newY);
        }

        private Tuple<double, double> DegreeToXY(double degrees, double radius)
        {
            // Change from compass degrees (Robocode) to x/y degrees
            // If degrees less than 90, add 360 to make degrees not negative
            var modifiedDegrees = degrees < 90 ? degrees + 360 : degrees;
            modifiedDegrees = (modifiedDegrees - 90) % 360;

            var radians = modifiedDegrees * Math.PI / 180.0;

            var x = Math.Cos(radians) * radius;
            var y = Math.Sin(-radians) * radius;

            x = Math.Abs(x) < 1.0E-15 ? 0 : x;
            y = Math.Abs(y) < 1.0E-15 ? 0 : y;

            return new Tuple<double, double>(x, y);
        }
    }
}
