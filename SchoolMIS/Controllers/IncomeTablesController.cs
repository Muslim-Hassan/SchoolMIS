using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using SchoolMIS.Models;

namespace SchoolMIS.Controllers
{
    public class IncomeTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        // GET: UserTables
        public ActionResult Index()
        {
            var incomeTables = db.IncomeTables.Include(e => e.StaffTable);
            return View(incomeTables.ToList());
        }

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Details(int? id)
        {
            var incomeTable = db.IncomeTables.Find(id);

            var staff = db.StaffTables.Find(incomeTable.StaffID).Name.ToString();


            string date = Converssion.ToShamsi(incomeTable.PaidDate);

            if (incomeTable != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        incomeTable.IncomeID,
                        incomeTable.Name,
                        incomeTable.Amount,
                        incomeTable.price,
                        incomeTable.Discount,
                        incomeTable.NetAmount,
                        date,
                        incomeTable.Description,
                        staff,

                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Create()
        {

            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.IsActive == true), "StaffID", "Name");

            return View();
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IncomeTable incomeTable ,string Date)
        {
            bool Exist = db.IncomeTables.Any(e => e.Name == incomeTable.Name
            && e.Amount == incomeTable.Amount
             && e.price == incomeTable.price
            && e.PaidDate == incomeTable.PaidDate);

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(Date);
            DateTime date = Converssion.ConvertShamsiToGregorian(DateInEnglish);
            var msg = "";
            incomeTable.PaidDate = date;
            //if (ModelState.IsValid)
            //{
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "IncomeTables"), JsonRequestBehavior.AllowGet });
                }

                //null data validation
                if (incomeTable.IncomeID == 0)
                {

              

                    db.IncomeTables.Add(incomeTable);
                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "IncomeTables"), JsonRequestBehavior.AllowGet });

                }
                else
                {
                    db.Entry(incomeTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "IncomeTables"), JsonRequestBehavior.AllowGet });
                }
            //}
            //msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            //return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "IncomeTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IncomeTable incomeTable = db.IncomeTables.Find(id);

            if (incomeTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.IsActive == true), "StaffID", "Name", incomeTable.StaffID);

            return View(incomeTable);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                IncomeTable incomeTable = db.IncomeTables.Find(id);

                var staff = db.StaffTables.Find(incomeTable.StaffID).Name.ToString();


                string date = Converssion.ToShamsi(incomeTable.PaidDate);

                if (incomeTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            incomeTable.IncomeID,
                            incomeTable.Name,
                            incomeTable.Amount,
                            incomeTable.price,
                            incomeTable.Discount,
                            incomeTable.NetAmount,
                            date,
                            incomeTable.Description,
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
            var data = db.IncomeTables.Find(id);
            db.IncomeTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "IncomeTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult incomeReport(string fromDate, string toDate)
        {
    
            if (fromDate != null && toDate != null)
            {
                string FDateInEnglish = Converssion.ConvertEasternArabicToWestern(fromDate);
                string TDateInEnglish = Converssion.ConvertEasternArabicToWestern(toDate);
                DateTime Fdate = Converssion.ConvertShamsiToGregorian(FDateInEnglish);
                DateTime Tdate = Converssion.ConvertShamsiToGregorian(TDateInEnglish);
                var income = db.IncomeTables.Where(f =>f.PaidDate >= Fdate && f.PaidDate <= Tdate);
                @ViewBag.incomeCollection = income.DefaultIfEmpty().Sum(f => f.NetAmount);
                @ViewBag.fromDate = FDateInEnglish;
                ViewBag.toDate = TDateInEnglish;
               
                return View(income);
            }
            else
            {
                var income = db.IncomeTables.ToList();

                @ViewBag.incomeCollection = income.Sum(f => f.NetAmount);
                @ViewBag.currentDate = DateTime.Now.ToLongDateString();
                return View(income);

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
