using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using SchoolMIS.Models;


namespace SchoolMIS.Controllers
{

    public class SubjectTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Index()
        {
            return View(db.SubjectTables.ToList());
        }


        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        public ActionResult Create()
        {

            return View();
        }

        // POST: UserTypeTables/Create

        [Authorize(Roles = "admin,Admin,استاذ,استاد,teacher,Teacher,اډمین,ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult Create(SubjectTable subjectTable)
        {
            bool Exist = db.ClassSubjectTables.Any(e => e.SubjectTable.Name == subjectTable.Name && e.ClassTable.Name == subjectTable.Name);
            var msg = "";
            if (ModelState.IsValid)
            {
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "SubjectTables"), JsonRequestBehavior.AllowGet });
                }
                if (subjectTable.SubjectID == 0)
                {

                    db.SubjectTables.Add(subjectTable);
                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "SubjectTables"), JsonRequestBehavior.AllowGet });
                }
                else
                {

                    db.Entry(subjectTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "SubjectTables"), JsonRequestBehavior.AllowGet });

                }


            }
            else
            {
                msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
                return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "SubjectTables"), JsonRequestBehavior.AllowGet });
            }
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        // GET: UserTypeTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectTable subjectTable = db.SubjectTables.Find(id);
         
            if (subjectTable == null)
            {
                return HttpNotFound();
            }

            return View(subjectTable);
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
          
            if (id != null)
            {
                SubjectTable subjectTable = db.SubjectTables.Find(id);
                if (subjectTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }
              
                return Json(new
                {
                    success = true,
                    
                    data = new
                    {
                        subjectTable.SubjectID,
                        subjectTable.Name,
                      
                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = "";
            var data = db.SubjectTables.Find(id);
            db.SubjectTables.Remove(data);
            db.SaveChanges();
            msg = "معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true,msg ,redirectUrl = Url.Action("Index", "SubjectTables"), JsonRequestBehavior.AllowGet });
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
