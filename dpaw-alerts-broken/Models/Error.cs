using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class Error
    {
        public int Id { get; set; }

        [Display(Name="Source"), MaxLength(120)]
        public string Source { get; set; }

        public string Description { get; set; }

        [ReadOnly(true)]
        public DateTime Date
        {
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