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
    [CustomAuthorize(Roles ="Admin,Manager")]
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        public async Task<ActionResult> Index()
        {
            return View(await db.Contacts.OrderBy(a=>a.OfficeName).ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contacts/Details/5
        public async Task<ActionResult> AjaxDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return PartialView("_AjaxDetails", contact);
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ContactId,OfficeName,Email,Phone,Address,OfficeHours,CreatedBy")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                var usr = User.Identity.Name;
                contact.CreatedBy = usr;
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
                TempData["Message"] = String.Format("The {0} has been added successfully", contact.OfficeName);
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ContactId,OfficeName,Email,Phone,Address,OfficeHours,LastUpdate,UpdatedBy")] Contact contact)
        {
            
            if (ModelState.IsValid)
            {
                contact.LastUpdate = DateTime.Now;
                contact.UpdatedBy = User.Identity.Name.ToString();
                db.Entry(contact).State = EntityState.Modified;
                //exclude columns from modification
                db.Entry(contact).Property(x => x.CreateDate).IsModified = false;
                db.Entry(contact).Property(x => x.CreatedBy).IsModified = false;
                await db.SaveChangesAsync();
                //set the status message
                TempData["Message"] = String.Format("The {0} has been updated successfully", contact.OfficeName);

                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Contact contact = await db.Contacts.FindAsync(id);
            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        //  [ChildActionOnly]
        public async Task<ActionResult> AjaxDelete(int? Id)
        {

            if (Id == null)
            {
                return Json(new { success = false });
            }

            Contact contact = await db.Contacts.FindAsync(Id);
            db.Contacts.Remove(contact);
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
