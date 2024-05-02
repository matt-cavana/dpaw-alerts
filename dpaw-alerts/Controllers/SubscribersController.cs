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

namespace dpaw_alerts.Controllers
{
    public class SubscribersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscribers
        public async Task<ActionResult> Index()
        {
            return View(await db.Subscribers.ToListAsync());
        }

        // GET: Subscribers/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = await db.Subscribers.FindAsync(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return View(subscriber);
        }

        // GET: Subscribers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,Email,Mobile,SubscriptionDate,SubscriptionType,Status,CreateDate,CreatedBy")] Subscriber subscriber)
        {
            if(!ValidateSubscriber(subscriber.Email))
            {
                string usr = User.Identity.Name;
           
            if (ModelState.IsValid)
            {
                subscriber.Id = Guid.NewGuid();
                subscriber.CreatedBy = usr;
                db.Subscribers.Add(subscriber);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            
            }

            ModelState.AddModelError("", "The email address exists in the system, you cannot use same email.");

            return View(subscriber);
        }

        // GET: Subscribers/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = await db.Subscribers.FindAsync(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,Email,Mobile,SubscriptionDate,Status,CreateDate,CreatedBy, SubscriptionType")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscriber).State = EntityState.Modified;
                //exclude columns from modification
                db.Entry(subscriber).Property(x => x.CreateDate).IsModified = false;
                db.Entry(subscriber).Property(x => x.CreatedBy).IsModified = false;
                //save changes to the database
                await db.SaveChangesAsync();
                TempData["Message"] = String.Format("Subscriber <span class='red'> {0} {1} </span> has been updated successfully!", subscriber.FirstName, subscriber.LastName);
                return RedirectToAction("Index");
            }
            return View(subscriber);
        }

        protected bool ValidateSubscriber(string email)
        {
            
            var subs = db.Subscribers.Where(a => a.Email == email.ToLower()).Count();
            if (subs > 0)
            {
                return true;
            }
            return false;

        }

        [HttpPost]
        //  [ChildActionOnly]
        public async Task<ActionResult> AjaxDelete(Guid? Id)
        {

            if (Id == null)
            {
                return Json(new { success = false });
            }

            Subscriber subscriber = await db.Subscribers.FindAsync(Id);
            db.Subscribers.Remove(subscriber);
            await db.SaveChangesAsync();
            return Json(new { success = true });


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
