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
    public class DriverRepository : Repository<Driver>
    {
        protected override Expression<Func<Driver, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }
        protected override Expression<Func<Driver, bool>> QueryById(Driver driver)
        {
            return p => p.Id == driver.Id;
        }

        public DriverRepository(TaxiHeavenContext ctx) : base(ctx) { }

        public void ArrivedToBasement(string deviceId)
        {
            var entity = _context.Driver.FirstOrDefault(d => d.Device.HardwareId == deviceId);
            entity.StatusId = (int) DriverStatusEnum.Idle;
            _context.SaveChanges();
        }
    }
}
