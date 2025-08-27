using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
//using SchoolMIS.App_Start;
using SchoolMIS.Models;

namespace SchoolMIS
{
    public class MvcApplication : System.Web.HttpApplication
    {
   

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
         
        }

        //public class customAuthorizationAttribute : AuthorizeAttribute
        //{
        //    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //    {
        //        if (filterContext.HttpContext.Request.IsAjaxRequest())
        //        {
        //            filterContext.Result = new JsonResult
        //            {
        //                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //                Data = new { success = false, message = "تاسو دی برخی ته پیزاندل شوی نه یاست" }
        //            };
        //        }
        //        else
        //        {
        //            string script = "<script>alert('you are not authorize');window.history.back();</script>";
        //            filterContext.Result = new ContentResult
        //            {
        //                Content = script,
        //                ContentType = "text/html"
        //            };
        //        }
        //    }
        //}


        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies["AuthCookie"];
            if (authCookie != null)
            {
                var encodeUserName = authCookie.Value;
                var username =HttpUtility.UrlDecode(encodeUserName);
                using (var db = new SchoolMISEntities())
                {
                    var user = db.UserTables.FirstOrDefault(u => u.UserName == username);
                    if(user != null)
                    {
                        var roles = new string[] { user.UserTypeTable.Type.ToString() }; // assuming navigation property
                        var customUser = new CustomPrincipal(user.UserName) { Roles = roles };
                        HttpContext.Current.User = customUser;
                    }
                }
            }

        }


    }
}
