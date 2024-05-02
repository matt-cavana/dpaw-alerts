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
using System.IO;
using dpaw_alerts.Services;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize (Roles ="Admin,Manager")]
    public class AlertTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AlertTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.AlertTypes.ToListAsync());
        }

        // GET: AlertTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertType alertType = await db.AlertTypes.FindAsync(id);
            if (alertType == null)
            {
                return HttpNotFound();
            }
            return View(alertType);
        }

        // GET: AlertTypes/Create
        public ActionResult CreateOld()
        {
            return View();
        }

        // POST: AlertTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOld([Bind(Include = "AId,Name,Description,CreateDate,CreatedBy")] AlertType alertType)
        {

            string name = Convert.ToString(Request["txtName"].ToString());
            string description = Convert.ToString(Request["txtDescription"].ToString());
            
            if (ModelState.IsValid)
            {
                db.AlertTypes.Add(alertType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(alertType);
        }

        [HttpPost]
        public async Task<ActionResult> Create( HttpPostedFileBase file)
        {
            AlertType alertType = new AlertType();

            string name = Convert.ToString(Request["txtName"].ToString());
            string description = Convert.ToString(Request["txtDescription"].ToString());
            
            if(!ValidateAlertType(name))
            {

                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    
                    if (IsImageExtension(extension))
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/media/"), fileName);
                        file.SaveAs(path);
                        string fpath = "~/media/" + fileName;
                        //add the file name to table column
                        alertType.IconUrl = fpath.ToString();
                    }
                    else
                    {
                        ModelState.AddModelError("", "This file extension is not allowed, only JPG, GIF and PNG files are allowed");
                        return View(alertType);
                    }
                }

                //create the slug dynamically based on user input
           
                alertType.Slug = name.Replace(" ", "-").ToLower();
                alertType.Status = "Active";
                alertType.Description = description;
                alertType.Name = name;
                alertType.CreatedBy = User.Identity.Name.ToString();

                db.AlertTypes.Add(alertType);
                await db.SaveChangesAsync();
                TempData["Message"] = "The alert type has been created successfully.";
                return RedirectToAction("Index");

            }
            TempData["Message"] = "Error: Alert type exists in the system.";

            return RedirectToAction("Index");
        }

        public JsonResult CheckAlertName(string name)
        {
            var result = db.AlertTypes.Where(a=>a.Name == name).Count() == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected bool ValidateAlertType(string name)
        {
            var atype = db.AlertTypes.Where(a => a.Name == name).Count();
            if (atype > 0)
            {
                return true;
            }
            return false;

        }

        // GET: AlertTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertType alertType = await db.AlertTypes.FindAsync(id);
            if (alertType == null)
            {
                return HttpNotFound();
            }
            return View(alertType);
        }

        // POST: AlertTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AId,Name,Slug,Description,CreateDate,CreatedBy,IconUrl,Status")] AlertType alertType, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);

                    //remove the old file first
                    if(alertType.IconUrl!= null)
                    {
                        var fname = alertType.IconUrl;
                        var fullPath = Server.MapPath(fname);
                        //check the file exists before you delete if it has wrong file name
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    if (IsImageExtension(extension))
                    {
                        
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/media/"), fileName);
                        try
                        {
                            file.SaveAs(path);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.ToString());
                            return View(alertType);
                        }
                       
                        string fpath = "~/media/" + fileName;
                        //add the file name to table column
                        alertType.IconUrl = fpath.ToString();
                    }
                    else
                    { 
                    ModelState.AddModelError("", "This file extension is not allowed, only JPG, GIF and PNG files are allowed");
                    return View(alertType);
                    }
                
            }
                //create the slug dynamically based on user input
                if(alertType.Slug==null)
                {
                    alertType.Slug = (alertType.Name).Replace(" ", "-").ToLower();
                }
                else
                {
                    alertType.Slug = (alertType.Slug).Replace(" ", "-").ToLower();
                }
                db.Entry(alertType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(alertType);
        }

        private static readonly string[] _validExtensions = { ".jpg", ".bmp", ".gif", ".png",".svg" }; //  etc

        public static bool IsImageExtension(string ext)
        {
            return _validExtensions.Contains(ext.ToLower());
        }

        //// GET: AlertTypes/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AlertType alertType = await db.AlertTypes.FindAsync(id);
        //    if (alertType == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(alertType);
        //}

        //// POST: AlertTypes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    AlertType alertType = await db.AlertTypes.FindAsync(id);

        //    var fname = alertType.IconUrl.ToString();
        //    var fullPath = Server.MapPath(fname);
        //    if (System.IO.File.Exists(fullPath))
        //    {
        //        System.IO.File.Delete(fullPath);
        //    }
            

        //    db.AlertTypes.Remove(alertType);
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
