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
        int x = 0;
        int increment = 20;
        DateTime time_collapsed;
        int direction = 1;
        int _delay = 1000; // milliseconds
        string _sender = "";
        Timer t;
        int _mouse_down_x = 0;
        private bool _brushedaside;

        private Notification()
        {
            InitializeComponent();
        }

        public object Tag { get; set; }
        public UnmanagedMemoryStream Audiofile { get; internal set; }

        public Notification(int delay, string sender, string message)
        {
            InitializeComponent();

            _delay = delay;
            _sender = sender;
            lblMessage.Text = sender + ": " + message;

            MAX_NOTIFICATION_WIDTH = 500;

            time_collapsed = DateTime.MinValue;

            t = new Timer();
            t.Tick += T_Tick;
            t.Interval = 1;
            t.Start();

            Audiofile = Properties.Resources.icq_uh_oh;

            
        }


        public delegate void NotificationClicked(object Tag);
        public event NotificationClicked NotificationClickedEvent;
        private void fireNotificationClickedEvent()
        {
            if (NotificationClickedEvent != null)
            {
                NotificationClickedEvent(Tag);
            }
        }


        private void shutdown()
        {
            t.Stop();
            this.Close();
        }

        private void fadeOut()
        {
            _brushedaside = true;
            direction = 2;
        }
        private void T_Tick(object sender, EventArgs e)
        {
            if (direction == 2 && (this.Width < 10))
            {
                shutdown();
            }
            else if (time_collapsed != DateTime.MinValue || _brushedaside)
            {
                if ((DateTime.Now - time_collapsed).TotalMilliseconds > _delay)
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

            if (direction == 0) return;

            if (direction == 1)
                this.Width += increment;
            if (direction == 2)
                this.Width -= increment;

            if (!this.IsDisposed && !this.Disposing)
                setLocationEfficiently(Screen.FromControl(this).Bounds.Size.Width - this.Width, 20);
        }

        private void setLocationEfficiently(int x, int y)
        {
            Point p = this.Location;
            if (!this.IsDisposed && !this.Disposing)
            {
                p.X = x;
                p.Y = y;
                this.Location = p;
            }
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            
        }

        private void Notification_MouseDown(object sender, MouseEventArgs e)
        {
            _mouse_down_x = e.Location.X;
        }

        private void Notification_MouseMove(object sender, MouseEventArgs e)
        {

            if (_mouse_down_x == 0)
                // start acting after mouse down
                return;

            //Console.WriteLine(String.Format("{0} {1}", _mouse_down_x, e.Location.X));
            int delta = _mouse_down_x - e.Location.X;
          //Console.WriteLine(String.Format("{0} ", delta));

            _mouse_down_x = e.Location.X;
            setLocationEfficiently( this.Location.X - delta,  10);

        }

        private void Notification_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine("direct: " + direction);

            _mouse_down_x = 0;
            if (this.Width<MAX_NOTIFICATION_WIDTH)
            {
                //user brushed window aside
                fadeOut();
            }
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            fadeOut();
            fireNotificationClickedEvent();
        }

        private void Notification_Shown(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Audiofile);
            player.Play();
        }
    }
}
