using dpaw_alerts.Models;
using dpaw_alerts.Services;
using dpaw_alerts.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Controllers
{

    [CustomAuthorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserService _users;

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        ApplicationDbContext db = new ApplicationDbContext();

        public UserController()
        {
            _userRepository = new UserRepository();
            _roleRepository = new RoleRepository();
            _users = new UserService(ModelState, _userRepository, _roleRepository);
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/User
        [Route("")]
       // [Authorize(Roles="admin")]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.CreateUserSuccess ? "User has been created successfully."
                : message == ManageMessageId.SetPasswordSuccess ? "User password has been updated."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.UpdateUserSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemoveUserSuccess ? "The user has been deleted from the system."
                : "";

            var users = await _userRepository.GetAllUsersAsync();

            return View(users);
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            
            //var DistQry = from d in db.Parks
            //               orderby d.district
            //               select d.district;

            //district.AddRange(DistQry.Distinct());
            //ViewBag.district = new SelectList(district);
           

            var model = new UserViewModel();
           
            model.LoadUserRoles(await _roleRepository.GetAllRolesAsync());

            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(UserViewModel model)
        {
           
            model.LoadUserRoles(await _roleRepository.GetAllRolesAsync());
            TempData["email"] = model.Email;
            var fname = model.FirstName;
            var lname = model.LastName;
            var uname = model.UserName;

          //  ViewBag.ArtistId = new SelectList(db.Artists, "ArtistId", "Name", album.ArtistId);

            //get email from web.config file
            var adminEmail = System.Configuration.ConfigurationManager.AppSettings["adminEmail"];
            var senderEmail = System.Configuration.ConfigurationManager.AppSettings["senderEmail"];

            var completed = await _users.CreateAsync(model);
            

            if (completed)
            {
                string body;
                var path= Server.MapPath("~\\Content\\Welcome.txt");
                //Read template file from the App_Data folder
                using (var sr = new StreamReader(path))
                {
                    body = sr.ReadToEnd();
                }

            try
            {
                //add email logic here to email the customer that their invoice has been voided
                //Username: {1}
                string firstname = HttpUtility.UrlEncode(fname);
                string lastname = HttpUtility.UrlEncode(lname);
                string username = HttpUtility.UrlEncode(uname);
              
                    //set the values in the stream
                string messageBody = string.Format(body, firstname, lastname, username);

                    MailHelper.SendMailMessage(senderEmail, TempData["email"].ToString(), "", "", senderEmail, "Congratulations - DPaW Alerts System Login Information", messageBody);

                    //return RedirectToAction("EmailConfirm");
                }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

                return RedirectToAction("index", new { Message = ManageMessageId.CreateUserSuccess });
            }


            return View(model);
        }

       
        [HttpPost]
        public ActionResult GetUserData(string userName)
        {

            DirectoryEntry entry = new DirectoryEntry("LDAP://corporateict.domain");
            DirectorySearcher Dsearch = new DirectorySearcher(entry);
            Dsearch.Filter = string.Format("(&(samaccountname={0})(objectClass=person))", userName);

            var model = new UserViewModel();

            foreach (SearchResult sResultSet in Dsearch.FindAll())
            {

                // First Name
                model.FirstName = getProperty(sResultSet, "givenName");
                model.LastName= getProperty(sResultSet, "sn");
                model.Email = (getProperty(sResultSet, "mail"));
                // Middle Initials
                //lstUser.Items.Add(getProperty(sResultSet, "initials"))
                // Last Name
                model.PhoneNumber = (getProperty(sResultSet, "telephoneNumber"));
                return Json(new { success = true, model });
            }
                       
            return Json(new { success = false });

        }

        public string getProperty(SearchResult SearchResult, string propertyName)
        {

            if (SearchResult.Properties.Contains(propertyName))
            {
                return SearchResult.Properties[propertyName][0].ToString();
            }
            else {
                return string.Empty;
            }

        }

        [HttpGet]
       // [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(string username)
        {
            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name");

            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            

            var currentUser = User.Identity.Name;

            //if (!User.IsInRole("admin") &&
            //    !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var user = await _users.GetUserByNameAsync(username);

            
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
      //  [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(UserViewModel model, string username)
        {
            var currentUser = User.Identity.Name;
            var isAdmin = User.IsInRole("Admin");
            ViewBag.AId = new SelectList(db.AlertTypes, "AId", "Name");

            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            

            //if (!isAdmin &&
            //    !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var userUpdated = await _users.UpdateUser(model);

            if (userUpdated)
            {

                return RedirectToAction("index", new { Message = ManageMessageId.UpdateUserSuccess });
            }

            return View(model);
        }

        // GET: /User/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /User/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model, string id)
        {
            if (ModelState.IsValid)
            {
                //remove the password first
                var premoved = await UserManager.RemovePasswordAsync(id);
                if (premoved.Succeeded)
                {
                    AddErrors(premoved);
                }

                    var result = await UserManager.AddPasswordAsync(id, model.NewPassword);
                
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
     //   [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string username)
        {
            await _users.DeleteAsync(username);

            return RedirectToAction("index");
        }

        public async Task<ActionResult> DeleteInline(string username)
        {

            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            await _users.DeleteAsync(username);

            return RedirectToAction("Index", new { Message = ManageMessageId.RemoveUserSuccess });

        }

        [HttpPost]
        public async Task<ActionResult> AjaxDelete(string username)
        {

            if (username == null)
            {
                return Json(new { success = false });
            }

            await _users.DeleteAsync(username);
            return Json(new { success = true});
           

        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {

            if (!_isDisposed)
            {
                _userRepository.Dispose();
                _roleRepository.Dispose();
            }
            _isDisposed = true;

            base.Dispose(disposing);
        }

    #region helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult GetRolesForUser(string userId)
        {

            var user = _userRepository.GetUserByName(userId);
            
            var userRoles = _userRepository.GetRolesForUser(user);

            string rstr =  userRoles.Count() > 1 ?
                userRoles.FirstOrDefault() : userRoles.SingleOrDefault();
           
            return Content(rstr);

        }

        public void UpdateStatus(int? id)
        {

            using (var db = new ApplicationDbContext())
            {
                //var ar = db.AccessRequests.Find(id);
                ////db.AccessRequests.Attach(ar);
                //// db.Entry(ar).Property(x => x.Status).IsModified = true;
                ////  db.Entry(ar).Property(x => x.ProcessedDate).IsModified = true;
                //if (ar.Id > 0)
                //{
                //    ar.Status = "Processed";
                //    ar.ProcessedDate = DateTime.Now.Date;
                //    db.Entry(ar).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Sorry cannot update the Access request status!");

                //}
            }
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();

            // For each string in the 'elements' variable, create a new SelectListItem object
            // that has both its Value and Text properties set to a particular value.
            // This will result in MVC rendering each item as:
            //     <option value="State Name">State Name</option>
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element,
                    Text = element
                });
            }

            return selectList;
        }

        public enum ManageMessageId
        {
            UpdateUserSuccess,
            SetPasswordSuccess,
            CreateUserSuccess,
            RemoveUserSuccess,
            Error
        }

       
#endregion
    }
}