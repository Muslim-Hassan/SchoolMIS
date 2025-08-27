using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SchoolMIS.Models;
using MD.PersianDateTime;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace SchoolMIS.Controllers
{
  
    public class ExamTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        // GET: UserTables

        [Authorize(Roles = "admin,Admin,teacher,Teacher,استاذ,استاد,اډمین,ادمین")]
        public ActionResult Index()
        {
            var examTables = db.ExamTables.Include(e => e.ClassTable).Include(e => e.StaffTable).Include(e => e.SubjectTable).Include(e => e.StudentTable);
            return View(examTables.ToList());
        }

        // GET: UserTables/Details/5
        [Authorize(Roles = "admin,Admin,teacher,Teacher,استاذ,استاد,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Details(int? id)
        {

            var examTable = db.ExamTables.Find(id);

            var staff = db.StaffTables.Find(examTable.StaffID).Name.ToString();
            var subject = db.SubjectTables.Find(examTable.SubjectID).Name.ToString();
            var student = db.StudentTables.Find(examTable.StudentID).Name.ToString();
            var className = db.ClassTables.Find(examTable.ClassID).Name.ToString();


            string date = Converssion.ToShamsi(examTable.Date);

            if (examTable != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        examTable.ExamID,
                        examTable.ExamType,
                        examTable.TotalScore,
                        examTable.AbtainScore,
                        examTable.writtenTest,
                        examTable.verbalTest,
                        examTable.ClassWork,
                        examTable.homeWork,
                        date,
                        className,
                        subject,
                        student,
                        staff,
                       
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ClassName(int? id)
        {

            var ClassName = db.StudentTables.Where(f => f.StudentID == id).Select(s => new {
                Text = s.ClassTable.Name,
                Value = s.ClassID.ToString()
            }).Distinct().ToList();
            return Json(new
            {
                success = true,
                data = new
                {
                    ClassName

                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult subjects(int? id)
        {

            var subject = db.ScheduleTables.Where(f => f.StaffID == id).Select(s => new {
                Text = s.SubjectTable.Name,
                Value = s.SubjectID.ToString()
            }).Distinct().ToList();
            return Json(new
            {
                success = true,
                data = new
                {
                    subject

                }
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,Admin,teacher,Teacher,استاذ,استاد,اډمین,ادمین")]
        public ActionResult Create()
        {
            var student = db.StudentTables.ToList();

            var mahroomStudent = new List<StudentTable>();
            var notMahroomStudent = new List<StudentTable>();
            foreach (var std in student)
            {
                var attendency = db.StudentAttendencyTables.Where(f => f.StudentID == std.StudentID).ToList();

                if (attendency == null || attendency.Count == 0)
                {
                    continue;
                }
                else
                {
                    var totalFormalDay = attendency.Sum(f => f.FormalDays);
                    var totalPresentDay = attendency.Sum(f => f.Present);
                    var totalAbsentDay = totalFormalDay - totalPresentDay;
                    double status = 0.0;
                    if (totalFormalDay!=0)
                    {
                         status = (double)(((double?)totalAbsentDay * 100) / totalFormalDay);
                    }
                   
                  

                    if (status >= 25)
                    {
                        mahroomStudent.Add(std);
                       
                        
                    }
                    else
                    {
                       
                        notMahroomStudent.Add(std);
                    }
                }
            }

            //  db.StaffTables.Where(f => f.Name == teacherName && f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.StaffTypeTable.Type == "استاذ")
            //ViewBag.Subject = db.ScheduleTables.Where(f => f.StaffTable.Name == Session["Username"]).FirstOrDefault().SubjectTable.Name;
            string teacherName= @Session["Username"]?.ToString();
           string image = @Session["ProfileImage"]?.ToString();
            var teacherList = db.StaffTables.Where(f => f.Name == teacherName && f.image == image && f.StaffTypeTable.Type == "teacher" 
            || f.StaffTypeTable.Type=="Teacher"
            || f.StaffTypeTable.Type=="استاذ"
            || f.StaffTypeTable.Type == "admin"
             || f.StaffTypeTable.Type == "اډمین"
            || f.StaffTypeTable.Type=="Admin").ToList();
                ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
                ViewBag.StaffID = new SelectList(teacherList, "StaffID", "Name");
                ViewBag.StudentID = new SelectList(notMahroomStudent, "StudentID", "Name");
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name");
            ViewBag.mahroomStudent = mahroomStudent;
            return View();
        }
        [Authorize(Roles = "admin,Admin,teacher,Teacher,,اډمین,ادمین,استاذ,استاد")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExamTable examTable , string date)
        {
            bool Exist = db.ExamTables.Any(e => e.ExamType == examTable.ExamType
            && e.SubjectID == examTable.SubjectID
            && e.ClassID == examTable.ClassID
            && e.StudentID == examTable.StudentID);

            var msg = "";

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(date);
            DateTime Date = Converssion.ConvertShamsiToGregorian(DateInEnglish);
            examTable.Date = Date;
            //if (ModelState.IsValid)
            //{
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ExamTables"), JsonRequestBehavior.AllowGet });
                }

                //Maraks validation
                if (examTable.ExamType == "څلورنیمه")
                {
                    if (examTable.TotalScore == 40 && examTable.AbtainScore > 40 )
                    {
                        msg = " نمری باید د (۴۰) څخه پورته نه وی ";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ExamTables"), JsonRequestBehavior.AllowGet });
                    }

                }
                else if(examTable.ExamType == "کلنۍ")
                {

                    if (examTable.TotalScore == 60 && examTable.AbtainScore > 60 )
                    {
                        msg = " نمری باید د (۶۰) څخه پورته نه وی ";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ExamTables"), JsonRequestBehavior.AllowGet });
                    }

                }
                else{
                    if (examTable.TotalScore == 20 && examTable.AbtainScore > 20 )
                    {
                        msg = " نمری باید د (۲۰) څخه پورته نه وی ";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ExamTables"), JsonRequestBehavior.AllowGet });
                    }
                }
                //null data validation
                if (examTable.ExamID == 0)
                {
                 
                
                    db.ExamTables.Add(examTable);
                  
                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ExamTables"), JsonRequestBehavior.AllowGet });

                }
                else
                {
                  
                    db.Entry(examTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ExamTables"), JsonRequestBehavior.AllowGet });


                }


           // }
            //msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ExamTables"), JsonRequestBehavior.AllowGet });
        }
       
        [Authorize(Roles = "admin,Admin,teacher,Teacher,استاذ,استاد,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamTable examTable = db.ExamTables.Find(id);

            if (examTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name", examTable.StaffID);

            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name", examTable.StudentID);
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name", examTable.SubjectID);
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name", examTable.ClassID);
            DateTime date = examTable.Date;
            string toShamsi = Converssion.ToShamsi(date);
            ViewBag.Date = toShamsi;
            return View(examTable);
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                ExamTable examTable = db.ExamTables.Find(id);

                var staff = db.StaffTables.Find(examTable.StaffID).Name.ToString();
                var subject = db.SubjectTables.Find(examTable.SubjectID).Name.ToString();
                var student = db.StudentTables.Find(examTable.StudentID).Name.ToString();
                var className = db.ClassTables.Find(examTable.ClassID).Name.ToString();

            
                var date = String.Format("{0:MM/dd/yyyy}", examTable.Date);

                if (examTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            examTable.ExamID,
                            examTable.ExamType,
                            examTable.TotalScore,
                            examTable.AbtainScore,
                            examTable.writtenTest,
                            examTable.verbalTest,
                            examTable.ClassWork,
                            examTable.homeWork,
                            date,
                            className,
                            subject,
                            student,
                            staff,

                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.ExamTables.Find(id);
            db.ExamTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ExamTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult examPaper()
        {
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name");
            return View();
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public ActionResult examPaper(int? classID, int? subjectID,string examType)
        {
            var studentMarks = db.ExamTables.Where(f => f.ClassID == classID && f.SubjectID == subjectID && f.ExamType == examType).ToList();
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name");
            if (studentMarks != null)
            {
                ViewBag.Teacher = studentMarks.Where(f=>f.ClassID==classID && f.SubjectID==subjectID && f.ExamType==examType).FirstOrDefault().ClassTable.StaffTable.Name;
                ViewBag.Staff = studentMarks.FirstOrDefault().StaffTable.Name;
                ViewBag.Class = studentMarks.FirstOrDefault().ClassTable.Name;
                ViewBag.Exam = studentMarks.FirstOrDefault().ExamType;
                ViewBag.Date = studentMarks.FirstOrDefault().Date;
                ViewBag.Subject = studentMarks.FirstOrDefault().SubjectTable.Name;
                 var std= db.StudentTables.Count(f => f.ClassID == classID);
                ViewBag.ClassStudentAmount = std;
                DateTime Now= DateTime.Now;
                var includeExam = db.ExamTables.Count(f => f.ClassID == classID && f.ExamType == examType && f.Date == DateTime.Now);
                ViewBag.includeExam = includeExam;
                if (examType == "څلورنیمه")
                {
                    var success1 = db.ExamTables.Count(f => f.ClassID == classID && f.ExamType == examType && f.Date == DateTime.Now && f.AbtainScore >= 16);
                    ViewBag.succes = success1;
                }
                var success = db.ExamTables.Count(f => f.ClassID == classID && f.ExamType == examType && f.Date == DateTime.Now && f.AbtainScore >= 24);
                ViewBag.succes = success;
                ViewBag.fail = ViewBag.includeExam - ViewBag.succes;
                return View(studentMarks);
            }
            else
            {
                return View(studentMarks);
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