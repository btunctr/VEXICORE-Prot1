using Prot1.Valorant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Valorant;
using Valorant.API;
using static Globals;
using Colors = Globals.Colors;
using Player = Valorant.API.GameMatch.ActiveMatch.MatchDetail.Player;

namespace Prot1.Forms
{
    #region CustomControls
    public class TeamTableItem : Button
    {
        public Brush TeamBackBrush
        {
            get { return (Brush)GetValue(TeamBackBrushProperty); }
            set { SetValue(TeamBackBrushProperty, value); }
        }

        public static readonly DependencyProperty TeamBackBrushProperty =
            DependencyProperty.RegisterAttached("TeamBackBrush", typeof(Brush), typeof(TeamTableItem), new PropertyMetadata(Brushes.White));

        public string TeamName
        {
            get { return (string)GetValue(TeamNameProperty); }
            set { SetValue(TeamNameProperty, value); }
        }

        public static readonly DependencyProperty TeamNameProperty =
            DependencyProperty.RegisterAttached("TeamName", typeof(string), typeof(TeamTableItem), new PropertyMetadata(string.Empty));


        public Brush TeamForeBrush
        {
            get { return (Brush)GetValue(TeamForeBrushProperty); }
            set { SetValue(TeamForeBrushProperty, value); }
        }

        public static readonly DependencyProperty TeamForeBrushProperty =
            DependencyProperty.RegisterAttached("TeamForeBrush", typeof(Brush), typeof(TeamTableItem), new PropertyMetadata(Brushes.White));


