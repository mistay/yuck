using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace yuck
{
    public sealed class Businesslogic
    {
        private static readonly Businesslogic instance = new Businesslogic();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Businesslogic()
        {
        }

        private Businesslogic()
        {
        }

        public static Businesslogic Instance
        {
            get
            {
                return instance;
            }
        }

        public Main MainForm { get; internal set; }

        private static readonly HttpClient client = new HttpClient();
        MatrixLoginResult matrixResult;
        internal string loggedInUserID;
        public Dictionary<string, string> roomCache = new Dictionary<string, string>();


        public static string MatrixUsernameToShortUsername(string username)
        {
            string[] tmp = username.Split(':');
            if (tmp.Length == 2)
                if (tmp[0].Length >= 1)
                    return tmp[0].Substring(1);
                else
                    return username;
            else
                return username;
        }



        public delegate void membersLoadedEvent(MatrixMemberResult matrixMemberResult);
        public event membersLoadedEvent MembersLoaded;
        private void fireMembersLoaded(MatrixMemberResult matrixMemberResult)
        {
            if (MembersLoaded != null)
                MembersLoaded(matrixMemberResult);
        }

        public delegate void joinedRoomyLoaded(MatrixJoinedRoomsResult matrixJoinedRoomsResult);
        public event joinedRoomyLoaded JoinedRoomsLoadedEvent;
        private void fireJoinedRoomyLoadedEvent(MatrixJoinedRoomsResult matrixJoinedRoomsResult)
        {
            if (JoinedRoomsLoadedEvent != null)
                JoinedRoomsLoadedEvent(matrixJoinedRoomsResult);
        }

        internal static Uri MXC2HTTP(string mxc_url)
        {
            if (mxc_url == null)
                return null;

            Uri u = new Uri(mxc_url);

            return new Uri(String.Format("https://{0}/_matrix/media/r0/download/{1}{2}", Properties.Settings.Default.matrixserver_hostname, u.Host, u.AbsolutePath ));
        }

        
        public delegate void Whoami(MatrixWhoamiResult matrixWhoamiResult);
        public event Whoami WhoamiEvent;
        private void fireWhoamiEvent(MatrixWhoamiResult matrixWhoamiResult)
        {
            if (WhoamiEvent != null)
                WhoamiEvent(matrixWhoamiResult);
        }


        public delegate void RoomResolved();
        public event RoomResolved RoomResolvedEvent;
        private void fireRoomResolvedEvent()
        {
            if (RoomResolvedEvent != null)
                RoomResolvedEvent();
        }


        public delegate void LoginCompleted();
        public event LoginCompleted LoginCompletedEvent;
        private void fireLoginCompletedEvent()
        {
            if (LoginCompletedEvent != null)
                LoginCompletedEvent();
        }

        public delegate void RoomsDirectResolved();
        public event RoomsDirectResolved RoomsDirectResolvedEvent;
        private void fireRoomsDirectResolvedEvent()
        {
            if (RoomsDirectResolvedEvent != null)
                RoomsDirectResolvedEvent();
        }

        public delegate void MatrixUploadCompleted(MatrixUploadResult matrixUploadResult);
        public event MatrixUploadCompleted MatrixUploadCompletedEvent;
        private void fireMatrixUploadCompletedEvent(MatrixUploadResult matrixUploadResult)
        {
            if (MatrixUploadCompletedEvent != null)
                MatrixUploadCompletedEvent(matrixUploadResult);
        }


        public delegate void AvatarURLReceived(MatrixAvatarResult matrixAvatarResult);

        internal void BringToFront(string roomID)
        {
            
        }

        public event AvatarURLReceived AvatarURLReceivedEvent;
        private void fireAvatarURLReceivedEvent(MatrixAvatarResult matrixAvatarResult)
        {
            if (AvatarURLReceivedEvent != null)
                AvatarURLReceivedEvent(matrixAvatarResult);
        }

        public delegate void SyncCompleted(MatrixSyncResult matrixSyncResult, bool initSync);
        public event SyncCompleted SyncCompletedEvent;
        private void fireSyncCompletedEvent(MatrixSyncResult matrixSyncResult, bool initSync)
        {
            if (SyncCompletedEvent != null)
                SyncCompletedEvent(matrixSyncResult, initSync);
        }

        public delegate void UserPrecenseReceived(Dictionary<string, string> changed);
        public event UserPrecenseReceived UserPrecenseReceivedEvent;
        private void fireUserPrecenseReceivedEvent(Dictionary<string,string> changed)
        {
            if (UserPrecenseReceivedEvent != null)
                UserPrecenseReceivedEvent(changed);
        }

        public delegate void MessageCompleted(MatrixMessagesResult matrixMessagesResult);
        public event MessageCompleted MessageCompletedEvent;
        private void fireMessageCompletedEvent(MatrixMessagesResult matrixMessagesResult)
        {
            if (MessageCompletedEvent != null)
                MessageCompletedEvent(matrixMessagesResult);
        }

        public delegate void MediadownloadCompleted(MatrixMediaRequest matrixMediaRequest, Image image);
        public event MediadownloadCompleted MediadownloadCompletedEvent;
        private void fireMediadownloadCompletedEvent(MatrixMediaRequest matrixMediaRequest, Image image)
        {
            if (MediadownloadCompletedEvent != null)
                MediadownloadCompletedEvent(matrixMediaRequest, image);
        }

        public delegate void AvatarDownloadCompleted(Image image);
        public event AvatarDownloadCompleted AvatarDownloadCompletedEvent;
        private void fireAvatarDownloadCompletedEvent(Image image)
        {
            if (AvatarDownloadCompletedEvent != null)
                AvatarDownloadCompletedEvent(image);
        }

        public delegate void Typing(string room_id, List<string> user_ids);
        public event Typing TypingEvent;
        private void fireTypingEvent(string room_id, List<string> user_ids)
        {
            if (TypingEvent != null)
                TypingEvent(room_id, user_ids);
        }

        public delegate void UnreadNotifications(string roomID, MatrixSyncUnreadNotifications matrixSyncUnreadNotifications);
        public event UnreadNotifications UnreadNotificationsEvent;
        private void fireUnreadNotificationsEvent(string roomID, MatrixSyncUnreadNotifications matrixSyncUnreadNotifications)
        {
            if (UnreadNotificationsEvent != null)
                UnreadNotificationsEvent(roomID, matrixSyncUnreadNotifications);
        }

        public delegate void MessageRecieved(List<ReceiptEvent> receiptEvent);
        public event MessageRecieved MessageRecievedEvent;
        private void fireMessageRecievedEvent(List<ReceiptEvent> receiptEvent)
        {
            if (MessageRecievedEvent != null)
                MessageRecievedEvent(receiptEvent);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        internal async Task<MatrixLoginResult> UserTyping(string roomID, bool isTyping)
        {
            HttpClient client = new HttpClient();

            string uri = String.Format("https://{0}/_matrix/client/r0/rooms/{1}/typing/{2}?access_token={3}", Properties.Settings.Default.matrixserver_hostname, roomID, HttpUtility.UrlEncode(loggedInUserID), matrixResult.access_token);
            Console.WriteLine("uri: " + uri);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            String template = @"
                  ""typing"": {0},
                  ""timeout"": 1000
                ";
            String jsonLogin = String.Format(template, isTyping ? "true" : "false");
            jsonLogin = "{" + jsonLogin + "}";

            StringContent myStringContent = new StringContent(jsonLogin);
            try
            {
                Console.WriteLine("UserTyping() calling: " + client.BaseAddress + " w/ content: " + jsonLogin);
                HttpResponseMessage response = await client.PutAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("UserTyping() response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {

                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("UserTyping() response from server:" + responseString);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not UserTyping(): " + e.Message);
            }
            return null;
        }

        public async Task<object> resolveRoomname(string roomID)
        {
            HttpClient client = new HttpClient();
            MatrixRoomResolvedResult matrixRoomResolvedResult = null;

            string uri = String.Format("https://{0}/_matrix/client/r0/rooms/{1}/state/m.room.name?access_token={2}", Properties.Settings.Default.matrixserver_hostname, roomID, matrixResult.access_token);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;

                    matrixRoomResolvedResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixRoomResolvedResult>(responseString);
                    roomCache.Add(roomID, matrixRoomResolvedResult.name);

                    fireRoomResolvedEvent();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not resolveRoomnameAwait(): " + e.Message);
            }

            return null;
        }

        public async Task downloadAvatarURLAsync(string user_id)
        {
            await downloadAvatarURLAwait(user_id);
        }
        internal async Task<MatrixAvatarResult> downloadAvatarURLAwait(string user_id)
        {
            HttpClient client = new HttpClient();
            MatrixAvatarResult matrixAvatarResult;

            string uri = String.Format("https://{0}/_matrix/client/r0/profile/{1}/avatar_url", Properties.Settings.Default.matrixserver_hostname,  (user_id));
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;

                    matrixAvatarResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixAvatarResult>(responseString);

                    fireAvatarURLReceivedEvent(matrixAvatarResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not downloadAvatarURLAwait(): " + e.Message);
            }

            return null;
        }

        internal async Task<Image> downloadMedia(MatrixMediaRequest matrixImageRequest,  string mxcurl)
        {
            HttpClient client = new HttpClient();

            Uri uri = MXC2HTTP(mxcurl);

            //string uri = String.Format("https://{0}/_matrix/media/r0/download/st0ne.net/wEmUPhSlPdqDNlBHxlBAHAVx", Properties.Settings.Default.matrixserver_hostname);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {

                    Stream s = response.Content.ReadAsStreamAsync().Result;
                    s.Seek(0, SeekOrigin.Begin);
                    Image i = Image.FromStream(s);

                    appendImageToChatmessages(mxcurl, i);

                    fireMediadownloadCompletedEvent(matrixImageRequest, i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not downloadMediaAwait(): " + e.Message);
            }
            return null;
        }

        private void appendImageToChatmessages(string url, Image i)
        {
            foreach (ChatMessage chatMessage in chatMessages)
            {
                if (chatMessage is ChatMessageImage)
                {
                    ChatMessageImage chatMessageImage = (ChatMessageImage)chatMessage;
                    if (chatMessageImage.Url == url) {
                        // found

                        chatMessage.Image = i;
                        break;
                    }
                }
            }
        }

        internal async Task<Image> downloadAvatar(Uri uri)
        {
            HttpClient client = new HttpClient();
            //string uri = String.Format("https://{0}/_matrix/media/r0/download/st0ne.net/wEmUPhSlPdqDNlBHxlBAHAVx", Properties.Settings.Default.matrixserver_hostname);
            client.BaseAddress = uri;
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {

                    Stream s = response.Content.ReadAsStreamAsync().Result;
                    s.Seek(0, SeekOrigin.Begin);
                    Image i = Image.FromStream(s);

                    fireAvatarDownloadCompletedEvent(i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not downloadMediaAwait(): " + e.Message);
            }
            return null;
        }

        public async Task whoamiAsync()
        {
            await whoamiAwait();
        }
        internal async Task<MatrixWhoamiResult> whoamiAwait()
        {
            HttpClient client = new HttpClient();
            MatrixWhoamiResult matrixWhoamiResult = null;

            string uri = String.Format("https://{0}/_matrix/client/r0/account/whoami?access_token={1}", Properties.Settings.Default.matrixserver_hostname, matrixResult.access_token);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;

                    matrixWhoamiResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixWhoamiResult>(responseString);

                    this.loggedInUserID = matrixWhoamiResult.user_id;

                    fireWhoamiEvent(matrixWhoamiResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not syncAwait(): " + e.Message);
            }

            return null;
        }





        public async Task messagesAsync(string roomID, string from)
        {
            await messagesAwait(roomID, from);
        }
        internal async Task<MatrixMessagesResult> messagesAwait(string roomID, string from)
        {
            HttpClient client = new HttpClient();
            MatrixMessagesResult matrixMessagesResult = null;

            string uri = String.Format("https://{0}/_matrix/client/r0/rooms/{1}/messages?access_token={2}&from={3}&dir=f", Properties.Settings.Default.matrixserver_hostname, roomID, matrixResult.access_token, from);
            //Console.WriteLine("uri:" + uri);

            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                //Console.WriteLine("/messages response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine("response from server:" + responseString);

                    matrixMessagesResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixMessagesResult>(responseString);
                    fireMessageCompletedEvent(matrixMessagesResult);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not messagesAwait(): " + e.Message);
            }

            return null;
        }


        public Dictionary<string, string> presence = new Dictionary<string, string>();

        public Dictionary<string, List<string>> direct = new Dictionary<string, List<string>>();
        public List<ChatMessage> chatMessages = new List<ChatMessage>();

        private string _next_batch = null;
        public Dictionary<string, List<string>> UsersTyping = new Dictionary<string, List<string>>();

        public async Task<MatrixSyncResult> sync()
        {
            bool initSync = _next_batch == null;

            HttpClient client = new HttpClient();
            MatrixSyncResult matrixSyncResult = null;

            // speedup "login": resolves room names more quickly after startup
            int timeout = _next_batch == null ? 1000 : 12000;

            string uri = String.Format("https://{0}/_matrix/client/r0/sync?timeout={1}&access_token={2}{3}", Properties.Settings.Default.matrixserver_hostname, timeout, matrixResult.access_token, _next_batch == null ? "" : "&since=" + _next_batch);
            Console.WriteLine("uri:" + uri);

            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await client.GetAsync(client.BaseAddress);
                } catch (Exception ex)
                {

                }
                Task<string> sss = response.Content.ReadAsStringAsync();
                Console.WriteLine("/sync response status code:" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("/sync response from server:" + responseString);

                    matrixSyncResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixSyncResult>(responseString);
                    _next_batch = matrixSyncResult.next_batch;


                    if (matrixSyncResult.account_data.events != null)
                    {
                        foreach (SyncResultsAccountDataEvents @event in matrixSyncResult.account_data.events)
                        {
                            if (@event.type == "m.direct")
                            {
                                System.Collections.Generic.Dictionary<string, object> a = (System.Collections.Generic.Dictionary<string, object>)@event.content;

                                foreach (KeyValuePair<string, object> o in a)
                                {
                                    string tmpUser = o.Key;

                                    List<string> tmpRooms = new List<string>();
                                    foreach (object aaa in (object[])o.Value)
                                    {
                                        tmpRooms.Add(aaa.ToString());
                                    }
                                    direct.Add(tmpUser, tmpRooms);
                                }
                            }
                        }
                        fireRoomsDirectResolvedEvent();
                    }

                    foreach (KeyValuePair<string, List<string>> d in direct)
                    {
                        foreach (string s in d.Value)
                        {
                            //Console.WriteLine(String.Format("direct: {0} {1}", d.Key, s));
                        }
                    }


                    if (matrixSyncResult.presence.events.Count > 0)
                    {
                        Dictionary<string, string> presenceChanged = new Dictionary<string, string>();
                        foreach (MatrixSyncResultPresenceEvents @event in matrixSyncResult.presence.events)
                        {
                            if (@event.type == "m.presence")
                            {
                                bool add = false;
                                if (presence.ContainsKey(@event.sender))
                                {
                                    if (presence[@event.sender] != @event.content.presence)
                                        add = true;
                                }
                                else
                                    add = true;

                                if (add)
                                {
                                    presence[@event.sender] = @event.content.presence;
                                    presenceChanged.Add(@event.sender, @event.content.presence);
                                }
                            }
                        }
                        if (presenceChanged.Count > 0)
                            fireUserPrecenseReceivedEvent(presenceChanged);
                    }


                    foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> wrapper in matrixSyncResult.rooms.join)
                    {
                        string roomID = wrapper.Key;
                        List<UserTypingInRoom> typingsRemoved = new List<UserTypingInRoom>();

                        if (wrapper.Value.unread_notifications != null) 
                        {
                            MatrixSyncUnreadNotifications notifications = wrapper.Value.unread_notifications;
                            fireUnreadNotificationsEvent(roomID, notifications);
                        }

                        foreach (MatrixSyncResultEphemeralEvents @event in wrapper.Value.ephemeral.events)
                        {
                            if (@event.type == "m.typing")
                            {
                                List<string> filtered_users = new List<string>();
                                // filter own typing events. user should now that she's/he's typing..

                                System.Collections.Generic.Dictionary<string, object> a = (Dictionary<string, object>)@event.content;
                                foreach ( KeyValuePair<string,object> b in a)
                                {
                                    object[] c = (object[])b.Value;
                                    foreach (object d in c)
                                    {
                                        string userid = d.ToString();
                                        if (loggedInUserID == userid)
                                        {
                                            continue;
                                        }
                                        filtered_users.Add(userid);
                                    }
                                }
                                fireTypingEvent(roomID, filtered_users);
                                
                            }

                            if (@event.type == "m.receipt")
                            {
                                List<ReceiptEvent> usersRecipients = new List<ReceiptEvent>();

                                System.Collections.Generic.Dictionary<string, object> a = (Dictionary<string, object>)@event.content;
                                foreach (KeyValuePair<string, object> b in a)
                                {
                                    System.Collections.Generic.Dictionary<string, object> c = (Dictionary<string, object>)b.Value;
                                    //string eventid = c.Key;

                                    foreach (KeyValuePair<string, object> d in c ) {
                                        if (d.Key == "m.read")
                                        {
                                            foreach (KeyValuePair<string, object> e in (System.Collections.Generic.Dictionary<string, object>)d.Value)
                                            {
                                                string user_id = e.Key;
                                                foreach (KeyValuePair<string, object> fe in (System.Collections.Generic.Dictionary<string, object>)e.Value)
                                                {
                                                    if (fe.Key == "ts")
                                                    {
                                                        long ts = (long)fe.Value;
                                                        usersRecipients.Add(new ReceiptEvent(roomID, user_id, ts));

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                fireMessageRecievedEvent(usersRecipients);
                            }
                        }

                    }

                    if (responseString.Contains("m.room.encrypted"))
                    {

                    }



                    foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> messagesForRoomID in matrixSyncResult.rooms.join)
                    {
                        string roomID = messagesForRoomID.Key;
                        Console.WriteLine("found message for room:" + roomID);

                        MatrixSyncResultTimelineWrapper wrapper = messagesForRoomID.Value;
                        foreach (MatrixSyncResultEvents events in wrapper.timeline.events)
                        {
                            ChatMessage chatMessage = null;
                            if (events.content.msgtype == "m.text")
                            {
                                string message = "";
                                if (events.content.format == "org.matrix.custom.html")
                                {
                                    //unencrypted
                                    message = events.content.formatted_body;
                                }
                                else if (events.content.format == null)
                                {
                                    message = events.content.body;
                                }
                                chatMessage = new ChatMessage(roomID, events.sender, message);
                            } else if (events.content.msgtype == "m.image") {
                                MatrixMediaRequest matrixMediaRequest = new MatrixMediaRequest();
                                matrixMediaRequest.filename = events.content.body;
                                matrixMediaRequest.sender = events.sender;
                                matrixMediaRequest.roomID = roomID;

                                chatMessage = new ChatMessageImage(roomID, events.sender, events.content.body, events.content.url);
                                downloadMedia(matrixMediaRequest, events.content.url);
                            }
                            if (chatMessage!=null)
                                chatMessages.Add(chatMessage);



                            //chat.processIncomingChatMessage(events.content.ciphertext);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not /sync(): " + e.Message);
            }

            fireSyncCompletedEvent(matrixSyncResult, initSync);

            return null;
        }

        internal async Task<MatrixLoginResult> loadRooms()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/joined_rooms?access_token={1}",Properties.Settings.Default.matrixserver_hostname,  matrixResult.access_token));
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                Console.WriteLine("loadRooms() response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("loadRooms() response from server:" + responseString);

                    MatrixJoinedRoomsResult matrixJoinedRoomsResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixJoinedRoomsResult>(responseString);

                    fireJoinedRoomyLoadedEvent(matrixJoinedRoomsResult);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not loadRooms(): " + e.Message);
            }
            return null;
        }

        internal async Task<MatrixLoginResult> loadMembers(string roomID)
        {
            MatrixLoginResult matrixLoginResult = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/rooms/{1}/members?access_token={2}", Properties.Settings.Default.matrixserver_hostname, roomID, matrixResult.access_token));
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                Console.WriteLine("response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("response from server:" + responseString);

                    MatrixMemberResult matrixMemberResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixMemberResult>(responseString);


                    foreach (MatrixMemberChunkResult chunk in  matrixMemberResult.chunk)
                    {
                        Console.WriteLine("user: " + chunk.user_id);
                    }
                    fireMembersLoaded(matrixMemberResult);

                    Console.WriteLine("matrixMemberResult.chunk:" + matrixMemberResult.chunk);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not loadMembersAwait(): " + e.Message);
            }

            return matrixLoginResult;
        }


        public void login()
        {
            loginAsync();
        }

        private async Task loginAsync()
        {
            await loginAwait();
        }



        public async Task setPresence(string roomID, string message)
        {
            await setPresenceAwait(roomID, message);
        }

        internal async Task<MatrixLoginResult> setPresenceAwait(string status, string status_message)
        {
            HttpClient client = new HttpClient();

            string uri = String.Format("https://{0}/_matrix/client/r0/presence/{1}/status?access_token={2}", Properties.Settings.Default.matrixserver_hostname, HttpUtility.UrlEncode(loggedInUserID), matrixResult.access_token);
            Console.WriteLine("uri: " + uri);
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            String template = @"
                  ""presence"": ""{0}"",
                  ""status_msg"": ""{1}""
                ";
            String jsonLogin = String.Format(template, status.ToLower(), status_message);
            jsonLogin = "{" + jsonLogin + "}";


            StringContent myStringContent = new StringContent(jsonLogin);
            try
            {
                Console.WriteLine("calling: " + client.BaseAddress + " w/ content: " + myStringContent);
                HttpResponseMessage response = await client.PutAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {

                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("response from server:" + responseString);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not setPresenceAwait(): " + e.Message);
            }
            return null;
        }





        public async Task sendMessageFile(string roomID, string filename)
        {
            await sendMessageFileAwait(roomID, filename);
        }

        internal async Task<MatrixLoginResult> sendMessageFileAwait(string roomID, string filename)
        {
            MatrixUploadResult matrixUploadResult = null;
            HttpClient client = new HttpClient();

            string fileNameLeaf = Path.GetFileName(filename);

            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/media/r0/upload?filename={1}&access_token={2}", Properties.Settings.Default.matrixserver_hostname, fileNameLeaf, matrixResult.access_token));

            FileStream s = new FileStream(filename, FileMode.Open);
            StreamContent myStringContent = new StreamContent(s);

            string mimeType = MimeMapping.GetMimeMapping(filename);

            myStringContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
            try
            {
                //Console.WriteLine("calling: " + client.BaseAddress + " w/ content: " + myStringContent);
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("sendMessageFileAwait() response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {

                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("response from server:" + responseString);

                    matrixUploadResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixUploadResult>(responseString);

                    matrixUploadResult.original_source_filename = filename;

                    Console.WriteLine("matrixUploadResult content_uri:" + matrixUploadResult.content_uri);

                    fireMatrixUploadCompletedEvent(matrixUploadResult);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not Liveddatapush(): " + e.Message);
            }




            return null;
        }


        public async Task sendMessageFile(string roomID, string filename, string original_source_filename)
        {
            await sendMessageFileAwait(roomID, filename, original_source_filename);
        }

        internal async Task<MatrixLoginResult> sendMessageFileAwait(string roomID, string mxc_uri, string original_source_filename)
        {
            MatrixLoginResult matrixLoginResult = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/rooms/{1}/send/m.room.message?access_token={2}", Properties.Settings.Default.matrixserver_hostname, roomID, matrixResult.access_token));
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            String template = @"
                  ""msgtype"": ""m.file"",
                  ""body"": ""{0}"",
                  ""filename"": ""{1}"",
                  ""url"": ""{2}""
                ";
            String jsonLogin = String.Format(template, Path.GetFileName(original_source_filename), Path.GetFileName(original_source_filename), mxc_uri);
            jsonLogin = "{" + jsonLogin + "}";


            StringContent myStringContent = new StringContent(jsonLogin);
            try
            {
                Console.WriteLine("calling: " + client.BaseAddress + " w/ content: " + myStringContent);
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {

                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("response from server:" + responseString);

                    matrixLoginResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixLoginResult>(responseString);


                    Console.WriteLine("livedataResult user_id:" + matrixLoginResult.user_id);


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not Liveddatapush(): " + e.Message);
            }




            return matrixLoginResult;
        }

        internal async Task<MatrixLoginResult> sendMessageImage(string roomID, string mxc_uri, string original_source_filename)
        {
            MatrixLoginResult matrixLoginResult = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/rooms/{1}/send/m.room.message?access_token={2}", Properties.Settings.Default.matrixserver_hostname, roomID, matrixResult.access_token));
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            String template = @"
                  ""msgtype"": ""m.image"",
                  ""body"": ""{0}"",
                  ""url"": ""{1}""
                ";
            String jsonLogin = String.Format(template, Path.GetFileName(original_source_filename), mxc_uri);
            jsonLogin = "{" + jsonLogin + "}";


            StringContent myStringContent = new StringContent(jsonLogin);
            try
            {
                Console.WriteLine("calling: " + client.BaseAddress + " w/ content: " + myStringContent);
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("response from server:" + responseString);

                    matrixLoginResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixLoginResult>(responseString);

                    Console.WriteLine("livedataResult user_id:" + matrixLoginResult.user_id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not Liveddatapush(): " + e.Message);
            }




            return matrixLoginResult;
        }

        internal async Task sendMessage(string roomID, string message)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/rooms/{1}/send/m.room.message?access_token={2}", Properties.Settings.Default.matrixserver_hostname, roomID, matrixResult.access_token));
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            String template = @"
                  ""msgtype"": ""m.text"",
                  ""body"": """",
                    ""format"": ""org.matrix.custom.html"",
                    ""formatted_body"": ""{0}""
                ";
            String jsonLogin = String.Format(template, message);
            jsonLogin = "{" + jsonLogin + "}";

            StringContent myStringContent = new StringContent(jsonLogin);
            try
            {
                Console.WriteLine("sendMessage() calling: " + client.BaseAddress + " w/ content: " + myStringContent);
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("sendMessage() response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {

                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("sendMessage() response from server:" + responseString);
                    //matrixLoginResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixLoginResult>(responseString);
                    //Console.WriteLine("livedataResult user_id:" + matrixLoginResult.user_id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not sendMessage(): " + e.Message);
            }
        }


        internal async Task<MatrixLoginResult> loginAwait()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/login", Properties.Settings.Default.matrixserver_hostname));
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            // todo: escape both username and password or generate json properly. removing " is just a quickfix.
            String jsonLogin = String.Format(@"{{
                  ""type"": ""m.login.password"",
                  ""identifier"": {{
                                ""type"": ""m.id.user"",
                    ""user"": ""{0}""
                  }},
                  ""password"": ""{1}""
                }}", Properties.Settings.Default.matrixserver_username.Replace("\"", ""), Properties.Settings.Default.matrixserver_password.Replace("\"", ""));



            StringContent myStringContent = new StringContent(jsonLogin);
            try
            {
                Console.WriteLine("calling: " + client.BaseAddress + " w/ content: " + myStringContent);
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, myStringContent);

                Task<string> sss = response.Content.ReadAsStringAsync();

                Console.WriteLine("response status code:" + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {

                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("response from server:" + responseString);

                    matrixResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixLoginResult>(responseString);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not Liveddatapush(): " + e.Message);
            }

            fireLoginCompletedEvent();

            return null;


        }
    }
}