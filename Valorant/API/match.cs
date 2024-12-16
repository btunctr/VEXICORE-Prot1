using Newtonsoft.Json;
using Prot1.Forms;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Valorant.API.GameMatch.ActiveMatch;
using static Valorant.API.GameMatch.ActiveMatch.MatchDetail.Player;

namespace Valorant.API
{
    public class GameMatch
    {
        public class MatchEnd
        {
            public MatchInfo matchInfo { get; set; }
            public List<Player> players { get; set; }
            public List<object> bots { get; set; }
            public List<Team> teams { get; set; }
            public List<RoundResult> roundResults { get; set; }
            public List<Kill> kills { get; set; }
            public int StatusCode { get; set; }
            public class MatchInfo
            {
                public string matchId { get; set; }
                public string mapId { get; set; }
                public string gamePodId { get; set; }
                public string gameLoopZone { get; set; }
                public string gameServerAddress { get; set; }
                public string gameVersion { get; set; }
                public int gameLengthMillis { get; set; }
                public long gameStartMillis { get; set; }
                public string provisioningFlowID { get; set; }
                public bool isCompleted { get; set; }
                public string customGameName { get; set; }
                public bool forcePostProcessing { get; set; }
                public string queueID { get; set; }
                public string gameMode { get; set; }
                public bool isRanked { get; set; }
                public bool canProgressContracts { get; set; }
                public bool isMatchSampled { get; set; }
                public string seasonId { get; set; }
                public string completionState { get; set; }
                public string platformType { get; set; }
            }

            public class PlatformInfo
            {
                public string platformType { get; set; }
                public string platformOS { get; set; }
                public string platformOSVersion { get; set; }
                public string platformChipset { get; set; }
            }

            public class Stats
            {
                public int score { get; set; }
                public int roundsPlayed { get; set; }
                public int kills { get; set; }
                public int deaths { get; set; }
                public int assists { get; set; }
                public int playtimeMillis { get; set; }
            }

            public class BasicMovement
            {
                public int idleTimeMillis { get; set; }
                public int objectiveCompleteTimeMillis { get; set; }
            }

            public class BasicGunSkill
            {
                public int idleTimeMillis { get; set; }
                public int objectiveCompleteTimeMillis { get; set; }
            }

            public class AdaptiveBots
            {
                public int idleTimeMillis { get; set; }
                public int objectiveCompleteTimeMillis { get; set; }
                public int adaptiveBotAverageDurationMillisAllAttempts { get; set; }
                public int adaptiveBotAverageDurationMillisFirstAttempt { get; set; }
                public object killDetailsFirstAttempt { get; set; }
            }

            public class Ability
            {
                public int idleTimeMillis { get; set; }
                public int objectiveCompleteTimeMillis { get; set; }
                public object grenadeEffects { get; set; }
                public object ability1Effects { get; set; }
                public object ability2Effects { get; set; }
                public object ultimateEffects { get; set; }
            }

            public class BombPlant
            {
                public int idleTimeMillis { get; set; }
                public int objectiveCompleteTimeMillis { get; set; }
            }

            public class DefendBombSite
            {
                public int idleTimeMillis { get; set; }
                public int objectiveCompleteTimeMillis { get; set; }
                public bool success { get; set; }
            }

            public class SettingStatus
            {
                public bool isMouseSensitivityDefault { get; set; }
                public bool isCrosshairDefault { get; set; }
            }

            public class NewPlayerExperienceDetails
            {
                public BasicMovement basicMovement { get; set; }
                public BasicGunSkill basicGunSkill { get; set; }
                public AdaptiveBots adaptiveBots { get; set; }
                public Ability ability { get; set; }
                public BombPlant bombPlant { get; set; }
                public DefendBombSite defendBombSite { get; set; }
                public SettingStatus settingStatus { get; set; }
            }

            public class Player
            {
                public string subject { get; set; }
                public string gameName { get; set; }
                public string tagLine { get; set; }
                public PlatformInfo platformInfo { get; set; }
                public string teamId { get; set; }
                public string partyId { get; set; }
                public string characterId { get; set; }
                public Stats stats { get; set; }
                public object roundDamage { get; set; }
                public int competitiveTier { get; set; }
                public string playerCard { get; set; }
                public string playerTitle { get; set; }
                public int sessionPlaytimeMinutes { get; set; }
                public NewPlayerExperienceDetails newPlayerExperienceDetails { get; set; }
            }

