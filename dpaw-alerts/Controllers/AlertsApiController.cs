using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using dpaw_alerts.Models;
using System.Net.Http.Headers;

namespace dpaw_alerts.Controllers
{
    public class AlertsApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DateTime exD = DateTime.Now;

        // GET: api/AlertsApi
        public HttpResponseMessage GetAlerts(Guid id, string alert)
        {
            var token = db.Apis.Where(a => a.Token == id && a.ExpiryDate >= DateTime.Now && a.alertType.Slug==alert);

            if (token.Count()==0)
            {
                var message = string.Format("The token with id = {0} has been expired or invalid. Please contact system admin.", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
                
            }
                            

            var alerts = db.Alerts.Where(a => a.alertType.Slug == alert && (a.StartDate>= DateTime.Now || a.EndDate==null || a.EndDate >= DateTime.Now) && a.Published == "Yes").Select(s => new
            { s.Title, s.Description, s.alertType.Slug, s.StartDate, s.EndDate, locations = s.Location.Select(l => new { l.Name, l.Latitude, l.Longitude, l.RPrkId, l.Contact, l.Email }), files= s.Files.Select(f=> new {f.FileTitle, f.FilePath, f.FileType, f.FileSize})
            });

            if(alerts.Count()==0)
            {
                var message = string.Format("There is no active alerts for this category.", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.OK, err);
            }
           

            return Request.CreateResponse(HttpStatusCode.OK, alerts.ToList());

        }

        [HttpGet]
        [ActionName("park")]
       // [Route("ActionApi")]
        public HttpResponseMessage Park(int id, string token, bool count)
        {
            string stoken = "6e8981e3-7d62-4577-ae47-ab1bd72a51b3";

            if (id<0)
            {
                var message = string.Format("Requested park id = {0} is not valid, please check and try again.", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }

            if (stoken!=token)
            {
                var message = string.Format("The token with id = {0} has been expired or invalid. Please contact system admin.", token);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }

            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            //var alt= from a in db.Alerts
            //         join p in db.Locations on a.AlertId equals p.AlertId
                     
            //         where a.StartDate >= exD || a.EndDate >= exD || a.EndDate == null && a.Published=="Yes" && p.RPrkId==id
            //         select new { a.Title, a.Description, p.Latitude, p.Longitude, a.AlertId, p.Name, a.alertType.IconUrl };

            var alerts = db.Locations.Where(a => a.RPrkId==id && (a.alert.EndDate == null || a.alert.EndDate >= exD) && a.alert.Published == "Yes").Select(s => new
            {
                s.alert.Title,
                s.alert.Description,
                s.alert.alertType.Slug,
                s.alert.StartDate,
                s.alert.EndDate,
                 s.Name, s.Latitude, s.Longitude, s.RPrkId, s.Contact, s.Email 
                
            });

            if (alerts.Count() == 0)
            {
                var message = string.Format("There is no active alerts for this park parkId - {0}.", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.OK, err);
            }
            if(count==true)
            {
                var cnt = db.Locations
                .Where(a => a.RPrkId == id && (a.alert.EndDate == null || a.alert.EndDate >= exD) && a.alert.Published == "Yes")
                .GroupBy(f => f.RPrkId)
                .Select(g => new { aid = g.Key, count = g.Count() })
                .ToDictionary(k => k.aid, i => i.count);
                string alertId = (from l in db.Locations
                               where l.RPrkId == id && (l.alert.EndDate == null || l.alert.EndDate >= exD) && l.alert.Published == "Yes"
                               select l.AlertId).Take(1).SingleOrDefault().ToString();
                    db.Locations.Where(a => a.RPrkId == id && (a.alert.EndDate == null || a.alert.EndDate >= exD) && a.alert.Published == "Yes").Select(s =>  new { s.AlertId }).Take(1).SingleOrDefault();
                var cn =alerts.Select(a=>a.Name).Count();
               
                string st = alertId.ToString();
                var resp = "{\"status\":\"SUCCESS\",\"count\":" + cn + ",\"alertId\":\"" +alertId + "\"}";  // TEST !!!!!           

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(resp, System.Text.Encoding.UTF8, "application/json");
                
               return response;
              
            }


            return Request.CreateResponse(HttpStatusCode.OK, alerts.ToList());

        }

        [HttpGet]
        [ActionName("aid")]
        // [Route("ActionApi")]
        public HttpResponseMessage Aid(Guid id, string token)
        {
            string stoken = "6e8981e3-7d62-4577-ae47-ab1bd72a51b3";

            if (id ==null)
            {
                var message = string.Format("Requested park id = {0} is not valid, please check and try again.", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }

            if (stoken != token)
            {
                var message = string.Format("The token with id = {0} has been expired or invalid. Please contact system admin.", token);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);
            }

            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);


            var alert = db.Alerts
                .Where(a => a.AlertId == id && (a.EndDate == null || a.EndDate >= exD) && a.Published == "Yes")
                .Select(a => new { a.Title, a.Description, alertType = a.alertType.Name, a.StartDate, Locations = a.Location.Select(l => new { l.Name, l.Latitude, l.Longitude, l.Contact, l.Email }) });
               

            if (alert.Count() == 0)
            {
                var message = string.Format("Sorry cannot retrieved the alert details for alert - {0}.", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.OK, err);
            }
           
            return Request.CreateResponse(HttpStatusCode.OK, alert.ToList());

        }


        // GET: api/AlertsApi/5
        [ResponseType(typeof(Alert))]
        public async Task<IHttpActionResult> GetAlert(Guid id)
        {
            Alert alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            return Ok(alert);
        }

        // PUT: api/AlertsApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAlert(Guid id, Alert alert)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alert.AlertId)
            {
                return BadRequest();
            }

            db.Entry(alert).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlertExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AlertsApi
        [ResponseType(typeof(Alert))]
        public async Task<IHttpActionResult> PostAlert(Alert alert)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Alerts.Add(alert);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = alert.AlertId }, alert);
        }

        // DELETE: api/AlertsApi/5
        [ResponseType(typeof(Alert))]
        public async Task<IHttpActionResult> DeleteAlert(Guid id)
        {
            Alert alert = await db.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            db.Alerts.Remove(alert);
            await db.SaveChangesAsync();

            return Ok(alert);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlertExists(Guid id)
        {
            return db.Alerts.Count(e => e.AlertId == id) > 0;
        }
    }
}