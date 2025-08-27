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
    public class ExpencesTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
       

        // GET: UserTables
        public ActionResult Index()
        {
            var expencesTables = db.ExpencesTables.Include(e => e.StaffTable);
            return View(expencesTables.ToList());
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
        public ActionResult Create(ExpencesTable expencesTable,string Date)
        {
            bool Exist = db.ExpencesTables.Any(e => e.ItemName == expencesTable.ItemName
            && e.StaffID == expencesTable.StaffID
            && e.Amount == expencesTable.Amount
            && e.Date == expencesTable.Date);

            string DateInEnglish = Converssion.ConvertEasternArabicToWestern(Date);
            DateTime date = Converssion.ConvertShamsiToGregorian(DateInEnglish);
            expencesTable.Date = date;
            var msg = "";

          
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ExpencesTables"), JsonRequestBehavior.AllowGet });
                }
             
                //null data validation
                if (expencesTable.ExpencesID == 0)
                {

                    db.ExpencesTables.Add(expencesTable);
                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ExpencesTables"), JsonRequestBehavior.AllowGet });

                }
                else
                {
                    db.Entry(expencesTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ExpencesTables"), JsonRequestBehavior.AllowGet });
                }
          
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpencesTable expencesTable = db.ExpencesTables.Find(id);

            if (expencesTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f=>f.IsActive==true), "StaffID", "Name", expencesTable.StaffID);
           
            return View(expencesTable);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                ExpencesTable expencesTable = db.ExpencesTables.Find(id);

                var Staff = db.StaffTables.Find(expencesTable.StaffID);


                var staff = Staff.Name.ToString();
                string date = Converssion.ToShamsi(expencesTable.Date);

                if (expencesTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            expencesTable.ExpencesID,
                            expencesTable.ItemName,
                            expencesTable.Amount,
                            expencesTable.Price,
                            date,
                            expencesTable.Description,
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
            var data = db.ExpencesTables.Find(id);
            db.ExpencesTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ExpencesTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult expencesReport(string fromDate, string toDate)
        {
            
            if (fromDate!=null && toDate!=null)
            {
                string FDateInEnglish = Converssion.ConvertEasternArabicToWestern(fromDate);
                string TDateInEnglish = Converssion.ConvertEasternArabicToWestern(toDate);
                DateTime Fdate = Converssion.ConvertShamsiToGregorian(FDateInEnglish);
                DateTime Tdate = Converssion.ConvertShamsiToGregorian(TDateInEnglish);
                var expences = db.ExpencesTables.Where(f=>f.Date>= Fdate && f.Date<= Tdate);
                if (expences != null)
                {
                    @ViewBag.expencesCollection = expences.DefaultIfEmpty().Sum(f => f.Price * f.Amount);
                }
                else
                {
                    @ViewBag.expencesCollection = "څه پیدا نشو!";
                }
              
               
                @ViewBag.fromDate = FDateInEnglish;
                ViewBag.toDate = TDateInEnglish;
              
                return View(expences);
            }
            else
            {

                var expences = db.ExpencesTables.ToList();
                
                @ViewBag.expencesCollection = expences.Sum(f => f.Price * f.Amount);
                @ViewBag.currentDate = DateTime.Now.ToLongDateString();
                return View(expences);

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