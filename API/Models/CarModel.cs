using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string VehicleIdentificationNumber { get; set; }
        public string Make { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public bool Special { get; set; }
    }
}