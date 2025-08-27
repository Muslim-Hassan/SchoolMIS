using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using SchoolMIS.Models;

namespace SchoolMIS.Controllers
{
    public class StudentAttendencyTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
     
        public ActionResult Index()
        {
            var studentAttendencyTables = db.StudentAttendencyTables.Include(s => s.StaffTable).Include(s => s.StudentTable);
            return View(studentAttendencyTables.ToList());
        }

        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]

        [HttpPost]
        public JsonResult Details(int? id)
        {

            var studentAttendencyTable = db.StudentAttendencyTables.Find(id);

            var staff = db.StaffTables.Find(studentAttendencyTable.StaffID).Name;
            var student = db.StudentTables.Find(studentAttendencyTable.StudentID).Name;
     
            var date =Converssion.ToShamsi(studentAttendencyTable.Date);

            if (studentAttendencyTable != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        studentAttendencyTable.StudentAttendencyID,
                        student,
                        studentAttendencyTable.Month,
                        studentAttendencyTable.FormalDays,
                        studentAttendencyTable.Present,
                        date,
                        staff,

                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getClass(int? classID)
        {
            var staff = db.ClassTables.Where(f => f.ClassID == classID).Select(f => new SelectListItem
            {
                Text = f.StaffTable.Name,
                Value = f.StaffID.ToString()
            }).ToList();

            var studentList = db.StudentTables.Where(f=>f.ClassID==classID).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value=f.StudentID.ToString()
            }).ToList();
            return Json(new {StudentList= studentList ,Staff=staff}, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Create()
        {
          
           
           
            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name");
               
          
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.IsActive == true || f.StaffTypeTable.Type == "Teacher" || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name");
         
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");

            return View();
        }
        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentAttendencyTable studentAttendencyTable, string Date)
        {
            ViewBag.ClassID = new SelectList(db.ClassTables.Where(f=>f.ClassID==studentAttendencyTable.ClassID), "ClassID", "Name");
            bool Exist = db.StudentAttendencyTables.Any(e => e.StudentID == studentAttendencyTable.StudentID
            && e.StaffID == studentAttendencyTable.StaffID
            && e.Month == studentAttendencyTable.Month
              && e.FormalDays == studentAttendencyTable.FormalDays
              && e.Present == studentAttendencyTable.Present
               && e.Date == studentAttendencyTable.Date

            );

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(Date);
            DateTime date = Converssion.ConvertShamsiToGregorian(DateInEnglish);
            studentAttendencyTable.Date = date;
            var msg = "";

         
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                }

                //null data validation
                if (studentAttendencyTable.StudentAttendencyID == 0)
                {
                    if (studentAttendencyTable.FormalDays >= studentAttendencyTable.Present)
                    {
                        if (studentAttendencyTable.FormalDays <= 31 && studentAttendencyTable.Present <= 31)
                        {
                            db.StudentAttendencyTables.Add(studentAttendencyTable);
                            db.SaveChanges();
                            msg = "معلومات په کامیابۍ سره داخل شول";
                            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            msg = "رسمی ورځی او  حاضری باید میاشتی  په تړاو  وی";
                            return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        msg = "رسمی ورځی باید د حاضرو ورځو زیاتی یا مساوی وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                    }



                }
                else
                {
                    if (studentAttendencyTable.FormalDays >= studentAttendencyTable.Present)
                    {
                        if (studentAttendencyTable.FormalDays <= 31 && studentAttendencyTable.Present <= 31)
                        {
                            db.Entry(studentAttendencyTable).State = EntityState.Modified;
                            db.SaveChanges();
                            msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            msg = "رسمی ورځی او  حاضری باید میاشتی په تړاو وی";
                            return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                       
                    }
                    else
                    {
                        msg = "رسمی ورځی باید د حاضرو ورځو زیاتی یا مساوی وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
                    }

                }
         
         
        }
        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentAttendencyTable studentAttendencyTable = db.StudentAttendencyTables.Find(id);

            if (studentAttendencyTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher"||f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name",studentAttendencyTable.StaffID);
            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name", studentAttendencyTable.StudentID);
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name", studentAttendencyTable.ClassID);
            string Date = Converssion.ToShamsi(studentAttendencyTable.Date);
            string iraniFormate = Converssion.ToPersianDateString(Date);

            var stdAttendencyModel = new StudentAttendencyEditModel
            {
                StudentAttendencyID = studentAttendencyTable.StudentAttendencyID,
                StudentID = studentAttendencyTable.StudentID,
                Month = studentAttendencyTable.Month,
                FormalDays = studentAttendencyTable.FormalDays,
                Present = studentAttendencyTable.Present,
                Date = iraniFormate,
                ClassID = studentAttendencyTable.ClassID,
                StaffID = studentAttendencyTable.StaffID,
            };

            return View(stdAttendencyModel);
        }
        [Authorize(Roles = "admin,Admin,teacher,Teacher,اډمین,ادمین,استاذ,استاد")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                StudentAttendencyTable studentAttendencyTable = db.StudentAttendencyTables.Find(id);

                var staff = db.StaffTables.Find(studentAttendencyTable.StaffID).Name;
                var student = db.StudentTables.Find(studentAttendencyTable.StudentID).Name;

                var date = Converssion.ToShamsi(studentAttendencyTable.Date);

                if (studentAttendencyTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            studentAttendencyTable.StudentAttendencyID,
                            student,
                            studentAttendencyTable.Month,
                            studentAttendencyTable.FormalDays,
                            studentAttendencyTable.Present,
                            date,
                            staff,

                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,teacher,Teacher,اډمین,ادمین,استاذ,استاد")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.StudentAttendencyTables.Find(id);
            db.StudentAttendencyTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StudentAttendencyTables"), JsonRequestBehavior.AllowGet });
        }


        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult stdAttendency(int? studentID, int? ClassID, string month)
        {
            


            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name");
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            if (month == null || month=="All" )
            {
                
                var attendency = db.StudentAttendencyTables.Where(f => f.StudentID == studentID && f.ClassID == ClassID).ToList();

                return View(attendency);
            }
            else
            {
                var attendency = db.StudentAttendencyTables.Where(f => f.StudentID == studentID && f.ClassID == ClassID && f.Month == month).ToList();

                return View(attendency);
            }
           
        }
        [Authorize(Roles = "admin,Admin,teacher,Teacher,اډمین,ادمین,استاذ,استاد")]
        public ActionResult devoidStudent(int? ClassID)
        {
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            var allStudents=db.StudentTables.Where(f=>f.ClassID== ClassID).ToList();
            if (ClassID == null)
            {

                ViewBag.ClassName = " ";
            }
            else
            {
                ViewBag.ClassName = db.ClassTables.Find(ClassID).Name;
            }

          

            var mahroomList = new List<dynamic>();
            var notMahroomList = new List<dynamic>();
            foreach (var student in allStudents)
            {
                var attendency=db.StudentAttendencyTables.Where(f=>f.StudentID==student.StudentID).ToList();

                var totalFormalDay = attendency.Sum(f => f.FormalDays);
                var presentDay = attendency.Sum(f => f.Present);
                var absentDay = totalFormalDay - presentDay;
             

                double absencePercentage = 0.0;
                if (totalFormalDay != 0)
                {
                    absencePercentage = (double)(((double?)absentDay * 100) / totalFormalDay);

                }
                dynamic studentInfo = new
                    System.Dynamic.ExpandoObject();
                   
                    studentInfo.RollNO=student.RollNO;
                    studentInfo.Name = student.Name;
                    studentInfo.FatherName = student.FatherName;
                    studentInfo.formalDay = totalFormalDay;
                    studentInfo.absentDay = absentDay;
                    studentInfo.Image = student.Image;

                studentInfo.absentPercent = Math.Round(absencePercentage, 2);
                
                if (absencePercentage > 25)
                {
                    mahroomList.Add(studentInfo);
                }
                else
                {
                    notMahroomList.Add(studentInfo);
                }
              
            }

            ViewBag.mahroom = mahroomList;
            ViewBag.notMahroom = notMahroomList;
            return View();
          

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