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
        private static readonly HttpClient client = new HttpClient();
        MatrixLoginResult matrixResult;
        private string loggedInUserID;

        public delegate void membersLoadedEvent(MatrixMemberResult matrixMemberResult);
        public event membersLoadedEvent MembersLoaded;
        private void fireMembersLoaded(MatrixMemberResult matrixMemberResult)
        {
            if (MembersLoaded != null)
            {
                MembersLoaded(matrixMemberResult);
            }
        }

        public delegate void joinedRoomyLoaded(MatrixJoinedRoomsResult matrixJoinedRoomsResult);
        public event joinedRoomyLoaded JoinedRoomsLoadedEvent;
        private void fireJoinedRoomyLoadedEvent(MatrixJoinedRoomsResult matrixJoinedRoomsResult)
        {
            if (JoinedRoomsLoadedEvent != null)
            {
                JoinedRoomsLoadedEvent(matrixJoinedRoomsResult);
            }
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
            {
                WhoamiEvent(matrixWhoamiResult);
            }
        }


        public delegate void RoomResolved();
        public event RoomResolved RoomResolvedEvent;
        private void fireRoomResolvedEvent()
        {
            if (RoomResolvedEvent != null)
            {
                RoomResolvedEvent();
            }
        }


        public delegate void LoginCompleted();
        public event LoginCompleted LoginCompletedEvent;
        private void fireLoginCompletedEvent()
        {
            if (LoginCompletedEvent != null)
            {
                LoginCompletedEvent();
            }
        }

        public delegate void AvatarURLReceived(MatrixAvatarResult matrixAvatarResult);
        public event AvatarURLReceived AvatarURLReceivedEvent;
        private void fireAvatarURLReceivedEvent(MatrixAvatarResult matrixAvatarResult)
        {
            if (AvatarURLReceivedEvent != null)
            {
                AvatarURLReceivedEvent(matrixAvatarResult);
            }
        }

        public delegate void SyncCompleted(MatrixSyncResult matrixSyncResult);
        public event SyncCompleted SyncCompletedEvent;
        private void fireSyncCompletedEvent(MatrixSyncResult matrixSyncResult)
        {
            if (SyncCompletedEvent != null)
            {
                SyncCompletedEvent(matrixSyncResult);
            }
        }

        public delegate void UserPrecenseReceived(Dictionary<string, string> changed);
        public event UserPrecenseReceived UserPrecenseReceivedEvent;
        private void fireUserPrecenseReceivedEvent(Dictionary<string,string> changed)
        {
            if (UserPrecenseReceivedEvent != null)
            {
                UserPrecenseReceivedEvent(changed);
            }
        }


        


        public delegate void MessageCompleted(MatrixMessagesResult matrixMessagesResult);
        public event MessageCompleted MessageCompletedEvent;
        private void fireMessageCompletedEvent(MatrixMessagesResult matrixMessagesResult)
        {
            if (MessageCompletedEvent != null)
            {
                MessageCompletedEvent(matrixMessagesResult);
            }
        }


        public delegate void MediadownloadCompleted(Image image);
        public event MediadownloadCompleted MediadownloadCompletedEvent;
        private void fireMediadownloadCompletedEvent(Image image)
        {
            if (MediadownloadCompletedEvent != null)
            {
                MediadownloadCompletedEvent(image);
            }
        }


        public delegate void Typing(List<string> user_ids);
        public event Typing TypingEvent;
        private void fireTypingEvent(List<string> user_ids)
        {
            if (TypingEvent != null)
            {
                TypingEvent(user_ids);
            }
        }


        public async Task resolveRoomnameAsync(string roomID)
        {
            await resolveRoomnameAwait(roomID);
        }

        public Dictionary<string, string> roomCache = new Dictionary<string, string>();

        internal async Task<MatrixRoomResolvedResult> resolveRoomnameAwait(string roomID)
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

        public async Task downloadMediaAsync(Uri uri)
        {
            await downloadMediaAwait(uri);
        }
        internal async Task<Image> downloadMediaAwait(Uri uri)
        {
            HttpClient client = new HttpClient();

            //string uri = String.Format("https://{0}/_matrix/media/r0/download/st0ne.net/wEmUPhSlPdqDNlBHxlBAHAVx", Properties.Settings.Default.matrixserver_hostname);
            //client.BaseAddress = new Uri(uri);
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

                    //%%string responseString = response.Content.ReadAsStringAsync().Result;
                   // MemoryStream s = new MemoryStream();

                   // byte[] buffer = Encoding.ASCII.GetBytes(responseString.ToCharArray());
                    ////s.Write(buffer, 0, buffer.Length);
                    //s.Flush();
                    s.Seek(0, SeekOrigin.Begin);

                    Image i = Image.FromStream(s);

                    fireMediadownloadCompletedEvent(i);
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
                Console.WriteLine("could not syncAwait(): " + e.Message);
            }

            return null;
        }


        public Dictionary<string, string> presence = new Dictionary<string, string>();

        public async Task syncAsync(string next_batch)
        {
            await syncAwait(next_batch);
        }
        internal async Task<MatrixSyncResult> syncAwait(string next_batch)
        {

            HttpClient client = new HttpClient();
            MatrixSyncResult matrixSyncResult = null;

            string uri = String.Format("https://{0}/_matrix/client/r0/sync?timeout=120000&access_token={1}{2}", Properties.Settings.Default.matrixserver_hostname, matrixResult.access_token, next_batch == null ? "" : "&since=" + next_batch);
            Console.WriteLine("uri:" + uri);

            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                Task<string> sss = response.Content.ReadAsStringAsync();
                Console.WriteLine("/sync response status code:" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("syncAwait response from server:" + responseString);

                    matrixSyncResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixSyncResult>(responseString);

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
                        if (presenceChanged.Count>0)
                               fireUserPrecenseReceivedEvent(presenceChanged);
                    }



                    foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> wrapper in matrixSyncResult.rooms.join)
                    {
                        foreach (MatrixSyncResultEphemeralEvents @event in wrapper.Value.ephemeral.events)
                        {
                            fireTypingEvent(@event.content.user_ids);

                            if (@event.content.user_ids.Count == 0)
                            {
                                Console.WriteLine("noone is typing...");
                            }
                            else
                            {

                                foreach (string user_id in @event.content.user_ids)
                                {
                                    Console.WriteLine("user: " + user_id + "is typing ...");
                                }
                            }
                        }
                    }

                    if (responseString.Contains("m.room.encrypted"))
                    {

                    }

                    /*if (matrixSyncResult.presence.events != null)
                    {
                        fireUserPrecenseReceivedEvent(next_batch==null, matrixSyncResult);
                    }*/
                }
                

            }
            catch (Exception e)
            {
                Console.WriteLine("could not syncAwait(): " + e.Message);
            }

            fireSyncCompletedEvent(matrixSyncResult);

            return null;
        }

        public void loadRooms()
        {
            loadRoomsAsync();
        }
        public async Task loadRoomsAsync()
        {
            await loadRoomsAwait();
        }
        internal async Task<MatrixLoginResult> loadRoomsAwait()
        {
            MatrixLoginResult matrixLoginResult = null;
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(String.Format("https://{0}/_matrix/client/r0/joined_rooms?access_token={1}",Properties.Settings.Default.matrixserver_hostname,  matrixResult.access_token));
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

                    MatrixJoinedRoomsResult matrixJoinedRoomsResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MatrixJoinedRoomsResult>(responseString);

                    fireJoinedRoomyLoadedEvent(matrixJoinedRoomsResult);


                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not Liveddatapush(): " + e.Message);
            }

            return matrixLoginResult;
        }

        public void loadMembers(string roomID)
        {
            loadMembersAsync(roomID);
        }
        private async Task loadMembersAsync(string roomID)
        {
            await loadMembersAwait(roomID);
        }
        internal async Task<MatrixLoginResult> loadMembersAwait(string roomID)
        {
            MatrixLoginResult matrixLoginResult = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://matrix.st0ne.net/_matrix/client/r0/rooms/{0}/members?access_token={1}", roomID, matrixResult.access_token));
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





        public async Task sendMessageFile(string roomID, string message)
        {
            //await sendMessageFileAwait(roomID, message);
        }

        public async Task sendMessage(string roomID, string message)
        {
            await sendMessageAwait(roomID, message);
        }

        internal async Task<MatrixLoginResult> sendMessageAwait(string roomID, string message)
        {
            MatrixLoginResult matrixLoginResult = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(String.Format("https://matrix.st0ne.net/_matrix/client/r0/rooms/{0}/send/m.room.message?access_token={1}", roomID, matrixResult.access_token));
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