        static TeamTableItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TeamTableItem), new FrameworkPropertyMetadata(typeof(TeamTableItem)));
        }
    }

    public class PlayerTableItem : Button
    {
        public Brush TeamColor
        {
            get { return (Brush)GetValue(TeamColorProperty); }
            set { SetValue(TeamColorProperty, value); }
        }

        public static readonly DependencyProperty TeamColorProperty =
            DependencyProperty.Register("TeamColor", typeof(Brush), typeof(PlayerTableItem), new PropertyMetadata(null));

        public ImageSource RankIcon
        {
            get { return (ImageSource)GetValue(RankIconProperty); }
            set { SetValue(RankIconProperty, value); }
        }

        public static readonly DependencyProperty RankIconProperty =
            DependencyProperty.Register("RankIcon", typeof(ImageSource), typeof(PlayerTableItem), new PropertyMetadata(null));

        public ImageSource AgentIcon
        {
            get { return (ImageSource)GetValue(AgentIconProperty); }
            set { SetValue(AgentIconProperty, value); }
        }

        public static readonly DependencyProperty AgentIconProperty =
            DependencyProperty.Register("AgentIcon", typeof(ImageSource), typeof(PlayerTableItem), new PropertyMetadata(null));

        public string PlayerName
        {
            get { return (string)GetValue(PlayerNameProperty); }
            set { SetValue(PlayerNameProperty, value); }
        }

        public static readonly DependencyProperty PlayerNameProperty =
            DependencyProperty.Register("PlayerName", typeof(string), typeof(PlayerTableItem), new PropertyMetadata(null));

        public Brush BackgroundGradient
        {
            get { return (Brush)GetValue(BackgroundGradientProperty); }
            set { SetValue(BackgroundGradientProperty, value); }
        }

        public static readonly DependencyProperty BackgroundGradientProperty =
            DependencyProperty.Register("BackgroundGradient", typeof(Brush), typeof(PlayerTableItem), new PropertyMetadata(null));


        public string Tagline
        {
            get { return (string)GetValue(TaglineProperty); }
            set { SetValue(TaglineProperty, value); }
        }

        public static readonly DependencyProperty TaglineProperty =
            DependencyProperty.Register("Tagline", typeof(string), typeof(PlayerTableItem), new PropertyMetadata(null));

        public ImageSource LevelBorder
        {
            get { return (ImageSource)GetValue(LevelBorderProperty); }
            set { SetValue(LevelBorderProperty, value); }
        }

        public static readonly DependencyProperty LevelBorderProperty =
            DependencyProperty.Register("LevelBorder", typeof(ImageSource), typeof(PlayerTableItem), new PropertyMetadata(null));

        public string Level
        {
            get { return (string)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(string), typeof(PlayerTableItem), new PropertyMetadata(null));

        public static readonly DependencyProperty PeakRankIconProperty =
            DependencyProperty.Register("PeakRankIcon", typeof(ImageSource), typeof(PlayerTableItem), new PropertyMetadata(null));

        public static readonly DependencyProperty AvgRankIconProperty =
            DependencyProperty.Register("AvgRankIcon", typeof(ImageSource), typeof(PlayerTableItem), new PropertyMetadata(null));

        public ImageSource PeakRankIcon
        {
            get { return (ImageSource)GetValue(PeakRankIconProperty); }
            set { SetValue(PeakRankIconProperty, value); }
        }

        public ImageSource AvgRankIcon
        {
            get { return (ImageSource)GetValue(AvgRankIconProperty); }
            set { SetValue(AvgRankIconProperty, value); }
        }

        public static readonly DependencyProperty DetailedSectionVisibilityProperty =
            DependencyProperty.Register("DetailedSectionVisibility", typeof(Visibility), typeof(PlayerTableItem), new PropertyMetadata(Visibility.Collapsed));

        public Visibility DetailedSectionVisibility
        {
            get { return (Visibility)GetValue(DetailedSectionVisibilityProperty); }
            set { SetValue(DetailedSectionVisibilityProperty, value); }
        }

        public static readonly DependencyProperty IncognitoModeVisibilityProperty =
            DependencyProperty.Register("IncognitoModeVisibility", typeof(Visibility), typeof(PlayerTableItem), new PropertyMetadata(Visibility.Collapsed));

        public Visibility IncognitoModeVisibility
        {
            get { return (Visibility)GetValue(IncognitoModeVisibilityProperty); }
            set { SetValue(IncognitoModeVisibilityProperty, value); }
        }

        public static readonly RoutedEvent CardClickedEvent =
            EventManager.RegisterRoutedEvent("CardClicked", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(PlayerTableItem));

        public event RoutedEventHandler CardClicked
        {
            add { AddHandler(CardClickedEvent, value); }
            remove { RemoveHandler(CardClickedEvent, value); }
        }

        static PlayerTableItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayerTableItem), new FrameworkPropertyMetadata(typeof(PlayerTableItem)));
        }
    }
    #endregion

    public partial class Oyuncular : Page
    {
        #region Static Methods
        public enum EPlayerTeam
        {
            Blue,
            Red
        }
        public class GamePlayerTeam
        {
            public EPlayerTeam PTeam;
            public List<GameMatch.ActiveMatch.MatchDetail.Player> Players;
        }
        public class GameMatchTeamCollection : List<GamePlayerTeam>
        {
            public IEnumerable<GameMatch.ActiveMatch.MatchDetail.Player> GetAllPlayers()
            {
                var iList = new List<GameMatch.ActiveMatch.MatchDetail.Player>();
                foreach (var team in this)
                    iList.AddRange(team.Players);

                return iList;
            }
        }

        public static bool IsLoadingData = false;
        public static bool IsDataLoaded = false;
        public static GameMatchTeamCollection MatchTeams;
        public static Dictionary<string, int> AllPlayerIndexes;

        public async static Task LoadPlayerInformations()
        {
            await Task.Run(() =>
            {
                try
                {
                    IsLoadingData = true;

                    var mId = GamePlayer.GetMatchPlayer(GameAuth).MatchID;
                    if (mId != null)
                    {
                        Thread.Sleep(30);
                        var activeMatch = GameMatch.GetActiveMatch(GameAuth, mId, true, true);
                        var myTeamID = activeMatch.Players.FirstOrDefault(x => x.Subject.CompareEquality(Globals.GameAuth.subject))?.TeamID ?? "Blue";

                        if (activeMatch != null)
                        {
                            MatchTeams = new GameMatchTeamCollection()
                            {
                                new GamePlayerTeam()
                                {
                                    Players = new List<GameMatch.ActiveMatch.MatchDetail.Player>(),
                                    PTeam = EPlayerTeam.Blue
                                },
                                new GamePlayerTeam()
                                {
                                    Players = new List<GameMatch.ActiveMatch.MatchDetail.Player>(),
                                    PTeam = EPlayerTeam.Red
                                }
                            };

                            foreach (var player in activeMatch.Players)
                            {
                                if (player != null)
                                {
                                    player.PlayerTeam = player.TeamID.CompareEquality(myTeamID) ? Oyuncular.EPlayerTeam.Blue : Oyuncular.EPlayerTeam.Red;
                                    MatchTeams[player.TeamID.CompareEquality(myTeamID) ? 0 : 1].Players.Add(player);
                                }
                            }

                        }
                    }

                    AllPlayerIndexes = new Dictionary<string, int>();
                    var array = MatchTeams.GetAllPlayers().ToArray();
                    for (int i = 0; i < array.Length; ++i)
                    {
                        AllPlayerIndexes.Add(array[i].Subject.ToLower(), i);
                    }

                    GameChats = GameChat.GetCurrentGameConversationChannels(GameAuth, Oyuncular.MatchTeams.FirstOrDefault(x => x.PTeam == Oyuncular.EPlayerTeam.Red)?.Players?.Select(x => x.Subject).ToArray());

                    IsLoadingData = false;
                    IsDataLoaded = MatchTeams.Count > 0;
                }
                catch (Exception ex)
                {
                    string message = "Error: " + ex.Message + "\nStack Trace;\n\n" + ex.StackTrace + "\n\nInner Exception: " + ex.InnerException;
                    Debug.WriteLine("(Oyuncular.xaml.cs) Error at LoadPlayerInformations: " + ex.Message);
                }
            });
        }
        #endregion

        public Oyuncular()
        {
            InitializeComponent();
            this.Loaded += async (sender, e) =>
            {
                await LoadPlayerCards();

                Dispatcher.Invoke(() =>
                {
                    NormalGrid.Visibility = Visibility.Visible;
                    LoadingGrid.Visibility = Visibility.Collapsed;
                });
            };
        }

        private PlayerTableItem activeCard;
        private string activeId;

        public async Task LoadPlayerCards()
        {
            IDM.LoadDictionary();
            IDM.ImageRetrived += (type, file) =>
            {
                fileLoadingName.Dispatcher.Invoke(() => fileLoadingName.Text = file);
            };

            try
            {
                BitmapImage UnrankedImage = await IDM.GetImageAsync(GAME_CONTENT.CompetitiveTiers.ElementAt(0).tiers[0].largeIcon);

                t1Sp.Children.Clear();

                var rPlayers = new List<string>();
                foreach (var team in Oyuncular.MatchTeams)
                {
                    var partyPlayers = new Dictionary<string, List<Player>>();
                    foreach (var player in team.Players)
                    {
                        if (string.IsNullOrEmpty(player.PartyID))
                            player.PartyID = "test";

                        //if (!string.IsNullOrEmpty(player.PartyID))
                        {
                            if (!partyPlayers.ContainsKey(player.PartyID))
                            {
                                partyPlayers[player.PartyID] = new List<Player>();
                            }
                            partyPlayers[player.PartyID].Add(player);
                        }
                    }

                    bool SoloAdded = false;
                    foreach (var kvp in partyPlayers)
                    {
                        if (kvp.Value.Count == 1)
                        {
                            // SOLO
                            if (!SoloAdded)
                            {
                                AddTeamToList("Maç Takımı", team.PTeam);
                                SoloAdded = true;
                            }

                            await AddTeamPlayersToList(kvp.Value[0], UnrankedImage, team);
                        }
                    }

                    foreach (var kvp in partyPlayers)
                    {
                        if (kvp.Value.Count > 1)
                        {
                            // PARTY
                            AddTeamToList("Oyuncu Takımı", team.PTeam);
                            foreach (var p in kvp.Value)
                                await AddTeamPlayersToList(p, UnrankedImage, team);
                        }
                    }
                }

                IDM.SetDictionary();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Oyuncular: " + ex.Message + " ST:" + ex.StackTrace);
            }
        }

        private void AddTeamToList(string TeamName, EPlayerTeam team)
        {
            Dispatcher.Invoke(() =>
            {
                var teamCard = new TeamTableItem()
                {
                    TeamBackBrush = team == EPlayerTeam.Blue ? Colors._OYUNCULAR_TEAM_BANNER_BLUE : Colors._OYUNCULAR_TEAM_BANNER_RED,
                    TeamForeBrush = team == EPlayerTeam.Blue ? Colors.ForeBlue : Colors.ForeRed,
                    TeamName = TeamName,
                    Visibility = Visibility.Visible
                };
                (team == EPlayerTeam.Red ? t2Sp : t1Sp).Children.Add(teamCard);
            });
        }

        private async Task AddTeamPlayersToList(Player player, BitmapImage UnrankedImage, GamePlayerTeam team)
        {
            Debug.WriteLine(player.Username.GameName);

            var agent = GAME_CONTENT.Agents.FirstOrDefault(j => j.uuid.CompareEquality(player.CharacterID == string.Empty || player.CharacterID == null ? "320b2a48-4d9b-a075-30f1-1f93a9b638fa" : player.CharacterID));
            int pLevel = player.PlayerIdentity.AccountLevel;

            string borderUrl = string.Empty;
            int minLevel = 0;

            if (!player.PlayerIdentity.HideAccountLevel)
            {
                foreach (var border in GAME_CONTENT.LevelBorders)
                {
                    if (border.uuid == player.PlayerIdentity.PreferredLevelBorderID)
                    {
                        borderUrl = border.levelNumberAppearance;
                        break;
                    }

                    if (border.startingLevel > minLevel && pLevel >= border.startingLevel)
                    {
                        minLevel = border.startingLevel;
                        borderUrl = border.levelNumberAppearance.ToString();
                    }
                }
            }

            await Dispatcher.Invoke(async () =>
            {
                var card = new PlayerTableItem()
                {
                    RankIcon = await IDM.GetImageAsync(player.Rank?.Rank?.largeIcon) ?? UnrankedImage,
                    PeakRankIcon = player.PlayerIdentity.Incognito ? UnrankedImage : (await IDM.GetImageAsync(player.Rank?.PeakRank?.largeIcon) ?? UnrankedImage),
                    AvgRankIcon = player.PlayerIdentity.Incognito ? UnrankedImage : (await IDM.GetImageAsync(player.Rank?.AvgRank?.largeIcon) ?? UnrankedImage),
                    LevelBorder = borderUrl == string.Empty ? null : await IDM.GetImageAsync(borderUrl),
                    Level = borderUrl == string.Empty ? string.Empty :  pLevel.ToString(),
                    PlayerName = player.PlayerIdentity.Incognito && false ? agent.displayName : (player?.Username?.GameName ?? string.Empty),
                    Tagline = player.PlayerIdentity.Incognito && false ? "Gizli Hesap" : ('#' + player?.Username?.TagLine ?? string.Empty),
                    DetailedSectionVisibility = Visibility.Collapsed,
                    IncognitoModeVisibility = player.PlayerIdentity .Incognito ? Visibility.Visible : Visibility.Collapsed
                };

                card.Tag = (card, player.Subject);
                card.Click += (sender, e) =>
                {
                    var tag = ((PlayerTableItem, string))(((FrameworkElement)sender).Tag);

                    if (activeCard != null)
                    {
                        if (activeId == tag.Item2)
                        {
                            activeCard.DetailedSectionVisibility = Visibility.Collapsed;
                            activeCard = null;
                            activeId = string.Empty;
                        }
                        else
                        {
                            activeCard.DetailedSectionVisibility = Visibility.Collapsed;
                            activeCard = tag.Item1;
                            activeCard.DetailedSectionVisibility = Visibility.Visible;
                            activeId = tag.Item2;
                        }
                    }
                    else
                    {
                        tag.Item1.DetailedSectionVisibility = Visibility.Visible;
                        activeCard = tag.Item1;
                        activeId = tag.Item2;
                    }
                };

                if (agent != null)
                    card.AgentIcon = await IDM.GetImageAsync(agent.displayIcon);

                card.TeamColor = new SolidColorBrush(team.PTeam != EPlayerTeam.Blue ? Color.FromRgb(255, 70, 85) : Color.FromRgb(22, 229, 180));
                card.BackgroundGradient = team.PTeam != EPlayerTeam.Blue ?
                new LinearGradientBrush(new GradientStopCollection()
                {
                                    new GradientStop(Color.FromArgb(76,255,70,85), 0),
                                    new GradientStop(Color.FromRgb(36, 40, 48), 1),
                })
                { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) } :
                new LinearGradientBrush(new GradientStopCollection()
                {
                                    new GradientStop(Color.FromRgb(0, 87, 76), 0),
                                    new GradientStop(Color.FromRgb(36, 40, 48), 1),
                })
                { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };

                (player.PlayerTeam == EPlayerTeam.Red ? t2Sp : t1Sp).Children.Add(card);
            });
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (activeId == string.Empty)
                return;

            switch (((FrameworkElement)sender)?.Tag)
            {
                case "skin":
                    Globals.PageFrame.Content = new Desenler(new string[] { activeId.ToLower() });
                    break;
                case "overview":
                    Globals.PageFrame.Content = new PlayerOverview(activeId.ToLower());
                    break;
            }
        }
    }
}
