namespace MAIG_RoboCode_Project.GameObjects
{
    using System;

    /// <summary>
    /// Used as bullets in the Monte-Carlo Tree Search simulation for Robocode.
    /// </summary>
    public class Projectile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Projectile"/> class. 
        /// </summary>
        /// <param name="x">The x-position of the projectile.</param>
        /// <param name="y">The y-position of the projectile.</param>
        /// <param name="heading">The heading of the projectile.</param>
        /// <param name="power">The power of the projectile.</param>
        /// <param name="active">Whether the projectile is active.</param>
        /// <param name="owner">The owner of the projectile.</param>
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

        /// <summary>
        /// Gets the heading of the projectile.
        /// </summary>
        public double Heading { get; private set; }

        /// <summary>
        /// Gets whho fired the projectile.
        /// </summary>
        public string Owner { get; private set; }

        /// <summary>
        /// Gets the power of the projectile.
        /// </summary>
        public double Power { get; private set; }

        /// <summary>
        /// Gets the velocity of the projectile.
        /// </summary>
        public double Velocity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the projectile is active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the x-position of the projectile.
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// Gets the y-position of the projectile.
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// Gets the damage that the projectile will inflict.
        /// </summary>
        public double Damage
        {
            get
            {
                return (4 * this.Power) + (2 * Math.Max(this.Power - 1, 0));
            }
        }

        /// <summary>
        /// Gets the energy returned upon hitting an enemy.
        /// </summary>
        public double EnergyReturn
        {
            get
            {
                return 3 * this.Power;
            }
        }

        /// <summary>
        /// Gets the heat generated when firing this projectile.
        /// </summary>
        public double HeatGenerated
        {
            get
            {
                return 1 + (this.Power / 5);
            }
        }

        /// <summary>
        /// Gets the state of the projectile in the next tick of the game.
        /// </summary>
        /// <returns>
        /// The newly simulated <see cref="Projectile"/>.
        /// </returns>
        public Projectile NextTick()
        {
            if (!this.IsActive)
            {
                return new Projectile(this.X, this.Y, this.Heading, this.Power, false, this.Owner);
            }

            var newPos = this.NextPosition();
            var x = newPos.Item1;
            var y = newPos.Item2;

            if (x > Global.BfWidth || x < 0)
            {
                this.IsActive = false;
            }

            if (y > Global.BfHeight || y < 0)
            {
                this.IsActive = false;
            }

            return new Projectile(x, y, this.Heading, this.Power, this.IsActive, this.Owner);
        }

        /// <summary>
        /// Deactivates this projectile.
        /// </summary>
        public void Deactivate()
        {
            this.IsActive = false;
        }

        /// <summary>
        /// Calculates the velocity of a projectile based on its power.
        /// </summary>
        /// <param name="power">The power with which the projectile was shot.</param>
        /// <returns>The velocity of the projectile.</returns>
        private static double PowerToVelocity(double power)
        {
            return 20 - (3 * power);
        }

        /// <summary>
        /// Gets the position of the projectile in the next tick of the game.
        /// </summary>
        /// <returns>The next position of the projectile.</returns>
        private Tuple<double, double> NextPosition()
        {
            var displacedPosition = Global.DegreeToXY(this.Heading, this.Velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            newX = newX < 0 ? 0 : newX;
            newX = newX > Global.BfWidth ? Global.BfWidth : newX;
            newY = newY < 0 ? 0 : newY;
            newY = newY > Global.BfHeight ? Global.BfHeight : newY;

            return new Tuple<double, double>(newX, newY);
        }
    }
}
