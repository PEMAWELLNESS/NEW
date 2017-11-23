using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Rooms.Controllers
{
    public class PackageConfirmationController : BaseController
    {
        private pema_dbEntities obj = new pema_dbEntities();
        public ActionResult Checking(string stod, string PRNO, string SugArrival, string roomview, string roomtype)
        {
            string roomNo = "";
            string str = stod;
            IFormatProvider cultureInfo = new CultureInfo("fr-FR", true);
            DateTime dateTime = DateTime.Parse(str, cultureInfo, DateTimeStyles.AssumeLocal);
            string sugArrival = SugArrival;
            CultureInfo cultureInfo1 = new CultureInfo("fr-FR", true);
            DateTime dateTime1 = DateTime.Parse(sugArrival, cultureInfo, DateTimeStyles.AssumeLocal);
            string pRNO = PRNO;
            (
                from x in this.obj.NC_TBL_BOOKED_DATES
                where x.PRNO == pRNO
                select x).FirstOrDefault<NC_TBL_BOOKED_DATES>();
            IQueryable<NC_TBL_ROOM_MASTER> nCTBLROOMMASTER =
                from y in this.obj.NC_TBL_ROOM_MASTER
                where !this.obj.NC_TBL_GUEST_ROOM_TRANSACTION.Select<NC_TBL_GUEST_ROOM_TRANSACTION, string>((NC_TBL_GUEST_ROOM_TRANSACTION x) => x.Room_No).Contains<string>(y.Room_No) && (y.Active == "Y") && (y.Room_View == roomview) && (y.Room_Type == roomtype)
                select y;
            if (nCTBLROOMMASTER.Count<NC_TBL_ROOM_MASTER>() != 0)
            {
                roomNo = nCTBLROOMMASTER.FirstOrDefault<NC_TBL_ROOM_MASTER>().Room_No;
            }
            else
            {
                List<NC_TBL_ROOM_MASTER> list = (
                    from a in this.obj.NC_TBL_ROOM_MASTER
                    where (a.Room_View == roomview) && (a.Room_Type == roomtype) && (a.Active == "Y")
                    select a).ToList<NC_TBL_ROOM_MASTER>();
                foreach (NC_TBL_ROOM_MASTER nCTBLROOMMASTER1 in list)
                {
                    if (roomNo != "")
                    {
                        continue;
                    }
                    if ((
                        from a in this.obj.NC_TBL_GUEST_ROOM_TRANSACTION
                        where a.Room_No == nCTBLROOMMASTER1.Room_No
                        select a).ToList<NC_TBL_GUEST_ROOM_TRANSACTION>() == null)
                    {
                        roomNo = nCTBLROOMMASTER1.Room_No;
                    }
                    else
                    {
                        if ((
                            from b in this.obj.NC_TBL_GUEST_ROOM_TRANSACTION
                            where (b.Room_No == nCTBLROOMMASTER1.Room_No) && ((b.ArrivalDate >= dateTime1) && (b.ArrivalDate < dateTime) || (b.DepartureDate > dateTime1) && (b.DepartureDate <= dateTime) || (b.ArrivalDate < dateTime1) && (b.DepartureDate > dateTime))
                            select new { RoomNo = b.Room_No }).ToList().Count != 0)
                        {
                            continue;
                        }
                        roomNo = nCTBLROOMMASTER1.Room_No;
                    }
                }
            }
            return base.Json(roomNo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckMembership(string PRNO, string arrivaldate, string departuredate, string PackageName)
        {
            int num = PackageName.IndexOf(' ', PackageName.IndexOf(' '));
            int num1 = Convert.ToInt32(PackageName.Substring(0, num));
            DateTime dateTime = Convert.ToDateTime(arrivaldate);
            DateTime dateTime1 = dateTime.AddDays((double)num1);
            string str = (
                from a in this.obj.NC_TBL_BOOKED_DATES
                where (a.PRNO == PRNO) && (a.Mem_Camp_Type == "M")
                select a into b
                select b.Group_Code).FirstOrDefault<string>();
            if (str == null || !(str != ""))
            {
                return base.Json("", JsonRequestBehavior.AllowGet);
            }
            NC_TBL_MEMBERSHIPDETAILS_HEADER nCTBLMEMBERSHIPDETAILSHEADER = (
                from e in this.obj.NC_TBL_MEMBERSHIPDETAILS_HEADER
                where (e.Mem_Id == str) && (e.Mem_Payment_Status == "Y")
                select e).FirstOrDefault<NC_TBL_MEMBERSHIPDETAILS_HEADER>();
            DateTime dateTime2 = Convert.ToDateTime(arrivaldate);
            DateTime dateTime3 = Convert.ToDateTime(dateTime1);
            DateTime dateTime4 = Convert.ToDateTime(nCTBLMEMBERSHIPDETAILSHEADER.Mem_StartDate);
            DateTime dateTime5 = Convert.ToDateTime(nCTBLMEMBERSHIPDETAILSHEADER.Mem_EndDate);
            if (dateTime4 <= dateTime2 && dateTime3 <= dateTime5)
            {
                return base.Json("", JsonRequestBehavior.AllowGet);
            }
            return base.Json("This Package is not allowed,Membership dates exceeded Please select another Package ", JsonRequestBehavior.AllowGet);
        }
        private static bool customCertValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        public ActionResult GetDays(string PackageType, string PackageName, string RomView, string RomType, int AccompanyingGuests, string Arrivaldate, string Departuredate)
        {
            string str = base.TempData["accompany"].ToString();
            DateTime dateTime = Convert.ToDateTime(Arrivaldate);
            Convert.ToDateTime(Departuredate);
            if (str != null)
            {
                var variable = (
                    from R in this.obj.NC_TBL_PACKAGE_MASTER
                    where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (dateTime >= R.EffetedFrom) && (dateTime <= R.EffetedTo) && (R.Room_View == RomView) && (R.Room_Type == RomType)
                    select new { NoOfDays = R.NoOfDays, PackageAmount = R.DO_Calc_Amt }).FirstOrDefault();
                return base.Json(variable, JsonRequestBehavior.AllowGet);
            }
            var variable1 = (
                from R in this.obj.NC_TBL_PACKAGE_MASTER
                where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (R.Room_View == RomView) && (R.Room_Type == RomType) && (dateTime >= R.EffetedFrom) && (dateTime <= R.EffetedTo)
                select new { NoOfDays = R.NoOfDays, PackageAmount = R.SO_Calc_Amt }).FirstOrDefault();
            return base.Json(variable1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Getnoofdays(string packagetype, string packagename, string roomview, string roomtype)
        {
            ActionResult actionResult;
            try
            {
                var variable = (
                    from n in this.obj.NC_TBL_PACKAGE_MASTER
                    where (n.PackageType == packagetype) && (n.PackageName == packagename) && (n.Room_View == roomview) && (n.Room_Type == roomtype)
                    select new { noofdays = n.NoOfDays }).Distinct().FirstOrDefault();
                actionResult = (variable == null ? base.Json(new { Value = "False", noofdays = variable.noofdays }, JsonRequestBehavior.AllowGet) : base.Json(new { Value = "True", noofdays = variable.noofdays }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception exception)
            {
                Exception innerException = exception.InnerException;
                actionResult = base.Json(new { Value = "False" }, JsonRequestBehavior.AllowGet);
            }
            return actionResult;
        }
        public ActionResult GetPackageNames(string PackageType, string RoomView, string Roomtype)
        {
            var collection =
                from x in (
                    from R in this.obj.NC_TBL_PACKAGE_MASTER
                    where (R.PackageType == PackageType) && (R.Room_View == RoomView) && (R.Room_Type == Roomtype)
                    orderby R.NoOfDays + (int?)0
                    select new { PackageName = R.PackageName, NoOfDays = R.NoOfDays }).Distinct()
                orderby x.NoOfDays
                select x;
            if (collection.Count() > 0)
            {
                return base.Json(collection.ToList(), JsonRequestBehavior.AllowGet);
            }
            return base.Json(collection.Count(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPackageTypes(string RoomView, string Roomtype)
        {
            var collection = (
                from R in this.obj.NC_TBL_PACKAGE_MASTER
                where (R.Room_Type == Roomtype) && (R.Room_View == RoomView)
                select new { PackageType = R.PackageType }).Distinct();
            if (collection.Count() > 0)
            {
                return base.Json(collection.ToList(), JsonRequestBehavior.AllowGet);
            }
            return base.Json(collection.Count(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRoomTypes(string PackageType, string PackageName, string RoomView)
        {
            var collection = (
                from R in this.obj.NC_TBL_PACKAGE_MASTER
                where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (R.Room_View == RoomView)
                select new { Room_Type = R.Room_Type }).Distinct();
            if (collection.Count() > 0)
            {
                return base.Json(collection.ToList(), JsonRequestBehavior.AllowGet);
            }
            return base.Json(collection.Count(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRoomViews(string PackageType, string PackageName)
        {
            var collection = (
                from R in this.obj.NC_TBL_PACKAGE_MASTER
                where (R.PackageType == PackageType) && (R.PackageName == PackageName)
                select new { Room_View = R.Room_View }).Distinct();
            if (collection.Count() > 0)
            {
                return base.Json(collection.ToList(), JsonRequestBehavior.AllowGet);
            }
            return base.Json(collection.Count(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult HBDetails(string PRNum)
        {
            IQueryable<HabitModel> nCTBLHABITDETAILS =
                from c in this.obj.NC_TBL_HABIT_DETAILS
                join p in this.obj.NC_TBL_HBQuest_master on c.HBQCode equals p.HBCode
                where c.PRNO == PRNum
                orderby c.HBQCode
                select new HabitModel()
                {
                    Id = c.Id,
                    HBCode = c.HBQCode,
                    HBQFreq = (c.HBQFreq == "0" ? "None" : (c.HBQFreq == "1" ? "Daily" : "Occasionally")),
                    HBDesc = c.HBDesc,
                    HBQuestion = p.HBQuestion
                };
            return this.PartialView("PRHabitDetails", nCTBLHABITDETAILS.ToList<HabitModel>());
        }
        public ActionResult HLDetails(string PRNum)
        {
            IQueryable<HDModel> nCTBLHEALTHDETAILS =
                from c in this.obj.NC_TBL_HEALTH_DETAILS
                join p in this.obj.NC_TBL_HLQuest_master on c.HLCode equals p.HLCode
                where c.PRNO == PRNum
                orderby c.HLCode
                select new HDModel()
                {
                    Id = c.Id,
                    HLCode = c.HLCode,
                    HLFlag = (c.HLFlag == "Y" ? "Yes" : "No"),
                    HLDesc = c.HLDesc,
                    HLQuestion = p.HLQuestion
                };
            return this.PartialView("PRHealthDetails", nCTBLHEALTHDETAILS.ToList<HDModel>());
        }
        public ActionResult Index(string uname)
        {
            ((dynamic)base.ViewBag).Prno = uname;
            string str = base.getdetails(uname);
            char[] chrArray = new char[] { ',' };
            string str1 = str.Split(chrArray)[0];
            char[] chrArray1 = new char[] { ',' };
            string str2 = str.Split(chrArray1)[1];
            char[] chrArray2 = new char[] { ',' };
            string str3 = str.Split(chrArray2)[2];
            if ((
                from x in this.obj.NC_TBL_BOOKED_DATES
                where x.PRNO == str2
                select x).FirstOrDefault<NC_TBL_BOOKED_DATES>() != null)
            {
                base.TempData["accompany"] = "1".ToString();
                base.TempData.Keep("accompany");
            }
            if (str1.ToString() == "Main Guest")
            {
                ((dynamic)base.ViewBag).protypename = str1;
                ((dynamic)base.ViewBag).oppprnumber = str2;
                ((dynamic)base.ViewBag).oppNamevalue = str3;
            }
            else if (str1.ToString() == "Accompany Guest")
            {
                ((dynamic)base.ViewBag).protypename = str1;
                ((dynamic)base.ViewBag).oppprnumber = str2;
                ((dynamic)base.ViewBag).oppNamevalue = str3;
            }
            return base.View();
        }
        public ActionResult PackageDetails(string PRNum)
        {
            BookedDatesCustomized bookedDatesCustomized = (
                from packge in this.obj.NC_TBL_BOOKED_DATES
                where packge.PRNO == PRNum
                select new BookedDatesCustomized()
                {
                    PRNO = packge.PRNO,
                    PackageType = packge.PackageType,
                    PackageName = packge.PackageName,
                    NoOfDays = (int)packge.NoOfDays,
                    ArrivalDate = packge.ArrivalDate,
                    DepartureDate = packge.DepartureDate,
                    AlternateArrivalDate = packge.AlternateArrivalDate,
                    AlternateDepartureDate = packge.AlternateDepartureDate,
                    AccompanyingGuests = (int?)((int)packge.AccompanyingGuests),
                    RoomView = packge.RoomView,
                    RoomType = packge.RoomType,
                    RoomTariff = packge.RoomTariff,
                    ConfirmedFromDate = packge.ConfirmedFromDate,
                    ConfirmedToDate = packge.ConfirmedToDate,
                    DoctorsComments = packge.DoctorsComments,
                    PackageConfirmStatus = packge.PackageConfirmStatus,
                    Package_Amount = packge.Package_Amount,
                    Id = packge.Id
                }).FirstOrDefault<BookedDatesCustomized>();
            return this.PartialView("PRPackageDetails", bookedDatesCustomized);
        }
        public ActionResult PersonalDetails(string PRNum)
        {
            Personal_detailscustomized personalDetailscustomized = (
                from personal in this.obj.NC_TBL_PERSONAL_DETAILS
                where personal.PRNO == PRNum
                select new Personal_detailscustomized()
                {
                    Id = personal.Id,
                    UHID = personal.UHID,
                    PRNO = personal.PRNO,
                    FirstName = personal.FirstName,
                    MiddleName = personal.MiddleName,
                    LastName = personal.LastName,
                    FatherHusbandName = personal.FatherHusbandName,
                    Gender = (personal.Gender == "M" ? "Male" : "Female"),
                    MaritalStatus = (personal.MaritalStatus == "S" ? "Single" : "Married"),
                    FlatNo = personal.FlatNo,
                    Building = personal.Building,
                    Street = personal.Street,
                    City = personal.City,
                    PinCode = personal.PinCode,
                    Nationality = personal.Nationality,
                    Country = personal.Country,
                    DOB = personal.DOB,
                    Age = (int)personal.Age,
                    Height = (int)personal.Height,
                    Weight = (int)personal.Weight,
                    MobileNo = personal.MobileNo,
                    EmailId = personal.EmailId,
                    Occupation = personal.Occupation,
                    AdmissionDate = personal.AdmissionDate,
                    EmergencyContactNo = personal.EmergencyContactNo,
                    BP = personal.BP,
                    PulseRate = (int)personal.PulseRate,
                    ReferralFrom = personal.ReferralFrom,
                    ReferralPRNO = personal.ReferralPRNO,
                    AdmissionStatus = personal.AdmissionStatus,
                    User_Type = personal.User_Type,
                    CheckinProcess = personal.CheckinProcess,
                    AssignedDoctor = personal.AssignedDoctor
                }).FirstOrDefault<Personal_detailscustomized>();
            return this.PartialView("PRPersonalDetails", personalDetailscustomized);
        }
        public JsonResult Update(NC_TBL_BOOKED_DATES b, string Chekboxchecked, string Radio)
        {
            decimal? packageAmount;
            decimal? nullable;
            decimal? nullable1;
            decimal? nullable2;
            decimal? nullable3;
            decimal? nullable4;
            decimal? nullable5;
            decimal? nullable6;
            decimal? nullable7;
            decimal? nullable8;
            decimal? nullable9;
            decimal? nullable10;
            decimal? nullable11;
            decimal? nullable12;
            decimal? nullable13;
            decimal? nullable14;
            decimal? nullable15;
            decimal? nullable16;
            decimal num = new decimal(0);
            NC_TBL_BOOKED_DATES confirmedFromDate = this.obj.NC_TBL_BOOKED_DATES.First<NC_TBL_BOOKED_DATES>((NC_TBL_BOOKED_DATES x) => x.PRNO == b.PRNO);
            (
                from x in this.obj.NC_TBL_BOOKED_DATES
                where (x.ConfirmedToDate == b.ConfirmedToDate) && (x.PRNO == b.PRNO)
                select x).FirstOrDefault<NC_TBL_BOOKED_DATES>();
            NC_TBL_PERSONAL_DETAILS dateTime = (
                from x in this.obj.NC_TBL_PERSONAL_DETAILS
                where x.PRNO == b.PRNO
                select x).FirstOrDefault<NC_TBL_PERSONAL_DETAILS>();
            if (Chekboxchecked != "" && Chekboxchecked != null)
            {
                confirmedFromDate.CMOEscalated = "E";
                confirmedFromDate.PackageConfirmStatus = "N";
                confirmedFromDate.ConfirmedFromDate = b.ConfirmedFromDate;
                confirmedFromDate.ConfirmedToDate = b.ConfirmedToDate;
                confirmedFromDate.DoctorsComments = b.DoctorsComments;
                confirmedFromDate.SuggestDays = b.SuggestDays;
                confirmedFromDate.PackageType = b.PackageType;
                confirmedFromDate.PackageName = b.PackageName;
                confirmedFromDate.RoomView = b.RoomView;
                confirmedFromDate.RoomType = b.RoomType;
                dateTime.AdmissionDate = Convert.ToDateTime(b.ConfirmedFromDate);
                if (base.TempData["accompany"] == null)
                {
                    var variable = (
                        from R in this.obj.NC_TBL_PACKAGE_MASTER
                        where (R.PackageType == b.PackageType) && (R.PackageName == b.PackageName) && (R.Room_View == b.RoomView) && (R.Room_Type == b.RoomType) && (b.ConfirmedFromDate >= (DateTime?)R.EffetedFrom) && (b.ConfirmedFromDate <= (DateTime?)R.EffetedTo)
                        select new { PackageAmount = R.SO_Calc_Amt, RoomTariff = R.SO_Pkg_Amt }).FirstOrDefault();
                    confirmedFromDate.Package_Amount = new decimal?(variable.PackageAmount);
                    confirmedFromDate.RoomTariff = variable.RoomTariff;
                }
                else
                {
                    var variable1 = (
                        from R in this.obj.NC_TBL_PACKAGE_MASTER
                        where (R.PackageType == b.PackageType) && (R.PackageName == b.PackageName) && (R.Room_View == b.RoomView) && (R.Room_Type == b.RoomType) && (b.ConfirmedFromDate >= (DateTime?)R.EffetedFrom) && (b.ConfirmedFromDate <= (DateTime?)R.EffetedTo)
                        select new { PackageAmount = R.DO_Calc_Amt, RoomTariff = R.DO_Pkg_Amt }).FirstOrDefault();
                    confirmedFromDate.Package_Amount = new decimal?(variable1.PackageAmount);
                    confirmedFromDate.RoomTariff = variable1.RoomTariff;
                }
                this.obj.SaveChanges();
                return base.Json(new { Value = "Success" }, JsonRequestBehavior.AllowGet);
            }
            bool flag = false;
            if (Radio != "Rejection")
            {
                confirmedFromDate.PackageConfirmStatus = "Y";
                confirmedFromDate.ConfirmedFromDate = b.ConfirmedFromDate;
                confirmedFromDate.ConfirmedToDate = b.ConfirmedToDate;
                confirmedFromDate.DoctorsComments = b.DoctorsComments;
                confirmedFromDate.SuggestDays = b.SuggestDays;
                confirmedFromDate.PackageType = b.PackageType;
                confirmedFromDate.PackageName = b.PackageName;
                confirmedFromDate.RoomView = b.RoomView;
                confirmedFromDate.RoomType = b.RoomType;
                dateTime.AdmissionDate = Convert.ToDateTime(b.ConfirmedFromDate);
                if (base.TempData["accompany"] == null)
                {
                    var variable2 = (
                        from R in this.obj.NC_TBL_PACKAGE_MASTER
                        where (R.PackageType == b.PackageType) && (R.PackageName == b.PackageName) && (R.Room_View == b.RoomView) && (R.Room_Type == b.RoomType) && (b.ConfirmedFromDate >= (DateTime?)R.EffetedFrom) && (b.ConfirmedFromDate <= (DateTime?)R.EffetedTo)
                        select new { PackageAmount = R.SO_Calc_Amt, RoomTariff = R.SO_Pkg_Amt }).FirstOrDefault();
                    confirmedFromDate.Package_Amount = new decimal?(variable2.PackageAmount);
                    confirmedFromDate.RoomTariff = variable2.RoomTariff;
                }
                else
                {
                    var variable3 = (
                        from R in this.obj.NC_TBL_PACKAGE_MASTER
                        where (R.PackageType == b.PackageType) && (R.PackageName == b.PackageName) && (R.Room_View == b.RoomView) && (R.Room_Type == b.RoomType) && (b.ConfirmedFromDate >= (DateTime?)R.EffetedFrom) && (b.ConfirmedFromDate <= (DateTime?)R.EffetedTo)
                        select new { PackageAmount = R.DO_Calc_Amt, RoomTariff = R.DO_Pkg_Amt }).FirstOrDefault();
                    confirmedFromDate.Package_Amount = new decimal?(variable3.PackageAmount);
                    confirmedFromDate.RoomTariff = variable3.RoomTariff;
                }
                num = confirmedFromDate.RoomTariff * Convert.ToInt32(b.SuggestDays);
                this.obj.SaveChanges();
                string str = b.PRNO.ToString();
                NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE = this.obj.NC_TBL_BOOKED_DATES.FirstOrDefault<NC_TBL_BOOKED_DATES>((NC_TBL_BOOKED_DATES i) => i.PRNO == str);
                if (base.TempData["accompany"] == null)
                {
                    var variable4 = (
                        from bill in this.obj.nc_tbl_bill_type_master
                        where bill.Bill_TYPE_CODE == 3
                        select new { BillTypeCode = bill.Bill_TYPE_CODE, BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : nCTBLBOOKEDDATE.Package_Amount * bill.Fixed_Amount / (decimal?)(new decimal(100))), BillPercentage = (bill.Fixed == "Y" ? (decimal?)(new decimal(0)) : bill.Fixed_Amount) }).FirstOrDefault();
                    packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                    Math.Round(packageAmount.Value, 0);
                    packageAmount = variable4.BillPercentage;
                    Math.Round(packageAmount.Value, 0);
                    packageAmount = variable4.BillAmount;
                    Math.Round(packageAmount.Value, 0);
                    string uniqueno = base.getUniqueno("BILN");
                    NC_TBL_Billing_Trans nCTBLBillingTran = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 2);
                    if (nCTBLBillingTran != null)
                    {
                        nCTBLBillingTran.TOTAL_AMOUNT = nCTBLBOOKEDDATE.Package_Amount;
                        nCTBLBillingTran.NET_AMOUNT = nCTBLBOOKEDDATE.Package_Amount;
                        this.obj.SaveChanges();
                    }
                    else
                    {
                        NC_TBL_Billing_Trans nCTBLBillingTran1 = new NC_TBL_Billing_Trans()
                        {
                            PR_NO = b.PRNO.ToString(),
                            TAX = new decimal?(new decimal(0)),
                            Discount = new decimal?(new decimal(0)),
                            Je_NARRATION = "",
                            Je_AMOUNT = new decimal?(new decimal(0)),
                            TOTAL_AMOUNT = nCTBLBOOKEDDATE.Package_Amount,
                            NET_AMOUNT = nCTBLBOOKEDDATE.Package_Amount,
                            Bill_TYPE_CODE = 2,
                            BillNo = uniqueno,
                            billed_date = new DateTime?(DateTime.Now),
                            Paid_status = "N",
                            MASTER_UPDATED = "N",
                            Staff_code = new int?(0),
                            Remarks = ""
                        };
                        this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran1);
                        this.obj.SaveChanges();
                    }
                    string uniqueno1 = base.getUniqueno("BILN");
                    NC_TBL_Billing_Trans billAmount = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 3);
                    if (billAmount != null)
                    {
                        billAmount.TOTAL_AMOUNT = variable4.BillAmount;
                        billAmount.NET_AMOUNT = variable4.BillAmount;
                        this.obj.SaveChanges();
                    }
                    else
                    {
                        NC_TBL_Billing_Trans nCTBLBillingTran2 = new NC_TBL_Billing_Trans()
                        {
                            PR_NO = b.PRNO.ToString(),
                            TAX = new decimal?(new decimal(0)),
                            Discount = new decimal?(new decimal(0)),
                            Je_NARRATION = "",
                            Je_AMOUNT = new decimal?(new decimal(0)),
                            TOTAL_AMOUNT = variable4.BillAmount,
                            NET_AMOUNT = variable4.BillAmount,
                            Bill_TYPE_CODE = variable4.BillTypeCode,
                            BillNo = uniqueno1,
                            billed_date = new DateTime?(DateTime.Now),
                            Paid_status = "N",
                            MASTER_UPDATED = "N",
                            Staff_code = new int?(0),
                            Remarks = ""
                        };
                        this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran2);
                        this.obj.SaveChanges();
                    }
                    NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE1 = (
                        from x in this.obj.NC_TBL_BOOKED_DATES
                        where x.PRNO == b.PRNO
                        select x).FirstOrDefault<NC_TBL_BOOKED_DATES>();
                    string str1 = base.getUniqueno("BILN");
                    if (this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 10 && i.Guest_Attender_ARNO == null) != null)
                    {
                        if (nCTBLBOOKEDDATE1.Accompany_Type != "Attender")
                        {
                            billAmount.TOTAL_AMOUNT = new decimal?(num);
                            billAmount.NET_AMOUNT = new decimal?(num);
                        }
                        else
                        {
                            billAmount.TOTAL_AMOUNT = new decimal?(num / new decimal(2));
                            billAmount.NET_AMOUNT = new decimal?(num / new decimal(2));
                        }
                        this.obj.SaveChanges();
                    }
                    else
                    {
                        NC_TBL_Billing_Trans nCTBLBillingTran3 = new NC_TBL_Billing_Trans()
                        {
                            PR_NO = b.PRNO.ToString(),
                            TAX = new decimal?(new decimal(0)),
                            Discount = new decimal?(new decimal(0)),
                            Je_NARRATION = "",
                            Je_AMOUNT = new decimal?(new decimal(0))
                        };
                        if (nCTBLBOOKEDDATE1.Accompany_Type != "Attender")
                        {
                            nCTBLBillingTran3.TOTAL_AMOUNT = new decimal?(num);
                            nCTBLBillingTran3.NET_AMOUNT = new decimal?(num);
                        }
                        else
                        {
                            nCTBLBillingTran3.TOTAL_AMOUNT = new decimal?(num / new decimal(2));
                            nCTBLBillingTran3.NET_AMOUNT = new decimal?(num / new decimal(2));
                        }
                        nCTBLBillingTran3.Bill_TYPE_CODE = 10;
                        nCTBLBillingTran3.BillNo = str1;
                        nCTBLBillingTran3.billed_date = new DateTime?(DateTime.Now);
                        nCTBLBillingTran3.Paid_status = "N";
                        nCTBLBillingTran3.MASTER_UPDATED = "N";
                        nCTBLBillingTran3.Staff_code = new int?(0);
                        nCTBLBillingTran3.Remarks = "";
                        this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran3);
                        this.obj.SaveChanges();
                    }
                }
                else
                {
                    NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE2 = this.obj.NC_TBL_BOOKED_DATES.FirstOrDefault<NC_TBL_BOOKED_DATES>((NC_TBL_BOOKED_DATES i) => i.AccompanyingGuestPRNO == str);
                    NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE3 = this.obj.NC_TBL_BOOKED_DATES.FirstOrDefault<NC_TBL_BOOKED_DATES>((NC_TBL_BOOKED_DATES i) => i.PRNO == str);
                    if (nCTBLBOOKEDDATE2 == null)
                    {
                        var variable5 = (
                            from bill in this.obj.nc_tbl_bill_type_master
                            where bill.Bill_TYPE_CODE == 3
                            select new { BillTypeCode = bill.Bill_TYPE_CODE, BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : nCTBLBOOKEDDATE.Package_Amount * bill.Fixed_Amount / (decimal?)(new decimal(100))), BillPercentage = (bill.Fixed == "Y" ? (decimal?)(new decimal(0)) : bill.Fixed_Amount) }).FirstOrDefault();
                        packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                        Math.Round(packageAmount.Value, 0);
                        packageAmount = variable5.BillPercentage;
                        Math.Round(packageAmount.Value, 0);
                        packageAmount = variable5.BillAmount;
                        Math.Round(packageAmount.Value, 0);
                        string uniqueno2 = base.getUniqueno("BILN");
                        NC_TBL_Billing_Trans nCTBLBillingTran4 = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 2);
                        if (nCTBLBillingTran4 != null)
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran5 = nCTBLBillingTran4;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable1 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable1 = nullable;
                            }
                            nCTBLBillingTran5.TOTAL_AMOUNT = nullable1;
                            NC_TBL_Billing_Trans nCTBLBillingTran6 = nCTBLBillingTran4;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable2 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable2 = nullable;
                            }
                            nCTBLBillingTran6.NET_AMOUNT = nullable2;
                            this.obj.SaveChanges();
                        }
                        else
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran7 = new NC_TBL_Billing_Trans()
                            {
                                PR_NO = b.PRNO.ToString(),
                                TAX = new decimal?(new decimal(0)),
                                Discount = new decimal?(new decimal(0)),
                                Je_NARRATION = "",
                                Je_AMOUNT = new decimal?(new decimal(0))
                            };
                            NC_TBL_Billing_Trans nCTBLBillingTran8 = nCTBLBillingTran7;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable15 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable15 = nullable;
                            }
                            nCTBLBillingTran8.TOTAL_AMOUNT = nullable15;
                            NC_TBL_Billing_Trans nCTBLBillingTran9 = nCTBLBillingTran7;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable16 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable16 = nullable;
                            }
                            nCTBLBillingTran9.NET_AMOUNT = nullable16;
                            nCTBLBillingTran7.Bill_TYPE_CODE = 2;
                            nCTBLBillingTran7.BillNo = uniqueno2;
                            nCTBLBillingTran7.billed_date = new DateTime?(DateTime.Now);
                            nCTBLBillingTran7.Paid_status = "N";
                            nCTBLBillingTran7.MASTER_UPDATED = "N";
                            nCTBLBillingTran7.Staff_code = new int?(0);
                            nCTBLBillingTran7.Remarks = "";
                            this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran7);
                            this.obj.SaveChanges();
                        }
                        base.getUniqueno("BILN");
                        NC_TBL_Billing_Trans nCTBLBillingTran10 = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == nCTBLBOOKEDDATE3.AccompanyingGuestPRNO) && i.Bill_TYPE_CODE == 2);
                        if (nCTBLBillingTran10 != null)
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran11 = nCTBLBillingTran10;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable3 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable3 = nullable;
                            }
                            nCTBLBillingTran11.TOTAL_AMOUNT = nullable3;
                            NC_TBL_Billing_Trans nCTBLBillingTran12 = nCTBLBillingTran10;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable4 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable4 = nullable;
                            }
                            nCTBLBillingTran12.NET_AMOUNT = nullable4;
                            this.obj.SaveChanges();
                        }
                        else
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran13 = new NC_TBL_Billing_Trans()
                            {
                                PR_NO = nCTBLBOOKEDDATE3.AccompanyingGuestPRNO,
                                TAX = new decimal?(new decimal(0)),
                                Discount = new decimal?(new decimal(0)),
                                Je_NARRATION = "",
                                Je_AMOUNT = new decimal?(new decimal(0))
                            };
                            NC_TBL_Billing_Trans nCTBLBillingTran14 = nCTBLBillingTran13;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable13 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable13 = nullable;
                            }
                            nCTBLBillingTran14.TOTAL_AMOUNT = nullable13;
                            NC_TBL_Billing_Trans nCTBLBillingTran15 = nCTBLBillingTran13;
                            packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                            if (packageAmount.HasValue)
                            {
                                nullable14 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable14 = nullable;
                            }
                            nCTBLBillingTran15.NET_AMOUNT = nullable14;
                            nCTBLBillingTran13.Bill_TYPE_CODE = 2;
                            nCTBLBillingTran13.BillNo = uniqueno2;
                            nCTBLBillingTran13.billed_date = new DateTime?(DateTime.Now);
                            nCTBLBillingTran13.Paid_status = "N";
                            nCTBLBillingTran13.MASTER_UPDATED = "N";
                            nCTBLBillingTran13.Staff_code = new int?(0);
                            nCTBLBillingTran13.Remarks = "";
                            this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran13);
                            this.obj.SaveChanges();
                        }
                        string str2 = base.getUniqueno("BILN");
                        NC_TBL_Billing_Trans nCTBLBillingTran16 = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 3);
                        if (nCTBLBillingTran16 != null)
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran17 = nCTBLBillingTran16;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable5 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable5 = nullable;
                            }
                            nCTBLBillingTran17.TOTAL_AMOUNT = nullable5;
                            NC_TBL_Billing_Trans nCTBLBillingTran18 = nCTBLBillingTran16;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable6 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable6 = nullable;
                            }
                            nCTBLBillingTran18.NET_AMOUNT = nullable6;
                            this.obj.SaveChanges();
                        }
                        else
                        {
                            NC_TBL_Billing_Trans billTypeCode = new NC_TBL_Billing_Trans()
                            {
                                PR_NO = b.PRNO.ToString(),
                                TAX = new decimal?(new decimal(0)),
                                Discount = new decimal?(new decimal(0)),
                                Je_NARRATION = "",
                                Je_AMOUNT = new decimal?(new decimal(0))
                            };
                            NC_TBL_Billing_Trans nCTBLBillingTran19 = billTypeCode;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable11 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable11 = nullable;
                            }
                            nCTBLBillingTran19.TOTAL_AMOUNT = nullable11;
                            NC_TBL_Billing_Trans nCTBLBillingTran20 = billTypeCode;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable12 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable12 = nullable;
                            }
                            nCTBLBillingTran20.NET_AMOUNT = nullable12;
                            billTypeCode.Bill_TYPE_CODE = variable5.BillTypeCode;
                            billTypeCode.BillNo = str2;
                            billTypeCode.billed_date = new DateTime?(DateTime.Now);
                            billTypeCode.Paid_status = "N";
                            billTypeCode.MASTER_UPDATED = "N";
                            billTypeCode.Staff_code = new int?(0);
                            billTypeCode.Remarks = "";
                            this.obj.NC_TBL_Billing_Trans.Add(billTypeCode);
                            this.obj.SaveChanges();
                        }
                        base.getUniqueno("BILN");
                        NC_TBL_Billing_Trans nCTBLBillingTran21 = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == nCTBLBOOKEDDATE3.AccompanyingGuestPRNO) && i.Bill_TYPE_CODE == 3);
                        if (nCTBLBillingTran21 != null)
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran22 = nCTBLBillingTran21;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable7 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable7 = nullable;
                            }
                            nCTBLBillingTran22.TOTAL_AMOUNT = nullable7;
                            NC_TBL_Billing_Trans nCTBLBillingTran23 = nCTBLBillingTran21;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable8 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable8 = nullable;
                            }
                            nCTBLBillingTran23.NET_AMOUNT = nullable8;
                            this.obj.SaveChanges();
                        }
                        else
                        {
                            NC_TBL_Billing_Trans billTypeCode1 = new NC_TBL_Billing_Trans()
                            {
                                PR_NO = nCTBLBOOKEDDATE3.AccompanyingGuestPRNO,
                                TAX = new decimal?(new decimal(0)),
                                Discount = new decimal?(new decimal(0)),
                                Je_NARRATION = "",
                                Je_AMOUNT = new decimal?(new decimal(0))
                            };
                            NC_TBL_Billing_Trans nCTBLBillingTran24 = billTypeCode1;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable9 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable9 = nullable;
                            }
                            nCTBLBillingTran24.TOTAL_AMOUNT = nullable9;
                            NC_TBL_Billing_Trans nCTBLBillingTran25 = billTypeCode1;
                            packageAmount = variable5.BillAmount;
                            if (packageAmount.HasValue)
                            {
                                nullable10 = new decimal?(packageAmount.GetValueOrDefault() / new decimal(2));
                            }
                            else
                            {
                                nullable = null;
                                nullable10 = nullable;
                            }
                            nCTBLBillingTran25.NET_AMOUNT = nullable10;
                            billTypeCode1.Bill_TYPE_CODE = variable5.BillTypeCode;
                            billTypeCode1.BillNo = str2;
                            billTypeCode1.billed_date = new DateTime?(DateTime.Now);
                            billTypeCode1.Paid_status = "N";
                            billTypeCode1.MASTER_UPDATED = "N";
                            billTypeCode1.Staff_code = new int?(0);
                            billTypeCode1.Remarks = "";
                            this.obj.NC_TBL_Billing_Trans.Add(billTypeCode1);
                            this.obj.SaveChanges();
                        }
                        string uniqueno3 = base.getUniqueno("BILN");
                        if (this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 10 && i.Guest_Attender_ARNO == null) != null)
                        {
                            nCTBLBillingTran16.TOTAL_AMOUNT = new decimal?(num / new decimal(2));
                            nCTBLBillingTran16.NET_AMOUNT = new decimal?(num / new decimal(2));
                            this.obj.SaveChanges();
                        }
                        else
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran26 = new NC_TBL_Billing_Trans()
                            {
                                PR_NO = b.PRNO.ToString(),
                                TAX = new decimal?(new decimal(0)),
                                Discount = new decimal?(new decimal(0)),
                                Je_NARRATION = "",
                                Je_AMOUNT = new decimal?(new decimal(0)),
                                TOTAL_AMOUNT = new decimal?(num / new decimal(2)),
                                NET_AMOUNT = new decimal?(num / new decimal(2)),
                                Bill_TYPE_CODE = 10,
                                BillNo = uniqueno3,
                                billed_date = new DateTime?(DateTime.Now),
                                Paid_status = "N",
                                MASTER_UPDATED = "N",
                                Staff_code = new int?(0),
                                Remarks = ""
                            };
                            this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran26);
                            this.obj.SaveChanges();
                        }
                    }
                    else
                    {
                        var variable6 = (
                            from bill in this.obj.nc_tbl_bill_type_master
                            where bill.Bill_TYPE_CODE == 3
                            select new { BillTypeCode = bill.Bill_TYPE_CODE, BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : nCTBLBOOKEDDATE.Package_Amount * bill.Fixed_Amount / (decimal?)(new decimal(100))), BillPercentage = (bill.Fixed == "Y" ? (decimal?)(new decimal(0)) : bill.Fixed_Amount) }).FirstOrDefault();
                        packageAmount = nCTBLBOOKEDDATE.Package_Amount;
                        Math.Round(packageAmount.Value, 0);
                        packageAmount = variable6.BillPercentage;
                        Math.Round(packageAmount.Value, 0);
                        packageAmount = variable6.BillAmount;
                        Math.Round(packageAmount.Value, 0);
                        string str3 = base.getUniqueno("BILN");
                        NC_TBL_Billing_Trans nullable17 = this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 10 && i.Guest_Attender_ARNO == null);
                        if (nullable17 != null)
                        {
                            nullable17.TOTAL_AMOUNT = new decimal?(num / new decimal(2));
                            nullable17.NET_AMOUNT = new decimal?(num / new decimal(2));
                            this.obj.SaveChanges();
                        }
                        else
                        {
                            NC_TBL_Billing_Trans nCTBLBillingTran27 = new NC_TBL_Billing_Trans()
                            {
                                PR_NO = b.PRNO.ToString(),
                                TAX = new decimal?(new decimal(0)),
                                Discount = new decimal?(new decimal(0)),
                                Je_NARRATION = "",
                                Je_AMOUNT = new decimal?(new decimal(0)),
                                TOTAL_AMOUNT = new decimal?(num / new decimal(2)),
                                NET_AMOUNT = new decimal?(num / new decimal(2)),
                                Bill_TYPE_CODE = 10,
                                BillNo = str3,
                                billed_date = new DateTime?(DateTime.Now),
                                Paid_status = "N",
                                MASTER_UPDATED = "N",
                                Staff_code = new int?(0),
                                Remarks = ""
                            };
                            this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran27);
                            this.obj.SaveChanges();
                        }
                    }
                }
                NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE4 = (
                    from x in this.obj.NC_TBL_BOOKED_DATES
                    where x.PRNO == b.PRNO
                    select x).FirstOrDefault<NC_TBL_BOOKED_DATES>();
                if (nCTBLBOOKEDDATE4.Accompany_Type == "Attender")
                {
                    string uniqueno4 = base.getUniqueno("BILN");
                    if (this.obj.NC_TBL_Billing_Trans.FirstOrDefault<NC_TBL_Billing_Trans>((NC_TBL_Billing_Trans i) => (i.PR_NO == str) && i.Bill_TYPE_CODE == 10 && i.Guest_Attender_ARNO != null) == null)
                    {
                        NC_TBL_Billing_Trans nCTBLBillingTran28 = new NC_TBL_Billing_Trans()
                        {
                            PR_NO = b.PRNO.ToString(),
                            TAX = new decimal?(new decimal(0)),
                            Discount = new decimal?(new decimal(0)),
                            Je_NARRATION = "",
                            Je_AMOUNT = new decimal?(new decimal(0)),
                            TOTAL_AMOUNT = new decimal?(num / new decimal(2)),
                            NET_AMOUNT = new decimal?(num / new decimal(2)),
                            Bill_TYPE_CODE = 10,
                            BillNo = uniqueno4,
                            billed_date = new DateTime?(DateTime.Now),
                            Paid_status = "N",
                            MASTER_UPDATED = "N",
                            Staff_code = new int?(0),
                            Remarks = "",
                            Guest_Attender_ARNO = nCTBLBOOKEDDATE4.Guest_Attender_ARNO
                        };
                        this.obj.NC_TBL_Billing_Trans.Add(nCTBLBillingTran28);
                        this.obj.SaveChanges();
                    }
                }
                NC_TBL_PERSONAL_DETAILS nCTBLPERSONALDETAIL = (
                    from x in this.obj.NC_TBL_PERSONAL_DETAILS
                    where x.PRNO == b.PRNO
                    select x).FirstOrDefault<NC_TBL_PERSONAL_DETAILS>();
                (
                    from x in this.obj.NC_TBL_BOOKED_DATES
                    where x.PRNO == b.PRNO
                    select x).FirstOrDefault<NC_TBL_BOOKED_DATES>();
                if (nCTBLPERSONALDETAIL != null)
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            string empty = string.Empty;
            empty = (flag ? "Fail" : "Mail Error");
            return base.Json(new { Value = empty }, JsonRequestBehavior.AllowGet);
        }
    // GET: PackageConfirmation
    public ActionResult Index()
        {
            return View();
        }
    }
}