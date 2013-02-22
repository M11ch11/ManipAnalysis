namespace ManipAnalysis.Container
{
    public class SubjectInformationContainer
    {
        public readonly int ID;
        public readonly string SubjectID;
        public readonly string SubjectName;

        public SubjectInformationContainer(int id, string subjectName, string subjectID)
        {
            ID = id;
            SubjectName = subjectName;
            SubjectID = subjectID;
        }

        public override string ToString()
        {
            return SubjectID;
        }
    }
}