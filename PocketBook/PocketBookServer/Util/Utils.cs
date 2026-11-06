using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using PocketBookClient.Helper;

namespace PocketBookServer.Util
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

        [HttpPost]
        private static async Task<bool> SendEmail(string to, string subject, string body)
        {
            var smtpUser = SecretConfiguration.GetRequired(SecretConfiguration.SmtpUserKey, SecretConfiguration.SmtpUserEnv);
            var smtpPassword = SecretConfiguration.GetRequired(SecretConfiguration.SmtpPasswordKey, SecretConfiguration.SmtpPasswordEnv);
            var fromName = SecretConfiguration.Get(SecretConfiguration.SmtpFromNameKey, SecretConfiguration.SmtpFromNameEnv);
            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = "PocketAdmin";
            }

            var host = SecretConfiguration.Get(SecretConfiguration.SmtpHostKey, SecretConfiguration.SmtpHostEnv);
            if (string.IsNullOrWhiteSpace(host))
            {
                host = "smtp.gmail.com";
            }

            var portText = SecretConfiguration.Get(SecretConfiguration.SmtpPortKey, SecretConfiguration.SmtpPortEnv);
            var port = 465;
            if (!string.IsNullOrWhiteSpace(portText) && !int.TryParse(portText, out port))
            {
                port = 465;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, smtpUser));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(host, port, true);
                    await client.AuthenticateAsync(smtpUser, smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true; // Redirect to a success page
            }catch(Exception) {
                return false;
            }
        }

        public static async Task<bool> SendNewPasswordEmail(string to,string password)
        {
            var subject = "Reset Password";
            var stringBody = $"new password is {password}";

            return await SendEmail(to, subject, stringBody);
        }
    }
}
