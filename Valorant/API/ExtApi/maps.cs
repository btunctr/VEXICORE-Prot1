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
    public class Map
    {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string coordinates { get; set; }
            public string displayIcon { get; set; }
            public string listViewIcon { get; set; }
            public string splash { get; set; }
            public string assetPath { get; set; }
            public string mapUrl { get; set; }
            public float xMultiplier { get; set; }
            public float yMultiplier { get; set; }
            public float xScalarToAdd { get; set; }
            public float yScalarToAdd { get; set; }
            public Callout[] callouts { get; set; }

        public class Callout
        {
            public string regionName { get; set; }
            public string superRegionName { get; set; }
            public Location location { get; set; }
        }

        public class Location
        {
            public float x { get; set; }
            public float y { get; set; }
        }


        public static List<Map> GetMaps(string language = "tr-TR")
        {
            RestClient versionclient = new RestClient($"https://valorant-api.com/v1/maps?language={language}");
            RestRequest versionrequest = new RestRequest(Method.GET);

            string json = versionclient.Execute(versionrequest).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<Map>>(obj["data"].ToString());
        }
    }
}
