﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yuck
{
    public partial class Chat : Form
    {
        public Chat()
        {
            InitializeComponent();
        }

        private string roomID;
        public string RoomID { get { return roomID; } set { roomID = value;  this.Text = "Yuck Chat Room " + roomID; } }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            processChatMessage();
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {
        }

        private void TxtMessage_KeyUp(object sender, KeyEventArgs e)
        {
            // via Form.AcceptButton proccessed
        }
        private void processChatMessage()
        {
            string message = txtMessage.Text;
            txtMessage.Text = "";

            string newline = (txtChatmessages.Text == "") ? "" : Environment.NewLine;

            txtChatmessages.AppendText(newline + message);
            txtChatmessages.SelectionAlignment = HorizontalAlignment.Right;

            Businesslogic.Instance.sendMessage(this.roomID, message);
        }

        private void LstMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Chat_Load(object sender, EventArgs e)
        {
            Businesslogic.Instance.MembersLoaded += membersLoadedCallback;
            Businesslogic.Instance.loadMembers(RoomID);

            // prevent "ding" sound when pressing enter on txtMessage, weired.
            // https://stackoverflow.com/questions/6290967/stop-the-ding-when-pressing-enter
            // AND: processes message after ENTER pressed on txtMessage
            this.AcceptButton = btnSend;

            Businesslogic.Instance.SyncCompletedEvent += SyncCompletedCallback;
            Businesslogic.Instance.MessageCompletedEvent += MessageCompletedCallback; ;

            Businesslogic.Instance.syncAsync(null);
        }

        private void MessageCompletedCallback(MatrixMessagesResult matrixMessagesResult)
        {
            //Thread.Sleep(10000);
            Businesslogic.Instance.messagesAsync(roomID, matrixMessagesResult.end);

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
            Businesslogic.Instance.messagesAsync(roomID, matrixSyncResult.next_batch);
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
    }
}