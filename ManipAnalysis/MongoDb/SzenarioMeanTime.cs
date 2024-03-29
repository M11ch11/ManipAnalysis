﻿using System;
using MongoDB.Bson;

namespace ManipAnalysis_v2.MongoDb
{
    public class SzenarioMeanTime
    {
        public ObjectId Id { get; set; }

        public string Study { get; set; }

        public string Group { get; set; }

        public SubjectContainer Subject { get; set; }

        public string Szenario { get; set; }

        public TargetContainer Target { get; set; }

        public TargetContainer Origin { get; set; }

        public MeasureFileContainer MeasureFile { get; set; }

        public TimeSpan MeanTime { get; set; }

        public TimeSpan MeanTimeStd { get; set; }
    }
}