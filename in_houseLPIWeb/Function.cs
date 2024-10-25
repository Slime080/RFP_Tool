using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace in_houseLPIWeb
{
    public class Function
    {
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder stringBuidler = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                stringBuidler.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuidler.ToString();
        }
        public static void SendEmail(string toEmail, string code)
        {
            try
            {
                Console.WriteLine("Sending Email is on going");

                string smtpServer = "smtp.office365.com";
                int smtpPort = 587;
                //string smtpUser = "e.christian-jay.awitan.slm@lawson-philippines.com";
                //string smtpPass = "#123Lawson";
                string smtpUser = "no-reply.green-system@lawson-philippines.com";
                string smtpPass = "P@ssw0rd";

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(smtpUser);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = "RFP user reset code";
                    mailMessage.Body = @"
                            <html>
                            <body>
                                <p>
                                <pre>Good day sir/ma'am,<br/><br/></pre>
                                    <pre>     We received a request to reset the password for your RFP in-house account. Please use the following confirmation code to proceed: <br/><br/>     Confirmation Code: <strong>" + code + @"</strong></pre>
                                <br/>
                                    <pre>     If you didn't request a password reset, please ignore this email. Someone may have entered your email address by mistake. </pre>
                                </p>
                            </body>
                            </html>
                        ";
                    mailMessage.IsBodyHtml = true;

                    smtpClient.Send(mailMessage);

                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (SmtpException ex)
            {
                // Log the specific details of the SmtpException
                Console.WriteLine($"SMTP Exception: {ex.StatusCode}, {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // Log other exceptions
                Console.WriteLine($"Failed to send email. Exception: {ex.Message}");
            }
        }
    }
}
