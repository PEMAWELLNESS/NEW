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
using System.Configuration;
using System.Data.OleDb;
using LinqToExcel;

namespace Rooms.Controllers
{
    public class HRController : Controller
    {
        MisRoomsEntities db = new MisRoomsEntities();
        string targetpath = ConfigurationManager.AppSettings["folderpath2"] + "Attendance.xlsx";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BiometricAttendance()
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
        public ActionResult BiometricAttendance(HttpPostedFileBase FileUpload)
        {
            targetpath = ConfigurationManager.AppSettings["folderpath2"]+"Attendance.xls";
            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string extension = System.IO.Path.GetExtension(FileUpload.FileName);
                    //string filename = "Attendance";
                    //string filedetail = filename + extension;
                    if (System.IO.File.Exists(targetpath))
                    {
                        System.IO.File.Delete(targetpath);
                    }
                    FileUpload.SaveAs(targetpath);
                   // return RedirectToAction("BiometricAttendance", "HR");
                }
                string pathToExcelFile = targetpath;
                var connectionString = "";
                if (targetpath.EndsWith(".xls"))
                {
                    connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                }
                else if (targetpath.EndsWith(".xlsx"))
                {
                    connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                }

                var adapter = new OleDbDataAdapter("SELECT * FROM [AttendanceLogs$]", connectionString);
                var ds = new DataSet();

                adapter.Fill(ds, "ExcelTable");

                DataTable dtable = ds.Tables["ExcelTable"];
                var excelFile = new ExcelQueryFactory(pathToExcelFile);
                var artistAlbums = from a in dtable.AsEnumerable() select a;
                List<ApplicationData> datas = new List<ApplicationData>();
                foreach (var a in artistAlbums)
                {
                    PEMASSL pema = new PEMASSL();
                    pema.Employee_Name = Convert.ToString(a["Employee Name"]);
                    pema.Company = Convert.ToString(a["Company"]);
                    pema.AttendanceDate = Convert.ToDateTime(a["AttendanceDate"]);
                    pema.Designation = Convert.ToString(a["Designation"]);
                    pema.Department = Convert.ToString(a["Department"]);
                    //return Json(a["InTime"].ToString());
                    try
                    {
                        DateTime dd = DateTime.ParseExact(a["InTime"].ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        pema.InTime = TimeSpan.Parse(dd.ToShortTimeString());
                    }
                    catch
                    {
                       // pema.InTime = TimeSpan.ParseExact(a["InTime"].ToString(), "g", CultureInfo.InvariantCulture);
                    }
                    try
                    {
                        DateTime dd = DateTime.ParseExact(a[" OutTime"].ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        pema.C_OutTime = TimeSpan.Parse(dd.ToShortTimeString());
                    }
                    catch
                    {
                        //pema.C_OutTime = TimeSpan.ParseExact(a[" OutTime"].ToString(), "g", CultureInfo.InvariantCulture);
                    }
                    //pema.C_OutTime = TimeSpan.ParseExact(a[" OutTime"].ToString(), "hh:mm", CultureInfo.InvariantCulture);
                    //DateTime OutTime = DateTime.Parse(a[" OutTime"].ToString());
                    //pema.C_OutTime = TimeSpan.Parse(OutTime.ToShortTimeString());
                    pema.Status = Convert.ToString(a["Status"]);
                    pema.PunchRecords = Convert.ToString(a["PunchRecords"]);
                    pema.Employee_Code =Convert.ToInt32(a["Employee Code"]);
                    pema.Category = Convert.ToString(a["Category"]);
                    pema.Employement_type = Convert.ToString(a["Employement type"]);
                    pema.In_Device_Name = Convert.ToString(a["In Device Name"]);
                    pema.Out_Device_Name = Convert.ToString(a["Out Device Name"]);
                    pema.StatusCode = Convert.ToString(a["StatusCode"]);
                    pema.Duration = Convert.ToInt32(a["Duration"]);
                    pema.LateBy = Convert.ToInt32(a["LateBy"]);
                    pema.Early_by = Convert.ToInt32(a["Early by"]);
                    pema.Overtime = Convert.ToInt32(a["Overtime"]);
                    pema.Is_On_Leave = Convert.ToBoolean(a["Is On Leave"]);
                    pema.LeaveType = Convert.ToString(a["LeaveType"]);
                    pema.Is_On_OutDoor_Entries = Convert.ToBoolean(a["Is On OutDoor Entries"]);
                    pema.OutDoor_Duration = Convert.ToInt32(a["OutDoor Duration"]);
                    pema.ShiftName = Convert.ToString(a["ShiftName"]);
                    pema.Created_by = "Admin";
                    pema.Created_dt = DateTime.Now;
                    db.PEMASSLs.Add(pema);
                    db.SaveChanges();
                }
                return Content("<script>alert('Data Saved Successful'); window.location = './BiometricAttendance';</script>");
            }
            else
            {
                data.Add("<ul>");
                data.Add("<li>Only Excel file format is allowed</li>");
                data.Add("</ul>");
                data.ToArray();
                return RedirectToAction("BiometricAttendance", "HR");
            }

            return View();
        }
        public FileResult DownloadExcel()
        {
            targetpath = ConfigurationManager.AppSettings["folderpath2"] + "Attendance.xls";
            return File(targetpath, "application/vnd.ms-excel", "Attendance.xls");
        }
    }
}