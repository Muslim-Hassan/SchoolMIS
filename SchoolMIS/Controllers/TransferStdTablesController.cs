using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using MD.PersianDateTime;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using SchoolMIS.Models;
using System.Globalization;
using static SchoolMIS.Models.Converssion;

namespace SchoolMIS.Controllers
{
    [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
    public class TransferStdTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        // GET: UserTables
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Index()
        {
          var transferStdTables = db.TransferStdTables.Include(t => t.ClassTable);
          return View(transferStdTables.ToList());
        }
        //[Authorize(Roles = "admin,Admin")]
        //public JsonResult Attendency(int? id,int? classID)
        //{
        //    long score = 0;
        //    var present = db.StudentAttendencyTables.DefaultIfEmpty().Where(f=>f.StudentID==id && f.ClassID==classID).DefaultIfEmpty().Sum(f=>f.Present);
        //     score = db.ExamTables.DefaultIfEmpty().Where(f => f.StudentID == id && f.ClassID == classID).DefaultIfEmpty().Sum(f => f.AbtainScore);
           
         
        //    return Json(new
        //    {
        //        success = true,
        //        data = new
        //        {
        //            score,
                 
        //            present
        //        }
        //    }, JsonRequestBehavior.AllowGet);
       

        //}

        // GET: UserTables/Details/5
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Details(int? id)
        {

            var transferStdTable = db.TransferStdTables.Find(id);

         
            //var student = db.StudentTables.Find(transferStdTable.StudentID).Name;
            //var FatherName = db.StudentTables.Find(transferStdTable.StudentID).FatherName;
            //var RollNo = db.StudentTables.Find(transferStdTable.StudentID).RollNO;
            var Class = db.ClassTables.Find(transferStdTable.ClassID).Name;
         

            var FrontImage = "";
            var BackImage = "";
            if (transferStdTable.FrontImage == null && transferStdTable.BackImage == null)
            {
                FrontImage = "/Content/img/UserIcon.png";
                BackImage = "/Content/img/UserIcon.png";
            }
            else
            {
                FrontImage = transferStdTable.FrontImage.Substring(1);
                BackImage = transferStdTable.BackImage.Substring(1);
            }
            string Date = ToShamsi(transferStdTable.Date);
            if (transferStdTable != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        transferStdTable.TransferStdID,
                        transferStdTable.FromSchool,
                        transferStdTable.ToSchool,
                        transferStdTable.RollNo,
                        transferStdTable.Name,
                        transferStdTable.FatherName,
                        Date,
                        FrontImage,
                        BackImage,
                        Class,

                    }
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        }
        //[Authorize(Roles = "admin,Admin")]
        public ActionResult Create()
        {

           
       
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name");
 
            return View();
        }
        [HttpPost]
        public JsonResult ClassName(int rollNo)
        {
            var ClassName = db.StudentTables.Where(f => f.RollNO == rollNo).Select(s => new {
                Text = s.ClassTable.Name,
                Value = s.ClassID.ToString()
            }).Distinct().ToList();
            return Json(new
            {
                success = true,
                data = new
                {
                    ClassName
                }
            }, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransferStdTable transferStdTable,HttpPostedFileBase FrontImage, HttpPostedFileBase BackImage, string Date,int rollNo)
        {
            bool Exist = db.TransferStdTables.Any(e => e.RollNo == rollNo
            && e.ClassID == transferStdTable.ClassID
         
            && e.FromSchool == transferStdTable.FromSchool
            && e.ToSchool == transferStdTable.ToSchool
         
            && e.Date == transferStdTable.Date);
            var msg = "";
           
            //Exist record validation
            if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "TransferStdTables"), JsonRequestBehavior.AllowGet });
                }
             

            //null data validation
            if (transferStdTable.TransferStdID == 0)
            {
                string engDate = ConvertEasternArabicToWestern(Date);
                DateTime date = ConvertShamsiToGregorian(engDate);

                if (transferStdTable.FromSchool != transferStdTable.ToSchool)
                    {
                      
                        var stdRecord = db.StudentTables.Find(rollNo);
                        if (FrontImage != null)
                        {
                            string path = Path.Combine(Server.MapPath("~/Content/img/"), FrontImage.FileName);
                            FrontImage.SaveAs(path);
                          var saveFrontImage=  transferStdTable.FrontImage = "~/Content/img/" + FrontImage.FileName;



                        }
                        if (BackImage != null)
                        {

                            string path = Path.Combine(Server.MapPath("~/Content/img/"), BackImage.FileName);
                            BackImage.SaveAs(path);
                            var saveBackImage = transferStdTable.BackImage = "~/Content/img/" + BackImage.FileName;
                        }
                        var transfer = new TransferStdTable
                        {
                            TransferStdID=transferStdTable.TransferStdID,

                            StudentID = stdRecord.StudentID,
                            FrontImage=   transferStdTable.FrontImage,
                            BackImage = transferStdTable.BackImage,
                            FromSchool =transferStdTable.FromSchool,
                            ToSchool = transferStdTable.ToSchool,
                            ClassID = transferStdTable.ClassID,
                            Name =stdRecord.Name,
                            FatherName = stdRecord.FatherName,
                            RollNo=stdRecord.RollNO,
                            Date= date,


                        };

                       

                        db.TransferStdTables.Add(transfer);
                        db.SaveChanges();
                        db.StudentTables.Remove(stdRecord);
                        db.SaveChanges();
                        msg = "معلومات په کامیابۍ سره داخل شول";
                        return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "TransferStdTables"), JsonRequestBehavior.AllowGet });

                    }
                    else
                    {
                        msg = "د شاګرد سه پرچه باید بل مکتب ته وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "TransferStdTables"), JsonRequestBehavior.AllowGet });

                    }


                }
                else
                {
                    if (FrontImage != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/img/"), FrontImage.FileName);
                        FrontImage.SaveAs(path);
                        var saveFrontImage = transferStdTable.FrontImage = "~/Content/img/" + FrontImage.FileName;



                    }
                    if (BackImage != null)
                    {

                        string path = Path.Combine(Server.MapPath("~/Content/img/"), BackImage.FileName);
                        BackImage.SaveAs(path);
                        var saveBackImage = transferStdTable.BackImage = "~/Content/img/" + BackImage.FileName;
                    }
                
               

                    db.Entry(transferStdTable).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                    return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "TransferStdTables"), JsonRequestBehavior.AllowGet });
                }
         
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferStdTable transferStdTable = db.TransferStdTables.Find(id);
            if (transferStdTable.FrontImage == null && transferStdTable.BackImage==null)
            {
                Session["FrontImage"] = "/Content/img/UserIcon.png";
                Session["BackImage"] = "/Content/img/UserIcon.png";
            }
            else
            {
                Session["FrontImage"] = transferStdTable.FrontImage.ToString();
                Session["BackImage"] = transferStdTable.BackImage.ToString();
            }
            if (transferStdTable == null)
            {
                return HttpNotFound();
            }
           
            ViewBag.StudentID = new SelectList(db.StudentTables, "StudentID", "Name", transferStdTable.StudentID);
            ViewBag.ClassID = new SelectList(db.ClassTables, "ClassID", "Name", transferStdTable.ClassID);


            return View(transferStdTable);
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";


            if (id != null)
            {
                TransferStdTable transferStdTable = db.TransferStdTables.Find(id);

              
                var Class = db.ClassTables.Find(transferStdTable.ClassID).Name;
                var studentAttendencyFormalDay = db.StudentAttendencyTables.Where(f => f.StudentID == id).Sum(f => f.FormalDays);


                var FrontImage = "";
                var BackImage = "";
                if (transferStdTable.FrontImage == null && transferStdTable.BackImage == null)
                {
                    FrontImage = "/Content/img/UserIcon.png";
                    BackImage = "/Content/img/UserIcon.png";
                }
                else
                {
                    FrontImage = transferStdTable.FrontImage.Substring(1);
                    BackImage = transferStdTable.BackImage.Substring(1);
                }

                string Date = ToShamsi(transferStdTable.Date);
                if (transferStdTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            transferStdTable.TransferStdID,
                            studentAttendencyFormalDay,
                            Date,
                            transferStdTable.FromSchool,
                            transferStdTable.ToSchool,
                            FrontImage,
                            BackImage,
                            transferStdTable.RollNo,
                            transferStdTable.Name,
                            transferStdTable.FatherName,
                          
                            Class,

                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, JsonRequestBehavior.AllowGet });
        }
        [Authorize(Roles = "admin,Admin,اډمین,ادمین")]
        public JsonResult ConDelete(int? id)
        {
            var msg = " ";
            var data = db.TransferStdTables.Find(id);
            db.TransferStdTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "TransferStdTables"), JsonRequestBehavior.AllowGet });
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

