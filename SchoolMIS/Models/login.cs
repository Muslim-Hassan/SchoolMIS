using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolMIS.Models
{
    public class login
    {

        [Required(ErrorMessage = "نوم داخلول مهم دی")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "کوډ داخلول مهم دی")]
        public string Password { get; set; }
        public int UserTypeID { get; set; }
   
    }
}