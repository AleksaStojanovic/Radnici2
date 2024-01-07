using System.Net.Mail;
using System.Net;

namespace Radnici.Logic
{
    public class Logic
    {

        private string mailBody = "<!DOCTYPE html><html><body> <h1> Hello </h1>html";

        private string mailTitle = "Attachment Demo";

        private string mailSubject = "Email With Attachment";

        private string fromEmail = "###";

        private string mailPassword = "###";



        public void sendEmail(string toEmail, IFormFile fileToAttach)
        {
            MailMessage mailMessage = new MailMessage(new MailAddress(fromEmail, mailTitle), new MailAddress(toEmail));
            mailMessage.Subject = mailSubject;
            mailMessage.Body = mailBody;
            mailMessage.IsBodyHtml = true;
            mailMessage.Attachments.Add(new Attachment(fileToAttach.OpenReadStream(), fileToAttach.FileName));
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.office365.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = fromEmail;
            networkCredential.Password = mailPassword;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = networkCredential;
            smtpClient.Send(mailMessage);
        }




    }
}
