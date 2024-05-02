using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dpaw_alerts.Models;
using System.IO;
using dpaw_alerts.Services;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize]
    public class AlertFilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AlertFiles
        public ActionResult Index(Guid id)
        {
            var files = db.AlertFiles.Where(a => a.AlertId == id);
            return PartialView("_list", files.ToList());
        }

        // GET: AlertFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertFile alertFile = db.AlertFiles.Find(id);
            if (alertFile == null)
            {
                return HttpNotFound();
            }
            return View(alertFile);
        }

        //  [HttpGet]
        public ActionResult AddFile(Guid id)
        {
            var afile = new AlertFile();
            return PartialView("_create", afile);

        }

        //Post method to add details
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddFile(AlertFile obj, string id, HttpPostedFileBase file, string FileTitle)
        {

            var url = Url.Action("Index", "AlertFiles", new { id = id });
            Guid alertId = new Guid(id);
                if (file != null && file.ContentLength > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                
                if (IsImageExtension(extension))
                    {
                    Guid strGuid = Guid.NewGuid();
                    var fileName = strGuid + extension; //Path.GetFileName(file.FileName);
                    var fullPath = Server.MapPath(fileName);
                   

                        float fileSize = file.ContentLength/1024;
                    string fsize;

                    if(fileSize>1024)
                    {
                        fsize = Math.Round((fileSize/1024),2).ToString() + " mb";
                    }
                    else
                    {
                        fsize = Math.Round(fileSize,2).ToString() + " kb";
                    }
                        var path = Path.Combine(Server.MapPath("~/media/"), fileName);
                        file.SaveAs(path);
                        string fpath = "~/media/" + fileName;
                         //add the file name to table column
                        obj.FileTitle = FileTitle;
                        obj.AlertId = alertId;
                        obj.FilePath = fpath.ToString();
                        obj.FileSize =fsize ;
                        obj.FileType =  extension.Replace(".","");
                        obj.CreatedBy = User.Identity.Name;
                        db.AlertFiles.Add(obj);
                        db.SaveChanges();
                        return Json(new { success = true, message = "<div class='alert alert-info'>The file has been added successfully.</div>", url=url });

                    }
                    else
                    {
                        ModelState.AddModelError("", "This file extension is not allowed, only pdf, doc, docx file types are allowed");
                        return PartialView("_create",obj);
                    }
        
            }
            else
            {
                ModelState.AddModelError("", "Select a file.");
            }
            
            return PartialView("_create");
        }

        private static readonly string[] _validExtensions = { ".pdf", ".xls", ".docx", ".doc" }; //  etc

        public static bool IsImageExtension(string ext)
        {
            return _validExtensions.Contains(ext.ToLower());
        }

       

        // GET: AlertFiles/Create
        public ActionResult Create()
        {
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title");
            return View();
        }

        // POST: AlertFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AlertId,Description,FilePath,FileSize,FileType,CreateDate,CreatedBy")] AlertFile alertFile)
        {
            if (ModelState.IsValid)
            {
                db.AlertFiles.Add(alertFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", alertFile.AlertId);
            return View(alertFile);
        }

        // GET: AlertFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlertFile alertFile = db.AlertFiles.Find(id);
            if (alertFile == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", alertFile.AlertId);
            return View(alertFile);
        }

        // POST: AlertFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AlertId,Description,FilePath,FileSize,FileType,CreateDate,CreatedBy")] AlertFile alertFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alertFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AlertId = new SelectList(db.Alerts, "AlertId", "Title", alertFile.AlertId);
            return View(alertFile);
        }

        //Post method to add details
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot find any record.</div>" });
            }

            AlertFile alertFile = db.AlertFiles.Find(id);

            var fname = alertFile.FilePath.ToString();
            var fullPath = Server.MapPath(fname);
            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                AlertFile afile = db.AlertFiles.Find(id);
                db.AlertFiles.Remove(afile);

                if (db.SaveChanges() > 0)
                {

                    //redirect to index
                    string url = Url.Action("List", "Movies");
                    return Json(new { success = true, message = "<div class='alert alert-danger'>The file has been deleted successfully.</div>" });
                };
            }
            catch (IOException ex)
            {
                LogError.log("File delete", ex.ToString());
                return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot remove the file from the server.</div>" });
            }
           
            
            

            return Json(new { success = false, message = "<div class='alert alert-warning'>Sorry cannot add this file.</div>" });

        }

        public FilePathResult DownloadExampleFiles(string fileName, string ftype)
        {
            var fpath = Server.MapPath(fileName);
            
            return new FilePathResult(fpath, string.Format("application/{0}", ftype));
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
