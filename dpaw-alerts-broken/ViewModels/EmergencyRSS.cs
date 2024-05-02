using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace dpaw_alerts.ViewModels
{
    [Serializable]
    [XmlRoot("Channel"), XmlType("Channel")]
    public class EmergencyRSS
    {

        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public DateTime pubDate { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
    }
}