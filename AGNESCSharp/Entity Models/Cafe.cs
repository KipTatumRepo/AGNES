//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AGNESCSharp.Entity_Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cafe
    {
        public long PID { get; set; }
        public long CostCenter { get; set; }
        public long BldgId { get; set; }
        public long ProfitCenterId { get; set; }
        public string Manager { get; set; }
        public string DistrictManager { get; set; }
        public Nullable<bool> HasHood { get; set; }
        public Nullable<short> StationCount { get; set; }
        public Nullable<long> AnchorStationFoodType { get; set; }
        public Nullable<long> AnchorStationFoodSubType { get; set; }
        public short BrandStations { get; set; }
        public int SqFt { get; set; }
        public int FOHSqFt { get; set; }
        public int BOHSqFt { get; set; }
        public int EspressoSqFt { get; set; }
    }
}
