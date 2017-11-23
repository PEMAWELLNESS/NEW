using Rooms.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rooms.Controllers
{
    public class CheckoutController : Controller
    {
        MisRoomsEntities db = new MisRoomsEntities();
        // GET: Checkout
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
        public ActionResult GetGuestdetails(int roomno)
        {
            DateTime dt = DateTime.Today;
            //var Guestdetails = (
            //    from a in db.NC_TBL_PERSONAL_DETAILS
            //    join b in db.NC_TBL_ROOM_Status on a.PRNO equals b.PRNO
            //    where (b.Room_No ==Convert.ToInt32(prno))
            //    select new { a, b }).ToList();
            var Guestdetails = (from a in db.NC_TBL_ROOM_Status
                               // join b in db.NC_TBL_PERSONAL_DETAILS on a.PRNO equals b.PRNO
                                where (a.Room_No == roomno && a.Status == "Occupied" && a.Date_To == null)
                                select new { a }).ToList();
            return Json(Guestdetails, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase billFile, IEnumerable<HttpPostedFileBase> DischargeFile, FormCollection coll)
        {
            int index = 1;
            foreach (var file in DischargeFile)
            {
                if (file != null)
                {
                    Session["DischargeFile" + index] = ConvertToBytes(file);
                }
                index++;
            }
            var ID = coll["rowCount"];
            for (int j = 0; j <= Convert.ToInt32(ID); j++)
            {
                var guest = coll["Guest" + j];
                if (guest != null)
                {
                    TBL_CheckOut_File users = new TBL_CheckOut_File();
                    string PRNO = "";
                    try
                    {
                        PRNO = coll["guestid" + j].ToString();
                    }
                    catch
                    {
                        PRNO = null;
                    }
                    users.PRNO = PRNO;
                    string bill = "";
                    try
                    {
                        bill = coll["bill"].ToString();
                    }
                    catch
                    {
                        bill = null;
                    }
                    users.Bill = bill;
                    string Discharge = "";
                    try
                    {
                        Discharge = coll["Discharge"].ToString();
                    }
                    catch
                    {
                        Discharge = null;
                    }
                    users.Discharge = "on";
                    string CheckOut = "";
                    try
                    {
                        CheckOut = coll["CheckOut"].ToString();
                    }
                    catch
                    {
                        CheckOut = null;
                    }
                    users.CheckOut = CheckOut;
                    users.Bill_File = ConvertToBytes(billFile);

                    users.Discharge_Summery = Session["DischargeFile" + j] as byte[];
                    db.TBL_CheckOut_File.Add(users);
                    int i = db.SaveChanges();
                    if (i == 1)
                    {
                        string prnumber = coll["guestid" + j].ToString();
                        int room_no = Convert.ToInt32(coll["roomno" + j]);
                        var Room_Status = db.NC_TBL_ROOM_Status.Where(kkl => kkl.PRNO == prnumber && kkl.Room_No == room_no && kkl.Status == "Occupied").FirstOrDefault();
                        Room_Status.Date_To = DateTime.Now;
                        Room_Status.Modified_by = Session["LogedUserID"].ToString();
                        Room_Status.Modified_dt = DateTime.Now;
                        db.NC_TBL_ROOM_Status.Attach(Room_Status);
                        var entry1 = db.Entry(Room_Status);
                        entry1.State = EntityState.Modified;
                        entry1.Property(e => e.Modified_dt).IsModified = true;
                        entry1.Property(e => e.Modified_by).IsModified = true;
                        entry1.Property(e => e.Date_To).IsModified = true;
                        int kjjj = db.SaveChanges();
                        if (Convert.ToInt32(ID) == j)
                        {
                            NC_TBL_ROOM_Status status = new NC_TBL_ROOM_Status();
                            status.Room_No = Convert.ToInt32(coll["roomno" + j]);
                            status.Status = "Room Cleaning";
                            status.Created_by = Session["LogedUserID"].ToString();
                            status.Created_dt = DateTime.Now;
                            status.Date_From = DateTime.Now;
                            db.NC_TBL_ROOM_Status.Add(status);
                            db.SaveChanges();
                            return Content("<script>alert('Check Out Successful'); window.location = './Index';</script>");
                        }
                    }
                }
            }
            return View();
        }
        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        //public ActionResult RoomStatusDetails()
        //{
        //    DateTime dt = DateTime.Today;
        //    var Guestdetails = (
        //         from a in db.NC_TBL_PERSONAL_DETAILS
        //         join b in db.NC_TBL_BOOKED_DATES on a.PRNO equals b.PRNO
        //         select new { a, b }).ToList();
        //    return Json(Guestdetails, JsonRequestBehavior.AllowGet);
        //}
    }
}