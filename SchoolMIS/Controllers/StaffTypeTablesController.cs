using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolMIS.Models;


namespace SchoolMIS.Controllers
{
    [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
    public class StaffTypeTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        // GET: UserTypeTables
       
        public ActionResult Index()
        {
            return View(db.StaffTypeTables.ToList());
        }

       
        public ActionResult Create()
        {

            return View();
        }

        // POST: UserTypeTables/Create
 
        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Create(StaffTypeTable staffTypeTable)
        {
            bool Exist = db.StaffTypeTables.Any(e => e.Type == staffTypeTable.Type && e.Description == staffTypeTable.Description);
            var msg = "";
            if (ModelState.IsValid)
            {
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StaffTypeTables"), JsonRequestBehavior.AllowGet });
                }
                    if (staffTypeTable.StaffTypeID == 0)
                    {

                        db.StaffTypeTables.Add(staffTypeTable);
                        db.SaveChanges();
                        msg = "معلومات په کامیابۍ سره داخل شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffTypeTables"), JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        
                      db.Entry(staffTypeTable).State = EntityState.Modified;
                      db.SaveChanges();
                      msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                      return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffTypeTables"), JsonRequestBehavior.AllowGet });
                     
                    }
                
              
            }
            else
            {
                msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
                return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StaffTypeTables"), JsonRequestBehavior.AllowGet });
            }
        }


        // GET: UserTypeTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StaffTypeTable staffTypeTable = db.StaffTypeTables.Find(id);
            if (staffTypeTable == null)
            {
                return HttpNotFound();
            }

            return View(staffTypeTable);
        }


        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                StaffTypeTable staffTypeTable = db.StaffTypeTables.Find(id);
                if (staffTypeTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }
               
                return Json(new
                {
                    success = true,
                    msg,
                    data = new
                    {
                        staffTypeTable.StaffTypeID,
                        staffTypeTable.Type,
                        staffTypeTable.Description
                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        // POST: UserTypeTables/Delete/5
        public JsonResult ConDelete(int? id)
        {
            var msg = "";
            var data = db.StaffTypeTables.Find(id);
            db.StaffTypeTables.Remove(data);
            db.SaveChanges();
            msg = "معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true ,msg, redirectUrl = Url.Action("Index", "StaffTypeTables"), JsonRequestBehavior.AllowGet });
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


