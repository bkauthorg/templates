using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using Newtonsoft.Json.Linq;

namespace NetCoreConsoleCodeFlow
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static OidcClient _oidcClient;
        private static HttpClient _apiClient;
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static string Authority { get; set; }
        public static string ClientId { get; set; }
        public static string TestApi { get; set; }
        public static string Tenant { get; set; }
        public static string Scope { get; set; }

        public static async Task MainAsync()
        {
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("|  Sign in with OIDC    |");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("");
            Console.WriteLine("Press any key to sign in...");
            Console.ReadKey();

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Authority = config.GetSection("authority").Value;
            TestApi = config.GetSection("testApi").Value;
            ClientId = config.GetSection("clientId").Value;
            Tenant = config.GetSection("authority").Value;
            Scope = config.GetSection("scope").Value;

            _apiClient = new HttpClient { BaseAddress = new Uri(TestApi) };

            await SignIn();
        }



        private static async Task SignIn()
        {
            // create a redirect URI using an available port on the loopback address.
            // requires the OP to allow random ports on 127.0.0.1 - otherwise set a static port
            var browser = new SystemBrowser();
            string redirectUri = string.Format($"http://127.0.0.1:{browser.Port}");

         

            var options = new OidcClientOptions
            {
                Authority = Authority,
                ClientId = ClientId,
                RedirectUri = redirectUri,
                Scope = $"openid profile {Scope}",
                FilterClaims = false,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
            };

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync(extraParameters: new { acr_values = $"tenant:{Tenant}"});

            ShowResult(result);
            await NextSteps(result);
        }
        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"access token:   {result.AccessToken}");
            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
        }

        private static async Task NextSteps(LoginResult result)
        {
            var currentAccessToken = result.AccessToken;
            var currentRefreshToken = result.RefreshToken;

            var menu = "  x...exit  c...call api   o..logout  ";
            if (currentRefreshToken != null) menu += "r...refresh token   ";

            while (true)
            {
                Console.WriteLine("\n\n");

                Console.Write(menu);
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.X) return;
                if (key.Key == ConsoleKey.C) await CallApi(currentAccessToken);
                if (key.Key == ConsoleKey.O) await _oidcClient.LogoutAsync();
                if (key.Key == ConsoleKey.R)
                {
                    var refreshResult = await _oidcClient.RefreshTokenAsync(currentRefreshToken);
                    if (result.IsError)
                    {
                        Console.WriteLine($"Error: {refreshResult.Error}");
                    }
                    else
                    {
                        currentRefreshToken = refreshResult.RefreshToken;
                        currentAccessToken = refreshResult.AccessToken;

                        Console.WriteLine("\n\n");
                        Console.WriteLine($"access token:   {result.AccessToken}");
                        Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
                    }
                }
            }
        }

        private static async Task CallApi(string currentAccessToken)
        {
            _apiClient.SetBearerToken(currentAccessToken);
            var response = await _apiClient.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var json = JArray.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine("\n\n");
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }
    }
}
