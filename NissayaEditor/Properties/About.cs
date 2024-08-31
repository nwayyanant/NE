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
    public partial class About : Form
    {
        public About(string progname, string version)
        {
            InitializeComponent();
            label_Title.Text = progname;
            label_Version.Text = version;
        }

        private void button_AboutQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
