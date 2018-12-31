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
    public class DriverController : ApiController
    {
        private readonly DriverRepository _driverRepository;
        private readonly WorkSessionRepository _workSessionRepository;
        private readonly OrderRepository _orderRepository;
        private readonly CarRepository _carRepository;
        private readonly DeviceRepository _deviceRepository;

        public DriverController()
        {
            TaxiHeavenContext ctx = new TaxiHeavenContext();
            _driverRepository = new DriverRepository(ctx);
            _workSessionRepository = new WorkSessionRepository(ctx);
            _orderRepository = new OrderRepository(ctx);
            _carRepository = new CarRepository(ctx);
            _deviceRepository = new DeviceRepository(ctx);
        }

        public IQueryable<DriverModel> Get()
        {
            var entities = _driverRepository.Get();
            var result = entities.ProjectTo<DriverModel>();
            return result;
        }

        public DriverModel Get(int id)
        {
            var entity = _driverRepository.Get(id);
            var result = Mapper.Map<DriverModel>(entity);
            return result;
        }
        [BasicHttpAuthorize(3)]
        public IHttpActionResult Post(AddDriverModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest("Wprowadzone dane są niepoprawne.");
            //var driver = Mapper.Map<DriverModel>(model);
            //var car = Mapper.Map<CarModel>(model);
            //var device = Mapper.Map<DeviceModel>(model);

            var driverEntity = Mapper.Map<Driver>(model);
            var deviceEntity = Mapper.Map<Device>(model);
            deviceEntity.Active = false;
            var carEntity = Mapper.Map<Car>(model);

            var device = _deviceRepository.Post(deviceEntity);
            var car = _carRepository.Post(carEntity);
            driverEntity.CarId = car.Id;
            driverEntity.DeviceId = device.Id;
            driverEntity.StatusId = (int) DriverStatusEnum.Inactive;
            _driverRepository.Post(driverEntity);

            return Ok("Zarejestrowano kierowcę.");
        }

        [HttpPut]
        public void UpdateCoordinates(CoordinatesModel coordinates)
        {
            var entity = _driverRepository.Get().FirstOrDefault(x => x.Device.HardwareId == coordinates.DeviceId);
            if (entity == null)
                return;
            entity.Latitude = coordinates.Latitude;
            entity.Longitude = coordinates.Longitude;
            _driverRepository.Put(entity);
        }

        [HttpPost]
        [Route("api/Driver/TakeOrders")]
        public OrderModel TakeOrders(string deviceId)
        {
            var order = _workSessionRepository.GetNextOrder(deviceId);
            var result = Mapper.Map<OrderModel>(order);
            return result;
        }


        [HttpPost]
        [Route("api/Driver/RejectOrder")]
        public void RejectOrder(string deviceId, int orderId)
        {
            _orderRepository.RejectOrder(deviceId, orderId);
        }

        [HttpPost]
        [Route("api/Driver/AcceptOrder")]
        public void AcceptOrder(int orderId)
        {
            var order = _orderRepository.Get(orderId);
            var driver = order.WorkSession.Driver;
            driver.StatusId = (int) DriverStatusEnum.FillsAnOrder;
            order.StatusId = (int) OrderStatusEnum.Ongoing;
            _driverRepository.Put(driver);
            _orderRepository.Put(order);
        }

        [HttpPost]
        [Route("api/Driver/MarkAsDone")]
        public void MarkAsDone(int orderId)
        {
            var order = _orderRepository.Get(orderId);
            var driver = order.WorkSession.Driver;
            driver.StatusId = (int)DriverStatusEnum.DrivesBack;
            order.StatusId = (int)OrderStatusEnum.Done;
            order.DestinationLatitude = driver.Latitude;
            order.DestinationLongitude = driver.Longitude;
            order.EndTime = DateTime.Now;
            _driverRepository.Put(driver);
            _orderRepository.Put(order);
        }

        [HttpPost]
        [Route("api/Driver/ArrivedToBasement")]
        public void ArrivedToBasement(string deviceId)
        {
            _driverRepository.ArrivedToBasement(deviceId);

        }

        [HttpPost]
        [Route("api/Driver/GetCurrent")]
        public DriverModel GetCurrent(string deviceId)
        {
            var entity = _driverRepository.Get().FirstOrDefault(x => x.Device.HardwareId == deviceId);
            var result = Mapper.Map<DriverModel>(entity);
            return result;
        }

        [HttpPost]
        [Route("api/Driver/SetState")]
        public DriverModel SetState(string deviceId, bool state)
        {
            var entity = _driverRepository.Get().FirstOrDefault(x => x.Device.HardwareId == deviceId);
            entity.StatusId = state ? (int) DriverStatusEnum.Idle : (int) DriverStatusEnum.Inactive;
            _driverRepository.Put(entity);
            var device = entity.Device;
            device.Active = state;
            _deviceRepository.Put(device);
            var result = Mapper.Map<DriverModel>(entity);
            return result;
        }
    }
}
