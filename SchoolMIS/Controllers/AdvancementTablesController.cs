using System;
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
using System.Xml.Linq;
using SchoolMIS.Models;
using static System.Net.WebRequestMethods;

namespace SchoolMIS.Controllers
{
    [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
    public class AdvancementTablesController : Controller
    {
     
        private SchoolMISEntities db = new SchoolMISEntities();



        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Index()
        {
            var advancementTables = db.AdvancementTables.Include(a => a.ClassTable).Include(a => a.ClassTable1).Include(a => a.StaffTable).Include(a => a.StudentTable);
            return View(advancementTables.ToList());
        }

        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
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
        public JsonResult Score(int? id,int? classID)
        {
            long score = 0;

         var   Stdscore = db.ExamTables.Where(f => f.StudentID == id && f.ClassID == classID);
            if (Stdscore!=null)
            {
                Stdscore.DefaultIfEmpty().Sum(f => f.AbtainScore);
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    score
                }
            }, JsonRequestBehavior.AllowGet);
        }
        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Create()
        {
         
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name");
            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name");
            ViewBag.FromClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            ViewBag.ToClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            //ViewBag.StudentScoreID = new SelectList(db.ExamTables, "StudentScoreID", "AbtainScore");
           
            return View();
        }
        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdvancementTable advancementTable, string Date)
        {
            bool Exist = db.AdvancementTables.Any(e => e.StudentID == advancementTable.StudentID
            && e.StaffID == advancementTable.StaffID
            && e.FromClassID == advancementTable.FromClassID
              && e.ToClassID == advancementTable.ToClassID
              && e.StudentScore == advancementTable.StudentScore
            && e.Date == advancementTable.Date);

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(Date);
            DateTime date = Converssion.ConvertShamsiToGregorian(DateInEnglish);
            advancementTable.Date = date;

            var msg = "";

            //if (ModelState.IsValid)
            //{
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "AdvancementTables"), JsonRequestBehavior.AllowGet });
                }



            //null data validation
            if (advancementTable.AdvancementID == 0)
                {

               
                var studentAttendance = db.StudentAttendencyTables
                   .Where(a => a.StudentID == advancementTable.StudentID)
                   .ToList();
                if (advancementTable.FromClassID!=advancementTable.ToClassID && advancementTable.FromClassID<advancementTable.ToClassID)
                    {
                    var totalDays = studentAttendance.Sum(f => f.FormalDays);
                    var presentDays = studentAttendance.Sum(f => f.Present);
                    var adsentDay = totalDays - presentDays;
                    double presentPercentage = (double)totalDays > 0 ? ((double)presentDays / (double)totalDays) * 100 : 0;
                    double absentPercentage = (double)totalDays > 0 ? ((double)adsentDay / (double)totalDays) * 100 : 0;
                   
                        var studentMarks = db.ExamTables
                   .Where(m => m.StudentID == advancementTable.StudentID && m.ClassID == advancementTable.FromClassID)
                   .ToList();
                    var totalMarks = studentMarks.Sum(m => m.TotalScore);
                    var obtainedMarks = studentMarks.Sum(m => m.AbtainScore);
                    double marksPercentage = (double)totalMarks > 0 ? ((double)obtainedMarks / (double)totalMarks) * 100 : 0;


                    if (absentPercentage < 25)
                    {
                        if (marksPercentage >= 40)
                        {
                            var className = db.ClassTables.Find(advancementTable.FromClassID).Name;
                            if (className == "نهم ټولګی")
                            {
                                ViewBag.msg = "نهم ټولګی څخه پورته ټولګی نشته";
                            }
                            bool classExist = db.ClassTables.Any(f => f.ClassID == advancementTable.FromClassID + 1);
                            if (classExist)
                            {
                               var student= db.StudentTables.Where(f=>f.StudentID==advancementTable.StudentID);
                                    student.FirstOrDefault().ClassID +=1;
                                    
                                    db.AdvancementTables.Add(advancementTable);
                                    db.ClassTables.Where(f=>f.ClassID==advancementTable.FromClassID).FirstOrDefault().StudentAmount -= 1;
                                    db.ClassTables.Where(f => f.ClassID == advancementTable.FromClassID).FirstOrDefault().PresentStudent -= 1;
                                    db.ClassTables.Where(f => f.ClassID == advancementTable.ToClassID).FirstOrDefault().StudentAmount += 1;
                                    db.ClassTables.Where(f => f.ClassID == advancementTable.ToClassID).FirstOrDefault().PresentStudent += 1;
                                    db.SaveChanges();
                                    msg = "معلومات په کامیابۍ سره داخل شول";
                                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "AdvancementTables"), JsonRequestBehavior.AllowGet });

                             }


                      
                           

                        }
                        else
                        {

                                msg = "شاګرد ناکام دی";
                                return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "AdvancementTables"), JsonRequestBehavior.AllowGet });
                            }
                    }
                    else
                    {
                            msg = "محروم";
                            return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "AdvancementTables"), JsonRequestBehavior.AllowGet });
                    }

                   

                }
                    else
                    {
                        msg = "ارتقا باید ورته صنف ته وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "AdvancementTables"), JsonRequestBehavior.AllowGet });

                    }


                }
                else
                {
                    db.Entry(advancementTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "AdvancementTables"), JsonRequestBehavior.AllowGet });
                }
            //}
            //msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "AdvancementTables"), JsonRequestBehavior.AllowGet });
        }
        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdvancementTable advancementTable = db.AdvancementTables.Find(id);
            string Date = Converssion.ToShamsi(advancementTable.Date);
            string iraniFormate = Converssion.ToPersianDateString(Date);
            if (advancementTable == null)
            {
                return HttpNotFound();
            }
            var advancementEditModel = new AdvancementEditModel
            {
                AdvancementID = advancementTable.AdvancementID,
                StudentID = advancementTable.StudentID,
                FromClassID = advancementTable.FromClassID,
                ToClassID = advancementTable.ToClassID,
                StudentScore = advancementTable.StudentScore,
                Date = iraniFormate,
            
                StaffID = advancementTable.StaffID,
            };


            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name", advancementTable.StaffID);
            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name", advancementTable.StudentID);
            ViewBag.FromClassID = new SelectList(db.ClassTables, "ClassID", "Name", advancementTable.FromClassID);
            ViewBag.ToClassID = new SelectList(db.ClassTables, "ClassID", "Name", advancementTable.ToClassID);
            //ViewBag.StudentScoreID = new SelectList(db.ExamTables, "StudentScoreID", "AbtainScore", advancementTable.StudentScore);

            return View(advancementEditModel);
        }
        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
               AdvancementTable advancementTable = db.AdvancementTables.Find(id);

                var staff = db.StaffTables.Find(advancementTable.StaffID).Name.ToString();
                var student = db.StudentTables.Find(advancementTable.StudentID).Name.ToString();
                var fromClass = db.ClassTables.Find(advancementTable.FromClassID).Name.ToString();
                var toClass = db.ClassTables.Find(advancementTable.ToClassID).Name.ToString();
                string date = Converssion.ToShamsi(advancementTable.Date);
                //var date = String.Format("{0:MM/dd/yyyy}", advancementTable.Date);

                if (advancementTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            advancementTable.AdvancementID,
                            student,
                            fromClass,
                            toClass,
                            date,
                            advancementTable.StudentScore, 
                            staff,

                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.AdvancementTables.Find(id);
            db.AdvancementTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "AdvancementTables"), JsonRequestBehavior.AllowGet });
        }
        //[Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult advancementAtOnce(int? classID)
        {
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            var allClassStudents = db.StudentTables.Where(s => s.ClassID == classID).ToList();

            var promoted = new List<dynamic>();
            var failed = new List<dynamic>();
            var droppedOut = new List<dynamic>();

            foreach (var student in allClassStudents)
            {
                var studentAttendance = db.StudentAttendencyTables
                    .Where(a => a.StudentID == student.StudentID)
                    .ToList();

                var totalDays = studentAttendance.Sum(f => f.FormalDays);
                var presentDays = studentAttendance.Sum(f => f.Present);
                var adsentDay = totalDays - presentDays;
                double presentPercentage = (double)totalDays > 0 ? ((double)presentDays / (double)totalDays) * 100 : 0;
                double absentPercentage = (double)totalDays > 0 ? ((double)adsentDay / (double)totalDays) * 100 : 0;

                var studentMarks = db.ExamTables
                    .Where(m => m.StudentID == student.StudentID && m.ClassID==classID)
                    .ToList();

                var totalMarks = studentMarks.Sum(m => m.TotalScore);
                var obtainedMarks = studentMarks.Sum(m => m.AbtainScore);
                double marksPercentage = (double)totalMarks > 0 ? ((double)obtainedMarks / (double)totalMarks) * 100 : 0;
                dynamic studentResult = new
                  System.Dynamic.ExpandoObject();

                studentResult.RollNO = student.RollNO;
                studentResult.Name = student.Name;
                studentResult.FatherName = student.FatherName;
                studentResult.formalDay = totalDays;
                studentResult.absentDay = adsentDay;
                studentResult.absentPercentage = Math.Round(absentPercentage, 2);
                studentResult.marksPercentage = Math.Round(marksPercentage, 2);
                int count = 0;
                student.ClassTable.StudentAmount -= 1;
                student.ClassTable.PresentStudent -= 1;

                if (absentPercentage < 25)
                {
                    if (marksPercentage >= 40)
                    {
                        var className = db.ClassTables.Find(classID).Name;
                        if (className=="نهم ټولګی")
                        {
                            ViewBag.msg = "نهم ټولګی څخه پورته ټولګی نشته";
                        }
                        bool classExist = db.ClassTables.Any(f => f.ClassID == student.ClassID + 1);
                       if (classExist)
                        {
                           
                            student.ClassID += 1;
                            
                        }


                        student.ClassTable.StudentAmount += 1;
                        student.ClassTable.PresentStudent += 1;
                        db.SaveChanges();
                        count += 1;
                        promoted.Add(studentResult);
                        student.ClassTable.StudentAmount = count;
                        student.ClassTable.PresentStudent = count;

                    }
                    else
                    {
                        failed.Add(studentResult);
                        student.ClassTable.StudentAmount += 1;
                        student.ClassTable.PresentStudent += 1;
                    }
                }
                else
                {
                    droppedOut.Add(studentResult);
                    student.ClassTable.StudentAmount += 1;
                    student.ClassTable.PresentStudent += 1;
                }
            }

            ViewBag.Promoted = promoted;
            ViewBag.Failed = failed;
            ViewBag.DroppedOut = droppedOut;


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
