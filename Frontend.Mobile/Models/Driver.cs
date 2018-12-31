using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Mobile.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string Basement { get; set; }
        public string FirstName { get; set; }
    }
}
