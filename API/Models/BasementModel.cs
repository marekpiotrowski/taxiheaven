﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class BasementModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }
}