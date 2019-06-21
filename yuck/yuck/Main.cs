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

            Businesslogic.Instance.SyncCompletedEvent += SyncCompletedCallback;

            Businesslogic.Instance.UserPrecenseReceivedEvent += UserPrecnseReceivedCallback;

            Businesslogic.Instance.WhoamiEvent += WhoamiCallback;

            Businesslogic.Instance.RoomResolvedEvent += RoomResolvedCallback;

            cbPresence.Items.Add("Online");
            cbPresence.Items.Add("Offline");
            cbPresence.Items.Add("Unavailable");

            cbPresence.Text = Properties.Settings.Default.user_presence;

            Businesslogic.Instance.MediadownloadCompletedEvent += MediadownloadedCallback;

            
            Businesslogic.Instance.AvatarURLReceivedEvent += AvatarURLReceivedCallback;
        }

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
                Businesslogic.Instance.downloadMediaAsync(uri);
            }
        }

        private void MediadownloadedCallback(Image image)
        {
            pbAvatar.Image = image;
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void WhoamiCallback(MatrixWhoamiResult matrixWhoamiResult)
        {
            lblUsername.Text = matrixWhoamiResult.user_id;

            Businesslogic.Instance.downloadAvatarURLAsync(matrixWhoamiResult.user_id);

        }

        private void UserPrecnseReceivedCallback(MatrixSyncResult matrixSyncResult)
        {
            lstUsers.Items.Clear();
            foreach (SyncResultEvents @event in matrixSyncResult.presence.events)
            {
                lstUsers.Items.Add(@event.sender + ": " + @event.content.presence + " " + @event.content.currenty_active + " " + @event.content.last_active_ago);
            }
        }

        private void SyncCompletedCallback(MatrixSyncResult matrixSyncResult)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is Chat)
                {
                    Chat chat = (Chat)form;
                    Console.WriteLine(chat.RoomID);

                    foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> messagesForRoomID in matrixSyncResult.rooms.join)
                    {
                        if (chat.RoomID == messagesForRoomID.Key)
                        {
                            Console.WriteLine("found message for room:" + chat.RoomID);

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
                                //chat.processIncomingChatMessage(events.content.ciphertext);
                            }
                        }
                    }
                }
            }

            Businesslogic.Instance.syncAsync(matrixSyncResult.next_batch);
        }

        public void LoginCompltedCallback()
        {
            tsstatus.Text = "Login Completed";
            Businesslogic.Instance.whoamiAsync();
            Businesslogic.Instance.loadRooms();

            Businesslogic.Instance.syncAsync(null);
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

                MatrixRoom gUIListboxRoomEntry = new MatrixRoom();
                gUIListboxRoomEntry.roomID = room;

                Console.WriteLine("added room:  " + room);
                lstRooms.Items.Add(gUIListboxRoomEntry);

                Businesslogic.Instance.resolveRoomnameAsync(room);

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
            chat.matrixRoom = matrixRoom;
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
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
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
    }
}
