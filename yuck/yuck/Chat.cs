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
            send(txtMessage.Text);
        }

        private void TxtMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                send(txtMessage.Text);
        }
        private void send(string message)
        {
            Businesslogic.Instance.sendMessage(message);
            txtMessage.Text = "";
        }

        private void LstMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Chat_Load(object sender, EventArgs e)
        {
            Businesslogic.Instance.MembersLoaded += membersLoadedCallback;
            Businesslogic.Instance.loadMembers(RoomID);
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
