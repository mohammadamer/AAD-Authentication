using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureSqlOAuthConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Thread(Run);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        static void Run()
        {
            AuthenticationResult authenticationResult = null;

            string errorMessage = null;
            try
            {
                Console.WriteLine("Trying to acquire token");
                authenticationResult = OAuthHelper.GetAuthenticationHeader(true);

            }
            catch (AdalException ex)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\nInnerException : " + ex.InnerException.Message;
                }
            }
            catch (ArgumentException ex)
            {
                errorMessage = ex.Message;
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine("Failed: {0}" + errorMessage);
                return;
            }
            Console.WriteLine("\nMaking the protocol call\n");


            Console.WriteLine(authenticationResult.AccessToken);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            //Data source name ex: tcp:xyz.database.windows.net,1433
            builder["Data Source"] = "Your Data Source Name";
            //Database Name ex: azureDb
            builder["Initial Catalog"] = "Database Name";
            builder["Connect Timeout"] = 30;

            string accessToken = authenticationResult.AccessToken;
            if (accessToken == null)
            {
                Console.WriteLine("Fail to acuire the token to the database.");
            }
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.AccessToken = accessToken;
                    Console.WriteLine("-------------------------");
                    connection.Open();
                    Console.WriteLine("Connected to the database");
                    Console.WriteLine("-------------------------");

                    SqlDataReader reader;
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    //Your Select statement ex: "Select * From dbo.[Xyz]";
                    cmd.CommandText = "Select statement...";

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetValue(1));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("Please press any key to stop");
            Console.ReadKey();



        }

    }
}
