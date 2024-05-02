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
using System.Net.Http;
using dpaw_alerts.Services;
using Facebook;
using System.Dynamic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using dpaw_alerts.ViewModels;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize(Roles ="Admin")]
    public class SocialNetworksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SocialNetworks
        public async Task<ActionResult> Index()
        {
            return View(await db.SocialNetworks.ToListAsync());
        }

        // GET: SocialNetworks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocialNetwork socialNetwork = await db.SocialNetworks.FindAsync(id);
            if (socialNetwork == null)
            {
                return HttpNotFound();
            }
            return View(socialNetwork);
        }

        // GET: SocialNetworks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SocialNetworks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Channel,AppId,AppSecret,AccessToken,LongLifeAccessToken,ExpiryDate")] SocialNetwork socialNetwork)
        {
            if (ModelState.IsValid)
            {
                db.SocialNetworks.Add(socialNetwork);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(socialNetwork);
        }

        // GET: SocialNetworks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocialNetwork socialNetwork = await db.SocialNetworks.FindAsync(id);
            if (socialNetwork == null)
            {
                return HttpNotFound();
            }
            return View(socialNetwork);
        }

        // POST: SocialNetworks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Channel,AppId,AppSecret,AccessToken,LongLifeAccessToken,ExpiryDate")] SocialNetwork socialNetwork)
        {
            if (ModelState.IsValid)
            {
                db.Entry(socialNetwork).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(socialNetwork);
        }

        // GET: SocialNetworks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SocialNetwork socialNetwork = await db.SocialNetworks.FindAsync(id);
            if (socialNetwork == null)
            {
                return HttpNotFound();
            }
            return View(socialNetwork);
        }

        // [HttpPost]
        public ActionResult FacebookAuth(string code)
        {
            // https://graph.facebook.com/v2.8/145798112110995?145798112110995?fields=access_token
            
            string page_id = "145798112110995";
            string app_id = "243610232718169";
            string app_secret = "1eec1b1924ea5ad043b8cfda69b8f69b";
            string scope = "publish_pages,manage_pages,publish_actions,user_events";
            string access_token = "";

            //if (Request["code"] == null)
            if (code == null)
            {
                Response.Redirect(string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, Request.Url.AbsoluteUri, scope));

                //https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={3}
            }
            else
            {
                Dictionary<string, string> tokens = new Dictionary<string, string>();

                string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}",
                    app_id, Request.Url.AbsoluteUri, scope, Request["code"].ToString(), app_secret);
                
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    string vals = reader.ReadToEnd();

                    foreach (string token in vals.Split('&'))
                    {
                        //meh.aspx?token1=steve&token2=jake&...
                        tokens.Add(token.Substring(0, token.IndexOf("=")),
                            token.Substring(token.IndexOf("=") + 1, token.Length - token.IndexOf("=") - 1));
                    }
                }


                // string access_token =  "EAAC7gaECrVsBAMHcKZARZCH0BuJ3j4Wm71TDnEgWO7fAywXcs2jt88ZAktc1bZAprh7FkMH1abpEZBTFfKQRKRGo7BxXl26wifTM0ZAdVqmVUloKXDlTYQpmWWcSXZAv6EB4pgT4AypRRUBIpAZCre73IMUMa8sZCTp7HgrUnFh5uJgZDZD";
                //"EAACEdEose0cBAFt1OSr3fj9irUZBnp1nKYVeL4koXxKSl13ZBz3Uo8aUcTrTdRtbbgRV1rFAe0TT4YU58q5xESqioXN1ZA3RB6x45ZBpiL675sgC2kPZC0KcbxPgoXdlxkzfROx1fg1TCgdK0tZAoRLxZCzjyGav90f7krJCCmXFAZDZD"

                access_token = tokens["access_token"];

                var client = new FacebookClient(access_token);

                // client.Post("/me/feed", new { message = "markhagan.me video tutorial" });
                dynamic parameters = new ExpandoObject();
                // parameters.title = "App title";
                parameters.access_token = access_token;
                parameters.message = "Posted from my mvc application";
                // parameters.link = "http://www.natiska.com/article.html";
                // parameters.picture = "http://www.natiska.com/dav.png";
                //parameters.name = "Article Title";
                //  parameters.caption = "Caption for the link";
                parameters.appsecret_proof = FaceBookSecret(access_token, app_secret);

              //  var result = client.Post("/" + page_id + "/feed", parameters);


            }

            TempData["fbmsg"] =string.Format( "You have successfully authorised the DPaW App to post in Facebook on your behalf. Access token - {0}",access_token);
            // return Content("Facebook post success, Token - {0}", code);
            return RedirectToAction("Index");
        }

        internal static string FaceBookSecret(string content, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(content);
            byte[] hash;
            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes))
            {
                hash = hmacsha256.ComputeHash(messageBytes);
            }

            StringBuilder sbHash = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sbHash.Append(hash[i].ToString("x2"));
            }
            return sbHash.ToString();
        }

        public ActionResult Tweet()
        {
            var sendTweetVM = new SendTweetViewModel
            {
                Message = "Testing async LINQ to Twitter in MVC - " + DateTime.Now.ToString()
            };

            return View(sendTweetVM);
        }

        [HttpPost]
        [ActionName("Tweet")]
        public async Task<ActionResult> TweetAsync(SendTweetViewModel tweet)
        {

            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;

            //var auth = new MvcAuthorizer
            //{
            //    CredentialStore = new SessionStateCredentialStore()
            //    {
            //        ConsumerKey = ConsumerKey,
            //        ConsumerSecret = AppSecret,
            //        OAuthToken = AccessToken,
            //        OAuthTokenSecret= TokenSecret

            //    }
            //};

            //var ctx = new TwitterContext(auth);

            //Status responseTweet = await ctx.TweetAsync(tweet.Message);

            ////var responseTweetVM = new SendTweetViewModel();
            //tweet.Message = "Testing async LINQ to Twitter in MVC - " + DateTime.Now.ToString();
            //tweet.Response = "Tweet successful! Response from Twitter: " + responseTweet.Text;

            //ViewBag.Message= "Tweet successful! Response from Twitter: " + responseTweet.Text;

            return View(tweet);
        }

        [HttpPost]
        public ActionResult getToken(string appId, string appSecret, string accessToken)
        {

            var tokenUri = new Uri(
                   string.Format(
                       "https://graph.facebook.com/v2.4/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", appId, appSecret, accessToken));

            HttpClient client = new HttpClient();
            var response = Task.Run(async () =>
            {
                var res = client.GetAsync(tokenUri).Result;
                return res;
            });

            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                var responseError = response.Result.Content.ReadAsStringAsync();
                var messageText = string.Format("{0}", responseError.Result);
                var statuscode = response.Result.StatusCode.ToString();

                return Json(new { success=false, message = messageText }, JsonRequestBehavior.AllowGet);
            }

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            var LongLivedAccessToken = jsonObject.access_token.ToString();
            int expiry = jsonObject.expires_in;
            int days = expiry / 86400;
            DateTime dt = DateTime.Now.AddDays(days);

            return Json(new { success = true, message = LongLivedAccessToken, expiry = dt.ToString() }, JsonRequestBehavior.AllowGet);

        }

        // POST: SocialNetworks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SocialNetwork socialNetwork = await db.SocialNetworks.FindAsync(id);
            db.SocialNetworks.Remove(socialNetwork);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
