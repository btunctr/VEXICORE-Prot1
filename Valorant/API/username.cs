using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Valorant.API
{
    public class Username
    {
        public string DisplayName { get; set; }
        public string Subject { get; set; }
        public string GameName { get; set; }
        public string TagLine { get; set; }
        public int StatusCode { get; set; }

        public static List<Username> GetUsernames(Auth au, string[] playerid = null)
        {
            if (playerid == null)
            {
                playerid = new string[] { au.subject };
            }
            string url = "https://pd." + au.region + ".a.pvp.net/name-service/v2/players";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");
            request.AddJsonBody(playerid);

            List<Username> list;
            try
            {
                list = JsonConvert.DeserializeObject<List<Username>>(client.Execute(request).Content);
            } catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at Username.cs");
                return null;
            }

            return list;
        }
    }
}
