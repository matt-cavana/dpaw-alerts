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
    [CustomAuthorize (Roles ="Admin")]
    public class UserAlertTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserAlertTypes
        public ActionResult Index(Guid id)
        {
            
            var userAlertTypes = db.UserAlertTypes.Include(u => u.alertType).Where(a=>a.UserId==id);
            return PartialView(userAlertTypes.ToList());
        }

       
        [HttpPost]
       public JsonResult AddUser(int aid, string id)
        {
            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name");

           
            if (!ValidateAlert(aid, new Guid(id)))
            {
                UserAlertType userAlertType = new UserAlertType();
                userAlertType.AId = aid;
                userAlertType.GrantedBy = User.Identity.Name;
                userAlertType.UserId = new Guid(id);
                db.UserAlertTypes.Add(userAlertType);
                
                if (db.SaveChanges() > 0)
                {
                   
                    return Json(new { success = true, message = "<div class='alert alert-info'>The alert type has been added successfully.</div>" });
                };
            }
            else
            {
                return Json(new { success = false, message = "<div class='alert alert-danger'>Error: This alert type already been added to this user.</div>" });
            }
          


           return Json(new { success = false, message = "<div class='alert alert-danger'>Error: This alert type already been added to this user.</div>" });

        }

        protected bool ValidateAlert(int AId, Guid userId)
        {

            bool recordExist = false;

            var park = db.UserAlertTypes.Where(p => p.AId == AId && p.UserId == userId).Count();

            if (park > 0)
            {
                recordExist = true;

            }
            else
            {
                recordExist = false;
            }

            return recordExist;
        }



        // GET: UserAlertTypes/Delete/5
        public JsonResult Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot find any record.</div>" });
            }

            UserAlertType alertType = db.UserAlertTypes.Find(id);
            var nm = alertType.alertType.Name;
            db.UserAlertTypes.Remove(alertType);

            if (db.SaveChanges() > 0)
            {
                var msg = String.Format("<div class='alert alert-danger'>The record {0} has been deleted successfully.</div>", nm);
                //redirect to index
                string url = Url.Action("List", "Movies");
                return Json(new { success = true, message = msg });
            };

            return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot add this park.</div>" });
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
