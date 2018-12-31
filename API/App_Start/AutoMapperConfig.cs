using API.Models;
using AutoMapper;
using Backend.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.App_Start
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<Driver, DriverModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DriverStatus.Title))
                .ForMember(dest => dest.Basement, opt => opt.MapFrom(src => src.Basement.Name));
            Mapper.CreateMap<Car, CarModel>();
            Mapper.CreateMap<WorkSession, WorkSessionModel>();
            Mapper.CreateMap<Order, OrderModel>();
            Mapper.CreateMap<OrderModel, Order>();
            Mapper.CreateMap<Order, AbbreviatedOrderModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus.Title));
            Mapper.CreateMap<User, UserModel>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRole))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRole.Select(x => x.RoleId)));
            // Mapper.CreateMap<UserRole, UserRoleModel>();
            Mapper.CreateMap<RegisterUserModel, User>();
            Mapper.CreateMap<Basement, BasementModel>();
            Mapper.CreateMap<BasementModel, Basement>();
            Mapper.CreateMap<AddDriverModel, Driver>();
            Mapper.CreateMap<AddDriverModel, Car>();
            Mapper.CreateMap<AddDriverModel, Device>();
        }
    }
}