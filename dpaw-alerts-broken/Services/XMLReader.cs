using dpaw_alerts.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Services
{
    public class XMLReader
    {

        public List<EmergencyRSS> RetrunListOfFeeds(string link)
        {
          //  string xmlData = "http://education.whispir.com/public/723/incident_FCAD.rss"; // HttpContext.Current.Server.MapPath("~/App_Data/ProductsData.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(link);
            var feeds = new List<EmergencyRSS>();
            feeds = (from rows in ds.Tables[2].AsEnumerable()
                        select new EmergencyRSS
                        {
                            title=rows[0].ToString(),
                            link= rows[1].ToString(),
                            description = rows[2].ToString(),
                            pubDate=Convert.ToDateTime(rows[3].ToString()),
                            longitude= float.Parse( rows[5].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                            latitude = float.Parse(rows[6].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                            
                        }).ToList();
            return feeds;
        }
    }
}