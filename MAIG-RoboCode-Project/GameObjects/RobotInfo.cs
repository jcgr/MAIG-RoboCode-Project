namespace MAIG_RoboCode_Project.GameObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Robocode;

    /// <summary>
    /// The status of a robot.
    /// </summary>
    public enum RoboStatus
    {
        /// <summary>
        /// The healthy status.
        /// </summary>
        Healthy = 0,

        /// <summary>
        /// The disabled status.
        /// </summary>
        Disabled = 1,

        /// <summary>
        /// The destroyed (or dead) status.
        /// </summary>
        Destroyed = 2
    }

    /// <summary>
    /// Class used to represent a robot in the MCTS algorithm.
    /// </summary>
    public class RobotInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RobotInfo"/> class.
        /// </summary>
        /// <param name="robot"> The robot to generate the RobotInfo from. </param>
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
            this.ScoreList = CreateScoreList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RobotInfo"/> class.
        /// </summary>
        /// <param name="energy"> The energy of the new robot. </param>
        /// <param name="velocity"> The velocity of the new robot. </param>
        /// <param name="x"> The x-coordinate of the new robot. </param>
        /// <param name="y"> The y-coordinate of the new robot. </param>
        /// <param name="heading"> The heading of the new robot. </param>
        /// <param name="gunHeading"> The gun heading of the new robot. </param>
        /// <param name="radarHeading"> The radar heading of the new robot. </param>
        /// <param name="gunHeat"> The gun heat of the new robot. </param>
        /// <param name="name"> The name of the new robot. </param>
        /// <param name="status"> The status of the new robot. </param>
        /// <param name="scores"> The scores of the new robot. </param>
        public RobotInfo(double energy, double velocity, double x, double y, double heading, double gunHeading, double radarHeading, double gunHeat, string name, RoboStatus status = RoboStatus.Healthy, Dictionary<string, double> scores = null)
        {
            this.Energy = energy;
            this.Velocity = velocity;
            this.X = x;
            this.Y = y;
            this.RobotHeading = (heading + 360) % 360;
            this.GunHeading = (gunHeading + 360) % 360;
            this.RadarHeading = (radarHeading + 360) % 360;
            this.GunHeat = gunHeat;
            this.Status = status;
            this.Name = name;
            this.ScoreList = scores ?? CreateScoreList();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the robot.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the energy of the robot.
        /// </summary>
        public double Energy { get; private set; }

        /// <summary>
        /// Gets the velocity of the robot.
        /// </summary>
        public double Velocity { get; private set; }

        /// <summary>
        /// Gets the x-coordinate of the robot.
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// Gets the y-coordinate of the robot.
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// Gets the heading of the robot's radar.
        /// </summary>
        public double RadarHeading { get; private set; }

        /// <summary>
        /// Gets the heading of the robot.
        /// </summary>
        public double RobotHeading { get; private set; }

        /// <summary>
        /// Gets the heading of the robot's gun.
        /// </summary>
        public double GunHeading { get; private set; }

        /// <summary>
        /// Gets the heat of the robot's gun.
        /// </summary>
        public double GunHeat { get; private set; }

        /// <summary>
        /// Gets the list of different score values for the robot.
        /// </summary>
        public Dictionary<string, double> ScoreList { get; private set; }

        /// <summary>
        /// Gets the status of the robot.
        /// </summary>
        public RoboStatus Status { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the robot can fire or not.
        /// </summary>
        public bool CanFire
        {
            get
            {
                return !(this.GunHeat > 0.0);
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates a score list.
        /// </summary>
        /// <param name="survival">The initial value of the survival score.</param>
        /// <param name="lastSurvivor">The initial value of the last survivor score.</param>
        /// <param name="bullet">The initial value of the bullet score.</param>
        /// <param name="robotHeading">The initial value of the robotHeading score.</param>
        /// <param name="shootScore">The initial value of the shoot score score.</param>
        /// <returns>The new score list.</returns>
        public static Dictionary<string, double> CreateScoreList(double survival = 0, double lastSurvivor = 0, double bullet = 0, double robotHeading = 0, double shootScore = 0)
        {
            var newList = new Dictionary<string, double>
                              {
                                  { "survival", survival },
                                  { "last", lastSurvivor },
                                  { "bullet", bullet },
                                  { "movementScore", bullet },
                                  { "robotHeading", robotHeading },
                                  { "shootScore", shootScore }
                              };

            return newList;
        }

        /// <summary>
        /// Copies a score list.
        /// </summary>
        /// <param name="scoreList">The score list to copy.</param>
        /// <returns>The copy of the original score list.</returns>
        public static Dictionary<string, double> CopyScoreList(Dictionary<string, double> scoreList)
        {
            var newList = new Dictionary<string, double>
                              {
                                  { "survival", scoreList["survival"] },
                                  { "last", scoreList["last"] },
                                  { "bullet", scoreList["bullet"] },
                                  { "movementScore", scoreList["movementScore"] },
                                  { "shootScore", scoreList["shootScore"] },
                                  { "robotHeading", scoreList["robotHeading"] }
                              };

            return newList;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Simulates the state of the robot in the next tick of the game.
        /// </summary>
        /// <param name="ri">The robot instructions indicating what the robot should do.</param>
        /// <param name="projectiles">The projectiles currently in flight.</param>
        /// <param name="enemyRobot">The information about the enemy robot.</param>
        /// <returns>The robot as it will be in the next tick of the game.</returns>
        public RobotInfo NextTick(RobotInstructions ri, List<Projectile> projectiles, RobotInfo enemyRobot)
        {
            var scores = CopyScoreList(this.ScoreList);

            var gunHeat = this.HeatAfterCooling(Global.CoolingRate);
            var projectileFired = ri.FirePower > 0 ? this.Shoot(ri.FirePower, ref gunHeat) : null;
            if (projectileFired != null)
            {
                projectiles.Add(projectileFired);
            }

            double damage;
            var energy = this.NewEnergy(projectiles, enemyRobot, out damage);
            scores["bullet"] = damage;
            var status = energy <= 0 ? RoboStatus.Destroyed : RoboStatus.Healthy;
            if (projectileFired != null)
            {
                energy -= projectileFired.Power;
            }

            status = energy <= 0 && status != RoboStatus.Destroyed ? RoboStatus.Disabled : RoboStatus.Healthy;

            var robotHeading = this.RobotHeading + NewHeading(ri.RobotDegrees, this.Velocity);
            var gunHeading = this.GunHeading + NewGunHeading(ri.GunDegrees) + robotHeading;
            var radarHeading = this.RadarHeading + NewRadarHeading(ri.RadarDegrees) + gunHeading;

            var velocity = this.NewVelocity(ri.VelocityChange);
            ri.MoveDistance = velocity;

            var newPos = this.NextPosition(robotHeading, velocity);

            var x = newPos.Item1;
            var y = newPos.Item2;

            if ((x < 0 || x > Global.BfWidth) || (y <= 0  || y >= Global.BfHeight))
            {
                velocity = 0;
            }

            return new RobotInfo(energy, velocity, x, y, robotHeading, gunHeading, radarHeading, gunHeat, this.Name, status, scores);
        }

        /// <summary>
        /// Calculates the next position of the robot.
        /// </summary>
        /// <param name="heading">The heading of the robot.</param>
        /// <param name="velocity">The speed of the robot.</param>
        /// <returns>The next position of the robot.</returns>
        public Tuple<double, double> NextPosition(double heading, double velocity)
        {
            var displacedPosition = Global.DegreeToXY(heading, velocity);

            var newX = displacedPosition.Item1 + this.X;
            var newY = displacedPosition.Item2 + this.Y;

            newX = newX <= 0 ? 0 : newX;
            newX = newX >= Global.BfWidth ? Global.BfWidth : newX;
            newY = newY <= 0 ? 0 : newY;
            newY = newY >= Global.BfHeight ? Global.BfHeight : newY;

            return new Tuple<double, double>(newX, newY);
        }

        /// <summary>
        /// Gets the score of this robot.
        /// </summary>
        /// <returns> The score of this robot. </returns>
        public double GetScore()
        {
            var score = 0.0;
            score -= this.Energy * Global.ScoreEnergy;
            score += this.ScoreList["bullet"] * Global.ScorePerBulletDamage;
            score += this.ScoreList["survival"] * Global.ScoreSurvivalBonus;
            score += this.ScoreList["shootScore"] * Global.ScoreShoot;
            score += this.ScoreList["movementScore"] * Global.ScoreMovement;
            score += this.ScoreList["robotHeading"] * Global.ScoreRobotHeading;
            return score;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates the change in gun heading.
        /// </summary>
        /// <param name="change">Amount to turn the gun.</param>
        /// <returns>How much the gun should turn.</returns>
        private static double NewGunHeading(double change)
        {
            return Math.Abs(change) > Global.MaxGunRotation ? (change > 0 ? Global.MaxGunRotation : -Global.MaxGunRotation) : change;
        }

        /// <summary>
        /// Calculates the new radar heading.
        /// </summary>
        /// <param name="change">The amount of change to apply in the radar heading.</param>
        /// <returns>How much the radar heading should change.</returns>
        private static double NewRadarHeading(double change)
        {
            return Math.Abs(change) > Global.MaxRadarRotation ? (change > 0 ? Global.MaxRadarRotation : -Global.MaxRadarRotation) : change;
        }

        /// <summary>
        /// Calculates the change in robot heading.
        /// </summary>
        /// <param name="change">Amount to turn the robot.</param>
        /// <param name="velocity">The speed of the robot.</param>
        /// <returns>How much the robot should turn.</returns>
        private static double NewHeading(double change, double velocity)
        {
            var maxTurnRate = 10 - (0.75 * Math.Abs(velocity));
            return Math.Abs(change) > maxTurnRate ? (change > 0 ? maxTurnRate : -maxTurnRate) : change;
        }

        /// <summary>
        /// Attempts to make the robot shoot.
        /// </summary>
        /// <param name="power">The power to fire with.</param>
        /// <param name="gunheat">The heat generated by the shot.</param>
        /// <returns>The projectile fired.</returns>
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

        /// <summary>
        /// Gets the new velocity of the robot.
        /// </summary>
        /// <param name="vc">The change to velocity.</param>
        /// <returns>The new velocity.</returns>
        private double NewVelocity(double vc)
        {
            return this.Velocity + vc;
        }

        /// <summary>
        /// Gets the energy of the robot in the next tick.
        /// </summary>
        /// <param name="projectiles">The projectiles currently flying.</param>
        /// <param name="enemy">The enemy robot.</param>
        /// <param name="damage">The damage that this robot dealt on this tick.</param>
        /// <returns>The new energy value.</returns>
        private double NewEnergy(List<Projectile> projectiles, RobotInfo enemy, out double damage)
        {
            damage = this.DamageDealt(projectiles, enemy);
            var energyChange = damage - this.DamageTaken(projectiles);
            return this.Energy + energyChange;
        }

        /// <summary>
        /// Gets the damage taken in this gamestate.
        /// </summary>
        /// <param name="projectiles">The projectiles in the air.</param>
        /// <returns>The damage taken by this robot on this tick.</returns>
        private double DamageTaken(IEnumerable<Projectile> projectiles)
        {
            var energyLoss = 0.0;
            foreach (var p in projectiles.Where(p => (Math.Abs(p.X - this.X) < Global.Tolerance) && (Math.Abs(p.Y - this.Y) < Global.Tolerance)))
            {
                energyLoss += p.Damage;
                p.Deactivate();
            }

            return energyLoss;
        }

        /// <summary>
        /// Gets the amount of damage dealt by this robot in this gamestate.
        /// </summary>
        /// <param name="projectiles">The projectiles currently in the air.</param>
        /// <param name="enemy">The enemy robot.</param>
        /// <returns>The damage dealt.</returns>
        private double DamageDealt(IEnumerable<Projectile> projectiles, RobotInfo enemy)
        {
            var energyGained = 0.0;
            foreach (var p in projectiles.Where(p => (Math.Abs(p.X - enemy.X) < Global.Tolerance) && (Math.Abs(p.Y - enemy.Y) < Global.Tolerance) && p.Owner.Equals(this.Name)))
            {
                energyGained += p.Damage;

                p.Deactivate();
            }

            return energyGained;
        }

        /// <summary>
        /// Calculates the heat of the gun after applying cooling.
        /// </summary>
        /// <param name="reduction">The reduction in heat to apply.</param>
        /// <returns>The new heat after applying the heat reduction.</returns>
        private double HeatAfterCooling(double reduction)
        {
            return Math.Max(this.GunHeat - reduction, 0);
        }

        #endregion
    }
}
