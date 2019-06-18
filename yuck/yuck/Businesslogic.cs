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


        public delegate void LoginCompleted();
        public event LoginCompleted LoginCompletedEvent;
        private void fireLoginCompletedEvent()
        {
            if (LoginCompletedEvent != null)
            {
                LoginCompletedEvent();
            }
        }


        public void loadRooms()
        {
            try
            {
                loadRoomsAsync();
            } catch (Exception e)
            {
                Console.WriteLine("could not loadRooms: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
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

                    Console.WriteLine("livedataResult user_id:" + matrixMemberResult.chunk);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("could not Liveddatapush(): " + e.Message);
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