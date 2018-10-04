#region MS Directives
using System.Net; 
using System.Net.Mail;
using System.Threading.Tasks;
#endregion

namespace ticket_management.Services
{
    public class EmailNotificationService
    {
        public void Sendmail(string fromEmail, string toEmail, string Password, string Subject, string Body, string from, string to)
        {
            var fromAddress = new MailAddress(fromEmail, from);
            var toAddress = new MailAddress(toEmail, to);
            string fromPassword = Password;
            string subject = Subject;
            string body = Body;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            message.IsBodyHtml = true;
            smtp.SendAsync(message, null);
            
        }
    }
}

