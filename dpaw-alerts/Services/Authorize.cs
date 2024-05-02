using dpaw_alerts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace dpaw_alerts.Services
{
    public class Authorize
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        //pass alert type id to check if user has permission 
        public static bool checkUser(int id)
        {
            bool authorised= false;

            ApplicationDbContext db = new ApplicationDbContext();

            //get the current user id
            Guid user = Guid.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId());

            var alert = db.UserAlertTypes.Where(a => a.UserId == user && a.AId==id).Count();

            
            if (alert>0)
            {
                authorised = false;
            }
            else
            {
                authorised = true;
            }

            return authorised;

        }

        public static bool canCreate()
        {
            bool authorised = false;

           
            if (System.Web.HttpContext.Current.User.IsInRole("Manager") || System.Web.HttpContext.Current.User.IsInRole("Admin"))
             {
                authorised = true;

            }
            

            return authorised;

        }

        public static bool canDelete(Guid alertId)
        {

            //check if the alert can be deleted
            var alrt = db.Alerts.Where(a => a.AlertId == alertId && a.Published == "No" && (a.EndDate == null || a.EndDate > DateTime.Now)).ToList();

            if (alrt.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool canEdit(Guid alertId)
        {

            //remove all the previous jobs first
            var alrt = db.Alerts.Where(a => a.AlertId == alertId && (a.StartDate >= DateTime.Now || a.Published == "No") && (a.EndDate == null || a.EndDate > DateTime.Now)).ToList();

            if (alrt.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool canCopy(Guid alertId)
        {

            //remove all the previous jobs first
            var alrt = db.Alerts.Where(a => a.AlertId == alertId &&  a.Published == "Yes").ToList();

            if (alrt.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



    }
}