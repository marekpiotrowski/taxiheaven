using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class DeviceModel
    {
        public int Id { get; set; }
        public string HardwareId { get; set; }
        public bool Active { get; set; }
    }
}