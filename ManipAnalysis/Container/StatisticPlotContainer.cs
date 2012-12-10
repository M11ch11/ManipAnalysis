using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class StatisticPlotContainer
    {

        string study, szenario, group, turn;
        SubjectInformationContainer subject;
        List<int> trials;

        public StatisticPlotContainer(string _study, string _group, string _szenario, SubjectInformationContainer _subject, string _turn, string[] _trials)
        {
            study = _study;
            group = _group;
            szenario = _szenario;
            subject = _subject;
            turn = _turn;
            
            trials = new List<int>();

            foreach (string trial in _trials)
            {
                trials.Add(Convert.ToInt32(trial.Substring(6, 3)));
            }
        }

        public bool updateStatisticPlotContainer(string _study, string _group, string _szenario, SubjectInformationContainer _subject, string _turn, string[] _trials)
        {
            bool retval = false;

            if ((study == _study) && (group == _group) && (szenario == _szenario) && (subject.id == _subject.id) && (subject.subject_id == _subject.subject_id) && (subject.subject_name == _subject.subject_name) && (turn == _turn))
            {
                foreach (string trial in _trials)
                {
                    int temp = Convert.ToInt32(trial.Substring(6, 3));
                    if (!trials.Contains(temp))
                    {
                        trials.Add(temp);

                    }
                }
                retval = true;
            }

            return retval;
        }

        public List<int> Trials
        {
            get { return trials; }
            set { trials = value; }
        }

        public string Study
        {
            get { return study; }
            set { study = value; }
        }

        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        public string Szenario
        {
            get { return szenario; }
            set { szenario = value; }
        }

        public SubjectInformationContainer Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Turn
        {
            get { return turn; }
            set { turn = value; }
        }

        public override string ToString()
        {
            trials.Sort();
            string retVal = study + " - " + group + " - " + szenario + " - " + subject + " - " + turn + " - Trials ";

            retVal += trials[0].ToString();
            int tempCounter = 0;
            for (int i = 1; i < trials.Count(); i++)
            {
                if (trials[i - 1] != trials[i] - 1)
                {
                    if ((tempCounter+1) != i)
                    {
                        retVal += "-" + trials[i - 1].ToString();
                    }
                    retVal += "/" + trials[i].ToString();
                    tempCounter = i;
                }
                else if (i == trials.Count() - 1)
                {
                    retVal += "-" + trials[i].ToString();
                }
            }

            return retVal;
        }

        public string getTrialsString()
        {
            trials.Sort();
            string retVal = trials[0].ToString();

            int tempCounter = 0;
            for (int i = 1; i < trials.Count(); i++)
            {
                if (trials[i - 1] != trials[i] - 1)
                {
                    if ((tempCounter + 1) != i)
                    {
                        retVal += "-" + trials[i - 1].ToString();
                    }
                    retVal += "/" + trials[i].ToString();
                    tempCounter = i;
                }
                else if (i == trials.Count() - 1)
                {
                    retVal += "-" + trials[i].ToString();
                }
            }

            return retVal;
        }
    }
}
