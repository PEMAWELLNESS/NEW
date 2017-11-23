using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Data.Entity;
using System.Globalization;

namespace Rooms.Controllers
{
    public class CheckinController : Controller
    {
        // GET: Checkin
        MisRoomsEntities db = new MisRoomsEntities();
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var ID = collection["rowCount"];
            for (int j = 0; j <= Convert.ToInt32(ID); j++)
            {
                string prnumber = collection["prno" + j].ToString();
                var persondetails = db.NC_TBL_PERSONAL_DETAILS.Where(i => i.PRNO == prnumber).FirstOrDefault();
                persondetails.FirstName = collection["fname" + j].ToString();
                persondetails.LastName = collection["lname" + j].ToString();
                persondetails.MobileNo = collection["mbl" + j].ToString();
                persondetails.Age = Convert.ToInt32(collection["age" + j]);
                persondetails.Gender = collection["Gender" + j].ToString();
                persondetails.EmailId = collection["email" + j].ToString();
                persondetails.FlatNo = collection["flat" + j];
                persondetails.Street = collection["street" + j].ToString();
                persondetails.City = collection["city" + j].ToString();
                persondetails.Nationality = collection["NationalityDrop" + j];
                persondetails.Country = collection["CountryDrop" + j].ToString();
                //persondetails.Room_No = collection["roomno"].ToString();
                persondetails.CheckinDate = DateTime.Now;
                persondetails.CheckinProcess = "Y";
                db.NC_TBL_PERSONAL_DETAILS.Attach(persondetails);
                var entry = db.Entry(persondetails);
                entry.State = EntityState.Modified;
                entry.Property(e => e.CheckinDate).IsModified = true;
                entry.Property(e => e.CheckinProcess).IsModified = true;
                entry.Property(e => e.FirstName).IsModified = true;
                entry.Property(e => e.LastName).IsModified = true;
                entry.Property(e => e.MobileNo).IsModified = true;
                entry.Property(e => e.Age).IsModified = true;
                entry.Property(e => e.Gender).IsModified = true;
                entry.Property(e => e.EmailId).IsModified = true;
                entry.Property(e => e.FlatNo).IsModified = true;
                entry.Property(e => e.Street).IsModified = true;
                entry.Property(e => e.City).IsModified = true;
                entry.Property(e => e.Nationality).IsModified = true;
                entry.Property(e => e.Country).IsModified = true;
                //entry.Property(e => e.Room_No).IsModified = true;
                int k = db.SaveChanges();
                NC_TBL_ROOM_Status status = new NC_TBL_ROOM_Status();
                status.PRNO = collection["prno" + j].ToString(); ;
                status.Room_No = Convert.ToInt32(collection["roomno"].ToString());
                status.Status = "Occupied";
                status.Date_From = DateTime.Now;
                status.Created_by = Session["LogedUserID"].ToString();
                status.Created_dt = DateTime.Now;
                db.NC_TBL_ROOM_Status.Add(status);
                db.SaveChanges();
                var packagedetails = db.NC_TBL_BOOKED_DATES.Where(i => i.PRNO == prnumber).FirstOrDefault();
                packagedetails.PackageName = collection["PackageNameDrop"].ToString();
                packagedetails.RoomView = collection["RoomviewDrop"].ToString();
                packagedetails.RoomType = collection["roomtypedrop"].ToString();
                packagedetails.ArrivalDate = DateTime.ParseExact(collection["arrivaldate"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //packagedetails.ArrivalDate = Convert.ToDateTime(collection["arrivaldate"].ToString());
                //packagedetails.DepartureDate = Convert.ToDateTime(collection["departuredate"].ToString());
                //packagedetails.ArrivalDate = Convert.ToDateTime(collection["alternatearrivaldate"].ToString());
                //packagedetails.DepartureDate = Convert.ToDateTime(collection["alternatedepartureldate"].ToString());
                if (collection["AccompanyTypeDrop"].ToString() == "-1")
                {
                    packagedetails.Accompany_Type = null;
                }
                else
                {
                    packagedetails.Accompany_Type = collection["AccompanyTypeDrop"].ToString();
                }
                db.NC_TBL_BOOKED_DATES.Attach(packagedetails);
                var entry1 = db.Entry(packagedetails);
                entry1.Property(e => e.PackageName).IsModified = true;
                entry1.Property(e => e.RoomView).IsModified = true;
                entry1.Property(e => e.RoomType).IsModified = true;
                entry1.Property(e => e.ArrivalDate).IsModified = true;
                entry1.Property(e => e.Accompany_Type).IsModified = true;
                int l = db.SaveChanges();


                int Room_NoNew = Convert.ToInt32(collection["roomno"].ToString());
                var RoomDet = db.NC_TBL_ROOM_Status.Where(i => i.Room_No == Room_NoNew && i.Status == "Vacant" && i.Date_To == null).FirstOrDefault();
                if (RoomDet != null)
                {
                    RoomDet.Date_To = DateTime.Now;
                    RoomDet.Modified_by = Session["LogedUserID"].ToString();
                    RoomDet.Modified_dt = DateTime.Now;
                    db.NC_TBL_ROOM_Status.Attach(RoomDet);
                    var entry2 = db.Entry(RoomDet);
                    entry2.Property(e => e.Date_To).IsModified = true;
                    entry2.Property(e => e.Modified_by).IsModified = true;
                    entry2.Property(e => e.Date_To).IsModified = true;
                    int l2 = db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Guestcheckin()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Guestcheckin(FormCollection collection)
        {
            NC_TBL_ROOM_Status status = new NC_TBL_ROOM_Status();
            status.Guest_Name = collection["fname"].ToString();
            status.PRNO = collection["prno"].ToString(); ;
            status.Room_No = Convert.ToInt32(collection["roomno"].ToString());
            status.Gender = collection["gender"].ToString();
            status.Status = "Occupied";
            status.Date_From = Convert.ToDateTime(collection["Checkindate"]);
            status.Created_by = Session["LogedUserID"].ToString();
            status.Created_dt = DateTime.Now;
            db.NC_TBL_ROOM_Status.Add(status);
            db.SaveChanges();
            int Room_NoNew = Convert.ToInt32(collection["roomno"].ToString());
            var RoomDet = db.NC_TBL_ROOM_Status.Where(i => i.Room_No == Room_NoNew && i.Status == "Vacant" && i.Date_To == null).FirstOrDefault();
            if (RoomDet != null)
            {
                RoomDet.Date_To = DateTime.Now;
                RoomDet.Modified_by = Session["LogedUserID"].ToString();
                RoomDet.Modified_dt = DateTime.Now;
                db.NC_TBL_ROOM_Status.Attach(RoomDet);
                var entry2 = db.Entry(RoomDet);
                entry2.Property(e => e.Date_To).IsModified = true;
                entry2.Property(e => e.Modified_dt).IsModified = true;
                entry2.Property(e => e.Modified_by).IsModified = true;
                int l2 = db.SaveChanges();
            }
            return RedirectToAction("Guestcheckin");
        }
        public ActionResult GetGuestdetails(string prno)
        {
            var Guestdetails = (
                from a in db.NC_TBL_PERSONAL_DETAILS
                join b in db.NC_TBL_BOOKED_DATES on a.PRNO equals b.PRNO
                join c in db.NC_TBL_ROOM_Status on a.PRNO equals c.PRNO
                where (a.PRNO == prno && c.PRNO != prno)
                select new { a, b }).ToList();
            //var Guestdetails = db.NC_TBL_PERSONAL_DETAILS.Where(i => i.UHID == uhid).Select(i => new {
            //i.FirstName,
            //i.LastName,
            //i.MobileNo,
            //i.Age,
            //i.FlatNo,
            //i.Building,
            //i.Street,
            //i.City,
            //i.Nationality,
            //i.Country,
            //i.Gender,
            //i.EmailId,
            //i.
            //});
            return Json(Guestdetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShiftingRoom()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult ShiftingRoom(FormCollection coll)
        {
            try
            {
                string guest0 = coll["Guest0"].ToString();
                string PRNO = coll["guestid0"].ToString();
                string guestname = coll["guestname0"].ToString();
                string guestgender = coll["guestgender0"].ToString();
                int roomno = Convert.ToInt32(coll["roomno0"]);
                int shiftroom = Convert.ToInt32(coll["shiftroom0"].ToString());
                VacateRoom(roomno, PRNO);
                OccupyRoom(PRNO, shiftroom, guestname, guestgender);
            }
            catch
            {

            }
            try
            {
                string guest0 = coll["Guest1"].ToString();
                string PRNO = coll["guestid1"].ToString();
                string guestname = coll["guestname1"].ToString();
                string guestgender = coll["guestgender1"].ToString();
                int roomno = Convert.ToInt32(coll["roomno1"]);
                int shiftroom = Convert.ToInt32(coll["shiftroom1"].ToString());
                VacateRoom(roomno, PRNO);
                OccupyRoom(PRNO, shiftroom, guestname, guestgender);
            }
            catch
            {

            }
            return RedirectToAction("ShiftingRoom");
        }
        public int VacateRoom(int roomno, string prno)
        {
            int count = 0;
            var vacatingroom = db.NC_TBL_ROOM_Status.Where(i => i.Room_No == roomno && i.Status == "Occupied" && i.Date_To == null && i.PRNO == prno).FirstOrDefault();
            vacatingroom.Date_To = DateTime.Now;
            vacatingroom.Modified_by = Session["LogedUserID"].ToString();
            vacatingroom.Modified_dt = DateTime.Now;
            db.NC_TBL_ROOM_Status.Attach(vacatingroom);
            var vas = db.Entry(vacatingroom);
            vas.State = EntityState.Modified;
            vas.Property(e => e.Date_To).IsModified = true;
            vas.Property(e => e.Modified_dt).IsModified = true;
            vas.Property(e => e.Modified_by).IsModified = true;
            count = db.SaveChanges();
            var existing = db.NC_TBL_ROOM_Status.Where(i => i.Room_No == roomno && i.Status == "Occupied" && i.Date_To == null).Count();
            if (existing == 0)
            {
                NC_TBL_ROOM_Status vacate = new NC_TBL_ROOM_Status();
                vacate.Room_No = roomno;
                vacate.Status = "Vacant";
                vacate.Date_From = DateTime.Now;
                vacate.Created_by = Session["LogedUserID"].ToString();
                vacate.Created_dt = DateTime.Now;
                db.NC_TBL_ROOM_Status.Add(vacate);
                db.SaveChanges();
            }
            return count;
        }
        public int OccupyRoom(string PRNO, int roomno, string guestname, string gender)
        {
            var alreaadyvacate = db.NC_TBL_ROOM_Status.Where(i => i.Room_No == roomno && i.Status == "Vacant" && i.Date_To == null).FirstOrDefault();
            if (alreaadyvacate != null)
            {
                alreaadyvacate.Date_To = DateTime.Now;
                alreaadyvacate.Modified_by = Session["LogedUserID"].ToString();
                alreaadyvacate.Modified_dt = DateTime.Now;
                db.NC_TBL_ROOM_Status.Attach(alreaadyvacate);
                var vas = db.Entry(alreaadyvacate);
                vas.State = EntityState.Modified;
                vas.Property(e => e.Date_To).IsModified = true;
                vas.Property(e => e.Modified_dt).IsModified = true;
                vas.Property(e => e.Modified_by).IsModified = true;
                db.SaveChanges();
            }
            NC_TBL_ROOM_Status Occupy = new NC_TBL_ROOM_Status();
            Occupy.PRNO = PRNO;
            Occupy.Room_No = roomno;
            Occupy.Guest_Name = guestname;
            Occupy.Gender = gender;
            Occupy.Date_From = DateTime.Now;
            Occupy.Created_by = Session["LogedUserID"].ToString();
            Occupy.Created_dt = DateTime.Now;
            Occupy.Status = "Occupied";
            db.NC_TBL_ROOM_Status.Add(Occupy);
            int count = db.SaveChanges();
            return count;
        }
        [ActionName("VR")]
        public JsonResult VacantRooms()
        {
            var result = db.NC_TBL_ROOM_Status.Where(i => i.Status == "Vacant" && i.Date_To == null).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RoomStatusDetails()
        {
            DateTime dt = DateTime.Today;
            //var Guestdetails = (
            //    from a in db.NC_TBL_ROOM_Status
            //    where (a.Status=="Occupied")
            //    select new { a.Room_No }).ToList();
            //var Guestdetails = db.NC_TBL_ROOM_Status.GroupBy(fu => fu.Room_No)
            //  .Select(g => new { Room_No = g.Key, Count = g.Count() });

            var Guestdetails = (from a in db.NC_TBL_ROOM_Status
                                where a.Date_To == null
                                group a by new { a.Room_No, a.Status } into d
                                select new { Room_No = d.Key.Room_No, Status = d.Key.Status, Count = d.Count() });
            return Json(Guestdetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getvacantcount()
        {
            DateTime dt = DateTime.Today;
            var model = (from a in db.NC_TBL_ROOM_Status.Where(a => a.Status == "Vacant" && a.Date_To == null) select a).Count();
            return Content(model.ToString());
        }
        public ActionResult getsinglecount()
        {
            DateTime dt = DateTime.Today;
            var model = (from a in db.NC_TBL_ROOM_Status
                         where (a.Date_To == null && a.Status == "occupied")
                         group a by new { a.Room_No } into d
                         select new { Room_No = d.Key.Room_No, Count = d.Count() }).ToList();
            var count = (from b in model.Where(b => b.Count == 1) select b).Count();
            return Content(count.ToString());
        }
        public ActionResult getdoublecount()
        {
            DateTime dt = DateTime.Today;
            var model = (from a in db.NC_TBL_ROOM_Status
                         where (a.Date_To == null && a.Status == "occupied")
                         group a by new { a.Room_No } into d
                         select new { Room_No = d.Key.Room_No, Count = d.Count() }).ToList();
            var count = (from b in model.Where(b => b.Count == 2) select b).Count();
            return Content(count.ToString());
        }
        public ActionResult getmaintenancecount()
        {
            DateTime dt = DateTime.Today;
            var model = (from a in db.NC_TBL_ROOM_Status.Where(a => a.Status == "Maintenance" && a.Date_To == null) select a).Count();
            return Content(model.ToString());
        }
        public JsonResult Generateprno()
        {
            string prno = (1000 + (db.NC_TBL_ROOM_Status.Max(a => a.ID)) + 1).ToString();
            string genprno = "P" + DateTime.Today.ToString("MM") + prno;
            //string genprno = prno.Substring(prno.Length - Math.Min(3, prno.Length));
            return Json(genprno, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Roomdashboard()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult totalroomsoccupied()
        {
            var list = db.NC_TBL_ROOM_Status.Where(a => a.Date_To == null && a.Status == "Occupied").GroupBy(a => a.Room_No).Count();
            return Content(list.ToString());
        }
        public ActionResult totalguests()
        {
            var list = db.NC_TBL_ROOM_Status.Where(a => a.Date_To == null && a.Status == "Occupied").Count();
            return Content(list.ToString());
        }
        public ActionResult totalmaleguests()
        {
            var list = db.NC_TBL_ROOM_Status.Where(a => a.Date_To == null && a.Status == "Occupied" && a.Gender == "Male").Count();
            return Content(list.ToString());
        }
        public ActionResult totalfemaleguests()
        {
            var list = db.NC_TBL_ROOM_Status.Where(a => a.Date_To == null && a.Status == "Occupied" && a.Gender == "Female").Count();
            return Content(list.ToString());
        }
        public ActionResult guestlist()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public JsonResult Totalguestlist(string dated)
        {
            DateTime scheduledate = DateTime.ParseExact(dated, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result = db.NC_TBL_ROOM_Status.Where(i => i.Status == "Occupied" && i.Date_To == null && i.Date_From <= scheduledate).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GuestPreBooking()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult GuestPreBooking(FormCollection collection)
        {
            if (string.IsNullOrEmpty(Session["UserCode"].ToString() as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                NC_TBL_Pre_Booking status = new NC_TBL_Pre_Booking();
                status.Guest_Name = collection["fname"].ToString();
                status.Booked_NO = collection["prno"].ToString();
                status.Room_No = Convert.ToInt32(collection["roomno"].ToString());
                status.Gender = collection["gender"].ToString();
                //status.Status = "Occupied";
                status.Status = collection["Status"].ToString();
                status.Date_From = DateTime.ParseExact(collection["Checkindate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //Convert.ToDateTime(collection["Checkindate"]);
                if (collection["Checkoutdate1"] != null && collection["Checkoutdate1"] != "")
                {
                    status.Date_To = DateTime.ParseExact(collection["Checkoutdate1"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //Convert.ToDateTime(collection["Checkoutdate1"]);
                }
                else
                {
                    status.Date_To = null;
                }
                status.Email = collection["email"].ToString();
                status.Mobile_No = collection["number"].ToString();
                status.Created_by = Session["UserCode"].ToString(); ;
                status.Created_dt = DateTime.Today;
                db.NC_TBL_Pre_Booking.Add(status);
                db.SaveChanges();
                int Room_NoNew = Convert.ToInt32(collection["roomno"].ToString());
                var RoomDet = db.NC_TBL_Pre_Booking.Where(i => i.Room_No == Room_NoNew && i.Status == "Vacant" && i.Date_To == null).FirstOrDefault();
                if (RoomDet != null)
                {
                    RoomDet.Date_To = DateTime.Now;
                    RoomDet.Created_by = Session["UserCode"].ToString(); ;
                    RoomDet.Created_dt = DateTime.Today;
                    db.NC_TBL_Pre_Booking.Attach(RoomDet);
                    var entry2 = db.Entry(RoomDet);
                    entry2.Property(e => e.Date_To).IsModified = true;
                    entry2.Property(e => e.Created_by).IsModified = true;
                    entry2.Property(e => e.Created_dt).IsModified = true;
                    int l2 = db.SaveChanges();
                }

                return RedirectToAction("GuestPreBooking");
            }
        }
        public JsonResult GenerateBookindNo()
        {
            string prno = (1000 + (db.NC_TBL_Pre_Booking.Max(a => a.ID)) + 1).ToString();
            string genprno = "B" + DateTime.Today.ToString("MM") + prno;
            //string genprno = prno.Substring(prno.Length - Math.Min(3, prno.Length));
            return Json(genprno, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getvacantcountForPreBook(string fd)
        {
            DateTime dt = DateTime.ParseExact(fd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var model = (from a in db.NC_TBL_Pre_Booking.Where(a => a.Status == "Vacant" && a.Date_To == null) select a).Count();
            return Content(model.ToString());
        }

        public ActionResult getmaintenancecountForPreBook(string fd)
        {
            DateTime dt = DateTime.ParseExact(fd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var model = (from a in db.NC_TBL_Pre_Booking.Where(a => a.Status == "Maintenance" && a.Date_To == null) select a).Count();
            return Content(model.ToString());
        }
        public ActionResult GetBlockedcount(string fd)
        {
            DateTime dt = DateTime.ParseExact(fd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var model = (from a in db.NC_TBL_Pre_Booking.Where(a => a.Status == "Blocked" && (a.Date_From <= dt && a.Date_To >= dt)) select a).Count();
            return Content(model.ToString());
        }
        public ActionResult RoomStatusDetailsForPreBook(string fd)
        {
            // DateTime dt = DateTime.Today;
            DateTime dt = DateTime.ParseExact(fd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var Guestdetails = (from a in db.NC_TBL_Pre_Booking
                                where a.Date_To == null || (a.Status == "Blocked" && (a.Date_From <= dt && a.Date_To >= dt))
                                group a by new { a.Room_No, a.Status } into d
                                select new { Room_No = d.Key.Room_No, Status = d.Key.Status, Count = d.Count() });
            return Json(Guestdetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBookeddetails(string fd, int RNO)
        {
            DateTime scheduledate = DateTime.ParseExact(fd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var Guestdetails = (
                from i in db.NC_TBL_Pre_Booking
                where (i.Room_No == RNO && i.Status == "Blocked" && ((scheduledate >= i.Date_From && i.Date_To >= scheduledate)))
                select new
                {
                    Date_From = i.Date_From,
                    Date_To = i.Date_To,
                }).ToList();
            return Json(Guestdetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PreBookingguestlist()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public JsonResult TotalPreBookingguestlist(string dated)
        {
            DateTime scheduledate = DateTime.ParseExact(dated, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result = db.NC_TBL_Pre_Booking.Where(i => i.Status == "Blocked" && (scheduledate >= i.Date_From && i.Date_To >= scheduledate)).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditPreBooking(string Booked_NO)
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ID = Booked_NO;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditPreBooking(FormCollection collection)
        {
            if (string.IsNullOrEmpty(Session["UserCode"].ToString() as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string prnumber = collection["prno"].ToString();
                var status = db.NC_TBL_Pre_Booking.Where(kkl => kkl.Booked_NO == prnumber).FirstOrDefault();
                status.Guest_Name = collection["fname"].ToString();
                status.Booked_NO = collection["prno"].ToString();
                status.Room_No = Convert.ToInt32(collection["roomno"].ToString());
                status.Gender = collection["gender"].ToString();
                status.Status = collection["Status"].ToString();
                status.Date_From = DateTime.ParseExact(collection["Checkindate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                status.Date_To = DateTime.ParseExact(collection["Checkoutdate1"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                status.Email = collection["email"].ToString();
                status.Mobile_No = collection["number"].ToString();
                status.Modified_by = Session["UserCode"].ToString(); 
                status.MOdified_dt = DateTime.Today;
                db.NC_TBL_Pre_Booking.Attach(status);
                var entry = db.Entry(status);
                entry.State = EntityState.Modified;
                entry.Property(e => e.Guest_Name).IsModified = true;
                entry.Property(e => e.Gender).IsModified = true;
                entry.Property(e => e.Email).IsModified = true;
                entry.Property(e => e.Mobile_No).IsModified = true;
                entry.Property(e => e.Room_No).IsModified = true;
                entry.Property(e => e.Date_From).IsModified = true;
                entry.Property(e => e.Date_To).IsModified = true;
                entry.Property(e => e.Status).IsModified = true;
                entry.Property(e => e.MOdified_dt).IsModified = true;
                entry.Property(e => e.Modified_by).IsModified = true;
                int k = db.SaveChanges();
                return Content("<script>alert('Updated Successful'); window.location = './PreBookingguestlist';</script>");
            }
        }

        public ActionResult GetPreBookingDet(string Booked_NO)
        {
            var items = (from ed in db.NC_TBL_Pre_Booking
                         select new
                         {
                             Booked_NO = ed.Booked_NO,
                             Guest_Name = ed.Guest_Name,
                             Room_No = ed.Room_No,
                             Gender = ed.Gender,
                             Mobile = ed.Mobile_No,
                             Email = ed.Email,
                             Status = ed.Status,
                             Date_From = ed.Date_From,
                             Date_To = ed.Date_To
                         }).Where(i => i.Booked_NO == Booked_NO);
            if (items.Count() > 0)
            {
                var jsonResult = Json(items, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(items.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Editguestlist(string PRNO)
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ID = PRNO;
                return View();
            }
        }
        [HttpPost]
        public ActionResult Editguestlist(FormCollection collection)
        {
            if (string.IsNullOrEmpty(Session["UserCode"].ToString() as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string prnumber = collection["PRNO"].ToString();
                var status = db.NC_TBL_ROOM_Status.Where(kkl => kkl.PRNO == prnumber).FirstOrDefault();
                status.Guest_Name = collection["fname"].ToString();
                status.PRNO = collection["prno"].ToString();
                status.Room_No = Convert.ToInt32(collection["roomno"].ToString());
                status.Gender = collection["gender"].ToString();
                status.Date_From = DateTime.ParseExact(collection["Checkindate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                status.Modified_by = Session["LogedUserID"].ToString();
                status.Modified_dt = DateTime.Now;
                db.NC_TBL_ROOM_Status.Attach(status);
                var entry = db.Entry(status);
                entry.State = EntityState.Modified;
                entry.Property(e => e.Guest_Name).IsModified = true;
                entry.Property(e => e.Gender).IsModified = true;
                entry.Property(e => e.Room_No).IsModified = true;
                entry.Property(e => e.Date_From).IsModified = true;
                entry.Property(e => e.Modified_dt).IsModified = true;
                entry.Property(e => e.Modified_by).IsModified = true;
                int k = db.SaveChanges();
                return Content("<script>alert('Updated Successful'); window.location = './guestlist';</script>");
            }
        }

        public ActionResult GeguestlistDet(string PRNO)
        {
            var items = (from ed in db.NC_TBL_ROOM_Status
                         select new
                         {
                             PRNO = ed.PRNO,
                             Guest_Name = ed.Guest_Name,
                             Room_No = ed.Room_No,
                             Gender = ed.Gender,
                             Date_From = ed.Date_From
                         }).Where(i => i.PRNO == PRNO);
            if (items.Count() > 0)
            {
                var jsonResult = Json(items, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(items.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
