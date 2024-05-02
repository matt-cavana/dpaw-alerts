using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Models
{
    public class SocialNetwork
    {
        [Key]
        public int Id { get; set; }

        public string Channel { get; set; }

        [Display(Name ="App ID"),Required]
        public string AppId { get; set; }

        [Display(Name = "App Secret"), Required]
        public string AppSecret { get; set; }

        [Display(Name = "Access Token"), Required, DataType(DataType.MultilineText)]
        public string AccessToken { get; set; }

        [Display(Name = " Long Life Access Token"), DataType(DataType.MultilineText)]
        public string LongLifeAccessToken { get; set; }

        [Display (Name ="Token Expiry")]
        public DateTime? ExpiryDate { get; set; }

    }
}