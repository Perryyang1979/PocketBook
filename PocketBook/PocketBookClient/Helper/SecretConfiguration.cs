using System.Collections.Concurrent;
using System.Text.Json;

namespace PocketBookClient.Helper
{
    /// <summary>
    /// Loads secrets from environment variables, a gitignored local JSON file,
    /// or .NET user-secrets files. Never commit real values.
    /// </summary>
    public static class SecretConfiguration
    {
        public const string GoogleClientIdKey = "Google:ClientId";
        public const string GoogleClientSecretKey = "Google:ClientSecret";
        public const string GoogleSpreadsheetIdKey = "Google:SpreadsheetId";
        public const string GoogleCredentialsJsonKey = "Google:CredentialsJson";
        public const string GoogleCredentialsPathKey = "Google:CredentialsPath";
        public const string SmtpHostKey = "Smtp:Host";
        public const string SmtpPortKey = "Smtp:Port";
        public const string SmtpUserKey = "Smtp:User";
        public const string SmtpPasswordKey = "Smtp:Password";
        public const string SmtpFromNameKey = "Smtp:FromName";
        public const string RsaPublicKeyKey = "Rsa:PublicKey";
        public const string RsaPrivateKeyKey = "Rsa:PrivateKey";

        public const string GoogleClientIdEnv = "POCKETBOOK_GOOGLE_CLIENT_ID";
        public const string GoogleClientSecretEnv = "POCKETBOOK_GOOGLE_CLIENT_SECRET";
        public const string GoogleSpreadsheetIdEnv = "POCKETBOOK_GOOGLE_SPREADSHEET_ID";
        public const string GoogleCredentialsJsonEnv = "POCKETBOOK_GOOGLE_CREDENTIALS_JSON";
        public const string GoogleCredentialsPathEnv = "POCKETBOOK_GOOGLE_CREDENTIALS_PATH";
        public const string SmtpHostEnv = "POCKETBOOK_SMTP_HOST";
        public const string SmtpPortEnv = "POCKETBOOK_SMTP_PORT";
        public const string SmtpUserEnv = "POCKETBOOK_SMTP_USER";
        public const string SmtpPasswordEnv = "POCKETBOOK_SMTP_PASSWORD";
        public const string SmtpFromNameEnv = "POCKETBOOK_SMTP_FROM_NAME";
        public const string RsaPublicKeyEnv = "POCKETBOOK_RSA_PUBLIC_KEY";
        public const string RsaPrivateKeyEnv = "POCKETBOOK_RSA_PRIVATE_KEY";
        public const string SecretsFileEnv = "POCKETBOOK_SECRETS_FILE";

        // Must match the UserSecretsId values in the client and server csproj files.
        private static readonly string[] UserSecretsIds =
        {
            "c3e8a91f-4b27-4d6e-9c1a-7f2e5b8d0a44",
            "a19d4c72-8e35-4f0b-b6a1-3c7e9d2f5b80"
        };

        private static readonly ConcurrentDictionary<string, string> ConfigurationFallbacks = new();
        private static readonly object FileCacheLock = new();
        private static List<JsonDocument>? _cachedDocuments;

        public static void ApplyConfigurationValue(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            ConfigurationFallbacks[key] = value;
        }

        public static string Get(string configurationKey, params string[] environmentNames)
        {
            foreach (var name in environmentNames)
            {
                var fromEnv = TryGetEnvironment(name);
                if (fromEnv != null)
                {
                    return fromEnv;
                }
            }

            var aspNetName = configurationKey.Replace(":", "__");
            var fromAspNetEnv = TryGetEnvironment(aspNetName);
            if (fromAspNetEnv != null)
            {
                return fromAspNetEnv;
            }

            var fromFiles = TryGetFromLocalFiles(configurationKey);
            if (fromFiles != null)
            {
                return fromFiles;
            }

            if (ConfigurationFallbacks.TryGetValue(configurationKey, out var fallback) &&
                !string.IsNullOrWhiteSpace(fallback))
            {
                return fallback;
            }

            return string.Empty;
        }

        public static string GetRequired(string configurationKey, params string[] environmentNames)
        {
            var value = Get(configurationKey, environmentNames);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var envHint = environmentNames.Length > 0
                ? string.Join(" or ", environmentNames)
                : configurationKey.Replace(":", "__");

            throw new InvalidOperationException(
                $"Missing required configuration '{configurationKey}'. " +
                $"Set {envHint}, use .NET user secrets, or add a gitignored appsettings.local.json.");
        }

        public static string? FindGoogleCredentialsPath()
        {
            var configured = Get(GoogleCredentialsPathKey, GoogleCredentialsPathEnv);
            if (!string.IsNullOrWhiteSpace(configured) && File.Exists(configured))
            {
                return Path.GetFullPath(configured);
            }

            foreach (var directory in EnumerateSearchDirectories())
            {
                var candidate = Path.Combine(directory, "googlesheetcredentials.json");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static string? TryGetEnvironment(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static string? TryGetFromLocalFiles(string configurationKey)
        {
            foreach (var document in GetJsonDocuments())
            {
                var value = ReadJsonValue(document.RootElement, configurationKey);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        private static List<JsonDocument> GetJsonDocuments()
        {
            if (_cachedDocuments != null)
            {
                return _cachedDocuments;
            }

            lock (FileCacheLock)
            {
                if (_cachedDocuments != null)
                {
                    return _cachedDocuments;
                }

                var documents = new List<JsonDocument>();
                foreach (var path in EnumerateSecretFilePaths())
                {
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    try
                    {
                        var json = File.ReadAllText(path);
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            continue;
                        }

                        documents.Add(JsonDocument.Parse(json));
                    }
                    catch (JsonException)
                    {
                        // Ignore unreadable local files and keep searching.
                    }
                    catch (IOException)
                    {
                        // Ignore unreadable local files and keep searching.
                    }
                }

                _cachedDocuments = documents;
                return _cachedDocuments;
            }
        }

        private static IEnumerable<string> EnumerateSecretFilePaths()
        {
            var fromEnv = Environment.GetEnvironmentVariable(SecretsFileEnv);
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                yield return fromEnv;
            }

            foreach (var directory in EnumerateSearchDirectories())
            {
                yield return Path.Combine(directory, "appsettings.local.json");
                yield return Path.Combine(directory, "secrets.json");
            }

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrWhiteSpace(userProfile))
            {
                foreach (var id in UserSecretsIds)
                {
                    yield return Path.Combine(userProfile, ".microsoft", "usersecrets", id, "secrets.json");
                }
            }
        }

        private static IEnumerable<string> EnumerateSearchDirectories()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var directory in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
            {
                if (!string.IsNullOrWhiteSpace(directory) && seen.Add(directory))
                {
                    yield return directory;
                }
            }
        }

        private static string? ReadJsonValue(JsonElement root, string key)
        {
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (TryGetPropertyIgnoreCase(root, key, out var flatValue))
            {
                return ToStringValue(flatValue);
            }

            var current = root;
            foreach (var part in key.Split(':'))
            {
                if (!TryGetPropertyIgnoreCase(current, part, out current))
                {
                    return null;
                }
            }

            return ToStringValue(current);
        }

        private static bool TryGetPropertyIgnoreCase(JsonElement element, string name, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        private static string? ToStringValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    var text = element.GetString();
                    return string.IsNullOrWhiteSpace(text) ? null : text;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.ToString();
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    var raw = element.GetRawText();
                    return string.IsNullOrWhiteSpace(raw) ? null : raw;
                default:
                    return null;
            }
        }
    }
}
