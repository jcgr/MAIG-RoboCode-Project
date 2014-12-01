namespace MAIG_RoboCode_Project
{
    using System.Collections.Generic;
    using System.Linq;

    public class Gamestate
    {
        public RobotInfo OurRobot { get; private set; }

        public RobotInfo EnemyRobt { get; private set; }

        public List<Projectile> FlyingProjectiles { get; private set; }

        public bool RobotDied { get; private set; }

        public Gamestate(RobotInfo ours, RobotInfo enemy, List<Projectile> projectilesInFlight)
        {
            OurRobot = ours;
            EnemyRobt = enemy;
            FlyingProjectiles = projectilesInFlight;
            RobotDied = false;
        }

        public static Gamestate SimulateTurn(Gamestate gs, RobotInstructions ours, RobotInstructions enemy = null)
        {
            enemy = enemy ?? RobotInstructions.GetEnemyInstructions(gs);

            var movedProjectiles = gs.FlyingProjectiles.Select(p => p.NextTick()).ToList();

            var ourRobot = gs.OurRobot.NextTick(ours, movedProjectiles, gs.EnemyRobt);
            var enemyRobot = gs.EnemyRobt.NextTick(enemy, movedProjectiles, gs.OurRobot);

            return new Gamestate(ourRobot,enemyRobot,movedProjectiles);
        }
    }
}