            public class Team
            {
                public string teamId { get; set; }
                public bool won { get; set; }
                public int roundsPlayed { get; set; }
                public int roundsWon { get; set; }
                public int numPoints { get; set; }
            }

            public class PlantLocation
            {
                public int x { get; set; }
                public int y { get; set; }
            }

            public class DefuseLocation
            {
                public int x { get; set; }
                public int y { get; set; }
            }

            public class VictimLocation
            {
                public int x { get; set; }
                public int y { get; set; }
            }

            public class FinishingDamage
            {
                public string damageType { get; set; }
                public string damageItem { get; set; }
                public bool isSecondaryFireMode { get; set; }
            }

            public class Kill
            {
                public int gameTime { get; set; }
                public int roundTime { get; set; }
                public string killer { get; set; }
                public string victim { get; set; }
                public VictimLocation victimLocation { get; set; }
                public List<string> assistants { get; set; }
                public List<object> playerLocations { get; set; }
                public FinishingDamage finishingDamage { get; set; }
                public int round { get; set; }
            }

            public class Economy
            {
                public int loadoutValue { get; set; }
                public string weapon { get; set; }
                public string armor { get; set; }
                public int remaining { get; set; }
                public int spent { get; set; }
            }

            public class PlayerStat
            {
                public string subject { get; set; }
                public List<Kill> kills { get; set; }
                public List<PlayerRoundDamage> damage { get; set; }
                public int score { get; set; }
                public Economy economy { get; set; }
                public Ability ability { get; set; }
                public bool wasAfk { get; set; }
                public bool wasPenalized { get; set; }
                public bool stayedInSpawn { get; set; }
            }

            public class PlayerRoundDamage
            {
                public string receiver { get; set; }
                public int damage { get; set; }
                public int legshots { get; set; }
                public int bodyshots { get; set; }
                public int headshots { get; set; }
            }

            public class RoundResult
            {
                public int roundNum { get; set; }
                public string roundResult { get; set; }
                public string roundCeremony { get; set; }
                public string winningTeam { get; set; }
                public int plantRoundTime { get; set; }
                public object plantPlayerLocations { get; set; }
                public PlantLocation plantLocation { get; set; }
                public string plantSite { get; set; }
                public int defuseRoundTime { get; set; }
                public object defusePlayerLocations { get; set; }
                public DefuseLocation defuseLocation { get; set; }
                public List<PlayerStat> playerStats { get; set; }
                public string roundResultCode { get; set; }
                public object playerEconomies { get; set; }
                public object playerScores { get; set; }
            }
        }
        public class PreGame
        {
            public string ID { get; set; }
            public long Version { get; set; }
            public List<Team> Teams { get; set; }
            public AllyTeamobj AllyTeam { get; set; }
            public object EnemyTeam { get; set; }
            public List<object> ObserverSubjects { get; set; }
            public List<object> MatchCoaches { get; set; }
            public int EnemyTeamSize { get; set; }
            public int EnemyTeamLockCount { get; set; }
            public string PregameState { get; set; }
            public DateTime LastUpdated { get; set; }
            public string MapID { get; set; }
            public string GamePodID { get; set; }
            public string Mode { get; set; }
            public string VoiceSessionID { get; set; }
            public string MUCName { get; set; }
            public string QueueID { get; set; }
            public string ProvisioningFlowID { get; set; }
            public bool IsRanked { get; set; }
            public long PhaseTimeRemainingNS { get; set; }
            public bool altModesFlagADA { get; set; }
            public int StatusCode { get; set; }
            public class PlayerIdentity
            {
                public string Subject { get; set; }
                public string PlayerCardID { get; set; }
                public string PlayerTitleID { get; set; }
                public bool Incognito { get; set; }
            }

            public class SeasonalBadgeInfo
            {
                public string SeasonID { get; set; }
                public int NumberOfWins { get; set; }
                public object WinsByTier { get; set; }
                public int Rank { get; set; }
                public int LeaderboardRank { get; set; }
            }

