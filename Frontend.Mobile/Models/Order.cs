using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Mobile.Models
{
    public class Order
    {
        public int Id { get; set; }
        public float StartLatitude { get; set; }
        public string Start { get; set; }
        public string Destination { get; set; }
        public string PhoneNumber { get; set; }
    }
}
