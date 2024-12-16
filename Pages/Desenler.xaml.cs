using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValAPINet;
using Valorant;
using Valorant.API;
using Valorant.API.External;
using static Valorant.API.LContent;
using static Globals;

namespace Prot1.Forms
{ 
    #region Custom Controls
    public class PlayerCardSmall : Button
    {
        public static readonly DependencyProperty AgentImageProperty =
                   DependencyProperty.Register("AgentImage", typeof(ImageSource), typeof(PlayerCardSmall), new PropertyMetadata(null));

        static PlayerCardSmall()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayerCardSmall), new FrameworkPropertyMetadata(typeof(PlayerCardSmall)));
        }

        public ImageSource AgentImage
        {
            get { return (ImageSource)GetValue(AgentImageProperty); }
            set { SetValue(AgentImageProperty, value); }
        }
    }

    public class WeaponCardButton : Button
    {
        public Brush AgentCardColor
        {
            get { return (Brush)GetValue(AgentCardColorProperty); }
            set { SetValue(AgentCardColorProperty, value); }
        }

        public static readonly DependencyProperty AgentCardColorProperty =
            DependencyProperty.Register("AgentCardColor", typeof(Brush), typeof(WeaponCardButton), new PropertyMetadata(null));

        public static readonly DependencyProperty AgentIconProperty =
           DependencyProperty.Register("AgentIcon", typeof(ImageSource), typeof(WeaponCardButton), new PropertyMetadata(null));

        public ImageSource AgentIcon
        {
            get { return (ImageSource)GetValue(AgentIconProperty); }
            set { SetValue(AgentIconProperty, value); }
        }

        public static readonly DependencyProperty AgentIconVisProperty =
           DependencyProperty.Register("AgentIconVis", typeof(Visibility), typeof(WeaponCardButton), new PropertyMetadata(Visibility.Visible));

        public Visibility AgentIconVis
        {
            get { return (Visibility)GetValue(AgentIconVisProperty); }
            set { SetValue(AgentIconVisProperty, value); }
        }

        public static readonly DependencyProperty TierIconProperty =
           DependencyProperty.Register("TierIcon", typeof(ImageSource), typeof(WeaponCardButton), new PropertyMetadata(null));

        public ImageSource TierIcon
        {
            get { return (ImageSource)GetValue(TierIconProperty); }
            set { SetValue(TierIconProperty, value); }
        }

        public static readonly DependencyProperty WeaponIconProperty =
            DependencyProperty.Register("WeaponIcon", typeof(ImageSource), typeof(WeaponCardButton), new PropertyMetadata(null));

        public ImageSource WeaponIcon
        {
            get { return (ImageSource)GetValue(WeaponIconProperty); }
            set { SetValue(WeaponIconProperty, value); }
        }

        public static readonly DependencyProperty ChromaIconProperty =
            DependencyProperty.Register("ChromaIcon", typeof(ImageSource), typeof(WeaponCardButton), new PropertyMetadata(null));

        public ImageSource ChromaIcon
        {
            get { return (ImageSource)GetValue(ChromaIconProperty); }
            set { SetValue(ChromaIconProperty, value); }
        }

        public static readonly DependencyProperty WeaponNameProperty =
            DependencyProperty.Register("WeaponName", typeof(string), typeof(WeaponCardButton), new PropertyMetadata(""));

        public string WeaponName
        {
            get { return (string)GetValue(WeaponNameProperty); }
            set { SetValue(WeaponNameProperty, value); }
        }

        public static readonly DependencyProperty CardBackColorProperty =
            DependencyProperty.Register("CardBackColor", typeof(Brush), typeof(WeaponCardButton), new PropertyMetadata(null));

        public Brush CardBackColor
        {
            get { return (Brush)GetValue(CardBackColorProperty); }
            set { SetValue(CardBackColorProperty, value); }
        }

        public static readonly DependencyProperty CardBorderColorProperty =
            DependencyProperty.Register("CardBorderColor", typeof(Brush), typeof(WeaponCardButton), new PropertyMetadata(null));

        public Brush CardBorderColor
        {
            get { return (Brush)GetValue(CardBorderColorProperty); }
            set { SetValue(CardBorderColorProperty, value); }
        }

        static WeaponCardButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeaponCardButton), new FrameworkPropertyMetadata(typeof(WeaponCardButton)));
        }

        public void SetColor(Brush[] barray)
        {
            this.CardBorderColor = barray[0];
            this.CardBackColor = barray[1];
        }

        public WeaponCardButton()
        {

        }
    }

    public class Desenler_WeaponCard : Button
    {
        public static readonly DependencyProperty WeaponIconProperty =
                   DependencyProperty.Register("WeaponIcon", typeof(ImageSource), typeof(Desenler_WeaponCard), new PropertyMetadata(null));

        static Desenler_WeaponCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Desenler_WeaponCard), new FrameworkPropertyMetadata(typeof(Desenler_WeaponCard)));
        }

        public ImageSource WeaponIcon
        {
            get { return (ImageSource)GetValue(WeaponIconProperty); }
            set { SetValue(WeaponIconProperty, value); }
        }

        public void SetActiveState(bool state)
        {
            if (state)
                this.BorderThickness = new Thickness(2.5, 0, 2.5, 0);
            else
                this.BorderThickness = new Thickness(2.5, 0, 0, 0);
        }

        public Desenler_WeaponCard()
        {
            SetActiveState(false);
        }
    }
    #endregion

    public partial class Desenler : Page
    {
        public List<string> WeaponFilter;
        public List<GameMatch.ActiveMatch.MatchDetail.Player> SelectedPlayers;      

        public Desenler(string[] id)
        {
            InitializeComponent();

            wpnFilterPistols.MouseDown += (sender, e) => WeaponCardClicked(sender, e);
            wpnFilterAll.MouseDown += (sender, e) => WeaponCardClicked(sender, e);
            wpnFilterOther.MouseDown += (sender, e) => WeaponCardClicked(sender, e);

            this.Loaded += (sender, e) =>
            {
                IDM.LoadDictionary();
                IDM.ImageRetrived += (type, file) => {
                    fileLoadingName.Dispatcher.Invoke(() => fileLoadingName.Text = file);
                };

                Task.Run(async () =>
                {
                    await this.Load();
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().Where(x => id.Contains(x.Subject.ToLower())).ToList();
                    await this.LoadWeapons();
                    IDM.SetDictionary();

                    Dispatcher.Invoke(() =>
                    {
                        NormalGrid.Visibility = Visibility.Visible;
                        LoadingGrid.Visibility = Visibility.Collapsed;
                    });
                });
            };
        }

        public Desenler()
        {
            InitializeComponent();

            wpnFilterPistols.MouseDown += (sender, e) => WeaponCardClicked(sender, e);
            wpnFilterAll.MouseDown += (sender, e) => WeaponCardClicked(sender, e);
            wpnFilterOther.MouseDown += (sender, e) => WeaponCardClicked(sender, e);

            this.Loaded += (sender, e) =>
            {
                IDM.LoadDictionary();
                IDM.ImageRetrived += (type, file) => {
                    fileLoadingName.Dispatcher.Invoke(() => fileLoadingName.Text = file);
                };

                Task.Run(async () =>
                {
                    await this.Load();
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().ToList();
                    await this.LoadWeapons();
                    IDM.SetDictionary();

                    Dispatcher.Invoke(() =>
                    {
                        NormalGrid.Visibility = Visibility.Visible;
                        LoadingGrid.Visibility = Visibility.Collapsed;
                    });
                });    
            };
        }

        public void ChangeLoadoutFilter(object sender, RoutedEventArgs e)
        {
            var tag = ((FrameworkElement)sender)?.Tag;

            if (tag == null)
                return;

            var myTeam = Oyuncular.MatchTeams.GetAllPlayers().First(x => x.Subject == GameAuth.subject).TeamID;
            switch (tag.ToString())
            {
                case "team":
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().Where(x => x.TeamID == myTeam).ToList();
                    break;
                case "nonteam":
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().Where(x => x.TeamID != myTeam).ToList();
                    break;
                case "all":
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().ToList();
                    break;
                case "me":
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().Where(x => x.Subject == GameAuth.subject).ToList();
                    break;

                default:
                    SelectedPlayers = Oyuncular.MatchTeams.GetAllPlayers().ToList();
                    break;
            }

            Task.Run(async () => await this.LoadWeapons());
        }

        private class WeaponClickResponce
        {
            public string WeaponId { get; set; }
            public string SkinName { get; set; }
            public string AgentName { get; set; }
            public string PlayerName { get; set; }
            public string PlayerTag { get; set; }
            public string PlayerID { get; set; }
        }

        public async Task LoadWeapons()
        {
            var wList = new List<(int, WeaponCardButton)>();

            foreach (var player in SelectedPlayers)
            {
                if (player.Loadout?.PlayerLoadout == null)
                    continue;

                //if (player.PlayerIdentity.Incognito)
                //    continue;

                foreach (var l in player.Loadout.PlayerLoadout)
                {
                    if (WeaponFilter != null && WeaponFilter.Count > 0)
                        if (!WeaponFilter.Any(x => x == l.ID.ToString()))
                        { continue; }

                    string name, swatch, img, tier;
                    var s = GAME_CONTENT.WeaponSkins.FirstOrDefault(j => j.uuid.ToLower() == l.Sockets[Sockets["skin"]].Item.ID.ToLower());

                    name = s.displayName;
                    tier = s.contentTierUuid;

                    if (string.IsNullOrEmpty(tier) || string.IsNullOrEmpty(name))
                        continue;

                    var chroma = s.chromas.FirstOrDefault(x => x.uuid == l.Sockets[Sockets["skin_chroma"]].Item.ID);
                    swatch = chroma.swatch;
                    img = chroma.displayIcon;

                    var ct = ContentTiers[tier];
                    var skinImg = await IDM.GetImageAsync(img);

                    if (skinImg == null)
                        continue;

                    var agent = GAME_CONTENT.Agents.FirstOrDefault(j => j.uuid.CompareEquality(player.CharacterID == string.Empty || player.CharacterID == null ? "320b2a48-4d9b-a075-30f1-1f93a9b638fa" : player.CharacterID));

                    await Dispatcher.Invoke( async () =>
                    {
                        var wpn = new WeaponCardButton()
                        {
                            TierIcon = await IDM.GetImageAsync(ct.ImageURL),
                            WeaponIcon = skinImg,
                            WeaponName = name,
                            ChromaIcon = await IDM.GetImageAsync(swatch),
                            Tag = player.PlayerTeam == Oyuncular.EPlayerTeam.Blue ? new WeaponClickResponce()
                            {
                                WeaponId = l.ID,
                                SkinName = name,
                                AgentName = agent?.displayName,
                                PlayerName = player.Username.GameName,
                                PlayerTag = player.Username.TagLine,
                                PlayerID = player.Subject
                            } : null,
                            Visibility = Visibility.Visible,

                            AgentCardColor = new SolidColorBrush(player.Subject == GameAuth.subject ? Color.FromRgb(239, 191, 108) : player.PlayerTeam == Oyuncular.EPlayerTeam.Blue ?  Color.FromRgb(22, 229, 180) : Color.FromRgb(250, 69, 85)),
                            AgentIcon = await IDM.GetImageAsync(agent?.displayIcon)
                        };

                        wpn.MouseDoubleClick += (sender, e) =>
                        {
                            if (sender == null)
                                return;

                            if (sender is FrameworkElement elem)
                            {
                                if (elem.Tag == null || !(elem.Tag is WeaponClickResponce))
                                    return;

                                var response = (WeaponClickResponce)elem.Tag;

                                if (GameChats == null && GameChats.Count != 2)
                                    return;

                                GameChat.Conversation teamChat = null;
                                GameChats.TryGetValue(GameChat.ChatChannel.Team, out teamChat);

                               if (teamChat != null && UserSettings.PlayerMessages.TryGetValue("deseler_want_weapon_skin", out string _msg))
                                {
                                    var msg = TextSchema.GetMessageTest(_msg, new Dictionary<string, string>()
                                    {
                                        { "agent", response.AgentName },
                                        { "skin", response.SkinName },
                                        { "puuid", response.PlayerID },
                                        { "player", response.PlayerName },
                                        { "playertag", response.PlayerTag }
                                    });
                                    GameChat.SendMessage(GameAuth, (teamChat ?? GameChats.Values.ElementAt(0)).cid, msg, GameChat.ChatRoom.Group);
                                }
                            }
                        };

                        wpn.CardBorderColor = ct.Brushes[0];
                        wpn.CardBackColor = ct.Brushes[1];

                        wList.Add((ct.Order, wpn));
                    });
                }
            }

            Dispatcher.Invoke(() =>
            {
                weaponSkins.Children.Clear();
                foreach (var entry in wList.OrderByDescending(x => x.Item1))
                {
                    weaponSkins.Children.Add(entry.Item2);
                }
            });
        }


        public async void PlayerCardClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is PlayerCardSmall))
                return;

            var control = ((PlayerCardSmall)sender);

            if (control.Tag is GameMatch.ActiveMatch.MatchDetail.Player)
                SelectedPlayers = new List<GameMatch.ActiveMatch.MatchDetail.Player>() { (GameMatch.ActiveMatch.MatchDetail.Player)control.Tag };

            await LoadWeapons();
        }

        public async void WeaponCardClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is Desenler_WeaponCard || (new string[] { "pistols", "all", "other" }).Contains(((FrameworkElement)sender).Tag.ToString() )))
                return;

            var control = sender is Desenler_WeaponCard ? ((Desenler_WeaponCard)sender) : (FrameworkElement)sender;

            if (WeaponFilter == null)
                WeaponFilter = new List<string>();

            switch (control.Tag.ToString())
            {
                case "all":
                    WeaponFilter.Clear();
                    foreach (var element in weapons.Children)
                        if (element is Desenler_WeaponCard)
                            ((Desenler_WeaponCard)element).SetActiveState(false);
                    break;

                case "pistols":
                    WeaponFilter.Clear();
                    WeaponFilter.AddRange(Pistols);
                    foreach (var element in weapons.Children)
                        if (element is Desenler_WeaponCard)
                            if (Pistols.Contains(((Desenler_WeaponCard)element).Tag.ToString()))
                            {
                                ((Desenler_WeaponCard)element).SetActiveState(true);
                            }
                            else
                            {
                                ((Desenler_WeaponCard)element).SetActiveState(false);
                            }
                    break;

                case "other":
                    WeaponFilter.Clear();
                    WeaponFilter.AddRange(OtherWeapons);
                    foreach (var element in weapons.Children)
                        if (element is Desenler_WeaponCard)
                            if (OtherWeapons.Contains(((Desenler_WeaponCard)element).Tag.ToString()))
                            {
                                ((Desenler_WeaponCard)element).SetActiveState(true);
                            }
                            else
                            {
                                ((Desenler_WeaponCard)element).SetActiveState(false);
                            }
                    break;

                default:
                    if (WeaponFilter.Any(x => x == control.Tag.ToString()))
                    {
                        WeaponFilter.Remove(control.Tag.ToString());
                        ((Desenler_WeaponCard)control).SetActiveState(false);
                    }
                    else
                    {
                        WeaponFilter.Add(control.Tag.ToString());
                        ((Desenler_WeaponCard)control).SetActiveState(true);
                    }
                    break;
            }

            await LoadWeapons();
        }

        public async Task Load()
        {
            try
            {
                await dBox.Dispatcher.Invoke(async () =>
                {
                    dBox.Children.Clear();

                    foreach (var player in Oyuncular.MatchTeams.GetAllPlayers().OrderByDescending(x => x.PlayerTeam == Oyuncular.EPlayerTeam.Blue))
                    {
                        var agent = GAME_CONTENT.Agents.FirstOrDefault(j => j.uuid.CompareEquality(player.CharacterID == string.Empty || player.CharacterID == null ? "320b2a48-4d9b-a075-30f1-1f93a9b638fa" : player.CharacterID));
                        var pc = new PlayerCardSmall();

                        pc.AgentImage = await IDM.GetImageAsync(agent?.displayIcon);
                        pc.Background = new SolidColorBrush(player.Subject == GameAuth.subject ? Color.FromRgb(239, 191, 108) : player.PlayerTeam == Oyuncular.EPlayerTeam.Blue ? Color.FromRgb(22, 229, 180) : Color.FromRgb(250, 69, 85));

                        pc.Tag = player;
                        pc.Click += PlayerCardClicked;
                        dBox.Children.Add(pc);
                    }
                });

                await weapons.Dispatcher.Invoke(async () =>
                {
                    weapons.Children.Clear();

                    foreach (var weapon in GAME_CONTENT.Weapons)
                    {
                        var card = new Desenler_WeaponCard()
                        {
                            WeaponIcon = (await IDM.GetImageAsync(weapon.displayIcon)).InvertColors(),
                            Content = weapon.displayName,
                            Tag = weapon.uuid
                        };
                        card.Click += WeaponCardClicked;
                        weapons.Children.Add(card);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error occurred during PageLoad: {ex.Message}");
            }
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeLoadoutFilter(sender, null);
        }
    }
}
