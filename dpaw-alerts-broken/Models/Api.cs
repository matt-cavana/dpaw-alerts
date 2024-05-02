using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class Api
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Alert Type")]
        public int AId { get; set; }
        [Required, MaxLength(200)]
        public string Description { get; set; }
        [Required]
        public Guid Token { get; set; }

        [Display(Name = "Token Expiry"), Required]
        public DateTime ExpiryDate { get; set; }

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

        [ForeignKey("AId"), Display(Name ="Alert Type")]
        public virtual AlertType alertType { get; set; }
    }
}