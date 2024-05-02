using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Display (Name ="Office Name" ), Required]
        public string OfficeName { get; set; }
             
        [DataType("Email"), Required]   
        public string Email { get; set; }
        
        public string Phone { get; set; }
        
        [Required]        
        public string Address { get; set; }
        [Display(Name = "Office Hours")]
        public string OfficeHours { get; set; }
        [Display(Name = "Create Date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
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
        [Display(Name = "Last Update"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LastUpdate { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }
    }
}