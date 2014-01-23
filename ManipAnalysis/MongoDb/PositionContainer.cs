﻿using System;

namespace ManipAnalysis_v2.MongoDb
{
    internal class PositionContainer
    {
        private DateTime _timeStamp;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        ///     Convert to UTC First!!!
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timeStamp.ToLocalTime(); }
            set { _timeStamp = value.ToUniversalTime(); }
        }

        public int PositionStatus { get; set; }
    }
}