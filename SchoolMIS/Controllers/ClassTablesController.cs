using System;
using System.Collections;
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

    public class ClassTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();

        [Authorize(Roles = "admin,Admin,اډمین,ادمین,teacher,Teacher,استاذ,استاد")]
        public ActionResult Index()
        {
            var classTables = db.ClassTables.Include(c => c.StaffTable);
            return View(classTables.ToList());
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Create()
        {
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name");

            return View();
        }

        // POST: UserTypeTables/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult Create(ClassTable classTable)
        {
            bool Exist = db.ClassTables.Any(e => e.Name == classTable.Name
            && e.StaffID == classTable.StaffID
            && e.StudentAmount == classTable.StudentAmount);
            var msg = "";

            if (ModelState.IsValid)
            {
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ClassTables"), JsonRequestBehavior.AllowGet });
                }
                if (classTable.StudentAmount >= classTable.PresentStudent)
                {
                    if (classTable.ClassID == 0)
                    {

                        db.ClassTables.Add(classTable);
                        db.SaveChanges();
                        msg = "معلومات په کامیابۍ سره داخل شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ClassTables"), JsonRequestBehavior.AllowGet });
                    }
                    else
                    {

                        db.Entry(classTable).State = EntityState.Modified;
                        db.SaveChanges();
                        msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ClassTables"), JsonRequestBehavior.AllowGet });

                    }
                }
                else
                {
                    msg = " .دحاضرو شاګردانو شمیر باید دټولو د شمیر څخه کم وی    ";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "ClassTables"), JsonRequestBehavior.AllowGet });

                }

            }
            else
            {
                msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
                return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "ClassTables"), JsonRequestBehavior.AllowGet });
            }
        }


        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {
          

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassTable classTable = db.ClassTables.Find(id);
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f => f.StaffTypeTable.Type == "teacher" || f.StaffTypeTable.Type == "Teacher" || f.StaffTypeTable.Type == "استاذ"), "StaffID", "Name", classTable.StaffID);

            if (classTable == null)
            {
                return HttpNotFound();
            }

            return View(classTable);
        }

        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                ClassTable classTable = db.ClassTables.Find(id);
                var staff = db.StaffTables.Find(classTable.StaffID).Name.ToString();
               
                if (classTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }
              
                return Json(new
                {
                    success = true,
                    msg,
                    data = new
                    {
                        classTable.ClassID,
                        classTable.Name,
                        classTable.StudentAmount,
                        classTable.PresentStudent,
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
            var data = db.ClassTables.Find(id);
            db.ClassTables.Remove(data);
            db.SaveChanges();
           var  msg = "معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ClassTables"), JsonRequestBehavior.AllowGet });
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
