
using MimeKit;
using MailKit.Net.Smtp;

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
            var smtpUser = SecretConfiguration.GetRequired(SecretConfiguration.SmtpUserKey, SecretConfiguration.SmtpUserEnv);
            var smtpPassword = SecretConfiguration.GetRequired(SecretConfiguration.SmtpPasswordKey, SecretConfiguration.SmtpPasswordEnv);
            var fromName = SecretConfiguration.Get(SecretConfiguration.SmtpFromNameKey, SecretConfiguration.SmtpFromNameEnv);
            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = "PocketBook Admin";
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
                message.From.Add( new MailboxAddress(fromName, smtpUser));
                message.To.Add( MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                      client.CheckCertificateRevocation = false;
                     client.Connect(host, port, true);
                     client.Authenticate(smtpUser, smtpPassword);
                     client.Send(message);
                     client.Disconnect(true);
                }
                return true; // Redirect to a success page
            }
            catch (Exception)
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
            catch(Exception)
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
