using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yuck
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Businesslogic.Instance.JoinedRoomsLoadedEvent += JoinedRoomsLoadedCallback;
            Businesslogic.Instance.LoginCompletedEvent += LoginCompltedCallback;

            Businesslogic.Instance.RoomsDirectResolvedEvent += RoomsDirectResolvedCallback;

            Businesslogic.Instance.SyncCompletedEvent += SyncCompletedCallback;

            Businesslogic.Instance.UserPrecenseReceivedEvent += UserPresenceReceivedCallback;

            Businesslogic.Instance.WhoamiEvent += WhoamiCallback;

            Businesslogic.Instance.RoomResolvedEvent += RoomResolvedCallback;

            cbPresence.Items.Add("Online");
            cbPresence.Items.Add("Offline");
            cbPresence.Items.Add("Unavailable");

            cbPresence.Text = Properties.Settings.Default.user_presence;

            Businesslogic.Instance.AvatarDownloadCompletedEvent += AvatarDownloadedCallback;

            
            Businesslogic.Instance.AvatarURLReceivedEvent += AvatarURLReceivedCallback;

            Businesslogic.Instance.TypingEvent += TypingCompletedCallback;
        }

        private void TypingCompletedCallback(List<string> user_ids)
        {
            string status;
            if (user_ids.Count == 0)
            {
                status = "";
            }
            else if(user_ids.Count == 1)
            {
                status = String.Join(" ", user_ids) + " is typing...";
            }
            else
            {
                status = String.Join(" ", user_ids) + " are typing...";
            }

            tsstatus.Text = status;
;        }

        private void AvatarURLReceivedCallback(MatrixAvatarResult matrixAvatarResult)
        {
            Uri uri = Businesslogic.MXC2HTTP(matrixAvatarResult.avatar_url);

            if (uri == null)
            {
                pbAvatar.Image = Properties.Resources.User_Avatar;
                pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                Businesslogic.Instance.downloadAvatar(uri);
            }
        }

        private void AvatarDownloadedCallback(Image image)
        {
            pbAvatar.Image = image;
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void WhoamiCallback(MatrixWhoamiResult matrixWhoamiResult)
        {
            lblUsername.Text = matrixWhoamiResult.user_id;

            Businesslogic.Instance.downloadAvatarURLAsync(matrixWhoamiResult.user_id);

        }

        private void UserPresenceReceivedCallback(Dictionary<string, string> changed)
        {
            if (presenceLoaded)
            {
                foreach (KeyValuePair<string, string> c in changed)
                {
                    Console.WriteLine("user came " + c.Value + ": " + c.Key);

                    string message = "";
                    switch (c.Value) {
                        case "online":
                            message = "user came online";
                            break;
                        case "offline":
                            message = "user went offline";
                            break;
                        default:
                            break;
                    }

                    notifyIcon1.ShowBalloonTip(1000, message, c.Key, ToolTipIcon.Info);
                }
            }
            presenceLoaded = true;

            lstUsers.Items.Clear();
            foreach (KeyValuePair<string, string> presence in Businesslogic.Instance.presence)
            {
                lstUsers.Items.Add(presence.Key + ": " + presence.Value);
            }
        }

        

        private void SyncCompletedCallback(MatrixSyncResult matrixSyncResult, bool initSync)
        {
            if (matrixSyncResult != null) {
                if (!initSync)
                {
                    foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> messagesForRoomID in matrixSyncResult.rooms.join)
                    {
                        Console.WriteLine("syncCompletedCallback()");

                        MatrixSyncResultTimelineWrapper wrapper = messagesForRoomID.Value;
                        foreach (MatrixSyncResultEvents events in wrapper.timeline.events)
                        {

                            if (events.content.msgtype == "m.text")
                            {
                                if (events.content.format == "org.matrix.custom.html")
                                {
                                }
                                else
                                {
                                    notifyIcon1.ShowBalloonTip(calcTimeoutFromWords(events.content.body), String.Format("{0}", Businesslogic.MatrixUsernameToShortUsername(events.sender)), events.content.body, ToolTipIcon.Info);
                                }
                            }

                            if (events.content.msgtype == "m.image")
                            {
                                notifyIcon1.ShowBalloonTip(3000, String.Format("{0}", Businesslogic.MatrixUsernameToShortUsername(events.sender)), "image", ToolTipIcon.Info);
                            }
                        }
                    }
                }


                foreach (Form form in Application.OpenForms)
                {
                    if (form is Chat)
                    {
                        Chat chat = (Chat)form;
                        Console.WriteLine(chat.MatrixRoom.roomID);


                        foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> messagesForRoomID in matrixSyncResult.rooms.join)
                        {
                            if (chat.MatrixRoom.roomID == messagesForRoomID.Key)
                            {
                                Console.WriteLine("found message for room:" + chat.MatrixRoom.roomID);

                                MatrixSyncResultTimelineWrapper wrapper = messagesForRoomID.Value;
                                foreach (MatrixSyncResultEvents events in wrapper.timeline.events)
                                {

                                    if (events.content.msgtype == "m.text")
                                    {
                                        //unencrypted
                                        if (events.content.format == "org.matrix.custom.html")
                                        {
                                            chat.processIncomingChatMessage(events.sender, events.content.formatted_body);
                                        }
                                        else
                                        {
                                            chat.processIncomingChatMessage(events.sender, events.content.body);
                                        }
                                    }

                                    if (events.content.msgtype == "m.image")
                                    {

                                        MatrixMediaRequest matrixMediaRequest = new MatrixMediaRequest();
                                        matrixMediaRequest.filename = events.content.body;
                                        matrixMediaRequest.sender = events.sender;
                                        matrixMediaRequest.roomID = chat.MatrixRoom.roomID;


                                        Businesslogic.Instance.downloadMedia(matrixMediaRequest, Businesslogic.MXC2HTTP(events.content.url));

                                    }
                                    //chat.processIncomingChatMessage(events.content.ciphertext);
                                }
                            }
                        }
                    }
                }
            }

            Businesslogic.Instance.sync();
        }

        private int calcTimeoutFromWords(string message)
        {
            // https://de.wikipedia.org/wiki/Lesegeschwindigkeit
            int words_per_minute = 150;
            string[] words = message.Split(' ');
            int numWords = words.Length;

            int timeout = (int)(((float)numWords / (float)words_per_minute) * 60 * 1000);

            if (timeout > 30000) timeout = 30000;

            return timeout;
        }

        public void LoginCompltedCallback()
        {
            presenceLoaded = false;
            tsstatus.Text = "Login Completed";
            Businesslogic.Instance.whoamiAsync();
            Businesslogic.Instance.loadRooms();
            Businesslogic.Instance.sync();
        }

        private void RoomsDirectResolvedCallback()
        {
            foreach (MatrixRoom matrixRoom in lstRooms.Items)
            {
                foreach (KeyValuePair<string, List<string>> d in Businesslogic.Instance.direct)
                {
                    foreach (string roomID in d.Value)
                    {
                        Console.WriteLine("vergeliche " + roomID + " mit " + matrixRoom.roomID);
                        if (roomID == matrixRoom.roomID)
                        {
                            Console.WriteLine("resolved room: " + d.Key);
                            matrixRoom.roomNameHumanReadable = d.Key;
                            matrixRoom.directRoom = true;
                        }
                    }
                }
            }
            refreshlstRoomsUpdate();

        }

        private void RoomResolvedCallback()
        {
            Console.WriteLine("RoomResolvedCallback()");
            foreach (KeyValuePair<string, string> cacheEntry in Businesslogic.Instance.roomCache)
            {
                foreach (MatrixRoom entry in lstRooms.Items)
                {
                    if (entry.roomID == cacheEntry.Key)
                    {
                        Console.WriteLine("RoomResolvedCallback() entry.roomID: " + entry.roomID + " value: " + cacheEntry.Value);

                        // found
                        entry.roomNameHumanReadable = cacheEntry.Value;
                        entry.directRoom = false;
                        break;
                    }
                }
            }
            refreshlstRoomsUpdate();
        }

        private void refreshlstRoomsUpdate()
        {
            //https://stackoverflow.com/questions/33175381/how-we-can-refresh-items-text-in-listbox-without-reinserting-it
            for (int i = 0; i < lstRooms.Items.Count; i++)
            {
                lstRooms.Items[i] = lstRooms.Items[i];
            }
        }

        

        private void JoinedRoomsLoadedCallback(MatrixJoinedRoomsResult matrixJoinedRoomsResult)
        {
            lstRooms.Items.Clear();

            foreach (string room in matrixJoinedRoomsResult.joined_rooms)
            {
                // todo: cache lookup needed? could be in cache, right?

                MatrixRoom matrixRoom = new MatrixRoom();
                matrixRoom.roomID = room;


                


                Console.WriteLine("added room:  " + room);
                lstRooms.Items.Add(matrixRoom);

                Businesslogic.Instance.resolveRoomname(room);

                /*
                string found = null;
                foreach (KeyValuePair<string, string> roomName in roomCache)
                {
                    if (roomName.Key == room)
                    {
                        // found
                        found = roomName.Value;
                        break;
                    }
                }

                if (found != null)
                {
                    lstRooms.Items.Add(found);
                }
                else
                {
                    
                }*/


                
            }
        }



        MatrixLoginResult matrixResult;
        private DateTime dateTimeLoginCompleted;
        private bool presenceLoaded;

        private void Button1_Click(object sender, EventArgs e)
        {
            Businesslogic.Instance.login();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
           
        }

        private void Button3_Click(object sender, EventArgs e)
        {
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            openChatWindow(((MatrixRoom)lstRooms.SelectedItem));
        }

        private void openChatWindow(MatrixRoom matrixRoom)
        {
            Chat chat = new Chat();
            chat.MatrixRoom = matrixRoom;
            chat.Show();
        }

        private void StatusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void LstRooms_DoubleClick(object sender, EventArgs e)
        {
            openChatWindow(((MatrixRoom)lstRooms.SelectedItem));
        }

        private void LstRooms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LstRooms_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                openChatWindow((MatrixRoom)lstRooms.SelectedItem);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        private void CbPresence_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.user_presence = cbPresence.Text;
            Properties.Settings.Default.Save();

            if (cbPresence.Text == "Online")
                Businesslogic.Instance.login();

            Businesslogic.Instance.setPresenceAwait(cbPresence.Text, "hello, i',m here!");
        }

        private void PbAvatar_Click(object sender, EventArgs e)
        {
            //Businesslogic.Instance.downloadMediaAwait();

        }

        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // minimize to system tray
            e.Cancel = true;
            Hide();
        }

        private void NotifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.Visible)
                this.Hide();
            else
            {
                Show();
                this.WindowState = FormWindowState.Normal;
            }
            //notifyIcon1.Visible = false;
        }

        private void LstRooms_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void LstRooms_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lstRooms.IndexFromPoint(e.Location);
                MatrixRoom matrixroom = (MatrixRoom)lstRooms.Items[index];
                Console.WriteLine(matrixroom.roomID);

                MatrixRoomProperties matrixRoomProperties =new MatrixRoomProperties();
                matrixRoomProperties.matrixRoom = matrixroom;
                matrixRoomProperties.updateGUI();

                matrixRoomProperties.Show();

            }
        }
    }
}
