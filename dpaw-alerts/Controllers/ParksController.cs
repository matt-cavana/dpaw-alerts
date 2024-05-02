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
    [Authorize]
    public class ParksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Parks
        public async Task<ActionResult> Index()
        {
            return View(await db.Parks.OrderBy(a=>a.Name).Include(c=>c.Contact).ToListAsync());
        }


        // GET: Parks
       
        public  ActionResult listMap()
        {
            
            return PartialView("_listMap",db.Parks.ToList());
        }

        // GET: Parks
         [HttpGet]
        public ActionResult map()
        {
            var q = from t in db.Parks select new { t.Name, t.Latitude, t.Longitude, t.Description };

            //  return PartialView("_map", q.ToList());

            return Json(q, JsonRequestBehavior.AllowGet);
        }

        // GET: Parks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Park park = await db.Parks.FindAsync(id);
            if (park == null)
            {
                return HttpNotFound();
            }
            // return View(park);
            return PartialView("Details", park);
        }

        // GET: Parks/Create
        public ActionResult Create()
        {
            ViewBag.status = new SelectListItem[]{
                new SelectListItem() {Text = "Active", Value="Active"},
                new SelectListItem() {Text = "Inactive ", Value="Inactive"}
                };
            ViewBag.ContactId = new SelectList(db.Contacts.OrderBy(a => a.OfficeName), "ContactId", "officeName","ContactId");
            return View();
        }

        // POST: Parks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "parkId,RPrkId,Name,Region,District,Description,Tenure,Longitude,Latitude,Status,CreatedBy,ContactId")] Park park)
        {
            ViewBag.status = new SelectListItem[]{
                new SelectListItem() {Text = "Active", Value="Active"},
                new SelectListItem() {Text = "Inactive ", Value="Inactive"}
                };

            ViewBag.ContactId = new SelectList(db.Contacts.OrderBy(a => a.OfficeName), "ContactId", "officeName", park.ContactId);
            if (ModelState.IsValid)
            {
                var usr = User.Identity.Name;
                park.CreatedBy = usr;
                db.Parks.Add(park);
                await db.SaveChangesAsync();
                TempData["Message"] = String.Format("The {0} has been created successfully", park.Name);
                return RedirectToAction("Index");
            }

            return View(park);
        }

        // GET: Parks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.status = new SelectListItem[]{
                new SelectListItem() {Text = "Active", Value="Active"},
                new SelectListItem() {Text = "Inactive ", Value="Inactive"}
                };
            Park park = await db.Parks.FindAsync(id);
            ViewBag.ContactId = new SelectList(db.Contacts.OrderBy(a=>a.OfficeName), "ContactId", "officeName", park.ContactId);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (park == null)
            {
                return HttpNotFound();
            }
            return View(park);
        }

        // POST: Parks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "parkId,RPrkId,Name,Region,District,Description,Tenure,Longitude,Latitude,Status,ContactId,CreateDate,CreatedBy")] Park park)
        {
            ViewBag.ContactId = new SelectList(db.Contacts.OrderBy(a => a.OfficeName), "ContactId", "officeName", park.ContactId);

            ViewBag.status = new SelectListItem[]{
                new SelectListItem() {Text = "Active", Value="Active"},
                new SelectListItem() {Text = "Inactive ", Value="Inactive"}
                };

            if (ModelState.IsValid)
            {
                db.Entry(park).State = EntityState.Modified;
                //exclude columns from modification
                db.Entry(park).Property(x => x.CreateDate).IsModified = false;
                db.Entry(park).Property(x => x.CreatedBy).IsModified = false;
                await db.SaveChangesAsync();
                TempData["Message"] = String.Format("The {0} has been updated successfully", park.Name);
                return RedirectToAction("Index");
            }
            return View(park);
        }

        // GET: Parks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Park park = await db.Parks.FindAsync(id);
            if (park == null)
            {
                return HttpNotFound();
            }
            return View(park);
        }

        // POST: Parks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Park park = await db.Parks.FindAsync(id);
            db.Parks.Remove(park);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
      //  [ChildActionOnly]
        public async Task<ActionResult> AjaxDelete(int? parkId)
        {

            if (parkId == null)
            {
                return Json(new { success = false });
            }

            Park park = await db.Parks.FindAsync(parkId);
            db.Parks.Remove(park);
            await db.SaveChangesAsync();
            return Json(new { success = true });


        }

        [ChildActionOnly]
        public ActionResult Status(List<String> sts)
        {
            return View(sts);
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
