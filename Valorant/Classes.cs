using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prot1.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Valorant;
using Valorant.API;
using Valorant.API.External;

public class Settings
{
    public Dictionary<string, string> PlayerMessages { get; set; }
    public List<ChatMessageTrigger> ChatTriggers { get; set; }

    private static ConfigManager cfm = new ConfigManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VEXICORE", "settings.json"));

    public static Settings GetSettings()
    {
        var defaultValue = new Settings()
        {
            PlayerMessages = new Dictionary<string, string>()
                {
                    { "deseler_want_weapon_skin", "Merhaba {%agent%}, rica etsem {%skin%} desenini alabilir miyim?" }
                }
        };

        if (cfm.TryGetValue<Settings>(out Settings sBuffer, JsonConvert.SerializeObject(defaultValue)))
        {
            return sBuffer;
        }
        else
            return null;
    }

    public static bool SaveSettings(ref Settings s)
    {
        return cfm.TrySaveValue(ref s);
    }
}

public class KeySupplier
{
    public double KeyPrice { get; set; }
    public double VpPrice { get; set; }

    public string SupplierName { get; set; }

    public KeySupplier(string name, double price, double vp)
    {
        this.KeyPrice = price;
        this.VpPrice = vp;
        this.SupplierName = name;
    }

    public static bool Equals(KeySupplier first, KeySupplier second) => first.VpPrice == second.VpPrice && first.KeyPrice == second.KeyPrice && first.SupplierName == second.SupplierName;

    public class KeySupplierCollection : List<KeySupplier>
    {
        private static KeySupplierCollection SetAndGetDefault()
        {
            var def = new KeySupplierCollection()
                    {
                        new KeySupplier("Valorant", 33, 225),
                        new KeySupplier("Valorant", 130, 925),
                        new KeySupplier("Valorant", 250, 1850),
                        new KeySupplier("Valorant", 450, 3400),
                        new KeySupplier("Valorant", 700, 5550),
                        new KeySupplier("Valorant", 1400, 11250)
                    };
            def.SaveCollection();
            return def;
        }

        public static KeySupplierCollection GetCollection()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string vexiCoreFolderPath = Path.Combine(appDataFolder, "VEXICORE");

            if (!Directory.Exists(vexiCoreFolderPath))
            {
                Directory.CreateDirectory(vexiCoreFolderPath);
            }

            string keysellersFilePath = Path.Combine(vexiCoreFolderPath, "keysellers.txt");

            if (!File.Exists(keysellersFilePath))
            {
                return SetAndGetDefault();
            }
            else
            {
                try
                {
                    string jsonContent = File.ReadAllText(keysellersFilePath);
                    KeySupplierCollection collection = JsonConvert.DeserializeObject<KeySupplierCollection>(jsonContent);

                    if (collection.Count() == 0)
                        return SetAndGetDefault();

                    return collection;
                }
                catch (Exception)
                {
                    return SetAndGetDefault();
                }
            }
        }

        public void SaveCollection()
        {
            SaveCollection(this);
        }

        private void SaveCollection(KeySupplierCollection collection)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string vexiCoreFolderPath = Path.Combine(appDataFolder, "VEXICORE");

            if (!Directory.Exists(vexiCoreFolderPath))
            {
                Directory.CreateDirectory(vexiCoreFolderPath);
            }

            string keysellersFilePath = Path.Combine(vexiCoreFolderPath, "keysellers.txt");

