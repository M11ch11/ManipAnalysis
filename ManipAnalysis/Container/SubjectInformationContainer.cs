using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    public class SubjectInformationContainer
    {
        public int id;
        public string subject_name,subject_id;

        public SubjectInformationContainer(int _id, string _subject_name, string _subject_id)
        {
            id = _id;
            subject_name = _subject_name;
            subject_id = _subject_id;
        }

        public override string ToString()
        {
            return subject_id;
        }
    }
}
