using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;

namespace Rooms.Controllers
{
    public class TreatmentAvailedsController : Controller
    {
        private MisRoomsEntities db = new MisRoomsEntities();

        // GET: TreatmentAvaileds
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var treatmentAvaileds = db.TreatmentAvaileds.Include(t => t.HealerDetail).Include(t => t.Treatment_Master).Include(t => t.TreatmentRoomMaster);
                return View(treatmentAvaileds.ToList());
            }
        }

        // GET: TreatmentAvaileds/Details/5
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

        // GET: TreatmentAvaileds/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME");
                ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatType");
                ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName");
                return View();
            }
        }

        // POST: TreatmentAvaileds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PRNO,HealerID,TreatmentID,RoomID,ScheduleDate,SchdeuleTime,StartTime,EndTime")] TreatmentAvailed treatmentAvailed)
        {
            if (ModelState.IsValid)
            {
                db.TreatmentAvaileds.Add(treatmentAvailed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HealerID = new SelectList(db.HealerDetails, "EMPNO", "EMPNAME", treatmentAvailed.HealerID);
            ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatType", treatmentAvailed.TreatmentID);
            ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
            return View(treatmentAvailed);
        }

        // GET: TreatmentAvaileds/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatType", treatmentAvailed.TreatmentID);
            ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
            return View(treatmentAvailed);
        }

        // POST: TreatmentAvaileds/Edit/5
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
            ViewBag.TreatmentID = new SelectList(db.Treatment_Master, "TreatmentID", "TreatType", treatmentAvailed.TreatmentID);
            ViewBag.RoomID = new SelectList(db.TreatmentRoomMasters, "ID", "RoomName", treatmentAvailed.RoomID);
            return View(treatmentAvailed);
        }

        // GET: TreatmentAvaileds/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: TreatmentAvaileds/Delete/5
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
    }
}
