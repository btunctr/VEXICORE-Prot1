using Prot1.Valorant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using Valorant.API;
using static Valorant.API.GameMatch;
using static Globals;

namespace Prot1.Forms
{
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    class AgentsTable_AgentCard : Button
    {
        // Custom property for Agent's Image
        public ImageSource AgentImage
        {
            get { return (ImageSource)GetValue(AgentImageProperty); }
            set { SetValue(AgentImageProperty, value); }
        }

        public static readonly DependencyProperty AgentImageProperty =
            DependencyProperty.Register("AgentImage", typeof(ImageSource), typeof(AgentsTable_AgentCard));

        // Custom property for Agent's Name
        public string AgentNameText
        {
            get { return (string)GetValue(AgentNameTextProperty); }
            set { SetValue(AgentNameTextProperty, value); }
        }

        public static readonly DependencyProperty AgentNameTextProperty =
            DependencyProperty.Register("AgentNameText", typeof(string), typeof(AgentsTable_AgentCard));

        // Custom property for Match Count
        public string MatchCountText
        {
            get { return (string)GetValue(MatchCountTextProperty); }
            set { SetValue(MatchCountTextProperty, value); }
        }

        public static readonly DependencyProperty MatchCountTextProperty =
            DependencyProperty.Register("MatchCountText", typeof(string), typeof(AgentsTable_AgentCard));

        // Custom property for Playtime
        public string PlaytimeText
        {
            get { return (string)GetValue(PlaytimeTextProperty); }
            set { SetValue(PlaytimeTextProperty, value); }
        }

        public static readonly DependencyProperty PlaytimeTextProperty =
            DependencyProperty.Register("PlaytimeText", typeof(string), typeof(AgentsTable_AgentCard));

        // Custom property for Win Rate
        public string WinRateText
        {
            get { return (string)GetValue(WinRateTextProperty); }
            set { SetValue(WinRateTextProperty, value); }
        }

        public static readonly DependencyProperty WinRateTextProperty =
            DependencyProperty.Register("WinRateText", typeof(string), typeof(AgentsTable_AgentCard));

        // Custom property for Kill-Death Ratio (KD Ratio)
        public string KDText
        {
            get { return (string)GetValue(KDTextProperty); }
            set { SetValue(KDTextProperty, value); }
        }

        public static readonly DependencyProperty KDTextProperty =
            DependencyProperty.Register("KDText", typeof(string), typeof(AgentsTable_AgentCard));

    }

    class MapCard : Button
    {
        public string MapName
        {
            get { return (string)GetValue(MapNameProperty); }
            set { SetValue(MapNameProperty, value); }
        }

        public static readonly DependencyProperty MapNameProperty =
            DependencyProperty.Register("MapName", typeof(string), typeof(MapCard), new PropertyMetadata(string.Empty));

        public string WinLooseText
        {
            get { return (string)GetValue(WinLooseTextProperty); }
            set { SetValue(WinLooseTextProperty, value); }
        }

        public static readonly DependencyProperty WinLooseTextProperty =
            DependencyProperty.Register("WinLooseText", typeof(string), typeof(MapCard), new PropertyMetadata(string.Empty));

        public string WinPercentageText
        {
            get { return (string)GetValue(WinPercentageTextProperty); }
            set { SetValue(WinPercentageTextProperty, value); }
        }

