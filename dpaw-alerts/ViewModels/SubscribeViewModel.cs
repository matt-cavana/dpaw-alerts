using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.ViewModels
{
    public class SubscribeViewModel
    {

        [Display(Name = "Subscription Type"), MaxLength(120, ErrorMessage = "Subscription Type is too long"), Required]
        public string SubscriptionType { get; set; }
    }
}