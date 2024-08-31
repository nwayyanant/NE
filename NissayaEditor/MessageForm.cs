using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NissayaEditor
{
    public partial class MessageForm : Form
    {
        int respButton1 = 0;
        int respButton2 = 0;
        int respButton3 = 0;
        int respDialog = 0;
        public MessageForm(int respButton1 = 1, int respButton2 = 0x7FFFFFFF, int respButton3 = 0x7FFFFFFF)
        {
            InitializeComponent();
            this.respButton1 = respButton1;
            this.respButton2 = respButton2;
            this.respButton3 = respButton3;
            if (respButton2 == 0x7FFFFFFF && respButton3 == 0x7FFFFFFF)
            {
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.button1.Location = new System.Drawing.Point(200, 100);
            }
            else
            {
                if (respButton3 == 0x7FFFFFFF)
                {
                    this.button3.Visible = false;
                    Point p1 = this.button1.Location;
                    Point p2 = this.button2.Location;
                    this.button1.Location = new System.Drawing.Point(p1.X + 83, 100);
                    this.button2.Location = new System.Drawing.Point(p2.X + 83, 100);
                }
            }
            this.SuspendLayout();
        }
        public int ShowDialog(string cap, string msg, string btnMsg1, string btnMsg2 = "", string btnMsg3 = "")
        {
            this.Text = cap;
            this.label_Message.Text = msg;
            this.button1.Text = btnMsg1;
            this.button2.Text = btnMsg2;
            this.button3.Text = btnMsg3;
            base.ShowDialog();
            return respDialog;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            respDialog = respButton1;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            respDialog = respButton2;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            respDialog = respButton3;
            Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.CenterToParent();
        }
    }
}
