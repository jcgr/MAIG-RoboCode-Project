namespace MAIG_RoboCode_Project
{
    using System;

    public class Projectile
    {
        public double Heading { get; private set; }

        public string Name { get; private set; }

        public string Owner { get; private set; }

        public double Power { get; private set; }

        public double Velocity { get; private set; }

        public bool IsActive { get; private set; }

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

        public Projectile(double x, double y, double heading, double power, bool active, string owner)
        {
            this.Owner = owner;
            this.X = x;
            this.Y = y;
            this.Heading = heading;
            this.Power = power;
            this.Velocity = PowerToVelocity(this.Power);
            this.IsActive = active;
        }

        public Projectile NextTick()
        {
            if (!IsActive)
            {
                return null;
            }

            var newPos = this.NextPosition();
            var x = newPos.Item1;
            var y = newPos.Item2;

            if (x > Global.BF_WIDTH || x < 0)
            {
                this.IsActive = false;
            }

            if (y > Global.BF_HEIGHT || y < 0)
            {
                this.IsActive = false;
            }

            return new Projectile(x, y, this.Heading, this.Power, this.IsActive, this.Owner);
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }

        private Tuple<double, double> NextPosition()
        {
            var displacedPosition = Global.DegreeToXY(this.Heading, this.Velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            newX = newX < 0 ? 0 : newX;
            newX = newX > Global.BF_WIDTH ? Global.BF_WIDTH : newX;
            newY = newY < 0 ? 0 : newY;
            newY = newY > Global.BF_HEIGHT ? Global.BF_HEIGHT : newY;

            return new Tuple<double, double>(newX, newY);
        }

        private static double PowerToVelocity(double power)
        {
            return 20 - (3 * power);
        }
    }
}
