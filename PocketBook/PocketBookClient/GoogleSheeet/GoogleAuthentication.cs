using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public static async Task<UserCredential> LoginAsync(string googleClientId, string googleClientSecret, string[] scopes)
        {
            ClientSecrets clientSecrets = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret,
            };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "user", CancellationToken.None);
        }

        public static async Task<GoogleCredential> ReadTextFile(string filePath)
        {
            using (Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filePath))
            {
                GoogleCredential credential;
                credential = GoogleCredential.FromStream(fileStream).CreateScoped(Scopes);
                return credential;
            };
        }

        public static GoogleCredential LoginCredential()
        {
            GoogleCredential credential = ReadTextFile($"googlesheetcredentials.json").Result;

            //using (FileStream stream = new FileStream($"googlesheetcredentials.json", FileMode.Open, FileAccess.ReadWrite))
            //{
            //    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            //}

            return credential;
        }
    }
}
