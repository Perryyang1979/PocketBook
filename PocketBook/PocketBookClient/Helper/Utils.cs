
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;

namespace PocketBookClient.Helper
{
    public class Utils
    {
        public static string GetRandomPassword(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:'\",.<>?/"; // add your special characters here
            Random random = new Random();
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }

            return new string(password);
        }

        private static async Task<bool> SendEmail(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("PocketAdmin", "eguyang@gmail.com"));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                 //   client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                    client.CheckCertificateRevocation = false;
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync("eguyang@gmail.com", "jzkb ktet zsyk igzl");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true; // Redirect to a success page
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> SendNewPasswordEmail(string to, string password)
        {
            var subject = "Reset Password";
            var stringBody = $"new password is {password}";

            return await SendEmail(to, subject, stringBody);
        }
    }

}
