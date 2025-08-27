using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using MD.PersianDateTime;
using SchoolMIS.Models;


namespace SchoolMIS.Controllers
{
   
    public class StudentTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Index()
        {
            var studentTables = db.StudentTables.Include(s => s.ClassTable);

           

            return View(studentTables.ToList());
        }

        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]

        [HttpPost]
        public JsonResult Details(int? id)
        {

            var studentTable = db.StudentTables.Find(id);

            var ClassID = db.ClassTables.Find(studentTable.ClassID);
         
            var Image = "";
            if (studentTable.Image == null)
            {
                Image = "/Content/img/UserIcon.png";
            }
            else
            {
                Image = studentTable.Image.Substring(1);
            }

            var classID = ClassID.Name.ToString();
            string RegistrationDate = Converssion.ToShamsi(studentTable.RegistrationDate);
            string DOB = Converssion.ToShamsi(studentTable.DOB);

            if (studentTable != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        studentTable.Name,
                        studentTable.FatherName,
                        studentTable.GrandFatherName,
                        Image,
                        studentTable.IdentityID,
                        studentTable.Contact,
                        studentTable.CurrentAddress,
                        studentTable.PermenentAddress,
                       
                        studentTable.Gender,
                        DOB,
                        classID,
                        studentTable.Nationality,
                       RegistrationDate,
                        studentTable.FatherJob,
                        studentTable.NativeLanguage,
                        studentTable.Brother,
                        studentTable.Uncle,
                        studentTable.StudentType,
                        studentTable.RollNO,
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
           
            return View();
        }
        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentTable studentTable, HttpPostedFileBase Image,string RegistrationDate,string DOB)
        {
            bool Exist = false;
            bool EditExist = false;
            if (studentTable.StudentID == 0)
            {
                 Exist = db.StudentTables.Any(e => e.IdentityID == studentTable.IdentityID
                || e.RollNO == studentTable.RollNO);
            }
            else
            {
               EditExist = db.StudentTables.Any(e => e.IdentityID == studentTable.IdentityID
                                 && e.RollNO == studentTable.RollNO
                                 && e.Name == studentTable.Name && e.GrandFatherName == studentTable.GrandFatherName
                                 && e.FatherName == studentTable.FatherName
                                 && e.Contact == studentTable.Contact
                                 && e.Image == studentTable.Image
                                 && e.DOB == studentTable.DOB
                                 && e.RegistrationDate == studentTable.RegistrationDate
                                 && e.ClassID == studentTable.ClassID
                               );
            }


            string DOBInEnglish = Converssion.ConvertEasternArabicToWestern(DOB);
            string RegInEnglish = Converssion.ConvertEasternArabicToWestern(RegistrationDate);
            DateTime dob = Converssion.ConvertShamsiToGregorian(DOBInEnglish);
            DateTime date = Converssion.ConvertShamsiToGregorian(RegInEnglish);

            studentTable.RegistrationDate = date;
            studentTable.DOB = dob;
            var msg = "";
            DateTime today = DateTime.Today;
            int age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age))
            {
                age--;
            }
           
            //if (ModelState.IsValid)
            //{

            if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StudentTables"), JsonRequestBehavior.AllowGet });
                }
                if (EditExist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StudentTables"), JsonRequestBehavior.AllowGet });
                }
            if (age <= 5)
            {
                msg = " عمر باید د پنځو کالو څخه کم نه وی";
                return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StudentTables"), JsonRequestBehavior.AllowGet });
            }
            if (studentTable.StudentID == 0)
                {
                    var amount = db.ClassTables.Find(studentTable.ClassID);
                    amount.StudentAmount += 1;
                    amount.PresentStudent += 1;

                    if (Image != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/img/"), Image.FileName);
                        Image.SaveAs(path);
                        studentTable.Image = "~/Content/img/" + Image.FileName;
                    }


                    db.StudentTables.Add(studentTable);
                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StudentTables"), JsonRequestBehavior.AllowGet });

                }
                else
                {

                    if (Image != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/img/"), Image.FileName);
                        Image.SaveAs(path);
                        studentTable.Image = "~/Content/img/" + Image.FileName;
                    }
                    db.Entry(studentTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StudentTables"), JsonRequestBehavior.AllowGet });


                }


            //}
            //msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            //return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StudentTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentTable studentTable = db.StudentTables.Find(id);

            if (studentTable.Image == null)
            {
                Session["Image"] = "/Content/img/UserIcon.png";
            }
            else
            {
                Session["Image"] = studentTable.Image.ToString();
            }

            if (studentTable == null)
            {
                return HttpNotFound();
            }
            string Date = Converssion.ToShamsi(studentTable.RegistrationDate);
            string RegDateiraniFormate = Converssion.ToPersianDateString(Date);
            string DOB = Converssion.ToShamsi(studentTable.DOB);
            string DOBiraniFormate = Converssion.ToPersianDateString(DOB);
            var studentEditModel = new StudentEditModel
            {
                StudentID = studentTable.StudentID,
                Name = studentTable.Name,
                FatherName = studentTable.FatherName,
                GrandFatherName = studentTable.GrandFatherName,
                Contact = studentTable.Contact,
                DOB = DOBiraniFormate,
                ClassID = studentTable.ClassID,
                RegistrationDate = RegDateiraniFormate,
                CurrentAddress = studentTable.CurrentAddress,
                PermenentAddress = studentTable.PermenentAddress,
                Brother = studentTable.Brother,
                Uncle = studentTable.Uncle,
                RollNO = studentTable.RollNO,
                StudentType = studentTable.StudentType,
                NativeLanguage = studentTable.NativeLanguage,
                FatherJob = studentTable.FatherJob,
                Nationality = studentTable.Nationality,
                IdentityID = studentTable.IdentityID,
                Gender = studentTable.Gender,
                Image = studentTable.Image,
            };

            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name", studentTable.ClassID);
         
            return View(studentEditModel);
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var Image = "";
            var msg = "";
            if (id != null)
            {
                StudentTable studentTable = db.StudentTables.Find(id);

             
                var classID = db.ClassTables.Find(studentTable.ClassID).Name.ToString();
            
                if (studentTable.Image == null)
                {
                    Image = "/Content/img/UserIcon.png";
                }
                else
                {
                    Image = studentTable.Image.Substring(1);
                }

                if (studentTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }
                string RegistrationDate = Converssion.ToShamsi(studentTable.RegistrationDate);
                string DOB = Converssion.ToShamsi(studentTable.DOB);
                return Json(new
                {
                    success = true,

                    data = new
                    {
                        studentTable.StudentID,
                        studentTable.Name,
                        studentTable.FatherName,
                        studentTable.GrandFatherName,
                        Image,
                        studentTable.IdentityID,
                        studentTable.Contact,
                        studentTable.CurrentAddress,
                        studentTable.PermenentAddress,

                        studentTable.Gender,
                        DOB,
                        classID,
                        studentTable.Nationality,
                       RegistrationDate,
                        studentTable.FatherJob,
                        studentTable.NativeLanguage,
                        studentTable.Brother,
                        studentTable.Uncle,
                        studentTable.StudentType,
                        studentTable.RollNO,

                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.StudentTables.Find(id);
            db.StudentTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لری شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StudentTables"), JsonRequestBehavior.AllowGet });
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult allStudentReports()
        {
            var student=db.StudentTables.ToList();
           ViewBag.currentDate=DateTime.Now.ToLongDateString();
           

            return View(student);
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult StudentsByClassReport(int? classID)
        {

            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            if (classID != null)
            {
                ViewBag.ClassName = db.ClassTables.Find(classID).Name;
            }
            
            if (classID != null)
            {
                ViewBag.currentDate = new PersianDateTime(DateTime.Now);

                return View(db.StudentTables.Where(f => f.ClassID == classID).ToList());

            }
            else
            {
                ViewBag.currentDate = new PersianDateTime(DateTime.Now);

                return View(db.StudentTables.ToList());
            }
           
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