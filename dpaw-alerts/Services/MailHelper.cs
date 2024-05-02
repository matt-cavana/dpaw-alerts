using System.Net.Mail;
using System.IO;
using System.Web;


public class MailHelper
{

    
	/// <summary>
	/// Sends an mail message
	/// </summary>
	/// <param name="from">Sender address</param>
	/// <param name="recepient">Recepient address</param>
	/// <param name="bcc">Bcc recepient</param>
	/// <param name="cc">Cc recepient</param>
	/// <param name="subject">Subject of mail message</param>
	/// <param name="body">Body of mail message</param>
    public static void SendMailMessage(string from, string recepient, string bcc, string cc, string replyTo, string subject, string body) //,HttpPostedFile file -- add for attaching a file
    {

        
        try
        {
            // Instantiate a new instance of MailMessage
            MailMessage mMailMessage = new MailMessage();

            // Set the sender address of the mail message
            mMailMessage.From = new MailAddress(from);
            // Set the recepient address of the mail message
            mMailMessage.To.Add(new MailAddress(recepient));

            // Check if the bcc value is null or an empty string
            if (bcc != null & bcc != string.Empty)
            {
                // Set the Bcc address of the mail message
                mMailMessage.Bcc.Add(new MailAddress(bcc));
            }

            // Check if the cc value is null or an empty value
            if (cc != null & cc != string.Empty)
            {
                // Set the CC address of the mail message
                mMailMessage.CC.Add(new MailAddress(cc));
            }

            //mail priority
            mMailMessage.Priority = MailPriority.Normal;

            //set the reply address
            mMailMessage.ReplyTo = new MailAddress(replyTo);


            // Set the subject of the mail message
            mMailMessage.Subject = subject;

            // Set the body of the mail message
            mMailMessage.Body = body;

            //Attach the file
            //mMailMessage.Attachments.Add(new Attachment(file.InputStream, file.FileName));

            // Secify the format of the body as HTML
            mMailMessage.IsBodyHtml = true;

            //request a read receipt
            // mMailMessage.Headers.Add("Disposition-Notification-To", recepient);

            // Set the priority of the mail message to normal
            mMailMessage.Priority = MailPriority.Normal;

            // Instantiate a new instance of SmtpClient
            SmtpClient mSmtpClient = new SmtpClient();
            // Send the mail message
            mSmtpClient.Send(mMailMessage);

        }
        catch (SmtpException smtpEx)
        {

            //A problem occurred when sending the email message
            throw smtpEx;

        }
    }

}

