using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class Subscriber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name ="First Name"), MaxLength(60), Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name"), MaxLength(60), Required]
        public string LastName { get; set; }

        [Display(Name = "Email"), MaxLength(120), Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Mobile"), MaxLength(9)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be numeric without starting 0 and no blank space")]
        public string Mobile { get; set; }

        [Display(Name = "Subscription Type"), MaxLength(120, ErrorMessage = "Subscription Type is too long"), Required]
        public string SubscriptionType { get; set; }

        [Display(Name ="Subscription Date")]
        public DateTime? SubscriptionDate { get; set; }

        public string Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Create Date")]
        [ReadOnly(true)]
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
        [ReadOnly(true)]
        public string CreatedBy { get; set; }
    }
}