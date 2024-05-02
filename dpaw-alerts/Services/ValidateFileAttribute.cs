using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dpaw_alerts.Services
{
    public class ValidateFileAttribute: ValidationAttribute
    {
        
            public override bool IsValid(object value)
            {
                int maxContent = 1024 * 1024; //1 MB
                string[] sAllowedExt = new string[] { ".jpg", ".gif", ".png" };


                var file = value as HttpPostedFileBase;

                if (file == null)
                    return false;
                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                    ErrorMessage = "Please upload Your Photo of type: " + string.Join(", ", sAllowedExt);
                    return false;
                }
                else if (file.ContentLength > maxContent)
                {
                    ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (maxContent / 1024).ToString() + "MB";
                    return false;
                }
                else
                    return true;
            }
      
    }
}