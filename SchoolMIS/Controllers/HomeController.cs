using SchoolMIS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;

namespace SchoolMIS.Controllers
{
    // hkjhkjhkuhuih
    public class HomeController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        // GET: Login
    
        public ActionResult Index()
        {
           

            return View();
        }

      
        [HttpPost]
        public JsonResult Authenticate(login userTable)
        {
            // Check if user exists in the database
            var user = db.UserTables.FirstOrDefault(u => u.UserName == userTable.UserName && u.Password == userTable.Password);
            var student = db.StudentTables.FirstOrDefault(u => u.RollNO.ToString() == userTable.UserName && userTable.Password=="1234");
            if (userTable.UserName != null || userTable.Password != null)
            {
                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        FormsAuthentication.SignOut();
                       
                        // Optionally set session data
                        Session["UserId"] = user.UserID;
                        Session["Username"] = user.UserName;
                        Session["Password"] = user.Password;
                        if ((user.Image == null))
                        {
                            Session["ProfileImage"] = "/Content/img/st3.jpg";
                        }
                        else
                        {
                            Session["ProfileImage"] = user.Image.Replace("~", "");
                        }
                        Session["UserType"] = user.UserTypeTable.Type;


                        var encodedUserName = HttpUtility.UrlEncode(user.UserName);
                            var authCookie = new HttpCookie("AuthCookie", encodedUserName);
                            Response.Cookies.Add(authCookie);

                            
                        




                        return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "Home") });
                    }
                    else
                    {
                        return Json(new { success = false, message = "کوډ یا نوم غلط دی بیا کوشش وکړه" });
                    }
                }else if(student!=null)
                {
                    if (ModelState.IsValid)
                    {
                        FormsAuthentication.SignOut();
                        // Optionally set session data
                        Session["StudentId"] = student.StudentID;
                        Session["StudentName"] = student.Name;
                        Session["StudentFatherName"] = student.FatherName;
                        Session["StudentGrandFatherName"] = student.GrandFatherName;
                        Session["RollNo"] = student.RollNO;
                        Session["Gender"] = student.Gender;
                        Session["DOB"] = student.DOB;
                        Session["RegDate"] = student.RegistrationDate;
                        Session["IdentityID"] = student.IdentityID;
                        Session["Nationality"] = student.Nationality;
                        Session["NativeLanguage"] = student.NativeLanguage;
                        Session["FatherJob"] = student.FatherJob;
                        Session["Brother"] = student.Brother;
                        Session["Uncle"] = student.Uncle;
                        Session["Contact"] = student.Contact;
                        Session["C_Address"] = student.CurrentAddress;
                        Session["P_Address"] = student.PermenentAddress;
                        Session["ClassID"] = student.ClassTable.Name;
                        Session["FormalDay"] = db.StudentAttendencyTables.Where(f => f.StudentID == student.StudentID).Sum(f => f.FormalDays);
                        Session["Present"] = db.StudentAttendencyTables.Where(f => f.StudentID == student.StudentID).Sum(f => f.Present);
                        Session["Absent"] = Convert.ToInt32(Session["FormalDay"]) - Convert.ToInt32(Session["Present"]);
                        if ((student.Image == null))
                        {
                            Session["StudenteImage"] = "/Content/img/userIcon.jpg";
                        }
                        else
                        {
                            Session["StudenteImage"] = student.Image.Replace("~", "");
                        }
                        Session["StudentType"] = student.StudentType;
                    

                        return Json(new { success = true, redirectUrl = Url.Action("StudentView", "Home") });
                    }
                    else
                    {
                        return Json(new { success = false, message = "کوډ یا نوم غلط دی بیا کوشش وکړه" });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "شاګرد لپاره د نوم پر ځای اساس نمبر او کوډ 1234 دی" });
                }
            }
            else
            {
                return Json(new { success = false, message = "خانو ډکول مهم دی" });
            }
        }
       
        public ActionResult StudentView()
        {
            return View();
        }
        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Dashboard(string day,int? staffid)
        {
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f=>f.StaffTypeTable.Type=="teacher" || f.StaffTypeTable.Type=="Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ" || f.StaffTypeTable.Type == "استاد"), "StaffID", "Name");
            var today = DateTime.Now.DayOfWeek.ToString();
            var timeTable = db.ScheduleTables.Where(f => f.Day == day && f.StaffID == staffid).OrderBy(t=>t.StartTime ).ThenBy(t=>t.EndTime).ToList();


            ViewBag.AllStudent = db.StudentTables.Count();
            ViewBag.AllStaff = db.StaffTables.Count();
            decimal? feeIncom = null;
            feeIncom = db.FeeTables.Where(f =>f.PaidDate == DateTime.Now).Select(f => (decimal?)f.NetAmount ?? 0).DefaultIfEmpty().Sum();
            if (feeIncom != null)
            {
                ViewBag.TotalIncom = feeIncom;

            }
            else
            {
                ViewBag.TotalIncom = "ددی میاشتی فیسونه نشته ";

            }
            decimal? expences = null;
            expences = db.ExpencesTables.Where(f => f.Date.Month == DateTime.Now.Month).Select(f => (decimal?)f.Price ?? 0).DefaultIfEmpty().Sum();
            if (expences != null)
            {
                ViewBag.AllExpences = expences;

            }
            else
            {
                ViewBag.AllExpences = "ددی میاشتی مصارف  نشته ";

            }
            long? otherIncome = null;
            otherIncome = db.IncomeTables.Where(f =>f.PaidDate.Month == DateTime.Now.Month).Select(f => (long?)f.NetAmount ?? 0).DefaultIfEmpty().Sum();
            if (otherIncome != null)
            {
                ViewBag.otherIncome = otherIncome;

            }
            else
            {
                ViewBag.AllExpences = "ددی میاشتی عاید  نشته ";

            }


            return View(timeTable);
        }

        // GET: Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }
       
    }
}


           
  
