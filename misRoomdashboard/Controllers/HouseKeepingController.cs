using Rooms.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rooms.Views
{
    public class HouseKeepingController : Controller
    {
        MisRoomsEntities db = new MisRoomsEntities();
        // GET: HouseKeeping
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
        public ActionResult Index(FormCollection coll)
        {
            int room_no = Convert.ToInt32(coll["roomno"]);
            var Room_Status = db.NC_TBL_ROOM_Status.Where(kkl =>kkl.Room_No == room_no && kkl.Date_To ==null).FirstOrDefault();
            Room_Status.Date_To = DateTime.Now;
            Room_Status.Modified_by = Session["LogedUserID"].ToString();
            Room_Status.Modified_dt = DateTime.Now;
            db.NC_TBL_ROOM_Status.Attach(Room_Status);
            var entry1 = db.Entry(Room_Status);
            entry1.State = EntityState.Modified;
            entry1.Property(e => e.Date_To).IsModified = true;
            entry1.Property(e => e.Modified_dt).IsModified = true;
            entry1.Property(e => e.Modified_by).IsModified = true;
            int kjjj = db.SaveChanges();
            if (kjjj ==1)
            {
                NC_TBL_ROOM_Status status = new NC_TBL_ROOM_Status();
                status.Room_No = Convert.ToInt32(coll["roomno"]);
                status.Status = coll["Status"].ToString();
                status.Date_From = DateTime.Now;
                Room_Status.Created_by = Session["LogedUserID"].ToString();
                Room_Status.Created_dt = DateTime.Now;
                db.NC_TBL_ROOM_Status.Add(status);
                db.SaveChanges();
                return Content("<script>alert('Status changed Successful'); window.location = './Index';</script>");
            }
            else
            {
                return Content("<script>alert('Not Successful'); window.location = './Index';</script>");
            }
        }
        public ActionResult Getroomcleaningcount()
        {
            DateTime dt = DateTime.Today;
            var model = (from a in db.NC_TBL_ROOM_Status
                         where (a.Date_To == null && a.Status == "Room Cleaning")
                         group a by new { a.Room_No } into d
                         select new { Room_No = d.Key.Room_No, Count = d.Count() }).ToList();
            var count = (from b in model.Where(b => b.Count == 1) select b).Count();
            return Content(count.ToString());
        }
    }
}