using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using MD.PersianDateTime;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using SchoolMIS.Models;

namespace SchoolMIS.Controllers
{
    //[Authorize(Roles = "admin,Admin,teacher,Teacher,استاذ,استاد,اډمین,ادمین")]
    public class ClassSubjectTableController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        // GET: UserTables
        public ActionResult Index()
        {

            var classSubjectTables = db.ClassSubjectTables.Include(u => u.ClassTable).Include(u => u.SubjectTable);

            return View(classSubjectTables.ToList());
        }



        // GET: UserTables/Details/5

        //[HttpPost]
        //public JsonResult Details(int? id)
        //{

        //    var userTable = db.UserTables.Find(id);

        //    var type = db.UserTypeTables.Find(userTable.UserTypeID);
        //    var Image = "";
        //    if (userTable.Image == null)
        //    {
        //        Image = "/Content/img/st3.jpg";
        //    }
        //    else
        //    {
        //        Image = userTable.Image.Substring(1);
        //    }

        //    var Usertype = type.Type.ToString();

        //    if (userTable != null)
        //    {
        //        return Json(new
        //        {
        //            success = true,
        //            data = new
        //            {
        //                userTable.UserName,
        //                userTable.Password,
        //                Image,
        //                userTable.UserTypeID,
        //                Usertype,
        //            }
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Create()
        {
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassSubjectTable classSubjectTable)
        {
            bool Exist = db.ClassSubjectTables.Any(e => e.ClassID == classSubjectTable.ClassID && e.SubjectID == classSubjectTable.SubjectID );

            var msg = "";

            if (ModelState.IsValid)
            {

                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ClassSubjectTables"), JsonRequestBehavior.AllowGet });
                }
                if (classSubjectTable.ClassSubjectID == 0)
                {
                   
                    db.ClassSubjectTables.Add(classSubjectTable);

                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ClassSubjectTables"), JsonRequestBehavior.AllowGet });

                }
                else
                {

                
                    db.Entry(classSubjectTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ClassSubjectTables"), JsonRequestBehavior.AllowGet });


                }


            }
            msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "ClassSubjectTables"), JsonRequestBehavior.AllowGet });
        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassSubjectTable classSubjectTable = db.ClassSubjectTables.Find(id);

          

            if (classSubjectTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name", classSubjectTable.ClassID);
            ViewBag.SubjectID = new SelectList(db.SubjectTables, "SubjectID", "Name", classSubjectTable.SubjectID);
            return View(classSubjectTable);
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var Image = "";
            var msg = " ";
            if (id != null)
            {
                ClassSubjectTable classSubjectTable = db.ClassSubjectTables.Find(id);
                ClassTable classTable = db.ClassTables.Find(classSubjectTable.ClassID);
                SubjectTable subjectTable = db.SubjectTables.Find(classSubjectTable.SubjectID);

                var ClassName = classTable.Name.ToString();
                var SubjectName = classTable.Name.ToString();
                if (classSubjectTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }

                return Json(new
                {
                    success = true,

                    data = new
                    {
                        classSubjectTable.ClassSubjectID,
                        ClassName,
                        SubjectName
                

                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }

        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.ClassSubjectTables.Find(id);
            db.ClassSubjectTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره ختم شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "ClassSubjectTables"), JsonRequestBehavior.AllowGet });
        }
        //public ActionResult userReport()
        //{
        //    var allUser = db.UserTables.Include(f => f.UserTypeTable).ToList();
        //    return View(allUser);
        //}
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
