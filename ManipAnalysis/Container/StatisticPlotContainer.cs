﻿using System;
using System.Collections.Generic;
using System.Globalization;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.Container
{
    internal class StatisticPlotContainer
    {
        public StatisticPlotContainer(string study, string group, string szenario, SubjectContainer subject, string turn,
            IEnumerable<string> trials)
        {
            Study = study;
            Group = group;
            Szenario = szenario;
            Subject = subject;
            Turn = turn;

            Trials = new List<int>();

            foreach (var
                trial
                in
                trials)
            {
                Trials.Add(Convert.ToInt32(trial.Substring(6, 3)));
            }
        }

        public List<int> Trials { get; }

        public string Study { get; }

        public string Group { get; }

        public string Szenario { get; }

        public SubjectContainer Subject { get; }

        public string Turn { get; }

        public bool UpdateStatisticPlotContainer(string study, string group, string szenario, SubjectContainer subject,
            string turn, IEnumerable<string> trials)
        {
            var retval = false;

            if ((Study == study) && (Group == group) && (Szenario == szenario) && (Subject.PId == subject.PId) &&
                (Turn == turn))
            {
                foreach (var
                    trial
                    in
                    trials)
                {
                    var temp = Convert.ToInt32(trial.Substring(6, 3));
                    if (!Trials.Contains(temp))
                    {
                        Trials.Add(temp);
                    }
                }
                retval = true;
            }

            return retval;
        }

        public override string ToString()
        {
            Trials.Sort();
            var retVal = Study + " - " + Group + " - " + Szenario + " - " + Subject + " - " + Turn + " - Trials ";

            retVal += Trials[0];
            var tempCounter = 0;
            for (var i = 1; i < Trials.Count; i++)
            {
                if (Trials[i - 1] != Trials[i] - 1)
                {
                    if (tempCounter + 1 != i)
                    {
                        retVal += "-" + Trials[i - 1];
                    }
                    retVal += "/" + Trials[i];
                    tempCounter = i;
                }
                else if (i == Trials.Count - 1)
                {
                    retVal += "-" + Trials[i];
                }
            }

            return retVal;
        }

        public string GetTrialsString()
        {
            Trials.Sort();
            var retVal = Trials[0].ToString(CultureInfo.InvariantCulture);

            var tempCounter = 0;
            for (var i = 1; i < Trials.Count; i++)
            {
                if (Trials[i - 1] != Trials[i] - 1)
                {
                    if (tempCounter + 1 != i)
                    {
                        retVal += "-" + Trials[i - 1];
                    }
                    retVal += "/" + Trials[i];
                    tempCounter = i;
                }
                else if (i == Trials.Count - 1)
                {
                    retVal += "-" + Trials[i];
                }
            }

            return retVal;
        }
    }
}