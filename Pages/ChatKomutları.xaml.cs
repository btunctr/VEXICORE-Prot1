using Prot1.UserControls;
using Prot1.Valorant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Valorant;
using Valorant.API;
using static Globals;

namespace Prot1.Forms
{
    public class AutomaticMessage
    {
        private int MIndex = 0;

        public string[] Messages;    
        public MessageSendMethod Method;
        public GameChat.ChatChannel MsgChannel;

        public int RepeatingMessageDuration;
        public int InitialMessageDuration;

        public EventTrigger Trigger;

        [Flags]
        public enum EventTrigger
        {
            None = 0,
            OnMenuLoaded = 2,
            OnPreGameLoaded = 4,
            OnGameLoaded = 8
        }

        public enum MessageSendMethod
        {
            RandomSelected,
            Ordered
        }
        // TAGLARI EKLE
        public void SendMessage()
        {
            if (MsgChannel == GameChat.ChatChannel.Undefined)
                return;

            if (Messages.Length == 0)
                return;

            bool IsSingleMessage = Messages.Length == 1;
            GameChat.Conversation Channel;

            if (Trigger == EventTrigger.None)
                return;

            if (Trigger.HasFlag(EventTrigger.OnPreGameLoaded) || Trigger.HasFlag(EventTrigger.OnMenuLoaded)) {
                return;
            }

            if (!Globals.GameChats.TryGetValue(MsgChannel, out Channel))
                return;

            string message = string.Empty;

            if (Method == MessageSendMethod.RandomSelected)
                message = IsSingleMessage ? Messages[0] : Messages[new Random(DateTime.Now.Millisecond).Next(0, Messages.Length)];
            else if (Method == MessageSendMethod.Ordered)
            {
                if (MIndex > Messages.Length - 1)
                    MIndex = 0;

                message = Messages[MIndex++];
            }

            if (message != string.Empty)
                GameChat.SendMessage(Globals.GameAuth, Channel.cid, message, GameChat.ChatRoom.Group);
        }

        public static List<AutomaticMessage> AutomaticMessages = new List<AutomaticMessage>()
        {
            //new AutomaticMessage()
            //{
            //    Messages = new string[] {"Test", "Test1", "Test2"},
            //    Method = MessageSendMethod.RandomSelected,
            //    MsgChannel = GameChat.ChatChannel.Team,
            //    Trigger = EventTrigger.OnGameLoaded
            //}
        };

        public static void CheckMessagesAsync()
        {

        }