            try
            {
                string json = JsonConvert.SerializeObject(collection);

                if (File.Exists(keysellersFilePath))
                {
                    File.WriteAllText(keysellersFilePath, json);
                }
                else
                {
                    using (var sr = File.CreateText(keysellersFilePath))
                    {
                        sr.Write(json);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private List<List<KeySupplier>> TumKombinasyonlariGetir(List<KeySupplier> urunler)
        {
            List<List<KeySupplier>> kombinasyonlar = new List<List<KeySupplier>>();
            int n = urunler.Count;

            // Calculate the total number of combinations (2^n)
            int totalCombinations = 1 << n;

            for (int i = 0; i < totalCombinations; i++)
            {
                List<KeySupplier> kombinasyon = new List<KeySupplier>();

                for (int j = 0; j < n; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        kombinasyon.Add(urunler[j]);
                    }
                }

                kombinasyonlar.Add(kombinasyon);
            }

            return kombinasyonlar;
        }

        public List<KeySupplier> GetCheapestCombinations(double hedefVP)
        {
            List<KeySupplier> urunler = this;
            List<List<KeySupplier>> tumKombinasyonlar = TumKombinasyonlariGetir(urunler);

            List<KeySupplier> enUygunKombinasyon = new List<KeySupplier>();
            List<KeySupplier> ekKombinasyonlar = new List<KeySupplier>();
            double enUygunToplamTL = double.MaxValue;
            double enUygunToplamVP = 0;

            var mUrunler = urunler.Where(i => i.VpPrice == urunler.Max(j => j.VpPrice));
            var maxUrun = mUrunler.FirstOrDefault(k => k.KeyPrice == mUrunler.Min(x => x.KeyPrice));
            if (hedefVP > maxUrun.VpPrice)
            {
                for (int i = 0; i < (int)Math.Floor((double)hedefVP / (double)maxUrun.VpPrice); ++i)
                    ekKombinasyonlar.Add(maxUrun);

                hedefVP %= maxUrun.VpPrice;
            }

            foreach (var kombinasyon in tumKombinasyonlar)
            {
                double toplamTL = kombinasyon.Sum(KeySupplier => KeySupplier.KeyPrice);
                double toplamVP = kombinasyon.Sum(KeySupplier => KeySupplier.VpPrice);

                if (toplamVP >= hedefVP && toplamTL < enUygunToplamTL)
                {
                    enUygunKombinasyon = kombinasyon;
                    enUygunToplamTL = toplamTL;
                    enUygunToplamVP = toplamVP;
                }
            }

            enUygunKombinasyon = enUygunKombinasyon.Concat(ekKombinasyonlar).ToList();

            foreach (var item in ekKombinasyonlar)
            {
                enUygunToplamTL += item.KeyPrice;
                enUygunToplamVP += item.VpPrice;
            }

            tumKombinasyonlar.Clear();
            ekKombinasyonlar.Clear();

            tumKombinasyonlar.TrimExcess();
            ekKombinasyonlar.TrimExcess();

            tumKombinasyonlar = null;
            ekKombinasyonlar = null;

            GC.Collect();

            return enUygunKombinasyon;
        }
    }
}

public class ConfigManager
{
    public string FilePath;
    public ConfigManager(string filePath)
    {
        FilePath = filePath;
    }

    public bool TryGetValue<T>(out T value, string defaultValue = "")
    {
        var dir = FilePath.Replace(Path.GetFileName(FilePath), string.Empty);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(FilePath))
            File.Create(FilePath).Close();

        string text = File.ReadAllText(FilePath);

        if (text.Length <= 0)
        {
            File.WriteAllText(FilePath, defaultValue);
            text = defaultValue;
        }

