using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using Ebiscon.Common.Environment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static Ebiscon.Common.Environment.EnvironmentHelper;

namespace TestWebApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        private List<string> values = new List<string> { "value1", "value2" };

        //azure
        /* https://www.c-sharpcorner.com/blogs/fetching-secrets-from-key-vault-in-net-console-app */

        /* Azure AppService subscription Id */
        readonly string clientId = "859da05b-1ba9-46b5-af4b-a22b767b5e56";
        /* Azure Vault Uri */
        readonly string baseUri = "https://tasoskeyvaulttest1.vault.azure.net/";
        /* Azure  Client Secret*/
        readonly string clientSecret = "87b8da5f45744e548cedeb4c8d1c7e80";

        [HttpGet]
        [ActionName("getvalues")]
        public IEnumerable<string> Get()
        {
            return values;
        }

        [HttpGet]
        [ActionName("getvaluebyid")]
        public string Get([FromUri] int id)
        {
            return values.ElementAt(id);
        }

        [HttpPost]
        [ActionName("postvalue")]
        public List<string> Post([FromBody] string value)
        {
            values.Add(value);
            return values;
        }

        [HttpGet]
        [ActionName("putvalue")]
        public void Put(int id, [FromBody] string value)
        {
            values.Insert(id, value);
        }

        [HttpGet]
        [ActionName("deletevaluebyid")]
        public void Delete(int id)
        {
            values.RemoveAt(id);
        }

        [HttpGet]
        [ActionName("getsecretstring")]
        public string GetSecretString()
        {
            return EnvironmentHelper.GetSecretString("testSecretJson");
        }

        [HttpGet]
        [ActionName("getserviceconnection")]
        public string GetServiceConnection([FromUri] string connectionName)
        {
            ServiceConnectionSettings settings = EnvironmentHelper.GetServiceConnection(connectionName);
            return JsonConvert.SerializeObject(settings);
        }

        [HttpGet]
        [ActionName("getazuresecret")]
        /* https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/identity/Azure.Identity/README.md */
        public string GetAzureSecret()
        {
            /* https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/keyvault/Azure.Security.KeyVault.Secrets */
            var secretClient = new SecretClient(new Uri(baseUri), new DefaultAzureCredential());
            KeyVaultSecret azureSecret = secretClient.GetSecret("tasospap");
            string azureSecretValue = azureSecret.Value;
            return azureSecretValue;

        }

        [HttpGet]
        [ActionName("getazurekey")]
        public string GetAzureKey()
        {
            /* https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/keyvault/Azure.Security.KeyVault.Keys */
            var keyClient = new KeyClient(new Uri(baseUri), new DefaultAzureCredential());
            KeyVaultKey azureKey = keyClient.GetKey("TestRsaKey");

            JsonWebKey jsonWebKey = azureKey.Key;

            string azureKeyAlg = jsonWebKey.ToRSA().SignatureAlgorithm;
            int azureKeySize = jsonWebKey.ToRSA().KeySize;
            return $"alg= {azureKeyAlg}, size= {azureKeySize}";
        }

        [HttpGet]
        [ActionName("getazurecertificate")]
        public string GetAzureCertificate()
        {
            /* https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/keyvault/Azure.Security.KeyVault.Certificates */
            var certClient = new CertificateClient(new Uri(baseUri), new DefaultAzureCredential());
            KeyVaultCertificateWithPolicy cert = certClient.GetCertificate("TestCertificate");
            int certValue = cert.Cer.Length;
            return certValue.ToString();

        }
    }
}
