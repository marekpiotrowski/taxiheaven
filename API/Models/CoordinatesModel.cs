using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class CoordinatesModel
    {
        public string DeviceId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}