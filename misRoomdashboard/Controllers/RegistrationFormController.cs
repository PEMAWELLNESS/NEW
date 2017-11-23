using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Net.Mail;
using System.IO;
//using System.Data.Entity.Core.Objects;
using System.Text;
using System.Data.Entity.Validation;
using System.Data.Entity.SqlServer;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Data.Entity.Core.Objects;
using System.Xml;
using System.Globalization;
//using System.Data.Objects;
//using System.Data.Objects;


namespace Rooms.Controllers
{
    public class RegistrationFormController : BaseController
    {
        string mheader = "<div style='border:groove; border-width:medium; width:600px; height:500px;'><div style='width:600px; height:80px; font-weight:bold; background-color:skyblue' > <p style='float:left;  width:300px; margin:0px 0px 0px 0px; padding:0px 0px 0px 0px'>PEMA WELLNESS</p><p style='float:right;   margin:0px 0px 0px 0px; padding:0px 0px 0px 0px '>Vizag<br />Ph: 040-13654789<br /> Email: Support@pemaresort.com</p><br /> </div><div> ";
        string mfooter = "<table style='margin-top:20px; padding-top:20px;'><tr><td>  Thanking You,<br /> <br/> For,<br/>PEMA WELLNESS, Vizag </td> </tr> </table>  </div> </div> ";

        StringBuilder mbody;


        //DynamicMenuEntities db = new DynamicMenuEntities();
        private MisRoomsEntities db = new MisRoomsEntities();
        // GET: /Registrationform/
        [HttpGet]
        public ActionResult PersonalDetails(string PRNO, string UHID)
        {
            if (PRNO != null && PRNO != "")
            {
                Session["TMPPR"] = PRNO;
                Session["TMPUH"] = UHID;
            }
            //Session["PRNO"] = "15021900001";
            var PR = Session["TMPPR"].ToString();
            ViewBag.PRNO = Session["TMPPR"].ToString();
            ViewBag.UHID = Session["TMPUH"].ToString();

            var pp = Session["TMPPR"].ToString();
            var cc = Session["TMPUH"].ToString(); ;


            ViewBag.Countries = new SelectList(db.NC_TBL_COUNTRY_MASTER, "CountryCode", "CountryName");
            ViewBag.Nationalities = new SelectList(db.NC_TBL_NATIONALITY_MASTER, "NationalityCode", "NationalityName");


            List<SelectListItem> obj1 = new List<SelectListItem>();
            obj1.Add(new SelectListItem { Text = "Male", Value = "M" });
            obj1.Add(new SelectListItem { Text = "Female", Value = "F" });
            ViewBag.Gendr = obj1;

            List<SelectListItem> obj2 = new List<SelectListItem>();
            obj2.Add(new SelectListItem { Text = "Single", Value = "S" });
            obj2.Add(new SelectListItem { Text = "Married", Value = "M" });
            ViewBag.MaritalStat = obj2;



            var PRDetails = db.NC_TBL_PERSONAL_DETAILS.Where(a => a.PRNO == PR).FirstOrDefault();

            if (PRDetails != null)
            {

                return View(PRDetails);
            }
            else
            {
                return View();
            }


        }

