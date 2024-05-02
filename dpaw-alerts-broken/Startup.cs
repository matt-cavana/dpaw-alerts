using dpaw_alerts.Models;
using dpaw_alerts.Services;
using Hangfire;
using Hangfire.SqlServer;
//using Hangfire;
//using Hangfire.SqlServer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(dpaw_alerts.Startup))]
namespace dpaw_alerts
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //create role on first load
            createRolesandUsers();
            //var options = new SqlServerStorageOptions
            //{
            //    QueuePollInterval = TimeSpan.FromSeconds(15) // Default value
            //};
            

            //GlobalConfiguration.Configuration
            //   .UseSqlServerStorage("DefaultConnection");

            GlobalConfiguration.Configuration
            .UseSqlServerStorage(
           "DefaultConnection",
           new SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) });
            // //.UseFilter(new LogFailureAttribute());
            

            //bind the authorization
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                AuthorizationFilters = new[] { new HangfireAuthorization() }
            });
            //app.UseHangfireDashboard("/jobs");
            app.UseHangfireServer();

            //call the hangfire jobs
            HangfireScheduleJobs.HangfireJobList();
        }

        // In this method we will create default User roles and Admin user for login    
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                   

                var user = new ApplicationUser();
                user.UserName = "mohammedta";
                user.Email = "mohammed.tajuddin@dpaw.wa.gov.au";

                string userPWD = "%4$FD@Aadfsd*";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role     
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);

            }

            // creating Creating Employee role     
            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);

            }
            
        }
    }
}
