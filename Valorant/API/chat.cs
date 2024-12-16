using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prot1.Valorant;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valorant.API;

namespace Valorant.API
{
    public class GameChat
    {
        public class Conversation
        {
            public string cid { get; set; }
            public bool direct_messages { get; set; }
            public bool global_readership { get; set; }
            public bool message_history { get; set; }
            public string mid { get; set; }
            public bool muted { get; set; }
            public bool mutedRestriction { get; set; }
            public string type { get; set; }
            public Uistate uiState { get; set; }
            public int unread_count { get; set; }
            public ChatRoom RoomType { get; set; }
            public ChatChannel ChannelType { get; set; } = ChatChannel.Undefined;
        }

        public class Uistate
        {
            public bool changedSinceHidden { get; set; }
            public bool hidden { get; set; }
        }

        public enum ChatRoom
        {
            Group,
            Private,
            System
        }

        public enum ChatChannel
        {
            Undefined,
            Team,
            All
        }

        public class ChatParticipent
        {
            public string cid { get; set; }
            public string game_name { get; set; }
            public string game_tag { get; set; }
            public bool muted { get; set; }
            public string name { get; set; }
            public string pid { get; set; }
            public string puuid { get; set; }
            public string region { get; set; }
        }

        public static Dictionary<ChatChannel, Conversation> GetCurrentGameConversationChannels(Auth au, string[] enemys)
        {
            var response = new Dictionary<ChatChannel, Conversation>();
            var participents = GetChatParticipents(au);
            var chats = GetCurrentGameChats(au);

            if (participents == null || chats == null || enemys == null)
            {
                return null; // Handle the null case appropriately
            }

            foreach (var chat in chats)
            {
                if (chat.ChannelType == ChatChannel.Undefined)
                {
                    var chatParticipents = participents.Where(x => x?.cid.CompareEquality(chat?.cid) == true);

                    if (chatParticipents != null && chatParticipents.All(p => !enemys.Contains(p?.puuid)))
                    {
                        chat.ChannelType = ChatChannel.Team;
                    }
                    else
                    {
                        chat.ChannelType = ChatChannel.All;
                    }
                }

                if (!response.ContainsKey(chat.ChannelType))
                {
                    response.Add(chat.ChannelType, chat);
                }
            }

            return response;
        }


        public static List<ChatParticipent> GetChatParticipents(Auth au, string cid = "")
        {
            string url = $"https://127.0.0.1:{au.LockFile[2]}/chat/v5/participants" + (!string.IsNullOrEmpty(cid) && cid != "" ? "?cid=" + cid : string.Empty);
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{au.LockFile[3]}"))}");

            try
            {
                string content = client.Execute(request).Content;
                return JsonConvert.DeserializeObject<List<ChatParticipent>>(JToken.Parse(content)["participants"].ToString()).ToList(); 
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at chat.cs");
                return null;
            }
        }

        public static List<Conversation> GetCurrentGameChats(Auth au, string[] enemys = null)
        {
            string url = $"https://127.0.0.1:{au.LockFile[2]}/chat/v6/conversations/ares-coregame";
            RestClient client = new RestClient(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{au.LockFile[3]}"))}");

            try
            {
                var ret = JsonConvert.DeserializeObject<Conversation[]>(JToken.Parse(client.Execute(request).Content)["conversations"].ToString()).ToList();

                for (int i = 0; i < ret.Count; ++i)
                {
                    ret[i].RoomType = ret[i].type.CompareEquality("system") ? ChatRoom.System : ret[i].type.CompareEquality("groupchat") ? ChatRoom.Group : ChatRoom.Private;
                }

                return ret;
            }
            catch (JsonException)
            {
                Debug.WriteLine("Error occured while deserilizeing json at chat.cs");
                return null;
            }
        }
        public static void SendMessage(Auth au, string cid, string message, ChatRoom type)
        {
            var at = new
            {
                cid = cid,
                message = message,
                type = type == ChatRoom.Group ? "groupchat" : "chat"
            };

            string url = $"https://127.0.0.1:{au.LockFile[2]}/chat/v6/messages";
            RestClient client = new RestClient(url);
            client.CookieContainer = au.cookies;
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"riot:{au.LockFile[3]}"))}");
            request.AddJsonBody(at);

            var result = client.Execute(request);
            return;
        }
    }
}
