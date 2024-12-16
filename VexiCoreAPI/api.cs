using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Prot1.Vexi;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Prot1
{
    public class HWIDGenerator
    {
        public static string GetHWID()
        {
            string cpuId = GetProcessorId();
            string motherboardId = GetMotherboardId();
            string hddId = GetHDDSerialNumber();

            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{cpuId}-{motherboardId}-{hddId}"));
        }

        private static string GetProcessorId()
        {
            string result = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectCollection objects = searcher.Get();

            foreach (ManagementObject obj in objects)
            {
                result = obj["ProcessorId"].ToString();
                break; // Only get the first CPU ID
            }

            return result;
        }

        private static string GetMotherboardId()
        {
            string result = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection objects = searcher.Get();

            foreach (ManagementObject obj in objects)
            {
                result = obj["SerialNumber"].ToString();
                break; // Only get the first motherboard ID
            }

            return result;
        }

        private static string GetHDDSerialNumber()
        {
            string result = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            ManagementObjectCollection objects = searcher.Get();

            foreach (ManagementObject obj in objects)
            {
                result = obj["SerialNumber"].ToString();
                break; // Only get the first hard drive serial number
            }

            return result;
        }

        public static void Main(string[] args)
        {
            string hwid = GetHWID();
            Console.WriteLine($"HWID: {hwid}");
        }
    }

    public enum HttpStatusCode
    {
        // Informational 1xx
        _100_Continue = 100,
        _101_SwitchingProtocols = 101,
        _102_Processing = 102,
        _103_EarlyHints = 103,

        // Successful 2xx
        _200_OK = 200,
        _201_Created = 201,
        _202_Accepted = 202,
        _203_NonAuthoritativeInformation = 203,
        _204_NoContent = 204,
        _205_ResetContent = 205,
        _206_PartialContent = 206,
        _207_MultiStatus = 207,
        _208_AlreadyReported = 208,
        _226_IMUsed = 226,

        // Redirection 3xx
        _300_MultipleChoices = 300,
        _301_MovedPermanently = 301,
        _302_Found = 302,
        _303_SeeOther = 303,
        _304_NotModified = 304,
        _305_UseProxy = 305,
        _307_TemporaryRedirect = 307,
        _308_PermanentRedirect = 308,

        // Client Errors 4xx
        _400_BadRequest = 400,
        _401_Unauthorized = 401,
        _402_PaymentRequired = 402,
        _403_Forbidden = 403,
        _404_NotFound = 404,
        _405_MethodNotAllowed = 405,
        _406_NotAcceptable = 406,
        _407_ProxyAuthenticationRequired = 407,
        _408_RequestTimeout = 408,
        _409_Conflict = 409,
        _410_Gone = 410,
        _411_LengthRequired = 411,
        _412_PreconditionFailed = 412,
        _413_PayloadTooLarge = 413,
        _414_URITooLong = 414,
        _415_UnsupportedMediaType = 415,
        _416_RangeNotSatisfiable = 416,
        _417_ExpectationFailed = 417,
        _418_ImATeapot = 418,
        _421_MisdirectedRequest = 421,
        _422_UnprocessableEntity = 422,
        _423_Locked = 423,
        _424_FailedDependency = 424,
        _426_UpgradeRequired = 426,
        _428_PreconditionRequired = 428,
        _429_TooManyRequests = 429,
        _431_RequestHeaderFieldsTooLarge = 431,
        _451_UnavailableForLegalReasons = 451,

        // Server Errors 5xx
        _500_InternalServerError = 500,
        _501_NotImplemented = 501,
        _502_BadGateway = 502,
        _503_ServiceUnavailable = 503,
        _504_GatewayTimeout = 504,
        _505_HTTPVersionNotSupported = 505,
        _506_VariantAlsoNegotiates = 506,
        _507_InsufficientStorage = 507,
        _508_LoopDetected = 508,
        _510_NotExtended = 510,
        _511_NetworkAuthenticationRequired = 511
    }

    public class Vexi
    {
        private string sid;
        private bool saveLogin;

        public bool IsAuthenticated { get; private set; } = false;
        public UserAccountDataResponse UserAccountData;

        public class ServerResponse<T>
        {
            public ServerResponse(HttpStatusCode code)
            {
                StatusCode = code;
            }

            public ServerResponse(int code)
            {
                StatusCode = (HttpStatusCode)code;
            }

            public HttpStatusCode StatusCode { get; set; }
            public T Data { get; set; }
        }

        public class UserAccountDataResponse
        {
            public string uid { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public DateTime RegisterDate { get; set; }
            public int gsm { get; set; }
            public JSON_UserAccountPlan ActivePlan { get; set; }
        }

        public class JSON_UserAccountPlan
        {
            public string planid { get; set; }
            public DateTime start { get; set; }
            public DateTime expire { get; set; }
            public TimeSpan RemainingTime { get; set; }
        }

        public Vexi(bool SaveLoginCreds)
        {
            saveLogin = SaveLoginCreds;
        }

        public Vexi() : this(false)
        {
            
        }

        public ServerResponse<UserAccountDataResponse> GetAccountData()
        {
            try
            {
                if (sid == null || sid == string.Empty)
                    return null;

                var client = new RestClient("http://localhost:5050/account");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", sid);

                var response = client.Execute(request);

                if ((HttpStatusCode)response.StatusCode == HttpStatusCode._200_OK)
                {
                    var rContent = response.Content.ToString();

                        var data = JsonConvert.DeserializeObject<UserAccountDataResponse>(rContent);
                        if (data != null)
                        {
                            UserAccountData = data;
                            return new ServerResponse<UserAccountDataResponse>((HttpStatusCode)response.StatusCode) { Data = data };
                        } else
                        {
                            return null;
                        }
                }

                return new ServerResponse<UserAccountDataResponse>((HttpStatusCode)response.StatusCode);
            }
            catch (JsonException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private const string LoginCredFileName = "lock";
        private void SaveLoginCreds(string sid)
        {
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VEXICORE");
            var file = Path.Combine(folderPath, LoginCredFileName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            FileStream writer;

            if (!File.Exists(file))
                writer = File.Create(file);
            else
                writer = File.OpenWrite(file);

            byte[] buffer = Encoding.UTF8.GetBytes(sid);
            writer.Write(buffer, 0, buffer.Length);
            writer.Close();
        }

        private string GetLoginCreds()
        {
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VEXICORE");
            var file = Path.Combine(folderPath, LoginCredFileName);

            if (!Directory.Exists(folderPath))
                return null;

            string text;

            if (!File.Exists(file))
                return null;
            else
                text = File.ReadAllText(file);

            if (text.Length > 0)
                return text;
            else
                return null;
        }

        public bool TryAuthViaSavedCreds()
        {
            string lc;

            if ((lc = GetLoginCreds()) == null)
                return false;

            this.sid = lc;
            var uad = this.GetAccountData();

            if (uad.StatusCode == HttpStatusCode._200_OK && uad.Data != null)
            {
                UserAccountData = uad.Data;
                IsAuthenticated = true;
                return true;
            }
            else
            {
                this.sid = null;
                return false;
            }
        }

        public HttpStatusCode RegisterAccount(string uname, string password, string email)
        {
            try
            {
                var client = new RestClient("http://localhost:5050/account/register");
                var request = new RestRequest(Method.POST);

                request.AddJsonBody(new
                {
                    username = uname,
                    email = email,
                    password = password,
                    hwid = HWIDGenerator.GetHWID(),
                    ip = string.Empty
                });

                var response = client.Execute(request);

                if ((HttpStatusCode)response.StatusCode == HttpStatusCode._200_OK)
                {
                    var rContent = response.Content.ToString();

                    if (string.IsNullOrEmpty(rContent))
                        return HttpStatusCode._401_Unauthorized;


                    if (saveLogin)
                        SaveLoginCreds(rContent);

                    IsAuthenticated = true;
                    sid = rContent;
                }

                return (HttpStatusCode)response.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode._500_InternalServerError;
            }
        }

        public HttpStatusCode LoginToAccount(string uname, string password)
        {
            try
            {
                var client = new RestClient("http://localhost:5050/account/login");
                var request = new RestRequest(Method.POST);

                request.AddJsonBody(new
                {
                    username = uname,
                    email = string.Empty,
                    password = password,
                    hwid = HWIDGenerator.GetHWID(),
                    ip = string.Empty
                });

                var response = client.Execute(request);

                if ((HttpStatusCode)response.StatusCode == HttpStatusCode._200_OK)
                {
                    var rContent = response.Content.ToString();

                    if (string.IsNullOrEmpty(rContent))
                        return HttpStatusCode._401_Unauthorized;

                    if (saveLogin)
                        SaveLoginCreds(rContent);

                    IsAuthenticated = true;
                    sid = rContent;
                }

                return (HttpStatusCode)response.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode._500_InternalServerError;
            }
        }
    }
}
