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
    public class UserTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        // GET: UserTables
        public ActionResult Index()
        {
         
            var userTables = db.UserTables.Include(u => u.UserTypeTable);
          
            return View(userTables.ToList());
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
            ViewBag.UserTypeID = new SelectList(db.UserTypeTables, "UserTypeID", "Type");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserTable userTable, HttpPostedFileBase Image)
        {
            bool Exist = db.UserTables.Any(e => e.UserName == userTable.UserName && e.Password == userTable.Password && e.Image == userTable.Image && e.UserTypeID == userTable.UserTypeID);
          
            var msg = "";

            if (ModelState.IsValid)
            {

                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "UserTables"), JsonRequestBehavior.AllowGet });
                }
                    if (userTable.UserID == 0)
                    {
                        if (Image != null)
                        {
                            string path = Path.Combine(Server.MapPath("~/Content/img/"), Image.FileName);
                            Image.SaveAs(path);
                            userTable.Image = "~/Content/img/" + Image.FileName;
                        }
                  

                    db.UserTables.Add(userTable);
                  
                        db.SaveChanges();
                        msg = "معلومات په کامیابۍ سره داخل شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "UserTables"), JsonRequestBehavior.AllowGet });

                    }
                    else
                    {
                       
                            if (Image != null)
                            {
                                string path = Path.Combine(Server.MapPath("~/Content/img/"), Image.FileName);
                                Image.SaveAs(path);
                                userTable.Image = "~/Content/img/" + Image.FileName;
                            }
                            db.Entry(userTable).State = EntityState.Modified;
                            db.SaveChanges();
                            msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "UserTables"), JsonRequestBehavior.AllowGet });
                        
                       
                    }
                
               
            }
            msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "UserTables"), JsonRequestBehavior.AllowGet });
        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTable userTable = db.UserTables.Find(id);

            if (userTable.Image == null)
            {
                Session["UserImage"] = "/Content/image/userIcon.jpg";
            }
            else
            {
                Session["UserImage"] = userTable.Image.ToString().Substring(1);
            }

            if (userTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserTypeID = new SelectList(db.UserTypeTables, "UserTypeID", "Type", userTable.UserTypeID);
            return View(userTable);
        }

        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var Image = "";
            var msg = " ";
            if (id != null)
            {
                UserTable userTable = db.UserTables.Find(id);
                UserTypeTable userTypeTable = db.UserTypeTables.Find(userTable.UserTypeID);

                if (userTable.Image == null)
                {
                    Image = "/Content/img/st3.jpg";
                }
                else
                {
                    Image = userTable.Image.Substring(1);
                }

                var type = userTypeTable.Type.ToString();
                if (userTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }

                return Json(new
                {
                    success = true,

                    data = new
                    {
                        userTable.UserID,
                        userTable.UserName,
                        userTable.Password,
                        Image,
                        type

                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }

        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.UserTables.Find(id);
            db.UserTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره ختم شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "UserTables"), JsonRequestBehavior.AllowGet });
        }
        public ActionResult userReport()
        {
            var allUser = db.UserTables.Include(f=>f.UserTypeTable).ToList();
            return View(allUser);
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
