using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace dpaw_alerts.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        //add additional field
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

  
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Park>().Property(x => x.Latitude).HasPrecision(8, 3);
        //    modelBuilder.Entity<Park>().Property(x => x.Longitude).HasPrecision(8, 3);

        //}

        public static ApplicationDbContext Create()
        {
            
            return new ApplicationDbContext();
        }

        public DbSet<Park> Parks { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AlertType> AlertTypes { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Location> Locations { get; set; }
       
        public DbSet<ScheduleJob> ScheduleJobs { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<UserAlertType> UserAlertTypes { get; set; }
        public DbSet<Api> Apis { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<AlertFile> AlertFiles { get; set; }
        
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

    }
}