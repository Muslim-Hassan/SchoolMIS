using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolMIS.Models
{
    public class marksModel
    {
        public StudentTable Student { get; set; }
         public List<ExamTable> position { get; set; }
        public List<ExamTable> Score { get; set; }
        public List<StudentAttendencyTable> Attendence { get; set; }

     
    }
}