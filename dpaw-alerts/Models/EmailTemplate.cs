using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dpaw_alerts.Models
{
    public class EmailTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string From { get; set; }

        [Display(Name ="Header Section"), AllowHtml, Required]
        [DataType(DataType.MultilineText)]
        public string HeaderSection { get; set; }

        [Display(Name ="Footer Section"), AllowHtml, Required]
        [DataType(DataType.MultilineText)]
        public string FooterSection { get; set; }

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
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }
        [Display(Name = "Updated By"), MaxLength(160)]
        public string UpdatedBy { get; set; }


    }
}