using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Backend.Database.Base;
using Backend.Database.Model;

namespace Backend.Database.Business
{
    public class DeviceRepository : Repository<Device>
    {
        protected override Expression<Func<Device, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }

        protected override Expression<Func<Device, bool>> QueryById(Device device)
        {
            return p => p.Id == device.Id;
        }

        public DeviceRepository(TaxiHeavenContext ctx)
            : base(ctx)
        {
        }
    }
}
