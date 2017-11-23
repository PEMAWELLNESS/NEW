using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Data.Entity.Core.Objects;

namespace Rooms.Controllers
{
    public class AttendanceController : Controller
    {
        private MisRoomsEntities mis = new MisRoomsEntities();
        // GET: Attendance
        public ActionResult HealerAttendance()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var emplist = mis.HealerDetails.Where(i => i.EMPSTATUS == "Active").OrderBy(i => i.EMPNAME).ToList();
                return View(emplist);
            }
        }
        [HttpPost]
        public ActionResult HealerAttendance(FormCollection collection)
        {
            int count = Convert.ToInt32(collection["count"].ToString());
            AttendanceDetail ad = new AttendanceDetail();
            List<AttendanceDetail> ads = new List<AttendanceDetail>();
            for (int i = 0; i < count; i++)
            {
                var emp = Convert.ToInt32(collection["emps " + i].ToString());
                var personal = mis.AttendanceDetails.FirstOrDefault(j => j.EmpId == emp && EntityFunctions.TruncateTime(j.AttendanceTime) == DateTime.Today);
                if (personal == null)
                {
                    ad.EmpId = Convert.ToInt32(collection["emps " + i].ToString());
                    ad.AttendanceStatusId = Convert.ToInt32(collection["attendance " + i].ToString());
                    ad.AttendanceTime = DateTime.Now;
                    ad.CreatedBy = "admin";
                    ad.CreatedOn = DateTime.Now;
                    mis.AttendanceDetails.Add(ad);
                    mis.SaveChanges();
                }
                else
                {
                   // return Content("<script>alert('Attendance Registered already For this Employee'+emp);window.location = '/Home/Attendance';</script>");
                }
                //AttendanceDetail ad = new AttendanceDetail();
                //ad.AttendanceStatusID = Convert.ToInt32(collection["attendance"].ToString());
                //ad.EmpId = Convert.ToInt32(collection["empcode"].ToString());
                //ad.AttendanceDate = DateTime.Now;
                //ad.CreatedBy ="1";
                //ad.CreatedOn = DateTime.Now;
                //db.AttendanceDetails.Add(ad);
                //int i = db.SaveChanges();
                //if(i==1)
                //{
                //    return Content("<script>alert('Attendance Registered Successfully');window.location = '/Home/Attendance';</script>");
                //}

            }
            return Content("<script>alert('Attendance Registered Successfully');window.location = 'HealerAttendance';</script>");
            
            //return RedirectToAction("HealerAttendance");
        }
        public ActionResult GetEmployees()
        {
            var emplist = mis.HealerDetails.Select(i => new { Id = i.ID, EmpCode = i.EMPNO, EmpName = i.EMPNAME }).ToList();
            return Json(emplist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAttendanceStatus()
        {
            var status = mis.AttendanceStatus.Select(i => new { Id = i.ID, Status = i.AttendanceStatus }).ToList();
            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}