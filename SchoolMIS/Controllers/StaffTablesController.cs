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
using MD.PersianDateTime;
using SchoolMIS.Models;

namespace SchoolMIS.Controllers
{
    public class StaffTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        [Authorize(Roles = "admin,Admin,teacher,Teacher,ادمین,اډمین,استاذ,استاد")]
        public ActionResult Index()
        {
           

            var staffTables = db.StaffTables.Include(s => s.StaffTypeTable).Include(s => s.UserTypeTable);
            return View(staffTables.ToList());
        }

        // GET: UserTables/Details/5
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Details(int? id)
        {

            var staffTable = db.StaffTables.Find(id);
            var Usertype = db.UserTypeTables.Find(staffTable.UserTypeID).Type.ToString();
            var Stafftype = db.StaffTypeTables.Find(staffTable.StaffTypeID).Type.ToString();
            var isActive= db.StaffTables.Find(id).IsActive;
            var Gender = db.StaffTables.Find(id).gender;
            var Active = "";
            var gender = "";
            if (isActive == true)
            {
               Active = "فعال";
            }
            else
            {
               Active = "عیر فعال";
            }
            if (Gender == "male")
            {
                gender ="نارینه";
            }
            else if (Gender == "female")
            {
                gender = "ښځینه";
            }
            else
            {
                gender = "معلوم نه دی";
            }
         
           
            var Image = "";
            if (staffTable.image == null)
            {
                Image = "/Content/img/UserIcon.png";
            }
            else
            {
                Image = staffTable.image.Substring(1);
            }
            string RegistrationDate = Converssion.ToShamsi(staffTable.RegistrationDate);



            if (staffTable != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                   
                        staffTable.Name,
                        staffTable.FatherName,
                        Image,
                        staffTable.IdentityCardID,
                        staffTable.Email,
                        staffTable.CurrentAddress,
                        staffTable.PermenentAddress,
                        staffTable.Grade,
                        staffTable.Department,
                        staffTable.Contact,
                        RegistrationDate,
                        Active,
                        gender,
                        Usertype,
                        Stafftype,
                       
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Create()
        {
            ViewBag.UserTypeID = new SelectList(db.UserTypeTables, "UserTypeID", "Type");
            ViewBag.StaffTypeID = new SelectList(db.StaffTypeTables, "StaffTypeID", "Type");
            return View();
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StaffTable staffTable, HttpPostedFileBase Image)
        {
            bool Exist = false;
            bool EditExist = false;
            if (staffTable.StaffID == 0)
            {
                Exist = db.StaffTables.Any(e =>  e.Email == staffTable.Email
                 && e.Contact == staffTable.Contact
               && e.IdentityCardID == staffTable.IdentityCardID
            );
            }
            else
            {
                EditExist = db.StaffTables.Any(e => e.Name == staffTable.Name
                             && e.FatherName == staffTable.FatherName
                             && e.Email == staffTable.Email
                              && e.RegistrationDate == staffTable.RegistrationDate
                               && e.Grade == staffTable.Grade
                              && e.Department == staffTable.Department
                              && e.image == staffTable.image
                             && e.StaffTypeID == staffTable.StaffTypeID
                               && e.Contact == staffTable.Contact
                             && e.IdentityCardID == staffTable.IdentityCardID
                             && e.UserTypeID == staffTable.UserTypeID);
            }


            var msg = "";

            if (ModelState.IsValid)
            {

                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StaffTables"), JsonRequestBehavior.AllowGet });
                }
                if (EditExist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StaffTables"), JsonRequestBehavior.AllowGet });
                }
                if (staffTable.StaffID == 0)
                {
                    if (Image != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/img/"), Image.FileName);
                        Image.SaveAs(path);
                        staffTable.image = "~/Content/img/" + Image.FileName;
                    }


                    db.StaffTables.Add(staffTable);
                    db.SaveChanges();
                    msg = "معلومات په کامیابۍ سره داخل شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffTables"), JsonRequestBehavior.AllowGet });

                }
                else
                {

                    if (Image != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/img/"), Image.FileName);
                        Image.SaveAs(path);
                        staffTable.image = "~/Content/img/" + Image.FileName;
                    }
                    db.Entry(staffTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffTables"), JsonRequestBehavior.AllowGet });


                }


            }
            msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StaffTables"), JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StaffTable staffTable = db.StaffTables.Find(id);

            if (staffTable.image == null)
            {
                Session["StaffImage"] = "/Content/img/userIcon.png";
            }
            else
            {
                Session["StaffImage"] = staffTable.image.ToString();
            }
            if (staffTable.IsActive == true)
            {
                Session["isActive"] = "checked";
            }
            else
            {
                Session["isActive"] = "";
            }
            if (staffTable.gender =="male")
            {
                Session["male"] = "checked";
            }
            else
            {
                Session["femaale"] = "checked";
            }
            if (staffTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffTypeID = new SelectList(db.StaffTypeTables, "StaffTypeID", "Type", staffTable.StaffTypeID);
            ViewBag.UserTypeID = new SelectList(db.UserTypeTables, "UserTypeID", "Type", staffTable.UserTypeID);
            return View(staffTable);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var Image = "";
          
            if (id != null)
            {
                StaffTable staffTable = db.StaffTables.Find(id);

                var Usertype = db.UserTypeTables.Find(staffTable.UserTypeID).Type.ToString();
                var Stafftype = db.StaffTypeTables.Find(staffTable.StaffTypeID).Type.ToString();
                var date = String.Format("{0:MM/dd/yyyy}", staffTable.RegistrationDate);

                if (staffTable.image == null)
                {
                    Image = "/Content/img/UserIcon.png";
                }
                else
                {
                    Image = staffTable.image.Substring(1);
                }
                if (staffTable == null)
                {
                    return Json(new { success = false, JsonRequestBehavior.AllowGet });
                }
                var isActive = db.StaffTables.Find(id).IsActive;
                var Gender = db.StaffTables.Find(id).gender;
                var Active = "";
                var gender = "";
                if (isActive == true)
                {
                    Active = "فعال";
                }
                else
                {
                    Active = "عیر فعال";
                }
                if (Gender == "male")
                {
                    gender = "نارینه";
                }
                else if (Gender == "female")
                {
                    gender = "ښځینه";
                }
                else
                {
                    gender = "معلوم نه دی";
                }
                return Json(new
                {
                    success = true,

                    data = new
                    {
                        staffTable.StaffID,
                        staffTable.Name,
                        staffTable.FatherName,
                        Image,
                        staffTable.IdentityCardID,
                        staffTable.Email,
                        staffTable.CurrentAddress,
                        staffTable.PermenentAddress,
                        staffTable.Grade,
                        staffTable.Department,
                        staffTable.Contact,
                       Active,
                        gender,
                        Usertype,
                        Stafftype,
                        staffTable.RegistrationDate,

                    },
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.StaffTables.Find(id);
            db.StaffTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره ختم شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffTables"), JsonRequestBehavior.AllowGet });
        }

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult staffReport(string isActive)
        {
            if (isActive == null || isActive=="All")
            {
                var AllStaff=db.StaffTables.ToList();
                return View(AllStaff);
            }
            else if (isActive == "true")
            {
                var ActiveStaff = db.StaffTables.Where(f => f.IsActive == true).ToList();
                return View(ActiveStaff);
            }
            else
            {
             var   noneActive=db.StaffTables.Where(f=>f.IsActive==false).ToList();
                return View(noneActive);
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