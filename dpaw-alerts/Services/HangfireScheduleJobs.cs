using dpaw_alerts.Models;
using Facebook;
using Hangfire;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Twilio;

namespace dpaw_alerts.Services
{
    public class HangfireScheduleJobs
    {
       
        public static string EmailJob()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            
            DateTime exD= DateTime.Now;
            // dateTime = dateTime.Truncate(TimeSpan.FromMilliseconds(1)); // Truncate to whole ms
            // dateTime = dateTime.Truncate(TimeSpan.FromSeconds(1)); // Truncate to whole second
            //  dateTime = dateTime.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute

            exD = exD.Truncate(TimeSpan.FromMinutes(1));
            Console.Write(exD.ToString());
           // exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var job = db.ScheduleJobs.Where(a => a.Channel == "Email" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();
            var subscribers = db.Subscribers.Where(a => a.Status == "Active" && a.SubscriptionType=="Email" || a.SubscriptionType =="Email-SMS" ).ToList();
            var template = db.EmailTemplates.FirstOrDefault();
            

            if (job == null) return "No active email job found at " + DateTime.Now.ToString();


            //bind other variables
            string park = "<h3>Affected Parks/Locations</h3><p><ul>";
                string loc = "";
                foreach (var pk in job.alert.Location)
                {
                    loc = loc + ("<li>" + pk.Name + "</li>");
                }

                park = park + loc + "</ul></p>";
                string asites = string.Format("<p><strong>Affected sites:</strong> {0} </p>", job.alert.SitesAffected);
                string uri = string.Format("<p><strong>Website link:</strong> {0} </p>", job.alert.WebLink);
                var link = string.Format("<p>View alert at <a href='{1}/Home/Index#{0}'>{1}/Home/Index#{0}</a></p>", job.AlertId, BaseUrl());
            
            int tot = 0;

                foreach (var subsriber in subscribers)
                {
                    using (var message = new MailMessage(template.From, subsriber.Email))
                    {
                    var unsubsribe = string.Format("<p>To unsubscribe from the mailing list <a href='{2}/Home/unsubscribe?token={0}&email={1}'> click here</a></p>", subsriber.Id, subsriber.Email, BaseUrl() );
                    message.Subject = job.alert.Title;
                    message.Body = template.HeaderSection + job.alert.Description + park + asites + uri + link + template.FooterSection + unsubsribe;
                    message.IsBodyHtml = true;
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = false, // true,
                        Host = "10.6.20.100", //"smtp.gmail.com",
                        Port = 25//587,
                                 // Credentials = new NetworkCredential("user@gmail.com", "password")
                    })
                    {
                        client.Send(message);
                    }
                }

