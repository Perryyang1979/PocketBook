
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;

namespace PocketBookClient.Helper
{
    public class Utils
    {
        public static string SuperMarket = "Supermarket";
        public static string ShoppingMall = "ShoppingMall";
        public static string Insurance = "Insurance";
        public static string Salary = "Salary";
        public static string Lunch = "Lunch";
        public static string Parking = "Parking";

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

        private static  bool SendEmail(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add( new MailboxAddress("PocketBook Admin", "eguyang@gmail.com"));
                message.To.Add( MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    //   client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                      client.CheckCertificateRevocation = false;
                    //    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    //await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    //await client.AuthenticateAsync("eguyang@gmail.com", "jzkb ktet zsyk igzl");
                    //await client.SendAsync(message);
                    //await client.DisconnectAsync(true);
                     client.Connect("smtp.gmail.com", 465, true);
                     client.Authenticate("eguyang@gmail.com", "jzkb ktet zsyk igzl");
                     client.Send(message);
                     client.Disconnect(true);
                }
                return true; // Redirect to a success page
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool SendNewPasswordEmail(string to, string password)
        {
            var subject = "Reset Password";
            var stringBody = $"Hi:\n your password is reset, and new password is {password}.\n Best regards \n PocketBook team";

            return  SendEmail(to, subject, stringBody);
        }


        public static async Task<bool> SendNewPasswordByEmail(string to, string password)
        {
            var message = new EmailMessage()
            {
                To = new List<string>() { to },
                Subject = "Reset Password",
                Body = $"your password is reset , and new password is {password}\n Best Regards\n Pocket Team"
            };

            try
            {
                await Microsoft.Maui.ApplicationModel.Communication.Email.Default.ComposeAsync(message);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }

}
