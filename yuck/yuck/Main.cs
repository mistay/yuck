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
        }

        private void WhoamiCallback(MatrixWhoamiResult matrixWhoamiResult)
        {
            tsstatus.Text = "Logged in as " + matrixWhoamiResult.user_id;

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

        private void JoinedRoomsLoadedCallback(MatrixJoinedRoomsResult matrixJoinedRoomsResult)
        {
            lstRooms.Items.Clear();

            foreach (string room in matrixJoinedRoomsResult.joined_rooms)
            {
                lstRooms.Items.Add(room);
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
            openChatWindow(lstRooms.SelectedItem.ToString());
        }

        private void openChatWindow(string roomID)
        {
            Chat chat = new Chat();
            chat.RoomID = roomID;
            chat.Show();
        }

        private void StatusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void LstRooms_DoubleClick(object sender, EventArgs e)
        {
            openChatWindow(lstRooms.SelectedItem.ToString() );
        }

        private void LstRooms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LstRooms_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                openChatWindow( lstRooms.SelectedItem.ToString() );
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
    }
}
