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
    public class LocationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Locations
       // [ChildActionOnly]
        public ActionResult Index(Guid id)
        {
            //var locations = db.Locations.Include(l => l.alert);
            var locations = db.Locations.Where(a => a.AlertId == id);
            return PartialView("_list", locations.ToList());
        }

        // GET: Locations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = await db.Locations.FindAsync(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", location);
        }

        // GET: Locations/Create
        public ActionResult Create(Guid id)
        {
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title");
           

            return PartialView("_create");
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Longitude,Latitude,Contact,Email")] Location location, string id)
        {
            
            
            if (!ValidatePark(location.Name, new Guid(id)))
            {
                if (ModelState.IsValid)
                    {
                        location.AlertId = new Guid(id);
                        db.Locations.Add(location);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
            }

            ModelState.AddModelError("", "The location has already been added to this alert!");

            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", location.AlertId);
            return PartialView("_create",location);
        }


        //  [HttpGet]
        public ActionResult AddLocation()
        {
            ViewBag.RPrkId = new SelectList(db.Parks, "RPrkId", "Name", "RPrkId");

            return PartialView("_create");

        }

       

        //Post method to add details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLocation(Location obj, Guid id)
        {
            ViewBag.RPrkId = new SelectList(db.Parks, "RPrkId", "Name", "RPrkId");
            var url = Url.Action("Index", "Locations", new { id = id });
            if (!ValidatePark(obj.Name, id))
            {
                if (ModelState.IsValid)
                {
                    obj.AlertId = id;
                    db.Locations.Add(obj);
                    db.SaveChanges();
                    TempData["Message"] = String.Format("The Location {0} has been added successfully", obj.Name);
                    //redirect to index

                    return Json(new { success = true, message = "<div class='alert alert-info'>The location has been added successfully.</div>", url = url });
                }
            }
            else
            {
                ModelState.AddModelError("", "The location has already been added to this alert!");
            }

            
            return PartialView("_create");
        }

        protected bool ValidatePark(string name, Guid alertId)
        {
            var park = db.Locations.Where(p => p.AlertId == alertId && p.Name == name).Count();
            if (park > 0)
            {
                return true;
            }
            return false;

        }

        [HttpGet]
        public JsonResult GetPark (int? id)
        {
            // var park = db.Parks.Find(id);
            var park = from p in db.Parks
                       where p.RPrkId==id
                       select new { name=p.Name, latitude = p.Latitude, longitude = p.Longitude, district = p.District, contact = p.Contact.Phone, email=p.Contact.Email };

            return Json(park, JsonRequestBehavior.AllowGet);
         
        }

        //Post method to add details
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult DeleteLocation(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot find any record.</div>" });
            }

            Location location = db.Locations.Find(id);
            db.Locations.Remove(location);

            if (db.SaveChanges() > 0)
            {
               
                //redirect to index
                string url = Url.Action("List", "Movies");
                return Json(new { success = true, message = "<div class='alert alert-danger'>The location has been deleted successfully.</div>" });
            };

            return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot add this location.</div>" });

        }

        // GET: Locations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = await db.Locations.FindAsync(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", location.AlertId);
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LocId,Name,AlertId,Longitude,Latitude,Contact,Email")] Location location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(location).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", location.AlertId);
            return View(location);
        }

        //// GET: Locations/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Location location = await db.Locations.FindAsync(id);
        //    if (location == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(location);
        //}

        //// POST: Locations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Location location = await db.Locations.FindAsync(id);
        //    db.Locations.Remove(location);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
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
