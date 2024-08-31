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
    public partial class MessagingForm : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        bool flagTimer = true;
        int elapsedTime = 500;

        public MessagingForm()
        {
            InitializeComponent();
        }

        private void t_Elapsed(object sender, EventArgs e)
        {
            if (flagTimer) timer.Tick -= new EventHandler(t_Elapsed);
            timer.Enabled = false;
            flagTimer = true;
            this.Hide();
        }

        public void SetImage(int n)
        {
            switch (n)
            {
                case 1:
                    // green check mark
                    pictureBox1.Image = Properties.Resources.CheckMark_40;
                    pictureBox1.Size = pictureBox1.Image.Size;
                    pictureBox1.Refresh();
                    pictureBox1.Visible = true;
                    flagTimer = true;
                    elapsedTime = 500;
                    break;
                case 2:
                    // blue loading image
                    //pictureBox1.Image = Properties.Resources.Loading75;
                    pictureBox1.Size = pictureBox1.Image.Size;
                    pictureBox1.Refresh();
                    pictureBox1.Visible = true;
                    flagTimer = true;
                    elapsedTime = 3000;
                    break;
                default:
                    break;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.ClientSize = pictureBox1.Image.Size;
            //this.Size = pictureBox1.Size;
            this.CenterToParent();
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            if (flagTimer)
            {
                timer.Interval = elapsedTime;
                timer.Enabled = true;
                timer.Tick += new EventHandler(t_Elapsed);
            }
        }
    }
}
