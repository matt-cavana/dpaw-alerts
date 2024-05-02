using dpaw_alerts.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Models
{
    public class AlertType
    {
        [Key]
        public int AId { get; set; }

        [Required]
        [Display (Name ="Alert Type")]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(60)]
        public string Slug { get; set; }

        [Required, MaxLength(12)]
        public string Status { get; set; }

        [Display(Name ="Icon URL")]
      //  [ValidateFile]
        public string IconUrl { get; set; }

        [Display (Name ="Create Date")]
        public DateTime CreateDate
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        // Private
        private DateTime _createdOn = DateTime.Now;
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public virtual ICollection<UserAlertType> userAlertType { get; set; }


        public IEnumerable<SelectListItem> sts
        {
            get
            {
                return new SelectList(new[]
                                        {
                                             new {status= "Active" },
                                             new {status= "Inactive" },
                                          }, "status", "status", "status");
            }
        }
    }
}