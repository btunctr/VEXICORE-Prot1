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
    public class LevelBorder
    {

            public string uuid { get; set; }
            public int startingLevel { get; set; }
            public string levelNumberAppearance { get; set; }
            public string smallPlayerCardAppearance { get; set; }
            public string assetPath { get; set; }



        public static List<LevelBorder> GetLevelBorders()
        {
            RestClient versionclient = new RestClient($"https://valorant-api.com/v1/levelborders");
            RestRequest versionrequest = new RestRequest(Method.GET);

            string json = versionclient.Execute(versionrequest).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<LevelBorder>>(obj["data"].ToString());
        }
    }
}
