using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace yuck
{
    public partial class Chat : Form
    {
        public Chat()
        {
            InitializeComponent();
        }


        private MatrixRoom _matrixroom;
        internal MatrixRoom MatrixRoom { get { return _matrixroom; } set { _matrixroom = value; this.Text = "Yuck Chat Room " + _matrixroom.roomNameHumanReadable; } }

        private void Chat_Load(object sender, EventArgs e)
        {
            Businesslogic.Instance.MembersLoaded += membersLoadedCallback;
            Businesslogic.Instance.loadMembers( MatrixRoom.roomID);

            // prevent "ding" sound when pressing enter on txtMessage, weired.
            // https://stackoverflow.com/questions/6290967/stop-the-ding-when-pressing-enter
            // AND: processes message after ENTER pressed on txtMessage
            this.AcceptButton = btnSend;

            Businesslogic.Instance.MatrixUploadCompletedEvent += MatrixUploadCompletedCallback; ;
            //Businesslogic.Instance.MessageCompletedEvent += MessageCompletedCallback; ;

            //Businesslogic.Instance.syncAsync(null);
            Businesslogic.Instance.MediadownloadCompletedEvent += MediadownloadCompletedCallback;

        }

        private void MediadownloadCompletedCallback(MatrixMediaRequest matrixMediaRequest, Image image)
        {
            if (matrixMediaRequest.roomID == MatrixRoom.roomID)
            {
                // this image is intended for this chat
                processIncomingChatMessageImage(matrixMediaRequest.sender, matrixMediaRequest.filename, image);
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            processChatMessage();
            txtMessage.Focus();

        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {
        }

        private void TxtMessage_KeyUp(object sender, KeyEventArgs e)
        {
            // via Form.AcceptButton proccessed
        }
        public void processChatMessage()
        {
            string message = txtMessage.Text;
            txtMessage.Text = "";
            /*
            string newline = (txtChatmessages.Text == "") ? "" : "\n"; // Environment.NewLine;

            if (txtChatmessages.SelectionAlignment == HorizontalAlignment.Left)
                txtChatmessages.SelectionAlignment = HorizontalAlignment.Right;
            txtChatmessages.AppendText(newline + message);
            */
            Businesslogic.Instance.sendMessage(MatrixRoom.roomID, message);
        }

        public void processIncomingChatMessageImage(string sender, string filename, Image image)
        {
            yuckChatControl1.AddImage(sender == Businesslogic.Instance.loggedInUserID, image, filename);
            yuckChatControl1.AutoScrollPosition = new Point(1, 100000000);
        }


        public void processIncomingChatMessage(string sender, string message)
        {
            yuckChatControl1.AddMessage(sender == Businesslogic.Instance.loggedInUserID, message);
            yuckChatControl1.AutoScrollPosition = new Point(1, 100000000);
        }

        private void LstMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void MatrixUploadCompletedCallback(MatrixUploadResult matrixUploadResult)
        {

            string mimeType = MimeMapping.GetMimeMapping(matrixUploadResult.original_source_filename);
            if (mimeType.ToLower() == "image/jpeg" || mimeType.ToLower() == "image/png" || mimeType.ToLower() == "image/jpg" || mimeType.ToLower() == "image/png")
                Businesslogic.Instance.sendMessageImage(MatrixRoom.roomID, matrixUploadResult.content_uri, matrixUploadResult.original_source_filename);
            else
                Businesslogic.Instance.sendMessageFile(MatrixRoom.roomID, matrixUploadResult.content_uri, matrixUploadResult.original_source_filename);

        }

        private void MessageCompletedCallback(MatrixMessagesResult matrixMessagesResult)
        {
            //Thread.Sleep(10000);
            Businesslogic.Instance.messagesAsync(MatrixRoom.roomID, matrixMessagesResult.end);

            foreach (MatrixMessagesChunkResult matrixMessagesChunkResult in matrixMessagesResult.chunk)
            {
                try
                {
                    yuckChatControl1.AddMessage(true, matrixMessagesChunkResult.user_id + " eventid:" + matrixMessagesChunkResult.event_id + " " + matrixMessagesChunkResult.content.algorithm + " " + matrixMessagesChunkResult.content.ciphertext);
                }
                catch (Exception e)
                {
                    Console.WriteLine("could not AddMessage(): " + e.Message);
                }
            }
        }

        private void SyncCompletedCallback(MatrixSyncResult matrixSyncResult)
        {
            //Thread.Sleep(3000);
            Businesslogic.Instance.messagesAsync(MatrixRoom.roomID, matrixSyncResult.next_batch);
        }

        private void membersLoadedCallback(MatrixMemberResult matrixMemberResult)
        {
            if (matrixMemberResult.chunk[0].room_id == MatrixRoom.roomID)
            {
                lstMembers.Items.Clear();
                foreach (MatrixMemberChunkResult chunk in matrixMemberResult.chunk)
                {
                    lstMembers.Items.Add(chunk.user_id);
                }
            }
        }

        private void TxtChatmessages_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtChatmessages_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void Chat_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            Console.WriteLine("dragged");
        }

        private void Chat_DragDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("dropped");
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                Console.WriteLine("file: " + file);
                Businesslogic.Instance.sendMessageFile(MatrixRoom.roomID, file);
            }
        }

        private void TxtMessage_DoubleClick(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            yuckChatControl1.AddMessage(true, "foo\r\n\aaa");
            yuckChatControl1.AddMessage(false, "foo\r\n\aaa");
        }
    }
}
