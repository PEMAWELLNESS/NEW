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
    
    public partial class NC_TBL_PAYMENT_TRANS
    {
        public int id { get; set; }
        public string PR_NO { get; set; }
        public string RECEIPT_NO { get; set; }
        public System.DateTime TXN_DATE { get; set; }
        public System.DateTime ACTUAL_PAID_DATE { get; set; }
        public Nullable<int> COUNTER_NO { get; set; }
        public decimal PAID_AMOUNT { get; set; }
        public int PAYMENT_MODE_CODE { get; set; }
        public int PAYMENT_TYPE_CODE { get; set; }
        public string CC_DB_CARDNO { get; set; }
        public string NAME_ON_CARD { get; set; }
        public string status_code { get; set; }
        public string status_result { get; set; }
        public string SettlType { get; set; }
        public string Addl_Info1 { get; set; }
        public string Addl_Info2 { get; set; }
        public string Addl_Info3 { get; set; }
        public string Addl_Info4 { get; set; }
        public string Addl_Info5 { get; set; }
        public string Addl_Info6 { get; set; }
        public string Addl_Info7 { get; set; }
        public string Biller_Id { get; set; }
        public string Transaction_Id { get; set; }
        public string BankName { get; set; }
        public string BankMrcnhtID { get; set; }
        public string Err_Sts { get; set; }
        public string Err_Desc { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string RECEIPT_CANCELLED { get; set; }
        public string MASTER_UPDATED { get; set; }
        public Nullable<int> STAFF_CODE { get; set; }
        public string FINAL_STATUS { get; set; }
        public string REMARKS { get; set; }
        public string Pay_Category { get; set; }
        public string EntryType { get; set; }
        public string GatewayReceiptNo { get; set; }
    }
}
