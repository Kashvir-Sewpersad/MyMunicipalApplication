//***************************************** start of file **************************************************//


//------------------------------ start of imports ---------------------------------//
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
//---------------------------------- end of imports --------------------------------//
namespace Programming_7312_Part_1.Services
{
    
    
    
    
    //============================================== references ============================================//
    /*
     *
     *
   

            Brevo.com. (2025). Brevo | Email Marketing Software, Automation & CRM. [online] Available at: https://www.brevo.com/?gad_source=1&gad_campaignid=23086271243&gbraid=0AAAAAp4YiPJM7xOPrXsppAUZnGIAu-LvQ&gclid=Cj0KCQjw6bfHBhDNARIsAIGsqLjfE68ZXXIQRwSdE_GX3qLjhs0mMFeMj1Idn_ShXWA3lIatJ3mTXQQaArjtEALw_wcB [Accessed 11 Oct. 2025].
         SendGrid. (2024). Email API - Start for Free | SendGrid. [online] Available at: https://sendgrid.com/en-us/solutions/email-api.
     *
     *
     * 
     */
    //========================================================================================================//
    
    /*
     * the follwing class serves to handle email services for the application
     *
     * it is using an api to send emails to users who filled out the contact form 
     *
     *  this uses smtp to send the emails
     *
     * the api used was recived from  brevo
     *
     * there is 300 free emails per day
     *
     * this is used to send a confirmation email to the user
     *
     *
     * the default email adress is my own so i can monitor the emails being sent and this is monitored through brevos website dashboard 
     *
     * this took 6 hours to implement and test :( ;( :9
     */
    public class EmailService
    {
        private readonly IConfiguration _configuration; // configuration variable

        public EmailService(IConfiguration configuration) // constructor
        {
            _configuration = configuration; // setting the configuration variable
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? toName = null) // method to send email
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                var smtpServer = emailSettings["SmtpServer"];// smtp server variable
                
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587"); // smtp port variable
                
                var senderEmail = emailSettings["SenderEmail"]; 
                
                
                var senderName = emailSettings["SenderName"];
                
                var username = emailSettings["Username"];
                
                var password = emailSettings["Password"];
                 
                var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true"); // enable ssl 

                Console.WriteLine($"Attempting to send email to {toEmail} via {smtpServer}:{smtpPort}"); // logging the attempt to send email this shows up in the terminal 

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password), 
                    EnableSsl = enableSsl,
                    Timeout = 30000 // 30 seconds timeout
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(toEmail, toName)); // adding the to email and name

                await client.SendMailAsync(mailMessage); 
                
                Console.WriteLine($"Email sent successfully to {toEmail}"); // logging success message 
                
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app, you'd use a proper logging framework)
                Console.WriteLine($"Email sending failed: {ex.Message}"); // logging failure message
                
                
                
                
                Console.WriteLine($"Stack trace: {ex.StackTrace}"); // this is for debugging in terminal 
                return false;
            }
        }
        /* 
         * the below is to populate the email body for the contact confirmation email
         * 
         *
         * this is what shows up in the users email 
         * 
         *
         * 
         */
        public async Task<bool> SendContactConfirmationAsync(string recipientEmail, string recipientName, string subject, string message)
        {
            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #357EC7; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .footer {{ background-color: #333; color: white; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Municipal Services</h1>
                            <p>Contact Confirmation</p>
                        </div>
                        <div class='content'>
                            <h2>Dear {recipientName},</h2>
                            <p>Thank you for contacting Municipal Services. We have received your message and will respond to you as soon as possible.</p>

                            <h3>Your Message Details:</h3>
                            <p><strong>Subject:</strong> {subject}</p>
                            <p><strong>Message:</strong></p>
                            <p>{message.Replace("\n", "<br>")}</p>

                            <p>If you have any additional information or need to follow up, please don't hesitate to contact us again.</p>

                            <p>Best regards,<br>
                            Municipal Services Team</p>
                        </div>
                        <div class='footer'>
                            <p>This is an automated confirmation email. Please do not reply to this message.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(recipientEmail, "Contact Confirmation - Municipal Services", emailBody, recipientName); // sending the email
        }

        /*
         * the below is to populate the email body for the issue report confirmation email
         *
         *
         * this is what shows up in the users email
         *
         *
         *
         */
        public async Task<bool> SendIssueConfirmationAsync(string recipientEmail, int issueId, string location, string category, string description)
        {
            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #357EC7; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .footer {{ background-color: #333; color: white; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Municipal Services</h1>
                            <p>Issue Report Confirmation</p>
                        </div>
                        <div class='content'>
                            <h2>Thank you for reporting an issue!</h2>
                            <p>We have received your issue report and will address it as soon as possible.</p>

                            <h3>Your Issue Details:</h3>
                            <p><strong>Tracking Number:</strong> #{issueId}</p>
                            <p><strong>Location:</strong> {location}</p>
                            <p><strong>Category:</strong> {category}</p>
                            <p><strong>Description:</strong></p>
                            <p>{description.Replace("\n", "<br>")}</p>

                            <p>You can track the status of your report by visiting our Service Status page and using your tracking number. We will notify you of any updates via email.</p>

                            <p>Best regards,<br>
                            Municipal Services Team</p>
                        </div>
                        <div class='footer'>
                            <p>This is an automated confirmation email. Please do not reply to this message.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(recipientEmail, "Issue Report Confirmation - Municipal Services", emailBody); // sending the email
        }
    }
}

//***************************************************************** end of file ************************************************//