        public static readonly DependencyProperty WinPercentageTextProperty =
            DependencyProperty.Register("WinPercentageText", typeof(string), typeof(MapCard), new PropertyMetadata(string.Empty));

        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(MapCard), new PropertyMetadata(null));
    }


    public partial class PlayerOverview : Page
    {
        public PlayerOverview(string puuid)
        {
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                IDM.LoadDictionary();

                var sw = new Stopwatch();
                sw.Start();
                LoadMatchesFromPUUID(puuid);
                Debug.WriteLine("Player Matches loaded. Took: " + sw.ElapsedMilliseconds.ToString() + " ms");
                sw.Restart();
                LoadPlayerStats();
                Debug.WriteLine("Player Stats loaded. Took: " + sw.ElapsedMilliseconds.ToString() + " ms");
                sw.Restart();
                LoadPlayerRanks();
                Debug.WriteLine("Player Ranks loaded. Took: " + sw.ElapsedMilliseconds.ToString() + " ms");
                sw.Stop();

                IDM.SetDictionary();

                NotfLbl.Content = $"DİKKAT! Bu sekmede gösterilen veriler son {Matches.Count} maça aittir!";
            };
        }

        public string Subject { get; set; }
        private List<GameMatch.MatchEnd> Matches;
        private List<GameMatch.MatchHistory> Historys = new List<GameMatch.MatchHistory>();

        private void LoadMatchesFromPUUID(string puuid)
        {
            Subject = puuid.ToLower();
            Matches = new List<GameMatch.MatchEnd>();

            var tempBuffer = GameMatch.GetMatchHistory(GameAuth, 0, 0, null, puuid);

            if (tempBuffer != null)
            {
                int totalMatches = tempBuffer.Total;

                if (totalMatches > 75) totalMatches = 75;

                for (int startIndex = 0; startIndex < totalMatches; startIndex += 25)
                {
                    int endIndex = startIndex + 25;
                    if (endIndex > totalMatches)
                    {
                        endIndex = totalMatches;
                    }

                    var hs = GameMatch.GetMatchHistory(GameAuth, startIndex, endIndex, null, puuid);

                    if (hs != null)
                    {
                        if (hs.History != null)
                        {
                            foreach (var m in hs.History)
                            {
                                Thread.Sleep(60);

                                if (m != null)
                                {
                                    var matchData = GameMatch.GetMatchDataAfterEnd(GameAuth, m.MatchID);

                                    if (matchData != null)
                                    {
                                        Matches.Add(matchData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        public async void LoadPlayerRanks()
        {
            var player = Oyuncular.MatchTeams.GetAllPlayers().FirstOrDefault(x => x.Subject.CompareEquality(Subject));
            var rank = player.Rank;

            Taglinelbl.Content = player.Username?.TagLine ?? string.Empty;
            UsernameLbl.Content = player.Username?.GameName ?? string.Empty;

            BitmapImage UnrankedImage = await IDM.GetImageAsync(GAME_CONTENT.CompetitiveTiers.ElementAt(0).tiers[0].largeIcon);

            RankIcon.Source = await IDM.GetImageAsync(rank?.Rank?.largeIcon) ?? UnrankedImage;
            PeakRankIcon.Source = await IDM.GetImageAsync(rank?.PeakRank?.largeIcon) ?? UnrankedImage;
            AvgRankIcon.Source = await IDM.GetImageAsync(rank?.AvgRank?.largeIcon) ?? UnrankedImage;

            RankText.Content = rank?.Rank?.tierName ?? "Derecesiz";
            PeakRankText.Content = rank?.PeakRank?.tierName ?? "Derecesiz";
            AvgRankText.Content = rank?.AvgRank?.tierName ?? "Derecesiz";
        }

        private class WeaponExtender : Weapon
        {
            public int WeaponKills { get; set; } = 0;

            public static WeaponExtender FromWeapon(Weapon weapon)
            {
                WeaponExtender weaponExtender = new WeaponExtender
                {
                    uuid = weapon.uuid,
                    displayName = weapon.displayName,
                    category = weapon.category,
                    defaultSkinUuid = weapon.defaultSkinUuid,
                    displayIcon = weapon.displayIcon,
                    killStreamIcon = weapon.killStreamIcon,
                    assetPath = weapon.assetPath,
                    weaponStats = weapon.weaponStats,
                    shopData = weapon.shopData,
                    skins = weapon.skins,
                    WeaponKills = 0
                };

                return weaponExtender;
            }
        }

        private class MapWinLoose
        {
            public string MapID;
            public int Win;
            public int Loose;
        }

        private class AgentStats
        {
            public int MatchCount, GameTime, Win, Loose, Kill, Death;
        }

        private double YuzdeKaci(double A, double B, int decimal_digits = 1) => (double)Math.Round((decimal)((A / (B == 0 ? 1 : B)) * 100), decimal_digits);

        public async void LoadPlayerStats()
        {
            int body = 0, head = 0, leg = 0, total = 0;
            var weaponKills = new Dictionary<string, WeaponExtender>();
            var mapWinLoose = new Dictionary<string, MapWinLoose>();
            int mCount = 0;

            var agents = new Dictionary<string, AgentStats>();

            if (Matches != null)
            {
                foreach (var md in Matches)
                {
                    if (md != null)
                    {
                        string pTeam = md.players.FirstOrDefault(x => x != null && x.subject == Subject)?.teamId;
                        bool IsPlayersTeamWon = md.teams.FirstOrDefault(t => t != null && t.teamId == pTeam)?.won ?? false;

                        var gamePlayer = md.players.FirstOrDefault(x => x.subject.CompareEquality(Subject));

                        if (gamePlayer != null)
                        {
                            if (!agents.ContainsKey(gamePlayer.characterId.ToLower()))
                            {
                                agents[gamePlayer.characterId.ToLower()] = new AgentStats()
                                {
                                    MatchCount = 1,
                                    GameTime = 0,
                                    Win = IsPlayersTeamWon ? 1 : 0,
                                    Loose = !IsPlayersTeamWon ? 1 : 0,
                                    Kill = gamePlayer.stats.kills,
                                    Death = gamePlayer.stats.deaths
                                };
                                agents[gamePlayer.characterId.ToLower()].GameTime += (int)(md.matchInfo.gameLengthMillis / 1000 / 60);
                            } else
                            {
                                var pid = gamePlayer.characterId.ToLower();

                                agents[pid].MatchCount += 1;
                                agents[pid].GameTime += (int)(md.matchInfo.gameLengthMillis / 1000 / 60);
                                agents[pid].Win += IsPlayersTeamWon ? 1 : 0;
                                agents[pid].Loose += !IsPlayersTeamWon ? 1 : 0;
                                agents[pid].Kill += gamePlayer.stats.kills;
                                agents[pid].Death += gamePlayer.stats.deaths;
                            }
                        }

                        if (md.roundResults != null)
                        {
                            foreach (var rr in md.roundResults)
                            {
                                if (rr != null)
                                {
                                    var player = rr.playerStats.FirstOrDefault(x => x != null && x.subject == Subject);

                                    if (player != null)
                                    {
                                        if (md.matchInfo != null && (md.matchInfo.queueID == "competitive" || md.matchInfo.queueID == "unrated"))
                                        {
                                            foreach (var damageData in player.damage)
                                            {
                                                body += damageData?.bodyshots ?? 0;
                                                head += damageData?.headshots ?? 0;
                                                leg += damageData?.legshots ?? 0;
                                            }
                                        }

                                        if (player.kills != null)
                                        {
                                            foreach (var kill in player.kills)
                                            {
                                                if (kill != null && !new string[] { "Weapon", "Bomb", "Ability", "Fall", "Melee", "Invalid", "Ultimate", "" }
                                                        .Contains(kill.finishingDamage?.damageItem) && kill.finishingDamage?.damageItem?.Length > 8 && kill.finishingDamage.damageItem.Contains("-"))
                                                {
                                                    var weapon = GAME_CONTENT.Weapons.FirstOrDefault(w => w != null && w.uuid.ToLower() == kill.finishingDamage.damageItem.ToLower());

                                                    if (weapon == null)
                                                        continue;

                                                    if (weaponKills.ContainsKey(weapon.uuid))
                                                        weaponKills[weapon.uuid].WeaponKills++;
                                                    else
                                                        weaponKills[weapon.uuid] = WeaponExtender.FromWeapon(weapon);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (md.matchInfo != null)
                        {
                            if (mapWinLoose.ContainsKey(md.matchInfo.mapId))
                            {
                                if (IsPlayersTeamWon)
                                    mapWinLoose[md.matchInfo.mapId].Win++;
                                else
                                    mapWinLoose[md.matchInfo.mapId].Loose++;

                                ++mCount;
                            }
                            else
                            {
                                mapWinLoose.Add(md.matchInfo.mapId, new MapWinLoose()
                                {
                                    MapID = md.matchInfo.mapId,
                                    Win = IsPlayersTeamWon ? 1 : 0,
                                    Loose = IsPlayersTeamWon ? 0 : 1
                                });

                                ++mCount;
                            }
                        }
                    }
                }
            }

            Debug.WriteLine("Map count: " + mCount.ToString());
            total = body + head + leg;

            var list = new (string, double)[]
            {
        ("head", YuzdeKaci(head, total)),
        ("body", YuzdeKaci(body, total)),
        ("legs", YuzdeKaci(leg, total))
            };
            list = list.OrderByDescending(item => item.Item2).ToArray();

            var bList = new Brush[]
            {
        new SolidColorBrush(Color.FromRgb(255, 70, 85)),
        new SolidColorBrush(Color.FromRgb(22, 229, 180)),
        new SolidColorBrush(Color.FromRgb(255, 255, 255))
            };

            var lbls = new Label[]
            {
        Accuracy_Head_Text,
        Accuracy_Body_Text,
        Accuracy_Legs_Text
            };

            string img = $"{list[0].Item1}_{list[1].Item1}.png";
            dummyIcon.Source = new BitmapImage(new Uri("../Icons/accuracy/" + img, UriKind.Relative));

            for (int i = 0; i < list.Length; ++i)
            {
                Label lbl = null;

                switch (list[i].Item1)
                {
                    case "head":
                        lbl = lbls[0];
                        break;
                    case "body":
                        lbl = lbls[1];
                        break;
                    case "legs":
                        lbl = lbls[2];
                        break;
                }

                lbl.Content = '%' + list[i].Item2.ToString();
                lbl.Foreground = bList[i];
            }

            // CLASSIC
            if (weaponKills.ContainsKey("29a0cfab-485b-f5d5-779a-b59f85e204a8"))
                weaponKills.Remove("29a0cfab-485b-f5d5-779a-b59f85e204a8");

            var wko = weaponKills.Values.OrderByDescending(x => x.WeaponKills).ToList();
            if (wko.Count() >= 3)
            {
                TopWeaponIcon1.Source = (await IDM.GetImageAsync(wko.ElementAt(0).displayIcon))?.InvertColors();
                TopWeaponIcon2.Source = (await IDM.GetImageAsync(wko.ElementAt(1).displayIcon))?.InvertColors();
                TopWeaponIcon3.Source = (await IDM.GetImageAsync(wko.ElementAt(2).displayIcon))?.InvertColors();

                TopWeaponName1.Content = wko.ElementAt(0).displayName;
                TopWeaponName2.Content = wko.ElementAt(1).displayName;
                TopWeaponName3.Content = wko.ElementAt(2).displayName;

                TopWeaponsKill1.Content = wko.ElementAt(0).WeaponKills.ToString();
                TopWeaponsKill2.Content = wko.ElementAt(1).WeaponKills.ToString();
                TopWeaponsKill3.Content = wko.ElementAt(2).WeaponKills.ToString();
            }
            else
            {
                TopWeaponsStackPanel.Visibility = Visibility.Collapsed;
            }

            mapsStackPanel.Children.Clear();
            int count = 0;
            foreach (var map in mapWinLoose.OrderByDescending(x => YuzdeKaci(x.Value.Win, x.Value.Win + x.Value.Loose)))
            {
                if (map.Value != null)
                {
                    var _map = GAME_CONTENT.Maps.FirstOrDefault(x => x != null && x.mapUrl == map.Key);

                    if (_map != null)
                    {
                        var mpcard = new MapCard()
                        {
                            WinLooseText = $"{map.Value.Win}G-{map.Value.Loose}M",
                            WinPercentageText = '%' + YuzdeKaci(map.Value.Win, map.Value.Win + map.Value.Loose).ToString(),
                            MapName = _map.displayName,
                            BackgroundImage = await IDM.GetImageAsync(_map.listViewIcon),
                            Foreground = new SolidColorBrush(count == 0 ? Color.FromRgb(225, 196, 79) : Color.FromRgb(255, 255, 255))
                        };

                        mapsStackPanel.Children.Add(mpcard);
                        count++;
                    }
                }
            }

            bool IsFirstAgent = true;
            AgentsDetailList.Children.Clear();
            foreach (var kvp in agents.OrderByDescending(x => x.Value.GameTime).Take(5))
            {
                var agent = GAME_CONTENT.Agents.FirstOrDefault(a => a.uuid.CompareEquality(kvp.Key));

                if (agent != null)
                {
                    var card = new AgentsTable_AgentCard()
                    {
                        AgentImage = await IDM.GetImageAsync(agent.displayIcon),
                        AgentNameText = agent.displayName.ToUpper(),
                        MatchCountText = kvp.Value.MatchCount.ToString() + " Maç",
                        WinRateText = '%' + YuzdeKaci(kvp.Value.Win, kvp.Value.Win + kvp.Value.Loose, 1).ToString(),
                        KDText = (Math.Round((double)((double)kvp.Value.Kill / (double)kvp.Value.Death), 2)).ToString(),
                        PlaytimeText = Math.Truncate((decimal)(kvp.Value.GameTime / 60)).ToString() + " Saat",
                    };

                    if (IsFirstAgent)
                    {
                        card.BorderThickness = new Thickness(1.0, .0, .0, .0);
                        IsFirstAgent = false;
                    }
                    else
                        card.BorderThickness = new Thickness(.0, .0, .0, .0);

                    AgentsDetailList.Children.Add(card);
                }
            }
        }
    }
}
