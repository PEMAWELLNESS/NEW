using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Configuration;

namespace Rooms.Controllers
{
    public class DinacharyaController : Controller
    {
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
        public JsonResult guestlist(int roomno)
        {
            var list = db.NC_TBL_ROOM_Status.Where(a => a.Room_No == roomno && a.Date_To == null).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index(FormCollection coll)
        {
            var ID = coll["rowcount"];
            for (int j = 1; j <= Convert.ToInt32(ID); j++)
            {
                var guest = coll["Guest" + j];
                var guestname = coll["guestname" + j];
                if (guest != null && guestname != null)
                {
                    Session["PRNO" + j] = coll["guestid" + j];
                    Session["guestname" + j] = coll["guestname" + j];
                    if (Session["PRNO" + j] != null)
                    {
                        var id1 = coll["rowcount1"];
                        for (int i = 1; i <= Convert.ToInt32(id1); i++)
                        {
                            Feast_Details feast = new Feast_Details();
                            string PRNO = "";
                            try
                            {
                                PRNO = Session["PRNO" + j].ToString();
                            }
                            catch
                            {
                                PRNO = null;
                            }
                            feast.PRNO = PRNO;
                            string guestname1 = "";
                            try
                            {
                                guestname1 = Session["guestname" + j].ToString();
                            }
                            catch
                            {
                                guestname1 = null;
                            }
                            feast.Guest_name = guestname1;
                            string feastname = "";
                            try
                            {
                                feastname = coll["itemcode" + i].ToString();
                            }
                            catch
                            {
                                feastname = null;
                            }

                            feast.Feast_Time = TimeSpan.Parse(coll["itemname" + i].ToString());
                            feast.Feast_Name = feastname;
                            feast.Created_dt = DateTime.Today;
                            db.Feast_Details.Add(feast);
                            db.SaveChanges();
                        }
                        var id2 = coll["rowcount2"];
                        for (int k = 1; k <= Convert.ToInt32(id2); k++)
                        {
                            Dinacharya_Treatment dnt = new Dinacharya_Treatment();
                            string PRNO = "";
                            try
                            {
                                PRNO = Session["PRNO" + j].ToString();
                            }
                            catch
                            {
                                PRNO = null;
                            }
                            dnt.PRNO = PRNO;
                            string guestname1 = "";
                            try
                            {
                                guestname1 = Session["guestname" + j].ToString();
                            }
                            catch
                            {
                                guestname1 = null;
                            }
                            dnt.Guest_name = guestname1;
                            dnt.From_Time = TimeSpan.Parse(coll["from" + k].ToString());
                            dnt.To_Time = TimeSpan.Parse(coll["to" + k].ToString());
                            string Treatment = "";
                            try
                            {
                                Treatment = coll["Treatment" + k].ToString();
                            }
                            catch
                            {
                                Treatment = null;
                            }
                            dnt.Treatment_Name = Treatment;
                            dnt.Created_dt = DateTime.Today;
                            db.Dinacharya_Treatment.Add(dnt);
                            db.SaveChanges();
                        }
                        var gt = Session["PRNO" + j].ToString();
                        DateTime date = DateTime.Today;
                        var list = db.Feast_Details.Where(a => a.PRNO == gt && a.Created_dt == date).ToList();
                        var list1 = db.Dinacharya_Treatment.Where(a => a.PRNO == gt && a.Created_dt == date).ToList();

                        //string mheader = " <div style='border:groove;background-color:aliceblue;'><div style='font-weight:bold;text-align:center;width:100%;'><center><a href='http://pemawellness.co/Index/index'><img src='http://pemawellness.co/images/logo1.png'></a></center></div> <div><center><p style='font-size:30px;'><b>Dinacharya</b></p></center></div>";
                        string mfooter = "<div><div><span>*<span>Your arrival on time for the treatment will be appreciated and will not cause anyinconvenience for the following guests.</p></div></div>";
                        //StringBuilder mbody;
                        //string smtpAddress = "smtp.gmail.com";
                        //int portNumber = 587;
                        //bool enableSSL = true;
                        //string emailFrom = "itpemawellness@gmail.com";
                        //string password = "wellness@123";
                        //string emailTo1 = "b.vijay418@gmail.com";
                        //string emailTo2 = "vijayeshwar@gmail.com";
                        //string subject = "Dinacharya";
                        StringBuilder mbody = new StringBuilder();

                        //mbody.Append(mheader);
                        mbody.Append("<div style='background-color: aliceblue;'>");
                        mbody.Append("<div>");
                        mbody.Append("<img src='http://pemawellness.co/images/logo1.png'>");
                        mbody.Append("</div>");
                        mbody.Append("<div><center><p style='font-size:30px;'><b>Dinacharya</b></p></center></div>");
                        mbody.Append("<br>");
                        //mbody.Append("<div></div>");
                        //mbody.Append("<div><table style='font-size:15px;'><tr><td>PRNO :</td><td>" + list[0].PRNO+ "&nbsp;&nbsp;&nbsp;&nbsp;</td><td>Name :</td><td>" + list[0].Guest_name + "&nbsp;&nbsp;&nbsp;&nbsp;</td><td>Date :</td><td>" + DateTime.Now + "</td></tr></table></div>");
                        mbody.Append("<div><table style='font-size:15px;'><tr><td style='width:30%;'>PRNO :</td><td>" + list[0].PRNO + "&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");
                        mbody.Append("<tr><td style='width:30%;'>Name  :</td><td>" + list[0].Guest_name + "&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>");
                        mbody.Append("<tr><td style='width:30%;'>Date   :</td><td>" + DateTime.Now + "</td></tr>");
                        mbody.Append("</table></div>");
                        //mbody.Append("<hr>");
                        mbody.Append("<br>");
                        //mbody.Append("<hr>");
                        mbody.Append("<div style='font-size: 18px;'><b style:color:black;>FEAST PLAN </b></div>");
                        mbody.Append("<br>");
                        mbody.Append("<div style='font-size: 15px;'><table style='width:100%;color: black;'>");
                        mbody.Append("<thead><tr><th style='width:25%;text-align: -webkit-left;'>Timings</th><th style='width:30%;text-align: -webkit-left;'>Feast Name</th></tr></thead>");
                        for (int i = 0; i < list.Count(); i++)
                        {
                            mbody.Append("<tbody><tr><td style='width:25%'>" + DateTime.ParseExact(list[i].Feast_Time.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture).ToShortTimeString() + "</td><td style='width:30%'>" + '-' + "&nbsp;&nbsp;&nbsp;&nbsp;" + list[i].Feast_Name + "</td></tr></tbody>");
                        }
                        mbody.Append("</table></div>");
                        mbody.Append("<br>");
                        mbody.Append("<br>");
                        mbody.Append("<div style='font-size: 18px;'><b style:color:black;>HEALING PLAN FOR THE DAY</b></div>");
                        mbody.Append("<br>");
                        mbody.Append("<div style='font-size: 15px;'><table style='width:100%;color: black;'>");
                        mbody.Append("<thead><tr><th style='width:25%;text-align: -webkit-left;'>From-To</th><th style='width:30%;text-align: -webkit-left;'>Treatment Name</th></tr></thead>");
                        for (int h = 0; h < list1.Count(); h++)
                        {
                            mbody.Append("<tbody><tr><td style='width:25%'>" + DateTime.ParseExact(list1[h].From_Time.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture).ToShortTimeString() + "-" + DateTime.ParseExact(list1[h].To_Time.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture).ToShortTimeString() + "</td><td style='width:30%'>" + '-' + "&nbsp;&nbsp;&nbsp;&nbsp;" + list1[h].Treatment_Name + "</td></tr></tbody>");

                        }
                        mbody.Append("</table></div>");
                        mbody.Append("<br>");
                        mbody.Append("<br>");
                        mbody.Append(mfooter);
                        mbody.Append("<br>");
                        mbody.Append("<div><p>Thank You</p></div></div>");
                        StringReader sr = new StringReader(mbody.ToString());

                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            pdfDoc.Open();
                            htmlparser.Parse(sr);
                            pdfDoc.Close();

                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();
                            string fromEmail = "itpemawellness@gmail.com";
                            MailMessage mm = new MailMessage();
                            //string emailFrom = "itpemawellness@gmail.com";
                            //string password = "wellness@123";
                            string emailto1 = "b.vijay418@gmail.com";
                            //string emailto2 = "vijayeshwar@gmail.com";
                            mm.To.Add(emailto1);
                            // mm.To.Add(emailto2);
                            mm.From = new MailAddress(fromEmail);
                            mm.Subject = "Dinacharya";
                            mm.Body = "Thanks for your time, Please find the attached Dinacharya";
                            mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "Dinacharya.pdf"));
                            mm.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential NetworkCred = new NetworkCredential();
                            NetworkCred.UserName = "itpemawellness@gmail.com";
                            NetworkCred.Password = "wellness@123";
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.Send(mm);
                            string path = ConfigurationManager.AppSettings["folderpath1"];
                            //string path = Server.MapPath("DinacharyaReportsForK");
                            string path1 = list[0].PRNO;
                            string newpath = path + path1;
                            bool IsExists = System.IO.Directory.Exists(Server.MapPath(path1));
                            if (!IsExists)
                                System.IO.Directory.CreateDirectory(newpath);
                            var newpt = newpath + "\\" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "Dinacharya.pdf";
                            System.IO.File.WriteAllBytes(newpath + "\\" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "Dinacharya.pdf", bytes);

                            //string targetpath1 = ConfigurationManager.AppSettings["folderpath1"];
                            //string path = Server.MapPath("DinacharyaReportsForK");
                            //string path1 = list[0].PRNO + "\\Dinacharya.pdf";
                            //
                            //System.IO.File.WriteAllBytes(newpath, bytes);

                            //System.IO.Directory.CreateDirectory(newpath);
                        }
                        //using (MailMessage mail = new MailMessage())
                        //{
                        //    mail.From = new MailAddress(emailFrom);
                        //    mail.To.Add(emailTo1);
                        //    mail.To.Add(emailTo2);
                        //    //mail.To.Add(emailTo3);
                        //    mail.Subject = subject;
                        //    mail.Body = mbody.ToString();
                        //    mail.IsBodyHtml = true;
                        //    // Can set to false, if you are sending pure text.
                        //    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                        //    {
                        //        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        //        smtp.EnableSsl = enableSSL;
                        //        smtp.Send(mail);
                        //    }
                        //}
                        return Content("<script>alert('Request Submitted Successfully');window.location = '/Dinacharya/Index'</script>");

                    }
                }

            }

