namespace MAIG_RoboCode_Project.GameObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Robocode;

    public enum RoboStatus
    {
        Healthy = 0,

        Disabled = 1,

        Destroyed = 2
    }

    public class RobotInfo
    {

        public string Name { get; private set; }

        public double Energy { get; private set; }

        public double Velocity { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double RadarHeading { get; private set; }

        public double RobotHeading { get; private set; }

        public double GunHeading { get; private set; }

        public double GunHeat { get; private set; }

        public Dictionary<string, double> ScoreList { get; private set; }

        public RoboStatus Status { get; private set; }

        public bool CanFire
        {
            get
            {
                return !(this.GunHeat > 0.0);
            }
        }

        public RobotInfo(Robot robot)
        {
            this.Energy = robot.Energy;
            this.Velocity = robot.Velocity;
            this.X = robot.X;
            this.Y = robot.Y;
            this.RobotHeading = robot.Heading;
            this.GunHeading = robot.GunHeading;
            this.RadarHeading = robot.RadarHeading;
            this.GunHeat = robot.GunHeat;
            this.Status = robot.Energy <= 0 ? RoboStatus.Destroyed : RoboStatus.Healthy;
            this.Name = robot.Name;
            this.ScoreList = CreateScoreList(0, 0, 0);
        }

        public RobotInfo(double energy, double velocity, double x, double y, double heading, double gunHeading, double radarHeading, double gunHeat, string name, RoboStatus status = RoboStatus.Healthy, Dictionary<string, double> scores = null)
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
            this.Name = name;
            this.ScoreList = scores ?? CreateScoreList(0, 0, 0);
        }

        public RobotInfo NextTick(RobotInstructions ri, List<Projectile> projectiles, RobotInfo enemyRobot)
        {
            var scores = CopyScoreList(this.ScoreList);

            var gunHeat = this.HeatAfterCooling(Global.COOLING_RATE);
            var projectileFired = ri.FirePower > 0 ? this.Shoot(ri.FirePower, ref gunHeat) : null;
            if (projectileFired != null)
            {
                projectiles.Add(projectileFired);
            }

            var damage = 0.0;
            var energy = this.NewEnergy(projectiles, enemyRobot, ref damage);
            scores["bullet"] += damage;
            var status = energy <= 0 ? RoboStatus.Destroyed : RoboStatus.Healthy;
            if (projectileFired != null)
            {
                energy -= projectileFired.Power;
            }

            status = energy <= 0 && status != RoboStatus.Destroyed ? RoboStatus.Disabled : RoboStatus.Healthy;

            var gunHeading = this.NewGunHeading(ri.GunDegrees) + ri.RobotDegrees;
            var radarHeading = this.NewRadarHeading(ri.RadarDegrees) + ri.RobotDegrees + ri.GunDegrees;
            var robotHeading = this.NewHeading(ri.RobotDegrees);

            var velocity = this.NewVelocity(ri);
            ri.MoveDistance = velocity;

            var newPos = this.NextPosition(robotHeading, velocity);

            var x = newPos.Item1;
            var y = newPos.Item2;

            return new RobotInfo(energy, velocity, x, y, robotHeading, gunHeading, radarHeading, gunHeat, this.Name, status, scores);
        }

        public static Dictionary<string, double> CreateScoreList(double survival = 0, double lastSurvivor = 0, double bullet = 0, double gunDirection = 0, double shootScore = 0)
        {
            var newList = new Dictionary<string, double>
                              {
                                  { "survival", survival },
                                  { "last", lastSurvivor },
                                  { "bullet", bullet },
                                  { "movementScore", bullet },
                                  {"gunDirection", gunDirection },
                                  {"shootScore", shootScore}
                              };

            return newList;
        }

        public static Dictionary<string, double> CopyScoreList(Dictionary<string, double> scoreList)
        {
            var newList = new Dictionary<string, double>
                              {
                                  { "survival", scoreList["survival"] },
                                  { "last", scoreList["last"] },
                                  { "bullet", scoreList["bullet"] },
                                  { "movementScore", scoreList["movementScore"] },
                                  { "shootScore", scoreList["shootScore"] },
                                  { "gunDirection", scoreList["gunDirection"] }
                              };

            return newList;
        }

        private Projectile Shoot(double power, ref double gunheat)
        {
            if (!this.CanFire)
            {
                return null;
            }

            var projectileShot = new Projectile(this.X, this.Y, this.GunHeading, power, true, this.Name);
            gunheat = gunheat + projectileShot.HeatGenerated;
            return projectileShot;
        }

        private double NewVelocity(RobotInstructions ri)
        {
            return this.Velocity + ri.VelocityChange;

        }

        private double NewEnergy(List<Projectile> projectiles, RobotInfo enemy, ref double damage)
        {
            damage = this.DamageDealt(projectiles, enemy);
            var energyChange = damage - this.DamageTaken(projectiles);
            return this.Energy + energyChange;
        }

        private double DamageTaken(IEnumerable<Projectile> projectiles)
        {
            var energyLoss = 0.0;
            foreach (var p in projectiles.Where(p => (Math.Abs(p.X - this.X) < Global.TOLERANCE) && (Math.Abs(p.Y - this.Y) < Global.TOLERANCE)))
            {
                energyLoss += p.Damage;
                p.Deactivate();
            }

            return energyLoss;

        }

        private double DamageDealt(IEnumerable<Projectile> projectiles, RobotInfo enemy)
        {
            var energyGained = 0.0;
            foreach (var p in projectiles.Where(p => (Math.Abs(p.X - enemy.X) < Global.TOLERANCE) && (Math.Abs(p.Y - enemy.Y) < Global.TOLERANCE) && p.Owner.Equals(this.Name)))
            {
                energyGained += p.Damage;

                p.Deactivate();
            }

            return energyGained;
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

        public Tuple<double, double> NextPosition(double heading, double velocity)
        {
            var displacedPosition = Global.DegreeToXY(heading, velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            newX = newX < 0 ? 0 : newX;
            newX = newX > Global.BF_WIDTH ? Global.BF_WIDTH : newX;
            newY = newY < 0 ? 0 : newY;
            newY = newY > Global.BF_HEIGHT ? Global.BF_HEIGHT : newY;

            return new Tuple<double, double>(newX, newY);
        }

        public double GetScore()
        {
            var score = 0.0;
            score += this.ScoreList["bullet"] * Global.SCORE_PER_BULLET_DAMAGE;
            score += this.ScoreList["survival"] * Global.SCORE_SURVIVAL_BONUS;
            score += this.ScoreList["shootScore"] * Global.SCORE_SHOOT;
            score += this.ScoreList["movementScore"] * Global.SCORE_MOVEMENT;
            score += this.ScoreList["gunDirection"] * Global.SCORE_GUN_DIRECTION;
            return score;
        }
    }
}
