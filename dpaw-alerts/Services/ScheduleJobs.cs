using dpaw_alerts.Models;
using Facebook;
using LinqToTwitter;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Twilio;

namespace dpaw_alerts.Services
{
   
    public class EmailJobs: IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Execute(IJobExecutionContext context)
        {
            DateTime exD = DateTime.Now;
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var job = db.ScheduleJobs.Include("Alert").Where(a => a.Channel == "Email" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();
            var subscribers = db.Subscribers.Where(a => a.Status == "Active").ToList();
            var template = db.EmailTemplates.FirstOrDefault();

            if (job != null)
            {
                //bind other variables
                string park = "<h3>Affected Parks/Locations</h3><p><ul>";
                string loc = "";
                foreach (var pk in job.alert.Location)
                {
                    loc = loc + ("<li>" + pk.Name + "</li>");
                }

                park = park + loc + "</ul></p>";
                string asites = "<p><strong>Affected sites:</strong> " + job.alert.SitesAffected.ToString() + "</p>";
                string uri = "<p><strong>Website link:</strong> " + job.alert.WebLink.ToString() + "</p>";
                int tot = 0;

                foreach (var subsriber in subscribers)
                {
                    using (var message = new MailMessage(template.From, subsriber.Email))
                    {
                        message.Subject = "Park Alerts - " + job.alert.Title;
                        message.Body = template.HeaderSection + job.alert.Description + park + asites + uri + template.FooterSection;
                        message.IsBodyHtml = true;
                        using (SmtpClient client = new SmtpClient
                        {
                            EnableSsl = false, // true,
                            Host = "smtp.lan.fyi", //"smtp.gmail.com",
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
            }
            
        }
    }

    public class FacebookJob : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public void Execute(IJobExecutionContext context)
        {

            var fb = db.SocialNetworks.FirstOrDefault(a => a.Channel == "Facebook");
            // https://graph.facebook.com/v2.8/145798112110995?145798112110995?fields=access_token
            //AQBwzpK_S13pCUkXZtwLxY9WZXg-kxnqqs88zFBxRcNfDGQ-Ouc8hhlv6GfXMu2spKRw83PyDx_kXSrNt8K4KMvN7Hr4qeCfW8Y8Opo7iD_JhQiXw_11MXZMdpG7Xjq1UCBTkP0kdX1pANDEL-1t_k2D4dMIClslkaetSSHNuBnIURi3NeCXE-Q1fbjE8vPXcK2neJAYUECqGimkZGn55Cm2LS4rwzzvKSl3_gTQQBnEN-bnazq_uEpq2B5VhdhAFIMxDU9ljQCmE_w_DQIJcXPS-EKTVcWm1-DK0arRXbuTlVHaNFX6kEPNXWZRnxwhDJU#_=_
            string page_id = "145798112110995";
            string app_secret = fb.AppSecret; // "d06e2db9d4444ae349c34d34af2de3cf";

            string page_token = fb.LongLifeAccessToken; // "EAAC7gaECrVsBAEop7oPNyqjJ6njgiTZBaDH3xNaJ58RnxiUOKAfUlTLROTVxpRxbZCznVPJQOZALsq3zYTonMZA8OZB4rHD2pgRUo3AYaVxG18Dfx2ZBZBJIyqYCLGalJ5QAMCPGKUBkDYwepIFdu2XwShBPNwdD2gZD";
            DateTime exD= DateTime.Now;
            exD=  new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var job = db.ScheduleJobs.Include("Alert").Where(a=>a.Channel=="Facebook" && a.Status=="Pending" && a.ExecutionDate== exD).FirstOrDefault();

            if (job.Id > 0)
            {
                var p = "";

                foreach (var park in job.alert.Location)
                {
                   
                    p += park.Name +",";   
                }
                p= p.Remove(p.Length - 1);

                string bdy = job.alert.Title +  "\nAffected Parks\\Locations: " + p +"\n\n------------------------------- \n"+ job.alert.Description + "\nVisit https://www.dpaw.wa.gov.au for details" ;

             string desc = bdy ;
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
            }

        }
        
        private void UpdateFacebookJob(ScheduleJob job)
        {
            ScheduleJob scheduleJob = db.ScheduleJobs.Find(job);
            //set the values
            scheduleJob.Status = "Complete";
            scheduleJob.CompletedDate = DateTime.Now;
            db.Entry(scheduleJob).State = EntityState.Modified;
            db.SaveChanges();

        }

       

    }

    public class TwitterJob : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public void Execute(IJobExecutionContext context)
        {
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
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var job = db.ScheduleJobs.Include("Alert").Where(a => a.Channel == "Twitter" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();

          

            if (job.Id > 0 )
            {
                string bdy = job.alert.Title + ". Read more at https://www.dpaw.wa.gov.au for details";

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

            }
        }

    }

    public class SMSJob : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void Execute(IJobExecutionContext context)
        {
            var tw = db.SocialNetworks.FirstOrDefault(a => a.Channel == "SMS");

            string accountSid = tw.AppId;
            string authToken = tw.AccessToken;
            string twilioNo = tw.AppSecret;
            

            DateTime exD = DateTime.Now;
            exD = new DateTime(exD.Ticks - (exD.Ticks % TimeSpan.TicksPerMinute), exD.Kind);
            var job = db.ScheduleJobs.Include("Alert").Where(a => a.Channel == "SMS" && a.Status == "Pending" && a.ExecutionDate == exD).FirstOrDefault();
            var subscribers = db.Subscribers.Where(a => a.Status == "Active").ToList();

            if (job.Id > 0)
            {
                string bdy = job.alert.Title + ". Visit https://alerts.dbca.wa.gov.au/alerts/ for details. Reply STOP to unsubscribe.";

               
                var twilio = new TwilioRestClient(accountSid, authToken);
                int tot = 0;
                var message=new Twilio.Message();

                foreach (var subsriber in subscribers)
                {
                    var num ="+61"+ subsriber.Mobile;
                        message = twilio.SendMessage(
                        twilioNo, // From (Replace with your Twilio number)
                        num, // To (Replace with your phone number)
                        bdy
                        );
                    tot ++;
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
                job.Response = message.Sid + "Sent to " + tot + " subscribers";
                job.Status = "Complete";
                    
                }

                    job.CompletedDate = DateTime.Now;
                    job.Response = message.Sid;
                    db.Entry(job).State = EntityState.Modified;
                    db.SaveChanges();


            }
        }

    }

}