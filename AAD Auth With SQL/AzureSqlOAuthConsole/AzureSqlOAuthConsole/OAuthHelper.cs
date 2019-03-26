using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSqlOAuthConsole
{
    public class OAuthHelper
    {
        public static AuthenticationResult GetAuthenticationHeader(bool useWebAppAuthentication)
        {
            string aadTenant = ConfigurationManager.AppSettings["ActiveDirectoryTenant"];
            string aadClientAppId = ConfigurationManager.AppSettings["ActiveDirectoryClientAppId"];
            string aadClientAppSecret = ConfigurationManager.AppSettings["ActiveDirectoryClientAppSecret"];
            string aadResource = ConfigurationManager.AppSettings["ActiveDirectoryResource"];

            string authority = ConfigurationManager.AppSettings["AuthorizationUri"].Replace("common", "TenantName.onmicrosoft.com");
            AuthenticationContext authenticationContext = new AuthenticationContext(authority, false);
            AuthenticationResult authenticationResult;

            if (string.IsNullOrEmpty(aadClientAppSecret))
            {
                Console.WriteLine("Please fill AAD application secret in ClientConfiguration if you choose authentication by the application.");
                throw new Exception("Failed OAuth by empty application secret.");
            }

            try
            {
       

                // OAuth through application by application id and application secret.
                var creadential = new ClientCredential(aadClientAppId, aadClientAppSecret);
                authenticationResult = authenticationContext.AcquireTokenAsync(aadResource, creadential).Result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to authenticate with AAD by application with exception {0} and the stack trace {1}", ex.ToString(), ex.StackTrace));
                throw new Exception("Failed to authenticate with AAD by application.");
            }

            return authenticationResult;

        }

        public static string ConnectionStringBuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //Data source name ex: tcp:xyz.database.windows.net,1433
            builder["Data Source"] = "Your Data Source Name";
            //Database Name ex: azureDb
            builder["Initial Catalog"] = "Database Name";
            builder["Connect Timeout"] = 30;

            return builder.ConnectionString;
        }
    }
}
