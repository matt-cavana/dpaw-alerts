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
using dpaw_alerts.ViewModels;
using dpaw_alerts.Services;
using System.IO;
using Facebook;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity.Core;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;
using Hangfire;
using System.Transactions;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize]
    public class AlertsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DateTime exD = DateTime.Now;
        private string msg = "";
        // GET: Alerts
        public ActionResult Index()
        {
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var alerts = db.Alerts.Where(a => a.StartDate >= exD || a.EndDate >= exD || a.EndDate == null).OrderByDescending(a => a.CreateDate).ToList();
            
            //check only for admin user only
            if(User.IsInRole("Admin"))
            {
                DateTime dt = DateTime.Now.AddDays(+7);
                var fb = db.SocialNetworks.Where(a => a.Channel == "Facebook" && a.ExpiryDate <=dt ).FirstOrDefault();
                if(fb!=null)
                {
                    ViewBag.fbmsg = string.Format("The Facebook token will expire on {0}.", fb.ExpiryDate.ToString());
                }
            }

            return View(alerts);
        }

        public ActionResult Archived()
        {
            var alerts = db.Alerts.Where(a => a.EndDate < DateTime.Now && a.StartDate < exD ).ToList();

            return View(alerts);
        }


        // GET: Alerts/Details/5
        public async Task<ActionResult> Details(Guid id)
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
            return View(alert);
        }

        // GET: Alerts/Create
        public ActionResult Create()
        {

            if(!Authorize.canCreate())
            {
                //set the browser message               
                TempData["Message"] = "Error: Your current user permission does not allow you to create a post. Please contact system admin to review your permission.";
                return RedirectToAction("Index", "Alerts");
            }
            // return View();
            // Guid id = Guid.NewGuid(); 
            var alert = new Alert();
            Guid usr = Guid.Parse(User.Identity.GetUserId());
            int aid = db.AlertTypes.Where(a => a.userAlertType.Any(c => c.UserId == usr) && a.Status=="Active").Select(m => m.AId).FirstOrDefault();
            
            //bind some values

            alert.Title = "Enter title";
            alert.Description = "Enter description";
            alert.StartDate = DateTime.Now.AddMinutes(5);
            alert.Facebook = "No";
            alert.Twitter = "No";
            alert.SMS = "No";
            alert.PushNotification = "No";
            alert.Email = "No";
            alert.Published = "No";
            alert.CreatedBy = User.Identity.Name;
            //alert.EndDate = DateTime.Now.AddMinutes(20);
            alert.AId = aid;

            db.Alerts.Add(alert);
            db.SaveChanges();

            //log the user activity
            var id = alert.AlertId;
            LogError.LogUserActivity(User.Identity.Name, string.Format("New alert (id={0})  created", id), "New Alert");

            return RedirectToAction("Edit", new { id = alert.AlertId, tp="new" });
        }

        // POST: Alerts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AlertId,Title,Description,StartDate,SitesAffected,Facebook,SMS,Twitter,PushNotification,Email,WebLink,CreateDate,CreatedBy")] Alert alert)
        {
            if (ModelState.IsValid)
            {
                db.Alerts.Add(alert);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(alert);
        }

        public ActionResult Copy( Guid id)
        {
            if (!Authorize.canCopy(id))
            {
                //set the browser message               
                TempData["Message"] = "Error: You cannot copy any unpublished alert.";
                return RedirectToAction("Index", "Alerts");

            }
            var oldAlert = db.Alerts.Find(id);

            var alert = new Alert();

            alert.Title = oldAlert.Title;
            alert.Description = oldAlert.Description;
            alert.StartDate = DateTime.Now.AddMinutes(5);
            alert.CreatedBy = User.Identity.Name;
            alert.Facebook = "No";
            alert.Twitter = "No";
            alert.SMS = "No";
            alert.PushNotification = "No";
            alert.Email = "No";
            alert.Published = "No";
            alert.AId = oldAlert.AId;

            db.Alerts.Add(alert);
            db.SaveChanges();

            //log the user activity
            var nid = alert.AlertId;
            LogError.LogUserActivity(User.Identity.Name, string.Format("New alert (id={0})  created", nid), string.Format("Copy Alert ({0})", id));

            return RedirectToAction("Edit", new { id = alert.AlertId });
        }

        // GET: Alerts/Edit/5
        [Route("Alerts/Edit/{id}")]
        [CustomAuthorize(Roles = "Manager, Admin")]
        public ActionResult Edit(Guid id, string tp)
        {
            ViewBag.RPrkId = new SelectList(db.Parks.Where(a => a.Status == "Active"), "RPrkId", "Name");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(!Authorize.canEdit(id))
            {
                //set the browser message               
                TempData["Message"] = "Error: You cannot edit this alert, this alert does not meet the editing criteria.";
                return RedirectToAction("Index", "Alerts");

            }

            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            int min = Convert.ToInt32( exD.Ticks % TimeSpan.TicksPerMinute);
            
            var job = db.ScheduleJobs.Where(a => a.Channel == "SMS" && a.Status == "Pending" && DbFunctions.AddSeconds(a.ExecutionDate, -a.ExecutionDate.Minute) == exD).FirstOrDefault();

            Alert alert = db.Alerts.Find(id);
           
            if(tp !=null && tp=="new")
            {
                alert.AId = -1;
                alert.Title = "";
                alert.Description = "";
            }

            // //alternate solution
            //ViewBag.AId =
            // db.AlertTypes
            // .OrderBy(g => g.Name)
            // .AsEnumerable()
            // .Select(g => new SelectListItem
            // {
            //     Text = g.Name,
            //     Value = g.AId.ToString(),
            //     Selected = alert.AId == g.AId
            // });

            //filter the alert type based on user permission
            Guid usr = Guid.Parse(User.Identity.GetUserId());
            var at = db.AlertTypes.Where(a=>a.userAlertType.Any(c=>c.UserId==usr) && a.Status=="Active");
            
            ViewBag.AId = new SelectList(at, "AId", "Name", alert.AId);
            if (alert == null)
            {
                return HttpNotFound();
            }
            return View(alert);
        }

        // POST: Alerts/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [MultipleButton(Name = "action", Argument = "Edit")]
        [CustomAuthorize(Roles = "Manager, Admin")]
        public ActionResult Edit([Bind(Include = "AlertId,Title,Description,StartDate,EndDate,SitesAffected,Facebook,SMS,Twitter,PushNotification,Email,WebLink,CreateDate,CreatedBy,UpdateDate,UpdatedBy,AId,Published, PubImmediately")] Alert alert, string Publish)
        {
            
            //filter the alert type based on user permission
            Guid usr = Guid.Parse(User.Identity.GetUserId());
            var at = db.AlertTypes.Where(a => a.userAlertType.Any(c => c.UserId == usr) && a.Status=="Active");

            string message = "";

            ViewBag.AId = new SelectList(at, "AId", "Name", alert.AId);
            
            if (ModelState.IsValid)
            {
                //implement transaction to avoid data loss
                //using (TransactionScope tran = new TransactionScope())
                //{

                    try
                    {


                        if (alert.PubImmediately)
                        {                   
                            alert.StartDate = DateTime.Now;
                        }
                        else if(alert.StartDate<DateTime.Now)
                        {
                            ModelState.AddModelError("", "Start date and time must be later than the current date and time.");
                            return View(alert);
                        }

                        if (alert.EndDate < DateTime.Now)
                        {
                            ModelState.AddModelError("", "End date and time must be later than the current date and time.");
                            return View(alert);
                        }
                        else if (alert.StartDate >= alert.EndDate)
                        {
                            ModelState.AddModelError("", "End date must be later than the start date.");
                            return View(alert);
                        }

                        if (!string.IsNullOrWhiteSpace(Publish))
                        {
                            alert.Published = "Yes";
                            var loc = db.Locations.Where(a => a.AlertId == alert.AlertId).Count();

                            //check park or location before publish
                            if (loc == 0)
                            {
                                ModelState.AddModelError("", "You need to add a park or location before you publish this alert.");
                                return View(alert);
                            }
                    
                        }
                        else
                        {
                            alert.Published = "No";
                            message = "The record has been saved successfully.";
                        }

                        //remove the existing schedule jobs and create new
                        if (alert.Published=="Yes")
                        {
                        //DateTime dt = alert.StartDate.AddSeconds(-alert.StartDate.Second);
                        DateTime dt = alert.StartDate.Truncate(TimeSpan.FromMinutes(1));

                                if (removeJobs(alert.AlertId))
                                {

                                    var j = new ScheduleJob();
                                    if (alert.Facebook == "Yes")
                                    {
                                        j.Channel = "Facebook";
                                        j.Status = "Pending";
                                        j.AlertId = alert.AlertId;
                                        j.ExecutionDate = dt.AddMinutes(2);
                                        //create a schedule job
                                        msg = CreateJob(j);

                                    }
                                    if (alert.Twitter == "Yes")
                                    {
                                        j.Channel = "Twitter";
                                        j.Status = "Pending";
                                        j.AlertId = alert.AlertId;
                                        j.ExecutionDate = dt.AddMinutes(2);
                                        //create a schedule job
                                        msg = CreateJob(j);
                                    }

                                    if (alert.Email == "Yes")
                                    {
                                        j.Channel = "Email";
                                        j.Status = "Pending";
                                        j.AlertId = alert.AlertId;
                                        j.ExecutionDate = dt.AddMinutes(2);
                                        //create a schedule job
                                        msg = CreateJob(j);
                                    }

                                    if (alert.SMS == "Yes")
                                    {
                                        j.Channel = "SMS";
                                        j.Status = "Pending";
                                        j.AlertId = alert.AlertId;
                                        j.ExecutionDate = dt.AddMinutes(2);
                                        //create a schedule job
                                        msg = CreateJob(j);
                                    }

                            
                            }
                        }

                alert.UpdateDate = DateTime.Now;
                alert.UpdatedBy = User.Identity.Name;
               
                if (alert.Description.Length < 100)
                {
                    ModelState.AddModelError("", "The description is too short. Please make sure you entered the description correctly. At least 200 characters");
                    return View(alert);
                }

                
                db.Entry(alert).State = EntityState.Modified;
                //exclude columns from modification
                db.Entry(alert).Property(x => x.CreateDate).IsModified = false;
                //db.Entry(alert).Property(x => x.Published).IsModified = false;
                db.Entry(alert).Property(x => x.CreatedBy).IsModified = false;
                db.SaveChanges();

                //redirect to home page once publish button clicked and saved
                if (!string.IsNullOrWhiteSpace(Publish))
                {
                    TempData["Message"] = "The alert has been published successfully." +msg;
                    return RedirectToAction("Index", "Home");
                }
                //set the browser message               
                TempData["Message"] = message + msg;

                //complete the transaction
                // tran.Complete();

            }

            catch (Exception e)
            {

                TempData["Message"] = "An error has occured while processing your request. Error -" + e.ToString();
                return View(alert);
            }

            //return View(alert);
            //return View(alert);
            //}

         }
            return View(alert);
        }

       

        [HttpPost]
        // [MultipleButton(Name = "action", Argument = "Publish")]
        [CustomAuthorize(Roles = "Manager, Admin")]
        public JsonResult Publish(Guid id, Alert alert)
        {
            //remove the tempdata 
            TempData.Remove("Message");
           // Alert alert = db.Alerts.Find(id);
            var loc = db.Locations.Where(a => a.AlertId == id).Count();
            
            if (alert == null)
            {
              return  Json(new { status = false, message = "<div class='alert alert-danger'>Sorry cannot publish this record.</div>" });
            }

            if(alert.StartDate<=DateTime.Now)
            {

               return Json(new { status = false, message = "<div class='alert alert-danger'>Error: Start date must be greater than current date and time.</div>" });
            }
            //check park or location before publish
            if (loc==0 )
            {
              return  Json(new { status = false, message = "<div class='alert alert-danger'>Error: You need to add a park or location before you publish this alert.</div>" });
            }

            db.Entry(alert).State = EntityState.Modified;
            //exclude columns from modification
            db.Entry(alert).Property(x => x.CreateDate).IsModified = false;
            db.Entry(alert).Property(x => x.Published).IsModified = false;
            db.Entry(alert).Property(x => x.CreatedBy).IsModified = false;
            db.SaveChanges();

            //set the browser message               
            TempData["Message"] = "The record has been updated successfully. " + msg;


            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string queryString = "Update Alerts set Published='Yes' where AlertId=@AlertId;";
           
            using (var connection = new SqlConnection(connString))
            {
                var command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.AddWithValue("AlertId", id);

                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected>0)
                {
                    //DateTime dt = alert.StartDate.AddSeconds(-alert.StartDate.Second);
                    DateTime dt = alert.StartDate.Truncate(TimeSpan.FromMinutes(1));
                    //remove the existing schedule jobs and create new
                    if (removeJobs(id))
                    {
                        
                        var j = new ScheduleJob();
                            if (alert.Facebook == "Yes")
                            {
                                j.Channel = "Facebook";
                                j.Status = "Pending";
                                j.AlertId = alert.AlertId;
                                j.ExecutionDate = dt.AddMinutes(2);
                                //create a schedule job
                                msg = CreateJob(j);

                            }
                            if (alert.Twitter == "Yes")
                            {
                                j.Channel = "Twitter";
                                j.Status = "Pending";
                                j.AlertId = alert.AlertId;
                                j.ExecutionDate = dt.AddMinutes(2);
                                //create a schedule job
                                msg = CreateJob(j);
                            }

                            if (alert.Email == "Yes")
                            {
                                j.Channel = "Email";
                                j.Status = "Pending";
                                j.AlertId = alert.AlertId;
                                j.ExecutionDate = dt.AddMinutes(2);
                                //create a schedule job
                                msg = CreateJob(j);
                            }

                            if (alert.SMS == "Yes")
                            {
                                j.Channel = "SMS";
                                j.Status = "Pending";
                                j.AlertId = alert.AlertId;
                                j.ExecutionDate = dt.AddMinutes(2);
                                //create a schedule job
                                msg = CreateJob(j);
                            }

                    }
                    //create a job schedule
                   // BackgroundJob.Schedule(() => HangfireScheduleJobs.EmailJob(), dt.AddMinutes(2));
                   //dt= dt.AddMinutes(2);
                    //BackgroundJob.Schedule(() => HangfireScheduleJobs.EmailJob(), new DateTime(dt.Year, dt.Month, dt.Day, dt.Minute,00,00));

                    return Json(new { status = true, message = "<div class='alert alert-success'>The alert has been published successfully.</div>" });
                }
         

            }
            return Json(new { status = false, message = "<div class='alert alert-danger'>The alert cannot be published.</div>" });
        }


        [HttpPost]
        public JsonResult Unpublish(Guid id)
        {
            Alert alert = db.Alerts.Find(id);
           
            if (alert == null)
            {
                Json(new { status = false, message = "<div class='alert alert-danger'>Sorry cannot unpublish this record.</div>" });
            }
           
            
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string queryString = "Update Alerts set Published='No',  EndDate=@EndDate where AlertId=@AlertId;";

            using (var connection = new SqlConnection(connString))
            {
                var command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.AddWithValue("AlertId", id);
                command.Parameters.AddWithValue("EndDate", DateTime.Now);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    //remove the existing schedule jobs and create new
                    removeJobs(id);
                   
                    return Json(new { status = true, message = "<div class='alert alert-success'>The alert has been unpublished and all the future jobs has been deleted successfully.</div>" });
                }


            }
            return Json(new { status = false, message = "<div class='alert alert-danger'>The alert cannot be unpublished, contact system admin.</div>" });
        }


        //create schedule job
        protected string CreateJob(ScheduleJob job)
        {
            
            try
            {
                
                //create a schedule job record
                db.ScheduleJobs.Add(job);
                int result = db.SaveChanges();
                if (result > 0)
                {
                    msg = "The schedule job has been created successfully!";
                }
                
            }
            catch (UpdateException ex)
            {
                msg = "Sorry cannot create the schedule job, Error: " + ex.ToString();

                //create a log if failed to create a schedule job
                LogError.log("Create Alert", ex.ToString());
            }

            //if (cjob > 0)
            //{
            //    success = true;

            //}
            //else
            //{
            //    success = false;
            //}

            return msg;
        }
        

        public bool removeJobs(Guid alertId)
        {
            bool success = false;
            //remove all the previous jobs first
            var jbs = db.ScheduleJobs.Where(a => a.AlertId == alertId && a.Status=="Pending").ToList();

            try
            {
                foreach (var jb in jbs)
                {
                    db.ScheduleJobs.Remove(jb);
                }
                db.SaveChanges();
                //return true
                success = true;

            }
            catch
            {
                //return true
                success = false;
            }
           

            return success;
        }
        
        
        // GET: Parks
        [HttpGet]
        public ActionResult map()
        {
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var q = (from a in db.Alerts
                     join p in db.Locations on a.AlertId equals p.AlertId //into ps
                    // from p in ps.DefaultIfEmpty()
                     where a.EndDate >= exD || a.EndDate >= exD || a.EndDate == null
                     select new { a.Title, a.Description, p.Latitude, p.Longitude, a.AlertId, p.Name, a.alertType.IconUrl })
                     .OrderBy(l=>l.Name);
                     
            //  return PartialView("_map", q.ToList());

            return Json(q, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult JsonDfes()
        {
            // string xmlData = "http://education.whispir.com/public/723/incident_FCAD.rss";
            string xmlData = System.Configuration.ConfigurationManager.AppSettings["dfesRSS"];
            XMLReader readXML = new XMLReader();
            var data = readXML.RetrunListOfFeeds(xmlData);
            // return View(data.ToList());
            return Json(data.ToList(),JsonRequestBehavior.AllowGet);
        }

        // GET: Parks
        [HttpGet]
        public ActionResult ArchivedMap()
        {
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var q = (from a in db.Alerts
                     join p in db.Locations on a.AlertId equals p.AlertId
                     into ps from p in ps.DefaultIfEmpty()
                     where a.EndDate<=DateTime.Now && a.StartDate < exD
                     select new { a.Title, a.Description, p.Latitude, p.Longitude, a.AlertId, p.Name, a.alertType.IconUrl })
                     .OrderBy(a=>a.Name);

            //  return PartialView("_map", q.ToList());

            return Json(q, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        //  [ChildActionOnly]
        
        public async Task<JsonResult> AjaxDelete(Guid id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }
            Alert alert = await db.Alerts.FindAsync(id);
           
                if (!Authorize.canDelete(id))
                    {
               
                        return Json(new { success = false, message = "You cannot delete this alert, this alert does not meet the deleting criteria" });

                    }
                   
                    db.Alerts.Remove(alert);
                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = "The alert has been deleted successfully!" });
                
          
        }

        public async Task<ActionResult> DeleteInline(Guid id)
        {
            //filter the alert type based on user permission
            Guid usr = Guid.Parse(User.Identity.GetUserId());
            var at = db.AlertTypes.Where(a => a.userAlertType.Any(c => c.UserId == usr) && a.Status == "Active");
            Alert alert = await db.Alerts.FindAsync(id);
            if (Authorize.canDelete(id))
            {
                db.Alerts.Remove(alert);
                await db.SaveChangesAsync();
                //set the browser message               
                TempData["Message"] = "The record has been deleted successfully. " + msg;
                return RedirectToAction("Index", "Alerts");
            }
            else
            {
                //set the browser message               
                TempData["Message"] = "Error: The record cannot be deleted, please contact admin. " + msg;
                return RedirectToAction("Index", "Alerts");
            }
        }

        [HttpGet]
        public ActionResult ERSS()
        {
            // string xmlData = "http://education.whispir.com/public/723/incident_FCAD.rss";
            string xmlData = System.Configuration.ConfigurationManager.AppSettings["dfesRSS"];
            XMLReader readXML = new XMLReader();
            var data = readXML.RetrunListOfFeeds(xmlData);
           // return View(data.ToList());
            return PartialView(data.ToList());
        }

        [HttpGet]
        public JsonResult AlertsSummary()
        {
            var alerts = db.Alerts
                .Where(a => (a.StartDate >= DateTime.Now || a.EndDate == null || a.EndDate >= DateTime.Now) && a.Published == "Yes" && !a.alertType.Name.Contains("NP"))
                .GroupBy(f => f.alertType.Name)
                .Select(g => new { country = g.Key, count = g.Count() })
                .ToDictionary(k => k.country, i => i.count);

            return Json(alerts.ToList(), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Jobs()
        {

            return View();
        }

        //delete the record inline
        //public async Task<ActionResult> DeleteInline(Guid id)
        //{
        //    Alert alert = await db.Alerts.FindAsync(id);
        //    db.Alerts.Remove(alert);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index", "Home");
        //}

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
