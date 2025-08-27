using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using MD.PersianDateTime;



namespace SchoolMIS.Controllers
{
    public class DefaultController : Controller
    {
        public  DateTime ConvertShamsiToGregorian(string shamsiDate)
        {
            try
            {


                PersianCalendar pc = new PersianCalendar();
                var parts = shamsiDate.Trim().Replace('-', '/').Split('/');
                if (parts.Length != 3)
                    throw new FormatException("Invalid Shamsi date format");

                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                DateTime gregorianDate = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                DateTime dattte = new DateTime(year, month, day, pc);

                // Deb     ugging output
                System.Diagnostics.Debug.WriteLine("Converted Date: " + gregorianDate.ToString());

                return gregorianDate;
            }
            catch (Exception ex)
            {
                throw new Exception("Error converting Shamsi to Gregorian: " + ex.Message);
            }

        }


        // GET: Default
        public ActionResult Index()
        {

            string d = "1402/3/4";
            DateTime date = ConvertShamsiToGregorian(d);

            //PersianDateTime dateTime = new PersianDateTime(1437, 3, 5);

            //DateTime date = dateTime.ToDateTime();
            ViewBag.Date = date;



            return View();
        }
    }
}