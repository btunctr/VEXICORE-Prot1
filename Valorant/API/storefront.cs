using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Valorant.API.GameChat;
using Valorant.API;

namespace Prot1.Valorant.API
{
    public class Storefront
    {
        public class StorefrontResponse
        {
            public FeaturedBundleType FeaturedBundle { get; set; }
            public SkinsPanelLayoutType SkinsPanelLayout { get; set; }
            public UpgradeCurrencyStoreType UpgradeCurrencyStore { get; set; }
            public BonusStoreType BonusStore { get; set; }
        }

        public class FeaturedBundleType
        {
            public BundleType Bundle { get; set; }
            public List<BundleType> Bundles { get; set; }
            public int BundleRemainingDurationInSeconds { get; set; }
        }

        public class BundleType
        {
            public string ID { get; set; }
            public string DataAssetID { get; set; }
            public string CurrencyID { get; set; }
            public List<ItemType> Items { get; set; }
            public double BasePrice { get; set; }
            public double DiscountPercent { get; set; }
            public double DiscountedPrice { get; set; }
            public bool IsPromoItem { get; set; }
        }

        public class ItemType
        {
            public ITItem Item { get; set; }
        }

        public class ITItem
        {
            public string ItemTypeID { get; set; }
            public string ItemID { get; set; }
            public int Quantity { get; set; }
        }

        public class SkinsPanelLayoutType
        {
            public List<string> SingleItemOffers { get; set; }
            public List<SingleItemStoreOfferType> SingleItemStoreOffers { get; set; }
            public int SingleItemOffersRemainingDurationInSeconds { get; set; }
        }

        public class SingleItemStoreOfferType
        {
            public string OfferID { get; set; }
            public bool IsDirectPurchase { get; set; }
            public string StartDate { get; set; }
            public Dictionary<string, double> Cost { get; set; }
            public List<ITItem> Rewards { get; set; }
        }

        public class UpgradeCurrencyStoreType
        {
            public List<UpgradeCurrencyOfferType> UpgradeCurrencyOffers { get; set; }
        }

        public class UpgradeCurrencyOfferType
        {
            public string OfferID { get; set; }
            public string StorefrontItemID { get; set; }
            public OfferType Offer { get; set; }
        }

        public class OfferType
        {
            public string OfferID { get; set; }
            public bool IsDirectPurchase { get; set; }
            public string StartDate { get; set; }
            public Dictionary<string, double> Cost { get; set; }
            public List<ItemType> Rewards { get; set; }
        }

        public class BonusStoreType
        {
            public List<BonusStoreOfferType> BonusStoreOffers { get; set; }
            public int BonusStoreRemainingDurationInSeconds { get; set; }
        }

        public class BonusStoreOfferType
        {
            public string BonusOfferID { get; set; }
            public OfferType Offer { get; set; }
            public double DiscountPercent { get; set; }
            public Dictionary<string, double> DiscountCosts { get; set; }
            public bool IsSeen { get; set; }
        }

        public static StorefrontResponse GetStorefront(Auth au)
        {
            string url = $"https://pd.{au.region}.a.pvp.net/store/v2/storefront/{au.subject}";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");

            try
            {
                return JsonConvert.DeserializeObject<StorefrontResponse>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at storefront.cs");
                return null;
            }
        }

        public List<Offer> Offers { get; set; }

        public class Offer
        {
            public string OfferID { get; set; }
            public bool IsDirectPurchase { get; set; }
            public string StartDate { get; set; }
            public Dictionary<string, decimal> Cost { get; set; }
            public List<Reward> Rewards { get; set; }
        }

        public class Reward
        {
            public string ItemTypeID { get; set; }
            public string ItemID { get; set; }
            public decimal Quantity { get; set; }
        }

        public static Storefront GetPrices(Auth au)
        {
            string url = $"https://pd.{au.region.ToString().ToLower()}.a.pvp.net/store/v1/offers/";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");

            try
            {
                return JsonConvert.DeserializeObject<Storefront>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at storefront.cs");
                return null;
            }
        }
    }
}
