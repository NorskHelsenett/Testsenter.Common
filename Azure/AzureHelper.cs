using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Shared.AzureCommon
{
    public static class AzureHelper
    {
        public static bool RunningInAzure()
        {
            return !AssemblyDirectory.ToLower().Contains("c:");
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static X509Certificate2 GetCertificateInLocalAzureStore(string thumbprint)
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            if (certCollection.Count > 0)
                return certCollection[0];

            throw new Exception("Could not find certificate with thumbprint " + thumbprint);
        }

        public static void ReplaceWebJobConnectionStrings(string storageAccountConnectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

            connectionStringsSection.ConnectionStrings["AzureWebJobsStorage"].ConnectionString = storageAccountConnectionString;
            connectionStringsSection.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString = storageAccountConnectionString;

            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}
