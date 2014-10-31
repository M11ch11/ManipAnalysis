using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.Container
{
    internal class StatisticPlotContainer
    {
        private readonly string _group;

        private readonly string _study;

        private readonly SubjectContainer _subject;

        private readonly string _szenario;

        private readonly List<int> _trials;

        private readonly string _turn;

        public StatisticPlotContainer(string study, string group, string szenario, SubjectContainer subject, string turn, IEnumerable<string> trials)
        {
            _study = study;
            _group = group;
            _szenario = szenario;
            _subject = subject;
            _turn = turn;

            _trials = new List<int>();

            foreach (string
                trial
                in
                trials)
            {
                _trials.Add(Convert.ToInt32(trial.Substring(6, 3)));
            }
        }

        public List<int> Trials
        {
            get
            {
                return _trials;
            }
        }

        public string Study
        {
            get
            {
                return _study;
            }
        }

        public string Group
        {
            get
            {
                return _group;
            }
        }

        public string Szenario
        {
            get
            {
                return _szenario;
            }
        }

        public SubjectContainer Subject
        {
            get
            {
                return _subject;
            }
        }

        public string Turn
        {
            get
            {
                return _turn;
            }
        }

        public bool UpdateStatisticPlotContainer(string study, string group, string szenario, SubjectContainer subject, string turn, IEnumerable<string> trials)
        {
            bool retval = false;

            if ((_study == study) && (_group == group) && (_szenario == szenario) && (_subject.PId == subject.PId) && (_turn == turn))
            {
                foreach (string
                    trial
                    in
                    trials)
                {
                    int temp = Convert.ToInt32(trial.Substring(6, 3));
                    if (!_trials.Contains(temp))
                    {
                        _trials.Add(temp);
                    }
                }
                retval = true;
            }

            return retval;
        }

        public override string ToString()
        {
            _trials.Sort();
            string retVal = _study + " - " + _group + " - " + _szenario + " - " + _subject + " - " + _turn + " - Trials ";

            retVal += _trials[0];
            int tempCounter = 0;
            for (int i = 1; i < _trials.Count(); i
                                                     ++)
            {
                if (_trials[i - 1] != _trials[i] - 1)
                {
                    if ((tempCounter + 1) != i)
                    {
                        retVal += "-" + _trials[i - 1];
                    }
                    retVal += "/" + _trials[i];
                    tempCounter = i;
                }
                else if (i == _trials.Count() - 1)
                {
                    retVal += "-" + _trials[i];
                }
            }

            return retVal;
        }

        public string GetTrialsString()
        {
            _trials.Sort();
            string retVal = _trials[0].ToString(CultureInfo.InvariantCulture);

            int tempCounter = 0;
            for (int i = 1; i < _trials.Count(); i
                                                     ++)
            {
                if (_trials[i - 1] != _trials[i] - 1)
                {
                    if ((tempCounter + 1) != i)
                    {
                        retVal += "-" + _trials[i - 1];
                    }
                    retVal += "/" + _trials[i];
                    tempCounter = i;
                }
                else if (i == _trials.Count() - 1)
                {
                    retVal += "-" + _trials[i];
                }
            }

            return retVal;
        }
    }
}