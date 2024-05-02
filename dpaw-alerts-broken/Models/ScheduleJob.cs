using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class ScheduleJob
    {
        [Key]
        public int Id { get; set; }

        public Guid AlertId { get; set; }

        [Display(Name = "Channel"), MaxLength(60)]
        public string Channel { get; set; }

        [Display(Name ="Execution Date")]
        public DateTime ExecutionDate { get; set; }

        [Display(Name = "Status"), MaxLength(20)]
        public string Status { get; set; }

        [Display(Name = "Completed Time")]
        public DateTime? CompletedDate { get; set; }

        public string Response { get; set; }
       
        [Display(Name = "Created Date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
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
        [ForeignKey("AlertId")]
        public virtual Alert alert { get; set;}
    }


}