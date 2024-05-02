using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class Location
    {
        [Key]
        public int LocId { get; set; }

       
        [Display(Name = "Park/Location Name"), MaxLength(120, ErrorMessage = "Park or Location name is too long"), Required]
        public string Name { get; set; }
        
        public Guid AlertId { get; set; }

        [Display(Name ="Park Name")]
        public int? RPrkId { get; set; }

        [Required]
        public double Longitude { get; set; }

        [ Required]
        public double Latitude { get; set; }
                
        public string Contact { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage ="Please enter a valid email address")]
        [DataType(DataType.EmailAddress), MaxLength(120)]
        public string Email { get; set; }

        [ForeignKey("AlertId")]
        public virtual Alert alert { get; set; }
       
    }
}