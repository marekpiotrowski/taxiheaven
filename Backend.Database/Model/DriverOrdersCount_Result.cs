//------------------------------------------------------------------------------
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
    
    public partial class DriverOrdersCount_Result
    {
        public Nullable<int> DriverId { get; set; }
        public Nullable<int> BasementId { get; set; }
        public Nullable<float> Distance { get; set; }
        public Nullable<int> DriverStatusId { get; set; }
        public Nullable<float> BasementDistance { get; set; }
        public Nullable<int> WorkSessionId { get; set; }
        public Nullable<bool> DeviceActive { get; set; }
        public Nullable<bool> HasSpecialCar { get; set; }
        public Nullable<int> OrdersCount { get; set; }
    }
}
