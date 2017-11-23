using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Data.Entity.Core.Objects;

namespace Rooms.Controllers
{
    public class TreatmentController : Controller
    {
        private MisRoomsEntities pema = new MisRoomsEntities();
        public ActionResult TreatmentPlan()
        {
            var schedule = (from a in pema.NC_TBL_PERSONAL_DETAILS
                            join b in pema.NC_TBL_ROOM_Status on a.PRNO equals b.PRNO
                            where (b.Status == "Occupied" && b.Date_To == null)
                            select new { a, b }).ToList();
            ViewBag.schedule = schedule.ToList();
            return View();
        }
        public JsonResult traetmentlist()
        {
            var schedule = (from a in pema.NC_TBL_PERSONAL_DETAILS
                            join b in pema.NC_TBL_ROOM_Status on a.PRNO equals b.PRNO
                            where (b.Status == "Occupied" && b.Date_To == null)
                            select new { a, b }).ToList();
            ViewBag.schedule = schedule.ToList();
            return Json(schedule, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Currenttreatment()
        {
            return View();
        }
        public ActionResult GetTreatment(string roomno)
        {
            if (roomno == "null")
            {
                var x = (from TP in db.TreatmentPlans
                         join TPD in db.TreatmentPlanDetails on TP.PlanDetailID equals TPD.TPNumber
                         where TP.ScheduleDate == DateTime.Today && TP.StartTime <= EntityFunctions.CreateTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) && TP.EndTime >= EntityFunctions.CreateTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
                         select new { TP, TPD }).ToList();
                var y = (from PD in pema.NC_TBL_PERSONAL_DETAILS select new { PD }).ToList();
                var roomslist = (from a in x
                                 join b in y on a.TP.PRNO equals b.PD.PRNO
                                 select new
                                 {
                                     RoomNo = a.TPD.RoomNo
                                 }).ToList();
                return Json(roomslist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var x = (from TP in db.TreatmentPlans
                         join TPD in db.TreatmentPlanDetails on TP.PlanDetailID equals TPD.TPNumber
                         where TP.ScheduleDate == DateTime.Today && TP.StartTime <= EntityFunctions.CreateTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) && TP.EndTime >= EntityFunctions.CreateTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
                         select new { TP, TPD }).ToList();
                var y = (from PD in pema.NC_TBL_PERSONAL_DETAILS select new { PD }).ToList();
                var roomslist = (from a in x
                                 join b in y on a.TP.PRNO equals b.PD.PRNO
                                 where a.TPD.RoomNo==roomno
                                 select new
                                 {
                                     RoomNo = a.TPD.RoomNo,
                                     GuestName=b.PD.FirstName+" "+b.PD.LastName,
                                     TreatmentName=a.TPD.TreatmentName,
                                     HealerName=a.TPD.HealerName
                                 }).ToList();
                return Json(roomslist, JsonRequestBehavior.AllowGet);
            }
            //var roomnolist = (from TP in db.TreatmentPlans
            //                  join TPD in db.TreatmentPlanDetails on TP.PlanDetailID equals TPD.TPNumber
            //                  join PD in pema.NC_TBL_PERSONAL_DETAILS on TP.PRNO equals PD.PRNO
            //                  where TP.ScheduleDate == DateTime.Today && TP.StartTime >= DateTime.Now.TimeOfDay && TP.EndTime <= DateTime.Now.TimeOfDay select new
            //                  {
            //                      RoomNo = TPD.RoomNo
            //                  }).ToList();
            
        }
        [HttpPost]
        public ActionResult TreatmentPlan(FormCollection coll)
        {

            var id = db.TreatmentPlanIDs.FirstOrDefault();
            string tpdid = "TP" + (Convert.ToInt32(id.PlanDetailsID) + 1).ToString("0000");
            int rowcount = Convert.ToInt32(coll["rowcount"].ToString());
            int savedcount = 0;
            for (int i = 1; i <= rowcount; i++)
            {
                TreatmentPlanDetail tpd = new TreatmentPlanDetail();
                tpd.TPNumber = tpdid;
                tpd.TreatmentName = coll["trname" + i].ToString();
                tpd.HealerName = coll["healername" + i].ToString();
                tpd.RoomNo = coll["roomid" + i].ToString();
                db.TreatmentPlanDetails.Add(tpd);
                savedcount += db.SaveChanges();
            }
            if (savedcount > 0)
            {
                id.PlanDetailsID = Convert.ToInt32(id.PlanDetailsID) + 1;
                db.TreatmentPlanIDs.Attach(id);
                db.Entry(id).Property(i => i.PlanDetailsID).IsModified = true;
                db.SaveChanges();
                TreatmentPlan tp = new TreatmentPlan();
                tp.PRNO = coll["guestid"].ToString();
                tp.PlanDetailID = tpdid;
                tp.TreatmentSession = coll["session"].ToString();
                tp.ScheduleDate = DateTime.Today.AddDays(1);
                tp.ScheduleTime = TimeSpan.Parse(coll["scheduletime"].ToString());
                tp.StartTime = TimeSpan.Parse(coll["starttime"].ToString());
                tp.EndTime = TimeSpan.Parse(coll["endtime"].ToString());
                tp.CreatedBy = "admin";
                tp.CreatedOn = DateTime.Now;
                db.TreatmentPlans.Add(tp);
                db.SaveChanges();
            }
            return View();
        }
        public ContentResult GetGuestDetails(string prnumber)
        {
            var guestlist = pema.NC_TBL_PERSONAL_DETAILS.Where(i => i.PRNO == prnumber).FirstOrDefault();
            if(guestlist==null)
            {
                guestlist.FirstName = "0";
            }
            return Content(guestlist.FirstName);
        }
        public JsonResult GetHealerDetails()
        {
            var healername=(from DOB in db.TBL_DateofBirths
            join AD in pema.AttendanceDetails on DOB.Id equals AD.EmpId
            where EntityFunctions.TruncateTime(AD.AttendanceTime) == DateTime.Today && DOB.Dept== "Healing Hub" && AD.AttendanceStatusId==1
            select new { DOB.Emp_name,DOB.Ecno }).OrderBy(i=>i.Emp_name).ToList();
            //var healername = db.TBL_DateofBirths.Where(i=>i.Dept=="Healing Hub").Select(i=>new { i.Emp_name ,i.Ecno}).ToList();
            return Json(healername,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoomNumbers(string StartTime,string EndTime )
        {
            TimeSpan starttime = TimeSpan.Parse(StartTime);
            TimeSpan endtime = TimeSpan.Parse(EndTime);
            var roomslist = (from TRM in db.TreatmentRoomMasters
                             where !(from TPD in db.TreatmentPlanDetails
                                      join TP in db.TreatmentPlans on TPD.TPNumber equals TP.PlanDetailID
                                      where TP.StartTime >= starttime && TP.EndTime >= endtime
                                      select TPD.RoomNo).Contains(TRM.RoomName) // TRM.RoomName.Contains(occupiedroomslist)
                             select new
                             {
                                 Id = TRM.ID,
                                 RoomNumber = TRM.RoomName
                             }).OrderBy(i=>i.RoomNumber).ToList();
            return Json(roomslist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTreatmentNames()
        {
            var treatmentslist = pema.NC_TBL_Treatment_Master.Select(i => new { Id=i.Id, TreatmentName = i.Treatment_Name }).ToList();
            return Json(treatmentslist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TreatmentlistData(string prnumber)
        {
            var model = (from a in db.TreatmentPlans
                         join g in db.TreatmentPlanDetails on a.PlanDetailID equals g.TPNumber 
                         where a.PRNO == prnumber
                         select new { a,g }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}