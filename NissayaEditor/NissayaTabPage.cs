using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {

        public partial class NissayaTabPage //: TabPage
        {

            public TabControl parentTabControl;
            public TabPage tabPage;
            public bool dataGridView1_Updated = false;
            public bool richTextBox1_Updated = false;
            public bool fileUpdated = false;
            public string FileName = string.Empty;
            public TextBox dataBox;
            public Font fontMYA;
            public BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            DataInfo dataInfo;
            public Form1 parentForm;
            string findText = string.Empty;
            string findStatus = string.Empty;
            //List<int> listFindIndex = new List<int>();
            public string curPageNo = string.Empty;
            bool dataRefreshThreadUse = false;
            public bool MNBookFile = false;
            // false = not full size because DataBox is visible
            //public class DataBoxInfo
            //{
            //    public Form1 parentForm;
            //    public NissayaTabPage parentTabPage;
            //    public bool menuFlag;
            //    public string text;
            //    public int cellRow;
            //    public int cellCol;
            //    public bool dataBoxVisible;

            //    public DataBoxInfo(Form1 form, NissayaTabPage tb)
            //    {
            //        parentForm = form; parentTabPage = tb;
            //        menuFlag = false; text = string.Empty; cellRow = cellCol = -1;
            //    }
            //    public void SetValues(string text, int row, int col)
            //    {
            //        cellRow = row; cellCol = col; this.text = text;
            //        menuFlag = parentForm.dataBoxCtrlDToolStripMenuItem.Checked;
            //        dataBoxVisible = parentTabPage.dataBox.Visible;
            //    }
            //};
            //public DataBoxInfo dataBoxInfo;

            int indexCurCursor = 0;
            int caretPosition = 0;
            //int MN_no = 0;
            //public int MN_No
            //{
            //    get { return MN_no; }
            //    set { MN_no = value; }
            //}
            public int MN_No { get; set; }
            RichTextBox richTextBox1;//, richTextBox2;
            NissayaDataGridView dataGridView1; //, dataGridView2;
            

            const int ViewPali = 1;
            const int ViewPlain = 2;
            const int ViewFullDoc = 3;
            const int ViewTabular = 4;
            public int curViewCode = ViewFullDoc;

            public bool ValidDataInfo() { return dataInfo != null; }
            public bool richTextBox1_Visisble() { return richTextBox1.Visible; }
            public bool dataGridView1_Visible() { return dataGridView1.Visible; }
            public RichTextBox GetRichTextBox() { return richTextBox1; }
            public NissayaDataGridView GetDataGridView(bool main = true)
            { return dataGridView1; }
            public List<_GridViewSelection> GetGridDataMatches(string pg, string s) { return dataInfo.FindGridPageMatches(pg, s); }
            //public void SetDataContainers2() { dataInfo.SetDataContainers2(richTextBox2, dataGridView2); }
            public void UseSecondaryGridContainer(bool flag = false) { dataInfo.UseSecondaryGridContainer = flag; }
            public bool UsingSecondaryContainer() { return dataInfo.UseSecondaryGridContainer; }
            public void GetPageData(string pgno) { dataInfo.GetPageData(pgno, curViewCode); }
            //public string GetMN() { return dataInfo.MN; }
            public List<_NissayaDataRecord> GetDataList() { return dataInfo.GetDataList(); }
            public bool UpdateNotSaved()
                { return dataGridView1_Updated || richTextBox1_Updated || fileUpdated || dataInfo == null ? false : dataInfo.UpdateFileContent; }
            public string GetCurrentPage() { return curPageNo; }
            
            public List<string> GetPageList() { return dataInfo.GetPageList(); }

            public NissayaTabPage(Form1 pForm, TabControl tc, TabPage tp, TextBox tb)
            {
                parentForm = pForm;
                fontMYA = pForm.fontMYA;
                parentTabControl = tc;
                tabPage = tp;
                dataBox = tb;
                tp.Text = "<New" + tc.TabPages.Count.ToString() + ">";
                //dataBoxInfo = new DataBoxInfo(pForm, this);
                initControls();
            }
            public void InitView(int viewCode)
            {
                curViewCode = viewCode;
                dataGridView1.Visible = (viewCode == ViewTabular);
                dataGridView1.Enabled = (viewCode == ViewTabular);
                dataGridView1.ReadOnly = (viewCode != ViewTabular);
                richTextBox1.Visible = !dataGridView1.Visible;
                richTextBox1.Visible = !dataGridView1.Enabled;
            }
            /*
            public NissayaTabPage(Form1 pForm, TabControl tc, TextBox tb)
            {
                parentForm = pForm;
                fontMYA = pForm.fontMYA;
                parentTabControl = tc;
                dataBox = tb;
                tabPage.Text = "<New" + tc.TabPages.Count.ToString() + ">";
                initControls();
            }

            public NissayaTabPage(Form1 pForm, TabControl tc, TextBox tb, string filename)
            {
                parentForm = pForm;
                fontMYA = pForm.fontMYA;
                parentTabControl = tc;
                dataBox = tb;
                tabPage = new TabPage();
                tc.TabPages.Add(tabPage);
                initControls();
            }
            */
            void initControls()
            {
                Point ulPoint = parentTabControl.DisplayRectangle.Location;
                ulPoint.X += 2;
                ulPoint.Y -= 16;
                Size size = parentTabControl.DisplayRectangle.Size;
                size.Width -= 13;
                size.Height -= 15;

                richTextBox1 = new RichTextBox(); //richTextBox2 = new RichTextBox();
                //richTextBox1.HideSelection = richTextBox2.HideSelection = false;
                //richTextBox1 = new NissayaRichTextBox(this, ulPoint, size);
                dataGridView1 = new NissayaDataGridView(this); dataGridView1.ID = 1;
                //dataGridView2 = new NissayaDataGridView(this); dataGridView2.ID = 2;
                richTextBox1_initialize();
                //richTextBox1.Font = fontMYA;

                if (tabPage != null)
                {
                    tabPage.Controls.Add(richTextBox1);
                    tabPage.Controls.Add(dataGridView1);
                }
                richTextBox1.Enabled = false; // initially the user cannot enter data until New is clicked

                ulPoint = parentTabControl.DisplayRectangle.Location;
                ulPoint.X += 2;
                ulPoint.Y -= 16;
                richTextBox1.Location = ulPoint;
                size = parentTabControl.DisplayRectangle.Size;
                size.Width -= 13;
                size.Height -= 15;
                richTextBox1.Size = size;
                richTextBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

                dataGridView1.Location = ulPoint;
                dataGridView1.Size = size;
                dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

                //dataGridView2.Size = dataGridView1.Size;

                dataGridView1.initialize(); 
                //dataGridView2.initialize();

                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.Select(0, 0);
 
                // create an 'x' button on DataBox
                Button button1 = new Button();
                button1.Font = new Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                button1.Size = new Size(20, 20);
                button1.Text = "x";
                int X = dataBox.ClientSize.Width + dataBox.Width - 120;
                button1.Location = new Point(X, -1);//dataBox.Location.Y + 3);
                button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                button1.Visible = true;
                button1.Cursor = Cursors.Default;
                //button1.Padding = new Padding{Top=1, Bottom=1};
                button1.Click += parentForm.dataBoxCtrlDToolStripMenuItem_Click;
                dataBox.Controls.Add(button1);
                richTextBox1_registerEvents(rtbAllEventsOn);
            }

            public void InitViews()
            {
                //richTextBox1.Visible = true;
                //parentForm.ActiveControl = richTextBox1;
                richTextBox1.Text = "";
                richTextBox1.SelectionStart = 0;
                // initially richTextBox1 will be visible
                richTextBox1.Visible = true;
                richTextBox1.Enabled = false;
                dataGridView1.Visible = !richTextBox1.Visible;
                //dataGridView2.Visible = false;

                // richTextBox2 is used for Find on non-current pages
                //richTextBox2.Visible = false;
                //richTextBox2.Enabled = false;
                //richTextBox2.Size = richTextBox1.Size;

                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = dataGridView1.Visible;
                init_FileLoad_BackgroundWorker();
            }

            public void richTextBox1_registerEvents(int eventFlags)
            {
                richTextBox1.KeyDown -= richTextBox1_KeyDown;
                if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("-= richTextBox1_KeyDown");
                if ((eventFlags & rtbKeyDown) == rtbKeyDown)
                {
                    richTextBox1.KeyDown += richTextBox1_KeyDown;
                    if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("+= richTextBox1_KeyDown");
                }
                richTextBox1.KeyPress -= parentForm.richTextBox1_KeyPress;
                if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("-= richTextBox1_KeyPress");
                if ((eventFlags & rtbKeyPress) == rtbKeyPress)
                {
                    richTextBox1.KeyPress += richTextBox1_KeyPress;
                    if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("+= richTextBox1_KeyPress");
                }
                richTextBox1.TextChanged -= richTextBox1_TextChanged;
                if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("-= richTextBox1_TextChanged");
                if ((eventFlags & rtbTextChanged) == rtbTextChanged)
                {
                    richTextBox1.TextChanged += richTextBox1_TextChanged;
                    if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("+= richTextBox1_TextChanged");
                }
                richTextBox1.MouseClick -= richTextBox1_MouseClick;
                if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("-= richTextBox1_MouseClick");
                if ((eventFlags & rtbMouseClick) == rtbMouseClick)
                {
                    richTextBox1.MouseClick += richTextBox1_MouseClick;
                    if (Form1.dbgTrace != null) Form1.dbgTrace.WriteLine("+= richTextBox1_MouseClick");
                }
            }

            public bool DataFileExist()
            {
                return (dataInfo != null);
            }

            public void SetFile(string fname)
            {
                FileName = fname;
                if (dataInfo == null)
                    dataInfo = new DataInfo(parentForm, parentTabControl, tabPage, richTextBox1, dataGridView1, fname);
                dataInfo.SetFileName(fname);
            }

            public void LoadFile(string fname)
            {
                parentForm.ActiveControl = null;
                if (curViewCode == ViewTabular)
                {
                    //richTextBox1.Enabled = false;
                    richTextBox1.Visible = false;
                    dataGridView1.Visible = true;
                //    ShowDataBox(parentForm.dataBoxCtrlDToolStripMenuItem.Checked);
                }
                //else ShowDataBox(false);
                //if (parentForm.dataBoxCtrlDToolStripMenuItem.Checked && !parentForm.flagTabControlFullSize)
                //    ShowDataBox(false);
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = (curViewCode == ViewTabular);
                //curViewCode = ViewFullDoc;
                FileName = fname;
                dataInfo = new DataInfo(parentForm, parentTabControl, tabPage, richTextBox1, dataGridView1, fname);
                dataInfo.ThreadAction = thdFileLoad;
                parentForm.label_DataStatus.Text = "Loading...";
                parentForm.Cursor = Cursors.WaitCursor;
                richTextBox1.Cursor = Cursors.WaitCursor;
                richTextBox1_registerEvents(rtbAllEventsOff);// rtbTextChanged);
                dataGridView1.RegisterEvents(dgvCellFormatting); //dgvAllEventsOff);// dgvCellFormatting);
                init_FileLoad_BackgroundWorker();
                //bw.RunWorkerAsync(dataInfo);
                backgroundWorker1.RunWorkerAsync(dataInfo);
            }

            public void ShowDataBox()
            {
                //parentForm.dataBoxCtrlDToolStripMenuItem.Checked = !parentForm.dataBoxCtrlDToolStripMenuItem.Checked;
                //ShowDataBox(parentForm.dataBoxCtrlDToolStripMenuItem.Checked);
                //dataBox.ReadOnly = (dataGridView1.CurrentCell.ColumnIndex <= 1);
            }

            public void ShowDataBox(bool flagShow)
            {
                //if (flagShow && !parentForm.flagTabControlFullSize && dataBox.Visible) return;
                //if (!flagShow && parentForm.flagTabControlFullSize && !dataBox.Visible) return;
                //const int dataBoxHeight = 60;
                //switch (flagShow)
                //{
                //    case true:
                //        // to prevent button from moving unanchor the bottom before changing the ht
                //        parentTabControl.Anchor = dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                //        parentTabControl.Height -= dataBoxHeight;
                //        dataGridView1.Height -= dataBoxHeight;
                //        parentTabControl.Anchor = dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                //        Point loc = parentTabControl.Location;
                //        loc.Y += parentTabControl.Height;
                //        dataBox.Location = loc;
                //        dataBox.Width = parentTabControl.Width;
                //        dataBox.Height = dataBoxHeight;
                //        //dataBox.Text = dataGridView1.CurrentCell.FormattedValue.ToString();
                //        dataGridView1.ReadOnly = true;
                //        parentForm.flagTabControlFullSize = false;
                //        break;
                //    case false:
                //        // to prevent button from moving unanchor the bottom before changing the ht
                //        dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                //        parentTabControl.Height += dataBoxHeight;
                //        dataGridView1.Height += dataBoxHeight;
                //        dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                //        dataGridView1.ReadOnly = false;
                //        parentForm.flagTabControlFullSize = true;
                //        break;
                //}
                //dataBox.Visible = flagShow;
                //if (dataBox.Visible) dataGridView1.dataGridView1_CellEnter(null, null);
            }

            public bool NewAndEmpty()
            {
                return tabPage.Text.IndexOf("New") >= 0 && richTextBox1.Text.Length == 0;
            }

            public bool IsNewPage()
            {
                return tabPage.Text.IndexOf("New") >= 0;
            }

            public List<string> GetPageNo()
            {
                List<string> listPgNo = new List<string>();
                if (dataInfo != null)
                {
                    foreach (DataInfo.KeyMap keymap in dataInfo.KeyMapper)
                        listPgNo.Add(keymap.curKey);
                }
                return listPgNo;
            }

            public void LoadPage(string pgno)
            {
                Control t = (Control)richTextBox1;
                long handle = (long)t.Handle;
                dataInfo.UpdateFileContent = richTextBox1_Updated | dataGridView1_Updated;
                //dataInfo.UpdateFileContent = unthreadedDataRefresh();
                curPageNo = pgno;
                dataInfo.PageToLoad = pgno;
                dataGridView1.SetCurrentPageNo(curPageNo);
                parentForm.ActiveControl = null;
                richTextBox1.Enabled = false;
                dataGridView1.Enabled = false;
                //richTextBox1.Visible = true;
                //dataGridView1.Visible = false;
                //if (parentForm.dataBoxCtrlDToolStripMenuItem.Checked && parentForm.flagTabControlFullSize)
                //    ShowDataBox(true);
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = curViewCode == ViewTabular;
                //curViewCode = ViewFullDoc;
                
                dataInfo.ThreadAction = thdPageLoad;
                parentForm.label_DataStatus.Text = "Loading...";
                parentForm.Cursor = Cursors.WaitCursor;
                richTextBox1.Cursor = Cursors.WaitCursor;
                richTextBox1_registerEvents(rtbAllEventsOff);// rtbTextChanged);
                dataGridView1.RegisterEvents(dgvCellFormatting); //dgvAllEventsOff);// dgvCellFormatting);
                richTextBox1.Text = string.Empty;
                
                // check page last view info
                //if (curViewCode == ViewTabular) SetPageView(ViewTabular);
                //else curViewCode = GetPageView();

                dataRefreshThreadUse = false;
                if (dataRefreshThreadUse)
                {
                    richTextBox1.Text = string.Empty;
                    init_FileLoad_BackgroundWorker();
                    //bw.RunWorkerAsync(dataInfo);
                    bool flag = backgroundWorker1.IsBusy;
                    backgroundWorker1.RunWorkerAsync(dataInfo);
                }
                else
                {
                    dataInfo.LoadPageData(curViewCode);
                    CheckFindData();
                    normalStatus();
                }
                dataRefreshThreadUse = false;
            }

            public void EnablePage(bool flag)
            {
                richTextBox1.Enabled = dataGridView1.Enabled = flag;
            }

            public void NewPage(string pgno)
             {
                 // check if current page is dirty, if so save it
                 curPageNo = pgno;
                 if (richTextBox1.Visible)
                 {
                     pgno = "#" + pgno;
                     richTextBox1.Text = pgno;
                     adjustTextColor(pgno);
                     richTextBox1_Updated = true;
                     richTextBox1.Enabled = true;
                     curViewCode = ViewFullDoc;
                 }
                 if (dataGridView1.Visible)
                 {
                     dataGridView1.Rows.Clear();
                     //while (dataGridView1.Rows.Count > 1)
                     //    dataGridView1.Rows.RemoveAt(0);
                     if (dataGridView1.Rows.Count == 1 && dataGridView1.Rows[0].IsNewRow)
                     {
                         dataGridView1.Rows.Add();
                         dataGridView1.Rows[0].Cells[0].Value = "1";
                         dataGridView1.Rows[0].Cells[1].Value = "#";
                         dataGridView1.Rows[0].Cells[2].Value = pgno;
                         if (dataGridView1.Columns[4].Visible) dataGridView1.Rows[0].Cells[4].Value = string.Empty;
                     }
                     //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = dataGridView1.Enabled = true;
                 }
             }

            public void Reset()
            {
                richTextBox1.Text = string.Empty;
                richTextBox1_registerEvents(rtbAllEventsOff);
                dataGridView1.RegisterEvents(dgvAllEventsOff);
                if (dataInfo != null) dataInfo.ClearMemory();
                tabPage.Text = "<New1>";
                dataGridView1_Updated = false;
                richTextBox1_Updated = false;
                fileUpdated = false;
                FileName = string.Empty;
                findText = string.Empty;
                findStatus = string.Empty;
                //listFindIndex.Clear();
                indexCurCursor = 0;
                caretPosition = 0;
                richTextBox1_registerEvents(rtbAllEventsOn);
                dataGridView1.RegisterEvents(dgvAllEventsOn);
            }

            private void tabIndexChanged(object sender, EventArgs e)  // unused
            {
                parentForm.Text = title + space5 + FileName;
            }

            public void TableViewToggle(object sender, EventArgs e)
            {
                if (curViewCode == ViewTabular) return;     // already in Tabular View
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = (dataInfo != null);
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = dataGridView1.Enabled;
                //dataBox.Visible = (dataGridView1.Visible && parentForm.dataBoxCtrlDToolStripMenuItem.Checked);
                parentForm.ActiveControl = dataGridView1;
                if (dataInfo == null)
                {
                    curViewCode = ViewTabular;
                    dataGridView1.Visible = true;
                    richTextBox1.Visible = false;
                    //if (parentForm.dataBoxCtrlDToolStripMenuItem.Checked && parentForm.flagTabControlFullSize)
                    //    ShowDataBox(parentForm.dataBoxCtrlDToolStripMenuItem.Checked);
                    return;
                }
                RefreshDataGridView();
            }

            public void SaveFile()
            {
                if (dataInfo == null) return;
                unthreadedDataRefresh();
                dataInfo.MN = ((Form1)parentTabControl.Parent).textBox_MN.Text;
                fileUpdated = false;    // file just saved
                dataInfo.SaveFile();
            }

            public void SavePage()
            {
                if (dataInfo == null) return;

                if (unthreadedDataRefresh())
                {
                    if (curPageNo.Length > 0)
                    {
                        string newKey = dataInfo.SaveCurrentPage(curPageNo);
                        if (newKey != curPageNo)
                        {
                            parentForm.pageMenuBar.UpdateKey(newKey, curPageNo);
                            curPageNo = newKey;
                        }
                    }
                }
            }

            private void savePageThread()
            {
                dataInfo.UpdateFileContent = richTextBox1_Updated | dataGridView1_Updated;
                unthreadedDataRefresh();
                parentForm.ActiveControl = null;
                richTextBox1.Enabled = false;
                dataGridView1.Enabled = false;
                //if (parentForm.dataBoxCtrlDToolStripMenuItem.Checked && !parentForm.flagTabControlFullSize)
                //    ShowDataBox(false);
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = false;

                dataInfo.ThreadAction = thdSavePage;
                parentForm.label_DataStatus.Text = "Saving...";
                parentForm.Cursor = Cursors.WaitCursor;
                richTextBox1.Cursor = Cursors.WaitCursor;
                richTextBox1_registerEvents(rtbAllEventsOff);
                dataGridView1.RegisterEvents(dgvAllEventsOff);
                richTextBox1.Text = string.Empty;
                init_FileLoad_BackgroundWorker();
                //bw.RunWorkerAsync(dataInfo);
                backgroundWorker1.RunWorkerAsync(dataInfo);
            }

            public void RefreshDataGridView()
            {
                if (curViewCode == ViewTabular && !richTextBox1_Updated) return;
                dataGridView1.Visible = true;
                richTextBox1.Visible = false;
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = true;
                //if (parentForm.dataBoxCtrlDToolStripMenuItem.Checked && parentForm.flagTabControlFullSize)
                //    ShowDataBox(parentForm.dataBoxCtrlDToolStripMenuItem.Checked);
                //parentForm.FindClear();
                parentForm.label_DataStatus.Text = "Refreshing...";
                //if (!dataRefreshThreadUse) Thread.Sleep(10);
                dataInfo.ThreadAction = thdRefreshDataGridView;
                dataInfo.UpdateFileContent = richTextBox1_Updated;
                parentForm.Cursor = Cursors.WaitCursor;
                richTextBox1.Cursor = Cursors.WaitCursor;
                dataGridView1.RegisterEvents(dgvCellFormatting);//dgvAllEventsOff);
                //init_FileLoad_BackgroundWorker();
                dataGridView1.SetCurrentPageNo(curPageNo);
                //SetPageView(ViewTabular);
                if (!dataRefreshThreadUse)
                {
                    SavePage();
                    dataInfo.RefreshDataGridView();
                }
                else
                {
                    dataRefreshThreadUse = false;
                    backgroundWorker1.RunWorkerAsync(dataInfo);
                }
                normalStatus();
                curViewCode = ViewTabular;
                CheckFindData();
                //dataGridView1.DataGridViewFindClear();     // clear dataGridView find info
            }

            public void RefreshRichTextBox(int viewCode)
            {
                if (dataInfo != null && (curViewCode == viewCode && !dataGridView1_Updated)) return;
                dataGridView1.Visible = false;
                dataBox.Visible = false;
                //parentForm.dataBoxCtrlDToolStripMenuItem.Enabled = false;
                richTextBox1.Visible = true;
                //if (parentForm.dataBoxCtrlDToolStripMenuItem.Checked && !parentForm.flagTabControlFullSize)
                //    ShowDataBox(false);
                //parentForm.FindClear();
                curViewCode = viewCode;
                //SetPageView(viewCode);

                if (dataInfo == null) return;
                parentForm.ActiveControl = null;
                parentForm.label_DataStatus.Text = "Refreshing...";
                if (dataRefreshThreadUse) //Thread.Sleep(10);
                //else
                {
                    dataInfo.ThreadAction = thdRefreshRichTextBox;
                    dataInfo.UpdateFileContent = dataGridView1_Updated;
                    dataInfo.ViewCode = viewCode;
                }
                parentForm.Cursor = Cursors.WaitCursor;
                richTextBox1.Cursor = Cursors.WaitCursor;
                richTextBox1_registerEvents(rtbAllEventsOff);
                //init_FileLoad_BackgroundWorker();
                if (!dataRefreshThreadUse)
                {
                    SavePage();
                    richTextBox1.Text = string.Empty;
                    richTextBox1_registerEvents(rtbAllEventsOff);
                    dataInfo.RefreshRichTextBox(viewCode);
                    richTextBox1_registerEvents(rtbAllEventsOn);
                    richTextBox1.ReadOnly = (viewCode < Form1.ViewFullDoc);
                }
                else
                {
                    dataRefreshThreadUse = false;
                    richTextBox1.Text = string.Empty;
                    backgroundWorker1.RunWorkerAsync(dataInfo);
                }
                normalStatus();
                richTextBox1.SelectionStart = 0;
                //richTextBoxFindClear();
                //if (viewCode >= ViewFullDoc) 
                    CheckFindData();
                parentForm.ActiveControl = richTextBox1;
            }

            private bool unthreadedDataRefresh()
            {
                bool savePage = true;
                if (dataInfo != null)
                {
                    if (dataGridView1_Updated)
                    {
                        dataGridView1.RegisterEvents(dgvAllEventsOff);
                        dataInfo.DataGridViewToFileContent();
                        dataGridView1.RegisterEvents(dgvAllEventsOn);
                    }
                    if (richTextBox1_Updated) dataInfo.RichTextBoxToFileContent();
                }
                savePage = dataGridView1_Updated | richTextBox1_Updated;
                dataGridView1_Updated = false;
                richTextBox1_Updated = false;
                return savePage;
            }

            private bool threadedDataRefresh() // unused
            {
                bool savePage = true;
                if (dataInfo != null)
                {
                    if (dataGridView1_Updated) RefreshDataGridView();// dataInfo.DataGridViewToFileContent();
                    if (richTextBox1_Updated) RefreshRichTextBox(0); // dataInfo.RichTextBoxToFileContent();
                }
                savePage = dataGridView1_Updated | richTextBox1_Updated;
                dataGridView1_Updated = false;
                richTextBox1_Updated = false;
                return savePage;
            }

            public bool IsDataReadOnly() { return curViewCode < ViewFullDoc; }
            
            public void ClearDataGridView() { dataInfo.clearDataGridView(); }

            public bool FindNext(bool dirFlag = true)
            {
                if (curViewCode == ViewTabular)
                    dataGridView1.dataGridViewFindNext(dirFlag);
                else
                    return richTextBoxFindNext(dirFlag);

                return true;
            }

            public Color GetNTPColor(char marker)
            {
                Color color = Color.Black;
                //if (color == Color.Black) return Color.Silver;
                switch (marker)
                {
                    case '*':
                        color = Color.Brown;
                        break;
                    case '#':
                        color = Color.RoyalBlue;
                        break;
                    case '@':
                        color = Color.FromArgb(160, 125, 40); //227, 184, 74);
                        //color = Color.LightSalmon;
                        break;
                    case '^':
                    default:
                        color = Color.DimGray; //FromArgb(88, 88, 88); //DimGray;
                        break;
                }
                return color;
            }
        }
    }
}