        value = default(T);
        try
        {
            value = JsonConvert.DeserializeObject<T>(text);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public bool TrySaveValue<T>(ref T value)
    {
        var dir = FilePath.Replace(Path.GetFileName(FilePath), string.Empty);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(FilePath))
            File.Create(FilePath).Close();

        try
        {
            string text = JsonConvert.SerializeObject(value);
            File.WriteAllText(FilePath, text);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class TextSchema
{
    public static string GetMessageTest(string message, Dictionary<string, string> values)
    {
        string pattern = @"\{%([^%]+)%\}";
        Regex regex = new Regex(pattern);

        string replacedText = regex.Replace(message, match =>
        {
            string key = match.Groups[1].Value;
            if (values.ContainsKey(key))
            {
                return values[key];
            }
            else
            {
                return string.Empty;
            }
        });

        return replacedText;
    }
}

//public class SoundExtensions
//{
//    public static readonly DependencyProperty ClickSoundProperty =
//DependencyProperty.RegisterAttached("ClickSound", typeof(string), typeof(SoundExtensions));

//    public static void SetClickSound(UIElement element, string value)
//    {
//        element.MouseLeftButtonDown -= MouseClick;
//        element.MouseLeftButtonDown += MouseClick;

//        element.SetValue(ClickSoundProperty, value);
//    }

//    static DateTime lastClickTime = DateTime.MinValue;
//    static DateTime lastHoverTime = DateTime.MinValue;
//    private static void MouseClick(object sender, dynamic e)
//    {
//        var val = (sender as UIElement).GetValue(ClickSoundProperty);

//        if (val == null)
//            return;

//        if (val is string str)
//        {
//            DateTime now = DateTime.Now;

//            // Check if it has been at least 500 ms since the last click
//            if ((now - lastClickTime).TotalMilliseconds >= 500)
//            {
//                var ss = Properties.Resources.ResourceManager.GetStream(str);
//                var sp = new SoundPlayer(ss);
//                sp.Play();

//                // Update the last click time
//                lastClickTime = now;
//            }
//        }
//    }

//    public static string GetClickSound(UIElement element)
//    {
//        return (string)element.GetValue(ClickSoundProperty);
//    }

//    public static readonly DependencyProperty HoverSoundProperty =
//        DependencyProperty.RegisterAttached("HoverSound", typeof(string), typeof(SoundExtensions));

//    public static void SetHoverSound(UIElement element, string value)
//    {
//        element.MouseEnter -= OnHover;
//        element.MouseEnter += OnHover;
//        element.SetValue(HoverSoundProperty, value);
//    }

//    public static void OnHover(object sender, dynamic e)
//    {
//        var val = (sender as UIElement).GetValue(HoverSoundProperty);

//        if (val == null)
//            return;

//        if (val is string str)
//        {
//            DateTime now = DateTime.Now;

//            // Check if it has been at least 500 ms since the last click
//            if ((now - lastHoverTime).TotalMilliseconds >= 500)
//            {
//                var ss = Properties.Resources.ResourceManager.GetStream(str);
//                var sp = new SoundPlayer(ss);
//                sp.Play();

//                // Update the last click time
//                lastClickTime = now;
//            }
//        }
//    }

//public static string GetHoverSound(UIElement element)
//    {
//        return (string)element.GetValue(HoverSoundProperty);
//    }
//}

public class ValorantEventListener
{
    public string CurrentGameState = "NONE";

    public delegate Task GameStateChangedEventHandler(string OldState, string NewState);
    public GameStateChangedEventHandler GameStateChanged;

    public delegate Task GameChatRecivedMessageEventHandler(IncomingChatMessage msg);
    public GameChatRecivedMessageEventHandler MessageRecived;

    private int SleepDuration;

    public ValorantEventListener()
    {

    }

    public void StartListening(int sleepDuration = 150)
    {
        try
        {
            SleepDuration = sleepDuration;
            var t = new Thread(T_CheckEvents);
            t.Start();
        }
        catch { }
    }

    //private JToken get_private_presence(List<UserPresence.Presence> presences)
    //{
    //    if (presences == null)
    //        return null;

    //    foreach (UserPresence.Presence presence in presences)
    //    {
    //        if (presence == null)
    //            continue;

    //        if (presence.puuid == Helpers.GameAuth.subject)
    //        {
    //            if (presence.championId == null && presence.product != "league_of_legends")
    //            {
    //                return (JToken)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(presence.@private)));
    //            }
    //            else
    //            {
    //                return null;
    //            }
    //        }
    //    }

    //    return null;
    //}

    private List<string> lastMessages = new List<string>();
    async Task Handle(string json)
    {
        try
        {
            if (json.Length > 10)
            {
                JToken obj = JToken.Parse(json);
                if (obj[2]?["uri"]?.ToString() == "/chat/v4/presences")
                {
                    var presences = UserPresence.GetPresence(Globals.GameAuth);

                    if (presences == null || presences.presences == null || presences.presences.Count == 0)
                        return;

                    var sBuffer = presences.presences.FirstOrDefault(x => Globals.GameAuth.subject.CompareEquality(x.puuid ?? string.Empty));

                    if (sBuffer == null)
                        return;

                    if (sBuffer.privinfo == null)
                        return;

                    var new_state = new string(sBuffer.privinfo.sessionLoopState.ToString().ToCharArray());
                    if (new_state != CurrentGameState)
                    {
                        await Task.Run(async () =>
                        {
                            if (GameStateChanged == null)
                                return;

                            await GameStateChanged?.Invoke(CurrentGameState, new_state);
                        });
                        Thread.Sleep(30);
                    }

                    CurrentGameState = new_state;
                    Thread.Sleep(2);
                }
                else if (obj[2]?["uri"]?.ToString() == "/chat/v6/messages")
                {
                    //if (!obj[2]["data"]["messages"][0]["puuid"].ToString().CompareEquality(Helpers.GameAuth.subject))
                    {
                        try
                        {
                            var res = JsonConvert.DeserializeObject<IncomingChatMessage>(obj[2]["data"]["messages"][0].ToString());

                            if (lastMessages.Any(x => x.CompareEquality(res.id)))
                                return;
                            else
                            {
                                if (lastMessages.Count > 8)
                                    lastMessages.Clear();

                                lastMessages.Add(res.id);
                            }

                            await Task.Run(async () => await MessageRecived?.Invoke(res));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error at Helpers.cs:Handle (Message) = " + ex.Message);
                            return;
                        }
                    }
                }
            }
        }
        catch (JsonException jsonEx)
        {
            // Handle JSON parsing errors
            Debug.WriteLine("JSON parsing error: " + jsonEx.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Debug.WriteLine("Error in Handle.cs: " + ex.Message);
        }
    }

    public class IncomingChatMessage
    {
        public string body { get; set; }
        public string cid { get; set; }
        public bool droppedDueToThrottle { get; set; }
        public string game_name { get; set; }
        public string game_tag { get; set; }
        public string id { get; set; }
        public string mid { get; set; }
        public string name { get; set; }
        public string pid { get; set; }
        public string puuid { get; set; }
        public bool read { get; set; }
        public string region { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public bool uicEvent { get; set; }
    }

    public async void T_CheckEvents()
    {
        var seeIds = new List<string>();
        ClientWebSocket ws = null;

        bool firstTime_gamestate = true;
        while (true)
        {
            try
            {
                #region Game State
                if (firstTime_gamestate)
                {
                    bool run = true;
                    while (run)
                    {
                        var presences = UserPresence.GetPresence(Globals.GameAuth);

                        if (presences == null || presences.presences == null || presences.presences.Count == 0)
                            continue;

                        var sBuffer = presences.presences.FirstOrDefault(x => Globals.GameAuth.subject.CompareEquality(x.puuid ?? string.Empty));

                        if (sBuffer == null)
                            continue;

                        if (sBuffer.privinfo == null)
                            continue;

                        var new_state = new string(sBuffer.privinfo.sessionLoopState.ToString().ToCharArray());
                        if (new_state != CurrentGameState)
                        {
                            await Task.Run(async () =>
                            {
                                if (GameStateChanged == null)
                                    return;

                                await GameStateChanged?.Invoke(CurrentGameState, new_state);
                            });
                            Thread.Sleep(30);
                        }

                        CurrentGameState = new_state;

                        if (CurrentGameState != null)
                            run = false;

                        Thread.Sleep(2);
                    }

                    firstTime_gamestate = false;
                }
                else
                {
                    if (ws == null)
                    {

                        ws = new ClientWebSocket();
                        ws.Options.SetRequestHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{Globals.GameAuth.LockFile[3]}"))}");
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        Uri serverUri = new Uri($"wss://127.0.0.1:{Globals.GameAuth.LockFile[2]}");

                        try
                        {
                            await ws.ConnectAsync(serverUri, CancellationToken.None);

                            List<byte[]> sendBuffer = new List<byte[]>()
                            {
                                Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent_chat_v4_presences\"]"),
                                Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent_chat_v6_messages\"]")
                            };

                            foreach (var bytes in sendBuffer)
                                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("(EventListenerException) Websocket_Connect. Message : " + ex.Message + "  St: " + ex.StackTrace);
                        }
                    }

                    try
                    {
                        StringBuilder receivedData = new StringBuilder();

                        while (ws.State == WebSocketState.Open)
                        {
                            byte[] receiveBuffer = new byte[1024];
                            WebSocketReceiveResult receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                            if (receiveResult.MessageType == WebSocketMessageType.Close)
                            {
                                await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            }
                            else
                            {
                                string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                                receivedData.Append(receivedMessage);

                                if (receiveResult.EndOfMessage)
                                {
                                    string completeMessage = receivedData.ToString();
                                    await Handle(completeMessage);
                                    receivedData.Clear();
                                }
                            }
                        }

                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("(EventListenerException) Websocket_Handle. Message : " + ex.Message + "  St: " + ex.StackTrace);
                    }
                }
                #endregion

                Thread.Sleep(SleepDuration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("(EventListenerException) " + ex.Message + " ST: " + ex.StackTrace);
            }
        }
    }
}

public static class ExtentionMethods
{
    public static string ToDateStringHour(this TimeSpan ts)
    {
        var list = new List<string>();

        if (ts.Days > 0)
            list.Add($"{ts.Days} Gün");

        if (ts.Hours > 0)
            list.Add($"{ts.Hours} Saat");

        return string.Join(" ", list);
    }

    public static bool CompareEquality(this string source, string id) => source.ToLower() == id.ToLower();
    public static BitmapImage InvertColors(this BitmapImage originalImage)
    {
        if (originalImage == null)
            return null;

        int width = originalImage.PixelWidth;
        int height = originalImage.PixelHeight;

        FormatConvertedBitmap invertedBitmap = new FormatConvertedBitmap();
        invertedBitmap.BeginInit();
        invertedBitmap.Source = originalImage;
        invertedBitmap.DestinationFormat = PixelFormats.Bgra32;
        invertedBitmap.EndInit();

        int stride = (width * invertedBitmap.Format.BitsPerPixel + 7) / 8;
        int imageSize = stride * height;
        byte[] pixelBuffer = new byte[imageSize];
        invertedBitmap.CopyPixels(pixelBuffer, stride, 0);

        for (int i = 0; i < imageSize; i += 4)
        {
            pixelBuffer[i] = (byte)(255 - pixelBuffer[i]);         // Blue
            pixelBuffer[i + 1] = (byte)(255 - pixelBuffer[i + 1]); // Green
            pixelBuffer[i + 2] = (byte)(255 - pixelBuffer[i + 2]); // Red
                                                                   // Alpha channel (i + 3) remains unchanged
        }

        BitmapSource invertedSource = BitmapSource.Create(width, height, originalImage.DpiX, originalImage.DpiY,
            invertedBitmap.Format, null, pixelBuffer, stride);

        BitmapImage invertedImage = new BitmapImage();
        invertedImage.BeginInit();
        invertedImage.StreamSource = ConvertBitmapSourceToMemoryStream(invertedSource);
        invertedImage.CacheOption = BitmapCacheOption.OnLoad;
        invertedImage.EndInit();

        return invertedImage;
    }

    private static System.IO.MemoryStream ConvertBitmapSourceToMemoryStream(BitmapSource source)
    {
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(source));
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        encoder.Save(stream);
        return stream;
    }
}

public class ImageDownloadManager
{
    private HttpClient _client;
    public string BasePath;
    public Dictionary<string, string> keyValuePairs;

    private readonly string CacheHeaderFile;
    private readonly string CacheFolder;
    private readonly string CacheEntryFileExtention = ".cpack";

    public delegate void OnImageRetrived(string type, string file);
    public event OnImageRetrived ImageRetrived;

    private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    public ImageDownloadManager(string path, string hFile = "cache.chf", string cFolder = "download_cache")
    {
        _client = new HttpClient();
        BasePath = path;
        CacheHeaderFile = hFile;
        CacheFolder = cFolder;
    }

    private string _dPath
    {
        get => Path.Combine(BasePath, CacheHeaderFile);
    }

    private string _cacheDirectoryPath
    {
        get => Path.Combine(BasePath, CacheFolder);
    }

    public void LoadDictionary()
    {
        this.ImageRetrived = null;

        var sw = new Stopwatch();
        sw.Start();

        if (!File.Exists(this._dPath))
        {
            keyValuePairs = new Dictionary<string, string>();
            return;
        }

        try
        {
            var fContent = File.ReadAllText(this._dPath);
            keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(fContent)), _jsonSerializerSettings);
        }
        catch
        {
            keyValuePairs = new Dictionary<string, string>();
        }

        sw.Stop();
        Debug.WriteLine($"Dictionary loaded in {sw.ElapsedMilliseconds} ms");
    }

