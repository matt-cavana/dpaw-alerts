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
    public class ApisController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Apis
        public async Task<ActionResult> Index()
        {
            var apis = db.Apis.Include(a => a.alertType);
            return View(await apis.ToListAsync());
        }

        // GET: Apis/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Api api = await db.Apis.FindAsync(id);
            if (api == null)
            {
                return HttpNotFound();
            }
            return View(api);
        }

        // GET: Apis/Create
        public ActionResult Create()
        {
            var api = new Api();
            Guid tkn= Guid.NewGuid();
           
            api.Token = tkn;
            api.ExpiryDate = DateTime.Now.AddMonths(1);
            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name");
            return View(api);
        }

        // POST: Apis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AId,Token,ExpiryDate,CreateDate,CreatedBy,Description")] Api api)
        {
            var usr = User.Identity.Name;
            api.CreatedBy = usr;
            
            if (ModelState.IsValid)
            {
                db.Apis.Add(api);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name", api.AId);
            return View(api);
        }

        // GET: Apis/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Api api = await db.Apis.FindAsync(id);
            if (api == null)
            {
                return HttpNotFound();
            }
            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name", api.AId);
            return View(api);
        }

        // POST: Apis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AId,Token,ExpiryDate, Description")] Api api)
        {
            if (ModelState.IsValid)
            {
                db.Entry(api).State = EntityState.Modified;
                db.Entry(api).Property(x => x.CreateDate).IsModified = false;
                db.Entry(api).Property(x => x.CreatedBy).IsModified = false;
                await db.SaveChangesAsync();
                ViewBag.StatusMessage = "<div class='alert alert-success'>The record has been updated successfully.</div>";
                return RedirectToAction("Index");
            }
            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name", api.AId);
            return View(api);
        }

        [HttpPost]
        //  [ChildActionOnly]
        public async Task<ActionResult> AjaxDelete(int? Id)
        {

            if (Id == null)
            {
                return Json(new { success = false });
            }

            Api api = await db.Apis.FindAsync(Id);
            db.Apis.Remove(api);
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
