using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valorant.API
{
    public class GamePlayer
    {
        public class MatchPlayerInfo
        {
            public string Subject { get; set; }
            public string MatchID { get; set; }
            public long Version { get; set; }
            public int StatusCode { get; set; }
        }

        public class PartyPlayerInfo
        {
            public string Subject { get; set; }
            public long Version { get; set; }
            public string CurrentPartyID { get; set; }
            public object Invites { get; set; }
            public List<object> Requests { get; set; }
            public PlatformInfoobj PlatformInfo { get; set; }
            public int StatusCode { get; set; }
            public class PlatformInfoobj
            {
                public string platformType { get; set; }
                public string platformOS { get; set; }
                public string platformOSVersion { get; set; }
                public string platformChipset { get; set; }
            }
        }

        public static MatchPlayerInfo GetMatchPlayer(Auth au, string playerid = "useauth")
        {
            if (playerid == "useauth")
            {
                playerid = au.subject;
            }
            string url = "https://glz-" + au.region + "-1." + au.region + ".a.pvp.net/core-game/v1/players/" + playerid;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");

            try
            {
                return JsonConvert.DeserializeObject<MatchPlayerInfo>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at GamePlayer.cs");
                return null;
            }
        }

        public static PartyPlayerInfo GetPartyPlayer(Auth au)
        {
            PartyPlayerInfo ret = new PartyPlayerInfo();
            string url = "https://glz-" + au.region + "-1." + au.region + ".a.pvp.net/parties/v1/players/" + au.subject;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");


            try
            {
                return JsonConvert.DeserializeObject<PartyPlayerInfo>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at GamePlayer.cs");
                return null;
            }
        }

        public static MatchPlayerInfo GetPreGamePlayer(Auth au, string playerid = "useauth")
        {
            MatchPlayerInfo ret = new MatchPlayerInfo();
            if (playerid == "useauth")
            {
                playerid = au.subject;
            }
            string url = "https://glz-" + au.region + "-1." + au.region + ".a.pvp.net/pregame/v1/players/" + playerid;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;
            //client.CookieContainer = new CookieContainer();

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");

            try
            {
                return JsonConvert.DeserializeObject<MatchPlayerInfo>(client.Execute(request).Content); ;
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at GamePlayer.cs");
                return null;
            }
        }
    }
}
