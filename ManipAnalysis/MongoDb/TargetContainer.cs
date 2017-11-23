namespace ManipAnalysis_v2.MongoDb
{
    public class TargetContainer
    {

        //TODO: What does the trial.Target.Number describe? Just a code for the coordinates of the TargetPoint/some other kind of id for the target?
        public int Number { get; set; }

        public double XPos { get; set; }

        public double YPos { get; set; }

        public double ZPos { get; set; }

        public double Radius { get; set; }
    }
}