using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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


        public delegate void SyncCompleted(MatrixSyncResult matrixSyncResult);
        public event SyncCompleted SyncCompletedEvent;
        private void fireSyncCompletedEvent(MatrixSyncResult matrixSyncResult)
        {
            if (SyncCompletedEvent != null)
            {
                SyncCompletedEvent(matrixSyncResult);
            }
        }

        public delegate void UserPrecenseReceived(MatrixSyncResult matrixSyncResult);
        public event UserPrecenseReceived UserPrecenseReceivedEvent;
        private void fireUserPrecenseReceivedEvent(MatrixSyncResult matrixSyncResult)
        {
            if (UserPrecenseReceivedEvent != null)
            {
                UserPrecenseReceivedEvent(matrixSyncResult);
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
                    if (responseString.Contains("m.room.encrypted"))
                    {

                    }

                    if (matrixSyncResult.presence.events != null)
                    {
                        fireUserPrecenseReceivedEvent(matrixSyncResult);
                    }
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