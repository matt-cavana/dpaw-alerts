using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }

        public string Action { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        public string Message { get; set; }

        [Display(Name ="IP Address")]
        public string IpAddress { get; set; }
        [Display (Name ="Meta Data")]
        public string MetaData { get; set; }
        [Display(Name ="Date")]
        [ReadOnly(true)]
        public DateTime ActionDate {
            get
            {
                return (_createdOn == DateTime.MinValue) ? DateTime.Now : _createdOn;
            }
            set { _createdOn = value; }
        }
        // Private
        private DateTime _createdOn = DateTime.Now;
    }
}