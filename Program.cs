using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;

namespace apim_client
{
    class Program
    {
        private const string AppSettingsFile = "appsettings.json";

        private static string _token;
        static async Task Main(string[] args)
        {
            var config = LoadAppSettings();
            var authProvider = new ClientCredentialsAuthProvider(config);

            //authenticate
            await GetAccessToken(authProvider);
            await MakeRequest(config);

            Console.WriteLine("Done!");
        }

        static async Task MakeRequest(AuthenticationConfig config)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key",config.ApimKey);
            var uri = "https://cm-demo-apim.azure-api.net";

            var response = await client.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }

        private static AuthenticationConfig LoadAppSettings()
        {
            var config = new ConfigurationBuilder()
                                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                .AddJsonFile(AppSettingsFile)
                                .AddUserSecrets<Program>()
                                .Build();

            return config.Get<AuthenticationConfig>();
        }

        private static async Task GetAccessToken(ClientCredentialsAuthProvider authProvider)
        {
            try
            {
                _token = await authProvider.GetAccessTokenAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Auth token acquired :)");
                Console.WriteLine(_token);
                Console.WriteLine(string.Empty);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }

            Console.ResetColor();
        }
       
    }
}
