using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Globalization;

namespace Rooms.Controllers
{
    public class TPController : Controller
    {
        private MisRoomsEntities db = new MisRoomsEntities();
        #region maincode
        // GET: TP
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = DateTime.Today.AddDays(1);
                var treatmentAvaileds = db.TreatmentAvaileds.Where(i => i.ScheduleDate == today || i.ScheduleDate == tomorrow && i.Treatment_Status == "Y").Include(t => t.HealerDetail).Include(t => t.Treatment_Master).Include(t => t.TreatmentRoomMaster);
                return View(treatmentAvaileds.ToList());
            }
        }
        public ActionResult TreatmentList()
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
        // GET: TP/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreatmentAvailed treatmentAvailed = db.TreatmentAvaileds.Find(id);
            if (treatmentAvailed == null)
            {
                return HttpNotFound();
            }
            return View(treatmentAvailed);
        }

        // GET: TP/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME");
                ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatName");
                ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName");
                return View();
            }
        }
        public ActionResult Create1()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME");
                //ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatName");
                //ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName");
                return View();
            }
        }
        public JsonResult Getdrophealerlist()
        {
            var result = db.HealerDetails.Select(i => new { i.EMPNO, i.EMPNAME }).OrderBy(i => i.EMPNAME).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdroptreatlist()
        {
            var result = db.Treatment_Master.Select(i => new { i.TreatmentID, i.TreatName }).OrderBy(i => i.TreatName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdroproomlist()
        {
            var result = db.TreatmentRoomMasters.Select(i => new { i.ID, i.RoomName }).OrderBy(i => i.RoomName).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // POST: TP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PRNO,GuestName,HealerID,TreatmentID,RoomID,SchdeuleTime,StartTime,EndTime")] TreatmentAvailed treatmentAvailed)
        {
            if (ModelState.IsValid)
            {
                treatmentAvailed.ScheduleDate = DateTime.Today.AddDays(1);
                db.TreatmentAvaileds.Add(treatmentAvailed);
                db.SaveChanges();
                return RedirectToAction("TreatmentList");
            }

            ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME", treatmentAvailed.HealerID);
            ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatName", treatmentAvailed.TreatmentID);
            ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
            return View(treatmentAvailed);
        }

        // GET: TP/Edit/5
        public ActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TreatmentAvailed treatmentAvailed = db.TreatmentAvaileds.Find(id);
                if (treatmentAvailed == null)
                {
                    return HttpNotFound();
                }
                ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME", treatmentAvailed.HealerID);
                ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatName", treatmentAvailed.TreatmentID);
                ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
                return View(treatmentAvailed);
            }
        }

        // POST: TP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PRNO,HealerID,TreatmentID,RoomID,ScheduleDate,SchdeuleTime,StartTime,EndTime")] TreatmentAvailed treatmentAvailed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(treatmentAvailed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME", treatmentAvailed.HealerID);
            ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatName", treatmentAvailed.TreatmentID);
            ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
            return View(treatmentAvailed);
        }

        // GET: TP/Delete/5
        public ActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TreatmentAvailed treatmentAvailed = db.TreatmentAvaileds.Find(id);
                if (treatmentAvailed == null)
                {
                    return HttpNotFound();
                }
                return View(treatmentAvailed);
            }
        }

        // POST: TP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TreatmentAvailed treatmentAvailed = db.TreatmentAvaileds.Find(id);
            db.TreatmentAvaileds.Remove(treatmentAvailed);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult AddHealers()
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
        public ActionResult AddHealers(HealerDetail HD)
        {
            if (ModelState.IsValid)
            {
                db.HealerDetails.Add(HD);
                db.SaveChanges();

            }
            return RedirectToAction("HealerList");
        }
        public ActionResult HealerList()
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
        public ActionResult HealerList(FormCollection coll)
        {
            int empno = Convert.ToInt32(coll["EMPNO"]);
            var existing = db.HealerDetails.Where(i => i.EMPNO == empno).FirstOrDefault();
            existing.EMPSTATUS = coll["EMPSTATUS"].ToString();
            var entry = db.Entry(existing);
            entry.State = EntityState.Modified;
            entry.Property(e => e.EMPSTATUS).IsModified = true;
            db.SaveChanges();
            return RedirectToAction("HealerList");
        }
        #endregion
        #region AJAX
        [HttpPost]
        public JsonResult AddTreatment(string PRNO, string GuestName, TimeSpan ScheduleTime, TimeSpan StartTime, TimeSpan EndTime, int Healer, int Treatment, int Room)
        {
            TreatmentAvailed treate = new TreatmentAvailed();
            treate.PRNO = PRNO;
            treate.GuestName = GuestName;
            treate.SchdeuleTime = ScheduleTime;
            treate.StartTime = StartTime;
            treate.EndTime = EndTime;
            //var heleraid = db.HealerDetails.Where(a => a.EMPNAME == Healer).Select(a => a.EMPNO).ToList();
            //var treateid = db.Treatment_Master.Where(a => a.TreatName == Treatment).Select(a => a.TreatmentID).ToList();
            //var roomid = db.TreatmentRoomMasters.Where(a => a.RoomName == Room).Select(a => a.ID).ToList();
            treate.HealerID = Healer;
            treate.TreatmentID = Treatment;
            treate.RoomID = Room;
            treate.Treatment_Status = "N";
            treate.ScheduleDate = DateTime.Today.AddDays(1);
            db.TreatmentAvaileds.Add(treate);
            db.SaveChanges();
            var Redirect = "";
            var message = "Success";
            return Json(new { Value = "Success", message, Redirect }, JsonRequestBehavior.AllowGet);
        }
        [ActionName("VH")]
        public ContentResult VerifyHealer(int empno)
        {
            var result = db.HealerDetails.Where(i => i.EMPNO == empno).Count();
            return Content(result.ToString());
        }
        [ActionName("GH")]
        public JsonResult GETHEALERS()
        {
            var result = db.HealerDetails.Where(i => i.EMPSTATUS == "Active").Select(i => new { i.EMPNO, i.EMPNAME, i.EMPSTATUS, i.Designation, i.Gender }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GETPRNO()
        {
            var Guestlist = db.NC_TBL_ROOM_Status.Where(i => i.Status == "Occupied" && i.Date_To == null).OrderBy(i => i.PRNO).ToList();
            return Json(Guestlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoomNumbers(string StartTime, string EndTime)
        {
            TimeSpan starttime = TimeSpan.Parse(StartTime);
            TimeSpan endtime = TimeSpan.Parse(EndTime);
            var roomslist = (from TRM in db.TreatmentRoomMasters
                             where !(from TA in db.TreatmentAvaileds
                                     where TA.StartTime >= starttime && TA.EndTime >= endtime
                                     select TA.RoomID).Contains(TRM.ID) // TRM.RoomName.Contains(occupiedroomslist)
                             select new
                             {
                                 Id = TRM.ID,
                                 RoomNumber = TRM.RoomName
                             }).OrderBy(i => i.RoomNumber).ToList();
            return Json(roomslist, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GHG")]
        public JsonResult GETHEALERGENDERWISE(string PRNO, string StartTime, string EndTime)
        {
            TimeSpan starttime = TimeSpan.Parse(StartTime);
            TimeSpan endtime = TimeSpan.Parse(EndTime);
            DateTime scheduledate = DateTime.Today.AddDays(1);
            var GUESTGENDER = (from PD in db.NC_TBL_ROOM_Status
                               where PD.PRNO == PRNO
                               select PD.Gender).FirstOrDefault();
            var HEALERS = (from HD in db.HealerDetails
                           where (HD.Gender == GUESTGENDER || HD.Gender == null)
                           select HD).ToList();
            var HEALERDETAILS = (from HD in HEALERS
                                 where !(from PD in db.NC_TBL_ROOM_Status
                                         join TA in db.TreatmentAvaileds on PD.PRNO equals TA.PRNO
                                         where TA.ScheduleDate == scheduledate && (starttime >= TA.StartTime && starttime <= TA.EndTime || endtime > TA.StartTime && endtime < TA.EndTime)
                                         //where TA.StartTime >= starttime && TA.EndTime >= endtime
                                         select TA.HealerID).Contains(HD.EMPNO) && HD.EMPSTATUS == "Active"
                                 select new
                                 {
                                     HD.Gender,
                                     HD.EMPNO,
                                     HD.EMPNAME
                                 }).OrderBy(i => i.EMPNAME).ToList();
            return Json(HEALERDETAILS, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GHG1")]
        public JsonResult GETHEALERGENDERWISE1(int PRNO)
        {

            var HEALERS = (from HD in db.HealerDetails
                           where HD.EMPNO == PRNO
                           select new
                           {
                               HD.Gender,
                               HD.EMPNO,
                               HD.EMPNAME
                           }).OrderBy(i => i.EMPNAME).ToList();
            return Json(HEALERS, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GTN")]
        public JsonResult GETTREATMENTNAMES(string PRNO)
        {
            var TREATMENTDETAILS = (from TD in db.Treatment_Master
                                        //where !(from TA in db.TreatmentAvaileds 
                                        //join TM in db.Treatment_Master on TA.TreatmentID equals TM.TreatmentID
                                        //where TA.PRNO==PRNO
                                        //select TM.TreatmentID).Contains(TD.TreatmentID)
                                    select new
                                    {
                                        TD.TreatmentID,
                                        TD.TreatName
                                    }).OrderBy(i => i.TreatName).ToList();
            return Json(TREATMENTDETAILS, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GTN1")]
        public JsonResult GETTREATMENTNAMES1(int PRNO)
        {
            var TREATMENTDETAILS = (from TD in db.Treatment_Master
                                        //where !(from TA in db.TreatmentAvaileds 
                                        //join TM in db.Treatment_Master on TA.TreatmentID equals TM.TreatmentID
                                    where TD.TreatmentID == PRNO
                                    //select TM.TreatmentID).Contains(TD.TreatmentID)
                                    select new
                                    {
                                        TD.TreatmentID,
                                        TD.TreatName
                                    }).OrderBy(i => i.TreatName).ToList();
            return Json(TREATMENTDETAILS, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GRD")]
        public JsonResult GETROOMDETAILS(Int32 TRNAME, string StartTime)
        {
            TimeSpan starttime = TimeSpan.Parse(StartTime);
            DateTime scheduledate = DateTime.Today.AddDays(1);
            var x = (from TA in db.TreatmentAvaileds
                     join TM in db.TreatmentRoomMasters on TA.RoomID equals TM.ID
                     where TA.ScheduleDate == scheduledate && TA.Treatment_Status == "Y" && TA.StartTime <= starttime && TA.EndTime >= starttime
                     select TM.ID).ToList();
            var ROOMDETAILS = (from TM in db.Treatment_Master
                               join TRM in db.TreatmentRoomMasters on TM.TreatmentGroupID equals TRM.TreatmentGroupID
                               where TM.TreatmentID == TRNAME && !(from TA in db.TreatmentAvaileds
                                                                   join TM in db.TreatmentRoomMasters on TA.RoomID equals TM.ID
                                                                   where TA.ScheduleDate == scheduledate && TA.StartTime <= starttime && TA.EndTime >= starttime
                                                                   select TM.ID).Contains(TRM.ID)
                               select new
                               {
                                   TRM.ID,
                                   TRM.RoomName
                               }).OrderBy(i => i.RoomName).ToList();
            return Json(ROOMDETAILS, JsonRequestBehavior.AllowGet);

        }
        [ActionName("GRD1")]
        public JsonResult GETROOMDETAILS1(string TRNAME, string StartTime)
        {
            TimeSpan starttime = TimeSpan.Parse(StartTime);
            DateTime scheduledate = DateTime.Today.AddDays(1);

            var x = (from TA in db.TreatmentAvaileds
                     join TM in db.TreatmentRoomMasters on TA.RoomID equals TM.ID
                     where TA.ScheduleDate == scheduledate && TA.StartTime <= starttime && TA.EndTime >= starttime && TA.Treatment_Status == "Y"
                     select TM.ID).ToList();
            var ROOMDETAILS = (from TM in db.Treatment_Master
                               join TRM in db.TreatmentRoomMasters on TM.TreatmentGroupID equals TRM.TreatmentGroupID
                               where TM.TreatName == TRNAME && !(from TA in db.TreatmentAvaileds
                                                                 join TM in db.TreatmentRoomMasters on TA.RoomID equals TM.ID
                                                                 where TA.ScheduleDate == scheduledate && TA.StartTime <= starttime && TA.EndTime >= starttime && TA.Treatment_Status == "N" && TA.Treatment_Status == "Y"
                                                                 select TM.ID).Contains(TRM.ID)
                               select new
                               {
                                   TRM.ID,
                                   TRM.RoomName
                               }).OrderBy(i => i.RoomName).ToList();
            return Json(ROOMDETAILS, JsonRequestBehavior.AllowGet);

        }
        [ActionName("GRD2")]
        public JsonResult GETROOMDETAILS2(int PRNO)
        {

            var x = (from TM in db.TreatmentRoomMasters
                     where TM.ID == PRNO
                     select new
                     {
                         TM.ID,
                         TM.RoomName
                     }).OrderBy(i => i.RoomName).ToList();
            return Json(x, JsonRequestBehavior.AllowGet);

        }
        [ActionName("GGD")]
        public ActionResult GETGUESTDETAILS()
        {
            var GUESTDETAILS = (from RS in db.NC_TBL_ROOM_Status
                                where RS.Status == "Occupied" && RS.Date_To == null
                                select new
                                {
                                    RS.PRNO,
                                    RS.Room_No,
                                    RS.Guest_Name
                                }).ToList();
            //db.nc_tbl_room_status.ToList();
            return Json(GUESTDETAILS, JsonRequestBehavior.AllowGet);
        }
        [ActionName("PRS")]
        public JsonResult PRSCHEDULE(string PRNO, string StartTime, string EndTime)
        {
            TimeSpan starttime = TimeSpan.Parse(StartTime);
            TimeSpan endtime = TimeSpan.Parse(EndTime);
            DateTime scheduledate = DateTime.Today.AddDays(1);
            var EXISTING = db.TreatmentAvaileds.Where(i => i.PRNO == PRNO && i.Treatment_Status == "Y" && i.ScheduleDate == scheduledate && (starttime >= i.StartTime && starttime <= i.EndTime || endtime > i.StartTime && endtime < i.EndTime)).Count();
            //&& (starttime>i.StartTime  && starttime<i.EndTime) && (endtime>i.StartTime && endtime<i.EndTime)).Count();
            return Json(EXISTING, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GD")]
        public ActionResult GETGUESTDETAILS(string dated)
        {
            DateTime date = DateTime.ParseExact(dated, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var GUESTDETAILS = db.TreatmentAvaileds.Where(i => i.ScheduleDate == date && i.Treatment_Status == "Y").Include(t => t.HealerDetail).Include(t => t.Treatment_Master).Include(t => t.TreatmentRoomMaster)
                .Select(i => new
                {
                    ID = i.ID,
                    PRNO = i.PRNO,
                    GuestName = i.GuestName,
                    ScheduleDate = i.ScheduleDate,
                    ScheduleTime = i.SchdeuleTime,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    HealerName = i.HealerDetail.EMPNAME,
                    TreatmentName = i.Treatment_Master.TreatName,
                    RoomNumber = i.TreatmentRoomMaster.RoomName
                });
            return Json(GUESTDETAILS, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GETGUESTDETAILS1()
        {
            DateTime date = DateTime.Today;
            var GUESTDETAILS = db.TreatmentAvaileds.Where(i => i.ScheduleDate == date && (i.Treatment_Status == "Y" || i.Treatment_Status == "A" || i.Treatment_Status == "NA")).Include(t => t.HealerDetail).Include(t => t.Treatment_Master).Include(t => t.TreatmentRoomMaster)
                .Select(i => new
                {
                    ID = i.ID,
                    PRNO = i.PRNO,
                    GuestName = i.GuestName,
                    ScheduleDate = i.ScheduleDate,
                    ScheduleTime = i.SchdeuleTime,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    HealerName = i.HealerDetail.EMPNAME,
                    TreatmentName = i.Treatment_Master.TreatName,
                    RoomNumber = i.TreatmentRoomMaster.RoomName,
                    Status = i.Treatment_Status
                }).OrderBy(i => i.StartTime);
            return Json(GUESTDETAILS, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GETGUESTDETAILS2()
        {
            DateTime date = DateTime.Today;
            var getfeedbackdet = db.TreatmentFeedBacks.Select(i => i.TreatmentAvailedID).ToList();
            //var guestdetails=from TA in db.TreatmentAvaileds
            //                 join HD in db.HealerDetails on TA.HealerID equals HD.EMPNO
            //                 join TM in db.Treatment_Master on TA.TreatmentID equals TM.TreatmentID
            //                 join  TRM in db.TreatmentRoomMasters on TA.RoomID equals TRM.ID
            //                 where !()

            var GUESTDETAILS = db.TreatmentAvaileds.Where(i => i.ScheduleDate == date && i.Treatment_Status == "A").Include(t => t.HealerDetail).Include(t => t.Treatment_Master).Include(t => t.TreatmentRoomMaster)
                .Select(i => new
                {
                    ID = i.ID,
                    PRNO = i.PRNO,
                    GuestName = i.GuestName,
                    ScheduleDate = i.ScheduleDate,
                    ScheduleTime = i.SchdeuleTime,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime,
                    HealerName = i.HealerDetail.EMPNAME,
                    TreatmentName = i.Treatment_Master.TreatName,
                    RoomNumber = i.TreatmentRoomMaster.RoomName,
                    Status = i.Treatment_Status
                }).OrderBy(i => i.StartTime);
            var details = (from GD in GUESTDETAILS where !(getfeedbackdet).Contains(GD.ID) select GD).ToList();


            return Json(details, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create1([Bind(Include = "ID,PRNO,GuestName,HealerID,TreatmentID,RoomID,SchdeuleTime,StartTime,EndTime")] TreatmentAvailed treatmentAvailed)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        treatmentAvailed.ScheduleDate = DateTime.Today.AddDays(1);
        //        db.TreatmentAvaileds.Add(treatmentAvailed);
        //        db.SaveChanges();
        //        return RedirectToAction("TreatmentList");
        //    }

        //    ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME", treatmentAvailed.HealerID);
        //    ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatName", treatmentAvailed.TreatmentID);
        //    ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
        //    return View(treatmentAvailed);
        //}
        [HttpPost]
        public ActionResult Create1(FormCollection coll)
        {
            var ID = coll["rowcount1"];
            Session["PRNO"] = coll["PRNO"];
            Session["GuestName"] = coll["GuestName"];
            for (int j = 1; j <= Convert.ToInt32(ID); j++)
            {

                TreatmentAvailed treat = new TreatmentAvailed();
                string PRNO = "";
                try
                {
                    PRNO = Session["PRNO"].ToString();
                }
                catch
                {
                    PRNO = null;
                }
                treat.PRNO = PRNO;
                string guestname1 = "";
                try
                {
                    guestname1 = Session["GuestName"].ToString();
                }
                catch
                {
                    guestname1 = null;
                }
                treat.GuestName = guestname1;
                treat.SchdeuleTime = TimeSpan.Parse(coll["SchdeuleTime" + j].ToString());
                treat.StartTime = TimeSpan.Parse(coll["StartTime" + j].ToString());
                treat.EndTime = TimeSpan.Parse(coll["EndTime" + j].ToString());

                var empno = (coll["HealerID" + j]).ToString();
                var healerid = (from a in db.HealerDetails.Where(a => a.EMPNAME == empno) select a.EMPNO).ToList();
                treat.HealerID = Convert.ToInt32(healerid[0]);
                var treatid = coll["TreatmentID" + j].ToString();
                var treatid1 = (from a in db.Treatment_Master.Where(a => a.TreatName == treatid) select a.TreatmentID).ToList();
                treat.TreatmentID = Convert.ToInt32(treatid1[0]);

                var roomid = coll["RoomID" + j].ToString();
                var roomid1 = (from a in db.TreatmentRoomMasters.Where(a => a.RoomName == roomid) select a.ID).ToList();
                treat.RoomID = Convert.ToInt32(roomid1[0]);
                treat.Treatment_Status = "Y";
                treat.ScheduleDate = DateTime.Today.AddDays(1);
                treat.Created_by = "Admin";
                treat.Created_dt = DateTime.Now;
                db.TreatmentAvaileds.Add(treat);
                db.SaveChanges();
            }
            return RedirectToAction("Create1");
        }
        public void CancelledTreatments()
        {
            DateTime dt = DateTime.Today.AddDays(1);
            var data = (from a in db.TreatmentAvaileds
                        join b in db.TreatmentAvaileds on a.PRNO equals b.PRNO
                        where b.HealerID == a.HealerID && b.TreatmentID == a.TreatmentID && b.RoomID == a.RoomID &&
                               b.ScheduleDate == a.ScheduleDate && b.SchdeuleTime == a.SchdeuleTime && b.StartTime == a.StartTime &&
                               b.EndTime == a.EndTime && b.GuestName == a.GuestName && a.ScheduleDate==dt  && a.Treatment_Status != b.Treatment_Status
                        select a.ID).ToList();

            var list = (from a in db.TreatmentAvaileds
                        where !(data).Contains(a.ID) && a.ScheduleDate==dt select a).ToList();
            foreach(var id in list)
            {
                db.TreatmentAvaileds.Remove(db.TreatmentAvaileds.Find(id.ID));
                db.SaveChanges();
            }
        }
        public ActionResult Edittreatmentplan()
        {
            //DateTime scdt = DateTime.ParseExact( "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var scdt = DateTime.Now;
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public JsonResult Gettreatmentplanlist()
        {
            DateTime schdt = DateTime.Now.AddDays(1);
            var treatments = (from a in db.TreatmentAvaileds
                              join b in db.HealerDetails on a.HealerID equals b.EMPNO
                              join c in db.Treatment_Master on a.TreatmentID equals c.TreatmentID
                              join d in db.TreatmentRoomMasters on a.RoomID equals d.ID
                              where a.ScheduleDate >= DateTime.Today && a.Treatment_Status == "Y"
                              select new
                              {
                                  a.ID,
                                  a.PRNO,
                                  a.GuestName,
                                  a.ScheduleDate,
                                  a.SchdeuleTime,
                                  a.StartTime,
                                  a.EndTime,
                                  b.EMPNAME,
                                  c.TreatName,
                                  d.RoomName
                              }).ToList();
            return Json(treatments, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Editreatelist(string PRNO)
        {
            ViewBag.ID = PRNO;
            return View();
        }
        [HttpPost]
        public ActionResult Editreatelist(FormCollection collection)
        {
            //if (string.IsNullOrEmpty(Session["UserCode"].ToString() as string))
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            int prnumber = Convert.ToInt32(collection["ID"]);
            var status = db.TreatmentAvaileds.Where(kkl => kkl.ID == prnumber).FirstOrDefault();
            status.GuestName = collection["GuestName"].ToString();
            status.PRNO = collection["PRNO"].ToString();
            status.SchdeuleTime = TimeSpan.Parse(collection["SchdeuleTime"].ToString());
            status.StartTime = TimeSpan.Parse(collection["StartTime"].ToString());
            status.EndTime = TimeSpan.Parse(collection["EndTime"].ToString());

            var empno = (collection["HealerID"]).ToString();
            //var healerid = (from a in db.HealerDetails.Where(a => a.EMPNAME == empno) select a.EMPNO).ToList();
            status.HealerID = Convert.ToInt32(empno);
            var treatid = collection["TreatmentID"].ToString();
            //var treatid1 = (from a in db.Treatment_Master.Where(a => a.TreatName == treatid) select a.TreatmentID).ToList();
            status.TreatmentID = Convert.ToInt32(treatid);
            var roomid = collection["RoomID"].ToString();
            //var roomid1 = (from a in db.TreatmentRoomMasters.Where(a => a.RoomName == roomid) select a.ID).ToList();
            status.RoomID = Convert.ToInt32(roomid);

            db.TreatmentAvaileds.Attach(status);
            var entry = db.Entry(status);
            entry.State = EntityState.Modified;
            entry.Property(e => e.GuestName).IsModified = true;
            entry.Property(e => e.PRNO).IsModified = true;
            entry.Property(e => e.SchdeuleTime).IsModified = true;
            entry.Property(e => e.StartTime).IsModified = true;

            entry.Property(e => e.EndTime).IsModified = true;
            entry.Property(e => e.HealerID).IsModified = true;
            entry.Property(e => e.TreatmentID).IsModified = true;
            entry.Property(e => e.RoomID).IsModified = true;
            int k = db.SaveChanges();
            return Content("<script>alert('Updated Successful'); window.location = './Edittreatmentplan';</script>");
            //}
        }

        public ActionResult GeguestlistDet(int PRNO)
        {
            var items = (from a in db.TreatmentAvaileds
                         join b in db.HealerDetails on a.HealerID equals b.EMPNO
                         join c in db.Treatment_Master on a.TreatmentID equals c.TreatmentID
                         join d in db.TreatmentRoomMasters on a.RoomID equals d.ID
                         where a.ScheduleDate >= DateTime.Today && a.ID == PRNO
                         select new
                         {
                             PRNO = a.PRNO,
                             GuestName = a.GuestName,
                             ScheduleDate = a.ScheduleDate,
                             SchdeuleTime = a.SchdeuleTime,
                             StartTime = a.StartTime,
                             EndTime = a.EndTime,
                             HealerID = a.HealerID,
                             TreatmentID = a.TreatmentID,
                             RoomID = a.RoomID,
                             Treatment_Status = a.Treatment_Status,
                             ActualStartTime = a.ActualStartTime,
                             ActualEndTime = a.ActualEndTime,
                             Reason = a.Reason
                         });
            if (items.Count() > 0)
            {
                var jsonResult = Json(items, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            else
            {
                return Json(items.Count(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult treatmentattendence()
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
        public JsonResult AddTreatmentAttandence(int TreatmentAvailedID, string ActualStartTime, string ActualEndTime, string Reasion, string Treatment_Attendance)
        {
            var existing = db.TreatmentAvaileds.Where(i => i.ID == TreatmentAvailedID).FirstOrDefault();
            if (ActualStartTime != "" && ActualEndTime != "")
            {
                existing.ActualStartTime = TimeSpan.Parse(ActualStartTime);
                existing.ActualEndTime = TimeSpan.Parse(ActualEndTime);
            }
            existing.Reason = Reasion;
            existing.Treatment_Status = Treatment_Attendance;
            existing.Modified_by = "Admin";
            existing.MOdified_dt = DateTime.Now;
            var entry = db.Entry(existing);
            entry.State = EntityState.Modified;
            entry.Property(e => e.ActualStartTime).IsModified = true;
            entry.Property(e => e.ActualEndTime).IsModified = true;
            entry.Property(e => e.Reason).IsModified = true;
            entry.Property(e => e.Treatment_Status).IsModified = true;
            entry.Property(e => e.Modified_by).IsModified = true;
            entry.Property(e => e.MOdified_dt).IsModified = true;
            db.SaveChanges();
            var Redirect = "";
            var message = "Success";
            return Json(new { Value = "Success", message, Redirect }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult treatmentFeedback()
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
        public JsonResult AddtreatmentFeedback(int TreatmentAvailedID, string Status)
        {
            TreatmentFeedBack treat = new TreatmentFeedBack();
            treat.TreatmentAvailedID = TreatmentAvailedID;
            treat.Rating = Status;
            treat.Created_by = "Admin";
            treat.Created_dt = DateTime.Now;
            db.TreatmentFeedBacks.Add(treat);
            db.SaveChanges();
            var Redirect = "";
            var message = "Success";
            return Json(new { Value = "Success", message, Redirect }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}