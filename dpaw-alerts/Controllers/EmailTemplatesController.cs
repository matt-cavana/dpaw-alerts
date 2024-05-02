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
    [CustomAuthorize]
    public class EmailTemplatesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmailTemplates
        public async Task<ActionResult> Index()
        {
            return View(await db.EmailTemplates.ToListAsync());
        }

        // GET: EmailTemplates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,From,To,HeaderSection,FooterSection,CreateDate,CreatedBy")] EmailTemplate emailTemplate)
        {
            emailTemplate.CreatedBy = User.Identity.Name.ToString();
            if (ModelState.IsValid)
            {
                
                db.EmailTemplates.Add(emailTemplate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(emailTemplate);
        }

        // GET: EmailTemplates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // POST: EmailTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,From,HeaderSection,FooterSection,UpdateDate,UpdatedBy,CreatedBy,CreateDate")] EmailTemplate emailTemplate)
        {

            emailTemplate.UpdateDate = DateTime.Now;
            emailTemplate.UpdatedBy = User.Identity.Name;


            if (ModelState.IsValid)
            {
                db.Entry(emailTemplate).State = EntityState.Modified;
                //exclude columns from modification
                db.Entry(emailTemplate).Property(x => x.CreateDate).IsModified = false;
                db.Entry(emailTemplate).Property(x => x.CreatedBy).IsModified = false;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(emailTemplate);
        }

        // GET: EmailTemplates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            return View(emailTemplate);
        }

        // POST: EmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EmailTemplate emailTemplate = await db.EmailTemplates.FindAsync(id);
            db.EmailTemplates.Remove(emailTemplate);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
