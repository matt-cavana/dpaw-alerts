using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dpaw_alerts.Models;
using dpaw_alerts.Services;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize(Roles ="Admin")]
    public class ErrorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Errors
        public async Task<ActionResult> Index()
        {
            return View(await db.Errors.ToListAsync());
        }
               
    }
}
