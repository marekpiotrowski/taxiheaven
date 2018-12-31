using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.Models
{
    public class UserRoleModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
