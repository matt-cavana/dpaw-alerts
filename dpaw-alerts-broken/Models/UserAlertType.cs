using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class UserAlertType
    {

        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Display(Name ="Alert type")]
        public int AId { get; set; }

        [Display(Name = "Grant Date")]
        public DateTime GrantDate
        {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        // Private
        private DateTime _createdOn = DateTime.Now;
        [Display(Name = "Granted By")]
        public string GrantedBy { get; set; }

        [ForeignKey("AId"), Display(Name ="Alert type")]
        public virtual AlertType alertType { get; set; }

    }
}