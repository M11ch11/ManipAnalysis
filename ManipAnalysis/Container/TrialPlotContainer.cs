using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class TrialPlotContainer
    {
        public string study, group, date, szenario, subject, target, trial;

        public TrialPlotContainer(string _study, string _group, string _date, string _szenario, string _subject, string _target, string _trial)
        {
            study = _study;
            group = _group;
            date = _date;
            szenario = _szenario;
            subject = _subject;
            target = _target;
            trial = _trial;
        }

        public override string ToString()
        {
            return study + " - " + group + " - " + date + " - " + szenario + " - " + subject + " - Target " + target + " - Trial " + trial;
        }

    }
}