    public void SetDictionary()
    {
        var bJson = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(keyValuePairs, _jsonSerializerSettings));
        File.WriteAllText(this._dPath, Convert.ToBase64String(bJson));
    }

    public async Task<BitmapImage[]> GetMultipeImagesAsync(IEnumerable<string> urlBuffer, IProgress<DownloadStatus> progress = null)
    {
        var ret = new BitmapImage[urlBuffer.Count()];
        LoadDictionary();

        var ds = new DownloadStatus()
        {
            TotalItems = urlBuffer.Count(),
            DownloadedItems = 0,
            RemainingItems = urlBuffer.Count()
        };

        for (int i = 0; i < urlBuffer.Count(); ++i)
        {
            ret[i] = await GetImageAsync(urlBuffer.ElementAt(i));

            if (progress != null)
            {
                ds.DownloadedItems++;
                ds.RemainingItems--;
                ds.Percantage = (int)(((double)ds.DownloadedItems / (double)ds.TotalItems) * 100);

                progress.Report(ds);
            }
        }

        SetDictionary();

        return ret;
    }

    public class DownloadStatus
    {
        public int TotalItems { get; set; }
        public int RemainingItems { get; set; }
        public int DownloadedItems { get; set; }
        public int Percantage { get; set; }
    }

    private Random random = new Random();

