using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Valorant.API
{
    public class Auth
    {
        public string EntitlementToken { get; set; }
        public string AccessToken { get; set; }
        public string subject { get; set; }
        public CookieContainer cookies { get; set; }
        public string version;
        public Region region;
        public string[] LockFile;

        public static Auth GetAuthLocal(bool WaitForLockfile = true)
        {
            string lockfile = "";
            if (WaitForLockfile == true)
            {
                while (lockfile == "")
                {
                    try
                    {
                        using (var fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Riot Games\\Riot Client\\Config\\lockfile", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs, Encoding.Default))
                        {
                            lockfile = sr.ReadToEnd();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                try
                {
                    using (var fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Riot Games\\Riot Client\\Config\\lockfile", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        lockfile = sr.ReadToEnd();
                    }
                }
                catch 
                {
                    return null;
                }
            }
            RestClient versionclient = new RestClient("https://valorant-api.com/v1/version");
            RestRequest versionrequest = new RestRequest(Method.GET);
            string json = versionclient.Execute(versionrequest).Content;
            var version = JsonConvert.DeserializeObject(json);
            JToken versionobj = JObject.FromObject(version);
            JToken versiondata = JObject.FromObject(versionobj["data"]);

            //Get ID list
            string versionformat = versiondata["branch"].Value<string>() + "-shipping-" + versiondata["buildVersion"].Value<string>() + "-" + versiondata["version"].Value<string>().Substring(versiondata["version"].Value<string>().Length - 6);

            string[] lf = lockfile.Split(':');
            RestClient GetClient = new RestClient(new Uri($"https://127.0.0.1:{lf[2]}/entitlements/v1/token"));
            GetClient.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            RestRequest GetRequest = new RestRequest(Method.GET);
            GetRequest.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{lf[3]}"))}");
            GetRequest.AddHeader("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            GetRequest.AddHeader("X-Riot-ClientVersion", versionformat/*"release-02.06-shipping-14-540727"*/);
            IRestResponse getResp = GetClient.Get(GetRequest);
            var obj = new JObject();
            if (getResp.IsSuccessful)
                obj = JObject.Parse(getResp.Content);
            else
                return null;
            Auth au = new Auth();
            au.LockFile = lf;
            au.AccessToken = (string)obj["accessToken"];
            au.EntitlementToken = (string)obj["token"];
            au.subject = (string)obj["subject"];
            au.version = versionformat;
            au.cookies = new CookieContainer();

            IRestClient RegClient = new RestClient(new Uri($"https://127.0.0.1:{lf[2]}/player-affinity/product/v1/token"));
            RegClient.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            IRestRequest RegRequest = new RestRequest(Method.POST);
            RegRequest.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{lf[3]}"))}");
            var valorantData = new
            {
                product = "valorant"
            };
            RegRequest.AddJsonBody(JsonConvert.SerializeObject(valorantData));
            IRestResponse RegResp = RegClient.Post(RegRequest);
            var regobj = JObject.Parse(RegResp.Content);
            string reg = (string)regobj["affinities"]["live"].ToString().ToUpper();
            if (reg == "NA")
            {
                au.region = Region.NA;
            }
            else if (reg == "AP")
            {
                au.region = Region.AP;
            }
            else if (reg == "EU")
            {
                au.region = Region.EU;
            }
            else if (reg == "KO")
            {
                au.region = Region.KO;
            }
            return au;
        }
        public static Auth StartAndGetAuthLocal(Region region)
        {
            Process p = new Process();
            p.StartInfo.FileName = "C:\\Riot Games\\Riot Client\\RiotClientServices.exe";
            p.StartInfo.Arguments = "--launch-product=valorant --launch-patchline=live";
            p.Start();
            return GetAuthLocal();
        }

        public static Auth Login(string username, string password, Region reg)
        {
            Auth au = new Auth();
            au.region = reg;
            string EntitlementToken;
            string AccessToken;
            au.cookies = new CookieContainer();
            string url = "https://auth.riotgames.com/api/v1/authorization";
            RestClient client = new RestClient(url);

            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.POST);
            string body = "{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" + "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}";
            request.AddJsonBody(body);
            client.Execute(request);
            RestClient authclient = new RestClient("https://auth.riotgames.com/api/v1/authorization");

            authclient.CookieContainer = au.cookies;

            RestRequest authrequest = new RestRequest(Method.PUT);
            string authbody = "{\"type\":\"auth\",\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            authrequest.AddJsonBody(authbody);
            string authresult = authclient.Execute(authrequest).Content;
            var authJson = JsonConvert.DeserializeObject(authresult);
            JToken authObj = JObject.FromObject(authJson);
            if (authresult.Contains("auth_failure"))
            {
                return new Auth();
            }
            string authURL = authObj["response"]["parameters"]["uri"].Value<string>();
            var access_tokenVar = Regex.Match(authURL, @"access_token=(.+?)&scope=").Groups[1].Value;
            AccessToken = $"{access_tokenVar}";

            RestClient entitlementclient = new RestClient(new Uri("https://entitlements.auth.riotgames.com/api/token/v1"));
            RestRequest entitlementrequest = new RestRequest(Method.POST);

            entitlementrequest.AddHeader("Authorization", $"Bearer {AccessToken}");
            entitlementrequest.AddJsonBody("{}");
            entitlementclient.CookieContainer = au.cookies;
            string response = entitlementclient.Execute(entitlementrequest).Content;
            var entitlement_token = JsonConvert.DeserializeObject(response);
            JToken entitlement_tokenObj = JObject.FromObject(entitlement_token);

            EntitlementToken = entitlement_tokenObj["entitlements_token"].Value<string>();
            au.AccessToken = AccessToken;
            au.EntitlementToken = EntitlementToken;
            var jsonWebToken = new JsonWebToken(AccessToken);
            au.subject = jsonWebToken.Subject;

            RestClient versionclient = new RestClient("https://valorant-api.com/v1/version");
            RestRequest versionrequest = new RestRequest(Method.GET);
            string json = versionclient.Execute(versionrequest).Content;
            var version = JsonConvert.DeserializeObject(json);
            JToken versionobj = JObject.FromObject(version);
            JToken versiondata = JObject.FromObject(versionobj["data"]);

            //Get ID list
            string versionformat = versiondata["branch"].Value<string>() + "-shipping-" + versiondata["buildVersion"].Value<string>() + "-" + versiondata["version"].Value<string>().Substring(versiondata["version"].Value<string>().Length - 6);
            au.version = versionformat;
            return au;
        }
    }

}
