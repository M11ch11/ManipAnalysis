using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest.Container
{
    class PositionContainer
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        /// <summary>
        /// Convert to UTC First!!!
        /// </summary>
        public DateTime TimeStamp { get; set; }
        public int PositionStatus { get; set; }
    }
}
