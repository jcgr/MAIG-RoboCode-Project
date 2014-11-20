namespace MAIG_RoboCode_Project
{
    using System.Collections.Generic;

    class Gamestate
    {
        public RobotInfo OurRobot { get; private set; }

        public RobotInfo EnemyRobt { get; private set; }

        public List<Projectile> FlyingProjectiles { get; private set; }

        public Gamestate(RobotInfo ours, RobotInfo enemy, List<Projectile> projectilesInFlight)
        {
            OurRobot = ours;
            EnemyRobt = enemy;
            FlyingProjectiles = projectilesInFlight;
        }

        public static Gamestate SimulateTurn(Gamestate gs, RobotInstructions ours, RobotInstructions enemy)
        {
            //TODO: Implement this!

            return new Gamestate();
        }
    }
}
