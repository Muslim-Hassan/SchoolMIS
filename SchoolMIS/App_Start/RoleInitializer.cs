//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using SchoolMIS.Models; // Change to match your namespace

//namespace SchoolMIS.App_Start
//{
//    public class RoleInitializer
//    {
//        public static void Initialize(SchoolMISEntities context)
//        {
//            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
//            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

//            string[] roles = { "Admin", "admin", "Teacher", "teacher", "استاذ", "اډمین", "student", "Student", "شاګرد" };
//            foreach (var role in roles)
//            {
//                if (!roleManager.RoleExists(role))
//                    roleManager.Create(new IdentityRole(role));
//            }

//            var adminName = "Zaheer";
//            var adminUser = userManager.FindByName(adminName);

//            if (adminUser == null)
//            {
//                adminUser = new ApplicationUser
//                {
//                    UserName = adminName,
                   
//                };

//                var result = userManager.Create(adminUser, "18559");
//                if (result.Succeeded)
//                    userManager.AddToRole(adminUser.Id, "Admin");
//            }
//        }
//    }
//}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;


//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

//using SchoolMIS.Models; // <-- Replace with your actual namespace

//namespace SchoolMIS.App_Start // <-- Match your project namespace
//{
//    public class RoleInitializer
//    {
//        public static void Initialize(SchoolMISEntities db)
//        {
//            // Create Role and User Managers
//            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
//            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

//            // Create roles if they don't exist
//            string[] roles = { "Admin", "admin", "Teacher", "teacher", "استاذ", "اډمین", "student", "Student", "شاګرد" };
//            foreach (var role in roles)
//            {
//                if (!roleManager.RoleExists(role))
//                    roleManager.Create(new IdentityRole(role));
//            }

//            // Create default admin user if not exists
//            var adminName = "Zaheer";
//            var adminUser = userManager.FindByName(adminName);
//            if (adminUser == null)
//            {
//                adminUser = new ApplicationUser
//                {
//                    UserName = adminName

//                };

//                var result = userManager.Create(adminUser, "18559"); // Default password
//                if (result.Succeeded)
//                {
//                    userManager.AddToRole(adminUser.Id, "Admin");
//                }
//            }
//        }
//    }
//}
