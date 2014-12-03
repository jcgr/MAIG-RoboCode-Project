namespace MAIG_RoboCode_Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Gamestate
    {
        public RobotInfo OurRobot { get; private set; }

        public RobotInfo EnemyRobot { get; private set; }

        public List<Projectile> FlyingProjectiles { get; private set; }

        public bool RobotDied { get; private set; }

        public RobotInstructions Instructions { get; private set; }

        public Gamestate(RobotInfo ours, RobotInfo enemy, List<Projectile> projectilesInFlight, RobotInstructions ri)
        {
            this.OurRobot = ours;
            this.EnemyRobot = enemy;
            this.FlyingProjectiles = projectilesInFlight;
            this.RobotDied = false;
            this.Instructions = ri;
        }

        public static Gamestate SimulateTurn(Gamestate gs, RobotInstructions ours, RobotInstructions enemy = null)
        {
            var movedProjectiles = gs.FlyingProjectiles.Select(p => p.NextTick()).ToList();

            enemy = enemy ?? RobotInstructions.GetEnemyInstructions(gs, movedProjectiles);

            var ourRobot = gs.OurRobot.NextTick(ours, movedProjectiles, gs.EnemyRobot);
            var enemyRobot = gs.EnemyRobot.NextTick(enemy, movedProjectiles, gs.OurRobot);

            if (enemyRobot.Status == RoboStatus.Destroyed && ourRobot.Status != RoboStatus.Destroyed)
            {
                ourRobot.ScoreList["survival"] += 1;
            }

            if (ourRobot.Status == RoboStatus.Destroyed && enemyRobot.Status != RoboStatus.Destroyed)
            {
                enemyRobot.ScoreList["survival"] += 1;
            }

            var gunAngleToEnemy = Global.XYToDegree(enemyRobot.X, enemyRobot.Y, ourRobot.X, ourRobot.Y);
            var angle1 = Math.Abs(gunAngleToEnemy - ourRobot.GunHeading);
            var angle2 = Math.Abs(ourRobot.GunHeading - gunAngleToEnemy);

            ourRobot.ScoreList["gunDirection"] = angle1 < angle2 ? -angle1 / 10 : -angle2 / 10;

            if (Math.Abs(ourRobot.Velocity) <= Math.Abs(gs.OurRobot.Velocity))
            {
                ourRobot.ScoreList["movementScore"] -= 1;
            }
            else
            {
                ourRobot.ScoreList["movementScore"] += 1.5;
            }

            if (ours.FirePower > 0 && gs.OurRobot.CanFire)
            {
                ourRobot.ScoreList["shootScore"] += 1;
            }

            return new Gamestate(ourRobot, enemyRobot, movedProjectiles, ours);
        }
    }
}
