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
    
    public partial class NC_TBL_Billing_Trans
    {
        public string PR_NO { get; set; }
        public Nullable<decimal> TOTAL_AMOUNT { get; set; }
        public Nullable<decimal> TAX { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string Je_NARRATION { get; set; }
        public Nullable<decimal> Je_AMOUNT { get; set; }
        public Nullable<decimal> NET_AMOUNT { get; set; }
        public int Bill_TYPE_CODE { get; set; }
        public Nullable<System.DateTime> billed_date { get; set; }
        public string Paid_status { get; set; }
        public string MASTER_UPDATED { get; set; }
        public Nullable<int> Staff_code { get; set; }
        public string Remarks { get; set; }
        public int Id { get; set; }
        public string BillNo { get; set; }
        public string Guest_Attender_ARNO { get; set; }
        public string EntryType { get; set; }
    }
}
