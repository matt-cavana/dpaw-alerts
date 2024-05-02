using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
           // Response.StatusCode = 404;  //you may want to set this to 200
            return View();
        }

        
        public ViewResult AccessDenied()
        {
            //Response.StatusCode = 401;  //you may want to set this to 200
            return View();
        }

        public ViewResult Internal()
        {

            return View();
        }
    }
}