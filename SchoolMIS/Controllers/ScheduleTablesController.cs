using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using SchoolMIS.Models;
namespace SchoolMIS.Controllers
{
    public class ScheduleTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        [Authorize(Roles = "admin,Admin,teacher,Teacher,ادمین,اډمین,استاذ,استاد")]
        public ActionResult Index()
        {
            var scheduleTables = db.ScheduleTables.Include(s => s.ClassTable).Include(s => s.StaffTable).Include(s => s.SubjectTable);
            return View(scheduleTables.ToList());
        }

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Create()
        {

            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name");
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name");
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
          
            return View();
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ScheduleTable scheduleTable ,string Date)
        {
            bool Exist = db.ScheduleTables.Any(e => e.SubjectID == scheduleTable.SubjectID
            && e.StaffID == scheduleTable.StaffID
            && e.ClassID == scheduleTable.ClassID
              && e.Day == scheduleTable.Day
              && e.StartTime == scheduleTable.StartTime
              && e.EndTime == scheduleTable.EndTime
            && e.Date == scheduleTable.Date);

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(Date);
            DateTime date = Converssion.ConvertShamsiToGregorian(DateInEnglish);
            scheduleTable.Date = date;

            var msg = "";

            //if (ModelState.IsValid)
            //{
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ScheduleTables"), JsonRequestBehavior.AllowGet });
                }

                //null data validation
                if (scheduleTable.ScheduleID == 0)
                {

              
                if (scheduleTable.StartTime != scheduleTable.EndTime)
                    {
                        db.ScheduleTables.Add(scheduleTable);
                        db.SaveChanges();
                        msg = "معلومات په کامیابۍ سره داخل شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ScheduleTables"), JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        msg = " ساعت شروع او ختم باید سره مساوی نه وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "ScheduleTables"), JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    if (scheduleTable.StartTime != scheduleTable.EndTime)
                    {
                        db.Entry(scheduleTable).State = EntityState.Modified;
                        db.SaveChanges();
                        msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ScheduleTables"), JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        msg = " ساعت شروع او ختم باید سره مساوی نه وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "ScheduleTables"), JsonRequestBehavior.AllowGet });
                    }

                    
                }
            //}
            //msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            //return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ScheduleTables"), JsonRequestBehavior.AllowGet });
        }

       [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleTable scheduleTable = db.ScheduleTables.Find(id);

            if (scheduleTable == null)
            {
                return HttpNotFound();
            }
              ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name",scheduleTable.StaffID);
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name", scheduleTable.SubjectID);
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name", scheduleTable.ClassID);
          
            return View(scheduleTable);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                ScheduleTable scheduleTable = db.ScheduleTables.Find(id);

                var staff = db.StaffTables.Find(scheduleTable.StaffID).Name;
                var subject = db.SubjectTables.Find(scheduleTable.SubjectID).Name;
                var Class = db.ClassTables.Find(scheduleTable.ClassID).Name;
                var stime = scheduleTable.StartTime.ToString();
                var etime = scheduleTable.EndTime.ToString();
                string date = Converssion.ToShamsi(scheduleTable.Date);

                if (scheduleTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            scheduleTable.ScheduleID,
                            scheduleTable.Day,
                            Class,
                            scheduleTable.Credit,
                            date,
                            subject,
                            stime,
                            etime,
                            staff,

                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.ScheduleTables.Find(id);
            db.ScheduleTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ScheduleTables"), JsonRequestBehavior.AllowGet });
        }


        //public ActionResult pushstaffID()
        //{
        //    ViewBag.StaffList = new SelectList(db.StaffTables.ToList(), "StaffID", "Name");
        //    return View();
        //}
        //[Authorize(Roles = "admin,Admin,teacher,Teacher,ادمین,اډمین,استاذ,استاد")]
        public ActionResult teacherTimeTable(int? staffId)
        {
            var staffList = db.StaffTables.FirstOrDefault().StaffID;
            if (staffId == null)
            {
                staffId = staffList;
            }
          
                
                ViewBag.StaffList = new SelectList(db.StaffTables.Where(f=>f.StaffTypeTable.Type=="teacher"|| f.IsActive == true || f.StaffTypeTable.Type == "Teacher" || f.StaffTypeTable.Type == "Principle" || f.StaffTypeTable.Type == "principle" || f.StaffTypeTable.Type == "استاذ"|| f.StaffTypeTable.Type == "مدیر").ToList(), "StaffID", "Name");

                ViewBag.Name = db.StaffTables.DefaultIfEmpty().Where(f => f.StaffID == staffId).FirstOrDefault().Name;
                ViewBag.Type = db.StaffTables.Where(f => f.StaffID == staffId).FirstOrDefault().StaffTypeTable.Type;
                ViewBag.Image = db.StaffTables.Where(f => f.StaffID == staffId).FirstOrDefault().image;
            
         
            var schedules = db.ScheduleTables.Where(f=>f.StaffID==staffId)
                .Include(s => s.SubjectTable)
                .Include(s => s.ClassTable)
                .ToList();
            ViewBag.currentDate = DateTime.Now.ToLongDateString();
            var allTimes = schedules
                    .Select(x => $"{x.StartTime:hh\\:mm}-{x.EndTime:hh\\:mm}")
                   .Distinct()
                  .OrderBy(t => t)
                 .ToList();
            //ViewBag.AllTimes = allTimes;

            var groupedSchedule = schedules
       .GroupBy(s => s.Day)
       .Select(g => new timeTable
       {
           Day = g.Key,
           TimeSubjectMapping = g.ToDictionary(
               x => $"{x.StartTime:hh\\:mm}-{x.EndTime:hh\\:mm}",
               x => $"{x.SubjectTable.Name} ({x.ClassTable.Name})"
           )
       }).ToList();
            ViewBag.StaffID = staffId;
            ViewBag.AllTimes = allTimes;

            return View(groupedSchedule);

        }




        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult classTimeTable(int? classId)
        {
            var classLIst = db.ClassTables.FirstOrDefault().ClassID;
            if (classId == null)
            {
                classId = classLIst;
            }
            ViewBag.ClassList = new SelectList(db.ClassTables.ToList(), "ClassID", "Name");
            var studentTimeTable = db.ScheduleTables.Where(stdf => stdf.ClassID == classId)
                .Include(std => std.SubjectTable)
                .Include(std => std.StaffTable)
                .ToList();
            ViewBag.currentDate = DateTime.Now.ToLongDateString();
            ViewBag.Name = db.ClassTables.DefaultIfEmpty().Where(f => f.ClassID == classId).FirstOrDefault().Name;
            var stdAllTimes = studentTimeTable
                    .Select(stime => $"{stime.StartTime:hh\\:mm}-{stime.EndTime:hh\\:mm}")
                    .OrderBy(t => t)
                    .Distinct()
                  
                 .ToList();
        
         
          
            var groupedStdSchedule = studentTimeTable
           .GroupBy(s => s.Day)
           .Select(g => new stdTimeTable
           {
               Day = g.Key,
               subStaffAndTime = g.ToDictionary(
                 
                   x => $"{x.StartTime:hh\\:mm}-{x.EndTime:hh\\:mm}",
                   x => $"{x.SubjectTable.Name} ({x.StaffTable.Name})"
               )
           }).ToList();
            ViewBag.ClassID = classId;
            ViewBag.stdAllTimes = stdAllTimes;

            return View(groupedStdSchedule);

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