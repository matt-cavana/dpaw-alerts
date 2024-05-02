using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dpaw_alerts.ViewModels
{
   // [NotMapped]
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}