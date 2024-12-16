using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Windows.Media.Media3D;
using System.Reflection;
using System.Net.Http;
using System.Threading;
using static Globals;
using Prot1.Forms;
using System.Media;
using Valorant.API;
using System.Web.UI.WebControls;
using Valorant.API.External;
using Prot1.Valorant;
using System.Diagnostics;
using System.Web.WebSockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Valorant;
using Prot1.UserControls;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace Prot1
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public MainWindow()
        {
            Globals.GameAuth = Auth.GetAuthLocal(true);
            if (Globals.GameAuth == null)
            {
                MessageBox.Show("VEXICORE oyun kapalı iken çalışmaz!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            GAME_CONTENT = new GameContent();
            GAME_CONTENT.LoadContents();

            InitializeComponent();

            Instance = this;

            this.Loaded += (sender, e) =>
            {
                UserSettings = Settings.GetSettings();

                VEL = new ValorantEventListener();

                VEL.GameStateChanged += OnGameStateChange;
                VEL.MessageRecived += OnMessageRecived;

                VEL.StartListening();

                PageFrame = mainFrame;

                //goldText.Text = VAuth.UserAccountData.ActivePlan == null ? "GOLD" : VAuth.UserAccountData.ActivePlan.RemainingTime.ToDateStringHour().ToUpper();
            };
        }

        private List<ChatMessageTrigger> ChatTriggers = new List<ChatMessageTrigger>() 
        { 
            new ChatMessageTrigger() { Triggers = new string[] { "!saat" }, Responses=new string[] {"Saat: {%time%}","Test"}, Type=ChatMessageTrigger.TriggerType.Regex, MatchState=ChatMessageTrigger._MatchState.All } ,
            new ChatMessageTrigger() { Triggers = new string[] { "!rank", "!derece" }, Responses=new string[]{ "Rank: {%rank%}, Ortalama: {%avgrank%}, Max: {%peakrank%}" } } ,
            new ChatMessageTrigger() { Triggers = new string[] { "!bilgi", "!info" }, Responses=new string[]{"[{%date%}] Oynadığınız Karakter: {%agent%}, Bölge: {%region%}, Seviye: {%level%}" } }
        };

        public async Task OnMessageRecived(ValorantEventListener.IncomingChatMessage msg)
        {
            try
            {
                foreach (ChatMessageTrigger trigger in ChatTriggers)
                {
                    if (trigger.CheckMessage(msg, VEL.CurrentGameState.ToUpper(), out GameMatch.ActiveMatch.MatchDetail.Player player))
                    {
                        trigger.SendResponse(msg, player);
                    }
                }

                await Task.Delay(150);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task OnGameStateChange(string oldState, string newState)
        {
            try
            {
                Debug.WriteLine("Game state changed from: " + oldState.ToUpper() + " to: " + newState.ToUpper());

                if (newState.ToUpper() == "INGAME")
                {
                    await Oyuncular.LoadPlayerInformations();
                }
                else
                {
                    mainFrame.Dispatcher.Invoke(() =>
                    {
                        switch (ActivePage)
                        {
                            case "skins":
                            case "players":
                            case "partys":
                                ChangeSubPage("home");
                                break;
                        }
                    });
                }

                if (newState.ToUpper() == "MENUS")
                {
                    
                }

                if (newState.ToUpper() == "PREGAME")
                {
                    
                }

                VEL.GameStateChanged += AutomaticMessage.OnGameStateChange;
                await Task.Run(async () => await AutomaticMessage.OnGameStateChange(oldState, newState));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occured on game state change event : " + ex.Message + "\nST:" + ex.StackTrace);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           try { this.DragMove(); } catch { }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavButton selected = sidebar.SelectedItem as NavButton;
            Debug.WriteLine("Clicked nav_button {" + selected.NavTarget + "}");
            ChangeSubPage(selected.NavTarget);
        }

        private void ChangeSubPage(string PageShort)
        {
            if (PageShort == null || !(PageShort is string))
                return;

            switch (PageShort.ToString())
            {
                case "home":
                    mainFrame.Content = null;
                    ActivePage = "home";
                    return;

                case "players":
                    if (Forms.Oyuncular.IsDataLoaded && !Forms.Oyuncular.IsLoadingData)
                    {
                        mainFrame.Content = new Oyuncular();
                        ActivePage = PageShort.ToString();
                    }
                    break;
                case "skins":
                    if (Forms.Oyuncular.IsDataLoaded && !Forms.Oyuncular.IsLoadingData)
                    {
                        mainFrame.Content = new Desenler();
                        ActivePage = PageShort.ToString();
                    }
                    break;
                case "skinstore":
                    mainFrame.Content = new DesenMarketi();
                    ActivePage = PageShort.ToString();
                    break;
                case "settings":
                    mainFrame.Content = new Prot1.Forms.Ayarlar();
                    ActivePage = PageShort.ToString();
                    break;
                case "chatCommands":
                    mainFrame.Content = new ChatKomutları();
                    ActivePage = PageShort.ToString();
                    break;
            }
        }
    }

    public class NavButton : ListBoxItem
    {
        static NavButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
        }

        public string NavTarget
        {
            get { return (string)GetValue(NavlinkProperty); }
            set { SetValue(NavlinkProperty, value); }
        }
        public static readonly DependencyProperty NavlinkProperty = DependencyProperty.Register("NavTarget", typeof(string), typeof(NavButton), new PropertyMetadata(string.Empty));

        public string PageName
        {
            get { return (string)GetValue(PageNameProperty); }
            set { SetValue(PageNameProperty, value); }
        }
        public static readonly DependencyProperty PageNameProperty = DependencyProperty.Register("PageName", typeof(string), typeof(NavButton), new PropertyMetadata(string.Empty));


        public BitmapImage Icon
        {
            get { return (BitmapImage)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(BitmapImage), typeof(NavButton), new PropertyMetadata(null));

        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register("TextBrush", typeof(Brush), typeof(NavButton), new PropertyMetadata(new SolidColorBrush(Colors.White)));

    public Brush HoverColor
        {
            get { return (Brush)GetValue(HoverColorProperty); }
            set { SetValue(HoverColorProperty, value); }
        }
        public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register("HoverColor", typeof(Brush), typeof(NavButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(2, 112, 215))));
    }
}
