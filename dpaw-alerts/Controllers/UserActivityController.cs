using dpaw_alerts.Models;
using dpaw_alerts.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class UserActivityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserActivity
        public ActionResult Index()
        {
            var ac=  db.UserActivities.OrderByDescending(a => a.ActionDate).ToList();

            return View(ac);
        }
    }
}