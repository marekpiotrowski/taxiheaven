using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        [Required]
        public float StartLatitude { get; set; }
        [Required]
        public float StartLongitude { get; set; }
        public float DestinationLatitude { get; set; }
        public float DestinationLongitude { get; set; }
        [Required]
        public string Start { get; set; }
        public string Destination { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool RequiresSpecialCar { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}