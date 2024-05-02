using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Services
{
    public class HangfireAuthorization: Hangfire.Dashboard.IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            // return context.Authentication.User.Identity.IsAuthenticated;

            // Only admin users can access the dashboard
            var user = new OwinContext(owinEnvironment).Authentication.User;
            return user.Identity.IsAuthenticated && user.IsInRole("Admin");
        }
    }
}