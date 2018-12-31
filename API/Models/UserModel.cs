using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Backend.Database.Model;

namespace API.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<int> UserRoles { get; set; }
    }
}