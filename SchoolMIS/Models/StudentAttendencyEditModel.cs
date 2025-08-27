using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolMIS.Models
{
    public class StudentAttendencyEditModel
    {
        public int StudentAttendencyID { get; set; }
        public int StudentID { get; set; }
        public int StaffID { get; set; }
        public int Present { get; set; }
        public Nullable<long> FormalDays { get; set; }
        public string Month { get; set; }
        public int ClassID { get; set; }
        public string Date { get; set; }

        public virtual ClassTable ClassTable { get; set; }
        public virtual StaffTable StaffTable { get; set; }
        public virtual StudentTable StudentTable { get; set; }
    }

   public class StudentEditModel
    {
        public int StudentID { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string GrandFatherName { get; set; }
        public string Gender { get; set; }
        public long IdentityID { get; set; }
        public string Nationality { get; set; }
        public string FatherJob { get; set; }
        public string NativeLanguage { get; set; }
        public string Brother { get; set; }
        public string Uncle { get; set; }
        public Nullable<long> Contact { get; set; }
        public int ClassID { get; set; }
        public string CurrentAddress { get; set; }
        public string PermenentAddress { get; set; }
        public string StudentType { get; set; }
        public string Image { get; set; }
        public int RollNO { get; set; }
        public string DOB { get; set; }
        public string RegistrationDate { get; set; }
    }

    public class AdvancementEditModel
    {
        public int AdvancementID { get; set; }
        public int FromClassID { get; set; }
        public int ToClassID { get; set; }
        public int StudentScore { get; set; }
        public int StaffID { get; set; }
        public Nullable<int> StudentID { get; set; }
        public string Date { get; set; }

        public virtual ClassTable ClassTable { get; set; }
        public virtual StaffTable StaffTable { get; set; }
        public virtual StudentTable StudentTable { get; set; }
        public virtual ClassTable ClassTable1 { get; set; }
    }
}