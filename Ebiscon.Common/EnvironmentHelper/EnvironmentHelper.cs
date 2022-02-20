using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data.EntityClient;

namespace Ebiscon.Common.Environment
{
    public static class EnvironmentHelper
    {
        private static string _configSuffix = null;
        private static string separator = "-";
        private static ConcurrentDictionary<string, string> appSettingsCache;
        private static ConcurrentDictionary<string, string> appSecretCache;
        private static ConcurrentDictionary<string, string> connectionStringCache;
        private static ConcurrentDictionary<string, ServiceConnectionSettings> serviceConnectionCache;
        private static SecretClient secretClient = null;

        static EnvironmentHelper()
        {

        }

        public static void Initialize()
        {
            appSettingsCache = new ConcurrentDictionary<string, string>();
            appSecretCache = new ConcurrentDictionary<string, string>();
            connectionStringCache = new ConcurrentDictionary<string, string>();
            serviceConnectionCache = new ConcurrentDictionary<string, ServiceConnectionSettings>();

            secretClient = new SecretClient(new Uri(ConfigurationManager.AppSettings["Azure.KeyVault.BaseUri"]), new DefaultAzureCredential());
        }

        public static string ConfigSuffix
        {
            get
            {
                if (_configSuffix == null)
                {

                    //string roleName = Microsoft.Azure.CloudConfigurationManager.GetSetting("EnvironmentType");
                    string roleName = System.Environment.GetEnvironmentVariable("EnvironmentType");

                    if (!String.IsNullOrEmpty(roleName))
                    {
                        _configSuffix = roleName;
                    }
                    else
                    {
                        _configSuffix = System.Environment.MachineName;
                    }
                }

                return _configSuffix;
            }
        }

        //TODO: TP: Put in recycler & in-memory repository recycling
        public static void ClearCache()
        {
            appSettingsCache.Clear();
            connectionStringCache.Clear();
            appSecretCache.Clear();
            serviceConnectionCache.Clear();
        }

        public static ServiceConnectionSettings GetServiceConnection(string connectionName)
        {
            if (!serviceConnectionCache.TryGetValue(connectionName, out ServiceConnectionSettings setting))
            {
                KeyVaultSecret secret = secretClient.GetSecret(connectionName);
                string secretValue = secret.Value;


                if (!string.IsNullOrEmpty(secretValue))
                {
                    setting = JsonConvert.DeserializeObject<ServiceConnectionSettings>(secretValue);
                    serviceConnectionCache.TryAdd(connectionName, setting);
                }
                else
                {
                    string endpoint = ConfigurationManager.AppSettings[connectionName + ".Endpoint" + separator + ConfigSuffix];

                    if (string.IsNullOrEmpty(endpoint))
                    {
                        setting = new ServiceConnectionSettings()
                        {
                            Endpoint = endpoint,
                            AuthKey = ConfigurationManager.AppSettings[connectionName + ".AuthKey" + separator + ConfigSuffix],
                            ConsumerKey = ConfigurationManager.AppSettings[connectionName + ".ConsumerKey" + separator + ConfigSuffix],
                            ConsumerSecret = ConfigurationManager.AppSettings[connectionName + ".ConsumerSecret" + separator + ConfigSuffix],
                            Password = ConfigurationManager.AppSettings[connectionName + ".Password" + separator + ConfigSuffix],
                            RefreshToken = ConfigurationManager.AppSettings[connectionName + ".RefreshToken" + separator + ConfigSuffix],
                            Token = ConfigurationManager.AppSettings[connectionName + ".Token" + separator + ConfigSuffix],
                            Username = ConfigurationManager.AppSettings[connectionName + ".Username" + separator + ConfigSuffix]
                        };
                    }
                    else
                    {
                        endpoint = ConfigurationManager.AppSettings[connectionName + ".Endpoint"];
                        if (!string.IsNullOrEmpty(endpoint))
                        {
                            setting = new ServiceConnectionSettings()
                            {
                                Endpoint = endpoint,
                                AuthKey = ConfigurationManager.AppSettings[connectionName + ".AuthKey"],
                                ConsumerKey = ConfigurationManager.AppSettings[connectionName + ".ConsumerKey"],
                                ConsumerSecret = ConfigurationManager.AppSettings[connectionName + ".ConsumerSecret"],
                                Password = ConfigurationManager.AppSettings[connectionName + ".Password"],
                                RefreshToken = ConfigurationManager.AppSettings[connectionName + ".RefreshToken"],
                                Token = ConfigurationManager.AppSettings[connectionName + ".Token"],
                                Username = ConfigurationManager.AppSettings[connectionName + ".Username"]
                            };
                        }
                    }
                    /* add to cache, even if null to avoid always requesting azure key vault */
                    serviceConnectionCache.TryAdd(connectionName, setting);
                }
            }
            return setting;
        }

