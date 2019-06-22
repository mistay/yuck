﻿using System;
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

        internal MatrixRoom matrixRoom;
        public string RoomID { get { return matrixRoom.roomID; } set { matrixRoom.roomID = value; this.Text = "Yuck Chat Room " + matrixRoom.roomID; } }

        private void Chat_Load(object sender, EventArgs e)
        {
            Businesslogic.Instance.MembersLoaded += membersLoadedCallback;
            Businesslogic.Instance.loadMembers(RoomID);

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
            if (matrixMediaRequest.roomID == matrixRoom.roomID)
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
            Businesslogic.Instance.sendMessage(matrixRoom.roomID , message);
        }

        public void InsertImage(Image image)
        {
            Bitmap myBitmap = new Bitmap(image);
            // Copy the bitmap to the clipboard.
            Clipboard.SetDataObject(myBitmap);
            // Get the format for the object type.
            DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);
            // After verifying that the data can be pasted, paste
            if (txtChatmessages.CanPaste(myFormat))
            {
                txtChatmessages.Paste(myFormat);
            }
            else
            {
                Console.WriteLine("inserting clipboard into rtb file format not supported");
            }
        }

        public void processIncomingChatMessageImage(string sender, string filename, Image image)
        {
            string newline = (txtChatmessages.Text == "") ? "" : Environment.NewLine;
            string message = sender + " :" + filename;
            txtChatmessages.AppendText(newline + message);
            InsertImage(image);

            if (sender == Businesslogic.Instance.loggedInUserID) // e.g. "@armin:st0ne.net"
            {
                if (txtChatmessages.SelectionAlignment == HorizontalAlignment.Left)
                    txtChatmessages.SelectionAlignment = HorizontalAlignment.Right;
            }
            else
            {
                if (txtChatmessages.SelectionAlignment == HorizontalAlignment.Right)
                    txtChatmessages.SelectionAlignment = HorizontalAlignment.Left;
            }
        }


        public void processIncomingChatMessage(string sender, string message)
        {
            string newline = (txtChatmessages.Text == "") ? "" : Environment.NewLine;
            txtChatmessages.AppendText(newline + message);

            if (sender == Businesslogic.Instance.loggedInUserID) // e.g. "@armin:st0ne.net"
            {
                if (txtChatmessages.SelectionAlignment == HorizontalAlignment.Left)
                    txtChatmessages.SelectionAlignment = HorizontalAlignment.Right;
            } else
            {
                if (txtChatmessages.SelectionAlignment == HorizontalAlignment.Right)
                    txtChatmessages.SelectionAlignment = HorizontalAlignment.Left;
            }
        }

        private void LstMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void MatrixUploadCompletedCallback(MatrixUploadResult matrixUploadResult)
        {

            string mimeType = MimeMapping.GetMimeMapping(matrixUploadResult.original_source_filename);
            if (mimeType.ToLower() == "image/jpeg" || mimeType.ToLower() == "image/png" || mimeType.ToLower() == "image/jpg" || mimeType.ToLower() == "image/png")
                Businesslogic.Instance.sendMessageImage(matrixRoom.roomID, matrixUploadResult.content_uri, matrixUploadResult.original_source_filename);
            else
                Businesslogic.Instance.sendMessageFile(matrixRoom.roomID, matrixUploadResult.content_uri, matrixUploadResult.original_source_filename);

        }

        private void MessageCompletedCallback(MatrixMessagesResult matrixMessagesResult)
        {
            //Thread.Sleep(10000);
            Businesslogic.Instance.messagesAsync( matrixRoom.roomID  , matrixMessagesResult.end);

            foreach (MatrixMessagesChunkResult matrixMessagesChunkResult in matrixMessagesResult.chunk)
            {
                try
                {
                    txtChatmessages.AppendText(matrixMessagesChunkResult.user_id + " eventid:" + matrixMessagesChunkResult.event_id + " " + matrixMessagesChunkResult.content.algorithm + " " + matrixMessagesChunkResult.content.ciphertext +  Environment.NewLine);
                    txtChatmessages.SelectionAlignment = HorizontalAlignment.Left;

                }
                catch (Exception e)
                {
                    Console.WriteLine("could not add text: " + e.Message);
                }
            }
        }

        private void SyncCompletedCallback(MatrixSyncResult matrixSyncResult)
        {
            //Thread.Sleep(3000);
            Businesslogic.Instance.messagesAsync(matrixRoom.roomID, matrixSyncResult.next_batch);
        }

        private void membersLoadedCallback(MatrixMemberResult matrixMemberResult)
        {
            lstMembers.Items.Clear();

            foreach (MatrixMemberChunkResult chunk in matrixMemberResult.chunk)
            {
                lstMembers.Items.Add(chunk.user_id);
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
                Businesslogic.Instance.sendMessageFile(matrixRoom.roomID, file);
            }
        }
    }
}
