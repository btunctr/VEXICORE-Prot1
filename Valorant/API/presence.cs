using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Valorant;

namespace Valorant.API
{
    public class UserPresence
    {
        public List<Presence> presences;
        public class Presence
        {
            public string actor { get; set; }
            public string championId { get; set; }
            public string basic { get; set; }
            public string details { get; set; }
            public string game_name { get; set; }
            public string game_tag { get; set; }
            public string location { get; set; }
            public string msg { get; set; }
            public string name { get; set; }
            public object patchline { get; set; }
            public string pid { get; set; }
            public object platform { get; set; }
            public string @private { get; set; }
            public object privateJwt { get; set; }
            public string product { get; set; }
            public string puuid { get; set; }
            public string region { get; set; }
            public string resource { get; set; }
            public string state { get; set; }
            public string summary { get; set; }
            public object time { get; set; }
            public priv privinfo { get; set; }
        }
        public class priv
        {
            public bool isValid { get; set; }
            public string sessionLoopState { get; set; }
            public string partyOwnerSessionLoopState { get; set; }
            public string customGameName { get; set; }
            public string customGameTeam { get; set; }
            public string partyOwnerMatchMap { get; set; }
            public string partyOwnerMatchCurrentTeam { get; set; }
            public int partyOwnerMatchScoreAllyTeam { get; set; }
            public int partyOwnerMatchScoreEnemyTeam { get; set; }
            public string partyOwnerProvisioningFlow { get; set; }
            public string provisioningFlow { get; set; }
            public string matchMap { get; set; }
            public string partyId { get; set; }
            public bool isPartyOwner { get; set; }
            public string partyName { get; set; }
            public string partyState { get; set; }
            public string partyAccessibility { get; set; }
            public int maxPartySize { get; set; }
            public string queueId { get; set; }
            public bool partyLFM { get; set; }
            public string partyClientVersion { get; set; }
            public int partySize { get; set; }
            public long partyVersion { get; set; }
            public string queueEntryTime { get; set; }
            public string playerCardId { get; set; }
            public string playerTitleId { get; set; }
            public bool isIdle { get; set; }
        }
        public static UserPresence GetPresence(Auth auth)
        {
            try
            {
                RestClient GetClient = new RestClient(new Uri($"https://127.0.0.1:{auth.LockFile[2]}/chat/v4/presences"));
                GetClient.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                RestRequest GetRequest = new RestRequest(Method.GET);
                GetRequest.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{auth.LockFile[3]}"))}");
                GetRequest.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                GetRequest.AddHeader("X-Riot-ClientVersion", auth.version);
                IRestResponse getResp = GetClient.Get(GetRequest);

                if (getResp == null)
                    return null;

                var obj = new JObject();
                if (!getResp.IsSuccessful)
                    return null;
                UserPresence au = JsonConvert.DeserializeObject<UserPresence>(getResp.Content);

                for (int i = 0; i < au.presences.Count; ++i)
                {
                    if (au.presences[i].@private == null)
                        continue;

                    if (!Regex.IsMatch(au.presences[i].@private, @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$"))
                        continue;

                    try
                    {
                        au.presences[i].privinfo = JsonConvert.DeserializeObject<priv>(Encoding.UTF8.GetString(Convert.FromBase64String(au.presences[i].@private)));
                    }
                    catch (JsonException)
                    {
                        Debug.WriteLine("Error occured while deserilizeing json at presence.cs");
                        return null;
                    }
                }

                return au;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetPreseceError:" + ex.Message + " ST:" + ex.StackTrace);
                return null;
            }
        }
    }
}
