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
    
    public partial class NC_TBL_Gateway_Payment_Details
    {
        public int Id { get; set; }
        public string UHID { get; set; }
        public string PRNO { get; set; }
        public string Name { get; set; }
        public string EntryType { get; set; }
        public int PAYMENT_TYPE_CODE { get; set; }
        public System.DateTime Txn_Date { get; set; }
        public string PaidStatus { get; set; }
        public decimal Paid_Amount { get; set; }
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
        public string GatewayReceiptNo { get; set; }
        public string TransactionNo { get; set; }
        public string BankName { get; set; }
        public string BankMrcnhtID { get; set; }
        public string Err_Sts { get; set; }
        public string Err_Desc { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CardType { get; set; }
        public string GatewayRespStatus { get; set; }
        public Nullable<int> PAYMENT_MODE_CODE { get; set; }
        public string AMAFlag { get; set; }
        public string Gateway { get; set; }
    }
}
