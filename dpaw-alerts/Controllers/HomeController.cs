using dpaw_alerts.Models;
using dpaw_alerts.Services;
using dpaw_alerts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqToTwitter;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net.Mail;
using Twilio;
using System.Net;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;

namespace dpaw_alerts.Controllers
{

    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private DateTime exD = DateTime.Now;
        [AllowAnonymous]
        public ActionResult Index()
        {
            //redirect authenticated user to alert portal
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Alerts");
            }

            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var alerts = db.Alerts.Where(a => a.StartDate >= exD || a.EndDate >= exD || a.EndDate == null && a.Published == "Yes" && !a.alertType.Name.Contains("NP")).OrderByDescending(a => a.CreateDate).ToList();

            //set alert type for the search panel
            ViewBag.aType = db.AlertTypes.Where(a => a.Status == "Active" && !a.Name.Contains("NP")).ToList();
            return View(alerts);
        }

        [Throttle(Name = "TestThrottle", Message = "You must wait {n} seconds before accessing this url again.", Seconds = 5)]
        [AllowAnonymous]
        public ActionResult TestThrottle()
        {
            return Content("TestThrottle executed");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(string alType)
        {
            //redirect authenticated user to alert portal
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Alerts");
            }

            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var alerts = db.Alerts.Where(a => a.StartDate >= exD || a.EndDate >= exD || a.EndDate == null && a.Published == "Yes" && !a.alertType.Name.Contains("NP")).OrderByDescending(a => a.CreateDate).ToList();

            //set alert type for the search panel
            ViewBag.aType = db.AlertTypes.Where(a => a.Status == "Active").ToList();
            return View(alerts);
        }

        [AllowAnonymous]
        public PartialViewResult Help(string atype)
        {

            return PartialView();
        }

        [AllowAnonymous]
        public PartialViewResult ListView(string atype)
        {
            string[] atypes = { };
            //convert to array if atype has value
            if (atype != null)
            {
                atypes = atype.Split(',');
            }

            // atype = new string[] { "crocodile-alerts","riverpark-alerts" };
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            //var alerts = db.Alerts.Where(a => a.StartDate >= exD || a.EndDate >= exD || a.EndDate == null && a.Published == "Yes" && a.alertType.Slug=="crocodile-alert").OrderByDescending(a => a.CreateDate).ToList();
            var alerts1 = from a in db.Alerts
                          where a.Published == "Yes" && !a.alertType.Name.Contains("NP") && atypes.Contains(a.alertType.Slug) && (a.StartDate >= exD || a.EndDate >= exD || a.EndDate == null) 
                          select a;

            return PartialView(alerts1.ToList());
        }


        // GET: Locations/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Location location = await db.Locations.FindAsync(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return PartialView("_details", location);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ViewAlert(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alert alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return HttpNotFound();
            }
            return PartialView("_viewAlert", alert);
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAlert(string id)
        {
            Guid x;
            bool isValid = Guid.TryParse(id, out x);
            if (!isValid)
            {
                return Json(new { status = false, message = "No valid parameter provided." }, JsonRequestBehavior.AllowGet);
            }

            if (id == null)
            {
                return Json(new { status = false, message = "No valid parameter provided." }, JsonRequestBehavior.AllowGet);
            }
            var q = (from a in db.Alerts
                     join p in db.Locations on a.AlertId equals p.AlertId
                     into ps
                     from p in ps.DefaultIfEmpty()
                     where a.AlertId == x && a.Published == "Yes"
                     //orderby p.Name ascending
                     select new { a.Title, a.Description, p.Latitude, p.Longitude, a.AlertId, p.Name, a.alertType.IconUrl })
                     .OrderBy(l => l.Name);
            if (q == null)
            {
                return Json(new { status = false, message = "No alerts found with with supplied id." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, message = q }, JsonRequestBehavior.AllowGet);
        }

        // GET: Parks
        [HttpGet]
        [AllowAnonymous]
        public ActionResult map(string atype)
        {
            string[] atypes = { };
            //convert to array if atype has value
            if (atype != null)
            {
                atypes = atype.Split(',');
            }

            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var q = (from a in db.Alerts
                     join p in db.Locations on a.AlertId equals p.AlertId
                     into ps
                     from p in ps.DefaultIfEmpty()
                     where a.Published == "Yes" && !a.alertType.Name.Contains("NP") && atypes.Contains(a.alertType.Slug) && (a.EndDate >= exD || a.EndDate >= exD || a.EndDate == null) 
                     //orderby p.Name ascending
                     select new { a.Title, a.Description, p.Latitude, p.Longitude, a.AlertId, p.Name, a.alertType.IconUrl })
                     .OrderBy(l => l.Name);

            //  return PartialView("_map", q.ToList());

            return Json(q, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult JsonDfes()
        {
            string xmlData = System.Configuration.ConfigurationManager.AppSettings["dfesRSS"];
            // string xmlData = "https://www.emergency.wa.gov.au/data/incident_FCAD.rss";// "http://education.whispir.com/public/723/incident_FCAD.rss";
            XMLReader readXML = new XMLReader();
            var data = readXML.RetrunListOfFeeds(xmlData);
            // return View(data.ToList());
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ERSS()
        {
            // string xmlData = "https://www.emergency.wa.gov.au/data/incident_FCAD.rss";// "http://education.whispir.com/public/723/incident_FCAD.rss";
            string xmlData = System.Configuration.ConfigurationManager.AppSettings["dfesRSS"];
            XMLReader readXML = new XMLReader();
            var data = readXML.RetrunListOfFeeds(xmlData);
            // return View(data.ToList());
            return PartialView("_ERSS", data.ToList());
        }

        [AllowAnonymous]
        public ActionResult Unsubscribe(Guid? token, string email)
        {
            Subscriber subscriber = new Subscriber();
            subscriber = db.Subscribers.Where(s => s.Id == token && s.Email == email).FirstOrDefault();
            if (token == null || email == null)
            {
                TempData["Message"] = String.Format("You must enter an email address and a valid token to update the subscription list!", email);
                return View();
            }

            if (subscriber == null)
            {
                TempData["Message"] = String.Format("<span class='red'> {0} </span> has not been found in the mailing list!", email);
                //return HttpNotFound();
                return View();
            }

            return View(subscriber);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unsubscribe([Bind(Include = "SubscriptionType, Email, Id")] Subscriber subscriber)
        {


            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string queryString = "Update Subscribers set SubscriptionType=@sType where Id=@Id and Email=@email;";

            using (var connection = new SqlConnection(connString))
            {
                var command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.AddWithValue("Id", subscriber.Id);
                command.Parameters.AddWithValue("email", subscriber.Email);
                command.Parameters.AddWithValue("sType", subscriber.SubscriptionType);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {

                    TempData["Message"] = String.Format("<span class='red'> {0} </span> has been successfully unsubscribed from the mailing list!", subscriber.Email);

                }
                else
                {
                    TempData["Message"] = String.Format("Error: <span class='red'> {0} </span> cannot be updated in the mailing list, please contact admin!", subscriber.Email);
                }


            }


            ModelState.AddModelError("", "Sorry we cannot update your subscription status, please contact admin.");

            return View(subscriber);
        }


        

        public ActionResult kmlmap()
        {
           

            return View();
        }

      

        public ActionResult Alert()
        {

            return View();
        }

        [ChildActionOnly]
        [OutputCache(Duration =600)]
        [AllowAnonymous]
        public ActionResult Footer()
        {

            return PartialView("_footer");
        }
        
        
        [AllowAnonymous]
        public async Task<ActionResult> Share(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alert alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return HttpNotFound();
            }

            return PartialView("_share", alert);
        }

    }
}