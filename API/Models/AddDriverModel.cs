using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class AddDriverModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int BasementId { get; set; }
        [Required]
        public string VehicleIdentificationNumber { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Mileage { get; set; }
        [Required]
        public bool Special { get; set; }
        [Required]
        public string HardwareId { get; set; }
    }
}