using dpaw_alerts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dpaw_alerts.ViewModels
{
    public class AlertViewModel
    {
        public Alert alert { get; set; }
        
        public List<Park> park { get; set; }
    }
}