using SchoolMIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolMIS.Controllers
{
    [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
    public class marksReportController : Controller
    {
        SchoolMISEntities db=new SchoolMISEntities();
        [Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult marksByClass(int? classid,int? studentid )
        {
           var studentTotalMark = db.ExamTables.Where(f => f.ClassID == classid && db.StudentTables.Any(s=>s.StudentID==f.StudentID && s.ClassID==classid)).GroupBy(s => s.StudentID)
                .Select(g => new {StudentID=g.Key,TotalMarks=g.Sum(f=>f.AbtainScore)  })
                .OrderByDescending(x=>x.TotalMarks).ToList();

            var currentStudent = studentTotalMark.FirstOrDefault(x => x.StudentID == studentid);
            if (currentStudent != null) {

                int position = studentTotalMark.IndexOf(currentStudent) + 1;
                ViewBag.pos = position;

            }
            ViewBag.ClassList = new SelectList(db.ClassTables.ToList(), "ClassID", "Name");
            ViewBag.StudentList = new SelectList(db.StudentTables.Where(f=>f.ClassID==classid).ToList(), "StudentID", "Name");
       

            
           
            var student = db.StudentTables.Find(studentid);
            var Class = db.ClassTables.Find(classid);

            if (student == null)
            {
                student = new StudentTable()
                {
                    Image = "~/Content/images/logo.png",
                    ClassTable  = Class,

                };
              
            }
            var marks = db.ExamTables.Where(m => m.StudentID == studentid && m.ClassID==classid).ToList();


            var attendance = db.StudentAttendencyTables.Where(a => a.StudentID == studentid && a.ClassID==classid ).ToList();
            
            var viewModel = new marksModel
            {
                Student = student,
                Score = marks,
                Attendence = attendance,
              
               
            };

            return View("marksByClass", viewModel);
        
         
            
        }


        //[Authorize(Roles = "admin,Admin,ادمین,اډمین")]
        public ActionResult markSheet(int? studentId)
            {


            ViewBag.StudentList = new SelectList(db.StudentTables.ToList(), "StudentID", "Name");
            var student = db.StudentTables.Find(studentId);
            var marks = db.ExamTables.Where(m => m.StudentID == studentId).ToList();
           
            var attendance = db.StudentAttendencyTables.Where(a => a.StudentID == studentId).ToList();
            var prepTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "نرسری ټولګی" && f.StudentID == studentId);
            var firstTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "اول ټولګی" && f.StudentID == studentId);
            var secondTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "دوهم ټولګی" && f.StudentID == studentId);
            var  thirdTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "دریم ټولګی" && f.StudentID == studentId);
            var fourthTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "څلورم ټولګی" && f.StudentID == studentId);
            var fifthTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "پنځم ټولګی" && f.StudentID == studentId);
            var sixthTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "شپږم ټولګی" && f.StudentID == studentId);
            var seventhTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "اووم ټولګی" && f.StudentID == studentId);
            var eigthTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "اتم ټولګی" && f.StudentID == studentId);
           
            var ninthTotalMarks = db.ExamTables.DefaultIfEmpty().Where(f => f.ClassTable.Name == "نهم ټولګی" && f.StudentID == studentId);
            ViewBag.prepClassMarks = prepTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.prepPercentage = prepTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.firstClassMarks = firstTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.firstPercentage = firstTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.secondClassMarks= secondTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.secondPercentage= secondTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.thirdClassMarks= thirdTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.thirdPercentage= thirdTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.fourthClassMarks = fourthTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.fourthPercentage = fourthTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.fifthClassMarks = fifthTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.fifthPercentage = fifthTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.sixthClassMarks = sixthTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.sixthPercentage = sixthTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.seventhClassMarks = seventhTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.seventhPercentage = seventhTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.eigthClassMarks = eigthTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.eigthPercentage = eigthTotalMarks.Average(f => (double?)f.AbtainScore) * 2;
            ViewBag.ninthClassMarks = ninthTotalMarks.Sum(f => (long?)f.AbtainScore);
            ViewBag.ninthPercentage = ninthTotalMarks.Average(f => (double?)f.AbtainScore)*2;
            var viewModel = new marksModel
            {
                Student = student,
                Score = marks,
                Attendence = attendance
            };

            return View("markSheet", viewModel);

        }
        

           
        }
    }
