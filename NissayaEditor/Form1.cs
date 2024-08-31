using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        const string version = "Pre-Release Version 1.0.8 [2022-07-31]";
        const string progname = "Nissaya Editor";
        const string title = progname + ", " + version;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int fileCount = 0;
        int curTabIndex = -1;
        bool dataGridValue1_Updated = false;
        bool richTextBox1_Updated = false;
        bool MNBook_Open = false;

        public const int ViewPali = 1;
        public const int ViewPlain = 2;
        public const int ViewFullDoc = 3;
        public const int ViewTabular = 4;
        public const int ViewDataBox = 5;

        const string ConfigFile = "NissayaEditor.config";
        const string jsonDLL = "Newtonsoft.Json.dll";
        JSON.Json config = null;

        public static RichTextBox rtb;
        int indexCurCursor = 0;
        public static char[] textMarkers = { '*', '^', '#', '@' };
        public static char[] recMarkers = { '*', '#' }; // record marker; * - nissaya data record; '#' - comment record
        //public static string fileFilters = "txt files (*.txt)|*.txt|nbk files (*.nbk)|*.nbk|rtf files (*.rtf)|*.rtf|All files (*.*)|*.*";
        public static string fileFilters = "txt files (*.txt)|*.txt|rtf files (*.rtf)|*.rtf|All files (*.*)|*.*";
        static public DebugTracer dbgTrace = null; //new DebugTracer();
        MN_Catalog mnCatalog = null;
        Dictionary<NissayaTabPage, int> dictMNTabs = new Dictionary<NissayaTabPage, int>();

        public string SaveDataToFile = "Save data to a file first.";
        public bool flagTabControlFullSize = true; // tabcontrol is full size means DataBox is not visible
        // false = not full size because DataBox is visible

        public Dictionary<int, string> MN_Titles = new Dictionary<int, string>() {
            { 1, "မူလပရိယာယသုတ်" },
            { 2, "သဗ္ဗာသဝသုတ်" },
            { 3, "ဓမ္မဒါယာဒသုတ်" },
            { 4, "ဘယဘေရဝသုတ်" },
            { 5, "အနင်္ဂဏသုတ်" },
            { 6, "အာကင်္ခေယျသုတ်" },
            { 7, "ဝတ္ထသုတ်" },
            { 8, "သလ္လေခသုတ်" },
            { 9, "သမ္မာဒိဋ္ဌိသုတ်" },
            { 10, "မဟာသတိပဋ္ဌာနသုတ်" },
            { 11, "စူဠသီဟနာဒသုတ်" },
            { 12, "မဟာသီဟနာဒသုတ်" },
            { 13, "မဟာဒုက္ခက္ခန္ဓသုတ်" },
            { 14, "စူဠဒုက္ခက္ခန္ဓသုတ်" },
            { 15, "အနုမာနသုတ်" },
            { 16, "စေတောခိလသုတ်" },
            { 17, "ဝနပတ္တသုတ်" },
            { 18, "မဓုပဏ္ဍိကသုတ်" },
            { 19, "ဒွေဓာဝိတက္ကသုတ်" },
            { 20, "ဝိတက္ကသဏ္ဍာနသုတ်" },
            { 21, "ကကစူပမသုတ်" },
            { 22, "အလဂဒ္ဒူပမသုတ်" },
            { 23, "ဝမ္မိကသုတ်" },
            { 24, "ရထဝိနီတသုတ်" },
            { 25, "နိဝါပသုတ်" },
            { 26, "ပါသရာသိသုတ်" },
            { 27, "စူဠဟတ္ထိပဒေါပမသုတ်" },
            { 28, "မဟာသာရောပမသုတ်" },
            { 29, "မဟာသာရောပမသုတ်" },
            { 30, "စူဠသာရောပမသုတ်" },
            { 31, "စူဠဂေါသိင်္ဂသုတ်" },
            { 32, "မဟာဂေါသိင်္ဂသုတ်" },
            { 33, "မဟာဂေါပါလကသုတ်" },
            { 34, "စူဠဂေါပါလကသုတ်" },
            { 35, "စူဠသစ္စကသုတ်" },
            { 36, "မဟာသစ္စကသုတ်" },
            { 37, "စူဠတဏှာသင်္ခယသုတ်" },
            { 38, "မဟာတဏှာသင်္ခယသုတ်" },
            { 39, "မဟာအဿပုရသုတ်" },
            { 40, "စူဠအဿပုရသုတ်" },
            { 41, "သာလေယျကသုတ်" },
            { 42, "ဝေရဉ္ဇကသုတ်" },
            { 43, "မဟာဝေဒလဒသုတ်" },
            { 44, "စူဠဝေဒလဒသုတ်" },
            { 45, "စူဠဓမ္မသမာဒါနသုတ်" },
            { 46, "မဟာဓမ္မသမာဒါနသုတ်" },
            { 47, "ဝီမံသကသုတ်" },
            { 48, "ကောသမ္ဗိယသုတ်" },
            { 49, "ဗြဟ္မနိမန္တနိကသုတ်" },
            { 50, "မာရတဇ္ဇနီယသုတ်" },
            { 51, "ကန္ဒရကသုတ်" },
            { 52, "အဋ္ဌကနာဂရသုတ်" },
            { 53, "သေခသုတ်" },
            { 54, "ပေါတလိယသုတ်" },
            { 55, "ဇီဝကသုတ်" },
            { 56, "ဥပါလိသုတ်" },
            { 57, "ကုက္ကုရဝတိကသုတ်" },
            { 58, "အဘယရာဇကုမာရသုတ်" },
            { 59, "ဗဟုဝေဒနီယသုတ်" },
            { 60, "အပဏ္ဏကသုတ်" },
            { 61, "အမ္ဗလဋ္ဌိကရာဟုလောဝါဒသုတ်" },
            { 62, "မဟာရာဟုလောဝါဒသုတ်" },
            { 63, "စူဠမာလုကျသုတ်" },
            { 64, "မဟာမာလုကျသုတ်" },
            { 65, "ဘဒ္ဒါလိသုတ်" },
            { 66, "ကဋုကိကောပမသုတ်" },
            { 67, "စတုမသုတ်" },
            { 68, "နဠကပါနသုတ်" },
            { 69, "ဂေါလိယာနိသုတ်" },
            { 70, "ကီဋာဂိရိသုတ်" },
            { 71, "တေဝိဇ္ဇဝစ္ဆသုတ်" },
            { 72, "အဂ္ဂိဝစ္ဆသုတ်" },
            { 73, "မဟာဝစ္ဆသုတ်" },
            { 74, "ဒီဃနခသုတ်" },
            { 75, "မာဂဏ္ဍိယသုတ်" },
            { 76, "သန္ဒကသုတ်" },
            { 77, "မဟာသကုလုဒါယိသုတ်" },
            { 78, "သမဏမုဏ္ဍိကသုတ်" },
            { 79, "စူဠသကုလုဒါယိသုတ်" },
            { 80, "ဝေခနသသုတ်" },
            { 81, "ဃဋိကာရသုတ်" },
            { 82, "ရဋ္ဌပါလသုတ်" },
            { 83, "မဃဒေဝသုတ်" },
            { 84, "မဓုရသုတ်" },
            { 85, "ဗောဓိရာဇကုမာရသုတ်" },
            { 86, "အင်္ဂုလိမာလသုတ်" },
            { 87, "ပိယဇာတိကသုတ်" },
            { 88, "ဗာဟိဘိကသုတ်" },
            { 89, "ဓမ္မစေတိယသုတ်" },
            { 90, "ကဏ္ဏကတ္ထလသုတ်" },
            { 91, "ဗြဟ္မာယုသုတ်" },
            { 92, "သေလသုတ်" },
            { 93, "အဿလာယနသုတ်" },
            { 94, "ဃောဋမုခသုတ်" },
            { 95, "စင်္ကီသုတ်" },
            { 96, "ဧသုကာရီသုတ်" },
            { 97, "ဓနဉ္ဇာနိသုတ်" },
            { 98, "ဝါသေဋ္ဌသုတ်" },
            { 99, "သုဘသုတ်" },
            { 100, "သင်္ဂါရဝသုတ်" },
            { 101, "ဒေဝဒဟသုတ်" },
            { 102, "ပဉ္စတ္တယသုတ်" },
            { 103, "ကိန္တိသုတ်" },
            { 104, "သာမဂါမသုတ်" },
            { 105, "သုနက္ခတ္တသုတ်" },
            { 106, "အာနေဉ္ဇသပ္ပါယသုတ်" },
            { 107, "ဂဏကမောဂ္ဂလ္လာနသုတ်" },
            { 108, "ဂေါပကမောဂ္ဂလ္လာနသုတ်" },
            { 109, "မဟာပုဏ္ဏမသုတ်" },
            { 110, "စူဠပုဏ္ဏမသုတ်" },
            { 111, "အနုပဒသုတ်" },
            { 112, "ဆဗ္ဗိသောဓနသုတ်" },
            { 113, "သပ္ပုရိသသုတ်" },
            { 114, "သေဝိတဗ္ဗာသေဝိတဗ္ဗသုတ်" },
            { 115, "ဗဟုဓာတုကသုတ်" },
            { 116, "ဣသိဂိလိသုတ်" },
            { 117, "မဟာစတ္တာရီသကသုတ်" },
            { 118, "အာနာပါနဿတိသုတ်" },
            { 119, "ကာယဂတာသတိသုတ်" },
            { 120, "သင်္ခါရုပပတ္တိသုတ်" },
            { 121, "စူဠသုညတသုတ်" },
            { 122, "မဟာသုညတသုတ်" },
            { 123, "အစ္ဆရိယအဗ္ဘုတသုတ်" },
            { 124, "ဗာကုလသုတ်" },
            { 125, "ဒန္တဘူမိသုတ်" },
            { 126, "ဘူမိဇသုတ်" },
            { 127, "အနုရုဒ္ဓသုတ်" },
            { 128, "ဥပက္ကိလေသသုတ်" },
            { 129, "ဗာလပဏ္ဍိတသုတ်" },
            { 130, "ဒေဝဒူတသုတ်" },
            { 131, "ဘဒ္ဒေကရတ္တသုတ်" },
            { 132, "အာနန္ဒဘဒ္ဒေကရတ္တသုတ်" },
            { 133, "မဟာကစ္စာနဘဒ္ဒေကရတ္တသုတ်" },
            { 134, "လောမသကင်္ဂိယဘဒ္ဒေကရတ္တသုတ်" },
            { 135, "စူဠကမ္မဝိဘင်္ဂသုတ်" },
            { 136, "မဟာကမ္မဝိဘင်္ဂသုတ်" },
            { 137, "သဠာယတနဝိဘင်္ဂသုတ်" },
            { 138, "ဥဒ္ဒေသဝိဘင်္ဂသုတ်" },
            { 139, "အရဏဝိဘင်္ဂသုတ်" },
            { 140, "ဓာတုဝိဘင်္ဂသုတ်" },
            { 141, "သစ္စဝိဘင်္ဂသုတ်" },
            { 142, "ဒက္ခိဏာဝိဘင်္ဂသုတ်" },
            { 143, "အနာထပိဏ္ဍိကောဝါဒသုတ္တံ" },
            { 144, "ဆန္နောဝါဒသုတ္တံ" },
            { 145, "ပုဏ္ဏောဝါဒသုတ္တံ" },
            { 146, "နန္ဒကောဝါဒသုတ္တံ" },
            { 147, "စူဠရာဟုလောဝါဒသုတ္တံ" },
            { 148, "ဆဆက္ကသုတ္တံ" },
            { 149, "မဟာသဠာယတနိကသုတ္တံ" },
            { 150, "နဂရဝိန္ဒေယျသုတ္တံ" },
            { 151, "ပိဏ္ဍပါတပါရိသုဒ္ဓိသုတ္တံ" },
            { 152, "ဣန္ဒြိယဘာဝနာသုတ္တံ" }
            };
        int curViewCode = 3;
        int caretPosition = -1;
        int nListFindIndex = 0;
        int nCurGridFindIndex = 0;
        bool fileUpdated = false;
        const string space5 = "     ";
        MatchCollection matches = null;
        enum ViewMenu { undefined, Pali, Plain, Full, Tabular };
        ViewMenu currentViewMenu = ViewMenu.Full;
        NissayaTabPage curNTP = null;

        //MessagingForm msgForm = null;
        Color defaultDataGridViewSelectionColor = System.Drawing.SystemColors.Highlight;
        //string textRTB = string.Empty;
        bool newDesign = true;
        int newIndex = 0;

        // lists
        List<DataInfo> dataFileList = new List<DataInfo>();     // not used anymore
        List<NissayaBook> nissayaBooks = new List<NissayaBook>();
        List<int> listFindIndex = new List<int>();
        List<NissayaTabPage> nissayaTabPageList = new List<NissayaTabPage>();
        List<string> openFileList = new List<string>();
        Dictionary<string, NissayaBook> dictNissaya = new Dictionary<string, NissayaBook>();
        List<string> listLoadFiles = new List<string>();

        public Font fontMYA = new Font("Myanmar Text", 12F, System.Drawing.FontStyle.Regular); //, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        static Font fontROM = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        PageMenuBar pageMenuBar = new PageMenuBar();

        public struct _GridViewSelection
        {
            public int row, col;
            public _GridViewSelection(int r, int c) { row = r; col = c; }
        }
        
        class FindInfo
        {
            public string searchText;
            public MatchCollection matches;
            public int index;
            public void Clear() { searchText = string.Empty; matches = null;  index = -1; }
            public void init(MatchCollection m) { matches = m; index = 0; }
        }

        FindInfo findInfoPali = new FindInfo();
        FindInfo findInfoPlain = new FindInfo();
        FindInfo findInfoFull = new FindInfo();
        
        List<_GridViewSelection> listGridViewSelection = new List<_GridViewSelection>();
        string viewInfo = "FullDoc";

        private Point newLocation(Point p)
        {
            return new Point(p.X - 227, p.Y);
        }

        public Form1()
        {
            InitializeComponent();
            Text = title;
            if (File.Exists(ConfigFile))
            {
                config = new JSON.Json("NissayaEditor.config");
                string fontInfo = config.GetData("FONT");
                Font f = getUserFont(fontInfo);
                if (f != null && (fontMYA.Name != f.Name || fontMYA.Size != f.Size))
                    richTextBox1.Font = fontMYA = f;
                viewInfo = config.GetData("VIEW");
            }
            else
            {
                MessageBox.Show(ConfigFile + " is missing. The file is required to continue.");
                Environment.Exit(0);
            }
            mnCatalog = new MN_Catalog(this);
            mnCatalog.OnOpen += MNCatalogFileOpenEvent;
            // hide staus and move the rest to the left
            Size sz = textBox1.Size;
            Point loc = textBox1.Location;
            textBox1.Text = "";
            Boolean original = false;
            if (!original)
            {
                //label1.Text = label_DataStatus.Text = string.Empty;
                label1.Visible = false;
                label_DataStatus.Visible = false;
                button_Find.Location = newLocation(button_Find.Location);
                textBox_Find.Location = newLocation(textBox_Find.Location);
                textBox_FindStatus.Location = newLocation(textBox_FindStatus.Location);
                button_FindPrev.Location = newLocation(button_FindPrev.Location);
                button_FindNext.Location = newLocation(button_FindNext.Location);
                label2.Location = newLocation(label2.Location);
                button_X.Location = newLocation(button_X.Location);

                button_jjha.Location = newLocation(button_jjha.Location);
                button_kinsi.Location = newLocation(button_kinsi.Location);
                button_ttha.Location = newLocation(button_ttha.Location);
                button_ddha.Location = newLocation(button_ddha.Location);
                label_MN.Location = newLocation(label_MN.Location);
                textBox_MN.Location = newLocation(textBox_MN.Location);
                label_MNTitle.Location = newLocation(label_MNTitle.Location);
                textBox1.Location = newLocation(textBox1.Location);
            }
            label_MNTitle.Text = string.Empty;
            //textBox1.Location = newLocation(textBox1.Location);
            newDesign = true;
            label_DataStatus.Text = string.Empty;
            // find all clear button_X
            ToolTip ttp = new ToolTip();
            ttp.OwnerDraw = true;
            ttp.Draw += new DrawToolTipEventHandler(toolTip1_Draw);
            ttp.Popup += new PopupEventHandler(toolTip1_Popup);
            ttp.BackColor = Color.Yellow;
            ttp.ForeColor = Color.Black;
            ttp.SetToolTip(button_X, "Clear");
            ttp.SetToolTip(button_FindPrev, "Previous");
            ttp.SetToolTip(button_FindNext, "Next");
            ttp.SetToolTip(button_ddha, "ဍ + ဎ");
            ttp.SetToolTip(button_kinsi, "င်္ + က = င်္က");
            ttp.SetToolTip(button_jjha, "ဇ + ဈ");
            ttp.SetToolTip(button_ttha, "ဋ + ဌ");

            //dataBoxCtrlDToolStripMenuItem.Checked = false;

            // register KeyDown events
            //textBox1.KeyDown += textBox1_KeyDown;
            //tabControl1.KeyDown += textBox1_KeyDown;
            //pageMenuBar.KeyDown += textBox1_KeyDown;
            //textBox_MN.KeyDown += textBox1_KeyDown;
            //tabControl1.KeyPress += richTextBox1_KeyPress;

            foreach (Control control in this.Controls)
            {
                control.KeyDown -= textBox1_KeyDown;
                control.KeyDown += textBox1_KeyDown;
            }
            textBox_Find.KeyDown -= textBox1_KeyDown;
            richTextBox1.KeyDown -= textBox1_KeyDown;
            richTextBox1.KeyPress -= this.richTextBox1_KeyPress;
            richTextBox1.KeyPress += this.richTextBox1_KeyPress;

            textBox_Find.KeyPress += textBox_Find_KeyPress;
            textBox_Find.Click += TextBox_Click;
            tabControl1.SelectedIndexChanged += Form1_TabSelectedIndexChanged;

            textBox_FindStatus.Text = string.Empty;
            button_FindNext.Enabled = button_FindPrev.Enabled = false;

            //*****************************************************
            //*** richTextBox1 is no longer used
            //*** richTextBox is created with eacn NissayaTabPage
            //*****************************************************
            richTextBox1.Enabled = false;
            richTextBox1.Visible = false;
            //tabControl1.KeyDown += tabControl1_KeyDown;

            TabPage tb = tabControl1.TabPages[0];
            //tabControl1.TabPages[0] = new NissayaTabPage(this, tabControl1, textBox1); //Form1 pForm, TabControl tc, TextBox tb);
            if (newDesign)
            {
                //tabControl1.DrawMode = 
                //tabControl1.DrawItem += DrawOnTab;
                // Hide separator.
                toolStripSeparator1.Visible = false;
                foreach (string s in getMRU())
                    if (s.Length > 0) addMRUEnd(s);

                this.Controls.Add(pageMenuBar);
                this.textBox1.Font = fontMYA;
                this.textBox1.ScrollBars = ScrollBars.Vertical;
                initNewDesign();
                //ControlCollection cc = (System.Windows.Forms.Form.ControlCollection)this.Controls;
                //string name, typename;
                //foreach (Control c in cc)
                //{
                //    Type type = c.GetType();
                //    typename = type.Name;
                //    name = c.Name;
                //    c.Visible = false;
                //}
                return;
            }

            //quitAppendMode();
            initDataGridView();
            dataStatus();
            rtb = richTextBox1;
                
            defaultDataGridViewSelectionColor = dataGridView1.DefaultCellStyle.SelectionBackColor;

            tabControl1.TabPages[0].Text = "<New" + (++newIndex).ToString() + ">";

            string[] args = Environment.GetCommandLineArgs();

            if (args.Count() == 2)
            {
                // command line has filename argument
                if (File.Exists(args[1]))
                    loadFile(args[1]);
                else
                    MessageBox.Show(args[1] + " file not found.");
            }
        }
        void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            Font f = new Font("Myanmar Text", 10.0f);
            ToolTip ttp = (ToolTip)sender;
            ttp.BackColor = System.Drawing.Color.Yellow;
            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText, f, Brushes.Black, new PointF(6, 4));
        }
        void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            ToolTip ttp = (ToolTip)sender;
            string s = ttp.GetToolTip(e.AssociatedControl);
            // on popip set the size of tool tip
            e.ToolTipSize = TextRenderer.MeasureText(s, new Font("Myanmar Text", 10.0f));
        }
        //**********************************************************************
        // #MRU
        private void addMRUEnd(string fName)
        {
            toolStripSeparator1.Visible = true;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)menuStrip1.Items[0];
            menuItem.DropDownItems.Add(fName);
            int n = menuItem.DropDownItems.Count;
            //string s = menuItem.DropDownItems[8].Text;
            menuItem.DropDownItems[n - 1].Click -= new EventHandler(dropDownItemClickedEvent);
            menuItem.DropDownItems[n - 1].Click += new EventHandler(dropDownItemClickedEvent);
        }
        // #MRU
        private void addMRUBegin(string fName)
        {
            toolStripSeparator1.Visible = true;
            ToolStripItem item = menuStrip1.Items[0];
            ToolStripMenuItem menuItem = (ToolStripMenuItem)item;
            int count = menuItem.DropDownItems.Count;
            int index = menuItem.DropDownItems.IndexOf(toolStripSeparator1) + 1;
            if (index < count && menuItem.DropDownItems[index].Text.ToLower() == fName.ToLower()) return;

            // since this file now becomes the last used file
            // insert file at the top of MRU
            ToolStripMenuItem subMenuItem = new ToolStripMenuItem(fName);
            menuItem.DropDownItems.Insert(index, subMenuItem);
            menuItem.DropDownItems[index].Click -= new EventHandler(dropDownItemClickedEvent);
            menuItem.DropDownItems[index].Click += new EventHandler(dropDownItemClickedEvent);

            // remove the file if it is already in the list below.

            int n = -1;
            for (int i = index + 1; i < menuItem.DropDownItems.Count; ++i )
            {
                if (menuItem.DropDownItems[i].Text.ToLower() == fName.ToLower())
                {
                    n = i; break;
                }
            }
            if (n != -1)
            {
                // exact same file found; remove from menu
                menuItem.DropDownItems.RemoveAt(n);
            }

            int max = index + 5;    // file list size = 5
            count = menuItem.DropDownItems.Count;
            if (count > max)
                menuItem.DropDownItems.RemoveAt(--count);
        }
        // #MRU
        private void clearMRU()
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)menuStrip1.Items[0];
            int count = menuItem.DropDownItems.Count;

            while (count > 0)
            {
                --count;
                var type = menuItem.DropDownItems[count].GetType();
                if (type.Name == "ToolStripSeparator")
                {
                    toolStripSeparator1.Visible = false;
                    break;
                }
                menuItem.DropDownItems.RemoveAt(count);
            }
        }
        // #MRU
        private void adjustMRU(ToolStripItem currentSelectedItem)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)menuStrip1.Items[0];
            int index = menuItem.DropDownItems.IndexOf(currentSelectedItem);
            menuItem.DropDownItems.RemoveAt(index);
            addMRUBegin(currentSelectedItem.Text);
        }
        // #MRU
        private void dropDownItemClickedEvent(object sender, EventArgs e)
        {
            ToolStripItem currentSelectedItem = (ToolStripItem)sender;
            if (openFileList.Contains(currentSelectedItem.Text) ||
                IsNewPageWithData()) return;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)menuStrip1.Items[0];
            int index = menuItem.DropDownItems.IndexOf(toolStripSeparator1) + 1;
            int n = menuItem.DropDownItems.IndexOf(currentSelectedItem);
            if (index < menuItem.DropDownItems.IndexOf(currentSelectedItem)) adjustMRU(currentSelectedItem);
            //loadFile(currentSelectedItem.Text);
            string pattern = "{ MN-(.*?) }";
            MatchCollection matches = Regex.Matches(currentSelectedItem.Text, pattern);
            if (matches.Count > 0)
            {
                int pos = currentSelectedItem.Text.Substring(5).IndexOf(" }");
                string MN_No = currentSelectedItem.Text.Substring(5, pos);
                textBox_MN.Text = MN_No;
                label_MNTitle.Text = MN_Titles[Convert.ToInt16(MN_No)];
                MNOpenEventArgs eargs = new MNOpenEventArgs();
                eargs.MNTitle = label_MNTitle.Text;
                eargs.MN_No = MN_No;
                MNCatalogFileOpenEvent(this, eargs);
                return;
            }
            if (!File.Exists(currentSelectedItem.Text))
            {
                MessageBox.Show(currentSelectedItem.Text + " file does not exist and will be removed from the list.");
                deleteMRU(currentSelectedItem.Text);
                return;
            }
            if (Path.GetExtension(currentSelectedItem.Text).ToLower() == ".nbk")
            {
                loadBook(currentSelectedItem.Text, true);
            }
            else loadFile(currentSelectedItem.Text);
        }
        // #MRU
        private void saveMRU()
        {
            List<string> cfgList = new List<string>();
            int n = 0;
            string s1 = string.Empty;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)menuStrip1.Items[0];
            int count = menuItem.DropDownItems.Count;
            int index = menuItem.DropDownItems.IndexOf(toolStripSeparator1) + 1;
            while (index < count)
            {
                if (index < count)
                {
                    s1 = "MRU" + (++n).ToString() + "|";
                    s1 += menuItem.DropDownItems[index++].Text;
                    cfgList.Add(s1);
                }
            }
            while (n < 5)
            {
                s1 = "MRU" + (++n).ToString();
                cfgList.Add(s1);
            }
            // save view info
            s1 = "VIEW|";
            s1 += (curViewCode == 4) ? "Tabular" : "FullDoc";
            cfgList.Add(s1);

            config.UpdateData(cfgList.ToArray());
        }
        // #MRU
        private List<string> getMRU()
        {
            List<string> listMRU = new List<string>();
            int n = 0;
            string value = string.Empty;
            while (n < 5)
            {
                value = config.GetData("MRU" + (++n).ToString());
                if (value.Length > 0) listMRU.Add(value);
            }
            return listMRU;
        }
        // #MRU
        private void deleteMRU(string s)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)menuStrip1.Items[0];
            int count = menuItem.DropDownItems.Count;
            int index = menuItem.DropDownItems.IndexOf(toolStripSeparator1);
            //ToolStripMenuItem item = new ToolStripMenuItem();
            //item.Text = s;
            //int index1 = menuItem.DropDownItems.IndexOf(item);
            while (++index < count)
            {
                if (menuItem.DropDownItems[index].Text == s)
                {
                    menuItem.DropDownItems.RemoveAt(index);
                    break;
                }
            }
        }
        //**********************************************************************

        private Font getUserFont(string fontInfo)
        {
            InstalledFontCollection ifc = new InstalledFontCollection();
            FontFamily[] fontFamilies = ifc.Families;
            Font font = null;
            string[] fInfo = fontInfo.Split(';');
            if (fontInfo.Length > 0)
            {
                bool found = false;
                for (int i = 0; i < fontFamilies.Count() && !found; ++i)
                {
                    if (fontFamilies[i].Name.Contains(fInfo[0]))
                    {
                        float fSize = 12F;
                        if (fInfo.Count() == 2) fSize = Convert.ToSingle(fInfo[1]);
                        font = new Font(fInfo[0], fSize, System.Drawing.FontStyle.Regular);
                        found = true;
                    }
                }
                if (!found)
                {
                    string errmsg = "'" + fInfo[0] + "' font not installed in the system." + System.Environment.NewLine +
                        "The app will use the default font.";
                    MessageBox.Show(errmsg);
                }
            }
            return font;
        }

        private void initNewDesign()
        {
            NissayaTabPage ntabPage = new NissayaTabPage(this, tabControl1, tabControl1.TabPages[0], textBox1);
            nissayaTabPageList.Add(ntabPage);
            this.ActiveControl = ntabPage.GetRichTextBox();  // to avoid textbox/datagrid scrolling while loading data
            richTextBox1.Visible = false;
            dataGridView1.Visible = false;
            richTextBox1.Enabled = false;
            dataGridView1.Enabled = false;

            // register textBox1 events
            textBox1.KeyPress += textBox1_KeyPress;
        }

        private void loadBook(string bname, bool showNissayaBookContent = false)
        {
            NissayaBook nbk;
            if (File.Exists(bname))
            {
                if (dictNissaya.ContainsKey(bname))
                    nbk = dictNissaya[bname];
                else
                {
                    nbk = new NissayaBook(bname);
                    dictNissaya.Add(bname, nbk);
                }
                if (showNissayaBookContent)
                {
                    NissayaBookContent nbc = new NissayaBookContent(bname, nbk);
                    nbc.ShowDialog();
                    if (!nbc.FlagOpenBook) return;
                }
                foreach (NissayaBook.Chapter ch in nbk.chapters)
                    if (ch.checkBox && !openFileList.Contains(ch.fileName))
                        listLoadFiles.Add(ch.fileName);
                loadBookFile();
                addMRUBegin(bname);
            }
            else MessageBox.Show(bname + " is not found.");
        }

        private void loadBookFile()
        {
            bool flagContinue = true;
            while (flagContinue)
            {
                if (listLoadFiles.Count() == 0)
                {
                    // fill page menu bar
                    NissayaTabPage ntabPage = nissayaTabPageList[tabControl1.SelectedIndex];
                    List<string> pgno = ntabPage.GetPageNo();
                    pageMenuBar.FillPageMenu(pgno);
                    return;
                }
                string f = listLoadFiles[0];
                listLoadFiles.RemoveAt(0);
                if (File.Exists(f))
                {
                    flagContinue = false;
                    if (textBox_MN.Text.Length > 0)
                    {
                        loadFile(f, Convert.ToInt16(textBox_MN.Text));
                    }
                    else loadFile(f);
                }
                else
                    MessageBox.Show(f + " file does not exist.");
                if (listLoadFiles.Count == 0 && MNBook_Open)
                {
                    MNBook_Open = false;
                    string MNBook_Name = "{ MN-" + textBox_MN.Text + " }";
                    openFileList.Add(MNBook_Name);
                    addMRUBegin(MNBook_Name);
                }

            }
        }

        public void LoadPageData(string pgno)
        {
            NissayaTabPage ntabPage = nissayaTabPageList[tabControl1.SelectedIndex];
            ntabPage.SavePage();
            //if (textBox1.Visible)
            //{
                // textBox1 is the common to all tab-pages dataBox
                // textBox1 is used for tabular data editing
                // if textBox1 is opened then close it before opening the new page
                textBox1.Text = string.Empty;
                //dataBoxCtrlDToolStripMenuItem.Checked = false;
                //ntabPage.ShowDataBox(false);
            //}
            ntabPage.LoadPage(pgno);
            //ntabPage.CheckFindData();
        }

        public void SavePageData()
        {
            NissayaTabPage ntabPage = nissayaTabPageList[tabControl1.SelectedIndex];
            ntabPage.SavePage();
        }

        private void loadFile(string fname, int MN_no = 0)
        {
            TabControl tc = tabControl1;
            RichTextBox rtb = richTextBox1;
            TabPage tb = null;
            NissayaTabPage ntp;

            if (newDesign)
            {
                Text = title + space5 + fname;
                this.ActiveControl = null;      // if richTextBox has focus there will be scrolling
                label_DataStatus.Text = "Loading...";
                ntp = nissayaTabPageList[tabControl1.SelectedIndex];
                if (ntp.NewAndEmpty())
                {
                    ntp.LoadFile(fname);
                    ntp.MNBookFile = MN_no > 0;
                    ntp.MN_No = MN_no;
                }
                else
                {
                    tb = new TabPage();
                    tc.TabPages.Add(tb);
                    ntp = new NissayaTabPage(this, tc, tb, textBox1);
                    // check for passing MN_no parameter
                    if (MN_no >= 1 && MN_no <= 152) ntp.MN_No = MN_no;
                    ntp.curViewCode = curViewCode;
                    ntp.MNBookFile = MN_no > 0;
                    nissayaTabPageList.Add(ntp);
                    ntp.LoadFile(fname);
                    if (ntp.MN_No >= 1 && ntp.MN_No <= 152)
                    {
                        MN_no = ntp.MN_No;
                        textBox_MN.Text = MN_no.ToString();
                        label_MNTitle.Text = MN_Titles[MN_no];
                    }
                    else textBox_MN.Text = label_MNTitle.Text = string.Empty;
                    tabControl1.SelectedIndexChanged -= Form1_TabSelectedIndexChanged;
                    tabControl1.SelectedTab = tb;
                    tabControl1.SelectedIndexChanged += Form1_TabSelectedIndexChanged;
                }
                // add individual file names to MRU
                if (MN_no == 0)
                {
                    openFileList.Add(fname);
                    addMRUBegin(fname);
                }
                else
                {
                    if (ntp != null) dictMNTabs.Add(ntp, MN_no);
                }
                fileCount = tabControl1.TabCount;
                curTabIndex = tabControl1.SelectedIndex;
                button_X_Click(this, null);
                return;
            }

            Text += space5 + fname;
            this.ActiveControl = null;      // if richTextBox has focus there will be scrolling
            // while the data is loaded into the rtb

            init_FileLoad_BackgroundWorker();
            DataInfo dInfo = new DataInfo(this, tabControl1, tb, rtb, dataGridView1, fname);
            dataFileList.Add(dInfo);
            
            label_DataStatus.Text = "Loading...";
            richTextBox1.TextChanged -= richTextBox1_TextChanged;
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            Cursor.Current = Cursors.WaitCursor;
            //Thread.Sleep(250);      // wait for cursor to change
            richTextBox1.Clear();
            backgroundWorker1.RunWorkerAsync(dInfo);
        }

        private void startNewProcess(string exeFile, string fname)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo(exeFile, "\"" + fname + "\"")
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Path.GetDirectoryName(exeFile),
                    UseShellExecute = true,
                    CreateNoWindow = true
                }
            };
            var v = process.Start();
        }

        private void dataStatus()
        {
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            label_DataStatus.Text = ntp.IsDataReadOnly() ? "Read-only Ready" : "Edit Ready";
        }

        private void TabControl1_Selecting(Object sender, TabControlCancelEventArgs e)
        {
            MessageBox.Show(SaveDataToFile);
            e.Cancel = true;
        }
        public void NewPage(string pgno)
        {
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            if (pageMenuBar.Rows.Count >= 3 && !ntp.DataFileExist())
            {
                MessageBox.Show(SaveDataToFile);
                return;
            }
            ntp.NewPage(pgno);
            button_X_Click(this, null);
            if (!ntp.DataFileExist())
            {
                // if this is a new tab and new page just added and not saved yet then
                // register event so that the user cannot switch tabPages
                tabControl1.Selecting -= TabControl1_Selecting;
                tabControl1.Selecting += TabControl1_Selecting;
            }
        }

        public bool IsNewPage()
        {
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            return ntp.IsNewPage();
        }

        private int curFindTabIndex = -1;

        public int GetCurFindTabIndex() { return curFindTabIndex; }

        public void FindNextTab()
        {
            if (tabControl1.TabCount == 1) return;
            if (curFindTabIndex == -1) curFindTabIndex = tabControl1.SelectedIndex + 1;
            else ++curFindTabIndex;
            if (curFindTabIndex >= tabControl1.TabCount) curFindTabIndex = 0;
            if (curFindTabIndex == tabControl1.SelectedIndex)
            {
                curFindTabIndex = -1;
                return;
            }
            NissayaTabPage ntp = nissayaTabPageList[curFindTabIndex];
            ntp.TabFindText();
        }

        private bool preListView(bool flag = false)
        {
            if (fileCount == 0 && richTextBox1.Text.Length > 0)
            {
                MessageBox.Show("Save data to a file first.");
                return false;
            }
            tabControl1.Visible = flag;
            dataGridView1.Visible = !flag;
            if (fileCount == 0 && richTextBox1.Text.Length == 0) return true;
            
            if (richTextBox1_Updated && currentViewMenu != ViewMenu.Tabular) // tableToolStripMenuItem.Text == "Tabular")
            {
                // copy richTextBox data to fileContent and to dataGridView1
                DataInfo dInfo = dataFileList[curTabIndex];
                label_DataStatus.Text = "Refreshing...";
                Cursor.Current = Cursors.WaitCursor;
                Thread.Sleep(200); 

                richTextBox1.SelectionStart = caretPosition;

                init_RefreshGridView_BackgroundWorker();
                dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
                //textRTB = richTextBox1.Text;
                backgroundWorker1.RunWorkerAsync(dInfo);
            }
            if (dataGridValue1_Updated && currentViewMenu == ViewMenu.Tabular)// tableToolStripMenuItem.Text == "Tabs")
            {
                // copy dataGridView1 data to fileContent and to richTextBox1 content
                DataInfo dInfo = dataFileList[curTabIndex];

                label_DataStatus.Text = "Refreshing...";
                Cursor.Current = Cursors.WaitCursor;
                Thread.Sleep(200);
                
                richTextBox1.TextChanged -= richTextBox1_TextChanged;
                dInfo.DataGridViewToFileContent();
                //init_RichTextView_BackgroundWorker();      
                dataGridValue1_Updated = false;
                richTextBoxFindClear();
                //backgroundWorker1.RunWorkerAsync(dInfo);
            }
            return true;
        }

        private void refreshRichTextBoxDisplay(int viewCode)
        {
            DataInfo dInfo = dataFileList[curTabIndex];
            this.ActiveControl = null;
            label_DataStatus.Text = "Refreshing...";
            Cursor.Current = Cursors.WaitCursor;
            richTextBox1.TextChanged -= richTextBox1_TextChanged;
            if (richTextBox1_Updated)
            {
                dInfo.RichTextBoxToFileContent();
                richTextBox1_Updated = false;
            }
            richTextBox1.Clear();
#if (!DEBUG)
            //Task task1 = Task.Factory.StartNew(() => dInfo.RefreshData(viewCode));
            //Task.WaitAny(task1);
            dInfo.RefreshData(viewCode);
#else
            dInfo.RefreshData(viewCode);
#endif
            Cursor.Current = Cursors.Default;
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            richTextBox1_Updated = false;
            dataStatus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AdjustControlPlacements();
            if (newDesign)
            {
                int index = tabControl1.SelectedIndex;
                nissayaTabPageList[index].InitViews();
                if (viewInfo == "Tabular") tableToolStripMenuItem_Click(null, null);
            }
        }

        private void AdjustControlPlacements()
        {
            const int minWidth = 1200; // 790;
            const int minHeight= 900;
            Size szForm = this.Size;
            if (szForm.Width < minWidth) szForm.Width = minWidth;
            if (szForm.Height < minHeight) szForm.Height = minHeight; // 560;
            this.Size = szForm;

            int rmargin = 25;
            int bmargin = 15;
            int lmargin = 0;
            int tmargin = 0;

            Size szClient = this.ClientSize;

            rmargin = 20;
            lmargin = 12;
            bmargin = 60;
            tmargin = 42;
            int w = szClient.Width - lmargin - rmargin;
            int h = szClient.Height - tmargin - bmargin;
        }

        private void SizeUpdate(object sender, EventArgs e)
        {
            AdjustControlPlacements();
        }

        private void initDataGridView()
        {
            //Font fontMYA = new Font("Myanmar Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //Font fontROM = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            dataGridView1.Location = new Point(25, 80);
            dataGridView1.Width = this.ClientSize.Width - 50;
            dataGridView1.Height = this.ClientSize.Height - 110;
            dataGridView1.Visible = false;

            // disable CellValueChanged event while populating the cells
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;

            // row, columns settings
            dataGridView1.RowTemplate.Height = 35;
            //dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 35;

            dataGridView1.ColumnCount = 3;
            // more row, columns settings
            dataGridView1.Columns[0].HeaderText = "No.";
            dataGridView1.Columns[1].HeaderText = "Pāḷi Text";
            dataGridView1.Columns[2].HeaderText = "Plain Text";
            dataGridView1.Columns[0].HeaderCell.Style.Font = fontROM;
            dataGridView1.Columns[1].HeaderCell.Style.Font = fontROM;
            dataGridView1.Columns[2].HeaderCell.Style.Font = fontROM;

            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = (dataGridView1.Width - dataGridView1.Columns[0].Width) / 2; ;
            dataGridView1.Columns[2].Width = dataGridView1.Columns[1].Width;

            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Brown;
            dataGridView1.Columns[2].DefaultCellStyle.ForeColor = Color.DimGray;
            dataGridView1.Columns[1].DefaultCellStyle.Font = fontMYA;
            dataGridView1.Columns[2].DefaultCellStyle.Font = fontMYA;

            dataGridView1.Columns[0].ReadOnly = true;

            // layout
            dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            // Set up the DataGridView.
            //dataGridView1.Dock = DockStyle.Fill;
            // Set the DataGridView control's border.
            //dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            // Put the cells in edit mode when user enters them.
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

            dataGridValue1_Updated = false;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            richTextBox1_Updated = false;
        }

        private void adjustTextColor(string newText)
        {
            int pos = newText.IndexOfAny(textMarkers);
            if (pos == -1) return;

            while (pos != -1)
            {
                richTextBox1.SelectionStart = indexCurCursor + pos;
                richTextBox1.SelectionLength = newText.Length - pos;
                richTextBox1.SelectionColor = DataInfo.GetColor(newText[pos]);
                pos = newText.IndexOfAny(textMarkers, pos + 1);
            }
            richTextBox1.SelectionStart = indexCurCursor + newText.Length;
            richTextBox1.SelectionLength = 0;
            return;
        }

        private void resetAll()
        {
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            if (tabControl1.TabCount > 1)
            {
                nissayaTabPageList.RemoveAt(tabControl1.SelectedIndex);
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                fileCount = tabControl1.TabCount;
            }
            else
            {
                ntp.Reset();
                pageMenuBar.FillPageMenu(new List<string>());
                this.Text = title;
            }
        }

        // check if the current tabPage is new and has data content
        private bool IsNewPageWithData()
        {
            if (pageMenuBar.Rows.Count == 1)
            {
                NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
                if (ntp.IsNewPage())
                {
                    MessageBox.Show(SaveDataToFile);
                    return true;
                }
            }
            return false;
        }
        private void Form1_TabSelectedIndexChanged(object sender, EventArgs e)
        {
            if (curNTP != null) curNTP.EnablePage(true);
            curNTP = nissayaTabPageList[tabControl1.SelectedIndex];
            if (curNTP.NewAndEmpty())
            {
                // create a new page list
                curNTP.curViewCode = this.curViewCode;
                pageMenuBar.FillPageMenu(new List<string>());
                //button_X_Click(this, null);
                textBox_MN.Text = label_MNTitle.Text = string.Empty;
                return;
            }
            //curNTP.getMatchesAccordingToView();
            //curNTP.setViewFindText();
            this.Text = (curNTP.FileName.Length > 0) ? title + space5 + curNTP.FileName : title;
            
            if (this.curViewCode == ViewTabular && curNTP.GetDataGridView().Rows.Count <= 1)
            {
                curNTP.curViewCode = 0;  // trick code to force refresh
                curNTP.RefreshDataGridView();
            }
             
            if (this.curViewCode != ViewTabular && curNTP.GetRichTextBox().Text.Length == 0)
            {
                curNTP.curViewCode = 0;  // trick code to force refresh
                curNTP.RefreshRichTextBox(this.curViewCode);
            }
            
            if (this.curViewCode != curNTP.curViewCode)
            {
                if (this.curViewCode == ViewTabular)
                    curNTP.RefreshDataGridView();
                else curNTP.RefreshRichTextBox(this.curViewCode);
            }
            if (curNTP.MN_No > 0)
            {
                textBox_MN.Text = curNTP.MN_No.ToString();
                label_MNTitle.Text = MN_Titles[curNTP.MN_No];
            }
            else textBox_MN.Text = label_MNTitle.Text = string.Empty;
            // fill pagemenubar
            curNTP.EnablePage(true);
            List<string> listPages = curNTP.GetPageNo();
            string t = curNTP.GetCurrentPage();
            //button_X_Click(this, null);
            int n = listPages.Count;
            pageMenuBar.FillPageMenu(listPages, curNTP.GetCurrentPage());
            //MessageBox.Show("PageCount = " + listPages.Count.ToString() + " curPageNo = " + curNTP.GetCurrentPage());
            curNTP.curPageNo = pageMenuBar.GetCurrentPageNo();
            curNTP.CheckFindData();
            //curNTP.getMatchesAccordingToView();
            //curNTP.setViewFindText();
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // if the current tab has unsaved data, prompt the user
            if (IsNewPageWithData()) return;
            // add new tab page to tabControl1
            TabPage tp = new TabPage();
            //tp.Text = "<New" + (++newIndex).ToString() + ">";    
            tabControl1.TabPages.Add(tp);
            NissayaTabPage ntabPage = new NissayaTabPage(this, tabControl1, tp, textBox1);
            nissayaTabPageList.Add(ntabPage);
            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
            ntabPage.InitView(curViewCode);
        }

        private void bookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NissayaBookContent nbc = new NissayaBookContent();
            nbc.ShowDialog();
            if (nbc.FlagUpdate)
            {
                NissayaBook nbk = nbc.GetNissayaBook();
                dictNissaya.Add(nbc.GetFileName(), nbk);
            }
        }

        public void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = handleMenuShortCuts(sender, e);
            return;
        }

        // https://stackoverflow.com/questions/69761/how-to-associate-a-file-extension-to-the-current-executable-in-c-sharp
        //string appPath = "";
        private void Create_NBK_FileAssociation()
        {
            /***********************************/
            /**** Key1: Create ".nbk" entry ****/
            /***********************************/
            Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            key1.CreateSubKey("Classes");
            key1 = key1.OpenSubKey("Classes", true);

            key1.CreateSubKey(".nbk");
            key1 = key1.OpenSubKey(".nbk", true);
            key1.SetValue("", "DemoKeyValue"); // Set default key value

            key1.Close();

            /*******************************************************/
            /**** Key2: Create "DemoKeyValue\DefaultIcon" entry ****/
            /*******************************************************/
            Microsoft.Win32.RegistryKey key2 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            key2.CreateSubKey("Classes");
            key2 = key2.OpenSubKey("Classes", true);

            key2.CreateSubKey("DemoKeyValue");
            key2 = key2.OpenSubKey("DemoKeyValue", true);

            key2.CreateSubKey("DefaultIcon");
            key2 = key2.OpenSubKey("DefaultIcon", true);
            key2.SetValue("", "\"" + "(The icon path you desire)" + "\""); // Set default key value

            key2.Close();

            /**************************************************************/
            /**** Key3: Create "DemoKeyValue\shell\open\command" entry ****/
            /**************************************************************/
            Microsoft.Win32.RegistryKey key3 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            key3.CreateSubKey("Classes");
            key3 = key3.OpenSubKey("Classes", true);

            key3.CreateSubKey("DemoKeyValue");
            key3 = key3.OpenSubKey("DemoKeyValue", true);

            key3.CreateSubKey("shell");
            key3 = key3.OpenSubKey("shell", true);

            key3.CreateSubKey("open");
            key3 = key3.OpenSubKey("open", true);

            key3.CreateSubKey("command");
            key3 = key3.OpenSubKey("command", true);
            key3.SetValue("", "\"" + "(The application path you desire)" + "\"" + " \"%1\""); // Set default key value

            key3.Close();
        }
        public Boolean flagFindTextBoxClicked = false;
        private void TextBox_Click(object sender, System.EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            flagFindTextBoxClicked = true;
        }

        private void HotKey(object sender, EventArgs e)
        {
            ControlCollection cc = (System.Windows.Forms.Form.ControlCollection)this.Controls;
            //foreach (Control c in cc)
            //{
            //    Type type = c.GetType();
            //    //if (type.Name == "TextBox" && c.Name == "textBox1")
            //    //{
            //    //    c.Location = new Point(c.Location.X + 50, c.Location.Y);
            //    //}
            //    string name = c.Name;
            //}
            //var control = this.ActiveControl;
            curNTP = nissayaTabPageList[tabControl1.SelectedIndex];
            Button btn = (Button) sender;
            Clipboard.SetText(btn.Text);

            if (flagFindTextBoxClicked)
            {
                // text is inserted in Find Text Box
                int nCurSelectionStart = textBox_Find.SelectionStart;
                textBox_Find.Text = textBox_Find.Text.Insert(textBox_Find.SelectionStart, btn.Text);
                textBox_Find.SelectionStart = nCurSelectionStart + btn.Text.Length;
                textBox_Find.SelectionLength = 0;
                textBox_Find.ScrollToCaret();
                textBox_Find.Focus();
                return;
            }
            switch (curNTP.curViewCode)
            {
                case ViewFullDoc:
                    if (curNTP.GetRichTextBox().Enabled)
                    {
                        curNTP.richTextBox1_InsertText(richTextBox1, btn.Text);
                    }
                    curNTP.GetRichTextBox().Focus();
                    break;
                case ViewTabular:
                    if (curNTP.GetDataGridView().Enabled)
                    {
                        if (!curNTP.dataBox.Visible)
                        {
                            MessageBox.Show("Hotkeys work only inside the databox. Open the databox first.");
                            return;
                        }
                        curNTP.GetDataGridView().InsertText();
                    }
                    break;
            }
        }

        private void MN_TextChanged(object sender, EventArgs e)
        {
            //if (curNTP != null) curNTP.richTextBox1_Updated = true;
        }

        private void mNCatalogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // to open the MN Catalog dialog box
            if (mnCatalog == null)
            {
                mnCatalog = new MN_Catalog(this);
                mnCatalog.OnOpen += MNCatalogFileOpenEvent;
            }
            mnCatalog.ShowDialog();
        }

        void MNCatalogFileOpenEvent(object sender, EventArgs e)
        {
            // if there is an MN_No do it.
            int mnNo = Convert.ToInt16(((MNOpenEventArgs)e).MN_No);
            if (mnNo > 0)
            {
                label_MNTitle.Text = ((MNOpenEventArgs)e).MNTitle;
                textBox_MN.Text = ((MNOpenEventArgs)e).MN_No;
                listLoadFiles = mnCatalog.GetFileList(mnNo);
                MNBook_Open = true;
                loadBookFile();
                button_X_Click(this, null);
            }
        }

        private void MN_Leave(object sender, EventArgs e)
        {
            int value;
            if (textBox_MN.Text.Length == 0)
            {
                label_MNTitle.Text = string.Empty; return;
            }
            var isNumeric = int.TryParse(textBox_MN.Text, out value);
            if (isNumeric && value >= 1 && value <= 152) label_MNTitle.Text = MN_Titles[value];
        }

        private void MN_Validating(object sender, CancelEventArgs e)
        {
            Boolean errorFlag = false;
            if (textBox_MN.Text.Length == 0) return;
            string MN_No = string.Empty;
            foreach (char c in textBox_MN.Text)
            {
                if (c < '0' || (c > '9' && c < '\u1040') || c > '\u1049')
                {
                    e.Cancel = true;
                    textBox_MN.Select(0, textBox1.Text.Length);
                    errorFlag = true;
                    break;
                }
                else
                {
                    if (c >= '\u1040' && c <= '\u1049')
                    {
                        MN_No += ((int)c - (int)'၀').ToString();
                    }
                    else MN_No += c;
                }
            }
            if (!errorFlag)
            {
                int n = Convert.ToInt16(MN_No);
                if (n < 1 || n > 152)
                {
                    e.Cancel = true; errorFlag = true;
                    textBox_MN.Select(0, textBox1.Text.Length);
                }
                else textBox_MN.Text = MN_No;
            }
            if (errorFlag)
            {
                MessageBox.Show("MN number should be between 1 and 152.");
                textBox_MN.Text = string.Empty;
            }
        }
        // Declares the event handler DrawOnTab which is a method that
        // draws a string and Rectangle on the tabPage1 tab.
        private void DrawOnTab(object sender, DrawItemEventArgs e)
        {
            TabControl tb = (TabControl)sender;
            TabPage tp = tb.TabPages[0];
            //e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
            //e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            //e.DrawFocusRectangle();
        }

        static string MyanmarCharsCleanup(string s)
        {
            const char thaythayTin = '\u1036';
            const char thagyi = '\u103F';
            const char nyakalay = '\u1009';
            const char yapin = '\u103B';
            const char yayit = '\u103C';
            const char virama = '\u1039';
            const char Asat = '\u103A';
            const char oughtMyit = '\u1037';
            const char naughtPyin = '\u1032';
            const char waswae = '\u103D';
            const char huthoe = '\u102F';
            const char vassa2lonepauk = '\u1038';

            char[] Consonants = { 'က', 'ခ', 'ဂ', 'ဃ', 'င', 'စ', 'ဆ', 'ဇ', 'ဈ', 'ည', 'ဋ', 'ဌ', 'ဍ', 'ဎ', 'ဏ',
                                    'တ', 'ထ', 'ဒ', 'ဓ', 'န', 'ပ', 'ဖ', 'ဗ', 'ဘ', 'မ', 'ယ', 'ရ', 'လ', 'ဝ', 'သ',
                                    'ဟ', 'ဠ','အ', thaythayTin, thagyi, nyakalay};
            char[] ReducedConjCons = { '\u103D', '\u103E', '\u103B', '\u103C' };
            char[] Vowels = { '\u102C', '\u102B', '\u102D', '\u102E', '\u102F', '\u1030', '\u1031', 
                                    '\u1032', '\u1036', '\u1037', '\u1038'};
            char[] consAssociates = { '\u102C', '\u102B', '\u102D', '\u102E', '\u102F', '\u1030', '\u1031',
                                    '\u1032', '\u1036', '\u1037', '\u1038',
                                // vowel killer
                                Asat, virama, 
                                // tone
                                thaythayTin, oughtMyit, naughtPyin, vassa2lonepauk,
                                // semi-vowels
                                yapin, yayit, huthoe, waswae
                                };
            string pattern = "စျ|ဥ်|ဦ|သြ|သြော်";
            s = s.Replace("စျ", "ဈ");
            s = s.Replace("ဥ်", "ဉ်");
            s = s.Replace("ဦ", "ဦ");
            s = s.Replace("သြ", "ဩ");
            s = s.Replace("သြော်", "ဪ");
            s = s.Replace("ဥ္", "ဉ္");

            // When walone is typed in as zero it replaces with proper walone.
            string mmChars = new string(Consonants) + new string(Vowels) +
                             new string(ReducedConjCons) + new string(consAssociates);
            pattern = "[" + mmChars + "]၀";
            pattern += "|၀[" + mmChars + "]";
            //string s1 = "၏ ၀ိပဿနာ";
            //int pos = s.IndexOf(s1);
            //string pattern = "၀[" + mmChars + "]";
            MatchCollection matches = Regex.Matches(s, pattern);
            foreach (Match m in matches)
            {
                //string rep = m.Value.Replace("၀", "ဝ");
                s = s.Replace(m.Value, m.Value.Replace("၀", "ဝ"));
            }
            return s;
        }

    }
    public class MNOpenEventArgs : EventArgs
    {
        public string MNTitle { get; set; }
        public string MN_No { get; set; }
    }

    public class DebugTracer
    {
        System.IO.StreamWriter file;

        public DebugTracer()
        {
            file = new System.IO.StreamWriter(@".\DebugTracer.out", true, Encoding.UTF8);
            file.AutoFlush = true;
            file.WriteLine("\nTracer started at " + DateTime.Now);
            file.WriteLine("===============================================================");
        }

        public void ElpasedTimes(double miliSeconds1, double miliSeconds2)
        {
            file.WriteLine("Elpased Time1  = \tTicks1 (milliseconds) = " + miliSeconds1.ToString());
            file.WriteLine("Elpased Time2  = \tTicks2 (milliseconds) = " + miliSeconds2.ToString());
        }

        public void OutElpasedTime(string s, double miliSeconds)
        {
            file.WriteLine(s + "\tTicks (milliseconds) = " + miliSeconds.ToString());
        }

        public void OutElpasedTime(TimeSpan ts)
        {
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            file.WriteLine("Elapsed time hh:mm:ss.nnn = " + elapsedTime);
        }

        public void OutputTrace(string[] lines)
        {
            //System.IO.File.WriteAllLines(@".\DebugTrace.out", lines);
        }

        public void WriteLine(string s)
        {
            file.WriteLine(s);
        }

        public void WriteLineItems(string s1, int n1, string s2, int n2, int nTabs, int nTabs2)
        {
            string line = string.Format("{0}={1} {2}={3}, Tabs={4}, Tabs2={5}", s1, n1, s2, n2, nTabs, nTabs2);
            file.WriteLine(line);
        }
    }
}
