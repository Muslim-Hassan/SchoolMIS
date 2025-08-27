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
using SchoolMIS.Models;

namespace SchoolMIS.Controllers
{
    public class StaffAttendencyTablesController : Controller
    {
        private SchoolMISEntities db = new SchoolMISEntities();


        // GET: UserTables
        [Authorize(Roles = "admin,Admin,teacher,Teacher,ادمین,اډمین,استاذ,استاد")]
        public ActionResult Index()
        {
            var staffAttendencyTables = db.StaffAttendencyTables.Include(s => s.StaffTable);
            return View(staffAttendencyTables.ToList());
        }

        // GET: UserTables/Details/5

        //[HttpPost]
        //public JsonResult Details(int? id)
        //{

        //    var staffAttendencyTable = db.StaffAttendencyTables.Find(id);

        //    var staff = db.StaffTables.Find(staffAttendencyTable.StaffID).Name;

        //    var date = String.Format("{0:MM/dd/yyyy}", staffAttendencyTable.SubmisionDate);

        //    if (staffAttendencyTable != null)
        //    {
        //        return Json(new
        //        {
        //            success = true,
        //            data = new
        //            {
        //                staffAttendencyTable.StaffAttendencyID,
        //                staffAttendencyTable.Month,
        //                staffAttendencyTable.FormalDays,
        //                staffAttendencyTable.Present,
        //                date,
        //                staff,

        //            }
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { success = false, message = "Record not found." }, JsonRequestBehavior.AllowGet);
        //}
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Create()
        {
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f=>f.IsActive==true), "StaffID", "Name");
            return View();
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StaffAttendencyTable staffAttendencyTable)
        {
            bool Exist = db.StaffAttendencyTables.Any(e => e.StaffID == staffAttendencyTable.StaffID
            && e.Month == staffAttendencyTable.Month
              && e.FormalDays == staffAttendencyTable.FormalDays
              && e.Present == staffAttendencyTable.Present
            && e.Date == staffAttendencyTable.Date);


            var msg = "";

            if (ModelState.IsValid)
            {
                //Exist record validation
                if (Exist)
                {
                    msg = "ستاسی معلومات په سیستم کی موجود دی";
                    return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                }

                //null data validation
                if (staffAttendencyTable.StaffAttendencyID == 0)
                {
                    if (staffAttendencyTable.FormalDays >= staffAttendencyTable.Present)
                    {
                        if (staffAttendencyTable.FormalDays <= 31 && staffAttendencyTable.FormalDays >= 29 && staffAttendencyTable.Present <= 31)
                        {
                            db.StaffAttendencyTables.Add(staffAttendencyTable);
                            db.SaveChanges();
                            msg = "معلومات په کامیابۍ سره داخل شول";
                            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            msg = "رسمی ورځی او  حاضری باید میاشتی  په تړاو  وی";
                            return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                    }
                    else
                    {
                        msg = "رسمی ورځی باید د حاضرو ورځو زیاتی یا مساوی وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                    }
                }
                else
                {
                    if (staffAttendencyTable.FormalDays >= staffAttendencyTable.Present)
                    {
                        if (staffAttendencyTable.FormalDays <= 31 && staffAttendencyTable.Present <= 31)
                        {
                            db.Entry(staffAttendencyTable).State = EntityState.Modified;
                            db.SaveChanges();
                            msg = ".ستاسی معلومات په کامیاۍ سره نوی شول";
                            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }
                        else
                        {
                            msg = "رسمی ورځی او  حاضری باید میاشتی په تړاو وی";
                            return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                        }

                    }
                    else
                    {
                        msg = "رسمی ورځی باید د حاضرو ورځو زیاتی یا مساوی وی";
                        return Json(new { success = false, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
                    }

                }
            }
            msg = " !مهربانۍ سره خانو معلومات پوره کړئ";
            return Json(new { success = false, msg, redirectUrl = Url.Action("Create", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
        }

        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StaffAttendencyTable staffAttendencyTable = db.StaffAttendencyTables.Find(id);

            if (staffAttendencyTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.StaffID = new SelectList(db.StaffTables.Where(f=>f.IsActive==true), "StaffID", "Name", staffAttendencyTable.StaffID);
 
            return View(staffAttendencyTable);
        }
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var msg = "";
            if (id != null)
            {
                StaffAttendencyTable staffAttendencyTable = db.StaffAttendencyTables.Find(id);

                var staff = db.StaffTables.Find(staffAttendencyTable.StaffID).Name;
                string date = Converssion.ToShamsi(staffAttendencyTable.Date);

                if (staffAttendencyTable != null)
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            staffAttendencyTable.StaffAttendencyID,
                            staffAttendencyTable.Month,
                            staffAttendencyTable.FormalDays,
                            staffAttendencyTable.Present,
                            date,
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
            var data = db.StaffAttendencyTables.Find(id);
            db.StaffAttendencyTables.Remove(data);
            db.SaveChanges();
            msg = " معلومات په کامیابۍ سره لري شول";
            return Json(new { success = true, msg, redirectUrl = Url.Action("Index", "StaffAttendencyTables"), JsonRequestBehavior.AllowGet });
        }
        //public ActionResult getid()
        //{
        //    ViewBag.StaffID = new SelectList(db.StaffTables, "StaffID", "Name");
        //    return View();
        //}
        //[Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult stfAttendency(int? staffID,  string month)
        {
            ViewBag.StaffID = new SelectList(db.StaffTables, "StaffID", "Name");
         
            if (month == null || month == "All")
            {

                var attendency = db.StaffAttendencyTables.Where(f => f.StaffID == staffID ).ToList();

                return View(attendency);
            }
            else
            {
                var attendency = db.StaffAttendencyTables.Where(f => f.StaffID == staffID && f.Month == month).ToList();

                return View(attendency);
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