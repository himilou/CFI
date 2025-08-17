using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace CFI.Pages
{
    public class SendEmailModel : PageModel
    {
        
        public string smtpuser;
        private string smtpPwd;
        private string mailRecipient;
        private MailAddress senderAddress;
        public bool mailSent = false;

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string? Message { get; set; }

    
        public SendEmailModel(IConfiguration configuration)
        {
            smtpuser = configuration.GetSection("SMTP")["SmtpUser"];
            smtpPwd = configuration.GetSection("SMTP")["SmtpPass"];
            mailRecipient = configuration.GetSection("SMTP")["SmtpTarget"];
            senderAddress = new MailAddress(smtpuser);
        }
        public void OnGet()
        {
            Name = "wtf";
        }

        public void OnPost()
        {
            bool error = false;
            bool success = false;
            if ((String.IsNullOrEmpty(Email)) || Email.Length > 254)
                error = true;
            
            if (String.IsNullOrEmpty(Name) || Name.Length > 254)
                error = true;
            
            if (String.IsNullOrEmpty(Message) || Message.Length > 512)
                error = true;
            if (!error)
            {
                string subject = "Information request from instruction.flymetoatp.com";
                string body = "Name: " + Name + "\n" +
                                "Email: " + Email + "\n" +
                                "Message: " + "\n" + Message;

                 success = SendSmtp(mailRecipient, subject, body);
            }

            if(success &&(!error))
                mailSent = true;
            else
                mailSent = false;

           
        }
        private bool SendSmtp(string mailrecipient, string subject, string body)
        {
            bool success = true;
            MailAddress reciever = new MailAddress(mailrecipient);

            SmtpClient client = new SmtpClient("smtp.maileroo.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(smtpuser, smtpPwd);
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage(senderAddress, reciever);
                mail.Subject = subject;
                mail.Body = body;

                client.Send(mail);
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex.Message);
            }
            return success;
        }

    }
}