            public class Player
            {
                public string Subject { get; set; }
                public string CharacterID { get; set; }
                public string CharacterSelectionState { get; set; }
                public string PregamePlayerState { get; set; }
                public int CompetitiveTier { get; set; }
                public PlayerIdentity PlayerIdentity { get; set; }
                public SeasonalBadgeInfo SeasonalBadgeInfo { get; set; }
            }

            public class Team
            {
                public string TeamID { get; set; }
                public List<Player> Players { get; set; }
            }

            public class AllyTeamobj
            {
                public string TeamID { get; set; }
                public List<Player> Players { get; set; }
            }
        }
        public class ActiveMatch
        {
            public class MatchPlayerLoadout
            {
                public string CharacterID { get; set; }
                public List<LoadoutItem> PlayerLoadout { get; set; }

                public class LoadoutItem
                {
                    public string ID { get; set; }
                    public string TypeID { get; set; }
                    public Dictionary<string, LoadoutItemSocket> Sockets { get; set; }
                }

                public class SocketItem
                {
                    public string ID { get; set; }
                    public string TypeID { get; set; }
                }

                public class LoadoutItemSocket
                {
                    public string ID { get; set; }
                    public SocketItem Item { get; set; }
                }
            }

            public class MatchDetail
            {
                public string MatchID { get; set; }
                public string Version { get; set; }
                public string State { get; set; }
                public string MapID { get; set; }
                public string ModeID { get; set; }
                public string ProvisioningFlow { get; set; }
                public string GamePodID { get; set; }
                public string AllMUCName { get; set; }
                public string TeamMUCName { get; set; }
                public string TeamVoiceID { get; set; }
                public bool IsReconnectable { get; set; }
                public List<Player> Players { get; set; }

                public class Player
                {
                    public string Subject { get; set; }
                    public string TeamID { get; set; }
                    public string CharacterID { get; set; }
                    public PlayerIdentity PlayerIdentity { get; set; }
                    public SeasonalBadgeInfo SeasonalBadgeInfo { get; set; }
                    public MatchPlayerLoadout Loadout { get; set; }
                    public Oyuncular.EPlayerTeam PlayerTeam { get; set; }

                    public string PartyID;
                    public Username Username;
                    public UserRankResponse Rank;

                    public class UserRankResponse
                    {
                        public string PlayerID;
                        public CompetitiveTier.Tier Rank, AvgRank, PeakRank;
                    }
                }

                public class PlayerIdentity
                {
                    public string Subject { get; set; }
                    public string PlayerCardID { get; set; }
                    public string PlayerTitleID { get; set; }
                    public int AccountLevel { get; set; }
                    public string PreferredLevelBorderID { get; set; }
                    public bool Incognito { get; set; }
                    public bool HideAccountLevel { get; set; }
                }

                public class SeasonalBadgeInfo
                {
                    public string SeasonID { get; set; }
                    public int NumberOfWins { get; set; }
                    public object WinsByTier { get; set; }
                    public int Rank { get; set; }
                    public int LeaderboardRank { get; set; }
                }

            }
        }
        public class MatchHistory
        {
            public string Subject { get; set; }
            public int BeginIndex { get; set; }
            public int EndIndex { get; set; }
            public int Total { get; set; }
            public int StatusCode { get; set; }
            public List<Matches> History { get; set; }
            public class Matches
            {
                public string MatchID { get; set; }
                public object GameStartTime { get; set; }
                public string TeamID { get; set; }
            }
        }

        public static MatchEnd GetMatchDataAfterEnd(Auth au, string matchID)
        {
            MatchEnd ret = new MatchEnd();
            string url = "https://pd." + au.region + ".a.pvp.net/match-details/v1/matches/" + matchID;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;
            //client.CookieContainer = new CookieContainer();

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");

            var responce = client.Execute(request);
            string responcecontent = responce.Content;
            try
            {
                ret = JsonConvert.DeserializeObject<MatchEnd>(responcecontent);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at match.cs");
                return null;
            }
            return ret;
        }

        public static PreGame GetPreMatch(Auth au, string matchid)
        {
            string url = "https://glz-" + au.region + "-1." + au.region + ".a.pvp.net/pregame/v1/matches/" + matchid;
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");
            try
            {
                return JsonConvert.DeserializeObject<PreGame>(client.Execute(request).Content);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at match.cs");
                return null;
            }
        }

