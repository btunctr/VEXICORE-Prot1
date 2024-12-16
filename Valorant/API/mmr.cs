using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;

namespace Valorant.API
{
    public class MMR
    {
            public string Version { get; set; }
            public string Subject { get; set; }
            public bool NewPlayerExperienceFinished { get; set; }
            public Dictionary<string, QueueSkillsData> QueueSkills { get; set; }
            public LatestCompetitiveUpdateData LatestCompetitiveUpdate { get; set; }
            public bool IsLeaderboardAnonymized { get; set; }
            public bool IsActRankBadgeHidden { get; set; }

        public class QueueSkillsData
        {
            public int TotalGamesNeededForRating { get; set; }
            public int TotalGamesNeededForLeaderboard { get; set; }
            public int CurrentSeasonGamesNeededForRating { get; set; }
            public Dictionary<string, SeasonalInfoData> SeasonalInfoBySeasonID { get; set; }
        }

        public class SeasonalInfoData
        {
            public string SeasonID { get; set; }
            public int NumberOfWins { get; set; }
            public int NumberOfWinsWithPlacements { get; set; }
            public int NumberOfGames { get; set; }
            public int Rank { get; set; }
            public int CapstoneWins { get; set; }
            public int LeaderboardRank { get; set; }
            public int CompetitiveTier { get; set; }
            public int RankedRating { get; set; }
            public Dictionary<string, int> WinsByTier { get; set; }
            public int GamesNeededForRating { get; set; }
            public int TotalWinsNeededForRank { get; set; }
        }

        public class LatestCompetitiveUpdateData
        {
            public string MatchID { get; set; }
            public string MapID { get; set; }
            public string SeasonID { get; set; }
            public long MatchStartTime { get; set; }
            public int TierAfterUpdate { get; set; }
            public int TierBeforeUpdate { get; set; }
            public int RankedRatingAfterUpdate { get; set; }
            public int RankedRatingBeforeUpdate { get; set; }
            public int RankedRatingEarned { get; set; }
            public int RankedRatingPerformanceBonus { get; set; }
            public string CompetitiveMovement { get; set; }
            public int AFKPenalty { get; set; }
        }

        public static dynamic GetMMRDynamic(Auth au, string playerid = "useauth")
        {
            if (playerid == "useauth")
            {
                playerid = au.subject;
            }
            RestClient client = new RestClient("https://pd." + au.region + ".a.pvp.net/mmr/v1/players/" + playerid);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");
            
            try
            {
                return JsonConvert.DeserializeObject(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at mmr.cs");
                return null;
            }
        }

        public static MMR GetMMR(Auth au, string playerid = "useauth")
        {
            if (playerid == "useauth")
            {
                playerid = au.subject;
            }
            RestClient client = new RestClient("https://pd." + au.region + ".a.pvp.net/mmr/v1/players/" + playerid);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");
            try
            {
                return JsonConvert.DeserializeObject<MMR>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at mmr.cs");
                return null;
            }
        }
    }
}
