﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Backend.Database.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class TaxiHeavenContext : DbContext
    {
        public TaxiHeavenContext()
            : base("name=TaxiHeavenContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Basement> Basement { get; set; }
        public virtual DbSet<Car> Car { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<Driver> Driver { get; set; }
        public virtual DbSet<DriverStatus> DriverStatus { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<WorkSession> WorkSession { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
    
        [DbFunction("TaxiHeavenContext", "DriverOrdersCount")]
        public virtual IQueryable<DriverOrdersCount_Result> DriverOrdersCount(Nullable<float> startLatitude, Nullable<float> startLongitude)
        {
            var startLatitudeParameter = startLatitude.HasValue ?
                new ObjectParameter("StartLatitude", startLatitude) :
                new ObjectParameter("StartLatitude", typeof(float));
    
            var startLongitudeParameter = startLongitude.HasValue ?
                new ObjectParameter("StartLongitude", startLongitude) :
                new ObjectParameter("StartLongitude", typeof(float));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<DriverOrdersCount_Result>("[TaxiHeavenContext].[DriverOrdersCount](@StartLatitude, @StartLongitude)", startLatitudeParameter, startLongitudeParameter);
        }
    
        public virtual int AssignCar(Nullable<int> driverId)
        {
            var driverIdParameter = driverId.HasValue ?
                new ObjectParameter("DriverId", driverId) :
                new ObjectParameter("DriverId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AssignCar", driverIdParameter);
        }
    
        public virtual int AssignDevice(Nullable<int> driverId)
        {
            var driverIdParameter = driverId.HasValue ?
                new ObjectParameter("DriverId", driverId) :
                new ObjectParameter("DriverId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AssignDevice", driverIdParameter);
        }
    
        public virtual int AssignOrder(Nullable<int> orderId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AssignOrder", orderIdParameter);
        }
    }
}