        private static bool IsFirstStatus = false;
        public static async Task OnGameStateChange(string oldState, string newState)
        {
            if (IsFirstStatus)
            {
                IsFirstStatus = false;
                return;
            }

            try
            {
                foreach (var msg in AutomaticMessages)
                {
                    if (msg.Trigger == EventTrigger.None)
                        continue;

                    if ((msg.Trigger.HasFlag(EventTrigger.OnMenuLoaded) && newState.ToUpper() == "MENUS")
                        || (msg.Trigger.HasFlag(EventTrigger.OnPreGameLoaded) && newState.ToUpper() == "PREGAME")
                        || (msg.Trigger.HasFlag(EventTrigger.OnGameLoaded) && newState.ToUpper() == "INGAME"))
                    {
                        await Task.Delay(150);
                        msg.SendMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occured on game state change event (AutomaticMessage): " + ex.Message + "\nST:" + ex.StackTrace);
            } finally
            {
                await Task.CompletedTask;
            }
        }
    }

    public class ChatMessageTrigger
    {
        public static ChatMessageTrigger[] NonVIPTriggers = new ChatMessageTrigger[] { };
        
        [JsonIgnore]
        public Guid _g;

        public bool AllowEnemyTeam;
        public bool AllowAllieTeam;

        public bool RespondeOnError;

        public string[] Triggers;
        public TriggerType Type;
        public _MatchState MatchState;
        public MessageChannel Channel;


        public string[] Responses;
        public TriggerFunctionality Functionality;

        /// <summary>
        /// Set to null for no agent filter
        /// </summary>
        public string[] AgentFilter; // V

        public ChatMessageTrigger()
        {
            AllowEnemyTeam = true;
            AllowAllieTeam = true;

            Type = TriggerType.Standart;
            MatchState = _MatchState.InGame;
            Channel = MessageChannel.GroupChat;
            Responses = Array.Empty<string>();
            Functionality = TriggerFunctionality.None;
            AgentFilter = null;
        }

        [Flags]
        public enum TriggerFunctionality
        {
            None = 0,
            InviteToParty = 2,
            SendFriendRequest = 4,
            RequestToJoinParty = 8,
            EnterMatchmakingQueue = 16
        }

        public bool CheckMessage(ValorantEventListener.IncomingChatMessage msg, string matchState, out GameMatch.ActiveMatch.MatchDetail.Player OutSender)
        {
            GameMatch.ActiveMatch.MatchDetail.Player Sender = OutSender = null;

            if (MatchState == _MatchState.InGame && (!Oyuncular.IsDataLoaded || Oyuncular.IsLoadingData))
                return false;

            if (!CheckForTriggerHit(msg))
                return false;

            if (!ChannelCheck(msg))
                return false;

            if (!MatchStateCheck(matchState))
                return false;

            if (MatchState == _MatchState.InGame)
            {
                if (Oyuncular.AllPlayerIndexes.TryGetValue(msg.puuid.ToLower(), out int index))
                {
                    var allPlayers = Oyuncular.MatchTeams.GetAllPlayers().ToArray();

                    if (index > allPlayers.Length - 1)
                        return false;

                    Sender = allPlayers[index];

                    if (Sender.PlayerTeam == Oyuncular.EPlayerTeam.Blue)
                    {
                        if (!AllowAllieTeam)
                        {
                            return false;
                        }
                    }
                    else if (Sender.PlayerTeam == Oyuncular.EPlayerTeam.Red)
                    {
                        if (!AllowEnemyTeam)
                        {
                            return false;
                        }
                    }

                    if (AgentFilter != null)
                    {
                        if (!AgentFilter.Any(x => Sender.CharacterID.CompareEquality(x)))
                            return false;
                    }
                }
            }

            OutSender = Sender;
            return true;
        }

        public void SendResponse(ValorantEventListener.IncomingChatMessage msg, GameMatch.ActiveMatch.MatchDetail.Player player)
        {
            if (Responses == null)
                return;

            if (Responses.Length == 0)
                return;

            string Response = Responses[new Random(DateTime.Now.Millisecond).Next(0, Responses.Length == 1 ? 0 : Responses.Length)];

            bool IsAnyTags = Regex.IsMatch(Response, TagEditor.pattern);

            var d = new Dictionary<string, string>()
            {
                { "region", msg.region },
                { "date", DateTime.Now.ToString("dd/MM/yyyy") },
                { "time", DateTime.Now.ToString("HH:mm") },
            };

            if (IsAnyTags)
            {
                d["player"] = msg.game_name;
                d["playertag"] = msg.game_tag;

                if (player != null)
                {
                     if (player.PlayerIdentity != null)
                    {
                        d["team"] = player.PlayerTeam == Oyuncular.EPlayerTeam.Blue ? "Mavi" : "Kırmızı";

                        if (!player.PlayerIdentity.HideAccountLevel)
                            d["level"] = player.PlayerIdentity.AccountLevel.ToString();

                        if (player.Rank != null)
                        {
                            d["rank"] = player.Rank.Rank.tierName;

                            if (!player.PlayerIdentity.Incognito)
                            {
                                d["peakrank"] = player.Rank.PeakRank.tierName;
                                d["avgrank"] = player.Rank.AvgRank.tierName;
                            }               
                        }
                    }

                    if (player.CharacterID != string.Empty)
                    {
                        var agent = Globals.GAME_CONTENT.Agents.FirstOrDefault(x => x.uuid.CompareEquality(player.CharacterID));
                        if (agent != null)
                        {
                            d["agent"] = agent.displayName;
                        }
                    }
                }

            }

            GameChat.SendMessage(Globals.GameAuth, msg.cid, IsAnyTags ? TextSchema.GetMessageTest(Response, d) : Response, msg.type == "groupchat" ? GameChat.ChatRoom.Group : msg.type == "chat" ? GameChat.ChatRoom.Private : GameChat.ChatRoom.System);
        }

        public void DoTriggerActions()
        {

        }

        private bool CheckForTriggerHit(ValorantEventListener.IncomingChatMessage msg)
        {
            switch (Type)
            {
                case TriggerType.Standart:
                    return Triggers.Any(x => x.CompareEquality(msg.body));
                case TriggerType.Containment:
                    return Triggers.Any(x => msg.body.ToLower().Contains(x.ToLower()));
                case TriggerType.Regex:
                    return Triggers.Any(pattern => Regex.IsMatch(msg.body, pattern));
            }

            return false;
        }

        private bool ChannelCheck(ValorantEventListener.IncomingChatMessage msg)
        {
            if (Channel == MessageChannel.All)
                return !msg.type.CompareEquality("system");

            string compareChannelName = Channel == MessageChannel.GroupChat ? "groupchat" : "chat";
            return msg.type.CompareEquality(compareChannelName);
        }

        private bool MatchStateCheck(string matchState) => MatchState == _MatchState.All ? true : matchState.CompareEquality(MatchState == _MatchState.InGame ? "INGAME" : MatchState == _MatchState.PreGame ? "PREGAME" : "MENUS");

        public enum MessageChannel
        {
            GroupChat,
            DirectMessage,
            All
        }

        public enum _MatchState
        {
            InGame,
            PreGame,
            InMenu,
            All
        }

        public enum TriggerType
        {
            Standart,
            Containment,
            Regex
        }
    }

    public partial class ChatKomutları : Page
    {
        public ChatKomutları()
        {
            InitializeComponent();

            this.Loaded += (a, b) =>
            {
                if (UserSettings == null)
                    UserSettings = Settings.GetSettings();

                if (UserSettings.ChatTriggers == null)
                    UserSettings.ChatTriggers = new List<ChatMessageTrigger>();
            };
        }

        private void CheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
                if (cb.Parent != null && ((FrameworkElement)cb.Parent).Parent != null && ((FrameworkElement)cb.Parent).Parent is Border b)
                {
                    b.BorderBrush = cb.IsChecked.HasValue && cb.IsChecked.Value ?  new SolidColorBrush(Color.FromArgb(255, 2, 112, 215)) : new SolidColorBrush(Color.FromArgb(255, 138, 148, 167));
                    b.Background = cb.IsChecked.HasValue && cb.IsChecked.Value ?  new SolidColorBrush(Color.FromArgb(51, 2, 112, 215)) : new SolidColorBrush(Color.FromArgb(0,0,0,0));
                }
        }
    }
}
