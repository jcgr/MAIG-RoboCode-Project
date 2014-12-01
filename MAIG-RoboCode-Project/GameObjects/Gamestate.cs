namespace MAIG_RoboCode_Project
{
    using System.Collections.Generic;
    using System.Linq;

    public class Gamestate
    {
        public RobotInfo OurRobot { get; private set; }

        public RobotInfo EnemyRobot { get; private set; }

        public List<Projectile> FlyingProjectiles { get; private set; }

        public bool RobotDied { get; private set; }

        public Gamestate(RobotInfo ours, RobotInfo enemy, List<Projectile> projectilesInFlight)
        {
            OurRobot = ours;
            this.EnemyRobot = enemy;
            FlyingProjectiles = projectilesInFlight;
            RobotDied = false;
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

            return new Gamestate(ourRobot,enemyRobot,movedProjectiles);
        }
    }
}
