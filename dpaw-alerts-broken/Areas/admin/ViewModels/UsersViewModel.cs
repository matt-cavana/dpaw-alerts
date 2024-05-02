using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Areas.admin.ViewModels
{
    public class UsersViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Display Name")]
        [Required]
        public string DisplayName { get; set; }

        public string CurrentPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword", ErrorMessage = "New password and confirmation password don't match")]
        [Display (Name ="New Password")]
        public string NewPassword { get; set; }
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        //add a pin to user accounts
        [Required]
        [StringLength(6)]
        [RegularExpression(@"\d{4,6}", ErrorMessage = "Account # must be 4 - 6 digits long")]
        public string Pin { get; set; }

        [Display(Name ="Role")]
        public string SelectedRole { get; set; }

        private readonly List<string> _roles = new List<string>();

        public IEnumerable<SelectListItem> Roles{
            get
            {
              return new SelectList(_roles);
            }
           
        }

        public void LoadUserRoles(IEnumerable<IdentityRole> roles)
        {

            _roles.AddRange(roles.Select(r => r.Name));
        }
    }
}