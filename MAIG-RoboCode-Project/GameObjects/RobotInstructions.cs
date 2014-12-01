namespace MAIG_RoboCode_Project
{
    public class RobotInstructions
    {

        public double MoveDistance { get; private set; }

        public double RobotDegrees { get; private set; }

        public double GunDegrees { get; private set; }

        public double RadarDegrees { get; private set; }

        public double FirePower { get; private set; }

        public RobotInstructions(double moveDistance, double robotDegrees, double gunDegrees, double radarDegrees, double firePower)
        {
            this.MoveDistance = moveDistance;
            this.RobotDegrees = robotDegrees;
            this.GunDegrees = gunDegrees;
            this.RadarDegrees = radarDegrees;
            this.FirePower = firePower;
        }

        public static RobotInstructions GetEnemyInstructions(Gamestate gs)
        {
            var distance = 0;
            var robotDegrees = 0;
            var gunDegrees = 0;
            var radarDegrees = 0;
            var firePower = 0;

            //TODO: Implement this!
            return new RobotInstructions(distance,robotDegrees,gunDegrees,radarDegrees,firePower);
        }
    }
}
