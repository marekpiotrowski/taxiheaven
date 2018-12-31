using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Database.Model
{
    public enum DriverStatusEnum
    {
        Inactive = 1,
        Idle = 2,
        DrivesToStart = 3,
        FillsAnOrder = 4,
        DrivesBack = 5,
        NotApplicable = 6,
        Deciding = 7
    }
}
