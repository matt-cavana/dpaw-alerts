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
    public class AlertFile
    {
        [Key]
        public int Id { get; set; }

        public Guid AlertId { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name ="Title")]
        public string FileTitle { get; set; }

        [Display(Name ="File Path"), Required]
        public string FilePath { get; set; }

        [Display(Name = "Size")]
        public string FileSize { get; set; }

        [Display(Name = "File Type")]
        public string FileType { get; set; }

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

        [ForeignKey("AlertId")]
        public virtual Alert alert { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }


    }

}