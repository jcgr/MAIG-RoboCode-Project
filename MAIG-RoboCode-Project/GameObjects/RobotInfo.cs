namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Robocode;

    public enum RotbotStatus
    {
        Healthy = 0, 
        
        Disabled = 1, 
        
        Destroyed = 2
    }

    public class RobotInfo
    {
        public double Energy { get; private set; }

        public double Velocity { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double RadarHeading { get; private set; }

        public double RobotHeading { get; private set; }

        public double GunHeading { get; private set; }

        public double GunHeat { get; private set; }

        public RobotStatus Status{ get; private set; }

        public bool CanFire
        {
            get
            {
                return !(GunHeat > 0.0);
            }
        }

        public RobotInfo(double energy, double velocity, double x, double y, double heading, double gunHeading, double radarHeading, double gunHeat, RobotStatus status)
        {
            this.Energy = energy;
            this.Velocity = velocity;
            this.X = x;
            this.Y = y;
            this.RobotHeading = heading;
            this.GunHeading = gunHeading;
            this.RadarHeading = radarHeading;
            this.GunHeat = gunHeat;
            this.Status = status;
        }

        public RobotInfo NextTick(RobotInstructions ri,  List<Projectile> projectiles)
        {
            var gunHeat = this.HeatAfterCooling(Global.COOLING_RATE);
            var projectileFired = ri.FirePower > 0 ? this.Shoot(ri.FirePower, ref gunHeat) : null;

            var energy = this.NewEnergy(projectiles);

            var robotHeading = this.NewHeading(ri.RobotDegrees);
            var gunHeading =  this.NewGunHeading(ri.GunDegrees);
            var radarHeading =  this.NewRadarHeading(ri.RadarDegrees);

            var velocity = 0;
            var x = 0;
            var y = 0;
            
            return new RobotInfo(energy, velocity, x, y, robotHeading, gunHeading, radarHeading, gunHeat);
        }

        private Projectile Shoot(double power, ref double gunheat)
        {
            if (this.CanFire)
            {
                var projectileShot = new Projectile(this.X, this.Y, this.GunHeading, power, true);
                gunheat = gunheat + projectileShot.HeatGenerated;
                return projectileShot;
            }

            return null;
        }

        private double NewEnergy(List<Projectile> projectiles)
        {
            var energyLoss = 0.0;

            foreach (var p in projectiles.Where(p => (Math.Abs(p.X - this.X) < Global.TOLERANCE) && (Math.Abs(p.Y - this.Y) < Global.TOLERANCE)))
            {
                energyLoss += p.Damage;
            }

            this.Energy += energyLoss;
        }

        private void GainEnergy(double energyGained)
        {
            
        }

        private double HeatAfterCooling(double reduction)
        {
            return Math.Max(this.GunHeat - reduction, 0);
        }

        private double NewHeading(double change)
        {
            var maxTurnRate = 10 - (0.75 * Math.Abs(this.Velocity));
            return Math.Abs(change) > maxTurnRate ? (change > 0 ? maxTurnRate : -maxTurnRate) : change;
        }

        private double NewGunHeading(double change)
        {
            return Math.Abs(change) > Global.MAX_GUN_ROTATION ? (change > 0 ? Global.MAX_GUN_ROTATION : -Global.MAX_GUN_ROTATION) : change;
        }

        private double NewRadarHeading(double change)
        {
            return Math.Abs(change) > Global.MAX_RADAR_ROTATION ? (change > 0 ? Global.MAX_RADAR_ROTATION : -Global.MAX_RADAR_ROTATION) : change;
        }

        private Tuple<double, double> NextPosition()
        {
            var displacedPosition = Global.DegreeToXY(this.RobotHeading, this.Velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            return new Tuple<double, double>(newX, newY);
        }
    }
}
