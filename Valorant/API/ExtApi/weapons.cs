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
    public class Weapon
    {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string category { get; set; }
            public string defaultSkinUuid { get; set; }
            public string displayIcon { get; set; }
            public string killStreamIcon { get; set; }
            public string assetPath { get; set; }
            public Weaponstats weaponStats { get; set; }
            public Shopdata shopData { get; set; }
            public Skin[] skins { get; set; }

        public class Weaponstats
        {
            public float fireRate { get; set; }
            public int magazineSize { get; set; }
            public float runSpeedMultiplier { get; set; }
            public float equipTimeSeconds { get; set; }
            public float reloadTimeSeconds { get; set; }
            public float firstBulletAccuracy { get; set; }
            public int shotgunPelletCount { get; set; }
            public string wallPenetration { get; set; }
            public string feature { get; set; }
            public string fireMode { get; set; }
            public string altFireType { get; set; }
            public Adsstats adsStats { get; set; }
            public Altshotgunstats altShotgunStats { get; set; }
            public Airburststats airBurstStats { get; set; }
            public Damagerange[] damageRanges { get; set; }
        }

        public class Adsstats
        {
            public float zoomMultiplier { get; set; }
            public float fireRate { get; set; }
            public float runSpeedMultiplier { get; set; }
            public int burstCount { get; set; }
            public float firstBulletAccuracy { get; set; }
        }

        public class Altshotgunstats
        {
            public int shotgunPelletCount { get; set; }
            public float burstRate { get; set; }
        }

        public class Airburststats
        {
            public int shotgunPelletCount { get; set; }
            public float burstDistance { get; set; }
        }

        public class Damagerange
        {
            public int rangeStartMeters { get; set; }
            public int rangeEndMeters { get; set; }
            public float headDamage { get; set; }
            public int bodyDamage { get; set; }
            public float legDamage { get; set; }
        }

        public class Shopdata
        {
            public int cost { get; set; }
            public string category { get; set; }
            public string categoryText { get; set; }
            public Gridposition gridPosition { get; set; }
            public bool canBeTrashed { get; set; }
            public object image { get; set; }
            public string newImage { get; set; }
            public object newImage2 { get; set; }
            public string assetPath { get; set; }
        }

        public class Gridposition
        {
            public int row { get; set; }
            public int column { get; set; }
        }

        public class Skin
        {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string themeUuid { get; set; }
            public string contentTierUuid { get; set; }
            public string displayIcon { get; set; }
            public string wallpaper { get; set; }
            public string assetPath { get; set; }
            public Chroma[] chromas { get; set; }
            public Level[] levels { get; set; }
        }

        public class Chroma
        {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string displayIcon { get; set; }
            public string fullRender { get; set; }
            public string swatch { get; set; }
            public string streamedVideo { get; set; }
            public string assetPath { get; set; }
        }

        public class Level
        {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string levelItem { get; set; }
            public string displayIcon { get; set; }
            public string streamedVideo { get; set; }
            public string assetPath { get; set; }
        }

        public class WeaponSkin
        {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string themeUuid { get; set; }
            public string contentTierUuid { get; set; }
            public string displayIcon { get; set; }
            public string wallpaper { get; set; }
            public string assetPath { get; set; }
            public Chroma[] chromas { get; set; }
            public Level[] levels { get; set; }
        }

        public static List<WeaponSkin> GetWeaponSkins(string language = "tr-TR")
        {
            RestClient versionclient = new RestClient($"https://valorant-api.com/v1/weapons/skins?language={language}");
            RestRequest versionrequest = new RestRequest(Method.GET);

            string json = versionclient.Execute(versionrequest).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<WeaponSkin>>(obj["data"].ToString());
        }

        public static List<Weapon> GetWeapons(string language = "tr-TR")
        {
            RestClient versionclient = new RestClient($"https://valorant-api.com/v1/weapons?language={language}");
            RestRequest versionrequest = new RestRequest(Method.GET);

            string json = versionclient.Execute(versionrequest).Content;
            JToken obj = JObject.FromObject(JsonConvert.DeserializeObject(json));
            return JsonConvert.DeserializeObject<List<Weapon>>(obj["data"].ToString());
        }
    }
}
