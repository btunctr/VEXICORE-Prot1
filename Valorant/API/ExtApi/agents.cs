using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valorant.API
{
    public class Agent
    {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string description { get; set; }
            public string developerName { get; set; }
            public string[] characterTags { get; set; }
            public string displayIcon { get; set; }
            public string displayIconSmall { get; set; }
            public string bustPortrait { get; set; }
            public string fullPortrait { get; set; }
            public string fullPortraitV2 { get; set; }
            public string killfeedPortrait { get; set; }
            public string background { get; set; }
            public string[] backgroundGradientColors { get; set; }
            public string assetPath { get; set; }
            public bool isFullPortraitRightFacing { get; set; }
            public bool isPlayableCharacter { get; set; }
            public bool isAvailableForTest { get; set; }
            public bool isBaseContent { get; set; }
            public Role role { get; set; }
            public Ability[] abilities { get; set; }

        public class Role
        {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string description { get; set; }
            public string displayIcon { get; set; }
            public string assetPath { get; set; }
        }

        public class Ability
        {
            public string slot { get; set; }
            public string displayName { get; set; }
            public string description { get; set; }
            public string displayIcon { get; set; }
        }


        public static List<Agent> GetAgents(string language = "tr-TR", bool isPlayableCharacter = true)
        {
            RestClient versionclient = new RestClient($"https://valorant-api.com/v1/agents?language={language}&isPlayableCharacter={isPlayableCharacter}");
            RestRequest versionrequest = new RestRequest(Method.GET);

            string json = versionclient.Execute(versionrequest).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<Agent>>(obj["data"].ToString());
        }
    }
}