            return View();
        }
        //public void test()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<header class='clearfix'>");
        //    sb.Append("<h1>INVOICE</h1>");
        //    sb.Append("<div id='company' class='clearfix'>");
        //    sb.Append("<div>Company Name</div>");
        //    //sb.Append("<div style='border:groove; border-width:medium; width:80%;background-color: aliceblue;'><div style='font-weight:bold;'><center><p style='width:300px; margin:0px 0px 0px 0px; padding:0px 0px 0px 0px'></p></center></div><div><center><p style='font-size:30px;'><b>Dinacharya</b></p></center></div><hr /><br /><div><table style='font-size:15px;'><tr><td>PRNO :</td><td>P090029&nbsp;&nbsp;&nbsp;&nbsp;</td><td>Name :</td><td>Mrs. HIMA PINAKIN &nbsp;&nbsp;&nbsp;&nbsp;</td><td>Date :</td><td>28-10-2017 11:38:18</td></tr></table></div><hr /><br /><div style='font-size: 18px;'><b style:color:black;>FEAST PLAN </b></div><br /><div style='font-size: 15px;'><table style='width:100%;color: black;'><thead><tr><th style='width:25%;text-align: -webkit-left;'>Timings</th><th style='width:30%;text-align: -webkit-left;'>Feast Name</th></tr></thead><tbody><tr><td style='width:25%'>11:35</td><td style='width:30%'>-&nbsp;&nbsp;&nbsp;&nbsp;dbvxvb</td></tr><tr><td style='width:25%'>14:35</td><td style='width:30%'>-&nbsp;&nbsp;&nbsp;&nbsp;hzghvxchgzxchgshg</td></tr><tr><td style='width:25%'>15:35</td><td style='width:30%'>-&nbsp;&nbsp;&nbsp;&nbsp;nxfxhfhj</td></tr><tr><td style='width:25%'>16:40</td><td style='width:30%'>-&nbsp;&nbsp;&nbsp;&nbsp;sjgdfhfh</td></tr></tbody></table></div><br /><br /><div style='font-size: 18px;'><b style:color:black;>HEALING PLAN FOR OF THE DAY</b></div><br /><div style='font-size: 15px;'><table style='width:100%;color: black;'><thead><tr><th style='width:25%;text-align: -webkit-left;'>From-To</th><th style='width:30%;text-align: -webkit-left;'>Treatment Name</th></tr></thead><tbody><tr><td style='width:25%'>08:40-09:40</td><td style='width:30%'>-&nbsp;&nbsp;&nbsp;&nbsp;asgfgsd</td></tr></tbody></table></div><br /><br /><div><span>*</span><p>Your arrival on time for the treatment will be appreciated and will not cause anyinconvenience for the following guests.</p></div><br /><div><p>Thank You</p></div></div>");
        //    sb.Append("<div>455 John Tower,<br /> AZ 85004, US</div>");
        //    sb.Append("<div>(602) 519-0450</div>");
        //    sb.Append("<div><a href='mailto:company@example.com'>company@example.com</a></div>");
        //    sb.Append("</div>");
        //    sb.Append("<div id='project'>");
        //    sb.Append("<div><span>PROJECT</span> Website development</div>");
        //    sb.Append("<div><span>CLIENT</span> John Doe</div>");
        //    sb.Append("<div><span>ADDRESS</span> 796 Silver Harbour, TX 79273, US</div>");
        //    sb.Append("<div><span>EMAIL</span> <a href='mailto:john@example.com'>john@example.com</a></div>");
        //    sb.Append("<div><span>DATE</span> April 13, 2016</div>");
        //    sb.Append("<div><span>DUE DATE</span> May 13, 2016</div>");
        //    sb.Append("</div>");
        //    sb.Append("</header>");
        //    sb.Append("<main>");
        //    sb.Append("<table>");
        //    sb.Append("<thead>");
        //    sb.Append("<tr>");
        //    sb.Append("<th class='service'>SERVICE</th>");
        //    sb.Append("<th class='desc'>DESCRIPTION</th>");
        //    sb.Append("<th>PRICE</th>");
        //    sb.Append("<th>QTY</th>");
        //    sb.Append("<th>TOTAL</th>");
        //    sb.Append("</tr>");
        //    sb.Append("</thead>");
        //    sb.Append("<tbody>");
        //    sb.Append("<tr>");
        //    sb.Append("<td class='service'>Design</td>");
        //    sb.Append("<td class='desc'>Creating a recognizable design solution based on the company's existing visual identity</td>");
        //    sb.Append("<td class='unit'>$400.00</td>");
        //    sb.Append("<td class='qty'>2</td>");
        //    sb.Append("<td class='total'>$800.00</td>");
        //    sb.Append("</tr>");
        //    sb.Append("<tr>");
        //    sb.Append("<td colspan='4'>SUBTOTAL</td>");
        //    sb.Append("<td class='total'>$800.00</td>");
        //    sb.Append("</tr>");
        //    sb.Append("<tr>");
        //    sb.Append("<td colspan='4'>TAX 25%</td>");
        //    sb.Append("<td class='total'>$200.00</td>");
        //    sb.Append("</tr>");
        //    sb.Append("<tr>");
        //    sb.Append("<td colspan='4' class='grand total'>GRAND TOTAL</td>");
        //    sb.Append("<td class='grand total'>$1,000.00</td>");
        //    sb.Append("</tr>");
        //    sb.Append("</tbody>");
        //    sb.Append("</table>");
        //    sb.Append("<div id='notices'>");
        //    sb.Append("<div>NOTICE:</div>");
        //    sb.Append("<div class='notice'>A finance charge of 1.5% will be made on unpaid balances after 30 days.</div>");
        //    sb.Append("</div>");
        //    sb.Append("</main>");
        //    sb.Append("<footer>");
        //    sb.Append("Invoice was created on a computer and is valid without the signature and seal.");
        //    sb.Append("</footer>");
        //    StringReader sr = new StringReader(sb.ToString());

        //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        pdfDoc.Open();

        //        htmlparser.Parse(sr);
        //        pdfDoc.Close();

        //        byte[] bytes = memoryStream.ToArray();
        //        memoryStream.Close();
        //        string fromEmail = "pavan.virgin@gmail.com";
        //        MailMessage mm = new MailMessage();
        //        mm.To.Add("pavan.virgin@gmail.com");
        //        mm.From = new MailAddress(fromEmail);
        //        mm.Subject = "Online Request";
        //        mm.Body = "Thanks for your time, Please find the attached invoice";
        //        mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "Invoice.pdf"));
        //        mm.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;
        //        NetworkCredential NetworkCred = new NetworkCredential();
        //        NetworkCred.UserName = "pavan.virgin@gmail.com";
        //        NetworkCred.Password = "P@van5185";
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = NetworkCred;
        //        smtp.Port = 587;
        //        smtp.Send(mm);
        //    }
        //}
    }
}