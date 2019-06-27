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
    public partial class MatrixRoomProperties : Form
    {
        public MatrixRoomProperties()
        {
            InitializeComponent();
        }

        public MatrixRoom matrixRoom {get; set; }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void updateGUI()
        {
            chkDirect.Checked = matrixRoom.directRoom;
            txtRoomName.Text = matrixRoom.roomNameHumanReadable;
            txtoomID.Text = matrixRoom.roomID;
        }

        private void MatrixRoomProperties_Load(object sender, EventArgs e)
        {

        }
    }
}
