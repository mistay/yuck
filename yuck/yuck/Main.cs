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

        private bool presenceLoaded;
        private bool _reallyQuit;

        public Main()
        {
            InitializeComponent();

            Businesslogic.Instance.MainForm = this;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _reallyQuit = true;
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Businesslogic.Instance.JoinedRoomsLoadedEvent += JoinedRoomsLoadedCallback;
            Businesslogic.Instance.LoginCompletedEvent += LoginCompletedCallback;

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
            Businesslogic.Instance.MessageRecievedEvent += MessageReceivedCallback;
            Businesslogic.Instance.UnreadNotificationsEvent += UnreadNotificationsCallback;

            if (Properties.Settings.Default.mainform_height > 0)
                this.Height = Properties.Settings.Default.mainform_height;

            if (Properties.Settings.Default.mainform_width > 0)
                this.Width = Properties.Settings.Default.mainform_width;
        }

        private void UnreadNotificationsCallback(string roomID, MatrixSyncUnreadNotifications matrixSyncUnreadNotifications)
        {
            Chat chat = findOpenChatForm(roomID);
            if (chat != null)
                chat.UnreadNotifications(matrixSyncUnreadNotifications);
        }

        private Chat findOpenChatForm(string roomID)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is Chat)
                {
                    Chat chat = (Chat)form;
                    if (chat.MatrixRoom.roomID == roomID)
                    {
                        return chat;
                    }
                }
            }
            return null;
        }

        private void TypingCompletedCallback(string room_id, List<string> user_ids)
        {
            Chat chat = findOpenChatForm(room_id);
            if (chat != null)
                chat.UserTyping(user_ids);
        }

        private void MessageReceivedCallback(List<ReceiptEvent> receiptEvents)
        {
            foreach (ReceiptEvent receiptEvent in receiptEvents)
            {
                Chat chat = findOpenChatForm(receiptEvent.roomID);
                if (chat != null)
                    chat.MessageReceived(receiptEvent);
            }
        }

        private void AvatarURLReceivedCallback(MatrixAvatarResult matrixAvatarResult)
        {
            Uri uri = Businesslogic.MXC2HTTP(matrixAvatarResult.avatar_url);

            if (uri == null)
            {
                Bitmap b = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage(b);
                Brush brush = Brushes.White;
                g.FillEllipse(Brushes.Blue, new Rectangle(0, 0, 100, 100));
                string firstcharusername = (Businesslogic.MatrixUsernameToShortUsername(Businesslogic.Instance.loggedInUserID)).Substring(0, 1).ToUpper();
                g.DrawString(firstcharusername, new Font(FontFamily.GenericSansSerif, 70), brush , new Point(5, 0));

                pbAvatar.Image = b;  // Properties.Resources.User_Avatar;

                pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                Businesslogic.Instance.downloadAvatar(uri);
            }
        }

        private void AvatarDownloadedCallback(Image image)
        {
            // get ratio from original image to resize to picturebox width
            float ratio = (float)image.Width / (float)image.Height;
            int resizedWidth = pbAvatar.Width;
            int resizedHeight = (int)(pbAvatar.Width / ratio);

            // resize the image to the new format
            Bitmap resizedImage = new Bitmap(resizedWidth, resizedHeight);
            Graphics g = Graphics.FromImage((Image)resizedImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.DrawImage(image, 0, 0, resizedWidth, resizedHeight);
            g.Dispose();
            pbAvatar.Image = resizedImage;

            // Center the image to cut away everything except the face in the middle (assumption: face is in the middle of the picture)
            pbAvatar.SizeMode = PictureBoxSizeMode.CenterImage;
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
                            message = "presence changed but dunno how. neither online nor offline. from server: " + c.Value;
                            break;
                    }

                    Notification n = new Notification(3000, message, Businesslogic.MatrixUsernameToShortUsername(c.Key));
                    n.NotificationClickedEvent += NotificationClickedCallback;
                    n.Tag = c.Key; //todo: keep users in list of e.g. class MatrixUsers and lookup user. pass as instance 
                    n.Audiofile = Properties.Resources.icq_knock;
                    n.Show();
                    //notifyIcon1.ShowBalloonTip(1000, message, c.Key, ToolTipIcon.Info);
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

                    // raise notifications for messages closed room-windows
                    foreach (KeyValuePair<string, MatrixSyncResultTimelineWrapper> messagesForRoomID in matrixSyncResult.rooms.join)
                    {
                        Console.WriteLine("syncCompletedCallback()");

                        string roomID = messagesForRoomID.Key;

                        bool skip = false;
                        foreach (Form form in Application.OpenForms)
                        {
                            if (form is Chat)
                            {
                                Chat chat = (Chat)form;
                                if (chat.MatrixRoom.roomID == messagesForRoomID.Key)
                                {
                                    // skip notifcation because window is open
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        if (skip)
                            continue;

                        MatrixSyncResultTimelineWrapper wrapper = messagesForRoomID.Value;
                        foreach (MatrixSyncResultEvents events in wrapper.timeline.events)
                        {

                            string sender = String.Format("{0}", Businesslogic.MatrixUsernameToShortUsername(events.sender));
                            int timeout = 3000;
                            string message = "";
                            if (events.content.msgtype == "m.text")
                            {
                                if (events.content.format == "org.matrix.custom.html")
                                {
                                }
                                else
                                {
                                    timeout = calcTimeoutFromWords(events.content.body);
                                    message = events.content.body;
                                }
                            }

                            if (events.content.msgtype == "m.image")
                            {
                                message = "new image received";
                            }
                            Notification n = new Notification(timeout, sender, message);

                            // todo: loop through matrixrooms in businesslogic and resolve there. note: Businesslogic.Instance.roomCache does not contain MatrixRooms
                            foreach (MatrixRoom m in lstRooms.Items)
                            {
                                if (m.roomID == roomID)
                                {
                                    //found
                                    n.Tag = m;
                                    break;
                                }
                            }
                            n.NotificationClickedEvent += NotificationClickedCallback; // todo: howto/when unsubscribe?
                            n.Show();

                        }
                    }
                }






                // show messages (in chat history) in openend room-windows
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Chat)
                    {
                        Chat chat = (Chat)form;
                        Console.WriteLine(chat.MatrixRoom.roomID);


                        foreach (ChatMessage chatMessage in Businesslogic.Instance.chatMessages)
                        {
                            if (chat.MatrixRoom.roomID == chatMessage.RoomID && !chatMessage.Displayed)
                            {
                                chatMessage.Displayed = true;
                                Console.WriteLine(String.Format("displaying message from {0} for room {1}: {2}" , chatMessage.Sender, chat.MatrixRoom.roomID, chatMessage.Message));
                                //unencrypted
                                chat.processIncomingChatMessage(chatMessage.Sender, chatMessage.Message);
                            }
                        }
                    }
                }
            }

            Businesslogic.Instance.sync();
        }

        private void NotificationClickedCallback(object Tag)
        {
            MatrixRoom matrixRoom = null;


            if (Tag is MatrixRoom)
            {
                matrixRoom = (MatrixRoom)Tag;
            }

            if (Tag is string)
            {
                // assume this is a username, todo: pass as instance of e.g. MatrixUser (todo: create class)
                foreach (MatrixRoom m in lstRooms.Items)
                {
                    string username = Tag.ToString();
                    
                    // todo: maybe better to  match instances instead of string comparison. need concept!
                    if (m.roomNameHumanReadable == username)
                    {
                        // found
                        matrixRoom = m;
                        break;
                    }
                }
            }
            if (matrixRoom != null)
            {
                Chat chat = findOpenChatForm(matrixRoom.roomID);
                if (chat == null)
                {
                    // should not be necessary as we came here from notication that shouldn't have been raised as it should only be rased on closed chat windows
                    chat = new Chat(matrixRoom);
                    chat.Show();
                }
                chat.BringToFront();
            }
        }

        private int calcTimeoutFromWords(string message)
        {
            // https://de.wikipedia.org/wiki/Lesegeschwindigkeit
            int words_per_minute = 150;
            string[] words = message.Split(' ');
            int numWords = words.Length;

            int timeout = (int)(((float)numWords / (float)words_per_minute) * 60 * 1000);

            if (timeout < 1000) timeout = 1000; //minimum display time: it's prette uncomfortable to display messages no longer than this time
            if (timeout > 30000) timeout = 30000; // maximum display time

            return timeout;
        }

        public void LoginCompletedCallback()
        {
            presenceLoaded = false;
            tsStatuslabel.Text = "Login Completed";
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

                lstRooms.Items.Add(matrixRoom);

                Businesslogic.Instance.resolveRoomname(room);
            }
        }

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
            bool found = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form is Chat)
                {
                    Chat chat = (Chat)form;
                    if (chat.MatrixRoom.roomID == matrixRoom.roomID)
                    {
                        found = true;
                        // found
                        // already open, bring to front
                        chat.BringToFront();
                        break;
                    }
                }
            }
            if (!found)
            {
                // open new chat window
                (new Chat(matrixRoom)).Show();
            }
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
                //Hide();
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
            if (_reallyQuit)
            {
                notifyIcon1.Visible = false; // hide icon manually as windows does not remove icons properly sometimes
            } else
            {
                // minimize to system tray
                e.Cancel = true;
                Hide();
            }
        }

        private void NotifyIcon1_Click(object sender, EventArgs e)
        {
            
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

        private void NotifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible)
                    this.Hide();
                else
                {
                    Show();
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void TstxtExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _reallyQuit = true;
            Application.Exit();
        }

        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _reallyQuit = true;
            this.Close();
        }

        private void ExitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _reallyQuit = true;
            Application.Exit();
        }

        private void SettingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            Properties.Settings.Default.mainform_width = this.Width;
            Properties.Settings.Default.mainform_height = this.Height;
            Properties.Settings.Default.Save();
        }
    }
}
