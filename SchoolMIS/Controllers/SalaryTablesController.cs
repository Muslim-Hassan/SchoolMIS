using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MD.PersianDateTime;
using SchoolMIS.Models;
using System.Globalization;


namespace SchoolMIS.Controllers
{

    public class SalaryTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Index()
        {
            var salaryTables = db.SalaryTables.Include(c => c.StaffTable);
          

            return View(salaryTables.ToList());
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult salary(int id)
        
        {
           
          var   conSalary = db.StaffTables.Where(f => f.StaffID == id ).FirstOrDefault().salary ?? 0;


            return Json(new
            {
                success = true,
                data = new
                {
                    conSalary
                }
            }, JsonRequestBehavior.AllowGet); ;

        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Create()
        {
           
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f=>f.IsActive==true), "StaffID", "Name");
            ViewBag.StaffTypeID = new SelectList(db.StaffTypeTables, "StaffTypeID", "Type");
         
            return View();
        }

        // POST: UserTypeTables/Create
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public JsonResult Create(SalaryTable salaryTable, string paidDate)
        {

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(paidDate);
            DateTime Date = Converssion.ConvertShamsiToGregorian(DateInEnglish);


            var msg = "";
            var salary = db.SalaryTables.ToList();
  

            bool Exist = db.SalaryTables.Any(e => e.Month == salaryTable.Month
            && e.StaffID == salaryTable.StaffID
            && e.PaidDate ==salaryTable.PaidDate
            && e.PaidDate == Date
            && e.remain == salaryTable.remain
            && e.paidAmount == salaryTable.paidAmount
            && e.ReciptNo == salaryTable.ReciptNo);


            salaryTable.PaidDate = Date;
      






          
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "SalaryTables"), JsonRequestBehavior.AllowGet });
                }

             
                if (salaryTable.SalaryID == 0)
                {

                    db.SalaryTables.Add(salaryTable);
                    db.SaveChanges();
                    var salaryid = salaryTable.SalaryID;
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, redirectUrl = Url.Action("printSlip", "SalaryTables", new { id = salaryid }), JsonRequestBehavior.AllowGet });

                }
                else
                {

                    db.Entry(salaryTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "SalaryTables"), JsonRequestBehavior.AllowGet });

                }
               

            
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult printSlip(int id)
        {
            var salary = db.SalaryTables.Find(id);

            if (salary == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(salary);
            }
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalaryTable salaryTable = db.SalaryTables.Find(id);
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f=>f.IsActive==true), "StaffID", "Name", salaryTable.StaffID);
            ViewBag.StaffTypeID = new SelectList(db.StaffTypeTables, "StaffTypeID", "Type");
            if (salaryTable == null)
            {
                return HttpNotFound();
            }

            return View(salaryTable);
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                SalaryTable salaryTable = db.SalaryTables.Find(id);
                var staffTable = db.StaffTables.Find(salaryTable.StaffID);
                var staff = staffTable.Name.ToString();
              
                if (staffTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }
                string date = Converssion.ToShamsi(salaryTable.PaidDate ?? DateTime.Now);
                return Json(new
                {
                    success = true,
                    msg,
                    data = new
                    {
                        salaryTable.SalaryID,
                        salaryTable.Month,
                        salaryTable.paidAmount,
                        salaryTable.remain,
                        date,
                        salaryTable.Amount,
                        staff,
                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult ConDelete(int? id)
        {
            var data = db.SalaryTables.Find(id);
            db.SalaryTables.Remove(data);
            db.SaveChanges();
            var msg = "معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "SalaryTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        // POST: UserTypeTables/Delete/5
        public JsonResult calculatesalary(int staffid, string month)
        {
            var attendency = db.StaffAttendencyTables.FirstOrDefault(f =>f.StaffID==staffid && f.Month==month);
            var present = 0;
            var formalDay = 0;
            var absent = 0;
            if (attendency != null)
            {
                 present = attendency.Present;
                formalDay = attendency.FormalDays;
                absent = formalDay - present;
            }
         
            decimal salaryAmount = 0;
            var salary = db.StaffTables.FirstOrDefault(f => f.StaffID == staffid);
            if(salary !=null )
            {
                salaryAmount = (decimal)salary.salary;
            }
            decimal totalCutSalary = 0;
            if (attendency != null)
            {
                decimal perDaySalary = (decimal)(salaryAmount / formalDay);
                decimal cutSalary = (20 * perDaySalary) / 100;
                 totalCutSalary = Math.Round((decimal)(cutSalary * absent), 2);
                ViewBag.totalCutSalary = totalCutSalary;
            }
            return Json(new
            {
                success = true,
                
                data = new
                {
                    absent,
                    totalCutSalary
                },
                JsonRequestBehavior.AllowGet
            });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public ActionResult salaryByMonthReport(string month)
        {
            if (month == null || month=="All")
            {

                var salaryByMonth = db.SalaryTables.ToList();
                if(month!=null)
                {
                    if (salaryByMonth != null)
                    {
                        ViewBag.slaryByMonth = salaryByMonth.DefaultIfEmpty().Sum(f => f.Amount);
                        ViewBag.monthName = month.ToString();
                    }
                    ViewBag.slaryByMonth =0;
                    ViewBag.monthName = 0;
                }
            
                ViewBag.currentDate = DateTime.Now.ToLongDateString();

                return View(salaryByMonth);
               
            }
            else
            {
                if (month != null)
                {
                  
                    ViewBag.monthName = month.ToString();
                }
                var salaryByMonth = db.SalaryTables.Where(f => f.Month == month).ToList();
                if (salaryByMonth != null)
                {
                    ViewBag.slaryByMonth = salaryByMonth.Sum(f => f.Amount);
                    ViewBag.currentDate = DateTime.Now.ToLongDateString();
                }
                ViewBag.slaryByMonth = 0;
                ViewBag.monthName = 0;

                return View(salaryByMonth);

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