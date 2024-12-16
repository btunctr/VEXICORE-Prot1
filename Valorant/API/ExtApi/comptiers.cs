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
    public class CompetitiveTier
    {
            public string uuid { get; set; }
            public string assetObjectName { get; set; }
            public Tier[] tiers { get; set; }
            public string assetPath { get; set; }

        public class Tier
        {
            public int tier { get; set; }
            public string tierName { get; set; }
            public string division { get; set; }
            public string divisionName { get; set; }
            public string color { get; set; }
            public string backgroundColor { get; set; }
            public string smallIcon { get; set; }
            public string largeIcon { get; set; }
            public string rankTriangleDownIcon { get; set; }
            public string rankTriangleUpIcon { get; set; }
        }


        public static List<CompetitiveTier> GetTiers(string language = "tr-TR")
        {
            RestClient versionclient = new RestClient($"https://valorant-api.com/v1/competitivetiers?language={language}");
            RestRequest versionrequest = new RestRequest(Method.GET);

            string json = versionclient.Execute(versionrequest).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<CompetitiveTier>>(obj["data"].ToString());
        }
    }
}
