using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;
using Backend.Database.Model;

namespace API.App_Start
{
    public class BasicHttpAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly List<int> _requiredRoles;
        public BasicHttpAuthorizeAttribute(params int[] requiredRoles)
        {
            _requiredRoles = new List<int>();
            foreach(var role in requiredRoles)
            {
                _requiredRoles.Add(role);
            }
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var ctx = new TaxiHeavenContext();
            AuthenticationHeaderValue auth = actionContext.Request.Headers.Authorization;
            if (string.Compare(auth.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string credentials = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(auth.Parameter));
                int separatorIndex = credentials.IndexOf(':');
                if (separatorIndex >= 0)
                {
                    string userName = credentials.Substring(0, separatorIndex);
                    string password = credentials.Substring(separatorIndex + 1);
                    var user = ctx.User.FirstOrDefault(x => x.Email == userName && x.Password == password);
                    if (user != null)
                    {
                        var roles = user.UserRole.Select(x => x.RoleId).ToList();
                        if (_requiredRoles.Except(roles).Count() != 0)
                        {
                            actionContext.RequestContext.RouteData.Values["Authorized"] = false;
                            return;
                        }
                        actionContext.RequestContext.RouteData.Values["Authorized"] = true;
                    }
                    else
                    {
                        actionContext.RequestContext.RouteData.Values["Authorized"] = false;
                    }
                }
            }
        }
    }
}