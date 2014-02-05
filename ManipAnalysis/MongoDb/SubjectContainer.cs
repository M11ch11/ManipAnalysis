namespace ManipAnalysis_v2.MongoDb
{
    internal class SubjectContainer
    {
        public string Name { get; set; }
        public string PId { get; set; }

        public override string ToString()
        {
            return PId;
        }
    }
}