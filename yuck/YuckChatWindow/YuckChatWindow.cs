﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YuckChatWindow
{
    public partial class YuckChatControl: UserControl
    {
        public int Y;
        private List<Component> yuckComponents = new List<Component>();


        public YuckChatControl()
        {
            InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }

        public void AddMessage(bool ownMessage, string message)
        {
            Console.WriteLine("new y: " + Y);
            TextBox l = new TextBox();
            yuckComponents.Add(l);

            l.Multiline = true;
            l.Text = message;
            Size size = TextRenderer.MeasureText(l.Text, l.Font, new Size(this.Width - 100, 100000000), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            int x = !ownMessage ? 0 : this.Width - size.Width - 30;
            l.Location = new Point(x,Y - this.VerticalScroll.Value);
            l.Size = size;
            l.ForeColor = ownMessage ? Color.White : Color.Black;
            Y += size.Height + 10;
            l.ReadOnly = true;
            l.BorderStyle = BorderStyle.None;

            l.BackColor = ownMessage ? Color.FromArgb(0x03, 0x7C, 0xFF) : Color.LightGray;
            this.Controls.Add(l);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            AddMessage(true, "foo\r\n\aaa");
        }

        public void AddImage(bool ownMessage, Image image, string message)
        {
            PictureBox p = new PictureBox();
            yuckComponents.Add(p);
            p.Image = image;
            p.Size = new Size(100, 100);
            p.SizeMode = PictureBoxSizeMode.Zoom;
            int x = !ownMessage ? 0 : this.Width - 100;
            p.Location = new Point(x, Y - this.VerticalScroll.Value);
            this.Controls.Add(p);

            Y += 100;
            TextBox l = new TextBox();
            l.Text = message;
            l.Multiline = true;
            Size size = TextRenderer.MeasureText(l.Text, l.Font, new Size(this.Width - 100, 100000000), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            x = !ownMessage ? 0 : this.Width - size.Width - 30;
            l.Location = new Point(x, Y - this.VerticalScroll.Value);
            l.Size = size;
            l.ForeColor = ownMessage ? Color.White : Color.Black;
            Y += size.Height + 10;
            l.ReadOnly = true;
            l.BorderStyle = BorderStyle.None;

            l.BackColor = ownMessage ? Color.FromArgb(0x03, 0x7C, 0xFF) : Color.LightGray;
            yuckComponents.Add(l);
            this.Controls.Add(l);
        }

        public void Clear()
        {
            this.Controls.Clear();
            Y = 0;
        }
    }
}
