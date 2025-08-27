using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolMIS.Models
{
    public class timeTable
    {
        public string Day { get; set; }
        public List<string> subject { get; set; }
        public Dictionary<string, string> TimeSubjectMapping { get; set; }

    }
}