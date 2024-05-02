using dpaw_alerts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace dpaw_alerts.Services
{
    public class LogError
    {
       

        //log error 
        public static void log(string source, string description)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var er = new Error();

            //bind values
            er.Source = source;
            er.Description = description;
            db.Errors.Add(er);
            db.SaveChanges();

            //notify admin user when error occurs

            using (var message = new MailMessage("no-reply@dpaw.wa.gov.au", "mohammed.tajuddin@dpaw.wa.gov.au"))
            {
                message.Subject = "Error report - Alerts Application";
                message.Body = "<p>Source: " + source +"</p><p>Description: <br />" + description +"</p>";
                message.IsBodyHtml = true;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = false, // true,
                    Host = "10.6.20.100", //"smtp.gmail.com",
                    Port = 25//587,
                             // Credentials = new NetworkCredential("user@gmail.com", "password")
                })
                {
                    client.Send(message);
                }
            }
        }

        public static void LogUserActivity(string username, string message, string action)
        {
            UserActivity ua = new UserActivity();
            ApplicationDbContext db = new ApplicationDbContext();
            string ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string s = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + " Name = " + browser.Browser + "\n"
                + " Version = " + browser.Version + "\n"
                + " Major Version = " + browser.MajorVersion + "\n"
                + " Minor Version = " + browser.MinorVersion + "\n"
                + " Platform = " + browser.Platform + "\n"
                + " Is Beta = " + browser.Beta + "\n"
                + " Is Crawler = " + browser.Crawler + "\n"
                + " Is AOL = " + browser.AOL + "\n"
                + " Is Win16 = " + browser.Win16 + "\n"
                + " Is Win32 = " + browser.Win32 + "\n"
                + " Supports Frames = " + browser.Frames + "\n"
                + " Supports Tables = " + browser.Tables + "\n"
                + " Supports Cookies = " + browser.Cookies + "\n"
                + " Supports VBScript = " + browser.VBScript + "\n"
                + " Supports JavaScript = " +
                    browser.EcmaScriptVersion.ToString() + "\n"
                + " Supports Java Applets = " + browser.JavaApplets + "\n"
                + " Supports ActiveX Controls = " + browser.ActiveXControls
                      + "\n"
                + " Supports JavaScript Version = " +
                    browser["JavaScriptVersion"] + "\n";
           
            s = s + "User Agent: " + HttpContext.Current.Request.UserAgent;
            s = s + " Machine Name: " + System.Environment.MachineName;
            s = s + " Domain Name: " + System.Environment.UserDomainName;
            s = s + " Computer Username: " + System.Environment.UserName;

            //bind variables to model
            ua.Action = action;
            ua.Message = message;
            ua.MetaData = s;
            ua.IpAddress = ip;
            ua.UserName = username;
            //save the record
            db.UserActivities.Add(ua);
            db.SaveChanges();

        }

    }
}