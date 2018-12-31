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
    public class WorkSessionRepository : Repository<WorkSession>
    {
        protected override Expression<Func<WorkSession, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }
        protected override Expression<Func<WorkSession, bool>> QueryById(WorkSession ws)
        {
            return p => p.Id == ws.Id;
        }
        public WorkSessionRepository(TaxiHeavenContext ctx) : base(ctx) { }

        public Order GetNextOrder(string deviceId)
        {
            var driver = _context.Driver.FirstOrDefault(x => x.Device.HardwareId == deviceId);

            if (driver.StatusId == (int) DriverStatusEnum.Deciding ||
                driver.StatusId == (int) DriverStatusEnum.FillsAnOrder ||
                driver.StatusId == (int) DriverStatusEnum.Inactive ||
                driver.StatusId == (int) DriverStatusEnum.NotApplicable)
                return null;

            var order = _context.Order.FirstOrDefault(x => x.WorkSession.Driver.Device.HardwareId == deviceId
                && x.StatusId == (int)OrderStatusEnum.ToApproveByTheDriver);

            if (order != null)
            {
                driver.StatusId = (int) DriverStatusEnum.Deciding;
                _context.SaveChanges();
            }
            return order;
        }
    }
}
