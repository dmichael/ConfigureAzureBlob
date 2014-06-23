
// For reference see
// http://stackoverflow.com/questions/23596917/set-cors-on-windows-azure-blob-storage-using-asp-net 


using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

using Newtonsoft.Json;

namespace ConfigureAzureBlob
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Configuring CORS on the Windows Azure BLOB service");
            SetsServiceProperties(new Uri(ConfigurationManager.AppSettings["BlobEndpoint"]), ConfigurationManager.AppSettings["AccountName"], ConfigurationManager.AppSettings["AccountKey"]);
        }

        static void SetsServiceProperties(Uri blobEndpoint, string accountName, string accountKey)
        {
           var client = new Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient(blobEndpoint, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, accountKey));

            // Set the service properties.
            ServiceProperties serviceProperties = new ServiceProperties();
            serviceProperties.DefaultServiceVersion = "2013-08-15";
            serviceProperties.Cors.CorsRules.Clear();
            serviceProperties.Cors.CorsRules.Add(new CorsRule(){
                AllowedHeaders = new List<string>() {"*"},
                ExposedHeaders = new List<string>() {"*"},
                AllowedMethods = CorsHttpMethods.Delete | CorsHttpMethods.Put,
                AllowedOrigins = new List<string>() { "*" },
                MaxAgeInSeconds = 3600
            });

            serviceProperties.Logging.Version = "1.0";
            serviceProperties.HourMetrics.Version = "1.0";
            serviceProperties.MinuteMetrics.Version = "1.0";
           
            client.SetServiceProperties(serviceProperties);
            ServiceProperties blobServiceProperties = client.GetServiceProperties();

            var blobPropertiesStringified = Newtonsoft.Json.JsonConvert.SerializeObject(blobServiceProperties, Formatting.Indented); ;
            Console.WriteLine("Current Properties:\n{0}", blobPropertiesStringified);
            Console.ReadLine();
        }
    }
}
