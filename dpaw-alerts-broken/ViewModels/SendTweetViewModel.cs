using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.ViewModels
{
    public class SendTweetViewModel
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Tweet Text:")]
            [Required]
            [DataType(DataType.MultilineText)]
            public string Message { get; set; }
        
            public string Response { get; set; }
       
    }
}