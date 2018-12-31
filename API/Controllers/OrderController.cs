using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using API.App_Start;
using API.Models;
using AutoMapper.QueryableExtensions;
using Backend.Database.Business;
using Backend.Database.Model;
using AutoMapper;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrderController : ApiController
    {
        private readonly DriverRepository _driverRepository;
        private readonly WorkSessionRepository _workSessionRepository;
        private readonly OrderRepository _orderRepository;

        public OrderController()
        {
            TaxiHeavenContext ctx = new TaxiHeavenContext();
            _driverRepository = new DriverRepository(ctx);
            _workSessionRepository = new WorkSessionRepository(ctx);
            _orderRepository = new OrderRepository(ctx);
        }

        [HttpPost]
        [Route("api/Order/GetRecentOrders")]
        public IQueryable<AbbreviatedOrderModel> GetRecentOrders(string driverDeviceId)
        {
            var ws = _workSessionRepository.Get().FirstOrDefault(x => x.Driver.Device.HardwareId == driverDeviceId
                && x.EndTime == null);
            var orders = _orderRepository.Get().Where(x => x.WorkSessionId == ws.Id && x.StatusId != (int)OrderStatusEnum.ToApproveByTheDriver);
            var result = orders.ProjectTo<AbbreviatedOrderModel>(orders);
            return result;
        }

        public OrderModel Get(int id)
        {
            var entity = _orderRepository.Get(id);
            var result = Mapper.Map<OrderModel>(entity);
            return result;
        }
        public IHttpActionResult Post(OrderModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest("Tylko cel podróży może pozostać nie wypełniony.");
            var entity = Mapper.Map<Order>(model);
            entity.StartTime = DateTime.Now;
            entity.StatusId = (int)OrderStatusEnum.ToApproveByTheCustomer;
            _orderRepository.Post(entity);
            var result = Mapper.Map<OrderModel>(entity);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/Order/Confirm")]
        public void Confirm(int orderId)
        {
            var order = _orderRepository.Get(orderId);
            order.StatusId = (int) OrderStatusEnum.ToAssign;
            _orderRepository.Put(order);
            _orderRepository.AssignOrder(orderId);
        }

        [HttpPost]
        [Route("api/Order/Cancel")]
        public void Cancel(int orderId)
        {
            var order = _orderRepository.Get(orderId);
            order.StatusId = (int)OrderStatusEnum.Done;
            _orderRepository.Put(order);
        }
    }
}
