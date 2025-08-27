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
using SchoolMIS.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static System.Globalization.PersianCalendar;

using static SchoolMIS.Models.Converssion;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SchoolMIS.Controllers
{
    public class FeeTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        [Authorize(Roles = "admin,Admin,teacher,Teacher,ادمین,اډمین,استاذ,استاد")]
        // GET: UserTables
        public ActionResult Index()
        {
            var feeTables = db.FeeTables.Include(f => f.StaffTable).Include(f => f.StudentTable);
            return View(feeTables.ToList());
        }

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Create()
        {

            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name");
            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name");
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");

            return View();
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FeeTable feeTable ,string paidDate)
        {
            bool Exist = db.FeeTables.Any(e => e.Amount == feeTable.Amount
            && e.month == feeTable.month
            && e.ReciptNo == feeTable.ReciptNo
            && e.StudentID == feeTable.StudentID);
            string englishTextDate =Converssion.ConvertEasternArabicToWestern(paidDate);
            DateTime meladiDate = Converssion.ConvertShamsiToGregorian(englishTextDate);
            feeTable.PaidDate = meladiDate;
            var msg = "";

            //if (ModelState.IsValid)
            //{
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "FeeTables"), JsonRequestBehavior.AllowGet });
                }

                //null data validation
                if (feeTable.FeeID == 0)
                {
                    db.FeeTables.Add(feeTable);
                    db.SaveChanges();
                    var feeid = feeTable.FeeID;
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, redirectUrl = Url.Action("printSlip", "FeeTables", new { id = feeid }), JsonRequestBehavior.AllowGet });

                }
                else
                {

                    db.Entry(feeTable).State = EntityState.Modified;
                    db.SaveChanges();
                    var feeid = feeTable.FeeID;
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, redirectUrl = Url.Action("printSlip", "FeeTables", new { id = feeid }), JsonRequestBehavior.AllowGet });
                }
            //}
            //msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            //return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "FeeTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult printSlip(int id)
        {
            var fee = db.FeeTables.Find(id);

            if (fee == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(fee);
            }
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeeTable feeTable = db.FeeTables.Find(id);

            if (feeTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.IsActive == true || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name", feeTable.StaffID);

            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name", feeTable.StudentID);


            return View(feeTable);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                FeeTable feeTable = db.FeeTables.Find(id);

                var staff = db.StaffTables.Find(feeTable.StaffID).Name.ToString();
                var student = db.StudentTables.Find(feeTable.StudentID).Name.ToString();
                string date = Converssion.ToShamsi(feeTable.PaidDate);

                if (feeTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            feeTable.FeeID,
                            feeTable.Amount,
                            feeTable.Discount,
                            feeTable.Remain,
                            feeTable.NetAmount,
                            date,
                            student,
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
            var data = db.FeeTables.Find(id);
            db.FeeTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "FeeTables"), JsonRequestBehavior.AllowGet });
        }
        //private DateTime ConvertShamsiToGregorian(string shamsiDate)
        //{
        //    if (string.IsNullOrEmpty(shamsiDate))
        //        throw new ArgumentException("Invalid Shamsi date");

        //    string[] parts = shamsiDate.Split('/');
        //    if (parts.Length != 3)
        //        throw new FormatException("Shamsi date must be in yyyy/MM/dd format");

        //    int year = int.Parse(parts[0]);
        //    int month = int.Parse(parts[1]);
        //    int day = int.Parse(parts[2]);

        //    PersianCalendar pc = new PersianCalendar();
        //    return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        //}

    
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult feeReport(string fromDate, string toDate)
        {

            if (fromDate != null && toDate != null)
            {


                string fromDateInEnglish = Converssion.ConvertEasternArabicToWestern(fromDate);
                string toDateInEnglish = Converssion.ConvertEasternArabicToWestern(toDate);


                DateTime FromDate = Converssion.ConvertShamsiToGregorian(fromDateInEnglish);
            
                DateTime ToDate = Converssion.ConvertShamsiToGregorian(toDateInEnglish);

                var allFees = db.FeeTables.ToList();
                var filteredFees = allFees.Where(f => f.PaidDate >= FromDate && f.PaidDate <= ToDate).ToList();

                return View(filteredFees);
            }
            else
            {
                var feecollection = db.FeeTables.ToList();

                @ViewBag.feeCollection = feecollection.Sum(f => f.NetAmount);
                @ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                return View(feecollection);

            }

        }
   
         [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult stdPaidAndUnpaidFee(string fromDate, string toDate ,int? classID ,string status)
        {

            if (fromDate != null && toDate != null && classID!=null)
            {
                string fromDateInEnglish = Converssion.ConvertEasternArabicToWestern(fromDate);
                string toDateInEnglish = Converssion.ConvertEasternArabicToWestern(toDate);
                DateTime FromDate = Converssion.ConvertShamsiToGregorian(fromDateInEnglish);
                DateTime ToDate = Converssion.ConvertShamsiToGregorian(toDateInEnglish);
                // Students of the selected class
                var studentsInClass = db.StudentTables.Where(s => s.ClassID == classID);

                if (status == "paid")
                {
                    var paidStudents = db.FeeTables
                                  .Where(f => f.PaidDate >= FromDate && f.PaidDate <= ToDate && f.StudentTable.ClassID == classID)
                                  .Select(f => f.StudentTable)
                                  .Distinct()
                                  .ToList();
                    ViewBag.PaidStudents = paidStudents;
                    @ViewBag.msg = "داخل کړی دی";
                    ViewBag.FromDate = fromDate;
                    ViewBag.ToDate = toDate;
                    var Fee = db.FeeTables?
                                  .Where(f => f.PaidDate >= FromDate && f.PaidDate <= ToDate && f.StudentTable.ClassID == classID).DefaultIfEmpty().FirstOrDefault().Amount;
                    if (Fee != null)
                    {
                        ViewBag.Fee = Fee;
                    }
                    else
                    {
                        ViewBag.Fee = "فیسونو داخلوونکی شاګردان نشته";
                    }
                   
                    ViewBag.feecollection = db.FeeTables?
                                  .Where(f => f.PaidDate >= FromDate && f.PaidDate <= ToDate && f.StudentTable.ClassID == classID).Sum(f => f.NetAmount);

                }
                else
                {
                    var notPaidStudents = studentsInClass?
                  .Where(s => !db.FeeTables.Any(f => f.StudentID == s.StudentID && f.PaidDate >= FromDate && f.PaidDate <= ToDate)).ToList();


                    @ViewBag.msg = "یی نه دی داخل کړی";
                    ViewBag.NotPaidStudents = notPaidStudents;
                    ViewBag.FromDate = fromDate;
                    ViewBag.ToDate = toDate;
                }

            }
            else
            {
                var feecollection = db.FeeTables.ToList();

                @ViewBag.feeCollection = feecollection.Sum(f => f.NetAmount);
                @ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.ClassName = db.ClassTables.Where(c => c.ClassID == classID).Select(c => c.Name).FirstOrDefault();
                ViewBag.ClassList = new SelectList(db.ClassTables, "ClassID", "Name");
                return View(feecollection);
               
            }



            ViewBag.ClassName = db.ClassTables.Where(c => c.ClassID == classID).Select(c => c.Name).FirstOrDefault();
            ViewBag.ClassList = new SelectList(db.ClassTables, "ClassID", "Name");
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
