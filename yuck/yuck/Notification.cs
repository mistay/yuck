using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yuck
{
    public partial class Notification : Form
    {
        public int MAX_NOTIFICATION_WIDTH { get; private set; }
        int x = 0; int y = 0;
        int increment = 10;
        DateTime time_collapsed;
        int direction = 1;
        int delay;
        Timer t;

        private Notification()
        {
            InitializeComponent();
        }
        public Notification(string message, int delay)
        {
            InitializeComponent();

            y = 0;
            lblMessage.Text = message;
            this.delay = delay;

            MAX_NOTIFICATION_WIDTH = 500;

            time_collapsed = DateTime.MinValue;

            t = new Timer();
            t.Tick += T_Tick;
            t.Interval = 1;
            t.Start();

            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.icq_uh_oh);
            player.Play();
        }

        private void shutdown()
        {
            t.Stop();
            this.Close();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (direction == 2 && (this.Width < 10)) {
                shutdown();
            }
            else if (time_collapsed != DateTime.MinValue)
            {
                if ((DateTime.Now - time_collapsed).TotalMilliseconds > delay)
                {
                    direction = 2;
                }
            }

            else if (this.Width > MAX_NOTIFICATION_WIDTH)
            {
                direction = 0;
                if (time_collapsed == DateTime.MinValue)
                    time_collapsed = DateTime.Now;
            }

            if (direction==0) return;

            if (direction==1)
                this.Width+=increment;
            if (direction ==2)
                this.Width -= increment;

            Point p = this.Location;
            if (!this.IsDisposed && !this.Disposing)
            {
                p.X = Screen.FromControl(this).Bounds.Size.Width - this.Width;
                p.Y = y;
                this.Location = p;
            }
        }

        private void Notification_Load(object sender, EventArgs e)
        {

        }
    }
}
