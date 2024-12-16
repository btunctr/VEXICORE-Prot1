using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prot1;
using Prot1.Forms;
using Prot1.Valorant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using Valorant.API;
using Valorant.API.External;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Path = System.IO.Path;

    public static class Globals
    {
        public static Dictionary<GameChat.ChatChannel, GameChat.Conversation> GameChats;
        public static Settings UserSettings;
        public static string ActivePage = "ana_menu";
        public static GameContent GAME_CONTENT;
        public static ImageDownloadManager IDM = new ImageDownloadManager(string.Empty);
        public static ValorantEventListener VEL;

        public static Auth GameAuth;
        public static Vexi VAuth;


        public static string[] Pistols = { "42da8ccc-40d5-affc-beec-15aa47b42eda", "e336c6b8-418d-9340-d77f-7a9e4cfe0702", "1baa85b4-4c70-1284-64bb-6481dfc3bb4e", "44d4e95c-4157-0037-81b2-17841bf2e8e3", "29a0cfab-485b-f5d5-779a-b59f85e204a8" };
        public static string[] OtherWeapons = { "2f59173c-4bed-b6c3-2191-dea9b58be9c7", "f7e1b454-4ad4-1063-ec0a-159e56b58941", "462080d1-4035-2937-7c09-27aa2a5c27a7", "c4883e50-4494-202c-3ec3-6b8a9284f00b", "4ade7faa-4cf1-8376-95ef-39884480959b", "a03b24d3-4319-996d-0f8c-94bbfba1dfc7", "910be174-449b-c412-ab22-d0873436b21b", "ec845bf4-4f79-ddda-a3da-0db3774b2794", "ae3de142-4d85-2547-dd26-4e90bed35cf7", "ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a", "9c82e19d-4575-0200-1a81-3eacf00cf872", "63e6c2b6-4a8e-869c-3d4c-e38355226584", "55d8a0f4-4274-ca67-fe2c-06ab45efdf58" };

        public static Dictionary<string, string> errors = new Dictionary<string, string>
        {
            { "game_not_open", "VEXICORE oyun kapalı iken çalışmaz!" }
        };

        public static string OpenPage;
        public static object OpenPageInstance;
        public static Frame PageFrame;

        public static readonly Dictionary<string, string> TagNames = new Dictionary<string, string>()
        {
            { "skin", "Desen" },
            { "weapon", "Silah" },
            { "agent", "Ajan" },
            { "player", "Oyuncu" },
            { "playertag", "Oyuncu Etiketi" },
            { "teamid", "Takım Rengi" },
            { "puuid", "Oyuncu ID" }
        };

        public static readonly Dictionary<string, string> Gamemodes = new Dictionary<string, string>
        {
            { "newmap", "New Map" },
            { "competitive", "Competitive" },
            { "unrated", "Unrated" },
            { "swiftplay", "Swiftplay" },
            { "spikerush", "Spike Rush" },
            { "deathmatch", "Deathmatch" },
            { "ggteam", "Escalation" },
            { "onefa", "Replication" },
            { "custom", "Custom" },
            { "snowball", "Snowball Fight" },
            { "", "Custom" }
        };

        public static readonly List<string> BeforeAscendantSeasons = new List<string>
        {
            "0df5adb9-4dcb-6899-1306-3e9860661dd3",
            "3f61c772-4560-cd3f-5d3f-a7ab5abda6b3",
            "0530b9c4-4980-f2ee-df5d-09864cd00542",
            "46ea6166-4573-1128-9cea-60a15640059b",
            "fcf2c8f4-4324-e50b-2e23-718e4a3ab046",
            "97b6e739-44cc-ffa7-49ad-398ba502ceb0",
            "ab57ef51-4e59-da91-cc8d-51a5a2b9b8ff",
            "52e9749a-429b-7060-99fe-4595426a0cf7",
            "71c81c67-4fae-ceb1-844c-aab2bb8710fa",
            "2a27e5d2-4d30-c9e2-b15a-93b8909a442c",
            "4cb622e1-4244-6da3-7276-8daaf1c01be2",
            "a16955a5-4ad0-f761-5e9e-389df1c892fb",
            "97b39124-46ce-8b55-8fd1-7cbf7ffe173f",
            "573f53ac-41a5-3a7d-d9ce-d6a6298e5704",
            "d929bc38-4ab6-7da4-94f0-ee84f8ac141e",
            "3e47230a-463c-a301-eb7d-67bb60357d4f",
             "808202d6-4f2b-a8ff-1feb-b3a0590ad79f",
        };

        public static readonly Dictionary<string, string> Sockets = new Dictionary<string, string>
        {
            { "skin", "bcef87d6-209b-46c6-8b19-fbe40bd95abc" },
            { "skin_level", "e7c63390-eda7-46e0-bb7a-a6abdacd2433" },
            { "skin_chroma", "3ad1b2b2-acdb-4524-852f-954a76ddae0a" },
            { "skin_buddy", "77258665-71d1-4623-bc72-44db9bd5b3b3" },
            { "skin_buddy_level", "dd3bf334-87f3-40bd-b043-682a57a8dc3a" }
        };

        public static readonly Dictionary<string, ContentTier> ContentTiers = new Dictionary<string, ContentTier>
            {
                { "0cebb8be-46d7-c12a-d306-e9907bfc5a25", new ContentTier()
                    {
                        Brushes = new Brush[] { new SolidColorBrush(Color.FromRgb(0, 158, 129)), new SolidColorBrush(Color.FromRgb(5, 125, 107)) },
                        ImageURL = "https://media.valorant-api.com/contenttiers/0cebb8be-46d7-c12a-d306-e9907bfc5a25/displayicon.png",
                        Order = 1
                    }
                },
                { "e046854e-406c-37f4-6607-19a9ba8426fc", new ContentTier()
                    {
                        Brushes = new Brush[] { new SolidColorBrush(Color.FromRgb(244, 149, 90)), new SolidColorBrush(Color.FromRgb(187,118,77)) },
                        ImageURL = "https://media.valorant-api.com/contenttiers/e046854e-406c-37f4-6607-19a9ba8426fc/displayicon.png",
                        Order = 3
                    }
                },
                { "60bca009-4182-7998-dee7-b8a2558dc369", new ContentTier()
                    {
                        Brushes = new Brush[] { new SolidColorBrush(Color.FromRgb(209, 84, 140)), new SolidColorBrush(Color.FromRgb(159, 69, 115)) },
                        ImageURL = "https://media.valorant-api.com/contenttiers/60bca009-4182-7998-dee7-b8a2558dc369/displayicon.png",
                    Order = 2
                    }
                },
                { "12683d76-48d7-84a3-4e09-6985794f0445", new ContentTier()
                    {
                        Brushes = new Brush[] { new SolidColorBrush(Color.FromRgb(90, 159, 225)), new SolidColorBrush(Color.FromRgb(71, 125, 178)) },
                        ImageURL = "https://media.valorant-api.com/contenttiers/12683d76-48d7-84a3-4e09-6985794f0445/displayicon.png",
                        Order = 0
                    }
                },
                { "411e4a55-4e59-7757-41f0-86a53f101bb5", new ContentTier()
                    {
                        Brushes = new Brush[] { new SolidColorBrush(Color.FromRgb(249, 214, 99)), new SolidColorBrush(Color.FromRgb(192, 167, 82)) },
                        ImageURL = "https://media.valorant-api.com/contenttiers/411e4a55-4e59-7757-41f0-86a53f101bb5/displayicon.png",
                        Order = 4
                    }
                }
            };

        public class ContentTier
        {
            public string ImageURL;
            public Brush[] Brushes;
            public int Order;
        }

    public static class Colors
    {
        public static SolidColorBrush ForeBlue = new SolidColorBrush(Color.FromRgb(22, 229, 180));
        public static SolidColorBrush ForeRed = new SolidColorBrush(Color.FromRgb(255, 70, 85));

        public static LinearGradientBrush _ANA_MENU_MENU_BUTTON_ACIVE = (new LinearGradientBrush()
        {
            StartPoint = new System.Windows.Point(0, 0),
            EndPoint = new System.Windows.Point(1, 1),
            GradientStops = new GradientStopCollection()
                                {
                                    new GradientStop(System.Windows.Media.Color.FromRgb(39, 39, 39), 0),
                                    new GradientStop(System.Windows.Media.Color.FromRgb(23, 23, 23), 1)
                                }
        });

        public static LinearGradientBrush _ANA_MENU_MENU_BUTTON_DEACTIVE = (new LinearGradientBrush()
        {
            StartPoint = new System.Windows.Point(0, 0),
            EndPoint = new System.Windows.Point(1, 1),
            GradientStops = new GradientStopCollection()
                                {
                                    new GradientStop(System.Windows.Media.Color.FromRgb(176, 0, 32), 0),
                                    new GradientStop(System.Windows.Media.Color.FromArgb(0, 34, 34, 34), .3),
                                    new GradientStop(System.Windows.Media.Color.FromArgb(0, 34, 34, 34), .7),
                                    new GradientStop(System.Windows.Media.Color.FromRgb(176, 0, 32), 1)
                                }
        });

        public static LinearGradientBrush _OYUNCULAR_TEAM_BANNER_BLUE = new LinearGradientBrush()
        {
            StartPoint = new System.Windows.Point(0, 0),
            EndPoint = new System.Windows.Point(1, 0),
            GradientStops = new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(33, 131, 112), 0),
                    new GradientStop(Color.FromRgb(0, 87, 76), 1)
                }
        };

        public static LinearGradientBrush _OYUNCULAR_TEAM_BANNER_RED = new LinearGradientBrush()
        {
            StartPoint = new System.Windows.Point(.3, 0),
            EndPoint = new System.Windows.Point(.6, 0),
            GradientStops = new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(76, 29, 41), 0),
                    new GradientStop(Color.FromRgb(76, 29, 38), 1)
                }
        };
    }


}