        public static string GetConnectionString(string connectionName)
        {
            if (!connectionStringCache.TryGetValue(connectionName, out string connectionString))
            {
                KeyVaultSecret connectionStringSecret = secretClient.GetSecret(connectionName);
                connectionString = connectionStringSecret.Value;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    connectionStringCache.TryAdd(connectionName, connectionString);
                }
                else
                {
                    ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[connectionName + separator + ConfigSuffix];
                    if (setting != null)
                    {
                        connectionString = setting.ConnectionString;
                    }
                    else
                    {
                        connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
                    }
                    connectionStringCache.TryAdd(connectionName, connectionString);
                }
            }
            return connectionString;
        }
        public static string GetSecretString(string key)
        {
            if (!appSecretCache.TryGetValue(key, out string secretValue))
            {
                KeyVaultSecret secret = secretClient.GetSecret(key);
                secretValue = secret.Value;
                string type = secret.Properties.ContentType;
                ;

                if (!string.IsNullOrEmpty(secretValue))
                {
                    appSecretCache.TryAdd(key, secretValue);
                }
                else
                {
                    secretValue = ConfigurationManager.AppSettings[key + separator + ConfigSuffix];
                    if (secretValue == null)
                    {
                        secretValue = ConfigurationManager.AppSettings[key];
                    }
                    appSecretCache.TryAdd(key, secretValue);
                }
            }
            return secretValue;
        }

        public static string GetLinqConnectionConfig(string connectionName)
        {
            return GetConnectionString(connectionName);
        }

        public static string GetAppSetting(string key)
        {
            string setting = GetCachedAppSetting(key + separator + ConfigSuffix);
            if (setting != null)
            {
                return setting;
            }

            return GetCachedAppSetting(key);
        }

        public static bool GetAppSettingAsBool(string key, bool defaultValue = false)
        {
            string setting = GetAppSetting(key);
            if (setting == null)
            {
                return defaultValue;
            }

            return string.Equals(setting, "true", StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(setting, "1");
        }

        public static string GetAppSetting(string key, string defaultValue)
        {
            string result = GetAppSetting(key);
            if (result != null)
            {
                return result;
            }
            return defaultValue;
        }

        public static string GetEntityConnectionString(string connectionName)
        {
            string entityConnectionString = GetConnectionString(connectionName);
            return new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
        }

        public static ServiceConnectionSettings CreateServiceConnectionSetting
            (string connectionName, string endpoint, string authKey, string consumerKey, string consumerSecret, string password, string refreshToken, string token, string username)
        {
            ServiceConnectionSettings setting = new ServiceConnectionSettings()
            {
                Endpoint = endpoint,
                AuthKey = authKey,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                Password = password,
                RefreshToken = refreshToken,
                Token = token,
                Username = username
            };

            string serializedSettings = JsonConvert.SerializeObject(setting);
            /* add to azure secret vault */

            bool success = true;
            if (!success)
            {
                return null;
            }

            serviceConnectionCache.TryAdd(connectionName, setting);
            return setting;
        }

        private static string GetCachedAppSetting(string key)
        {
            return appSettingsCache.GetOrAdd(key, ConfigurationManager.AppSettings[key]);
        }

        public class ServiceConnectionSettings
        {
            public string Endpoint { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AuthKey { get; set; }
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
