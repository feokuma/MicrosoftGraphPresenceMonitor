using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace MicrosoftToken
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tenantId = "[TENANT_ID]";
            var clientId = "[CLIENT_ID]";
            var scopes = new string[] { "https://graph.microsoft.com/.default", "offline_access" };
            var status = 0;
            var lastActivity = "OffWork";
            HttpResponseMessage responseMessage;


            var publicClientApplication = PublicClientApplicationBuilder
                            .Create(clientId)
                            .WithTenantId(tenantId)
                            .WithRedirectUri($"http://localhost")
                            .Build();

            var authenticationResult = await publicClientApplication
                .AcquireTokenInteractive(scopes)
                .WithUseEmbeddedWebView(false)
                .ExecuteAsync();

            var accounts = await publicClientApplication.GetAccountsAsync();

            var presenceUri = "https://graph.microsoft.com/beta/me/presence"
                                .WithOAuthBearerToken(authenticationResult.AccessToken);

            while (true)
            {
                try
                {
                    responseMessage = await presenceUri.GetAsync();

                    var presence = JsonConvert
                        .DeserializeObject<Presence>(await responseMessage.Content.ReadAsStringAsync());

                    if (lastActivity != presence.Activity)
                    {
                        if ((presence.Activity == "Away")
                            || (presence.Activity == "BeRightBack")
                            || (presence.Activity == "OffWork"))
                            status = 3;
                        else if ((presence.Activity == "Busy")
                            || (presence.Activity == "DoNotDisturb"))
                            status = 1;
                        else if (presence.Activity == "Available")
                            status = 2;

                        var data = new { numLeds = 1, status };

                        _ = await "http://192.168.15.53/leds".PostJsonAsync(data);
                        lastActivity = presence.Activity;
                        Console.WriteLine(JsonConvert.SerializeObject(presence, Formatting.Indented));
                    }
                }
                catch (FlurlHttpException ex)
                {
                    authenticationResult = await publicClientApplication.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .WithAuthority(publicClientApplication.Authority)
                        .ExecuteAsync();
                    presenceUri = "https://graph.microsoft.com/beta/me/presence"
                        .WithOAuthBearerToken(authenticationResult.AccessToken);
                    Console.WriteLine("TOKEN REFRESHED");
                }

                Thread.Sleep(1000);
            }
        }
    }

    internal class Presence
    {
        [JsonProperty("@odata.context")]
        public string DataContext { get; set; }

        public string Id { get; set; }

        public string Availability { get; set; }

        public string Activity { get; set; }
    }
}
