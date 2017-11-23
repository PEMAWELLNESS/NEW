//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rooms.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NC_TBL_BOOKED_DATES
    {
        public string PRNO { get; set; }
        public string PackageType { get; set; }
        public string PackageName { get; set; }
        public decimal NoOfDays { get; set; }
        public System.DateTime ArrivalDate { get; set; }
        public System.DateTime DepartureDate { get; set; }
        public Nullable<System.DateTime> AlternateArrivalDate { get; set; }
        public Nullable<System.DateTime> AlternateDepartureDate { get; set; }
        public Nullable<decimal> AccompanyingGuests { get; set; }
        public string RoomView { get; set; }
        public string RoomType { get; set; }
        public decimal RoomTariff { get; set; }
        public Nullable<System.DateTime> ConfirmedFromDate { get; set; }
        public Nullable<System.DateTime> ConfirmedToDate { get; set; }
        public string DoctorsComments { get; set; }
        public string PackageConfirmStatus { get; set; }
        public string SuggestDays { get; set; }
        public Nullable<decimal> Package_Amount { get; set; }
        public int Id { get; set; }
        public string CMOComments { get; set; }
        public string CMOEscalated { get; set; }
        public string Group_Code { get; set; }
        public string Accompany_Type { get; set; }
        public string Guest_Attender_Name { get; set; }
        public string ApplicationStatus { get; set; }
        public string AccompanyingGuestUHID { get; set; }
        public string AccompanyingGuestPRNO { get; set; }
        public string Guest_Attender_ARNO { get; set; }
        public string UHID { get; set; }
        public string UserType { get; set; }
        public string CallingStatus { get; set; }
        public string CallingRemarks { get; set; }
        public string Transportation_Required { get; set; }
        public string Mem_Camp_Type { get; set; }
        public string Trans_CallingStatus { get; set; }
        public string Trans_CallingRemarks { get; set; }
        public string Temp_PRNO { get; set; }
    }
}
