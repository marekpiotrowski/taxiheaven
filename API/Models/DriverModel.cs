using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class DriverModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public int? StatusId { get; set; }
        public string Basement { get; set; }
        public int? BasementId { get; set; }
        public string PhoneNumber { get; set; }
        public int? CarId { get; set; }
        public int? DeviceId { get; set; }
    }
}