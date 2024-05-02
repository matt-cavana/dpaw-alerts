using dpaw_alerts.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.ViewModels
{
    public class UserViewModel
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public string Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Index("IX_EMAIL", 1, IsUnique = true)]
        public string Email { get; set; }
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        

        [Required]
        [Display(Name = "Role")]
        public string SelectedRole { get; set; }
        

        private readonly List<string> _roles = new List<string>();
        

        //public IEnumerable<SelectListItem> d
        //{
        //    get
        //    {
        //        return new SelectList(db.Districts, "Name", "Name");
        //    }

        //}

        //public UserViewModel()
        //{
        //     var district = new List<string>();

        //    district = db.Districts.Select(p => p.Name).ToList();
        //    d = new SelectList(district,District);
        //}

        public IEnumerable<SelectListItem> Roles
        {
            get { return new SelectList(_roles); }
        }

        public void LoadUserRoles(IEnumerable<IdentityRole> roles)
        {
            _roles.AddRange(roles.Select(r => r.Name));
        }
    }
}