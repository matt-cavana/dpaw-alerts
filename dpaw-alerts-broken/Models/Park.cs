using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Models
{
    public class Park
    {
        [Key]
        public int ParkId { get; set; }

        [Display(Name ="RATIS ID")]
        public int RPrkId { get; set; }

        [Display(Name = "Park Name"), MaxLength(120, ErrorMessage ="Park name is too long"), Required]
        public string Name { get; set; }

        public string Region { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Tenure { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        [DisplayName("Contact")]
        public int? ContactId { get; set; }

        [Required]
        public string Status { get; set; }

        [Display(Name="Created Date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }

        public string CreatedBy { get; set; }

        // Private
        private DateTime _createdOn = DateTime.Now;


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

        //make the relationship with contact table
        public virtual Contact Contact { get; set; }
    }
}