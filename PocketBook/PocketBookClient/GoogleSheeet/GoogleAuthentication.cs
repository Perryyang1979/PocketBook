using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using PocketBookClient.Helper;
using System.Text;

namespace PocketBookClient.GoogleSheeet
{
    public static class GoogleAuthentication
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        public static UserCredential Login(string googleClientId, string googleClientSecret, string[] scopes)
        {
            ClientSecrets clientSecrets = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret,
            };

            return GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "user", CancellationToken.None).Result;
        }

        public static UserCredential Login(string[] scopes)
        {
            var googleClientId = SecretConfiguration.GetRequired(SecretConfiguration.GoogleClientIdKey, SecretConfiguration.GoogleClientIdEnv);
            var googleClientSecret = SecretConfiguration.GetRequired(SecretConfiguration.GoogleClientSecretKey, SecretConfiguration.GoogleClientSecretEnv);
            return Login(googleClientId, googleClientSecret, scopes);
        }

        public static async Task<UserCredential> LoginAsync(string googleClientId, string googleClientSecret, string[] scopes)
        {
            ClientSecrets clientSecrets = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret,
            };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "user", CancellationToken.None);
        }

        public static Task<UserCredential> LoginAsync(string[] scopes)
        {
            var googleClientId = SecretConfiguration.GetRequired(SecretConfiguration.GoogleClientIdKey, SecretConfiguration.GoogleClientIdEnv);
            var googleClientSecret = SecretConfiguration.GetRequired(SecretConfiguration.GoogleClientSecretKey, SecretConfiguration.GoogleClientSecretEnv);
            return LoginAsync(googleClientId, googleClientSecret, scopes);
        }

        public static GoogleCredential LoginCredential()
        {
            var json = SecretConfiguration.Get(SecretConfiguration.GoogleCredentialsJsonKey, SecretConfiguration.GoogleCredentialsJsonEnv);
            if (!string.IsNullOrWhiteSpace(json))
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }
            }

            var path = SecretConfiguration.FindGoogleCredentialsPath();
            if (!string.IsNullOrWhiteSpace(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    return GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }
            }

            throw new InvalidOperationException(
                "Google Sheets service-account credentials were not found. " +
                "Set POCKETBOOK_GOOGLE_CREDENTIALS_JSON or POCKETBOOK_GOOGLE_CREDENTIALS_PATH, " +
                "or place a gitignored googlesheetcredentials.json next to the app.");
        }
    }
}
