﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DTOs
{
    public class ChartDataDTO
    {
        public String Date {  get; set; }

        public Double Value { get; set; }
    }
}
