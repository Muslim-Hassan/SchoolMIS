using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SchoolMIS.Models
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public string[] Roles { get; set; }

        public CustomPrincipal(string name)
        {
            this.Identity = new GenericIdentity(name);
        }

        public bool IsInRole(string role)
        {
            return Roles != null && Roles.Contains(role);
        }
    }
}




