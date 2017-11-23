using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.IO;
using System.Data.Entity.Core.Objects;

namespace Rooms.Controllers
{
    public class BaseController : Controller 
    {
        //
        // GET: /Base/
        MisRoomsEntities db = new MisRoomsEntities();
      
        public void ErrorLog(string Message,string innerexcep)
        {
            Session.Abandon();
            StreamWriter sw = new StreamWriter(Server.MapPath("~") + "\\ErrorDetails\\ErrorList.txt", true);

            string Controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");

            string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            string errorocu = "\n\n" + "Error Occured on" + DateTime.Now.ToString() + "\n";
            string design = "===========================================================" + "\n";
            string occured = "\n" + "Details-- Controller :" + Controller + "\n" + " Action name:" + action + "\n";
            sw.WriteLine(errorocu + design + occured + " Error Details :" + Message + "Inner Exception" + innerexcep);

            sw.Flush();
            sw.Close();
                       
        }

        //this is for the Guest UniqueNo Action Method
        public string getUniqueno(string ReqUniqueType)
        {
            ObjectParameter Unique_PRNo = new ObjectParameter("uniqueno", typeof(String));
            var UniqueNo = db.gen_unique_no(ReqUniqueType, Unique_PRNo);
            return (Unique_PRNo.Value.ToString());

        }

        public static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        //this is for the Guest Identification Action Method
        public string getdetails(string prno)
        {

            object[] parameters = new object[] { "prtype", typeof(String), "oppPRNO", typeof(String), "oppName", typeof(String) };
            ObjectParameter prtype = new ObjectParameter("prtype", typeof(String));
            ObjectParameter oppPRNO = new ObjectParameter("oppPRNO", typeof(String));
            ObjectParameter oppName = new ObjectParameter("oppName", typeof(String));

            var getdetails = db.Guest_PR_Identification(prno, prtype, oppPRNO, oppName);
            return (prtype.Value + "," + oppPRNO.Value + "," + oppName.Value);

        }


    }
}
