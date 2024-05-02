using dpaw_alerts.Services;
using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Controllers
{
    [CustomAuthorize(Roles ="Admin")]
    public class OAuthAsyncController : AsyncController
    {
        private string ConsumerKey = "6M5JDzdyn7mqiYElTABJg";
        private string AppSecret = "xpd9bxw0d38ymwGpp2yGIIxUl0qXRrSNwYZGePzOSw";

        // GET: OAuthAsync
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<ActionResult> BeginAsync()
        //{
        //    //var auth = new MvcSignInAuthorizer
        //    var auth = new MvcAuthorizer
        //    {
        //        CredentialStore = new SessionStateCredentialStore
        //        {
        //            ConsumerKey = ConsumerKey, // ConfigurationManager.AppSettings["consumerKey"],
        //            ConsumerSecret = AppSecret //ConfigurationManager.AppSettings["consumerSecret"]
        //        }
        //    };

        //    string twitterCallbackUrl = Request.Url.ToString().Replace("Begin", "Complete");
        //    return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        //}

        public async Task<ActionResult> CompleteAsync()
        {
            //var auth = new MvcAuthorizer
            //{
            //    CredentialStore = new SessionStateCredentialStore()
            //};

            //await auth.CompleteAuthorizeAsync(Request.Url);

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, 
            //   isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent 
            //   queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let 
            //   you make queries without re-authorizing.
            //
            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //

            return RedirectToAction("Index", "Home");
        }
    }
}