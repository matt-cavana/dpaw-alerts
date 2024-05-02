using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
   
    #region User Classes
    public class AlertsUserStore : UserStore<ApplicationUser>
    {
        public AlertsUserStore()
            : this(new ApplicationDbContext())
        { }

        public AlertsUserStore(ApplicationDbContext context)
            : base(context)
        { }
    }

    public class ColsUserManager : UserManager<ApplicationUser>
    {
        public ColsUserManager()
            : this(new AlertsUserStore())
        { }

        public ColsUserManager(UserStore<ApplicationUser> userStore)
            : base(userStore)
        { }
    }
    #endregion
}