        public ActionResult GetNONAvailability(string TDate, string RoomView, string RoomType)
        {
            //string strDateStarted = TDate;
            //DateTime datDateStarted;
            //DateTime.TryParseExact(strDateStarted, new string[] { "ddd MMM dd HH:mm:ss yyyy" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out datDateStarted);
            // var stod = TDate;
            //stod = stod.Substring(4, 1) + "/" + stod.Substring(0, 2) + "/" + stod.Substring(6, 4);
            string[] str1 = TDate.Split('/');

            int v_dd = 0, v_mm = 0, v_yyyy = 0;

            v_dd = System.Convert.ToInt32(str1[1].PadLeft(2, '0'));
            v_mm = System.Convert.ToInt32(str1[0].PadLeft(2, '0'));
            v_yyyy = System.Convert.ToInt32(str1[2]);

            //DateTime datDateStarted = DateTime.ParseExact(TDate, "ddd MMM dd HH:mm:ss yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            DateTime datDateStarted;
            IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
            datDateStarted = DateTime.Parse(TDate, culture);
            //datDateStarted = DateTime.ParseExact(TDate, "yyyy/MM/dd", null);

            var changeddate = datDateStarted.AddMonths(2);
            List<string> NonAvailablity = new List<string>();
            List<string> Availability = new List<string>();

            for (DateTime date = datDateStarted; date.Date <= changeddate.Date; date = date.AddDays(1))
            {
                var RoomList1 = (from RoomMaster in db.NC_TBL_ROOM_MASTER
                                 from RoomTrans in db.NC_TBL_GUEST_ROOM_TRANSACTION
                                 where RoomMaster.Active == "Y" && RoomMaster.Room_View == RoomView && RoomMaster.Room_Type == RoomType && RoomTrans.Room_No == RoomMaster.Room_No && RoomTrans.Status != "Cleaned" && (date >= RoomTrans.ArrivalDate && date <= RoomTrans.DepartureDate)
                                 select RoomTrans.Room_No).ToList();

                if (RoomList1.Count() > 0 && RoomList1.Count() == db.NC_TBL_ROOM_MASTER.Where(i => i.Active == "Y" && i.Room_View == RoomView && i.Room_Type == RoomType).Count())
                {
                    ViewBag.data = 1;
                    NonAvailablity.Add(date.ToString("dd/MM/yyyy"));
                }
                else if (RoomList1.Count() > 0)
                {
                    ViewBag.data = 0;
                    Availability.Add(date.ToString("dd/MM/yyyy"));
                }

                else if (RoomList1.Count() == 0 && RoomList1.Count() != db.NC_TBL_ROOM_MASTER.Where(i => i.Active == "Y" && i.Room_View == RoomView && i.Room_Type == RoomType).Count())
                {

                    ViewBag.data = 0;
                    Availability.Add(date.ToString("dd/MM/yyyy"));
                }
                //else if (RoomList1.Count() == 0 &&  db.NC_TBL_ROOM_MASTER.Where(i => i.Active == "Y" && i.Room_View == RoomView && i.Room_Type == RoomType).Count() ==0)
                //{
                //    ViewBag.data = 1;
                //    NonAvailablity.Add(date.ToString("dd/MM/yyyy"));

                //}
                //  Availability.Add(date.ToString("dd/MM/yyyy"));
            }




            return Json(String.Join(",", NonAvailablity) + "|" + String.Join(",", Availability), JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult PersonalDetails(NC_TBL_PERSONAL_DETAILS nc_tbl_Personal_Details)
        {



            //var recordUHID = (from user in db.NC_TBL_PERSONAL_DETAILS select user.UHID).ToList();
            //if (recordUHID.Count == 0)
            //{
            //    ViewBag.UHID = 1;
            //}
            //else
            //{
            //    ViewBag.UHID =Convert.ToInt32((from user in db.NC_TBL_PERSONAL_DETAILS select user.UHID).Max())+ 1;
            //}
            nc_tbl_Personal_Details.PRNO = Session["TMPPR"].ToString();
            nc_tbl_Personal_Details.UHID = Session["TMPUH"].ToString();


            ViewBag.Countries = new SelectList(db.NC_TBL_COUNTRY_MASTER, "CountryCode", "CountryName");
            ViewBag.Nationalities = new SelectList(db.NC_TBL_NATIONALITY_MASTER, "NationalityCode", "NationalityName");





            List<SelectListItem> obj1 = new List<SelectListItem>();
            obj1.Add(new SelectListItem { Text = "Male", Value = "M" });
            obj1.Add(new SelectListItem { Text = "Female", Value = "F" });
            ViewBag.Gendr = obj1;

            List<SelectListItem> obj2 = new List<SelectListItem>();
            obj2.Add(new SelectListItem { Text = "Single", Value = "S" });
            obj2.Add(new SelectListItem { Text = "Married", Value = "M" });
            ViewBag.MaritalStat = obj2;



            var Bookinginfo = db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == nc_tbl_Personal_Details.PRNO);

            // nc_tbl_Personal_Details.AdmissionDate = DateTime.Now.Date;
            nc_tbl_Personal_Details.AdmissionDate = Bookinginfo.ArrivalDate;
            nc_tbl_Personal_Details.AdmissionStatus = "Doctor";
            nc_tbl_Personal_Details.User_Type = "Webuser";
            nc_tbl_Personal_Details.CheckinProcess = "N";
            nc_tbl_Personal_Details.CheckoutProcess = "N";
            nc_tbl_Personal_Details.DischargeType = "NORMAL";
            nc_tbl_Personal_Details.PStatus = "N";
            if (ModelState.IsValid)
            {

                var personal = db.NC_TBL_PERSONAL_DETAILS.FirstOrDefault(i => i.PRNO == nc_tbl_Personal_Details.PRNO);
                try
                {

                    if (personal == null)
                    {
                        var UHIDpersonal = db.NC_TBL_PERSONAL_DETAILS.FirstOrDefault(i => i.UHID == nc_tbl_Personal_Details.UHID);

                        if (UHIDpersonal == null)
                        {
                            nc_tbl_Personal_Details.GuestPriority = "Normal";
                        }
                        else
                        {
                            nc_tbl_Personal_Details.GuestPriority = "Repeated";
                        }
                        db.NC_TBL_PERSONAL_DETAILS.Add(nc_tbl_Personal_Details);
                        db.SaveChanges();
                    }
                    else
                    {
                        NC_TBL_PERSONAL_DETAILS existing = db.NC_TBL_PERSONAL_DETAILS.Find(personal.Id);
                        //existing.UHID = nc_tbl_Personal_Details.UHID;
                        existing.FirstName = nc_tbl_Personal_Details.FirstName;
                        existing.MiddleName = nc_tbl_Personal_Details.MiddleName;
                        existing.LastName = nc_tbl_Personal_Details.LastName;
                        existing.Gender = nc_tbl_Personal_Details.Gender;
                        existing.MaritalStatus = nc_tbl_Personal_Details.MaritalStatus;
                        existing.FlatNo = nc_tbl_Personal_Details.FlatNo;
                        existing.Building = nc_tbl_Personal_Details.Building;
                        existing.Street = nc_tbl_Personal_Details.Street;
                        existing.City = nc_tbl_Personal_Details.City;
                        existing.PinCode = nc_tbl_Personal_Details.PinCode;
                        existing.Nationality = nc_tbl_Personal_Details.Nationality;
                        existing.Country = nc_tbl_Personal_Details.Country;
                        existing.DOB = nc_tbl_Personal_Details.DOB;
                        existing.Age = nc_tbl_Personal_Details.Age;
                        existing.Height = nc_tbl_Personal_Details.Height;
                        existing.Weight = nc_tbl_Personal_Details.Weight;
                        existing.BP = nc_tbl_Personal_Details.BP;
                        existing.PulseRate = nc_tbl_Personal_Details.PulseRate;
                        existing.MobileNo = nc_tbl_Personal_Details.MobileNo;
                        existing.EmergencyContactNo = nc_tbl_Personal_Details.EmergencyContactNo;
                        existing.EmailId = nc_tbl_Personal_Details.EmailId;
                        existing.Occupation = nc_tbl_Personal_Details.Occupation;
                        existing.AdmissionDate = nc_tbl_Personal_Details.AdmissionDate;

                        existing.ReferralFrom = nc_tbl_Personal_Details.ReferralFrom;
                        existing.ReferralPRNO = nc_tbl_Personal_Details.ReferralPRNO;
                        existing.ReferralOthers = nc_tbl_Personal_Details.ReferralOthers;
                        existing.AliasName = nc_tbl_Personal_Details.AliasName;
                        db.SaveChanges();
                    }


                    if (Request["btn"].Equals("Next"))
                    {
                        return RedirectToAction("GetHealthDetails");
                    }
                    else
                    {
                        return RedirectToAction("DatesConformation", new { PRNO = nc_tbl_Personal_Details.PRNO, UHID = nc_tbl_Personal_Details.UHID });
                    }


                }
                //catch (Exception e)
                //{
                //    var res = e.InnerException;

                //    return View(nc_tbl_Personal_Details);
                //}
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                        }

                    }
                    return View(nc_tbl_Personal_Details);
                }


            }
            else
            {
                return View(nc_tbl_Personal_Details);
            }
        }



        public ActionResult GetHealthDetails()
        {
            //var PR = "1234";
            var PR = Session["TMPPR"].ToString();
            var personal = db.NC_TBL_PERSONAL_DETAILS.FirstOrDefault(i => i.PRNO == PR);
            var HeathDetails = db.NC_TBL_HLQuest_master.Where(a => a.Gender == personal.Gender || a.Gender == "B").OrderBy(a => a.HLCode).ToList();
            var HL = db.NC_TBL_HEALTH_DETAILS.Where(a => a.PRNO == PR).ToList();
            if (HL.Count() == 0)
            {
                var HLDetails = new NC_TBL_HEALTH_DETAILS();
                foreach (NC_TBL_HLQuest_master cust in HeathDetails)
                {
                    HLDetails.PRNO = PR;
                    HLDetails.HLCode = cust.HLCode;
                    HLDetails.HLFlag = "N";
                    HLDetails.HLDesc = "";
                    db.NC_TBL_HEALTH_DETAILS.Add(HLDetails);
                    db.SaveChanges();
                }


            }
            else
            {
                var queryHL = (from c in db.NC_TBL_HEALTH_DETAILS
                               join p in db.NC_TBL_HLQuest_master
                               on c.HLCode equals p.HLCode
                               where (c.PRNO == PR) && (p.Gender == personal.Gender)
                               ////where c.Id == p.HLCode
                               orderby c.HLCode
                               select c);
                if (queryHL.Count() == 0)
                {
                    var queryHL1 = (from c in db.NC_TBL_HEALTH_DETAILS
                                    join p in db.NC_TBL_HLQuest_master
                                    on c.HLCode equals p.HLCode
                                    where (c.PRNO == PR) && (p.Gender != "B")
                                    ////where c.Id == p.HLCode
                                    orderby c.HLCode
                                    select c);

                    foreach (NC_TBL_HEALTH_DETAILS cust in queryHL1)
                    {
                        NC_TBL_HEALTH_DETAILS HLD = db.NC_TBL_HEALTH_DETAILS.Find(cust.Id);
                        db.NC_TBL_HEALTH_DETAILS.Remove(HLD);

                    }
                    db.SaveChanges();

                    var HeathDetails1 = db.NC_TBL_HLQuest_master.Where(a => a.Gender == personal.Gender).OrderBy(a => a.HLCode).ToList();

                    var HLDetails = new NC_TBL_HEALTH_DETAILS();
                    foreach (NC_TBL_HLQuest_master cust in HeathDetails1)
                    {
                        HLDetails.PRNO = PR;
                        HLDetails.HLCode = cust.HLCode;
                        HLDetails.HLFlag = "N";
                        HLDetails.HLDesc = "";
                        db.NC_TBL_HEALTH_DETAILS.Add(HLDetails);
                        db.SaveChanges();
                    }
                    //db.SaveChanges();
                }


            }
            return RedirectToAction("GetHabitDetails");


        }
        private static bool customCertValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {

            return true;

        }
        public ActionResult GetHabitDetails()
        {
            //var PR = "1234";
            var PR = Session["TMPPR"].ToString();
            var personal = db.NC_TBL_PERSONAL_DETAILS.FirstOrDefault(i => i.PRNO == PR);
            var HabitDetails = db.NC_TBL_HBQuest_master.Where(a => a.Gender == personal.Gender || a.Gender == "B").OrderBy(a => a.HBCode).ToList();
            var HB = db.NC_TBL_HABIT_DETAILS.Where(a => a.PRNO == PR).ToList();
            if (HB.Count() == 0)
            {
                var HBDetails = new NC_TBL_HABIT_DETAILS();
                foreach (NC_TBL_HBQuest_master cust in HabitDetails)
                {
                    HBDetails.PRNO = PR;
                    HBDetails.HBQCode = cust.HBCode;
                    HBDetails.HBQFreq = "0";
                    HBDetails.HBDesc = "";
                    db.NC_TBL_HABIT_DETAILS.Add(HBDetails);
                    db.SaveChanges();

                }

            }
            else
            {
                var queryHB = (from c in db.NC_TBL_HABIT_DETAILS
                               join p in db.NC_TBL_HBQuest_master
                             on c.HBQCode equals p.HBCode
                               where (c.PRNO == PR) && (p.Gender == personal.Gender)
                               ////where c.Id == p.HLCode
                               orderby c.HBQCode
                               select c);
                if (queryHB.Count() == 0)
                {
                    var queryHB1 = (from c in db.NC_TBL_HABIT_DETAILS
                                    join p in db.NC_TBL_HBQuest_master
                                  on c.HBQCode equals p.HBCode
                                    where (c.PRNO == PR) && (p.Gender != "B")
                                    ////where c.Id == p.HLCode
                                    orderby c.HBQCode
                                    select c);

                    foreach (NC_TBL_HABIT_DETAILS cust in queryHB1)
                    {
                        NC_TBL_HABIT_DETAILS HBD = db.NC_TBL_HABIT_DETAILS.Find(cust.Id);
                        db.NC_TBL_HABIT_DETAILS.Remove(HBD);

                    }
                    db.SaveChanges();

                    var HabitDetails1 = db.NC_TBL_HBQuest_master.Where(a => a.Gender == personal.Gender).OrderBy(a => a.HBCode).ToList();

                    var HBDetails = new NC_TBL_HABIT_DETAILS();
                    foreach (NC_TBL_HBQuest_master cust in HabitDetails1)
                    {
                        HBDetails.PRNO = PR;
                        HBDetails.HBQCode = cust.HBCode;
                        HBDetails.HBQFreq = "0";
                        HBDetails.HBDesc = "";
                        db.NC_TBL_HABIT_DETAILS.Add(HBDetails);
                        db.SaveChanges();

                    }

                }



            }
            return RedirectToAction("HealthDetails");


        }

        //To display Existing UHID Details in Registration Table

        public ActionResult DisplayUHIDDetails()
        {
            var UHIDNO = Session["TMPUH"].ToString();

            var queryResult = from e in db.WS_Tbl_Registration
                              where e.UHID == UHIDNO
                              select e;
            var Record = (from a in db.WS_Tbl_Registration
                          where a.UHID == UHIDNO
                          select new
                          {
                              first_name = a.first_name,
                              middle_name = a.middle_name,
                              last_name = a.last_name,
                              father_husband_name = a.father_husband_name,
                              gender = a.gender,
                              mobile_no = a.mobile_no,
                              user_id = a.user_id,
                              date_of_birth = SqlFunctions.DateName("day", a.date_of_birth).Trim() + "/" + SqlFunctions.StringConvert((double)a.date_of_birth.Month).TrimStart() + "/" + SqlFunctions.DateName("year", a.date_of_birth),


                          }).FirstOrDefault();


            if (Record == null)
            {
                return HttpNotFound();

            }

            return Json(Record, JsonRequestBehavior.AllowGet);

        }


        //To display existing record
        public ActionResult DisplayPersonalDetails(string PRNO)
        {
            var ppNo = PRNO;

            //var queryResult = from e in db.NC_TBL_PERSONAL_DETAILS
            //                  where e.PRNO == PRNO
            //                  select e;
            //var Record = queryResult.FirstOrDefault();
            var Record = (from a in db.NC_TBL_PERSONAL_DETAILS
                          where a.PRNO == PRNO
                          select new
                          {
                              Id = a.Id,
                              FirstName = a.FirstName,
                              MiddleName = a.MiddleName,
                              LastName = a.LastName,
                              FatherHusbandName = a.FatherHusbandName,
                              Gender = a.Gender,
                              MaritalStatus = a.MaritalStatus,
                              AliasName = a.AliasName,
                              FlatNo = a.FlatNo,
                              Building = a.Building,
                              Street = a.Street,
                              City = a.City,
                              PinCode = a.PinCode,
                              Country = a.Country,
                              Nationality = a.Nationality,
                              DOB = SqlFunctions.DateName("day", a.DOB).Trim() + "/" + SqlFunctions.StringConvert((double)a.DOB.Month).TrimStart() + "/" + SqlFunctions.DateName("year", a.DOB),
                              Age = a.Age,
                              Height = a.Height,
                              Weight = a.Weight,
                              BP = a.BP,
                              PulseRate = a.PulseRate,
                              MobileNo = a.MobileNo,
                              EmergencyContactNo = a.EmergencyContactNo,
                              EmailId = a.EmailId,

                              Occupation = a.Occupation,
                              ReferralFrom = a.ReferralFrom,
                              ReferralPRNO = a.ReferralPRNO,
                              ReferralOthers = a.ReferralOthers,

                          }).FirstOrDefault();

            if (Record == null)
            {
                return HttpNotFound();

            }
            NC_TBL_PERSONAL_DETAILS nc_tbl_Personal_Details = db.NC_TBL_PERSONAL_DETAILS.Find(Record.Id);


            ViewBag.Countries = new SelectList(db.NC_TBL_COUNTRY_MASTER, "CountryCode", "CountryName", Record.Country);
            ViewBag.Nationalities = new SelectList(db.NC_TBL_NATIONALITY_MASTER, "NationalityCode", "NationalityName", Record.Nationality);



            List<SelectListItem> obj1 = new List<SelectListItem>();
            obj1.Add(new SelectListItem { Text = "Male", Value = "M" });
            obj1.Add(new SelectListItem { Text = "Female", Value = "F" });
            ViewBag.Gendr = new SelectList(obj1, Record.Gender);
            List<SelectListItem> obj2 = new List<SelectListItem>();
            obj2.Add(new SelectListItem { Text = "Single", Value = "S" });
            obj2.Add(new SelectListItem { Text = "Married", Value = "M" });
            ViewBag.MaritalStat = new SelectList(obj2, Record.MaritalStatus);
            // return View(nc_tbl_Personal_Details);
            return Json(Record, JsonRequestBehavior.AllowGet);
            //  return View(Record.ToString());


        }

        //HealthDetails Screen

        [HttpGet]
        public ActionResult HealthDetails()
        {
            //var PR = prno;
            var PR = Session["TMPPR"].ToString();
            // var PR = "15021900001";
            var personal = db.NC_TBL_PERSONAL_DETAILS.FirstOrDefault(i => i.PRNO == PR);
            ViewBag.Age = personal.Age;

            var BPVal = personal.BP;
            string[] BPHBP = BPVal.Split('/');

            var HBP = Convert.ToInt16(BPHBP[0]);
            var LBP = Convert.ToInt16(BPHBP[1]);

            var ss = (from bp in db.NC_TBL_VITAL_MASTER
                      where bp.vital_name == "BP"
                      select new
                      {
                          MBP = bp.vital_minus_tolerance,
                          PBP = bp.vital_plus_tolerance

                      }).FirstOrDefault();

            var CHBP = ss.MBP.Split('/');
            var CLBP = ss.PBP.Split('/');
            var F1HBP = Convert.ToInt16(CHBP[0]);
            var F1LBP = Convert.ToInt16(CHBP[1]);


            var F2HBP = Convert.ToInt16(CLBP[0]);
            var F2LBP = Convert.ToInt16(CLBP[1]);


            if (((HBP >= F1HBP) && (HBP <= F2HBP)) && ((LBP >= F1LBP) && (LBP <= F2LBP)))
            {

                ViewBag.HBPData = "Normal";

            }
            else
            {
                ViewBag.HBPData = "AbNormal";

            }

            List<SelectListItem> obj = new List<SelectListItem>();
            obj.Add(new SelectListItem { Text = "No", Value = "N" });
            obj.Add(new SelectListItem { Text = "Yes", Value = "Y" });
            ViewBag.Flag = obj;

            var query = (from c in db.NC_TBL_HEALTH_DETAILS
                         join p in db.NC_TBL_HLQuest_master
                         on c.HLCode equals p.HLCode
                         where c.PRNO == PR
                         //where c.Id == p.HLCode
                         orderby c.HLCode
                         select new HDModel
                         {
                             Id = c.Id,
                             HLCode = c.HLCode,
                             HLFlag = c.HLFlag == "Y" ? "Yes" : "No",


                             HLDesc = c.HLDesc,
                             HLQuestion = p.HLQuestion

                         });

            var UHID = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault().UHID;
            List<string> filePaths = new List<string>();
            List<string> filePaths1 = new List<string>();
            var FilePath = Server.MapPath("~/Content/" + UHID + "/" + PR + "/" + "Application" + "");
            if (Directory.Exists(FilePath))
            {
                var pdfpath = Directory.GetFiles(FilePath);

                if (pdfpath != null)
                {
                    foreach (var item in pdfpath)
                    {
                        if (Path.GetFileNameWithoutExtension(item).StartsWith("BR"))
                        {
                            filePaths.Add(Path.GetFileNameWithoutExtension(item));
                        }
                        else if (Path.GetFileNameWithoutExtension(item).StartsWith("ECG"))
                        {
                            filePaths1.Add(Path.GetFileNameWithoutExtension(item));
                        }

                    }
                }
            }

            TempData["BRFiles"] = String.Join(",", filePaths);
            TempData.Keep("BRFiles");
            TempData["ECGFiles"] = String.Join(",", filePaths1);
            TempData.Keep("ECGFiles");
            return View(query.ToList());
        }


        [HttpPost]
        public ActionResult HealthDetails(List<NC_TBL_HEALTH_DETAILS> HealthDetails, HttpPostedFileBase[] files, HttpPostedFileBase[] ECGfiles)
        {

            var PR = Session["TMPPR"].ToString();
            foreach (NC_TBL_HEALTH_DETAILS cust in HealthDetails)
            {
                NC_TBL_HEALTH_DETAILS existing = db.NC_TBL_HEALTH_DETAILS.Find(cust.Id);

                if (cust.HLFlag != null)
                {
                    existing.HLDesc = cust.HLDesc;
                    existing.HLFlag = cust.HLFlag;
                }
                else
                {
                    existing.HLDesc = cust.HLDesc;
                }
            }
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    string filepath = System.IO.Path.GetExtension(file.FileName);
                    string filename = "BR_Application" + filepath;
                    var uhid = db.NC_TBL_PERSONAL_DETAILS.Where(x => x.PRNO == PR).FirstOrDefault();
                    if (uhid != null)
                    {
                        var var1 = uhid.UHID;
                        var var2 = PR;
                        var var3 = "Application";
                        //string strMappath = "~/Content/Images/" + var1 + "/" + var2 + "/" + filename;
                        string strMappath = var1 + "/" + var2 + "/" + var3;// +"/" + filename;
                        var path = Server.MapPath("../Content/" + strMappath);
                        if (Directory.Exists(path))
                        {
                            foreach (var item in Directory.GetFiles(path))
                            {
                                if (Path.GetFileNameWithoutExtension(item).StartsWith("BR"))
                                {
                                    System.IO.File.Delete(Path.GetFileNameWithoutExtension(item));
                                }
                            }
                        }
                        Directory.CreateDirectory(path);

                        //file.SaveAs(Server.MapPath(path +"\\"+ filename));                 
                        file.SaveAs(Server.MapPath("../Content/" + strMappath) + "/" + filename);

                    }
                    TempData["BRFiles"] = file.FileName;
                    TempData.Keep("BRFiles");
                }
            }

            foreach (HttpPostedFileBase file in ECGfiles)
            {
                if (file != null)
                {
                    string filepath = System.IO.Path.GetFileName(file.FileName);
                    string filename = "ECG_Application" + filepath;
                    var uhid = db.NC_TBL_PERSONAL_DETAILS.Where(x => x.PRNO == PR).FirstOrDefault();
                    if (uhid != null)
                    {
                        var var1 = uhid.UHID;
                        var var2 = PR;
                        var var3 = "Application";
                        //string strMappath = "~/Content/Images/" + var1 + "/" + var2 + "/" + filename;
                        string strMappath = var1 + "/" + var2 + "/" + var3;// +"/" + filename;
                        var path = Server.MapPath("../Content/" + strMappath);
                        if (Directory.Exists(path))
                        {
                            foreach (var item in Directory.GetFiles(path))
                            {
                                if (Path.GetFileNameWithoutExtension(item).StartsWith("ECG"))
                                {
                                    System.IO.File.Delete(Path.GetFileNameWithoutExtension(item));
                                }
                            }
                        }
                        Directory.CreateDirectory(path);

                        //file.SaveAs(Server.MapPath(path +"\\"+ filename));                 
                        file.SaveAs(Server.MapPath("../Content/" + strMappath) + "/" + filename);

                    }
                    TempData["ECGFiles"] = file.FileName;
                    TempData.Keep("ECGFiles");
                }
            }


            db.SaveChanges();



            return RedirectToAction("HabitDetails");

        }

        //HabitDetails Screen
        public ActionResult HabitDetails()
        {
            var PR = Session["TMPPR"].ToString();
            TempData.Keep("BRFiles");
            TempData.Keep("ECGFiles");
            List<SelectListItem> obj = new List<SelectListItem>();
            obj.Add(new SelectListItem { Text = "None", Value = "0" });
            obj.Add(new SelectListItem { Text = "Daily", Value = "1" });
            obj.Add(new SelectListItem { Text = "Occasionally", Value = "2" });
            ViewBag.Frequency = obj;

            var query = (from c in db.NC_TBL_HABIT_DETAILS
                         join p in db.NC_TBL_HBQuest_master
                         on c.HBQCode equals p.HBCode
                         where c.PRNO == PR
                         orderby c.HBQCode
                         select new HabitModel
                         {
                             Id = c.Id,
                             HBCode = c.HBQCode,
                             HBQFreq = c.HBQFreq == "0" ? "None" :
                             c.HBQFreq == "1" ? "Daily" : "Occasionally",
                             HBDesc = c.HBDesc,
                             HBQuestion = p.HBQuestion,
                         });

            //var res = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault();


            //ViewBag.IsAccompany = "N";

            //if (res.Accompany_Type == "Accompany")
            //{
            //    ViewBag.IsAccompany = "Y";
            //    var res1 = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == res.AccompanyingGuestPRNO).FirstOrDefault();
            //    if (res1.ApplicationStatus == "Pending")
            //    {
            //        ViewBag.IsAccompanyPending = "Y";
            //        ViewBag.UserDetails = res1.PRNO + "," + res1.UHID;
            //    }
            //    else
            //    {
            //        ViewBag.IsAccompanyPending = "N";
            //    }
            //}

            return View(query.ToList());
        }
        [HttpPost]
        public ActionResult HabitDetails(List<NC_TBL_HABIT_DETAILS> HabitDetails)
        {
            string PR = this.Session["TMPPR"].ToString();
            foreach (NC_TBL_HABIT_DETAILS habitDetail in HabitDetails)
            {
                NC_TBL_HABIT_DETAILS ncTblHabitDetails = this.db.NC_TBL_HABIT_DETAILS.Find(habitDetail.Id);
                if (habitDetail.HBQFreq != null)
                {
                    ncTblHabitDetails.HBQFreq = habitDetail.HBQFreq;
                    ncTblHabitDetails.HBDesc = habitDetail.HBDesc;
                }
                else
                    ncTblHabitDetails.HBDesc = habitDetail.HBDesc;
            }
            this.db.SaveChanges();
            if (this.db.NC_TBL_BOOKED_DATES.Where(e => e.AccompanyingGuestPRNO == PR).ToList().Count() >= 1)
            {
                if (this.db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == PR && i.PAYMENT_TYPE_CODE == 1) != null)
                {
                    NC_TBL_BOOKED_DATES ncTblBookedDates = this.db.NC_TBL_BOOKED_DATES.Find(this.db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault<NC_TBL_BOOKED_DATES>().Id);
                    if (ncTblBookedDates != null)
                        ncTblBookedDates.ApplicationStatus = "Completed";
                    this.db.SaveChanges();
                }
            }
            string prn = this.Session["TMPPR"].ToString();
            NC_TBL_BOOKED_DATES PackageDetails = this.db.NC_TBL_BOOKED_DATES.FirstOrDefault<NC_TBL_BOOKED_DATES>(i => i.PRNO == prn);
            var data1 = this.db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 1).Select(bill => new
            {
                BillTypeCode = bill.Bill_TYPE_CODE,
                BillAmount = bill.Fixed == "Y" ? bill.Fixed_Amount : PackageDetails.Package_Amount * bill.Fixed_Amount / (Decimal?)new Decimal(100)
            }).FirstOrDefault();
            string uniqueno1 = this.getUniqueno("BILN");
            if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 1) == null)
            {
                this.db.NC_TBL_Billing_Trans.Add(new NC_TBL_Billing_Trans()
                {
                    PR_NO = this.Session["TMPPR"].ToString(),
                    TAX = new Decimal?(new Decimal(0)),
                    Discount = new Decimal?(new Decimal(0)),
                    Je_NARRATION = "",
                    Je_AMOUNT = new Decimal?(new Decimal(0)),
                    TOTAL_AMOUNT = data1.BillAmount,
                    NET_AMOUNT = data1.BillAmount,
                    Bill_TYPE_CODE = data1.BillTypeCode,
                    BillNo = uniqueno1,
                    billed_date = new DateTime?(DateTime.Now),
                    Paid_status = "N",
                    MASTER_UPDATED = "N",
                    Staff_code = new int?(0),
                    Remarks = ""
                });
                this.db.SaveChanges();
            }
            Decimal? nullable1;
            if (this.db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == prn && i.PAYMENT_TYPE_CODE == 1) == null)
            {
                nullable1 = data1.BillAmount;
                Decimal? nullable2 = !nullable1.HasValue ? new Decimal?(new Decimal(0)) : data1.BillAmount;
                NC_TBL_PAYMENT_TRANS entity = new NC_TBL_PAYMENT_TRANS();
                string uniqueno2 = this.getUniqueno("RCPT");
                entity.PR_NO = prn;
                entity.RECEIPT_NO = uniqueno2;
                entity.TXN_DATE = DateTime.Now;
                entity.ACTUAL_PAID_DATE = DateTime.Now;
                entity.PAID_AMOUNT = Convert.ToDecimal((object)nullable2);
                entity.PAYMENT_MODE_CODE = 1;
                entity.PAYMENT_TYPE_CODE = 1;
                entity.EntryType = "P";
                entity.RECEIPT_CANCELLED = "N";
                entity.Pay_Category = "R";
                this.db.NC_TBL_PAYMENT_TRANS.Add(entity);
                this.db.SaveChanges();
            }
            NC_TBL_BOOKED_DATES ncTblBookedDates1 = this.db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.AccompanyingGuestPRNO == prn);
            NC_TBL_BOOKED_DATES mainguest = this.db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == prn);
            if (ncTblBookedDates1 != null)
                this.db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3).Select(bill => new
                {
                    BillTypeCode = bill.Bill_TYPE_CODE,
                    BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : PackageDetails.Package_Amount * bill.Fixed_Amount / (Decimal?)new Decimal(100)) / (Decimal?)new Decimal(2),
                    BillPercentage = bill.Fixed == "Y" ? (Decimal?)new Decimal(0) : bill.Fixed_Amount
                }).FirstOrDefault();
            else if (mainguest.Accompany_Type == "Accompany")
            {
                var data2 = this.db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3).Select(bill => new
                {
                    BillTypeCode = bill.Bill_TYPE_CODE,
                    BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : PackageDetails.Package_Amount * bill.Fixed_Amount / (Decimal?)new Decimal(100)) / (Decimal?)new Decimal(2),
                    BillPercentage = bill.Fixed == "Y" ? (Decimal?)new Decimal(0) : bill.Fixed_Amount
                }).FirstOrDefault();
                string uniqueno2 = this.getUniqueno("BILN");
                if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 3) == null)
                {
                    this.db.NC_TBL_Billing_Trans.Add(new NC_TBL_Billing_Trans()
                    {
                        PR_NO = prn,
                        TAX = new Decimal?(new Decimal(0)),
                        Discount = new Decimal?(new Decimal(0)),
                        Je_NARRATION = "",
                        Je_AMOUNT = new Decimal?(new Decimal(0)),
                        TOTAL_AMOUNT = data2.BillAmount,
                        NET_AMOUNT = data2.BillAmount,
                        Bill_TYPE_CODE = data2.BillTypeCode,
                        BillNo = uniqueno2,
                        billed_date = new DateTime?(DateTime.Now),
                        Paid_status = "N",
                        MASTER_UPDATED = "N",
                        Staff_code = new int?(0),
                        Remarks = ""
                    });
                    this.db.SaveChanges();
                }
                string uniqueno3 = this.getUniqueno("BILN");
                if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 2) == null)
                {
                    NC_TBL_Billing_Trans entity = new NC_TBL_Billing_Trans();
                    entity.PR_NO = prn;
                    entity.TAX = new Decimal?(new Decimal(0));
                    entity.Discount = new Decimal?(new Decimal(0));
                    entity.Je_NARRATION = "";
                    entity.Je_AMOUNT = new Decimal?(new Decimal(0));
                    NC_TBL_Billing_Trans ncTblBillingTrans1 = entity;
                    nullable1 = PackageDetails.Package_Amount;
                    Decimal? nullable2 = nullable1.HasValue ? new Decimal?(nullable1.GetValueOrDefault() / new Decimal(2)) : new Decimal?();
                    ncTblBillingTrans1.TOTAL_AMOUNT = nullable2;
                    NC_TBL_Billing_Trans ncTblBillingTrans2 = entity;
                    nullable1 = PackageDetails.Package_Amount;
                    Decimal? nullable3 = nullable1.HasValue ? new Decimal?(nullable1.GetValueOrDefault() / new Decimal(2)) : new Decimal?();
                    ncTblBillingTrans2.NET_AMOUNT = nullable3;
                    entity.Bill_TYPE_CODE = 2;
                    entity.BillNo = uniqueno3;
                    entity.billed_date = new DateTime?(DateTime.Now);
                    entity.Paid_status = "N";
                    entity.MASTER_UPDATED = "N";
                    entity.Staff_code = new int?(0);
                    entity.Remarks = "";
                    this.db.NC_TBL_Billing_Trans.Add(entity);
                    this.db.SaveChanges();
                }
                this.getUniqueno("BILN");
                if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == mainguest.AccompanyingGuestPRNO && i.Bill_TYPE_CODE == 3) == null)
                {
                    this.db.NC_TBL_Billing_Trans.Add(new NC_TBL_Billing_Trans()
                    {
                        PR_NO = mainguest.AccompanyingGuestPRNO,
                        TAX = new Decimal?(new Decimal(0)),
                        Discount = new Decimal?(new Decimal(0)),
                        Je_NARRATION = "",
                        Je_AMOUNT = new Decimal?(new Decimal(0)),
                        TOTAL_AMOUNT = data2.BillAmount,
                        NET_AMOUNT = data2.BillAmount,
                        Bill_TYPE_CODE = data2.BillTypeCode,
                        BillNo = uniqueno2,
                        billed_date = new DateTime?(DateTime.Now),
                        Paid_status = "N",
                        MASTER_UPDATED = "N",
                        Staff_code = new int?(0),
                        Remarks = ""
                    });
                    this.db.SaveChanges();
                }
                this.getUniqueno("BILN");
                if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == mainguest.AccompanyingGuestPRNO && i.Bill_TYPE_CODE == 2) == null)
                {
                    NC_TBL_Billing_Trans entity = new NC_TBL_Billing_Trans();
                    entity.PR_NO = mainguest.AccompanyingGuestPRNO;
                    entity.TAX = new Decimal?(new Decimal(0));
                    entity.Discount = new Decimal?(new Decimal(0));
                    entity.Je_NARRATION = "";
                    entity.Je_AMOUNT = new Decimal?(new Decimal(0));
                    NC_TBL_Billing_Trans ncTblBillingTrans1 = entity;
                    nullable1 = PackageDetails.Package_Amount;
                    Decimal? nullable2 = nullable1.HasValue ? new Decimal?(nullable1.GetValueOrDefault() / new Decimal(2)) : new Decimal?();
                    ncTblBillingTrans1.TOTAL_AMOUNT = nullable2;
                    NC_TBL_Billing_Trans ncTblBillingTrans2 = entity;
                    nullable1 = PackageDetails.Package_Amount;
                    Decimal? nullable3 = nullable1.HasValue ? new Decimal?(nullable1.GetValueOrDefault() / new Decimal(2)) : new Decimal?();
                    ncTblBillingTrans2.NET_AMOUNT = nullable3;
                    entity.Bill_TYPE_CODE = 2;
                    entity.BillNo = uniqueno3;
                    entity.billed_date = new DateTime?(DateTime.Now);
                    entity.Paid_status = "N";
                    entity.MASTER_UPDATED = "N";
                    entity.Staff_code = new int?(0);
                    entity.Remarks = "";
                    this.db.NC_TBL_Billing_Trans.Add(entity);
                    this.db.SaveChanges();
                }
            }
            else
            {
                var data2 = this.db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3).Select(bill => new
                {
                    BillTypeCode = bill.Bill_TYPE_CODE,
                    BillAmount = bill.Fixed == "Y" ? bill.Fixed_Amount : PackageDetails.Package_Amount * bill.Fixed_Amount / (Decimal?)new Decimal(100),
                    BillPercentage = bill.Fixed == "Y" ? (Decimal?)new Decimal(0) : bill.Fixed_Amount
                }).FirstOrDefault();
                string uniqueno2 = this.getUniqueno("BILN");
                if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 3) == null)
                {
                    this.db.NC_TBL_Billing_Trans.Add(new NC_TBL_Billing_Trans()
                    {
                        PR_NO = prn,
                        TAX = new Decimal?(new Decimal(0)),
                        Discount = new Decimal?(new Decimal(0)),
                        Je_NARRATION = "",
                        Je_AMOUNT = new Decimal?(new Decimal(0)),
                        TOTAL_AMOUNT = data2.BillAmount,
                        NET_AMOUNT = data2.BillAmount,
                        Bill_TYPE_CODE = data2.BillTypeCode,
                        BillNo = uniqueno2,
                        billed_date = new DateTime?(DateTime.Now),
                        Paid_status = "N",
                        MASTER_UPDATED = "N",
                        Staff_code = new int?(0),
                        Remarks = ""
                    });
                    this.db.SaveChanges();
                }
                string uniqueno3 = this.getUniqueno("BILN");
                if (this.db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 2) == null)
                {
                    this.db.NC_TBL_Billing_Trans.Add(new NC_TBL_Billing_Trans()
                    {
                        PR_NO = prn,
                        TAX = new Decimal?(new Decimal(0)),
                        Discount = new Decimal?(new Decimal(0)),
                        Je_NARRATION = "",
                        Je_AMOUNT = new Decimal?(new Decimal(0)),
                        TOTAL_AMOUNT = PackageDetails.Package_Amount,
                        NET_AMOUNT = PackageDetails.Package_Amount,
                        Bill_TYPE_CODE = 2,
                        BillNo = uniqueno3,
                        billed_date = new DateTime?(DateTime.Now),
                        Paid_status = "N",
                        MASTER_UPDATED = "N",
                        Staff_code = new int?(0),
                        Remarks = ""
                    });
                    this.db.SaveChanges();
                }
            }
            NC_TBL_BOOKED_DATES ncTblBookedDates2 = this.db.NC_TBL_BOOKED_DATES.Find(this.db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault().Id);
            if (ncTblBookedDates2 != null)
                ncTblBookedDates2.ApplicationStatus = "Completed";
            this.db.SaveChanges();
            return (ActionResult)this.RedirectToAction("OnlinePayment");
        }

        //[HttpPost]
        public ActionResult BillingDetails()
        {

            try
            {
                var PR = Session["TMPPR"].ToString();
                //foreach (NC_TBL_HABIT_DETAILS cust in HabitDetails)
                //{
                //    NC_TBL_HABIT_DETAILS existing = db.NC_TBL_HABIT_DETAILS.Find(cust.Id);
                //    if (cust.HBQFreq != null)
                //    {
                //        existing.HBQFreq = cust.HBQFreq;
                //        existing.HBDesc = cust.HBDesc;
                //    }
                //    else
                //    {
                //        existing.HBDesc = cust.HBDesc;
                //    }
                //}
                db.SaveChanges();


                var checkPRAccompany = (from e in db.NC_TBL_BOOKED_DATES
                                        where e.AccompanyingGuestPRNO == PR
                                        select e).ToList();
                if (checkPRAccompany.Count() >= 1)
                {
                    var PaymentAccompanyPR = db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == PR && i.PAYMENT_TYPE_CODE == 1);
                    if (PaymentAccompanyPR != null)
                    {

                        var AccompanyBookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault();
                        NC_TBL_BOOKED_DATES AccompanyexistingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(AccompanyBookedDetails.Id);
                        if (AccompanyexistingDatesBooking != null)
                        {

                            AccompanyexistingDatesBooking.ApplicationStatus = "Completed";

                        }
                        db.SaveChanges();

                    }
                }
                ////////var TPRNO = Session["TMPPR"].ToString();
                ////////var BookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == TPRNO).FirstOrDefault();
                ////////NC_TBL_BOOKED_DATES existingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(BookedDetails.Id);
                ////////if (existingDatesBooking != null)
                ////////{
                ////////    existingDatesBooking.ApplicationStatus = "Completed";
                ////////}
                ////////db.SaveChanges();

                ///////To Update duplicate PRNO to original PRNO
                //var DuplicatePRNO = Session["TMPPR"].ToString();
                //ObjectParameter Unique_PRNo = new ObjectParameter("uniqueno", typeof(String));
                //var UniqueNo = db.gen_unique_no("PRNO", Unique_PRNo);
                //var OriginalPRNO = Unique_PRNo.Value;
                //var BookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == DuplicatePRNO).FirstOrDefault();
                //NC_TBL_BOOKED_DATES existingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(BookedDetails.Id);
                //if (existingDatesBooking != null)
                //{
                //    existingDatesBooking.PRNO = OriginalPRNO.ToString();
                //}
                //db.SaveChanges();
                //var PersonalDetails = db.NC_TBL_PERSONAL_DETAILS.Where(i => i.PRNO == DuplicatePRNO).FirstOrDefault();
                //NC_TBL_PERSONAL_DETAILS existingPersonalDetails = db.NC_TBL_PERSONAL_DETAILS.Find(PersonalDetails.Id);
                //if (existingPersonalDetails != null)
                //{
                //    existingPersonalDetails.PRNO = OriginalPRNO.ToString();
                //}
                //db.SaveChanges();

                //var HLDetails = db.NC_TBL_HEALTH_DETAILS.Where(i => i.PRNO == DuplicatePRNO).ToList();

                //foreach (NC_TBL_HEALTH_DETAILS cust in HLDetails)
                //{
                //    NC_TBL_HEALTH_DETAILS existingHL = db.NC_TBL_HEALTH_DETAILS.Find(cust.Id);
                //    if (existingHL != null)
                //    {
                //        existingHL.PRNO = OriginalPRNO.ToString();
                //    }

                //}
                //db.SaveChanges();

                //var HBDetails = db.NC_TBL_HABIT_DETAILS.Where(i => i.PRNO == DuplicatePRNO).ToList();

                //foreach (NC_TBL_HABIT_DETAILS cust in HBDetails)
                //{
                //    NC_TBL_HABIT_DETAILS existingHB = db.NC_TBL_HABIT_DETAILS.Find(cust.Id);
                //    if (existingHB != null)
                //    {
                //        existingHB.PRNO = OriginalPRNO.ToString();
                //    }

                //}
                //db.SaveChanges();



                ////To Rename Folder Duplicate PRNO folder to Original PRNO Folde
                //var UHID = PersonalDetails.UHID;
                //var DuplicatePRNum = DuplicatePRNO.ToString();
                //var NewPRNum = OriginalPRNO.ToString(); ;

                //string path = Server.MapPath("~");
                //path = path + "Content\\" + UHID;
                //string Fromfol = "\\" + DuplicatePRNum + "\\";
                //string Tofol = "\\" + NewPRNum + "\\";


                //try
                //{
                //    // If the directory exist, then rename it.
                //    if (Directory.Exists(path + Fromfol))
                //    {
                //        Directory.Move(path + Fromfol, path + Tofol);
                //    }
                //}
                //catch (Exception)
                //{
                //    // Fail silently
                //}



                /////End of Original PRNO generation
                //Session["TMPPR"] = Unique_PRNo.Value;
                //To Store Bill Amount Default in Bill Transaction Table (NC_TBL_Billing_Trans) from master table(nc_tbl_bill_type_master) 
                var prn = Session["TMPPR"].ToString();
                var PackageDetails = db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == prn);

                var billquery = (from bill in db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 1)
                                 select new
                                 {
                                     BillTypeCode = bill.Bill_TYPE_CODE,
                                     //BillAmount= bill.Fixed_Amount
                                     BillAmount = bill.Fixed == "Y" ? bill.Fixed_Amount : (PackageDetails.Package_Amount * bill.Fixed_Amount) / 100
                                 }).FirstOrDefault();

                //var recordBillTrans = (from billtrans in db.NC_TBL_Billing_Trans select billtrans.BillNo).ToList();
                //if (recordBillTrans.Count == 0)
                //{
                //    ViewBag.BillNo = 1;
                //}
                //else
                //{
                //    ViewBag.BillNo = (from billtrans in db.NC_TBL_Billing_Trans select billtrans.BillNo).Max() + 1;
                //}
                var BillNo = getUniqueno("BILN");


                var BillPR = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 1);
                if (BillPR == null)
                {
                    var BillDetails = new NC_TBL_Billing_Trans();

                    BillDetails.PR_NO = Session["TMPPR"].ToString();
                    BillDetails.TAX = 0;
                    BillDetails.Discount = 0;
                    BillDetails.Je_NARRATION = "";
                    BillDetails.Je_AMOUNT = 0;
                    BillDetails.TOTAL_AMOUNT = billquery.BillAmount;
                    BillDetails.NET_AMOUNT = billquery.BillAmount;
                    BillDetails.Bill_TYPE_CODE = billquery.BillTypeCode;
                    BillDetails.BillNo = BillNo;
                    BillDetails.billed_date = DateTime.Now;
                    //BillDetails.Paid_status = "N";
                    BillDetails.Paid_status = "Y";
                    BillDetails.MASTER_UPDATED = "N";
                    BillDetails.Staff_code = 0;
                    BillDetails.Remarks = "";
                    db.NC_TBL_Billing_Trans.Add(BillDetails);
                    db.SaveChanges();

                }


                var EPaymentPR = db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == prn && i.PAYMENT_TYPE_CODE == 1);
                if (EPaymentPR == null)
                {

                    var EconsultAmount = billquery.BillAmount == null ? 0 : billquery.BillAmount;

                    NC_TBL_PAYMENT_TRANS save = new NC_TBL_PAYMENT_TRANS();
                    var ReceiptNum = getUniqueno("RCPT");
                    save.PR_NO = prn;
                    save.RECEIPT_NO = ReceiptNum;
                    save.TXN_DATE = DateTime.Now;
                    save.ACTUAL_PAID_DATE = DateTime.Now;
                    save.PAID_AMOUNT = Convert.ToDecimal(EconsultAmount);
                    save.PAYMENT_MODE_CODE = 1;
                    save.PAYMENT_TYPE_CODE = 1;
                    save.EntryType = "P";
                    save.RECEIPT_CANCELLED = "N";
                    save.Pay_Category = "R";
                    db.NC_TBL_PAYMENT_TRANS.Add(save);
                    db.SaveChanges();


                }

                ////To comment Advance Payment

                var accompany = (db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.AccompanyingGuestPRNO == prn));
                var mainguest = (db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == prn));
                if (accompany != null)
                {
                    var billquery1 = (from bill in db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3)
                                      select new
                                      {
                                          BillTypeCode = bill.Bill_TYPE_CODE,
                                          // BillAmount = bill.Fixed_Amount
                                          BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : (PackageDetails.Package_Amount * bill.Fixed_Amount) / 100) / 2,
                                          BillPercentage = bill.Fixed == "Y" ? 0 : bill.Fixed_Amount,

                                      }).FirstOrDefault();

                }
                else
                {
                    if (mainguest.Accompany_Type == "Accompany")
                    {
                        var billquery1 = (from bill in db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3)
                                          select new
                                          {
                                              BillTypeCode = bill.Bill_TYPE_CODE,
                                              // BillAmount = bill.Fixed_Amount
                                              BillAmount = (bill.Fixed == "Y" ? bill.Fixed_Amount : (PackageDetails.Package_Amount * bill.Fixed_Amount) / 100) / 2,
                                              BillPercentage = bill.Fixed == "Y" ? 0 : bill.Fixed_Amount,

                                          }).FirstOrDefault();




                        var BillNo1 = getUniqueno("BILN");


                        var BillPR1 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 3);
                        if (BillPR1 == null)
                        {
                            var BillDetails = new NC_TBL_Billing_Trans();

                            BillDetails.PR_NO = prn;
                            BillDetails.TAX = 0;
                            BillDetails.Discount = 0;
                            BillDetails.Je_NARRATION = "";
                            BillDetails.Je_AMOUNT = 0;
                            BillDetails.TOTAL_AMOUNT = billquery1.BillAmount;
                            BillDetails.NET_AMOUNT = billquery1.BillAmount;
                            BillDetails.Bill_TYPE_CODE = billquery1.BillTypeCode;
                            BillDetails.BillNo = BillNo1;
                            BillDetails.billed_date = DateTime.Now;
                            BillDetails.Paid_status = "N";
                            BillDetails.MASTER_UPDATED = "N";
                            BillDetails.Staff_code = 0;
                            BillDetails.Remarks = "";
                            db.NC_TBL_Billing_Trans.Add(BillDetails);
                            db.SaveChanges();
                        }


                        var BillNo3 = getUniqueno("BILN");
                        var BillPR2 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 2);
                        if (BillPR2 == null)
                        {
                            var BillDetails2 = new NC_TBL_Billing_Trans();

                            BillDetails2.PR_NO = prn;
                            BillDetails2.TAX = 0;
                            BillDetails2.Discount = 0;
                            BillDetails2.Je_NARRATION = "";
                            BillDetails2.Je_AMOUNT = 0;
                            BillDetails2.TOTAL_AMOUNT = (PackageDetails.Package_Amount) / 2;
                            BillDetails2.NET_AMOUNT = (PackageDetails.Package_Amount) / 2;
                            BillDetails2.Bill_TYPE_CODE = 2;
                            BillDetails2.BillNo = BillNo3;
                            BillDetails2.billed_date = DateTime.Now;
                            BillDetails2.Paid_status = "N";
                            BillDetails2.MASTER_UPDATED = "N";
                            BillDetails2.Staff_code = 0;
                            BillDetails2.Remarks = "";
                            db.NC_TBL_Billing_Trans.Add(BillDetails2);
                            db.SaveChanges();
                        }

                        //to accompany

                        var BillNo2 = getUniqueno("BILN");


                        var BillPR3 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == mainguest.AccompanyingGuestPRNO && i.Bill_TYPE_CODE == 3);
                        if (BillPR3 == null)
                        {
                            var BillDetails = new NC_TBL_Billing_Trans();

                            BillDetails.PR_NO = mainguest.AccompanyingGuestPRNO;
                            BillDetails.TAX = 0;
                            BillDetails.Discount = 0;
                            BillDetails.Je_NARRATION = "";
                            BillDetails.Je_AMOUNT = 0;
                            BillDetails.TOTAL_AMOUNT = billquery1.BillAmount;
                            BillDetails.NET_AMOUNT = billquery1.BillAmount;
                            BillDetails.Bill_TYPE_CODE = billquery1.BillTypeCode;
                            BillDetails.BillNo = BillNo1;
                            BillDetails.billed_date = DateTime.Now;
                            BillDetails.Paid_status = "N";
                            BillDetails.MASTER_UPDATED = "N";
                            BillDetails.Staff_code = 0;
                            BillDetails.Remarks = "";
                            db.NC_TBL_Billing_Trans.Add(BillDetails);
                            db.SaveChanges();
                        }


                        var BillNo4 = getUniqueno("BILN");
                        var BillPR5 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == mainguest.AccompanyingGuestPRNO && i.Bill_TYPE_CODE == 2);
                        if (BillPR5 == null)
                        {
                            var BillDetails2 = new NC_TBL_Billing_Trans();

                            BillDetails2.PR_NO = mainguest.AccompanyingGuestPRNO;
                            BillDetails2.TAX = 0;
                            BillDetails2.Discount = 0;
                            BillDetails2.Je_NARRATION = "";
                            BillDetails2.Je_AMOUNT = 0;
                            BillDetails2.TOTAL_AMOUNT = (PackageDetails.Package_Amount) / 2;
                            BillDetails2.NET_AMOUNT = (PackageDetails.Package_Amount) / 2;
                            BillDetails2.Bill_TYPE_CODE = 2;
                            BillDetails2.BillNo = BillNo3;
                            BillDetails2.billed_date = DateTime.Now;
                            BillDetails2.Paid_status = "N";
                            BillDetails2.MASTER_UPDATED = "N";
                            BillDetails2.Staff_code = 0;
                            BillDetails2.Remarks = "";
                            db.NC_TBL_Billing_Trans.Add(BillDetails2);
                            db.SaveChanges();
                        }




                    }
                    else
                    {


                        var billquery1 = (from bill in db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3)
                                          select new
                                          {
                                              BillTypeCode = bill.Bill_TYPE_CODE,
                                              // BillAmount = bill.Fixed_Amount
                                              BillAmount = bill.Fixed == "Y" ? bill.Fixed_Amount : (PackageDetails.Package_Amount * bill.Fixed_Amount) / 100,
                                              BillPercentage = bill.Fixed == "Y" ? 0 : bill.Fixed_Amount,

                                          }).FirstOrDefault();






                        var BillNo1 = getUniqueno("BILN");


                        var BillPR1 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 3);
                        if (BillPR1 == null)
                        {
                            var BillDetails = new NC_TBL_Billing_Trans();

                            BillDetails.PR_NO = prn;
                            BillDetails.TAX = 0;
                            BillDetails.Discount = 0;
                            BillDetails.Je_NARRATION = "";
                            BillDetails.Je_AMOUNT = 0;
                            BillDetails.TOTAL_AMOUNT = billquery1.BillAmount;
                            BillDetails.NET_AMOUNT = billquery1.BillAmount;
                            BillDetails.Bill_TYPE_CODE = billquery1.BillTypeCode;
                            BillDetails.BillNo = BillNo1;
                            BillDetails.billed_date = DateTime.Now;
                            BillDetails.Paid_status = "N";
                            BillDetails.MASTER_UPDATED = "N";
                            BillDetails.Staff_code = 0;
                            BillDetails.Remarks = "";
                            db.NC_TBL_Billing_Trans.Add(BillDetails);
                            db.SaveChanges();
                        }


                        var BillNo3 = getUniqueno("BILN");
                        var BillPR2 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 2);
                        if (BillPR2 == null)
                        {
                            var BillDetails2 = new NC_TBL_Billing_Trans();

                            BillDetails2.PR_NO = prn;
                            BillDetails2.TAX = 0;
                            BillDetails2.Discount = 0;
                            BillDetails2.Je_NARRATION = "";
                            BillDetails2.Je_AMOUNT = 0;
                            BillDetails2.TOTAL_AMOUNT = PackageDetails.Package_Amount;
                            BillDetails2.NET_AMOUNT = PackageDetails.Package_Amount;
                            BillDetails2.Bill_TYPE_CODE = 2;
                            BillDetails2.BillNo = BillNo3;
                            BillDetails2.billed_date = DateTime.Now;
                            BillDetails2.Paid_status = "N";
                            BillDetails2.MASTER_UPDATED = "N";
                            BillDetails2.Staff_code = 0;
                            BillDetails2.Remarks = "";
                            db.NC_TBL_Billing_Trans.Add(BillDetails2);
                            db.SaveChanges();
                        }







                    }
                }

                //var billquery1 = (from bill in db.nc_tbl_bill_type_master.Where(bill => bill.Bill_TYPE_CODE == 3)
                //                 select new
                //                 {
                //                     BillTypeCode = bill.Bill_TYPE_CODE,
                //                     // BillAmount = bill.Fixed_Amount
                //                     BillAmount = bill.Fixed == "Y" ? bill.Fixed_Amount : (PackageDetails.Package_Amount * bill.Fixed_Amount) / 100,
                //                     BillPercentage = bill.Fixed == "Y" ? 0 : bill.Fixed_Amount,

                //                 }).FirstOrDefault();

                //if( mainguest.Accompany_Type != "Accompany")
                //             {
                //               var BillNo1 = getUniqueno("BILN");


                //               var BillPR1 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 3);
                //               if (BillPR1 == null)
                //               {
                //                   var BillDetails = new NC_TBL_Billing_Trans();

                //                   BillDetails.PR_NO = prn;
                //                   BillDetails.TAX = 0;
                //                   BillDetails.Discount = 0;
                //                   BillDetails.Je_NARRATION = "";
                //                   BillDetails.Je_AMOUNT = 0;
                //                   BillDetails.TOTAL_AMOUNT = billquery1.BillAmount;
                //                   BillDetails.NET_AMOUNT = billquery1.BillAmount;
                //                   BillDetails.Bill_TYPE_CODE = billquery1.BillTypeCode;
                //                   BillDetails.BillNo = BillNo1;
                //                   BillDetails.billed_date = DateTime.Now;
                //                   BillDetails.Paid_status = "N";
                //                   BillDetails.MASTER_UPDATED = "N";
                //                   BillDetails.Staff_code = 0;
                //                   BillDetails.Remarks = "";
                //                   db.NC_TBL_Billing_Trans.Add(BillDetails);
                //                   db.SaveChanges();
                //               }


                //               var BillNo3 = getUniqueno("BILN");
                //               var BillPR2 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 2);
                //               if (BillPR2 == null)
                //               {
                //                   var BillDetails2 = new NC_TBL_Billing_Trans();

                //                   BillDetails2.PR_NO = prn;
                //                   BillDetails2.TAX = 0;
                //                   BillDetails2.Discount = 0;
                //                   BillDetails2.Je_NARRATION = "";
                //                   BillDetails2.Je_AMOUNT = 0;
                //                   BillDetails2.TOTAL_AMOUNT = PackageDetails.Package_Amount;
                //                   BillDetails2.NET_AMOUNT = PackageDetails.Package_Amount;
                //                   BillDetails2.Bill_TYPE_CODE = 2;
                //                   BillDetails2.BillNo = BillNo3;
                //                   BillDetails2.billed_date = DateTime.Now;
                //                   BillDetails2.Paid_status = "N";
                //                   BillDetails2.MASTER_UPDATED = "N";
                //                   BillDetails2.Staff_code = 0;
                //                   BillDetails2.Remarks = "";
                //                   db.NC_TBL_Billing_Trans.Add(BillDetails2);
                //                   db.SaveChanges();
                //               }
                //    }
                //To Accompany Guest

                //if( mainguest.Accompany_Type == "Accompany")
                //{
                //    var BillNo1 = getUniqueno("BILN");


                //    var BillPR1 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 3);
                //    if (BillPR1 == null)
                //    {
                //        var BillDetails = new NC_TBL_Billing_Trans();

                //        BillDetails.PR_NO = mainguest.AccompanyingGuestPRNO ;
                //        BillDetails.TAX = 0;
                //        BillDetails.Discount = 0;
                //        BillDetails.Je_NARRATION = "";
                //        BillDetails.Je_AMOUNT = 0;
                //        BillDetails.TOTAL_AMOUNT = billquery1.BillAmount;
                //        BillDetails.NET_AMOUNT = billquery1.BillAmount;
                //        BillDetails.Bill_TYPE_CODE = billquery1.BillTypeCode;
                //        BillDetails.BillNo = BillNo1;
                //        BillDetails.billed_date = DateTime.Now;
                //        BillDetails.Paid_status = "N";
                //        BillDetails.MASTER_UPDATED = "N";
                //        BillDetails.Staff_code = 0;
                //        BillDetails.Remarks = "";
                //        db.NC_TBL_Billing_Trans.Add(BillDetails);
                //        db.SaveChanges();
                //    }


                //    var BillNo3 = getUniqueno("BILN");
                //    var BillPR2 = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == prn && i.Bill_TYPE_CODE == 2);
                //    if (BillPR2 == null)
                //    {
                //        var BillDetails2 = new NC_TBL_Billing_Trans();

                //        BillDetails2.PR_NO = mainguest.AccompanyingGuestPRNO;
                //        BillDetails2.TAX = 0;
                //        BillDetails2.Discount = 0;
                //        BillDetails2.Je_NARRATION = "";
                //        BillDetails2.Je_AMOUNT = 0;
                //        BillDetails2.TOTAL_AMOUNT = PackageDetails.Package_Amount;
                //        BillDetails2.NET_AMOUNT = PackageDetails.Package_Amount;
                //        BillDetails2.Bill_TYPE_CODE = 2;
                //        BillDetails2.BillNo = BillNo3;
                //        BillDetails2.billed_date = DateTime.Now;
                //        BillDetails2.Paid_status = "N";
                //        BillDetails2.MASTER_UPDATED = "N";
                //        BillDetails2.Staff_code = 0;
                //        BillDetails2.Remarks = "";
                //        db.NC_TBL_Billing_Trans.Add(BillDetails2);
                //        db.SaveChanges();
                //    }


                //}


                //var PR = Session["TMPPR"].ToString();

                var BookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault();
                NC_TBL_BOOKED_DATES existingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(BookedDetails.Id);
                if (existingDatesBooking != null)
                {
                    existingDatesBooking.ApplicationStatus = "Completed";
                }
                db.SaveChanges();
                ViewBag.PRNumber = PR;
                ///Starting to send Email
                //var prnum = db.NC_TBL_PERSONAL_DETAILS.Where(x => x.PRNO == PR).FirstOrDefault();
                //if (prnum != null)
                //{
                //    var email = prnum.EmailId;
                //    var name = prnum.FirstName + " " + prnum.LastName;
                //    var PRNO = prnum.PRNO;
                //    var UHID = prnum.UHID;

                //    var Emailsendingflag = db.NC_TBL_APPCONFIG.FirstOrDefault();
                //    if (Emailsendingflag != null)
                //    {
                //        if (Emailsendingflag.SendEmailFlag == "Y")
                //        {

                //            MailMessage mail = new MailMessage();
                //            mail.To.Add(email);
                //            mail.From = new MailAddress("nhcl.pema@hotmail.com");
                //            mail.Subject = "PEMA Reservation Details" + "[PRNO: " + PRNO + "]";
                //            mbody = new StringBuilder();
                //            mbody.Append(mheader);
                //            mbody.Append("<table> ");
                //            mbody.Append("<tr><td><h2 style='float:left; margin-left:100px;'>Reservation  Details</h2></td></tr> ");
                //            mbody.Append("<tr> <td>Dear " + name + ",</td> </tr> ");
                //            mbody.Append("<tr> <td>&nbsp;&nbsp;&nbsp;</td> </tr>  ");
                //            mbody.Append("<tr> <td> ");
                //            mbody.Append("Greetings from PEMA WELLNESS, Vizag.<br /><br /> ");
                //            mbody.Append("We thank you for choosing PEMA WELLNESS services.<br /><br />");
                //            mbody.Append("We confirm the receipt of your Application.<br /><br />");
                //            mbody.Append("Your application has been forwarded to our chief Medical Officer for review and suggestions.<br /><br />");
                //            mbody.Append("The suggested Treatments and Package details will be mailed to you once our Chief Medical Officer reviews your application.<br /><br />");
                //            mbody.Append("The Advance Payment need to be done against the suggested package to confirm your booking.<br /><br />");
                //            mbody.Append("Please use the following ID details for further communication with PEMA WELLNESS.<br /><br />");
                //            mbody.Append("UHID [Life Time ID] :&nbsp;<b style='color:blue'>" + UHID + "</b> <br /><br /> ");
                //            mbody.Append("PRNO [Visit ID] :&nbsp;<b style='color:blue'>" + PRNO + "</b><br /><br />");
                //            mbody.Append("</td>  </tr>  ");
                //            mbody.Append("</table>");
                //            mbody.Append(mfooter);
                //            mail.Body = mbody.ToString();
                //            mail.IsBodyHtml = true;
                //            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(customCertValidation);
                //            SmtpClient smtp = new SmtpClient("smtp.live.com");

                //            smtp.Port = 587;
                //            smtp.UseDefaultCredentials = true;
                //            smtp.EnableSsl = true;
                //            smtp.Credentials = new System.Net.NetworkCredential
                //           ("nhcl.pema@hotmail.com", "Pema@123");// Enter seders User name and password
                //            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //            smtp.Send(mail);
                //            mail.Dispose();
                //            smtp.Dispose();

                //        }
                //    }
                //}
                ///Ending to comment Email
                //List<HabitModel> model = new List<HabitModel>();

                //return View(model);

                return RedirectToAction("ResrvationStep5");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var innerexcep = ex.InnerException.ToString();
                    ErrorLog(ex.Message, innerexcep);

                }
                else
                {
                    ErrorLog(ex.Message, "");
                }

                return PartialView("Error");


            }


        }

        //Booking Information Screen
        public ActionResult DatesConformation(string PRNO, String UHID)
        {

            ViewBag.UserType = Session["UserCode"];
            //var PackageDetails1 = db.NC_TBL_BOOKED_DATES.Where(a => a.PRNO == PRNO).FirstOrDefault();

            var PackageDetails1 = (from Booked in db.NC_TBL_BOOKED_DATES
                                   where Booked.PRNO == PRNO
                                   select new
                                   {
                                       PackageType = Booked.PackageType,
                                       PackageName = Booked.PackageName,
                                       NoOfDays = Booked.NoOfDays,
                                       ArrivalDate = SqlFunctions.DateName("day", Booked.ArrivalDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.ArrivalDate.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.ArrivalDate),
                                       DepartureDate = SqlFunctions.DateName("day", Booked.DepartureDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.DepartureDate.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.DepartureDate),
                                       AlternateArrivalDate = (Booked.AlternateArrivalDate.HasValue) ? SqlFunctions.DateName("day", Booked.AlternateArrivalDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.AlternateArrivalDate.Value.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.AlternateArrivalDate) : string.Empty,
                                       AlternateDepartureDate = (Booked.AlternateDepartureDate.HasValue) ? SqlFunctions.DateName("day", Booked.AlternateDepartureDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.AlternateDepartureDate.Value.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.AlternateDepartureDate) : string.Empty,
                                       AccompanyingGuests = Booked.AccompanyingGuests,
                                       RoomView = Booked.RoomView,
                                       RoomType = Booked.RoomType,
                                       RoomTariff = Booked.RoomTariff,
                                       Package_Amount = Booked.Package_Amount,
                                       Group_Code = Booked.Group_Code,
                                       Accompany_Type = Booked.Accompany_Type,
                                       Guest_Attender_Name = Booked.Guest_Attender_Name,
                                       AccompanyingGuestUHID = Booked.AccompanyingGuestUHID,
                                       AccompanyingGuestPRNO = Booked.AccompanyingGuestPRNO,
                                       Guest_Attender_ARNO = Booked.Guest_Attender_ARNO,
                                       UHID = Booked.UHID,
                                       UserType = Booked.UserType,
                                       TransportationRequired = Booked.Transportation_Required


                                   }).FirstOrDefault();
            // ViewBag.noofdays = PackageDetails1.NoOfDays;

            if (PRNO != null)
            {
                //var PackageDetails = db.NC_TBL_BOOKED_DATES.Where(a => a.PRNO == PRNO).FirstOrDefault();
                //if (PackageDetails != null)
                //{
                //    Session["TMPPR"] = PRNO;
                //    ViewBag.PackgeType = PackageDetails.PackageType;
                //    ViewBag.PackgeName = PackageDetails.PackageName;
                //    ViewBag.RoomView = PackageDetails.RoomView;
                //    ViewBag.RoomType = PackageDetails.RoomType;
                //    return View(PackageDetails);
                //}
                //else
                //{
                //    Session["TMPPR"] = "";
                //    return View(PackageDetails);
                //}
                Session["TMPPR"] = PRNO;
                Session["TMPUH"] = UHID;
                ViewBag.Accompanytype = PackageDetails1.Accompany_Type == null ? "-1" : PackageDetails1.Accompany_Type;
                ViewBag.formmode = "Edit";
                return View();
            }
            else
            {
                ViewBag.formmode = "New";
                Session["TMPPR"] = "";
                Session["TMPUH"] = Session["UHID"];
                // var ss = Session["TMPUH"];
                return View();

            }
        }

        public ActionResult GetDays(string PackageType, string PackageName, string RomView, string RomType, int AccompanyingGuests, string Arrivaldate, string Departuredate)
        {
            
            try
            {
                if (!(Arrivaldate != "") || !(Departuredate != ""))
                {
                    if (AccompanyingGuests > 0)
                    {
                        var variable = (
                            from R in this.db.NC_TBL_PACKAGE_MASTER
                            where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (R.Room_View == RomView) && (R.Room_Type == RomType)
                            select new { NoOfDays = R.NoOfDays, PackageAmount = R.DO_Calc_Amt, RoomTariffamount = R.DO_Pkg_Amt }).FirstOrDefault();
                        return base.Json(variable, JsonRequestBehavior.AllowGet);
                    }
                    var variable1 = (
                        from R in this.db.NC_TBL_PACKAGE_MASTER
                        where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (R.Room_View == RomView) && (R.Room_Type == RomType)
                        select new { NoOfDays = R.NoOfDays, PackageAmount = R.SO_Calc_Amt, RoomTariffamount = R.SO_Pkg_Amt }).FirstOrDefault();
                    return base.Json(variable1, JsonRequestBehavior.AllowGet);
                }
                DateTime dateTime = DateTime.ParseExact(Arrivaldate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (AccompanyingGuests > 0)
                {
                    var variable2 = (
                        from R in this.db.NC_TBL_PACKAGE_MASTER
                        where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (R.Room_View == RomView) && (R.Room_Type == RomType) && (dateTime >= R.EffetedFrom) && (dateTime <= R.EffetedTo)
                        select new { NoOfDays = R.NoOfDays, PackageAmount = R.DO_Calc_Amt, RoomTariffamount = R.DO_Pkg_Amt }).FirstOrDefault();
                    return base.Json(variable2, JsonRequestBehavior.AllowGet);
                }
                var variable3 = (
                    from R in this.db.NC_TBL_PACKAGE_MASTER
                    where (R.PackageType == PackageType) && (R.PackageName == PackageName) && (R.Room_View == RomView) && (R.Room_Type == RomType) && (dateTime >= R.EffetedFrom) && (dateTime <= R.EffetedTo)
                    select new { NoOfDays = R.NoOfDays, PackageAmount = R.SO_Calc_Amt, RoomTariffamount = R.SO_Pkg_Amt }).FirstOrDefault();
                return base.Json(variable3, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }


        public ActionResult GetRoomViews(string PackageType, string PackageName)
        {
            var items = (from R in db.NC_TBL_PACKAGE_MASTER
                         where R.PackageType == PackageType && R.PackageName == PackageName
                         select new
                         {
                             Room_View = R.Room_View
                         }).Distinct();

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult GetRoomTypes(string PackageType, string PackageName, string RoomView)
        {
            var items = (from R in db.NC_TBL_PACKAGE_MASTER
                         where R.PackageType == PackageType && R.PackageName == PackageName && R.Room_View == RoomView
                         select new
                         {
                             Room_Type = R.Room_Type
                         }).Distinct();

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }



        public ActionResult GetPackageTypes()
        {
            var items = (from R in db.NC_TBL_PACKAGE_MASTER

                         select new
                         {
                             PackageType = R.PackageType
                         }).Distinct();

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult GetPackageNames(string PackageType)
        {
            var items = (from R in db.NC_TBL_PACKAGE_MASTER
                         where R.PackageType == PackageType
                         //orderby (R.Id) ascending
                         select new
                         {

                             PackageName = R.PackageName,
                             NoOfDays = R.NoOfDays,

                         }).Distinct().OrderBy(x => x.NoOfDays);

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult Nationality(string Nationality)
        {
            var items = (from R in db.NC_TBL_NATIONALITY_MASTER
                             //orderby (R.Id) ascending
                         select new
                         {

                             Nationality = R.NationalityName,
                             NationalityCode=R.NationalityCode

                             //NoOfDays = R.NoOfDays,

                         });

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult Country(string Country)
        {
            var items = (from R in db.NC_TBL_COUNTRY_MASTER
                             //orderby (R.Id) ascending
                         select new
                         {

                             Country = R.CountryName,
                             CountryCode=R.CountryCode
                             //NoOfDays = R.NoOfDays,

                         });

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult GetTariffAmount(string RoomType, string RoomView, int AccompanyingGuests, string Arrivaldate, string Departuredate)
        {


            if (Arrivaldate != "" && Departuredate != "")
            {

                DateTime ArrivalDate = Convert.ToDateTime(Arrivaldate);
                DateTime DeparDate = Convert.ToDateTime(Departuredate);

                if (AccompanyingGuests > 0)
                {
                    //var query = db.NC_TBL_ROOM_MASTER.FirstOrDefault(i => i.Room_Type == RoomType && i.Room_View == RoomView);

                    var query = (from R in db.NC_TBL_PACKAGE_MASTER
                                 where
                                 R.Room_View == RoomView && R.Room_Type == RoomType && (ArrivalDate >= R.EffetedFrom && ArrivalDate <= R.EffetedTo)
                                 select new
                                 {
                                     TariffAmount = R.DO_Pkg_Amt

                                 }).FirstOrDefault();
                    return Json(query, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = (from R in db.NC_TBL_PACKAGE_MASTER
                                 where
                                 R.Room_View == RoomView && R.Room_Type == RoomType
                                 select new
                                 {
                                     TariffAmount = R.SO_Pkg_Amt

                                 }).FirstOrDefault();
                    return Json(query, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                if (AccompanyingGuests > 0)
                {
                    //var query = db.NC_TBL_ROOM_MASTER.FirstOrDefault(i => i.Room_Type == RoomType && i.Room_View == RoomView);

                    var query = (from R in db.NC_TBL_PACKAGE_MASTER
                                 where
                                 R.Room_View == RoomView && R.Room_Type == RoomType
                                 select new
                                 {
                                     TariffAmount = R.DO_Pkg_Amt

                                 }).FirstOrDefault();
                    return Json(query, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = (from R in db.NC_TBL_PACKAGE_MASTER
                                 where
                                 R.Room_View == RoomView && R.Room_Type == RoomType
                                 select new
                                 {
                                     TariffAmount = R.SO_Pkg_Amt

                                 }).FirstOrDefault();
                    return Json(query, JsonRequestBehavior.AllowGet);
                }

            }






        }

        public ActionResult RoomsAndTariff()
        {
            ////  string RoomType
            //var allTarrifs = (from R in db.NC_TBL_ROOM_MASTER
            //                  select new
            //                  {
            //                      Room_View = R.Room_View,
            //                      Room_Type = R.Room_Type,
            //                      SO_Tariff = R.SO_Tariff,
            //                      DO_Tariff = R.DO_Tariff

            //                  }).Distinct().ToList();

            var allTarrifs = (from R in db.NC_TBL_ROOM_MASTER
                              group R by new { R.Room_View, R.Room_Type, R.SO_Tariff, R.DO_Tariff }
                                  into grp
                                  select new RoomTariffView
                                  {
                                      Room_View = grp.Key.Room_View,
                                      Room_Type = grp.Key.Room_Type,
                                      SO_Tariff = grp.Key.SO_Tariff,
                                      DO_Tariff = grp.Key.DO_Tariff,
                                  }).ToList();

            // var allTarrifs = db.NC_TBL_ROOM_MASTER.GroupBy(R => new { R.Room_View, R.Room_Type, R.SO_Tariff, R.DO_Tariff }).ToList();
            // var allTarrifs = db.NC_TBL_ROOM_MASTER.ToList();
            //return Json(allTarrifs, JsonRequestBehavior.AllowGet);
            return PartialView("RoomTariff", allTarrifs.ToList());

        }

        public ActionResult PackageTariff()
        {


            var allPackageTarrifs = (from R in db.NC_TBL_PACKAGE_MASTER
                                     group R by new { R.PackageType, R.PackageName, R.Room_View, R.Room_Type, R.SO_Pkg_Amt, R.DO_Pkg_Amt, R.NoOfDays }
                                         into grp
                                         select new PackageTariffView
                                         {
                                             PackageType = grp.Key.PackageType,
                                             PackageName = grp.Key.PackageName,
                                             Room_View = grp.Key.Room_View,
                                             Room_Type = grp.Key.Room_Type,
                                             SO_Pkg_Amt = (decimal)grp.Key.SO_Pkg_Amt,
                                             DO_Pkg_Amt = (decimal)grp.Key.DO_Pkg_Amt,
                                             NoOfDays = (int)grp.Key.NoOfDays
                                         }).ToList();

            // var allTarrifs = db.NC_TBL_ROOM_MASTER.GroupBy(R => new { R.Room_View, R.Room_Type, R.SO_Tariff, R.DO_Tariff }).ToList();
            // var allTarrifs = db.NC_TBL_ROOM_MASTER.ToList();
            //return Json(allTarrifs, JsonRequestBehavior.AllowGet);
            return PartialView("PackageTariffs", allPackageTarrifs.ToList());

        }
        public ActionResult Help()
        {


            return PartialView("AccompanyTypeHelp");

        }
        [HttpPost]
        public ActionResult saveDatesConformation(FormCollection coll)
        {
            try
            {
                NC_TBL_BOOKED_DATES nc_tbl_booked_dates = new NC_TBL_BOOKED_DATES();

                nc_tbl_booked_dates.PackageType = coll["PackageType"].ToString();
                nc_tbl_booked_dates.PackageName = coll["PackageName"].ToString();
                nc_tbl_booked_dates.NoOfDays = Convert.ToInt32(coll["NoOfDays"].ToString());
                nc_tbl_booked_dates.ArrivalDate = DateTime.ParseExact(coll["ArrivalDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                nc_tbl_booked_dates.DepartureDate = DateTime.ParseExact(coll["DepartureDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                nc_tbl_booked_dates.AlternateArrivalDate = DateTime.ParseExact(coll["AlternateArrivalDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                nc_tbl_booked_dates.AlternateDepartureDate = DateTime.ParseExact(coll["AlternateDepartureDate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                nc_tbl_booked_dates.RoomView = coll["RoomView"].ToString();
                nc_tbl_booked_dates.RoomType = coll["RoomType"].ToString();
                nc_tbl_booked_dates.RoomTariff = Convert.ToInt32(coll["RoomTariff"].ToString());
                nc_tbl_booked_dates.Accompany_Type = coll["Accompany_Type"].ToString();
                nc_tbl_booked_dates.Guest_Attender_Name = coll["Guest_Attender_Name"].ToString();
                nc_tbl_booked_dates.AccompanyingGuestUHID = coll["AccompanyingGuestUHID"].ToString();
                nc_tbl_booked_dates.Package_Amount = Convert.ToInt32(coll["Package_Amount"].ToString());
                nc_tbl_booked_dates.Group_Code = coll["Group_Code"].ToString();
                string UserUHID = coll["UserUHID"].ToString();
                nc_tbl_booked_dates.UserType = coll["UserType"].ToString();
                nc_tbl_booked_dates.Transportation_Required = coll["Transportation_Required"].ToString();
                nc_tbl_booked_dates.Mem_Camp_Type = coll["Mem_Camp_Type"].ToString();
                string successmessage = "";
                try
                {

                    var prnum = Session["TMPPR"].ToString();
                    if (prnum == null || prnum == "")
                    {
                        ObjectParameter Unique_TPRNo = new ObjectParameter("uniqueno", typeof(String));
                        var UniqueNo = db.gen_unique_no("TPRN", Unique_TPRNo);

                        Session["TMPPR"] = Unique_TPRNo.Value;
                        Session["PRNO"] = Unique_TPRNo.Value;

                        if (nc_tbl_booked_dates.Accompany_Type != "-1")
                        {
                            if (nc_tbl_booked_dates.Accompany_Type == "Accompany")
                            {
                                ObjectParameter Unique_TPRNoAccompany = new ObjectParameter("uniqueno", typeof(String));
                                var UniqueNoAccompany = db.gen_unique_no("TPRN", Unique_TPRNoAccompany);
                                ViewBag.AccompanyTPRNO = Unique_TPRNoAccompany.Value;
                            }
                            else
                            {
                                ObjectParameter Unique_TPRNoARNO = new ObjectParameter("uniqueno", typeof(String));
                                var UniqueNoARNO = db.gen_unique_no("ARNO", Unique_TPRNoARNO);
                                ViewBag.ARNO = Unique_TPRNoARNO.Value;

                            }
                        }
                    }
                    var PRNumber = Session["TMPPR"].ToString();
                    nc_tbl_booked_dates.PRNO = PRNumber;
                    if (nc_tbl_booked_dates.UserType == "WalkinUser")
                    {
                        Session["TMPUH"] = UserUHID;
                    }

                    nc_tbl_booked_dates.UHID = Session["TMPUH"].ToString();

                    // var dd = nc_tbl_booked_dates.UHID;

                    //Session["TMPPR"] = ViewBag.PRNO.ToString();
                    var Booking = db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == nc_tbl_booked_dates.PRNO);
                    if (Booking == null)
                    {
                        NC_TBL_BOOKED_DATES save = new NC_TBL_BOOKED_DATES();
                        save.PRNO = nc_tbl_booked_dates.PRNO;
                        save.PackageType = nc_tbl_booked_dates.PackageType;
                        save.PackageName = nc_tbl_booked_dates.PackageName;
                        save.NoOfDays = nc_tbl_booked_dates.NoOfDays;
                        save.ArrivalDate = nc_tbl_booked_dates.ArrivalDate;
                        save.DepartureDate = nc_tbl_booked_dates.DepartureDate;
                        save.AlternateArrivalDate = nc_tbl_booked_dates.AlternateArrivalDate;
                        save.AlternateDepartureDate = nc_tbl_booked_dates.AlternateDepartureDate;
                        save.AccompanyingGuests = nc_tbl_booked_dates.AccompanyingGuests;
                        save.RoomType = nc_tbl_booked_dates.RoomType;
                        save.RoomView = nc_tbl_booked_dates.RoomView;
                        save.RoomTariff = nc_tbl_booked_dates.RoomTariff;
                        save.PackageConfirmStatus = "N";
                        save.CMOEscalated = "N";
                        save.ApplicationStatus = "Pending";
                        save.Accompany_Type = nc_tbl_booked_dates.Accompany_Type == "-1" ? null : nc_tbl_booked_dates.Accompany_Type;
                        save.Guest_Attender_Name = nc_tbl_booked_dates.Guest_Attender_Name;
                        save.Guest_Attender_ARNO = ViewBag.ARNO;
                        save.AccompanyingGuestUHID = nc_tbl_booked_dates.AccompanyingGuestUHID;
                        save.AccompanyingGuestPRNO = ViewBag.AccompanyTPRNO;
                        save.UHID = nc_tbl_booked_dates.UHID;
                        save.Package_Amount = nc_tbl_booked_dates.Package_Amount;
                        save.Group_Code = nc_tbl_booked_dates.Group_Code;
                        save.UserType = nc_tbl_booked_dates.UserType;
                        save.Transportation_Required = nc_tbl_booked_dates.Transportation_Required;
                        save.Mem_Camp_Type = nc_tbl_booked_dates.Mem_Camp_Type;


                        db.NC_TBL_BOOKED_DATES.Add(save);
                        db.SaveChanges();

                        //To up date Accompanying Guest Record 
                        if (nc_tbl_booked_dates.Accompany_Type == "Accompany")
                        {

                            NC_TBL_BOOKED_DATES saveAccompany = new NC_TBL_BOOKED_DATES();
                            saveAccompany.PRNO = ViewBag.AccompanyTPRNO;
                            saveAccompany.PackageType = nc_tbl_booked_dates.PackageType;
                            saveAccompany.PackageName = nc_tbl_booked_dates.PackageName;
                            saveAccompany.NoOfDays = nc_tbl_booked_dates.NoOfDays;
                            saveAccompany.ArrivalDate = nc_tbl_booked_dates.ArrivalDate;
                            saveAccompany.DepartureDate = nc_tbl_booked_dates.DepartureDate;
                            saveAccompany.AlternateArrivalDate = nc_tbl_booked_dates.AlternateArrivalDate;
                            saveAccompany.AlternateDepartureDate = nc_tbl_booked_dates.AlternateDepartureDate;
                            saveAccompany.RoomType = nc_tbl_booked_dates.RoomType;
                            saveAccompany.RoomView = nc_tbl_booked_dates.RoomView;
                            saveAccompany.RoomTariff = nc_tbl_booked_dates.RoomTariff;
                            saveAccompany.PackageConfirmStatus = "N";
                            saveAccompany.CMOEscalated = "N";
                            saveAccompany.AccompanyingGuests = 0;
                            saveAccompany.ApplicationStatus = "Pending";
                            saveAccompany.UHID = nc_tbl_booked_dates.AccompanyingGuestUHID;
                            saveAccompany.Package_Amount = nc_tbl_booked_dates.Package_Amount;
                            saveAccompany.Group_Code = nc_tbl_booked_dates.Group_Code;
                            save.UserType = nc_tbl_booked_dates.UserType;

                            db.NC_TBL_BOOKED_DATES.Add(saveAccompany);
                            db.SaveChanges();
                            successmessage = "Successfully Saved!";
                        }
                        else
                        {
                            successmessage = "Successfully Saved!";
                        }
                    }
                    else
                    {

                        NC_TBL_BOOKED_DATES existingbooked = db.NC_TBL_BOOKED_DATES.Find(Booking.Id);
                        //var xx = existingbooked.Accompany_Type == null ? "-1" : Booking.Accompany_Type;
                        //if (xx == nc_tbl_booked_dates.Accompany_Type)
                        //{
                        existingbooked.PackageType = nc_tbl_booked_dates.PackageType;
                        existingbooked.PackageName = nc_tbl_booked_dates.PackageName;
                        existingbooked.NoOfDays = nc_tbl_booked_dates.NoOfDays;
                        existingbooked.ArrivalDate = nc_tbl_booked_dates.ArrivalDate;
                        existingbooked.DepartureDate = nc_tbl_booked_dates.DepartureDate;
                        existingbooked.AlternateArrivalDate = nc_tbl_booked_dates.AlternateArrivalDate;
                        existingbooked.AlternateDepartureDate = nc_tbl_booked_dates.AlternateDepartureDate;
                        existingbooked.AccompanyingGuests = nc_tbl_booked_dates.AccompanyingGuests;
                        existingbooked.RoomType = nc_tbl_booked_dates.RoomType;
                        existingbooked.RoomView = nc_tbl_booked_dates.RoomView;
                        existingbooked.RoomTariff = nc_tbl_booked_dates.RoomTariff;
                        existingbooked.PackageConfirmStatus = "N";
                        existingbooked.CMOEscalated = "N";
                        existingbooked.ApplicationStatus = "Pending";
                        existingbooked.Accompany_Type = nc_tbl_booked_dates.Accompany_Type == "-1" ? null : nc_tbl_booked_dates.Accompany_Type;
                        existingbooked.Guest_Attender_Name = nc_tbl_booked_dates.Guest_Attender_Name;
                        existingbooked.AccompanyingGuestUHID = nc_tbl_booked_dates.AccompanyingGuestUHID;
                        // existingbooked.UHID = nc_tbl_booked_dates.UHID;
                        existingbooked.Package_Amount = nc_tbl_booked_dates.Package_Amount;
                        existingbooked.Group_Code = nc_tbl_booked_dates.Group_Code;
                        existingbooked.UserType = nc_tbl_booked_dates.UserType;
                        existingbooked.Transportation_Required = nc_tbl_booked_dates.Transportation_Required;
                        existingbooked.Mem_Camp_Type = nc_tbl_booked_dates.Mem_Camp_Type;

                        db.SaveChanges();
                        if (nc_tbl_booked_dates.Accompany_Type == "Accompany")
                        {
                            var AccompanyDetails = db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == Booking.AccompanyingGuestPRNO);
                            NC_TBL_BOOKED_DATES existingbookedAccompany = db.NC_TBL_BOOKED_DATES.Find(AccompanyDetails.Id);

                            existingbookedAccompany.PackageType = nc_tbl_booked_dates.PackageType;
                            existingbookedAccompany.PackageName = nc_tbl_booked_dates.PackageName;
                            existingbookedAccompany.NoOfDays = nc_tbl_booked_dates.NoOfDays;
                            existingbookedAccompany.ArrivalDate = nc_tbl_booked_dates.ArrivalDate;
                            existingbookedAccompany.DepartureDate = nc_tbl_booked_dates.DepartureDate;
                            existingbookedAccompany.AlternateArrivalDate = nc_tbl_booked_dates.AlternateArrivalDate;
                            existingbookedAccompany.AlternateDepartureDate = nc_tbl_booked_dates.AlternateDepartureDate;
                            existingbookedAccompany.RoomType = nc_tbl_booked_dates.RoomType;
                            existingbookedAccompany.RoomView = nc_tbl_booked_dates.RoomView;
                            existingbookedAccompany.RoomTariff = nc_tbl_booked_dates.RoomTariff;
                            existingbookedAccompany.PackageConfirmStatus = "N";
                            existingbookedAccompany.CMOEscalated = "N";
                            existingbookedAccompany.ApplicationStatus = "Pending";
                            existingbookedAccompany.UHID = nc_tbl_booked_dates.AccompanyingGuestUHID;
                            existingbookedAccompany.Package_Amount = nc_tbl_booked_dates.Package_Amount;
                            existingbookedAccompany.Group_Code = nc_tbl_booked_dates.Group_Code;
                            existingbookedAccompany.UserType = nc_tbl_booked_dates.UserType;
                            db.SaveChanges();


                            successmessage = "Successfully Saved!";
                        }
                        else
                        {
                            successmessage = "Successfully Saved!";
                        }
                    }

                    return Json(successmessage, JsonRequestBehavior.AllowGet);
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                        }

                    }
                    return Json(successmessage, JsonRequestBehavior.AllowGet);
                }
                // return Json(successmessage, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        public ActionResult OnlinePayment(string PRNO = "", int billTypeCode = 0)
        {
            ViewBag.PRNumber = PRNO;
            ViewBag.billcode = billTypeCode;
            if (base.Session["TMPPR"] != null && base.Session["MEMID"] == null)
            {
                if (base.Session["TMPPR"] == null)
                {
                    base.Session["TMPPR"] = base.Session["PRNO"];
                }
                string str = base.Session["TMPPR"].ToString();
                if ((
                    from e in this.db.NC_TBL_BOOKED_DATES
                    where e.AccompanyingGuestPRNO == str
                    select e).ToList<NC_TBL_BOOKED_DATES>().Count<NC_TBL_BOOKED_DATES>() < 1)
                {
                    NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE = (
                        from e in this.db.NC_TBL_BOOKED_DATES
                        where e.PRNO == str
                        select e).FirstOrDefault<NC_TBL_BOOKED_DATES>();
                    if (nCTBLBOOKEDDATE.Accompany_Type != "Accompany")
                    {
                        ((dynamic)base.ViewBag).MainGuest = "No";
                    }
                    else
                    {
                        var variable = (
                            from a in this.db.nc_tbl_bill_type_master
                            join b in this.db.NC_TBL_Billing_Trans on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                            join c in this.db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO
                            where (b.PR_NO == str) && (b.Paid_status == "N") && (b.Bill_TYPE_CODE == 1 || b.Bill_TYPE_CODE == 3)
                            select new { PRNO = b.PR_NO, Name = (((c.FirstName + " ") + c.MiddleName) + " ") + c.LastName, BillCode = b.Bill_TYPE_CODE, BillType = a.BILL_TYPE, Amount = b.NET_AMOUNT }).FirstOrDefault();
                        if (variable == null)
                        {
                            ((dynamic)base.ViewBag).MainGuest = "No";
                        }
                        else if ((
                            from c in this.db.NC_TBL_PAYMENT_TRANS
                            where (c.PR_NO == nCTBLBOOKEDDATE.AccompanyingGuestPRNO) && c.PAYMENT_TYPE_CODE == variable.BillCode
                            select c).FirstOrDefault<NC_TBL_PAYMENT_TRANS>() != null)
                        {
                            ViewBag.MainGuest = "No";
                        }
                        else
                        {
                            ViewBag.MainGuest = "yes";
                        }
                    }
                }
                else
                {
                    ViewBag.MainGuest = "No";
                }
            }
            return base.View();
        }


        //public JsonResult GetMembershipDetails(string PRNO, int BillCode)
        //           {
        //               string Mem_ID = "";

        //               if (PRNO != null)
        //               {
        //                   Mem_ID = PRNO;
        //               }
        //               else
        //               {
        //                   if (Session["MEMID"] != null)
        //                   {
        //                       Mem_ID = Session["MEMID"].ToString();

        //                   }
        //                   else if (Session["UHID"] != null)
        //                   {

        //                       var uhid = Session["UHID"].ToString();

        //                       Mem_ID = db.NC_TBL_MEMBERSHIPDETAILS_HEADER.Where(a => a.UHID == uhid).Select(b => b.Mem_Id).FirstOrDefault();
        //                   }
        //               }

        //        var queryOnlinePayment1 = (from a in db.NC_TBL_Billing_Trans
        //                                   join b in db.NC_TBL_MEMBERSHIPDETAILS_HEADER
        //                                   on a.PR_NO equals b.Mem_Id
        //                                   join c in db.WS_Tbl_Registration
        //                                    on b.UHID equals c.UHID
        //                                   where b.Mem_Id == Mem_ID && a.Paid_status == "N"
        //                                   && (a.Bill_TYPE_CODE == 14)
        //                                   select new
        //                                   {
        //                                       PRNO = b.Mem_Id,
        //                                       Name = c.first_name + " " + c.middle_name + " " + c.last_name,
        //                                       BillCode = a.Bill_TYPE_CODE,
        //                                       BillType = db.nc_tbl_bill_type_master.Where(x => x.Bill_TYPE_CODE == 14).Select(y => y.BILL_TYPE).FirstOrDefault(),
        //                                       Amount = a.NET_AMOUNT
        //                                   }).FirstOrDefault();               

        //        return Json(queryOnlinePayment1, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult DataOnlinePayment(bool combined, string PRNO, int BillCode)
        {
            string str1 = "";
            List<int> nums = new List<int>();
            if (PRNO != "")
            {
                str1 = PRNO.ToString();
                nums.Add(BillCode);
            }
            else if (base.Session["TMPPR"] != null)
            {
                str1 = base.Session["TMPPR"].ToString();
                nums.Add(3);
            }
            else if (base.Session["MEMID"] != null)
            {
                str1 = base.Session["MEMID"].ToString();
                nums.Add(14);
            }
            if (nums.Count<int>() == 1 && nums.Contains(14))
            {
                var variable = (
                    from a in this.db.NC_TBL_Billing_Trans
                    join b in this.db.NC_TBL_MEMBERSHIPDETAILS_HEADER on a.PR_NO equals b.Mem_Id
                    join c in this.db.WS_Tbl_Registration on b.UHID equals c.UHID
                    where (b.Mem_Id == str1) && (a.Paid_status == "N") && a.Bill_TYPE_CODE == 14
                    select new { PRNO = b.Mem_Id, Name = (((c.first_name + " ") + c.middle_name) + " ") + c.last_name, BillCode = a.Bill_TYPE_CODE, BillType = this.db.nc_tbl_bill_type_master.Where<nc_tbl_bill_type_master>((nc_tbl_bill_type_master x) => x.Bill_TYPE_CODE == 14).Select<nc_tbl_bill_type_master, string>((nc_tbl_bill_type_master y) => y.BILL_TYPE).FirstOrDefault<string>(), Amount = a.NET_AMOUNT }).FirstOrDefault();
                return base.Json(variable, JsonRequestBehavior.AllowGet);
            }
            if (combined)
            {
                var variable1 = (
                    from a in this.db.nc_tbl_bill_type_master
                    join b in this.db.NC_TBL_Billing_Trans on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                    join c in this.db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO
                    where (b.PR_NO == str1) && (b.Paid_status == "N") && nums.Contains(b.Bill_TYPE_CODE)
                    select new { PRNO = b.PR_NO, Name = (((c.FirstName + " ") + c.MiddleName) + " ") + c.LastName, BillCode = b.Bill_TYPE_CODE, BillType = a.BILL_TYPE, Amount = b.NET_AMOUNT * (decimal?)(new decimal(2)) }).FirstOrDefault();
                return base.Json(variable1, JsonRequestBehavior.AllowGet);
            }
            NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE = this.db.NC_TBL_BOOKED_DATES.FirstOrDefault<NC_TBL_BOOKED_DATES>((NC_TBL_BOOKED_DATES i) => i.AccompanyingGuestPRNO == str1);
            NC_TBL_BOOKED_DATES nCTBLBOOKEDDATE1 = this.db.NC_TBL_BOOKED_DATES.FirstOrDefault<NC_TBL_BOOKED_DATES>((NC_TBL_BOOKED_DATES i) => i.PRNO == str1);
            List<PendingPayment> pendingPayments = new List<PendingPayment>();
            if (nCTBLBOOKEDDATE != null)
            {
                var variable2 = (
                    from a in this.db.nc_tbl_bill_type_master.AsEnumerable<nc_tbl_bill_type_master>()
                    join b in this.db.NC_TBL_Billing_Trans on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                    join c in this.db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO
                    select new {  c,b,a }).Where((argument0) => {
                        if (!(argument0.b.PR_NO == str1) || !(argument0.b.Paid_status == "N"))
                        {
                            return false;
                        }
                        return nums.Contains(argument0.b.Bill_TYPE_CODE);
                    }).Select((argument7) => new { PRNO = argument7.b.PR_NO, Name = string.Concat(new string[] { argument7.c.FirstName, " ", argument7.c.MiddleName, " ", argument7.c.LastName }), BillCode = argument7.b.Bill_TYPE_CODE, BillType = argument7.a.BILL_TYPE, Amount = Convert.ToDecimal(argument7.b.NET_AMOUNT) }).FirstOrDefault();
                return base.Json(variable2, JsonRequestBehavior.AllowGet);
            }
            if (nCTBLBOOKEDDATE1.Accompany_Type != "Accompany")
            {
                var variable4 = (
                    from a in this.db.nc_tbl_bill_type_master.AsEnumerable<nc_tbl_bill_type_master>()
                    join b in this.db.NC_TBL_Billing_Trans on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                    join c in this.db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO
                    select new { a,b,c }).Where((argument1) => {
                        if (!(argument1.b.PR_NO == str1) || !(argument1.b.Paid_status == "N"))
                        {
                            return false;
                        }
                        return nums.Contains(argument1.b.Bill_TYPE_CODE);
                    }).Select((argument10) => new { PRNO = argument10.b.PR_NO, Name = string.Concat(new string[] { argument10.c.FirstName, " ", argument10.c.MiddleName, " ", argument10.c.LastName }), BillCode = argument10.b.Bill_TYPE_CODE, BillType = argument10.a.BILL_TYPE, Amount = Convert.ToDecimal(argument10.b.NET_AMOUNT) }).FirstOrDefault();
                return base.Json(variable4, JsonRequestBehavior.AllowGet);
            }
            NC_TBL_Billing_Trans nCTBLBillingTran = (
                from bill in this.db.NC_TBL_Billing_Trans
                where (bill.PR_NO == nCTBLBOOKEDDATE1.AccompanyingGuestPRNO) && bill.Bill_TYPE_CODE == 3
                select bill).FirstOrDefault<NC_TBL_Billing_Trans>();
            if (nCTBLBillingTran.Paid_status == "N")
            {
                var variable6 = (
                    from a in this.db.nc_tbl_bill_type_master.AsEnumerable<nc_tbl_bill_type_master>()
                    join b in this.db.NC_TBL_Billing_Trans on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                    join c in this.db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO
                    select new { a,b,c }).Where((argument2) => {
                        if (!(argument2.b.PR_NO == str1) || !(argument2.b.Paid_status == "N"))
                        {
                            return false;
                        }
                        return nums.Contains(argument2.b.Bill_TYPE_CODE);
                    }).Select((argument4) => {
                        string pRNO = argument4.b.PR_NO;
                        string str = string.Concat(new string[] { argument4.c.FirstName, " ", argument4.c.MiddleName, " ", argument4.c.LastName });
                        int billTYPECODE = argument4.b.Bill_TYPE_CODE;
                        string bILLTYPE = argument4.a.BILL_TYPE;
                        decimal? nETAMOUNT = argument4.b.NET_AMOUNT;
                        decimal? nullable = nCTBLBillingTran.NET_AMOUNT;
                        return new { PRNO = pRNO, Name = str, BillCode = billTYPECODE, BillType = bILLTYPE, Amount = Convert.ToDecimal((nETAMOUNT.HasValue & nullable.HasValue ? new decimal?(nETAMOUNT.GetValueOrDefault() + nullable.GetValueOrDefault()) : null)) };
                    }).FirstOrDefault();
                return base.Json(variable6, JsonRequestBehavior.AllowGet);
            }
            var variable8 = (
                from a in this.db.nc_tbl_bill_type_master.AsEnumerable<nc_tbl_bill_type_master>()
                join b in this.db.NC_TBL_Billing_Trans on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                join c in this.db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO
                select new { a,b,c }).Where((argument3) => {
                    if (!(argument3.b.PR_NO == str1) || !(argument3.b.Paid_status == "N"))
                    {
                        return false;
                    }
                    return nums.Contains(argument3.b.Bill_TYPE_CODE);
                }).Select((argument15) => new { PRNO = argument15.b.PR_NO, Name = string.Concat(new string[] { argument15.c.FirstName, " ", argument15.c.MiddleName, " ", argument15.c.LastName }), BillCode = argument15.b.Bill_TYPE_CODE, BillType = argument15.a.BILL_TYPE, Amount = Convert.ToDecimal(argument15.b.NET_AMOUNT) }).FirstOrDefault();
            return base.Json(variable8, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CheckRoomAvailability()
        {
            var AvailableRoomNo = "";
            if (Session["TMPPR"] == null)
            {
                Session["TMPPR"] = Session["PRNO"];
            }

            var PRNum = Session["TMPPR"].ToString();


            //var checkPRAccompany = (from e in db.NC_TBL_BOOKED_DATES
            //                        where e.AccompanyingGuestPRNO == PRNum
            //                        select e).ToList();
            //if (checkPRAccompany.Count() >= 1)
            //{
            //    AvailableRoomNo = "AccompanyGuest";
            //}
            //else
            //{

            var vres = getdetails(PRNum);
            var prtype = vres.Split(',')[0];
            var oppPRNO = vres.Split(',')[1];
            var oppname = vres.Split(',')[2];
            NC_TBL_GUEST_ROOM_TRANSACTION roomres = new NC_TBL_GUEST_ROOM_TRANSACTION();
            //if (oppPRNO!="No_Acc" && oppPRNO!=null)
            //{
            roomres = db.NC_TBL_GUEST_ROOM_TRANSACTION.Where(x => x.PRNO == oppPRNO).FirstOrDefault();
            //}

            if (roomres != null)
            {
                AvailableRoomNo = "RoomAlreadyBooked to Accompany";
            }
            else
            {

                var Packagedetails = db.NC_TBL_BOOKED_DATES.Where(x => x.PRNO == PRNum).FirstOrDefault();

                var result = from y in db.NC_TBL_ROOM_MASTER
                             where !(from x in db.NC_TBL_GUEST_ROOM_TRANSACTION
                                     select x.Room_No
                                     ).Contains(y.Room_No) && y.Active == "Y" && y.Active == "Y" && y.Room_View == Packagedetails.RoomView && y.Room_Type == Packagedetails.RoomType
                             select y;

                if (result.Count() == 0)
                {
                    var per = db.NC_TBL_ROOM_MASTER.Where(a => a.Room_View == Packagedetails.RoomView && a.Room_Type == Packagedetails.RoomType && a.Active == "Y").ToList();
                    foreach (NC_TBL_ROOM_MASTER cust in per)
                    {
                        if (AvailableRoomNo == "")
                        {
                            var exist = db.NC_TBL_GUEST_ROOM_TRANSACTION.Where(a => a.Room_No == cust.Room_No).ToList();
                            if (exist != null)
                            {
                                var query1 = (from b in db.NC_TBL_GUEST_ROOM_TRANSACTION
                                              where b.Room_No == cust.Room_No
                                               && ((b.ArrivalDate >= Packagedetails.ConfirmedFromDate && b.ArrivalDate < Packagedetails.ConfirmedToDate) || (b.DepartureDate > Packagedetails.ConfirmedFromDate && b.DepartureDate <= Packagedetails.ConfirmedToDate) || (b.ArrivalDate < Packagedetails.ConfirmedFromDate && b.DepartureDate > Packagedetails.ConfirmedToDate))
                                              select new
                                              {
                                                  RoomNo = b.Room_No,
                                              }).ToList();

                                if (query1.Count != 0)
                                {
                                }
                                else
                                {
                                    AvailableRoomNo = cust.Room_No;
                                }
                            }
                            else
                            {
                                AvailableRoomNo = cust.Room_No;
                            }
                        }
                    }
                }
                else
                {
                    AvailableRoomNo = result.FirstOrDefault().Room_No;
                }
            }
            return Json(AvailableRoomNo, JsonRequestBehavior.AllowGet);
        }



        //public ActionResult SavePaymentDetails(NC_TBL_PAYMENT_TRANS nc_tbl_payment_trans, bool combined)
        //{
        //    var PPR = "";
        //    //**************************************************************
        //    ///Starting of Single payment
        //    ///
        //    //**************************************************************
        //    if (combined == false)
        //    {
        //        var PaymentPR = db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == nc_tbl_payment_trans.PR_NO && i.PAYMENT_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE);
        //        if (PaymentPR == null)
        //        {

        //            //    var ReceiptNo = getUniqueno("RCPT");


        //            //    var PaymentDetails = new NC_TBL_PAYMENT_TRANS();

        //            //    PaymentDetails.PR_NO = nc_tbl_payment_trans.PR_NO.ToString();
        //            //    PaymentDetails.RECEIPT_NO = ReceiptNo;
        //            //    PaymentDetails.TXN_DATE = DateTime.Now;
        //            //    PaymentDetails.ACTUAL_PAID_DATE = DateTime.Now;
        //            //    PaymentDetails.COUNTER_NO = 0;
        //            //    PaymentDetails.PAID_AMOUNT = nc_tbl_payment_trans.PAID_AMOUNT;
        //            //    PaymentDetails.PAYMENT_TYPE_CODE = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
        //            //    PaymentDetails.PAYMENT_MODE_CODE = 4;
        //            //    PaymentDetails.FINAL_STATUS = "N";
        //            //    PaymentDetails.RECEIPT_CANCELLED = "N";

        //            //if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 14)
        //            //{
        //            //    PaymentDetails.EntryType = "M";
        //            //}
        //            //else
        //            //{
        //            //    PaymentDetails.EntryType = "P";
        //            //}
        //            //    db.NC_TBL_PAYMENT_TRANS.Add(PaymentDetails);
        //            //    db.SaveChanges();



        //            //if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 1)
        //            //{
        //            //    //var TPRNO = Session["PRNO"].ToString();
        //            //    var BookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == nc_tbl_payment_trans.PR_NO.ToString()).FirstOrDefault();
        //            //    NC_TBL_BOOKED_DATES existingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(BookedDetails.Id);
        //            //    if (existingDatesBooking != null)
        //            //    {
        //            //        existingDatesBooking.ApplicationStatus = "Completed";
        //            //    }
        //            //    db.SaveChanges();
        //            //}


        //            if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 1)
        //            {
        //                PPR = "Payment Success";
        //               // PPR = "Your Application is Successfully submitted ...Please note PRNO " + Session["TMPPR"].ToString() + " for your future communication with PEMA";
        //            }

        //            else
        //            {
        //                if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 3)
        //                {
        //                    PPR = "Advance Payment Success";
        //                }
        //                else if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 14)
        //                {
        //                    //var id = db.NC_TBL_MEMBERSHIPDETAILS_HEADER.Where(i => i.Mem_Id == nc_tbl_payment_trans.PR_NO.ToString() && i.Mem_Payment_Status == "N").Select(b=>b.ID).FirstOrDefault();

        //                    var id = (from dis in db.NC_TBL_MEMBERSHIPDETAILS_HEADER
        //                              where dis.Mem_Id == nc_tbl_payment_trans.PR_NO && dis.Mem_Payment_Status == "N"
        //                              orderby dis.ID descending
        //                              select dis.ID).FirstOrDefault();

        //                    NC_TBL_MEMBERSHIPDETAILS_HEADER MEMBERSHIPDETAILS_HEADER = db.NC_TBL_MEMBERSHIPDETAILS_HEADER.Find(id);

        //                    if (MEMBERSHIPDETAILS_HEADER != null)
        //                    {
        //                        MEMBERSHIPDETAILS_HEADER.Mem_Payment_Status = "Y";
        //                    }

        //                    db.SaveChanges();
        //                    PPR = "Membership Payment Success";
        //                }
        //                else
        //                {
        //                    PPR = Session["TMPPR"].ToString();
        //                }

        //            }
        //            Session["TMPPR"] = null;

        //            //To update Paid Status in NC_TBL_Billing_Trans table
        //            //var billtrans = db.NC_TBL_Billing_Trans.Where(i => i.PR_NO == nc_tbl_payment_trans.PR_NO && i.Bill_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE && i.Paid_status == "N").FirstOrDefault();
        //            //NC_TBL_Billing_Trans existingbilltrans = db.NC_TBL_Billing_Trans.Find(billtrans.Id);
        //            //if (existingbilltrans != null)
        //            //{
        //            //    existingbilltrans.Paid_status = "Y";

        //            //    db.SaveChanges();
        //            var res = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
        //            var prnum = db.NC_TBL_PERSONAL_DETAILS.Where(x => x.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
        //            //if (res == 1)
        //            //{
        //            //    var email = prnum.EmailId;
        //            //    var name = prnum.FirstName + " " + prnum.LastName;
        //            //    var PRNO = nc_tbl_payment_trans.PR_NO;
        //            //    var UHID = prnum.UHID;
        //            //    MailMessage mail = new MailMessage();
        //            //    mail.To.Add(email);
        //            //    mail.From = new MailAddress("nhcl.pema@gmail.com");
        //            //    mail.Subject = "PEMA Reservation Details" + "[PRNO: " + PRNO + "]";
        //            //    // string Body = "<table><tr><td >Dear " + name + "</td> </tr><tr><td>Your reservation with PEMA is Confirmed </td></tr><tr><td>Our Chief doctor will verify the details and get back to you<br> with the treatment and package details to get better<br> results</td></tr><tr><td >Please Use The Following PRNO For<br> your Reference when you visit the<br> PEMA</td></tr><tr><td >UHID:" + UHID + "</td></tr><tr><td >PRNO:" + PRNO + "</td></tr></table>";

        //            //    mbody = new StringBuilder();
        //            //    mbody.Append(mheader);

        //            //    mbody.Append("<table> ");
        //            //    mbody.Append("<tr><td><h2 style='float:left; margin-left:100px;'>Reservation  Details</h2></td></tr> ");
        //            //    mbody.Append("<tr> <td>Dear " + name + ",</td> </tr> ");
        //            //    mbody.Append("<tr> <td>&nbsp;&nbsp;&nbsp;</td> </tr>  ");

        //            //    mbody.Append("<tr> <td> ");
        //            //    mbody.Append("Greetings from PEMA WELLNESS, Vizag.<br /><br /> ");
        //            //    mbody.Append("We thank you for choosing PEMA WELLNESS services.<br /><br />");
        //            //    mbody.Append("We confirm the receipt of your Application.<br /><br />");
        //            //    mbody.Append("Your application has been forwarded to our chief Medical Officer for review and suggestions.<br /><br />");
        //            //    mbody.Append("The suggested Treatments and Package details will be mailed to you once our Chief Medical Officer reviews your application.<br /><br />");
        //            //    mbody.Append("The Advance Payment need to be done against the suggested package to confirm your booking.<br /><br />");
        //            //    mbody.Append("Please use the following ID details for further communication with PEMA WELLNESS.<br /><br />");
        //            //    mbody.Append("UHID [Life Time ID] :&nbsp;<b style='color:blue'>" + UHID + "</b> <br /><br /> ");
        //            //    mbody.Append("PRNO [Visit ID] :&nbsp;<b style='color:blue'>" + PRNO + "</b><br /><br />");

        //            //    mbody.Append("</td>  </tr>  ");

        //            //    mbody.Append("</table>");

        //            //    mbody.Append(mfooter);

        //            //    mail.Body = mbody.ToString();
        //            //    mail.IsBodyHtml = true;

        //            //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(customCertValidation);






        //            //    SmtpClient smtp = new SmtpClient();
        //            //    smtp.Host = "smtp.gmail.com";
        //            //    smtp.Port = 587;
        //            //    smtp.UseDefaultCredentials = false;

        //            //    smtp.Credentials = new System.Net.NetworkCredential
        //            //    ("nhcl.pema@gmail.com", "Pema@123");// Enter seders User name and password
        //            //    smtp.EnableSsl = true;
        //            //    smtp.Send(mail);
        //            //}
        //            //else if (res == 3)
        //            //{
        //            //    //var email = prnum.EmailId;
        //            //    //var Booked = db.NC_TBL_BOOKED_DATES.Where(x => x.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
        //            //    //var PackageName = Booked.PackageName;
        //            //    //var Paymenttype = db.NC_TBL_PAYMENT_TRANS.Where(x => x.PAYMENT_TYPE_CODE == 3 && x.PR_NO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
        //            //    //var amount = Paymenttype.PAID_AMOUNT;
        //            //    //var TransactionDate = Paymenttype.TXN_DATE;
        //            //    //var chekindate = Booked.ConfirmedFromDate;
        //            //    //var name = prnum.FirstName + " " + prnum.LastName;
        //            //    //var PRNO = nc_tbl_payment_trans.PR_NO;
        //            //MailMessage mail = new MailMessage();
        //            //mail.To.Add(email);
        //            //mail.From = new MailAddress("nhcl.pema@gmail.com");
        //            //mail.Subject = "PEMA Booking Confirmation" + "[PRNO: " + PRNO + "]";
        //            //// string Body = "<table style='border-color:#707070'><tr><td style='color:#0000FF'>Dear " +name+ "</td> </tr><tr><td>Thank You for choosing PEMA Resorts, its our pleasure to serve you</td></tr><tr><td style='border:groove;border-width:thick;border-color:beige;'>The following are your transaction details</td></tr><tr><td style='color:#0000FF';>Package Name: " +PackageName+ "</td></tr><tr><td style='color:#0000FF'>PR NO: " +PRNO+ "</td></tr><tr><td style='color:#0000FF'>Amount: " +amount+ "</td></tr><tr><td style='color:#0000FF'>Transaction Date: " +TransactionDate+ "</td></tr><tr><td>Note: Please make sure to avaialable for check-in on: " +chekindate+ "</td></tr></table>";

        //            //mbody = new StringBuilder();
        //            //mbody.Append(mheader);

        //            //mbody.Append("<table> ");
        //            //mbody.Append("<tr><td><h2 style='float:left; margin-left:100px;'>Booking Confirmation</h2></td></tr> ");
        //            //mbody.Append("<tr> <td>Dear " + name + ",</td> </tr> ");
        //            //mbody.Append("<tr> <td>&nbsp;&nbsp;&nbsp;</td> </tr>  ");

        //            //mbody.Append("<tr> <td> ");

        //            //mbody.Append("Greetings from PEMA WELLNESS, Vizag. <br /><br />");
        //            //mbody.Append("We thank you for  making the advanced payment. We acknowledge the same. <br /><br />");
        //            //mbody.Append("Your Treatment and Package booking is now confirmed <br /><br />");
        //            //mbody.Append("<table>");
        //            //mbody.Append("<tr><td>Package Name     </td><td>:</td><td> " + PackageName + " </td></tr>");
        //            //mbody.Append("<tr><td>PR NO     </td><td>:</td><td> " + PRNO + " </td></tr>");
        //            //mbody.Append("<tr><td>Amount     </td><td>:</td><td> " + amount + " </td></tr>");
        //            //mbody.Append("<tr><td>Transaction Date     </td><td>:</td><td> " + TransactionDate + " </td></tr>");
        //            //mbody.Append("<tr><td>Check-In Date     </td><td>:</td><td>" + chekindate + "</td></tr>");
        //            //mbody.Append("</table><br />  ");

        //            //mbody.Append("We look forward to see you at our wellness centre on " + chekindate + ". Our Check-in time is 10:30 AM. <br /><br />");
        //            //mbody.Append("Please share your itinerary once it is finalized with us so that we can serve you better. <br /><br />");

        //            //mbody.Append("Please use the PRNO specified above for further communication with PEMA WELLNESS. <br /><br />");

        //            //mbody.Append("</td>  </tr>  ");

        //            //mbody.Append("</table>");

        //            //mbody.Append(mfooter);

        //            ////string ram = "Pr no" + Session["TMPPR"];
        //            //mail.Body = mbody.ToString();
        //            //mail.IsBodyHtml = true;
        //            ////System.Net.Mail.Attachment attachment;
        //            ////attachment = new System.Net.Mail.Attachment("your attachment file");
        //            ////mail.Attachments.Add(attachment);
        //            //SmtpClient smtp = new SmtpClient();
        //            //smtp.Host = "smtp.gmail.com";
        //            //smtp.Port = 587;
        //            //smtp.UseDefaultCredentials = false;
        //            //smtp.Credentials = new System.Net.NetworkCredential
        //            //("nhcl.pema@gmail.com", "Pema@123");// Enter seders User name and password
        //            //smtp.EnableSsl = true;
        //            //smtp.Send(mail);






        //            //    var vres = getdetails(nc_tbl_payment_trans.PR_NO);
        //            //    var prtype = vres.Split(',')[0];
        //            //    var oppPRNO = vres.Split(',')[1];
        //            //    var oppname = vres.Split(',')[2];


        //            //    var AvailableRoomNo = "";
        //            //    NC_TBL_GUEST_ROOM_TRANSACTION roomres = new NC_TBL_GUEST_ROOM_TRANSACTION();
        //            //    var Packagedetails = db.NC_TBL_BOOKED_DATES.Where(x => x.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();

        //            //    if (prtype.ToString() != "No_Acc")
        //            //    {
        //            //        roomres = db.NC_TBL_GUEST_ROOM_TRANSACTION.Where(x => x.PRNO == oppPRNO).FirstOrDefault();
        //            //    }


        //            //    if (roomres != null)
        //            //    {
        //            //        AvailableRoomNo = roomres.Room_No;

        //            //    }
        //            //    else
        //            //    {




        //            //        var result = from y in db.NC_TBL_ROOM_MASTER
        //            //                     where !(from x in db.NC_TBL_GUEST_ROOM_TRANSACTION
        //            //                             select x.Room_No
        //            //                             ).Contains(y.Room_No) && y.Active == "Y" && y.Room_View == Packagedetails.RoomView && y.Room_Type == Packagedetails.RoomType
        //            //                     select y;

        //            //        if (result.Count() == 0)
        //            //        {

        //            //            var per = db.NC_TBL_ROOM_MASTER.Where(a => a.Room_View == Packagedetails.RoomView && a.Room_Type == Packagedetails.RoomType && a.Active == "Y").ToList();
        //            //            foreach (NC_TBL_ROOM_MASTER cust in per)
        //            //            {
        //            //                if (AvailableRoomNo == "")
        //            //                {
        //            //                    var exist = db.NC_TBL_GUEST_ROOM_TRANSACTION.Where(a => a.Room_No == cust.Room_No).ToList();
        //            //                    if (exist != null)
        //            //                    {



        //            //                        var query1 = (from b in db.NC_TBL_GUEST_ROOM_TRANSACTION
        //            //                                      where b.Room_No == cust.Room_No
        //            //                                      && ((b.ArrivalDate >= Packagedetails.ConfirmedFromDate && b.ArrivalDate < Packagedetails.ConfirmedToDate) || (b.DepartureDate > Packagedetails.ConfirmedFromDate && b.DepartureDate <= Packagedetails.ConfirmedToDate)|| (b.ArrivalDate < Packagedetails.ConfirmedFromDate && b.DepartureDate > Packagedetails.ConfirmedToDate))
        //            //                                      select new
        //            //                                      {
        //            //                                          RoomNo = b.Room_No,

        //            //                                      }).ToList();

        //            //                        if (query1.Count != 0)
        //            //                        {


        //            //                        }
        //            //                        else
        //            //                        {
        //            //                            AvailableRoomNo = cust.Room_No;

        //            //                        }

        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        AvailableRoomNo = cust.Room_No;

        //            //                    }
        //            //                }
        //            //            }
        //            //        }
        //            //        else
        //            //        {
        //            //            AvailableRoomNo = result.FirstOrDefault().Room_No;
        //            //        }
        //            //    }

        //            //    var RoomTrans = db.NC_TBL_GUEST_ROOM_TRANSACTION.FirstOrDefault(i => i.PRNO == nc_tbl_payment_trans.PR_NO);
        //            //    if (RoomTrans == null)
        //            //    {
        //            //        var RoomTransaction = new NC_TBL_GUEST_ROOM_TRANSACTION();

        //            //        RoomTransaction.Room_No = AvailableRoomNo;
        //            //        RoomTransaction.ArrivalDate = Convert.ToDateTime(Packagedetails.ConfirmedFromDate);
        //            //        RoomTransaction.DepartureDate = Convert.ToDateTime(Packagedetails.ConfirmedToDate);

        //            //        RoomTransaction.PRNO = nc_tbl_payment_trans.PR_NO;
        //            //        RoomTransaction.Status = "Blocked";
        //            //        RoomTransaction.CheckedOut = "N";
        //            //        db.NC_TBL_GUEST_ROOM_TRANSACTION.Add(RoomTransaction);
        //            //        db.SaveChanges();

        //            //    }

        //            //    ////End of Guest Room transaction Table Room Blocked




        //            //}

        //            //PPR = Session["TMPPR"].ToString();
        //            // successmessage = "Successfully Saved!";
        //            //}

        //        }
        //    }
        //    //**************************************************************
        //    ///End of single payment
        //    //**************************************************************


        //  //**************************************************************
        //    ///Starting of Combined payment
        //    //**************************************************************
        //    else
        //    {
        //        var PaymentPR = db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == nc_tbl_payment_trans.PR_NO && i.PAYMENT_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE);
        //        if (PaymentPR == null)
        //        {

        //            //var ReceiptNo = getUniqueno("RCPT");



        //            //var PaymentDetails = new NC_TBL_PAYMENT_TRANS();

        //            //PaymentDetails.PR_NO = nc_tbl_payment_trans.PR_NO.ToString();
        //            //PaymentDetails.RECEIPT_NO = ReceiptNo;
        //            //PaymentDetails.TXN_DATE = DateTime.Now;
        //            //PaymentDetails.ACTUAL_PAID_DATE = DateTime.Now;
        //            //PaymentDetails.COUNTER_NO = 0;
        //            //PaymentDetails.PAID_AMOUNT = nc_tbl_payment_trans.PAID_AMOUNT / 2;
        //            //PaymentDetails.PAYMENT_TYPE_CODE = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
        //            //PaymentDetails.PAYMENT_MODE_CODE = 4;
        //            //PaymentDetails.FINAL_STATUS = "N";
        //            //PaymentDetails.RECEIPT_CANCELLED = "N";
        //            //db.NC_TBL_PAYMENT_TRANS.Add(PaymentDetails);
        //            //db.SaveChanges();


        //            //To Accompany guest payment record

        //            var PackageDetails = db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == nc_tbl_payment_trans.PR_NO.ToString());

        //            //var recordBillTrans = (from billtrans in db.NC_TBL_Billing_Trans select billtrans.BillNo).ToList();
        //            //if (recordBillTrans.Count == 0)
        //            //{
        //            //    ViewBag.BillNo = 1;
        //            //}
        //            //else
        //            //{
        //            //    ViewBag.BillNo = (from billtrans in db.NC_TBL_Billing_Trans select billtrans.BillNo).Max() + 1;
        //            //}



        //            //var BillPR = db.NC_TBL_Billing_Trans.FirstOrDefault(i => i.PR_NO == PackageDetails.AccompanyingGuestPRNO && i.Bill_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE);
        //            //if (BillPR == null)
        //            //{
        //            //    var BillDetails = new NC_TBL_Billing_Trans();

        //            //    BillDetails.PR_NO = PackageDetails.AccompanyingGuestPRNO;
        //            //    BillDetails.TAX = 0;
        //            //    BillDetails.Discount = 0;
        //            //    BillDetails.Je_NARRATION = "";
        //            //    BillDetails.Je_AMOUNT = 0;
        //            //    BillDetails.TOTAL_AMOUNT = nc_tbl_payment_trans.PAID_AMOUNT / 2;
        //            //    BillDetails.NET_AMOUNT = nc_tbl_payment_trans.PAID_AMOUNT / 2;
        //            //    BillDetails.Bill_TYPE_CODE = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
        //            //    BillDetails.BillNo = ViewBag.BillNo;
        //            //    BillDetails.billed_date = DateTime.Now;
        //            //    BillDetails.Paid_status = "N";
        //            //    BillDetails.MASTER_UPDATED = "N";
        //            //    BillDetails.Staff_code = 0;
        //            //    BillDetails.Remarks = "";
        //            //    db.NC_TBL_Billing_Trans.Add(BillDetails);
        //            //    db.SaveChanges();

        //            //}
        //            //var PaymentAccompanyPR = db.NC_TBL_PAYMENT_TRANS.FirstOrDefault(i => i.PR_NO == PackageDetails.AccompanyingGuestPRNO && i.PAYMENT_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE);
        //            //if (PaymentAccompanyPR == null)
        //            //{
        //            //var recordPayment1 = (from Payment in db.NC_TBL_PAYMENT_TRANS select Payment.RECEIPT_NO).ToList();
        //            //if (recordPayment1.Count == 0)
        //            //{
        //            //    ViewBag.ReceiptNo = 1;
        //            //}
        //            //else
        //            //{
        //            //    ViewBag.ReceiptNo = Convert.ToInt32((from Payment in db.NC_TBL_PAYMENT_TRANS select Payment.RECEIPT_NO).Max()) + 1;
        //            //}
        //            //var ReceiptNoAccompany = getUniqueno("RCPT");



        //            //var AccompanyPRPaymentDetails = new NC_TBL_PAYMENT_TRANS();

        //            //AccompanyPRPaymentDetails.PR_NO = PackageDetails.AccompanyingGuestPRNO;
        //            //AccompanyPRPaymentDetails.RECEIPT_NO = ReceiptNoAccompany;
        //            //AccompanyPRPaymentDetails.TXN_DATE = DateTime.Now;
        //            //AccompanyPRPaymentDetails.ACTUAL_PAID_DATE = DateTime.Now;
        //            //AccompanyPRPaymentDetails.COUNTER_NO = 0;
        //            //AccompanyPRPaymentDetails.PAID_AMOUNT = nc_tbl_payment_trans.PAID_AMOUNT / 2;
        //            //AccompanyPRPaymentDetails.PAYMENT_TYPE_CODE = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
        //            //AccompanyPRPaymentDetails.PAYMENT_MODE_CODE = 4;
        //            //AccompanyPRPaymentDetails.FINAL_STATUS = "N";
        //            //AccompanyPRPaymentDetails.RECEIPT_CANCELLED = "N";
        //            //db.NC_TBL_PAYMENT_TRANS.Add(AccompanyPRPaymentDetails);
        //            //db.SaveChanges();
        //            //}


        //            //To update Accomany Guest Paidstatus in billing

        //            ////To update Paid Status in NC_TBL_Billing_Trans table
        //            //var Accompanybilltransac = db.NC_TBL_Billing_Trans.Where(i => i.PR_NO == PackageDetails.AccompanyingGuestPRNO && i.Bill_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE && i.Paid_status == "N").FirstOrDefault();
        //            //NC_TBL_Billing_Trans Accompanyexistingbilltrans = db.NC_TBL_Billing_Trans.Find(Accompanybilltransac.Id);
        //            //if (Accompanyexistingbilltrans != null)
        //            //{
        //            //    Accompanyexistingbilltrans.Paid_status = "Y";

        //            //    db.SaveChanges();
        //            //}



        //            //if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 1)
        //            //{
        //            //    //var TPRNO = Session["PRNO"].ToString();
        //            //    var BookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == nc_tbl_payment_trans.PR_NO.ToString()).FirstOrDefault();
        //            //    NC_TBL_BOOKED_DATES existingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(BookedDetails.Id);
        //            //    if (existingDatesBooking != null)
        //            //    {
        //            //        existingDatesBooking.ApplicationStatus = "Completed";
        //            //    }
        //            //    db.SaveChanges();


        //            //    ///To update Accompany applicationstatus
        //            //    ///
        //            //    var AccompanyBookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PackageDetails.AccompanyingGuestPRNO).FirstOrDefault();
        //            //    NC_TBL_BOOKED_DATES AccompanyexistingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(AccompanyBookedDetails.Id);
        //            //    if (AccompanyexistingDatesBooking != null)
        //            //    {
        //            //        var AccompanyHBDetails = db.NC_TBL_HABIT_DETAILS.Where(i => i.PRNO == PackageDetails.AccompanyingGuestPRNO).ToList();
        //            //        if (AccompanyHBDetails.Count() > 0)
        //            //        {
        //            //            AccompanyexistingDatesBooking.ApplicationStatus = "Completed";
        //            //        }
        //            //    }
        //            //    db.SaveChanges();

        //            //}


        //            if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 1)
        //            {
        //                PPR = "Payment success";
        //                //PPR = "Your Application is Successfully submitted ...Please note PRNO " + Session["TMPPR"].ToString() + " for your future communication with PEMA";
        //            }
        //            else
        //            {
        //                if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 3)
        //                {
        //                    PPR = "Your Advance payment record successfully Added";
        //                }
        //                else
        //                {
        //                    PPR = Session["TMPPR"].ToString();
        //                }

        //            }
        //            Session["TMPPR"] = null;

        //            //To update Paid Status in NC_TBL_Billing_Trans table
        //            //var billtransac = db.NC_TBL_Billing_Trans.Where(i => i.PR_NO == nc_tbl_payment_trans.PR_NO && i.Bill_TYPE_CODE == nc_tbl_payment_trans.PAYMENT_TYPE_CODE && i.Paid_status == "N").FirstOrDefault();
        //            //NC_TBL_Billing_Trans existingbilltrans = db.NC_TBL_Billing_Trans.Find(billtransac.Id);
        //            //if (existingbilltrans != null)
        //            //{
        //            //    existingbilltrans.Paid_status = "Y";

        //            //    db.SaveChanges();
        //            var res = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
        //            var prnum = db.NC_TBL_PERSONAL_DETAILS.Where(x => x.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
        //            //if (res == 1)
        //            //{
        //            //    var email = prnum.EmailId;
        //            //    var name = prnum.FirstName + " " + prnum.LastName;
        //            //    var PRNO = nc_tbl_payment_trans.PR_NO;
        //            //    var UHID = prnum.UHID;
        //            //    MailMessage mail = new MailMessage();
        //            //    mail.To.Add(email);
        //            //    mail.From = new MailAddress("nhcl.pema@gmail.com");
        //            //    mail.Subject = "PEMA Reservation Details" + "[PRNO: " + PRNO + "]";
        //            //    // string Body = "<table><tr><td >Dear " + name + "</td> </tr><tr><td>Your reservation with PEMA is Confirmed </td></tr><tr><td>Our Chief doctor will verify the details and get back to you<br> with the treatment and package details to get better<br> results</td></tr><tr><td >Please Use The Following PRNO For<br> your Reference when you visit the<br> PEMA</td></tr><tr><td >UHID:" + UHID + "</td></tr><tr><td >PRNO:" + PRNO + "</td></tr></table>";

        //            //    mbody = new StringBuilder();
        //            //    mbody.Append(mheader);

        //            //    mbody.Append("<table> ");
        //            //    mbody.Append("<tr><td><h2 style='float:left; margin-left:100px;'>Reservation  Details</h2></td></tr> ");
        //            //    mbody.Append("<tr> <td>Dear " + name + ",</td> </tr> ");
        //            //    mbody.Append("<tr> <td>&nbsp;&nbsp;&nbsp;</td> </tr>  ");

        //            //    mbody.Append("<tr> <td> ");
        //            //    mbody.Append("Greetings from PEMA WELLNESS, Vizag.<br /><br /> ");
        //            //    mbody.Append("We thank you for choosing PEMA WELLNESS services.<br /><br />");
        //            //    mbody.Append("We confirm the receipt of your Application.<br /><br />");
        //            //    mbody.Append("Your application has been forwarded to our chief Medical Officer for review and suggestions.<br /><br />");
        //            //    mbody.Append("The suggested Treatments and Package details will be mailed to you once our Chief Medical Officer reviews your application.<br /><br />");
        //            //    mbody.Append("The Advance Payment need to be done against the suggested package to confirm your booking.<br /><br />");
        //            //    mbody.Append("Please use the following ID details for further communication with PEMA WELLNESS.<br /><br />");
        //            //    mbody.Append("UHID [Life Time ID] :&nbsp;<b style='color:blue'>" + UHID + "</b> <br /><br /> ");
        //            //    mbody.Append("PRNO [Visit ID] :&nbsp;<b style='color:blue'>" + PRNO + "</b><br /><br />");

        //            //    mbody.Append("</td>  </tr>  ");

        //            //    mbody.Append("</table>");

        //            //    mbody.Append(mfooter);

        //            //    mail.Body = mbody.ToString();
        //            //    mail.IsBodyHtml = true;

        //            //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(customCertValidation);



        //            //    SmtpClient smtp = new SmtpClient();
        //            //    smtp.Host = "smtp.gmail.com";
        //            //    smtp.Port = 587;
        //            //    smtp.UseDefaultCredentials = false;

        //            //    smtp.Credentials = new System.Net.NetworkCredential
        //            //    ("nhcl.pema@gmail.com", "Pema@123");// Enter seders User name and password
        //            //    smtp.EnableSsl = true;
        //            //    smtp.Send(mail);
        //            //}
        //            //else if (res == 3)
        //            //{
        //            //var email = prnum.EmailId;
        //            //var Booked = db.NC_TBL_BOOKED_DATES.Where(x => x.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
        //            //var PackageName = Booked.PackageName;
        //            //var Paymenttype = db.NC_TBL_PAYMENT_TRANS.Where(x => x.PAYMENT_TYPE_CODE == 3 && x.PR_NO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
        //            //var amount = Paymenttype.PAID_AMOUNT * 2;
        //            //var TransactionDate = Paymenttype.TXN_DATE;
        //            //var chekindate = Booked.ConfirmedFromDate;
        //            //var name = prnum.FirstName + " " + prnum.LastName;
        //            //var PRNO = nc_tbl_payment_trans.PR_NO;
        //            //MailMessage mail = new MailMessage();
        //            //mail.To.Add(email);
        //            //mail.From = new MailAddress("nhcl.pema@gmail.com");
        //            //mail.Subject = "PEMA Booking Confirmation" + "[PRNO: " + PRNO + "]";
        //            //// string Body = "<table style='border-color:#707070'><tr><td style='color:#0000FF'>Dear " +name+ "</td> </tr><tr><td>Thank You for choosing PEMA Resorts, its our pleasure to serve you</td></tr><tr><td style='border:groove;border-width:thick;border-color:beige;'>The following are your transaction details</td></tr><tr><td style='color:#0000FF';>Package Name: " +PackageName+ "</td></tr><tr><td style='color:#0000FF'>PR NO: " +PRNO+ "</td></tr><tr><td style='color:#0000FF'>Amount: " +amount+ "</td></tr><tr><td style='color:#0000FF'>Transaction Date: " +TransactionDate+ "</td></tr><tr><td>Note: Please make sure to avaialable for check-in on: " +chekindate+ "</td></tr></table>";

        //            //mbody = new StringBuilder();
        //            //mbody.Append(mheader);

        //            //mbody.Append("<table> ");
        //            //mbody.Append("<tr><td><h2 style='float:left; margin-left:100px;'>Booking Confirmation</h2></td></tr> ");
        //            //mbody.Append("<tr> <td>Dear " + name + ",</td> </tr> ");
        //            //mbody.Append("<tr> <td>&nbsp;&nbsp;&nbsp;</td> </tr>  ");

        //            //mbody.Append("<tr> <td> ");

        //            //mbody.Append("Greetings from PEMA WELLNESS, Vizag. <br /><br />");
        //            //mbody.Append("We thank you for  making the advanced payment. We acknowledge the same. <br /><br />");
        //            //mbody.Append("Your Treatment and Package booking is now confirmed <br /><br />");
        //            //mbody.Append("<table>");
        //            //mbody.Append("<tr><td>Package Name     </td><td>:</td><td> " + PackageName + " </td></tr>");
        //            //mbody.Append("<tr><td>PR NO     </td><td>:</td><td> " + PRNO + " </td></tr>");
        //            //mbody.Append("<tr><td>Amount     </td><td>:</td><td> " + amount + " </td></tr>");
        //            //mbody.Append("<tr><td>Transaction Date     </td><td>:</td><td> " + TransactionDate + " </td></tr>");
        //            //mbody.Append("<tr><td>Check-In Date     </td><td>:</td><td>" + chekindate + "</td></tr>");
        //            //mbody.Append("</table><br />  ");

        //            //mbody.Append("We look forward to see you at our wellness centre on " + chekindate + ". Our Check-in time is 10:30 AM. <br /><br />");
        //            //mbody.Append("Please share your itinerary once it is finalized with us so that we can serve you better. <br /><br />");

        //            //mbody.Append("Please use the PRNO specified above for further communication with PEMA WELLNESS. <br /><br />");

        //            //mbody.Append("</td>  </tr>  ");

        //            //mbody.Append("</table>");

        //            //mbody.Append(mfooter);

        //            ////string ram = "Pr no" + Session["TMPPR"];
        //            //mail.Body = mbody.ToString();
        //            //mail.IsBodyHtml = true;
        //            ////System.Net.Mail.Attachment attachment;
        //            ////attachment = new System.Net.Mail.Attachment("your attachment file");
        //            ////mail.Attachments.Add(attachment);
        //            //SmtpClient smtp = new SmtpClient();
        //            //smtp.Host = "smtp.gmail.com";
        //            //smtp.Port = 587;
        //            //smtp.UseDefaultCredentials = false;
        //            //smtp.Credentials = new System.Net.NetworkCredential
        //            //("nhcl.pema@gmail.com", "Pema@123");// Enter seders User name and password
        //            //smtp.EnableSsl = true;
        //            //smtp.Send(mail);


        //            //Starting of Guest room transaction table Room Blocked
        //            //var vres = getdetails(nc_tbl_payment_trans.PR_NO);
        //            //var prtype = vres.Split(',')[0];
        //            //var oppPRNO = vres.Split(',')[1];
        //            //var oppname = vres.Split(',')[2];


        //            //var AvailableRoomNo = "";
        //            //var Packagedetails = db.NC_TBL_BOOKED_DATES.Where(x => x.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();

        //            //var result = from y in db.NC_TBL_ROOM_MASTER
        //            //             where !(from x in db.NC_TBL_GUEST_ROOM_TRANSACTION
        //            //                     select x.Room_No
        //            //                     ).Contains(y.Room_No) && y.Active == "Y" && y.Room_View == Packagedetails.RoomView && y.Room_Type == Packagedetails.RoomType
        //            //             select y;

        //            //if (result.Count() == 0)
        //            //{

        //            //    var per = db.NC_TBL_ROOM_MASTER.Where(a => a.Room_View == Packagedetails.RoomView && a.Room_Type == Packagedetails.RoomType && a.Active == "Y").ToList();
        //            //    foreach (NC_TBL_ROOM_MASTER cust in per)
        //            //    {
        //            //        if (AvailableRoomNo == "")
        //            //        {
        //            //            var exist = db.NC_TBL_GUEST_ROOM_TRANSACTION.Where(a => a.Room_No == cust.Room_No).ToList();
        //            //            if (exist != null)
        //            //            {



        //            //                var query1 = (from b in db.NC_TBL_GUEST_ROOM_TRANSACTION
        //            //                              where b.Room_No == cust.Room_No
        //            //                              && ((b.ArrivalDate >= Packagedetails.ConfirmedFromDate && b.ArrivalDate < Packagedetails.ConfirmedToDate) || (b.DepartureDate > Packagedetails.ConfirmedFromDate && Packagedetails.ConfirmedToDate <= Packagedetails.ConfirmedToDate)|| (b.ArrivalDate < Packagedetails.ConfirmedFromDate && b.DepartureDate > Packagedetails.ConfirmedToDate))
        //            //                              select new
        //            //                              {
        //            //                                  RoomNo = b.Room_No,

        //            //                              }).ToList();

        //            //                if (query1.Count != 0)
        //            //                {


        //            //                }
        //            //                else
        //            //                {
        //            //                    AvailableRoomNo = cust.Room_No;

        //            //                }

        //            //            }
        //            //            else
        //            //            {
        //            //                AvailableRoomNo = cust.Room_No;

        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    AvailableRoomNo = result.FirstOrDefault().Room_No;
        //            //}


        //            //var RoomTrans = db.NC_TBL_GUEST_ROOM_TRANSACTION.FirstOrDefault(i => i.PRNO == nc_tbl_payment_trans.PR_NO);
        //            //if (RoomTrans == null)
        //            //{
        //            //    var RoomTransaction = new NC_TBL_GUEST_ROOM_TRANSACTION();

        //            //    RoomTransaction.Room_No = AvailableRoomNo;
        //            //    RoomTransaction.ArrivalDate = Convert.ToDateTime(Packagedetails.ConfirmedFromDate);
        //            //    RoomTransaction.DepartureDate = Convert.ToDateTime(Packagedetails.ConfirmedToDate);

        //            //    RoomTransaction.PRNO = nc_tbl_payment_trans.PR_NO;
        //            //    RoomTransaction.Status = "Blocked";
        //            //    RoomTransaction.CheckedOut = "N";
        //            //    db.NC_TBL_GUEST_ROOM_TRANSACTION.Add(RoomTransaction);
        //            //    db.SaveChanges();

        //            //}

        //            //var RoomTransAccompany = db.NC_TBL_GUEST_ROOM_TRANSACTION.FirstOrDefault(i => i.PRNO == oppPRNO);
        //            //var packageinfo = db.NC_TBL_BOOKED_DATES.FirstOrDefault(i => i.PRNO == oppPRNO);
        //            //if (RoomTransAccompany == null)
        //            //{
        //            //    var RoomTransactionAccompany = new NC_TBL_GUEST_ROOM_TRANSACTION();

        //            //    RoomTransactionAccompany.Room_No = AvailableRoomNo;
        //            //    RoomTransactionAccompany.ArrivalDate = Convert.ToDateTime(packageinfo.ConfirmedFromDate);
        //            //    RoomTransactionAccompany.DepartureDate = Convert.ToDateTime(packageinfo.ConfirmedToDate);

        //            //    RoomTransactionAccompany.PRNO = oppPRNO;
        //            //    RoomTransactionAccompany.Status = "Blocked";
        //            //    RoomTransactionAccompany.CheckedOut = "N";
        //            //    db.NC_TBL_GUEST_ROOM_TRANSACTION.Add(RoomTransactionAccompany);
        //            //    db.SaveChanges();

        //            //}



        //            ////End of Guest Room transaction Table Room Blocked








        //            //    }


        //            //}

        //        }
        //    }
        //    //**************************************************************
        //    ///End of Combined Payment
        //    //**************************************************************
        //    return Json(PPR, JsonRequestBehavior.AllowGet);


        //}

        public ActionResult SavePaymentDetails1(NC_TBL_PAYMENT_TRANS nc_tbl_payment_trans)
        {

            //if (nc_tbl_payment_trans.PAID_AMOUNT != null && nc_tbl_payment_trans.PAID_AMOUNT != 0 && nc_tbl_payment_trans.PAID_AMOUNT < 0 && nc_tbl_payment_trans.PAID_AMOUNT <= 2147483647)
            //{
            string PPR = "";

            var Paymode = nc_tbl_payment_trans.PAYMENT_MODE_CODE;

            //pnlRequest.Visible = false;
            try
            {
                var PaymentTransNo = getUniqueno("PTNO");


                if (Paymode == 3)
                {
                    TempData["Paymentmode"] = "NetBanking";
                    DateTime date = DateTime.Now;
                    var shortDate = date.ToString("dd/MM/yyyy");
                    var Amount = nc_tbl_payment_trans.PAID_AMOUNT.ToString();
                    //if (nc_tbl_payment_trans.PAID_AMOUNT <= 100000)
                    //{


                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://payment.atomtech.in/paynetz/epi/fts");

                    request.Method = "POST";
                    //------
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; CK={CVxk71YSfgiE6+6P6ftT7lWzblrdvMbRqavYf/6OcMIH8wfE6iK7TNkcwFAsxeChX7qRAlQhvPWso3KI6Jthvnvls9scl+OnAEhsgv+tuvs=}; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

                    //string postData = "login=160&pass=Test@123&ttype=NBFundTransfer&prodid=NSE&amt=" + Amount
                    //+ "&txncurr=INR&txnscamt=0&clientcode=007=&txnid=" + PaymentTransNo + "&date=25/06/2012%2012:23:23&custacc=123456789012";

                    //string postData = "login=160&pass=Test@123&ttype=NBFundTransfer&prodid=NSE&amt=" + Amount
                    //+ "&txncurr=INR&txnscamt=0&clientcode=007=&txnid=" + PaymentTransNo + "&date=25/06/2012%2012:23:23&custacc=123456789012&udf1=Thirumala&udf2=thiru237@gmail.com&udf3=9987675645&udf4=fgfdgdfg&ru=http://'"+Atom_ReturnURL+"'/GuestDashboard/PaymentResult";



                    string postData = "login=23741&pass=BAYPARK@123&ttype=NBFundTransfer&prodid=BAYPARK&amt=" + Amount + "&txncurr=INR&txnscamt=0&clientcode=007=&txnid=" + PaymentTransNo + "&date=" + shortDate + "&custacc=1234567890&ru=http://pemawellness.co/GuestDashboard/PaymentResult";

                    //string postData = "login=23741&pass=BAYPARK@123&ttype=NBFundTransfer&prodid=BAYPARK&amt=" + Amount + "&txncurr=INR&txnscamt=0&clientcode=007=&txnid=" + PaymentTransNo + "&date=" + shortDate + "&custacc=1234567890&ru=http://grandkoi.pemawellness.co/GuestDashboard/PaymentResult";


                    //https://payment.atomtech.in/paynetz/epi/fts?login=23741&pass=BAYPARK@123&ttype=NBFundTransfer&prodid=BAYPARK&amt=50&txncurr=INR&txnscamt=0&clientcode=007&txnid=123&custacc=1234567890&date=31/07/2013&ru=%3Cthe


                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                    request.ContentType = "application/x-www-form-urlencoded";

                    request.ContentLength = byteArray.Length;
                    request.AllowAutoRedirect = true;

                    request.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    Stream dataStream = request.GetRequestStream();

                    dataStream.Write(byteArray, 0, byteArray.Length);

                    dataStream.Close();

                    WebResponse response = request.GetResponse();

                    XmlDocument objXML = new XmlDocument();

                    dataStream = response.GetResponseStream();

                    objXML.Load(dataStream);

                    string TxnId = objXML.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[2].InnerText;

                    string Token = objXML.DocumentElement.ChildNodes[0].ChildNodes[0].ChildNodes[3].InnerText;
                    string txnData = "ttype=NBFundTransfer&txnStage=1&tempTxnId=" + TxnId + "&token=" + Token;

                    dataStream.Close();
                    response.Close();

                    ///To save payment dfata in table
                    NC_TBL_Gateway_Payment_Details save = new NC_TBL_Gateway_Payment_Details();


                    if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 14)
                    {

                        var uid = (from i in db.NC_TBL_MEMBERSHIPDETAILS_HEADER
                                   where i.Mem_Id == nc_tbl_payment_trans.PR_NO
                                   select new { UHID = i.UHID }
                                      ).FirstOrDefault();


                        var res = (from a in db.WS_Tbl_Registration
                                   where a.UHID == uid.UHID
                                   select new
                                   {
                                       name = a.first_name + " " + (a.middle_name == null ? "" : a.middle_name) + " " + a.last_name,
                                       mail = a.user_id,
                                       UHID = a.UHID,
                                   }
                            ).FirstOrDefault();

                        save.UHID = res.UHID;
                        save.Name = res.name;
                        save.PRNO = nc_tbl_payment_trans.PR_NO;
                    }
                    else
                    {
                        var per = db.NC_TBL_PERSONAL_DETAILS.Where(a => a.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
                        save.UHID = per.UHID;
                        save.PRNO = nc_tbl_payment_trans.PR_NO;
                        save.Name = per.FirstName + " " + (per.MiddleName == null ? "" : per.MiddleName) + " " + per.LastName;
                    }









                    save.PAYMENT_TYPE_CODE = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
                    save.PAYMENT_MODE_CODE = nc_tbl_payment_trans.PAYMENT_MODE_CODE;
                    save.Paid_Amount = nc_tbl_payment_trans.PAID_AMOUNT;
                    save.Txn_Date = DateTime.Now;
                    save.TransactionNo = PaymentTransNo;
                    save.PaidStatus = "N";
                    save.AMAFlag = "N";
                    save.EntryType = nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 14 ? "M" : "P";
                    save.GatewayRespStatus = "P";
                    db.NC_TBL_Gateway_Payment_Details.Add(save);
                    db.SaveChanges();



                    return Redirect("https://payment.atomtech.in/paynetz/epi/fts?" + txnData);



                }
                else
                {

                    TempData["Paymentmode"] = "Credit/DebitCards";
                    // Connect to the Payment Client
                    VPCRequest conn = new VPCRequest();
                    // Add the Digital Order Fields for the functionality you wish to use
                    // Core Transaction Fields

                    //string.Format("{0:0.00}"
                    //var ss = String.Format("{0:C}", nc_tbl_payment_trans.PAID_AMOUNT.ToString());
                    //var amount = String.Format("{0:N}", nc_tbl_payment_trans.PAID_AMOUNT);
                    var amount = nc_tbl_payment_trans.PAID_AMOUNT.ToString() + "00";
                    //var amount = nc_tbl_payment_trans.PAID_AMOUNT.ToString();
                    //nc_tbl_payment_trans.PAID_AMOUNT.ToString("#.##");
                    conn.AddDigitalOrderField("vpc_Version", conn.Version);
                    conn.AddDigitalOrderField("vpc_Command", conn.Command);
                    conn.AddDigitalOrderField("vpc_AccessCode", conn.AccessCode);
                    conn.AddDigitalOrderField("vpc_Merchant", conn.MerchantID);
                    conn.AddDigitalOrderField("vpc_ReturnURL", conn.FormatReturnURL(Request.Url.Scheme, Request.Url.Host, Request.Url.Port, Request.ApplicationPath));
                    conn.AddDigitalOrderField("vpc_MerchTxnRef", PaymentTransNo);
                    conn.AddDigitalOrderField("vpc_OrderInfo", nc_tbl_payment_trans.PR_NO);
                    conn.AddDigitalOrderField("vpc_Amount", amount);
                    conn.AddDigitalOrderField("vpc_Currency", "INR");
                    conn.AddDigitalOrderField("vpc_Locale", "INR");
                    // Perform the transaction
                    String url = conn.Create3PartyQueryString();

                    ///To save payment dfata in table
                    ///
                    NC_TBL_Gateway_Payment_Details save = new NC_TBL_Gateway_Payment_Details();
                    if (nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 14)
                    {

                        var uid = (from i in db.NC_TBL_MEMBERSHIPDETAILS_HEADER
                                   where i.Mem_Id == nc_tbl_payment_trans.PR_NO
                                   select new { UHID = i.UHID }
                                      ).FirstOrDefault();


                        var res = (from a in db.WS_Tbl_Registration
                                   where a.UHID == uid.UHID
                                   select new
                                   {
                                       name = a.first_name + " " + (a.middle_name == null ? "" : a.middle_name) + " " + a.last_name,
                                       mail = a.user_id,
                                       UHID = a.UHID,
                                   }
                            ).FirstOrDefault();

                        save.UHID = res.UHID;
                        save.Name = res.name;
                        save.PRNO = nc_tbl_payment_trans.PR_NO;
                    }
                    else
                    {
                        var per = db.NC_TBL_PERSONAL_DETAILS.Where(a => a.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();
                        save.UHID = per.UHID;
                        save.PRNO = nc_tbl_payment_trans.PR_NO;
                        save.Name = per.FirstName + " " + (per.MiddleName == null ? "" : per.MiddleName) + " " + per.LastName;
                    }


                    //var per = db.NC_TBL_PERSONAL_DETAILS.Where(a => a.PRNO == nc_tbl_payment_trans.PR_NO).FirstOrDefault();



                    save.PAYMENT_TYPE_CODE = nc_tbl_payment_trans.PAYMENT_TYPE_CODE;
                    save.PAYMENT_MODE_CODE = nc_tbl_payment_trans.PAYMENT_MODE_CODE;
                    save.Paid_Amount = nc_tbl_payment_trans.PAID_AMOUNT;
                    save.Txn_Date = DateTime.Now;
                    save.TransactionNo = PaymentTransNo;
                    save.PaidStatus = "N";
                    save.AMAFlag = "N";
                    save.EntryType = nc_tbl_payment_trans.PAYMENT_TYPE_CODE == 14 ? "M" : "P";
                    save.GatewayRespStatus = "P";
                    db.NC_TBL_Gateway_Payment_Details.Add(save);
                    db.SaveChanges();
                    return Redirect(url);

                }

            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    foreach (var validationErrors in dbEx.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

            //        }

            //    }
            //    return PartialView("Error");
            //}
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var innerexcep = ex.InnerException.ToString();
                    ErrorLog(ex.Message, innerexcep);

                }
                else
                {
                    ErrorLog(ex.Message, "");
                }

                return PartialView("Error");


            }
            //}
            //else
            //{
            //    TempData["Msg"] = "Invalid Payment Amount";
            //   // return View(nc_tbl_payment_trans);
            //    //return Redirect(url);
            //    var message="";
            //    return Json(message="Invalid Amount", JsonRequestBehavior.AllowGet);
            //}
        }

        public ActionResult SaveHLDetails(int[] HLId, int[] HLCode, string[] HLFlag, string[] HLDesc)
        {
            //var PR = Session["TMPPR"].ToString();
            string message = "";


            if (HLId.Count() > 0)
            {


                for (int i = 0; i < HLId.Length; i++)
                {

                    NC_TBL_HEALTH_DETAILS existing = db.NC_TBL_HEALTH_DETAILS.Find(HLId[i]);

                    if (HLFlag[i] != "")
                    {
                        existing.HLDesc = HLDesc[i];
                        existing.HLFlag = HLFlag[i];
                    }
                    else
                    {
                        existing.HLDesc = HLDesc[i];
                    }

                }

                db.SaveChanges();

                message = "Successfully Saved!";

            }
            else
            {
                message = "Records Updation Failed!";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveHBDetails(int[] HBId, string[] HBFlag, string[] HBDesc)
        {

            string message = "";


            if (HBId.Count() > 0)
            {


                for (int i = 0; i < HBId.Length; i++)
                {

                    NC_TBL_HABIT_DETAILS existing = db.NC_TBL_HABIT_DETAILS.Find(HBId[i]);

                    if (HBFlag[i] != "")
                    {
                        existing.HBDesc = HBDesc[i];
                        existing.HBQFreq = HBFlag[i];
                    }
                    else
                    {
                        existing.HBDesc = HBDesc[i];
                    }

                }

                db.SaveChanges();

                message = "Successfully Saved!";

            }
            else
            {
                message = "Records Updation Failed!";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayPRPackageDetails()
        {
            var PR = Session["TMPPR"].ToString();

            //var queryResult = from e in db.NC_TBL_BOOKED_DATES
            //                  where e.PRNO == PR
            //                  select e;

            //var Record = queryResult.FirstOrDefault();
            var Record = (from Booked in db.NC_TBL_BOOKED_DATES
                          where Booked.PRNO == PR
                          select new
                          {
                              PackageType = Booked.PackageType,
                              PackageName = Booked.PackageName,
                              NoOfDays = Booked.NoOfDays,
                              ArrivalDate = SqlFunctions.DateName("day", Booked.ArrivalDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.ArrivalDate.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.ArrivalDate),
                              DepartureDate = SqlFunctions.DateName("day", Booked.DepartureDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.DepartureDate.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.DepartureDate),
                              AlternateArrivalDate = (Booked.AlternateArrivalDate.HasValue) ? SqlFunctions.DateName("day", Booked.AlternateArrivalDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.AlternateArrivalDate.Value.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.AlternateArrivalDate) : string.Empty,
                              AlternateDepartureDate = (Booked.AlternateDepartureDate.HasValue) ? SqlFunctions.DateName("day", Booked.AlternateDepartureDate).Trim() + "/" + SqlFunctions.StringConvert((double)Booked.AlternateDepartureDate.Value.Month).TrimStart() + "/" + SqlFunctions.DateName("year", Booked.AlternateDepartureDate) : string.Empty,
                              AccompanyingGuests = Booked.AccompanyingGuests,
                              RoomView = Booked.RoomView,
                              RoomType = Booked.RoomType,
                              RoomTariff = Booked.RoomTariff,
                              Package_Amount = Booked.Package_Amount,
                              Group_Code = Booked.Group_Code,
                              Accompany_Type = Booked.Accompany_Type,
                              Guest_Attender_Name = Booked.Guest_Attender_Name,
                              AccompanyingGuestUHID = Booked.AccompanyingGuestUHID,
                              AccompanyingGuestPRNO = Booked.AccompanyingGuestPRNO,
                              Guest_Attender_ARNO = Booked.Guest_Attender_ARNO,
                              UHID = Booked.UHID,
                              UserType = Booked.UserType,
                              TransportationRequired = Booked.Transportation_Required


                          }).FirstOrDefault();


            if (Record == null)
            {
                return HttpNotFound();

            }

            return Json(Record, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckUHID(string AGUHID)
        {


            var queryResult = from e in db.WS_Tbl_Registration
                              where e.UHID == AGUHID
                              select e;

            var Record = queryResult.FirstOrDefault();


            return Json(Record, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// this is for checking Campaign Code is Valid or Not
        /// </summary>
        /// <param name="AGUHID"></param>
        /// <returns></returns>
        public ActionResult CheckCC(string CC)
        {
            var queryResult = from e in db.NC_TBL_CAMPAIGN_MASTER
                              where e.Camp_Code == CC && e.Active == "Y" && e.MemberShipReq == "N"
                              select e;
            var Record = queryResult.FirstOrDefault();
            return Json(Record, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// this is for checking for Membership Id and Validate the Visits and Membership Dates
        /// </summary>
        /// <param name="CC"></param>
        /// <returns></returns>
        public ActionResult CheckMemId(string MC, string walkinuserUHID, string arrivaldate, string departuredate)
        {

            if (walkinuserUHID == null || walkinuserUHID == "")
            {
                walkinuserUHID = Session["TMPUH"].ToString();
            }

            var queryResult = from e in db.NC_TBL_MEMBERSHIPDETAILS_HEADER
                              where e.Mem_Id == MC && e.UHID == walkinuserUHID && e.Mem_Payment_Status == "Y"
                              select e;
            var Record = queryResult.FirstOrDefault();
            if (Record == null)
            {

                var msg = "Invalid Membership Id";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int Rec = db.NC_TBL_MEMBERSHIPDETAILS_MASTER.Where(e => e.Mem_Code == Record.Mem_Code).Select(b => b.Mem_Visits).FirstOrDefault();
                var Record1 = db.NC_TBL_MEMBERSHIPDETAILS_HEADER.Where(e => e.Mem_Id == MC && e.UHID == walkinuserUHID && e.Mem_Payment_Status == "Y").FirstOrDefault();
                var query = from e in db.NC_TBL_MEMBERSHIPDETAILS_DETAILS
                            where e.Doc_Num == Record1.Doc_No
                            select e;

                if (query.Count() < Rec)
                {


                    DateTime AD = Convert.ToDateTime(arrivaldate);
                    DateTime DD = Convert.ToDateTime(departuredate);

                    DateTime SD = Convert.ToDateTime(Record1.Mem_StartDate);
                    DateTime ED = Convert.ToDateTime(Record1.Mem_EndDate);


                    if (SD <= AD && DD <= ED)
                    {
                        var msg = "";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var msg = "Membership not covered in this Period";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    var msg = "Maximun Number of Visits completed";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(Record, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// this is for cheking arrival and departure date when membership id not null
        /// </summary>
        /// <param name="MC"></param>
        /// <param name="walkinuserUHID"></param>
        /// <param name="arrivaldate"></param>
        /// <param name="departuredate"></param>
        /// <returns></returns>
        public ActionResult DatesCheckMemId(string MC, string walkinuserUHID, string arrivaldate, string departuredate)
        {

            if (walkinuserUHID == null || walkinuserUHID == "")
            {
                walkinuserUHID = Session["TMPUH"].ToString();
            }

            var queryResult = from e in db.NC_TBL_MEMBERSHIPDETAILS_HEADER
                              where e.Mem_Id == MC && e.UHID == walkinuserUHID && e.Mem_Payment_Status == "Y"
                              select e;
            var Record = queryResult.FirstOrDefault();
            if (Record == null)
            {

                var msg = "Invalid Membership Id";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int Rec = db.NC_TBL_MEMBERSHIPDETAILS_MASTER.Where(e => e.Mem_Code == Record.Mem_Code).Select(b => b.Mem_Visits).FirstOrDefault();
                var Record1 = db.NC_TBL_MEMBERSHIPDETAILS_HEADER.Where(e => e.Mem_Id == MC && e.UHID == walkinuserUHID && e.Mem_Payment_Status == "Y").FirstOrDefault();
                var query = from e in db.NC_TBL_MEMBERSHIPDETAILS_DETAILS
                            where e.Doc_Num == Record1.Doc_No
                            select e;

                if (query.Count() < Rec)
                {


                    DateTime AD = Convert.ToDateTime(arrivaldate);
                    DateTime DD = Convert.ToDateTime(departuredate);

                    DateTime SD = Convert.ToDateTime(Record1.Mem_StartDate);
                    DateTime ED = Convert.ToDateTime(Record1.Mem_EndDate);


                    if (SD <= AD && DD <= ED)
                    {
                        var msg = "";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var msg = "Membership not covered in this Period";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    var msg = "Maximun Number of Visits completed";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(Record, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckGroupCode(string GroupCode)
        {


            var queryResult = from e in db.NC_TBL_Guest_Group_Master
                              where e.Group_Code == GroupCode
                              select e;

            var Record = queryResult.FirstOrDefault();


            return Json(Record, JsonRequestBehavior.AllowGet);

        }
        public string getdetails(string prno)
        {

            object[] parameters = new object[] { "prtype", typeof(String), "oppPRNO", typeof(String), "oppName", typeof(String) };
            ObjectParameter prtype = new ObjectParameter("prtype", typeof(String));
            ObjectParameter oppPRNO = new ObjectParameter("oppPRNO", typeof(String));
            ObjectParameter oppName = new ObjectParameter("oppName", typeof(String));

            var getdetails = db.Guest_PR_Identification(prno, prtype, oppPRNO, oppName);
            return (prtype.Value + "," + oppPRNO.Value + "," + oppName.Value);

        }
        public string getUniqueno(string ReqUniqueType)
        {
            ObjectParameter Unique_PRNo = new ObjectParameter("uniqueno", typeof(String));
            var UniqueNo = db.gen_unique_no(ReqUniqueType, Unique_PRNo);
            return (Unique_PRNo.Value.ToString());

        }
        public ActionResult ResrvationStep5(string PRNO = "", int billTypeCode = 3)
        {



            ViewBag.billcode = billTypeCode;

            //var Booking = (from e in db.NC_TBL_BOOKED_DATES
            //               where e.PRNO == PRNO
            //               select e).FirstOrDefault();
            //ViewBag.noofdays = Booking.NoOfDays;

            if (Session["TMPPR"] != null && Session["MEMID"] == null)
            {
                if (Session["TMPPR"] == null)
                {
                    Session["TMPPR"] = Session["PRNO"];
                }
                var PRNum = Session["TMPPR"].ToString();
                ViewBag.PRNumber = PRNum;

                var checkPRAccompany = (from e in db.NC_TBL_BOOKED_DATES
                                        where e.AccompanyingGuestPRNO == PRNum
                                        select e).ToList();
                if (checkPRAccompany.Count() >= 1)
                {
                    ViewBag.MainGuest = "No";
                }
                else
                {

                    var BookingInfo = (from e in db.NC_TBL_BOOKED_DATES
                                       where e.PRNO == PRNum
                                       select e).FirstOrDefault();
                    ViewBag.noofdays = BookingInfo.NoOfDays;
                    ViewBag.fullamount = BookingInfo.Package_Amount;
                    if (BookingInfo.Accompany_Type == "Accompany")
                    {

                        var queryOnlinePayment1 = (from a in db.nc_tbl_bill_type_master
                                                   join b in db.NC_TBL_Billing_Trans
                                                    on a.Bill_TYPE_CODE equals b.Bill_TYPE_CODE
                                                   join c in db.NC_TBL_PERSONAL_DETAILS on b.PR_NO equals c.PRNO

                                                   where b.PR_NO == PRNum && b.Paid_status == "N"
                                                   && (b.Bill_TYPE_CODE == 1 || b.Bill_TYPE_CODE == 3)
                                                   select new
                                                   {
                                                       PRNO = b.PR_NO,
                                                       Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                                       BillCode = b.Bill_TYPE_CODE,
                                                       BillType = a.BILL_TYPE,
                                                       Amount = b.NET_AMOUNT
                                                   }).FirstOrDefault();
                        if (queryOnlinePayment1 != null)
                        {
                            var PaymenttoAccompany = (from c in db.NC_TBL_PAYMENT_TRANS
                                                      where c.PR_NO == BookingInfo.AccompanyingGuestPRNO
                                                         && c.PAYMENT_TYPE_CODE == queryOnlinePayment1.BillCode
                                                      select c).FirstOrDefault();
                            if (PaymenttoAccompany == null)
                            {
                                ViewBag.MainGuest = "yes";
                            }
                            else
                            {
                                ViewBag.MainGuest = "No";
                            }
                        }
                        else
                        {
                            ViewBag.MainGuest = "No";
                        }

                    }
                    else
                    {
                        ViewBag.MainGuest = "No";
                    }

                }

            }
            return View();
        }

        //public ActionResult ResrvationStep5()
        //{
        //     var PR = Session["TMPPR"].ToString();

        //     var BookedDetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == PR).FirstOrDefault();
        //     NC_TBL_BOOKED_DATES existingDatesBooking = db.NC_TBL_BOOKED_DATES.Find(BookedDetails.Id);
        //     if (existingDatesBooking != null)
        //     {
        //         existingDatesBooking.ApplicationStatus = "Completed";
        //     }
        //     db.SaveChanges();
        //     @ViewBag.PRNumber = PR;

        //     var prnum = db.NC_TBL_PERSONAL_DETAILS.Where(x => x.PRNO == PR).FirstOrDefault();
        //     if (prnum !=null)
        //     {
        //         var email = prnum.EmailId;
        //         var name = prnum.FirstName + " " + prnum.LastName;
        //         var PRNO = prnum.PRNO;
        //         var UHID = prnum.UHID;
        //         MailMessage mail = new MailMessage();
        //         mail.To.Add(email);
        //         mail.From = new MailAddress("nhcl.pema@gmail.com");
        //         mail.Subject = "PEMA Reservation Details" + "[PRNO: " + PRNO + "]";
        //         // string Body = "<table><tr><td >Dear " + name + "</td> </tr><tr><td>Your reservation with PEMA is Confirmed </td></tr><tr><td>Our Chief doctor will verify the details and get back to you<br> with the treatment and package details to get better<br> results</td></tr><tr><td >Please Use The Following PRNO For<br> your Reference when you visit the<br> PEMA</td></tr><tr><td >UHID:" + UHID + "</td></tr><tr><td >PRNO:" + PRNO + "</td></tr></table>";

        //         mbody = new StringBuilder();
        //         mbody.Append(mheader);

        //         mbody.Append("<table> ");
        //         mbody.Append("<tr><td><h2 style='float:left; margin-left:100px;'>Reservation  Details</h2></td></tr> ");
        //         mbody.Append("<tr> <td>Dear " + name + ",</td> </tr> ");
        //         mbody.Append("<tr> <td>&nbsp;&nbsp;&nbsp;</td> </tr>  ");

        //         mbody.Append("<tr> <td> ");
        //         mbody.Append("Greetings from PEMA WELLNESS, Vizag.<br /><br /> ");
        //         mbody.Append("We thank you for choosing PEMA WELLNESS services.<br /><br />");
        //         mbody.Append("We confirm the receipt of your Application.<br /><br />");
        //         mbody.Append("Your application has been forwarded to our chief Medical Officer for review and suggestions.<br /><br />");
        //         mbody.Append("The suggested Treatments and Package details will be mailed to you once our Chief Medical Officer reviews your application.<br /><br />");
        //         mbody.Append("The Advance Payment need to be done against the suggested package to confirm your booking.<br /><br />");
        //         mbody.Append("Please use the following ID details for further communication with PEMA WELLNESS.<br /><br />");
        //         mbody.Append("UHID [Life Time ID] :&nbsp;<b style='color:blue'>" + UHID + "</b> <br /><br /> ");
        //         mbody.Append("PRNO [Visit ID] :&nbsp;<b style='color:blue'>" + PRNO + "</b><br /><br />");

        //         mbody.Append("</td>  </tr>  ");

        //         mbody.Append("</table>");

        //         mbody.Append(mfooter);

        //         mail.Body = mbody.ToString();
        //         mail.IsBodyHtml = true;

        //         ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(customCertValidation);






        //         SmtpClient smtp = new SmtpClient();
        //         smtp.Host = "smtp.gmail.com";
        //         smtp.Port = 587;
        //         smtp.UseDefaultCredentials = false;

        //         smtp.Credentials = new System.Net.NetworkCredential
        //         ("nhcl.pema@gmail.com", "Pema@123");// Enter seders User name and password
        //         smtp.EnableSsl = true;
        //         smtp.Send(mail);
        //     }

        //    return View();
        //}
        public ActionResult ResrvationStep6()
        {
            return View();
        }
        public ActionResult ResrvationStep7()
        {
            return View();
        }
        public ActionResult PersonalDetails(string PRNum)
        {
            //var PRDetails = obj.NC_TBL_PERSONAL_DETAILS.Where(a => a.PRNO == PRNum).FirstOrDefault();
            var PRDetails = (from personal in db.NC_TBL_PERSONAL_DETAILS
                             where personal.PRNO == PRNum
                             select new Personal_detailscustomized
                             {
                                 Id = personal.Id,
                                 UHID = personal.UHID,
                                 PRNO = personal.PRNO,
                                 FirstName = personal.FirstName,
                                 MiddleName = personal.MiddleName,
                                 LastName = personal.LastName,
                                 FatherHusbandName = personal.FatherHusbandName,
                                 Gender = personal.Gender == "M" ? "Male" : "Female",
                                 MaritalStatus = personal.MaritalStatus == "S" ? "Single" : "Married",
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
                                 AssignedDoctor = personal.AssignedDoctor,
                             }).FirstOrDefault();
            return PartialView("PRPersonalDetails", PRDetails);
        }
        public ActionResult PackageDetails(string PRNum)
        {
            // var PackageDetails = obj.NC_TBL_BOOKED_DATES.Where(a => a.PRNO == PRNum).FirstOrDefault();
            var PackageDetails = (from packge in db.NC_TBL_BOOKED_DATES
                                  where packge.PRNO == PRNum
                                  select new BookedDatesCustomized
                                  {
                                      PRNO = packge.PRNO,
                                      PackageType = packge.PackageType,
                                      PackageName = packge.PackageName,
                                      NoOfDays = (int)packge.NoOfDays,
                                      ArrivalDate = packge.ArrivalDate,
                                      DepartureDate = packge.DepartureDate,
                                      AlternateArrivalDate = packge.AlternateArrivalDate,
                                      AlternateDepartureDate = packge.AlternateDepartureDate,
                                      AccompanyingGuests = (int)packge.AccompanyingGuests,
                                      RoomView = packge.RoomView,
                                      RoomType = packge.RoomType,
                                      RoomTariff = packge.RoomTariff,
                                      ConfirmedFromDate = packge.ConfirmedFromDate,
                                      ConfirmedToDate = packge.ConfirmedToDate,
                                      DoctorsComments = packge.DoctorsComments,
                                      PackageConfirmStatus = packge.PackageConfirmStatus,
                                      Package_Amount = packge.Package_Amount,
                                      Id = packge.Id,
                                  }).FirstOrDefault();
            return PartialView("PRPackageDetails", PackageDetails);
        }
        public ActionResult HLDetails(string PRNum)
        {
            var query = (from c in db.NC_TBL_HEALTH_DETAILS
                         join p in db.NC_TBL_HLQuest_master
                         on c.HLCode equals p.HLCode
                         where c.PRNO == PRNum
                         orderby c.HLCode
                         select new HDModel
                         {
                             Id = c.Id,
                             HLCode = c.HLCode,
                             HLFlag = c.HLFlag == "Y" ? "Yes" : "No",
                             HLDesc = c.HLDesc,
                             HLQuestion = p.HLQuestion
                         });
            return PartialView("PRHealthDetails", query.ToList());
        }
        public ActionResult HBDetails(string PRNum)
        {
            var query = (from c in db.NC_TBL_HABIT_DETAILS
                         join p in db.NC_TBL_HBQuest_master
                         on c.HBQCode equals p.HBCode
                         where c.PRNO == PRNum
                         orderby c.HBQCode
                         select new HabitModel
                         {
                             Id = c.Id,
                             HBCode = c.HBQCode,
                             HBQFreq = c.HBQFreq == "0" ? "None" :
                             c.HBQFreq == "1" ? "Daily" : "Occasionally",
                             HBDesc = c.HBDesc,
                             HBQuestion = p.HBQuestion,
                         });
            return PartialView("PRHabitDetails", query.ToList());
        }

        public ActionResult PaymodeTypes()
        {


            ///Gateway paymode Logic to differentiate the two gateways
            var items = (from recieptmode in db.NC_TBL_GATEWAYPAYMENT_MODE_MASTER
                         where new[] { 1, 2, 3 }.Contains(recieptmode.PAYMENT_MODE_CODE)
                         select new
                         {
                             Paymode = recieptmode.PAYMENT_MODE,
                             PaymodeCode = recieptmode.PAYMENT_MODE_CODE
                         }).Distinct();

            if (items.Count() > 0)
            {
                return Json(items.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }



        }
    }
}
