﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoDbTest.Entities
{
    class Subject
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string PId { get; set; }
    }
}