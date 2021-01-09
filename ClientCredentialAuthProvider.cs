using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace apim_client
{
    public class ClientCredentialsAuthProvider
    {
        private readonly IConfidentialClientApplication msalClient;
        private readonly string[] scopes;

        public ClientCredentialsAuthProvider(AuthenticationConfig config)
        {
            scopes = new string[] { config.Scopes };

            msalClient = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(AadAuthorityAudience.AzureAdMyOrg, true)
                .WithTenantId(config.TenantId)
                .Build();
        }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var result = await msalClient.AcquireTokenForClient(scopes)
                    .ExecuteAsync();

                return result.AccessToken;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error getting access token: {exception.Message}");
                return null;
            }

        }

        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessTokenAsync());
        }
    }
}