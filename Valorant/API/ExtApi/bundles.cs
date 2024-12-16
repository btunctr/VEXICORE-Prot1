using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valorant.API
{
    public class SkinBundles
    {
        public class SkinBundleItem
        {
            public string Uuid { get; set; }
            public string DisplayName { get; set; }
            public string DisplayNameSubText { get; set; }
            public string Description { get; set; }
            public string ExtraDescription { get; set; }
            public string PromoDescription { get; set; }
            public bool UseAdditionalContext { get; set; }
            public string DisplayIcon { get; set; }
            public string DisplayIcon2 { get; set; }
            public string VerticalPromoImage { get; set; }
            public string AssetPath { get; set; }
        }

        public static List<SkinBundleItem> GetBundles(string language = "tr-TR")
        {
            RestClient client = new RestClient($"https://valorant-api.com/v1/bundles?language={language}");
            RestRequest request = new RestRequest(Method.GET);

            string json = client.Execute(request).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<SkinBundleItem>>(obj["data"].ToString());
        }
    }
}
