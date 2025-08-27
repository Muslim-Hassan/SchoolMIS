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
    [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
    public class UserTypeTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        // GET: UserTypeTables
        public ActionResult Index()
        {
            return View(db.UserTypeTables.ToList());
        }

        // GET: UserTypeTables/Details/5
        //public JsonResult Details(int id)
        //{

        //    var Record = db.UserTypeTables.Find(id);
           
        //    if (Record != null)
        //    {
        //        return Json(new
        //        {
        //            success = true,
        //            data = new
        //            {
        //                Record.Type,
        //                Record.Description,
        //            }
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        //}

        // GET: UserTypeTables/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: UserTypeTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public JsonResult Create(UserTypeTable userTypeTable)
        {
            bool Exist = db.UserTypeTables.Any(e => e.Type == userTypeTable.Type && e.Description == userTypeTable.Description);
            var msg = "";
            if (ModelState.IsValid)
            {
                if (Exist)
                {

                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "UserTypeTables"), JsonRequestBehavior.AllowGet });

                }
                if (userTypeTable.UserTypeID == 0)
                    {

                        db.UserTypeTables.Add(userTypeTable);
                        db.SaveChanges();
                        msg = "معلومات په کامیابۍ سره داخل شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "UserTypeTables"), JsonRequestBehavior.AllowGet });
                    }
                    else
                    {
                        
                            db.Entry(userTypeTable).State = EntityState.Modified;
                            db.SaveChanges();
                            msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "UserTypeTables"), JsonRequestBehavior.AllowGet });
                        
                        
                    }
                
               
                
            }
            else
            {
                msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
                return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "UserTypeTables"), JsonRequestBehavior.AllowGet });
            }
            }


        // GET: UserTypeTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTypeTable userTypeTable = db.UserTypeTables.Find(id);
            if (userTypeTable == null)
            {
                return HttpNotFound();
            }
         
            return View(userTypeTable);
        }


        [HttpPost]
        public JsonResult Delete(int? id)
        {
           
            if (id != null)
            {
                UserTypeTable userTypeTable = db.UserTypeTables.Find(id);
                if (userTypeTable == null)
                {
                    return Json(new { success = false,  JsonRequestBehavior.AllowGet });
                }
               
                return Json(new { success = true, data = new {
                    userTypeTable.UserTypeID,
                    userTypeTable.Type,
                    userTypeTable.Description
                   }, JsonRequestBehavior.AllowGet });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }

        public JsonResult ConDelete(int? id)
        {
            var msg = "";
            var data = db.UserTypeTables.Find(id);
            db.UserTypeTables.Remove(data);
            db.SaveChanges();
            msg = "معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true,msg, redirectUrl = Url.Action("Index", "UserTypeTables"), JsonRequestBehavior.AllowGet });
        }

        // POST: UserTypeTables/Delete/5
       
        
      

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
