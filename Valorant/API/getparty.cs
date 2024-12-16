﻿using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using Valorant.API;

namespace ValAPINet
{
    public class PlayerPartyInfo
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
        public static PlayerPartyInfo GetPlayerParty(Auth au, string puuid)
        {
            var ret = new PlayerPartyInfo();
            string url = "https://glz-" + au.region + "-1." + au.region + ".a.pvp.net/parties/v1/players/" + puuid;
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
                return JsonConvert.DeserializeObject<PlayerPartyInfo>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at getparty.cs");
                return null;
            }
        }
    }
}