                    tot++;

                }

                job.Status = "Complete";
                job.CompletedDate = DateTime.Now;
                job.Response = "Email sent to " + tot.ToString() + " subscriber/s";// responseTweet.Text.ToString();
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
            string st = "Executed at " + DateTime.Now.ToString();

            return st;

        }

        public static string SMSJob()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var tw = db.SocialNetworks.FirstOrDefault(a => a.Channel == "SMS");
            DateTime exD=DateTime.Now;
            exD = exD.Truncate(TimeSpan.FromMinutes(1));
            string accountSid = tw.AppId;
            string authToken = tw.AccessToken;
            string twilioNo = tw.AppSecret;
            
            
            //var job = db.ScheduleJobs.Where(a => a.Channel == "SMS" && a.Status == "Pending" && DbFunctions.AddSeconds(a.ExecutionDate,-DateTime.Now.Second) == exD).FirstOrDefault();
            var job = db.ScheduleJobs.Where(a => a.Channel == "SMS" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();
            var subscribers = db.Subscribers.Where(a => a.Status == "Active" && a.SubscriptionType == "SMS" || a.SubscriptionType == "Email-SMS").ToList();

            if (job == null) return "No active job found at " + DateTime.Now.ToString();
            
                string bdy = job.alert.Title + string.Format( ". Visit {0}/Home/Index#"+job.AlertId+ " for details. Reply STOP to unsubscribe.",BaseUrl());


                var twilio = new TwilioRestClient(accountSid, authToken);
                int tot = 0;
                var message = new Twilio.Message();

                foreach (var subsriber in subscribers)
                {
                //check if the mobile number is null
                if(subsriber.Mobile !=null || subsriber.Mobile == string.Empty)
                {
                    var num = "+61" + subsriber.Mobile;
                    message = twilio.SendMessage(
                    twilioNo, // From (Replace with your Twilio number)
                    num, // To (Replace with your phone number)
                    bdy
                    );
                    tot++;
                }
                   
                    
                }
                if (message.RestException != null)
                {
                    //error = message.RestException.Message;
                    // Console.WriteLine(error);
                    // Console.Write("Press any key to continue.");
                    //  Console.ReadKey();
                    //log error to the system
                    LogError.log("SMS", message.RestException.Message);
                    // throw ex;
                    job.Status = "Failed";
                    job.Response = message.RestException.Message;
                }
                else
                {
                    job.Response = message.Sid + ". Sent to " + tot + " subscribers";
                    job.Status = "Complete";

                }

                job.CompletedDate = DateTime.Now;
                //job.Response = message.Sid;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();

            string st="Executed at "+ DateTime.Now.ToString();

            return st;

        }

        public static string BaseUrl()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string burl = appSettings["BaseUrl"];

            return burl;

        }

        

        public static string FacebookJob()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var fb = db.SocialNetworks.FirstOrDefault(a => a.Channel == "Facebook");
            // https://graph.facebook.com/v2.8/145798112110995?145798112110995?fields=access_token
           
            string page_id = "145798112110995";
            string app_secret = fb.AppSecret; // "d06e2db9d4444ae349c34d34af2de3cf";

            string page_token = fb.LongLifeAccessToken; 
            DateTime exD = DateTime.Now;
            exD = exD.Truncate(TimeSpan.FromMinutes(1));
            var job = db.ScheduleJobs.Include("Alert").Where(a => a.Channel == "Facebook" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();

            if (job == null) return "No active job found for Facebook at " + DateTime.Now.ToString();

           
                var p = "";

                foreach (var park in job.alert.Location)
                {

                    p += park.Name + ",";
                }
                p = p.Remove(p.Length - 1);

                string bdy = job.alert.Title + "\nAffected Parks\\Locations: " + p + "\n\n------------------------------- \n" + job.alert.Description + string.Format( "\nVisit {0}/Home/Index#",BaseUrl())+ job.AlertId+ " for details";

                string desc = bdy;
                var client = new FacebookClient(page_token);

                // client.Post("/me/feed", new { message = "markhagan.me video tutorial" });
                dynamic parameters = new ExpandoObject();
                // parameters.title = "App title";
                // parameters.access_token = page_token;
                parameters.message = HtmlRemoval.StripTagsRegex(desc);
                //"Posted from my mvc application";
                //  parameters.link = "https://www.dpaw.wa.gov.au";
                // parameters.picture = "https://alerts.dbca.wa.gov.au/img/logo.png";
                //parameters.name = "Article Title";
                // parameters.caption = "Department of Parks and Wildlife";
                // parameters.appsecret_proof = FaceBookSecret(page_token, app_secret);

                try
                {
                    var result = client.Post("/" + page_id + "/feed", parameters);

                    job.Status = "Complete";
                    job.Response = result.ToString();

                }
                catch (FacebookOAuthException ex)
                {

                    //log error to the system
                    LogError.log("Facebook", ex.ToString());
                    // throw ex;
                    job.Status = "Failed";
                    job.Response = ex.ToString();
                }

                job.CompletedDate = DateTime.Now;

                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();

            string st = "Executed at " + DateTime.Now.ToString();

            return st;
        }

        public static string TwitterJob()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var tw = db.SocialNetworks.FirstOrDefault(a => a.Channel == "Twitter");

            string ConsumerKey = tw.AppId;
            string ConsumerSecret = tw.AppSecret;
            string AccessToken = tw.AccessToken;
            string TokenSecret = tw.LongLifeAccessToken;

            IOAuthCredentials credentials = new InMemoryCredentials();// new SessionStateCredentials();

            MvcAuthorizer auth;
            TwitterContext twitterCtx;

            //bind the authentication variables
            credentials.ConsumerKey = ConsumerKey;
            credentials.ConsumerSecret = ConsumerSecret;
            credentials.AccessToken = TokenSecret; //token secret
            credentials.OAuthToken = AccessToken;


            auth = new MvcAuthorizer
            {
                Credentials = credentials
            };

            twitterCtx = new TwitterContext(auth);

            DateTime exD = DateTime.Now;
            exD = exD.Truncate(TimeSpan.FromMinutes(1));
            var job = db.ScheduleJobs.Include("Alert").Where(a => a.Channel == "Twitter" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();

            //exit the method if job returns nothing
            if (job == null) return "No active job found for Twitter at " + DateTime.Now.ToString();

           
                string bdy = job.alert.Title + string.Format(". Read more at {0}/Home/Index#",BaseUrl()) + job.AlertId + " for details";

                try
                {
                    var ctx = new TwitterContext(auth);

                    //for version 3.x
                    //Status responseTweet = ctx.TweetAsync(bdy);

                    //ctx.TweetWithMedia(bdy, false, null);

                    var tweet = twitterCtx.UpdateStatus(bdy);
                    string resp = "Status returned: " +
                                    "(" + tweet.StatusID + ")" +
                                    "[" + tweet.User.ID + "]" +
                                    tweet.User.Name + ", " +
                                    tweet.Text + ", " +
                                    tweet.CreatedAt + "\n";
                    job.Status = "Complete";
                    job.Response = resp;

                }
                catch (TwitterQueryException ex)
                {
                    //log error to the system
                    LogError.log("Twitter", ex.ToString());
                    job.Status = "Failed";
                    job.Response = ex.ToString();
                    // throw ex;
                }
                job.CompletedDate = DateTime.Now;
                // responseTweet.Text.ToString();
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();

                string st = "Executed at " + DateTime.Now.ToString();
                return st;
        }

        public static void HangfireJobList()
        {
            //RecurringJob.AddOrUpdate(() => EmailJob(), Cron.Minutely);
           RecurringJob.AddOrUpdate(() => EmailJob(), Cron.Minutely);

           // RecurringJob.AddOrUpdate<HangfireScheduleJobs>(x => x.SMSJob(), Cron.MinuteInterval(1));
            RecurringJob.AddOrUpdate(()=>SMSJob(), Cron.MinuteInterval(1));

            //Add facebook job in cron schedule
            RecurringJob.AddOrUpdate(() => FacebookJob(), Cron.Minutely);

            //Add facebook job in cron schedule
            RecurringJob.AddOrUpdate(() => TwitterJob(), Cron.Minutely);
        }

    }


    
}