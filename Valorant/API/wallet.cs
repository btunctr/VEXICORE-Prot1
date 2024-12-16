using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valorant.API;
using static Prot1.Valorant.API.Storefront;

namespace Prot1.Valorant.API
{
    public class Wallet
    {
        public Dictionary<string, double> Balances { get; set; }

        public static Wallet GetWallet(Auth au)
        {
            string url = $"https://pd.{au.region}.a.pvp.net/store/v1/wallet/{au.subject}";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");

            try
            {
                return JsonConvert.DeserializeObject<Wallet>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at wallet.cs");
                return null;
            }
        }
    }
}