    private string GenerateRandomFileName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder fileName = new StringBuilder();

        for (int i = 0; i < 24; i++)
        {
            fileName.Append(chars[random.Next(chars.Length)]);
        }

        return fileName.ToString();
    }

    public async Task<BitmapImage> GetImageAsync(string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var sw = new Stopwatch();
            sw.Start();

            if (keyValuePairs.TryGetValue(url, out var filePath))
            {
                if (!File.Exists(filePath))
                {
                    keyValuePairs.Remove(url);
                    goto DownloadImage;
                }

                var imageData = Convert.FromBase64String(File.ReadAllText(filePath));
                sw.Stop();
                Debug.WriteLine("Image retrived from cache. Took: " + sw.ElapsedMilliseconds.ToString() + " ms");

                if (ImageRetrived != null)
                    await Task.Run(() => ImageRetrived.Invoke("cache", filePath));

                return CreateBitmapImage(imageData);
            }

        DownloadImage:
            sw.Restart();

            try
            {
                var imageData = await _client.GetByteArrayAsync(url);

                if (!Directory.Exists(_cacheDirectoryPath))
                    Directory.CreateDirectory(_cacheDirectoryPath);

                var fpath = Path.Combine(_cacheDirectoryPath, GenerateRandomFileName() + CacheEntryFileExtention);

                if (File.Exists(fpath))
                    File.Delete(fpath);

                using (var writer = File.CreateText(fpath))
                    writer.Write(Convert.ToBase64String(imageData));

                keyValuePairs.Add(url, fpath);

                SetDictionary();
                return CreateBitmapImage(imageData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error occured at downloading image: {ex.Message}");
                return null;
            }
            finally
            {
                sw.Stop();
                Debug.WriteLine("Image Downloaded. Took: " + sw.ElapsedMilliseconds.ToString() + " ms");

                if (ImageRetrived != null)
                    await Task.Run(() => ImageRetrived.Invoke("download", filePath));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error occured at GetImageAsync image: {ex.Message}");
            return null;
        }
    }

    private BitmapImage CreateBitmapImage(byte[] imageData)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = new MemoryStream(imageData);
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }
}

public class ValorantBulkContentDownloader
{
    private GameContent _gc;
    private ImageDownloadManager _idm;

    public ValorantBulkContentDownloader(GameContent content, ImageDownloadManager idm)
    {
        _gc = content;
        _idm = idm;
    }

    public async Task DownloadWeaponSkins(IProgress<ImageDownloadManager.DownloadStatus> progress = null)
    {
        List<string> urls = new List<string>();

        foreach (var skin in _gc.WeaponSkins)
        {
            foreach (var chroma in skin.chromas)
            {
                urls.Add(chroma.swatch);
                urls.Add(chroma.displayIcon);
            }
        }

        await _idm.GetMultipeImagesAsync(urls, progress);
    }
}

public static class Extentions
{
    public static bool IsEndOfBlock(this TextPointer position)
    {
        for (; position != null; position = position.GetNextContextPosition(LogicalDirection.Forward))
        {
            switch (position.GetPointerContext(LogicalDirection.Forward))
            {
                case TextPointerContext.ElementEnd:
                    if (position.GetAdjacentElement(LogicalDirection.Forward) is Paragraph) return true;
                    break;
                default:
                    return false;
            }
        }
        return false;
    }
}
