using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Database.Base;
using Backend.Database.Model;
using System.Linq.Expressions;

namespace Backend.Database.Business
{
    public class OrderRepository : Repository<Order>
    {
        protected override Expression<Func<Order, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }
        protected override Expression<Func<Order, bool>> QueryById(Order order)
        {
            return p => p.Id == order.Id;
        }

        public OrderRepository(TaxiHeavenContext ctx) : base(ctx) { }


        public void RejectOrder(string deviceId, int orderId)
        {
            var driver = _context.Driver.FirstOrDefault(x => x.Device.HardwareId == deviceId);
            driver.StatusId = (int) DriverStatusEnum.Inactive;
            _context.SaveChanges();
        }

        public void AcceptOrder(int orderId)
        {
            var order = _context.Order.FirstOrDefault(x => x.Id == orderId);
            order.StatusId = (int)OrderStatusEnum.Ongoing;
            _context.SaveChanges();
        }

        public void MarkAsDone(int orderId)
        {
            var order = _context.Order.FirstOrDefault(x => x.Id == orderId);
            order.StatusId = (int)OrderStatusEnum.Done;
            _context.SaveChanges();
        }

        public void AssignOrder(int orderId)
        {
            _context.Database.ExecuteSqlCommand("EXEC dbo.AssignOrder " + orderId);
        }
    }
}
