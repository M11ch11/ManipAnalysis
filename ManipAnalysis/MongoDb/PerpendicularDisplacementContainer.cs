using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis_v2.MongoDb
{
    public class PerpendicularDisplacementContainer
    {
        private DateTime _timeStamp;
        public double PerpendicularDisplacement { get; set; }

        /// <summary>
        ///     Convert to UTC First!!!
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timeStamp.ToLocalTime(); }
            set { _timeStamp = value.ToUniversalTime(); }
        }
    }
}
