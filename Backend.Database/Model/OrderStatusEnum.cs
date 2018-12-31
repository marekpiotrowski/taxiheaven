using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Database.Model
{
    public enum OrderStatusEnum
    {
        ToApproveByTheCustomer = 1,
        ToAssign = 2,
        ToApproveByTheDriver = 3,
        Ongoing = 4,
        Done = 5
    }
}
