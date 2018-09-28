using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ticket_management.Services
{
    public class EmailNotificationService
    {
        public void sendmail() { 

            var fromAddress = new MailAddress("saikiran290695@gmail.com", "From saikiran_290695");
            var toAddress = new MailAddress("vasamsettisaikiran95@gmail.com", "To saikiran_95");
            const string fromPassword = "garysai95";
            const string subject = "TestMail";
            string body = "body to be sent";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

                using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
                    Body = body
                })
                
                smtp.Send(message);
        }
    }
}
