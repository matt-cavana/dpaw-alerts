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
using System.Data.Entity.Core;
using dpaw_alerts.Services;
using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ScheduleJobsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ScheduleJobs
        public async Task<ActionResult> Index()
        {
            var scheduleJobs = db.ScheduleJobs.Include(s => s.alert).OrderByDescending(s=>s.ExecutionDate);
            return View(await scheduleJobs.ToListAsync());
        }

        // GET: ScheduleJobs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleJob scheduleJob = await db.ScheduleJobs.FindAsync(id);
            if (scheduleJob == null)
            {
                return HttpNotFound();
            }
            return View(scheduleJob);
        }

        // GET: ScheduleJobs/Create
        public ActionResult Create()
        {
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title");
            return View();
        }

        // POST: ScheduleJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AlertId,channel,ExecutionDate,Status,CompletedDate,CreateDate")] ScheduleJob scheduleJob)
        {
            if (ModelState.IsValid)
            {
                db.ScheduleJobs.Add(scheduleJob);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", scheduleJob.AlertId);
            return View(scheduleJob);
        }

        // GET: ScheduleJobs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleJob scheduleJob = await db.ScheduleJobs.FindAsync(id);
            if (scheduleJob == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", scheduleJob.AlertId);
            return View(scheduleJob);
        }

        // POST: ScheduleJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AlertId,channel,ExecutionDate,Status,CompletedDate,CreateDate")] ScheduleJob scheduleJob)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduleJob).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", scheduleJob.AlertId);
            return View(scheduleJob);
        }



        //delete the record inline
        // POST: ScheduleJobs/Delete/5
        [HttpPost, ActionName("Delete")]

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { message = "No records found!", JsonRequestBehavior.AllowGet });
            }
            ScheduleJob scheduleJob = await db.ScheduleJobs.FindAsync(id);
            db.ScheduleJobs.Remove(scheduleJob);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
       public JsonResult ExecuteJob(Guid aid)
        {

            var appSettings = ConfigurationManager.AppSettings;
            string burl = appSettings["BaseUrl"];

            var job = db.ScheduleJobs.Where(a => a.Channel == "Email" && a.Status == "Pending" && a.AlertId == aid).FirstOrDefault();
            var subscribers = db.Subscribers.Where(a => a.Status == "Active" && a.SubscriptionType == "Email" || a.SubscriptionType == "Email-SMS").ToList();
            var template = db.EmailTemplates.FirstOrDefault();


            if (job == null) return Json(new { success = false, message = "No active email job found." });


            //bind other variables
            string park = "<h3>Affected Parks/Locations</h3><p><ul>";
            string loc = "";
            foreach (var pk in job.alert.Location)
            {
                loc = loc + ("<li>" + pk.Name + "</li>");
            }

            park = park + loc + "</ul></p>";
            string asites = string.Format("<p><strong>Affected sites:</strong> {0} </p>", job.alert.SitesAffected);
            string uri = string.Format("<p><strong>Website link:</strong> {0} </p>", job.alert.WebLink);
            var link = string.Format("<p>View alert at <a href='{1}/Home/Index#{0}'>{1}/Home/Index#{0}</a></p>", job.AlertId, burl);

            int tot = 0;

            foreach (var subsriber in subscribers)
            {
                var unsubsribe = string.Format("<p>To unsubscribe from the mailing list <a href='{2}/Home/unsubscribe?token={0}&email={1}'> click here</a></p>", subsriber.Id, subsriber.Email, burl);
                MailHelper.SendMailMessage(template.From, subsriber.Email, "", "", template.From, job.alert.Title, template.HeaderSection + job.alert.Description + park + asites + uri + link + template.FooterSection + unsubsribe);
                

                tot++;

            }

            job.Status = "Complete";
            job.CompletedDate = DateTime.Now;
            job.Response = "Email sent to " + tot.ToString() + " subscriber/s";// responseTweet.Text.ToString();
            db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            string st = "Executed at " + DateTime.Now.ToString();
            string msg = st + ". Email sent to " + tot.ToString() + " subscriber/s";


            return Json(new { success = true, message = msg });
           
        }
    


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