        public static MatchDetail GetActiveMatch(Auth au, string MatchID, bool loadPlayerLoadouts = false, bool loadParties = true)
        {
            var list = loadPlayerLoadouts ? GetPlayerLoadouts(au, MatchID) : null;

            MatchDetail ret = new MatchDetail();
            string url = $"https://glz-{au.region}-1.{au.region}.a.pvp.net/core-game/v1/matches/{MatchID}";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");
            try
            {
                string contr = client.Execute(request).Content;
                ret = JsonConvert.DeserializeObject<MatchDetail>(contr);
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at mmr.cs");
                return null;
            }


            var activeSeason = LContent.FetchContents(au)?.Seasons?.FirstOrDefault(j => j.IsActive);
            var uNames = Username.GetUsernames(au, ret.Players?.Select(j => j.Subject.ToLower())?.ToArray())?.ToDictionary(x => x.Subject.ToLower());

            List<UserPresence.Presence> presences = null;
            var presDict = new Dictionary<string, string>();

            if (loadParties)
            {
                var userPresence = UserPresence.GetPresence(au);
                    if (userPresence != null)
                {
                    presences = userPresence.presences;
                    if (presences != null)
                    {
                        foreach (var p in presences)
                        {
                            if (p?.privinfo == null)
                                continue;

                            if (!presDict.ContainsKey(p.puuid?.ToLower()))
                                presDict[p.puuid.ToLower()] = p.privinfo.partyId?.ToLower();
                        }
                    }
                }
            }

            for (int i = 0; i < ret.Players?.Count; ++i)
            {
                if (list != null && loadPlayerLoadouts)
                {
                    int invIndex;

                    if (ret.Players[i].TeamID == "Red")
                        invIndex = i + ret.Players.Count - list.Count();
                    else
                        invIndex = i;

                    ret.Players[i].Loadout = list.ElementAtOrDefault(invIndex);
                }

                if (uNames != null)
                {
                    uNames.TryGetValue(ret.Players[i].Subject?.ToLower(), out ret.Players[i].Username);
                }

                ret.Players[i].Rank = GetPlayerRanks(au, ret.Players[i].Subject?.ToLower(), activeSeason?.ID?.ToLower());

                if (presences != null)
                {
                    presDict.TryGetValue(ret.Players[i].Subject?.ToLower(), out ret.Players[i].PartyID);
                }
            }


            return ret;
        }
        private static UserRankResponse GetPlayerRanks(Auth au, string puuid, string activeSeason)
        {
            try
            {
                var br = MMR.GetMMR(au, puuid);

                if (br == null)
                    return null;

                int max_rank = 0;
                int avg_rank = 0;

                if (br.QueueSkills != null && br.QueueSkills.ContainsKey("competitive"))
                {
                    var seasons = br.QueueSkills["competitive"]?.SeasonalInfoBySeasonID;
                    if (seasons != null)
                    {
                        foreach (var season in seasons)
                        {
                            if (season.Value == null)
                                continue;

                            int sRank = season.Value.Rank;

                            if (Globals.BeforeAscendantSeasons.Any(x => x?.ToLower() == season.Value.SeasonID?.ToLower()))
                            {
                                if (sRank > 20)
                                    sRank += 3;
                            }
                            if (sRank > max_rank)
                            {
                                max_rank = sRank;
                            }
                            avg_rank += sRank;
                        }

                        if (seasons.Count() < 1)
                            avg_rank = 0;
                        else
                            avg_rank /= seasons.Count();

                        CompetitiveTier.Tier[] array = new CompetitiveTier.Tier[3];

                        if (Globals.GAME_CONTENT != null && Globals.GAME_CONTENT.CompetitiveTiers != null &&
                            Globals.GAME_CONTENT.CompetitiveTiers.Count > 0)
                        {
                            var sDictionary = br.QueueSkills["competitive"].SeasonalInfoBySeasonID;

                            array[0] = Globals.GAME_CONTENT.CompetitiveTiers[0].tiers[!sDictionary.ContainsKey(activeSeason) ? 0 : sDictionary[activeSeason]?.CompetitiveTier ?? 0];

                            array[1] = Globals.GAME_CONTENT.CompetitiveTiers[0].tiers[max_rank];
                            array[2] = Globals.GAME_CONTENT.CompetitiveTiers[0].tiers[avg_rank];

                            return new UserRankResponse()
                            {
                                PlayerID = puuid,
                                Rank = array[0],
                                PeakRank = array[1],
                                AvgRank = array[2],
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occurred while getting user ranks : " + ex.Message + "\nST:" + ex.StackTrace);
            }
            return null;
        }

        public static List<MatchPlayerLoadout> GetPlayerLoadouts(Auth au, string MatchID)
        {
            List<MatchPlayerLoadout> ret = new List<MatchPlayerLoadout>();
            string url = $"https://glz-{au.region}-1.{au.region}.a.pvp.net/core-game/v1/matches/{MatchID}/loadouts";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
            request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
            request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            request.AddHeader("X-Riot-ClientVersion", $"{au.version}");

            try
            {
                var response = client.Execute(request);
                if (response == null || response.ErrorException != null)
                {
                    return null;
                }

                var pl = JsonConvert.DeserializeObject<_private_loadout_response>(response.Content);

                if (pl != null)
                {
                    foreach (var l in pl.Loadouts)
                    {
                        var mpl = new MatchPlayerLoadout()
                        {
                            CharacterID = l.CharacterID,
                            PlayerLoadout = new List<MatchPlayerLoadout.LoadoutItem>()
                        };

                        foreach (var Item in l.Loadout.Items.Values)
                        {
                            var dctEntry = new MatchPlayerLoadout.LoadoutItem()
                            {
                                ID = Item.ID,
                                TypeID = Item.TypeID,
                                Sockets = new Dictionary<string, MatchPlayerLoadout.LoadoutItemSocket>()
                            };

                            foreach (var socket in Item.Sockets.Values)
                            {
                                if (!string.IsNullOrEmpty(socket.ID))
                                {
                                    var socketItem = new MatchPlayerLoadout.SocketItem()
                                    {
                                        ID = socket.Item.ID,
                                        TypeID = socket.Item.TypeID
                                    };

                                    dctEntry.Sockets.Add(socket.ID, new MatchPlayerLoadout.LoadoutItemSocket()
                                    {
                                        ID = socket.ID,
                                        Item = socketItem
                                    });
                                }
                            }

                            mpl.PlayerLoadout.Add(dctEntry);
                        }
                        ret.Add(mpl);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occurred while fetching player loadouts: " + ex.Message + "\nST:" + ex.StackTrace);
            }

            return ret;
        }

        public class _private_loadout_response
        {
            public List<LoadoutEntry> Loadouts { get; set; }

            public class Item
            {
                public string ID { get; set; }
                public string TypeID { get; set; }
            }

            public class Socket
            {
                public string ID { get; set; }
                public Item Item { get; set; }
            }

            public class LoadoutItem
            {
                public string ID { get; set; }
                public string TypeID { get; set; }
                public Dictionary<string, Socket> Sockets { get; set; }
            }

            public class Loadout
            {
                public Dictionary<string, LoadoutItem> Items { get; set; }
            }

            public class LoadoutEntry
            {
                public string CharacterID { get; set; }
                public Loadout Loadout { get; set; }
            }
        }

            public static MatchHistory GetMatchHistory(Auth au, int startindex, int endindex, string queue = null, string playerid = "useauth")
            {
                MatchHistory ret = new MatchHistory();
                if (playerid == "useauth")
                {
                    playerid = au.subject;
                }
                string paramz = "?startIndex=" + startindex + "&endIndex=" + endindex + (queue == null || queue == string.Empty ? string.Empty : "&queue=" + queue);
                RestClient client = new RestClient("https://pd." + au.region + ".a.pvp.net/match-history/v1/history/" + playerid + paramz);
                client.CookieContainer = au.cookies;

                RestRequest request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", $"Bearer {au.AccessToken}");
                request.AddHeader("X-Riot-Entitlements-JWT", $"{au.EntitlementToken}");
                request.AddHeader("X-Riot-ClientPlatform", $"ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
                request.AddHeader("X-Riot-ClientVersion", $"{au.version}");

                try
                {
                    return JsonConvert.DeserializeObject<MatchHistory>(client.Execute(request).Content);
                }
                catch (JsonException)
                {
                    Debug.WriteLine("Error occured while deserilizeing json at match.cs");
                    return null;
                }
            }
        }
    }