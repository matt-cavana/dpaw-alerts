using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Models
{

    public class Alert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AlertId { get; set; }

        [Display(Name = "Title"), MaxLength(160, ErrorMessage ="Title cannot be more than 200 characters long."), Required(ErrorMessage ="You must enter an alert title")]
       // [DataType(DataType.MultilineText)]
        public string Title { get; set; }

       
        [Display(Name = "Description"), MaxLength(20000, ErrorMessage = "Description cannot be more than 8000 characters long."), Required(ErrorMessage ="You must enter a description in {0} field")]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

       [Display(Name = "Start Date"), Required(ErrorMessage ="You must enter the start date and time")]
       [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
      // [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Affected Site/s"), MaxLength(200,ErrorMessage ="You cannot enter more than 200 characters.")]
        public string SitesAffected { get; set; }

        [MaxLength(10)]
        public  string Facebook { get; set; }

        [MaxLength(10)]
        public string SMS { get; set; }

        [MaxLength(10)]
        public string Twitter { get; set; }

        [Display(Name = "Push Notification"), MaxLength(10)]
        public string PushNotification { get; set; }

        [MaxLength(10)]
        public string Email { get; set; }

        [Display(Name ="Start immedtiately")]
        public bool PubImmediately { get; set; }

        [MaxLength(10)]
        public string Published { get; set; }

        [Display(Name = "Website Link"), MaxLength(200,ErrorMessage ="This cannot be more than 200 characters.")]
        public string WebLink { get; set; }

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

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display (Name ="Last Update")]
        public DateTime? UpdateDate { get; set; }
        [Display(Name = "Updated By"), MaxLength(160)]
        public string UpdatedBy { get; set; }

        [Display(Name ="Alert Type")]
        public int AId { get; set; }
        
        public virtual ICollection<Location> Location { get; set; }
        public virtual ICollection<AlertFile> Files { get; set; }
        [JsonIgnore]
        public virtual AlertType alertType {get; set;}

    }

   

}