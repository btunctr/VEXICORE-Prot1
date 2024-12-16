using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Valorant.API
{
    public class LContent
    {
        public Season[] Seasons { get; set; }
        public Event[] Events { get; set; }


        public class Season
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool IsActive { get; set; }
        }

        public class Event
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool IsActive { get; set; }
        }

        public static LContent FetchContents(Auth au)
        {
            string url = $"https://shared.{au.region}.a.pvp.net/content-service/v3/content";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");

            try
            {
                return JsonConvert.DeserializeObject<LContent>(client.Execute(request).Content);
            } catch (JsonException)
            {
                Debug.WriteLine("Error occured while fetching contents in lcontent.cs");
                return null;
            }
        }
    }
}
