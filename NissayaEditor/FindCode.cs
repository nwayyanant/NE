using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace NissayaEditor
{
    public partial class Form1 : Form
	{

        //*** New Find code
        public class ViewFindStatus
        {
            public string TotalFindStatus = "0/0";//string.Empty;
            public int curFindIndex = 0;
            public int TotalFindCount = 0;
            //public Dictionary<string, _FindMatchingInfo> DictFindMatches = new Dictionary<string, _FindMatchingInfo>();

            public void UpdateTotalFindCount(int addCount)
            {
                TotalFindCount += addCount;
                TotalFindStatus = curFindIndex.ToString() + "/" + TotalFindCount.ToString();
            }
            public void UpdateTotalFindIndex(int n)
            {
                curFindIndex += n;
                TotalFindStatus = curFindIndex.ToString() + "/" + TotalFindCount.ToString();
            }
            public void SetTotalFindIndex(int n)
            {
                curFindIndex = n;
                TotalFindStatus = curFindIndex.ToString() + "/" + TotalFindCount.ToString();
            }
            public void Clear()
            {
                TotalFindStatus = "0/0";
                curFindIndex = 0;
                TotalFindCount = 0;
            }
        }

        static Dictionary<string, PageFindInfo> PageFindResults_Pali = new Dictionary<string, PageFindInfo>();
        static Dictionary<string, PageFindInfo> PageFindResults_Plain = new Dictionary<string, PageFindInfo>();
        static Dictionary<string, PageFindInfo> PageFindResults_Full = new Dictionary<string, PageFindInfo>();
        static Dictionary<string, DataGridViewFindPage> DataGridViewFindPageList = new Dictionary<string, DataGridViewFindPage>();

        public ViewFindStatus MainViewFindStatus_Full = new ViewFindStatus();
        public ViewFindStatus MainViewFindStatus_Pali = new ViewFindStatus();
        public ViewFindStatus MainViewFindStatus_Plain = new ViewFindStatus();
        public ViewFindStatus MainViewFindStatus_Tabular = new ViewFindStatus();

        public PageFindInfo CurActivePageFindInfo = null;
        public HightlightType CurPageHighlightType = HightlightType.DoNothing;
        public ViewFindStatus GetCurViewFindStatus()
        {
            ViewFindStatus MainViewFindStatus = null;
            switch(curViewCode)
            {
                case ViewFullDoc:
                    MainViewFindStatus = MainViewFindStatus_Full;
                    break;
                case ViewPali:
                    MainViewFindStatus = MainViewFindStatus_Pali;
                    break;
                case ViewPlain:
                    MainViewFindStatus = MainViewFindStatus_Plain;
                    break;
                case ViewTabular:
                    MainViewFindStatus = MainViewFindStatus_Tabular;
                    break;
            }
            return MainViewFindStatus;
        }

        public int GetPageFindResultsViewCount()
        {
            int n = 0;
            switch (curViewCode)
            {
                case ViewFullDoc:
                    n = PageFindResults_Full.Count;
                    break;
                case ViewPali:
                    n = PageFindResults_Pali.Count;
                    break;
                case ViewPlain:
                    n = PageFindResults_Plain.Count;
                    break;
            }
            return n;
        }

        public void Add2ViewFindMatchesList(PageFindInfo pageFindInfo)
        {
            switch (curViewCode)
            {
                case ViewFullDoc:
                    PageFindResults_Full.Remove(pageFindInfo.key);
                    PageFindResults_Full.Add(pageFindInfo.key, pageFindInfo);
                    break;
                case ViewPali:
                    PageFindResults_Pali.Remove(pageFindInfo.key);
                    PageFindResults_Pali.Add(pageFindInfo.key, pageFindInfo);
                    break;
                case ViewPlain:
                    PageFindResults_Plain.Remove(pageFindInfo.key);
                    PageFindResults_Plain.Add(pageFindInfo.key, pageFindInfo);
                    break;
            }
        }

        public void ClearPagesFindResults() //bool curPageFlagOnly = false, string pgno = "")
        {
            CurActivePageFindInfo = null;
            //Color color;
            /*
            NissayaTabPage curNTP = nissayaTabPageList[tabControl1.SelectedIndex];
            curNTP.curFindPgNo = curNTP.curPageNo;
            string key = curNTP.GetThisTabPageKey();
            if (DataGridViewFindPageList.ContainsKey(key))
                gridFindInfo = DataGridViewFindPageList[key];
            if (gridFindInfo != null)
            {
                //NissayaDataGridView ndgv = CurActiveGridPageFindInfo.nissayaDataGridView;
                NissayaDataGridView ndgv = curNTP.GetDataGridView();
                ndgv.ClearSelection();
                foreach (_GridViewSelection g in gridFindInfo.listSelection)
                    ndgv[g.col, g.row].Style.BackColor = ndgv[g.col - 1, g.row].Style.BackColor;
            }
            */
            if (curViewCode == ViewTabular)
            {
                DataGridViewFindPage gridFindInfo = null;
                List<string> ClearedDataGridView = new List<string>();
                // clear the 
                NissayaTabPage curNTP = nissayaTabPageList[tabControl1.SelectedIndex];
                CurActiveGridPageFindInfo = null;
                curNTP.curFindPgNo = curNTP.curPageNo;
                string key = curNTP.GetThisTabPageKey();
                if (DataGridViewFindPageList.ContainsKey(key))
                {
                    gridFindInfo = DataGridViewFindPageList[key];
                    DataGridViewFindPageList.Remove(key);
                    ClearGridViewHighLights(gridFindInfo);
                    ClearedDataGridView.Add(gridFindInfo.nissayaTabPage.FileName);
                }
                foreach (KeyValuePair<string, DataGridViewFindPage> findPage in DataGridViewFindPageList)
                {
                    gridFindInfo = findPage.Value;
                    if (!ClearedDataGridView.Contains(gridFindInfo.nissayaTabPage.FileName))
                    {
                        ClearedDataGridView.Add(gridFindInfo.nissayaTabPage.FileName);
                        ClearGridViewHighLights(gridFindInfo);
                        /*
                        NissayaDataGridView ndgv = gridFindInfo.nissayaTabPage.GetDataGridView();
                        ndgv.ClearSelection();
                        foreach (_GridViewSelection g in gridFindInfo.listSelection)
                            ndgv[g.col, g.row].Style.BackColor = ndgv[g.col - 1, g.row].Style.BackColor;
                         * */
                    }
                }
            }
            else
            {
                if (PageFindResults_Full.Count > 0)
                {
                    List<string> ClearedRichTextFindInfo = new List<string>();
                    NissayaTabPage curNTP = nissayaTabPageList[tabControl1.SelectedIndex];
                    curNTP.curFindPgNo = curNTP.curPageNo;
                    string key = curNTP.GetThisTabPageKey();
                    PageFindInfo pgFindInfo = null;
                    if (PageFindResults_Full.ContainsKey(key))
                    {
                        pgFindInfo = PageFindResults_Full[key];
                        PageFindResults_Full.Remove(key);
                        RichTextBox rtb = pgFindInfo.nissayaTabPage.GetRichTextBox();
                        rtb.DeselectAll();
                        ClearRichTextBoxHighLights(pgFindInfo);
                        ClearedRichTextFindInfo.Add(pgFindInfo.nissayaTabPage.FileName);
                    }
                    foreach (KeyValuePair<string, PageFindInfo> findPage in PageFindResults_Full)
                    {
                        pgFindInfo = findPage.Value;
                        if (!ClearedRichTextFindInfo.Contains(pgFindInfo.nissayaTabPage.FileName))
                        {
                            RichTextBox rtb = pgFindInfo.nissayaTabPage.GetRichTextBox();
                            rtb.DeselectAll();
                            ClearRichTextBoxHighLights(pgFindInfo);
                            ClearedRichTextFindInfo.Add(pgFindInfo.nissayaTabPage.FileName);
                        }
                    }
                }
            }
            DataGridViewFindPageList.Clear();
            PageFindResults_Pali.Clear();
            PageFindResults_Plain.Clear();
            PageFindResults_Full.Clear();
            MainViewFindStatus_Pali.Clear();
            MainViewFindStatus_Plain.Clear();
            MainViewFindStatus_Full.Clear();
            MainViewFindStatus_Tabular.Clear();
        }

        private void ClearGridViewHighLights(DataGridViewFindPage gridFindInfo)
        {
            NissayaDataGridView ndgv = gridFindInfo.nissayaTabPage.GetDataGridView();
            ndgv.RegisterEvents(dgvAllEventsOff);
            ndgv.ClearSelection();
            foreach (_GridViewSelection g in gridFindInfo.listSelection)
            {
                if (g.row < ndgv.Rows.Count)
                    ndgv[g.col, g.row].Style.BackColor = ndgv[g.col - 1, g.row].Style.BackColor;
            }
            ndgv.RegisterEvents(dgvAllEventsOn);
        }

        private void ClearRichTextBoxHighLights(PageFindInfo pgFindInfo)
        {
            RichTextBox rtb = pgFindInfo.nissayaTabPage.GetRichTextBox();
            pgFindInfo.nissayaTabPage.richTextBox1_registerEvents(rtbAllEventsOff);
            foreach (Match match in pgFindInfo.matches)
            {
                if (match.Index + match.Length < rtb.Text.Length)
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionBackColor = Color.White;
                }
            }
            pgFindInfo.nissayaTabPage.richTextBox1_registerEvents(rtbAllEventsOn);
        }

        public enum HightlightType { DoNothing = 0, HighlightText = 2, HighlightSelectedText = 4, HighlightCurrentSelectedText = 8 };

        public class PageFindInfo
        {
            public string key;
            public NissayaTabPage nissayaTabPage;
            //public int nIndexFindPageResults;
            public string pageNo;
            public int viewCode;
            public int nextIncrement;
            public string searchText;
            public MatchCollection matches;
            public int nCurPageFindIndex;
            public HightlightType typeHighlight = HightlightType.DoNothing;
            public void Clear()
            {
                searchText = string.Empty;
                if (nissayaTabPage != null) nissayaTabPage.ClearFindIndices();
            }
            public PageFindInfo(string k, int vCode)
            {
                nCurPageFindIndex = -1; 
                viewCode = vCode; key = k;
                //flagHighlightDrawn = false;
                searchText = string.Empty;
                nextIncrement = 1;
            }
        }

        static PageFindInfo GetPageFindInfo(string TabPageKey, int curViewCode)
        {
            PageFindInfo pgFindInfo = null;

            switch (curViewCode)
            {
                case ViewPali:
                    if (PageFindResults_Pali.ContainsKey(TabPageKey))
                        pgFindInfo = PageFindResults_Pali[TabPageKey];
                    break;
                case ViewPlain:
                    if (PageFindResults_Plain.ContainsKey(TabPageKey))
                        pgFindInfo = PageFindResults_Plain[TabPageKey];
                    break;
                case ViewFullDoc:
                    if (PageFindResults_Full.ContainsKey(TabPageKey))
                        pgFindInfo = PageFindResults_Full[TabPageKey];
                    break;
            }
            return pgFindInfo;
        }

        public List<PageFindInfo> GetPageInfoListView(int viewCode)
        {
            List<PageFindInfo> l = null;
            switch (viewCode)
            {
                case ViewFullDoc:
                    l = PageFindResults_Full.Values.ToList();
                    break;
                case ViewPali:
                    l = PageFindResults_Pali.Values.ToList();
                    break;
                case ViewPlain:
                    l = PageFindResults_Plain.Values.ToList();
                    break;
            }
            return l;
        }
        
        public void UpdatePageFindInfo(string TabPageKey, PageFindInfo pgFindInfo)
        {
            switch (curViewCode)
            {
                case ViewFullDoc:
                    if (!PageFindResults_Full.ContainsKey(TabPageKey))
                        PageFindResults_Full.Add(TabPageKey, pgFindInfo);
                    else PageFindResults_Full[TabPageKey] = pgFindInfo;
                    break;
                case ViewPali:
                    if (!PageFindResults_Pali.ContainsKey(TabPageKey))
                        PageFindResults_Pali.Add(TabPageKey, pgFindInfo);
                    else PageFindResults_Pali[TabPageKey] = pgFindInfo;
                    break;
                case ViewPlain:
                    if (!PageFindResults_Plain.ContainsKey(TabPageKey))
                        PageFindResults_Plain.Add(TabPageKey, pgFindInfo);
                    else PageFindResults_Plain[TabPageKey] = pgFindInfo;
                    break;
            }
        }

        public string GetFindStatusView()
        {
            return GetCurViewFindStatus().TotalFindStatus;
        }

        public void RefreshFindStatus()
        {
            ViewFindStatus FormViewFindStatus_Temp = GetCurViewFindStatus();

            if (textBox_FindStatus.InvokeRequired)
            {
                var d = new SafeCallSetFindStatus(SetFindStatus);
                textBox_FindStatus.Invoke(d, new object[] { FormViewFindStatus_Temp.TotalFindStatus });
            }
            else
                SetFindStatus(FormViewFindStatus_Temp.TotalFindStatus);
        }

        private delegate void SafeCallSetFindStatus(string s);

        private void SetFindStatus(string s) { textBox_FindStatus.Text = s; }

        public void UpdateTotalFindCountView(int count)
        {
            ViewFindStatus vfs = GetCurViewFindStatus();
            vfs.UpdateTotalFindCount(count);
        }

        public int GetTotalFindCountView()
        {
            return GetCurViewFindStatus().TotalFindCount;
        }

        public void UpdateTotalFindIndexView(int n)
        {
            GetCurViewFindStatus().UpdateTotalFindIndex(n);
        }
        
        public void SetTotalFindIndexView(int n)
        {
            ViewFindStatus vfs = GetCurViewFindStatus();
            if (vfs.TotalFindCount > 0 && n <= vfs.TotalFindCount + 1) vfs.SetTotalFindIndex(n);
        }
        
        private void EnableFindNavButtons()
        {
            // get ViewFindStats
            ViewFindStatus v = GetCurViewFindStatus();
            button_FindPrev.Enabled = v.TotalFindCount > 1; // v.curFindIndex <= 1 ? false : true;
            button_FindNext.Enabled = v.TotalFindCount > 1; // v.curFindIndex >= v.TotalFindCount ? false : true;
        }

        //********************************************************************
        private void SelectFindText()
        {
            int n;

            richTextBox1.TextChanged -= richTextBox1_TextChanged;
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White; // richTextBox1.BackColor;
            richTextBox1.DeselectAll();
            getMatchesAccordingToView();

            if (currentViewMenu == ViewMenu.Tabular)
            {
                if (listFindIndex.Count == 0) textBox_FindStatus.Text = string.Empty;
                else
                {
                    listFindIndex.Clear();
                }
                return;
            }

            if (matches != null && matches.Count > 0)
            {

                foreach (Match match in matches)
                {
                    richTextBox1.Select(match.Index, match.Length);
                    richTextBox1.SelectionBackColor = Color.Yellow;
                }

                n = 0;
                foreach (Match match in matches)
                {
                    if (++n == nListFindIndex)
                    {
                        richTextBox1.Select(match.Index, match.Length);
                        break;
                    }
                }
                textBox_FindStatus.Text = nListFindIndex.ToString() + "/" + matches.Count.ToString();
                richTextBox1.Focus();
                richTextBox1.ScrollToCaret();
            }
            else
            {
                if (matches == null) textBox_FindStatus.Text = string.Empty;
                else textBox_FindStatus.Text = "0/0";
            }
            richTextBox1.TextChanged += richTextBox1_TextChanged;
        }

        private void getMatchesAccordingToView()
        {
            switch (currentViewMenu)
            {
                case ViewMenu.Pali:
                    matches = findInfoPali.matches;
                    nListFindIndex = findInfoPali.index;
                    break;
                case ViewMenu.Plain:
                    matches = findInfoPlain.matches;
                    nListFindIndex = findInfoPlain.index;
                    break;
                case ViewMenu.Full:
                    matches = findInfoFull.matches;
                    nListFindIndex = findInfoFull.index;
                    break;
                case ViewMenu.Tabular:
                    break;
            }
        }

        private void setMatchesAccordingToView()
        {
            switch (currentViewMenu)
            {
                case ViewMenu.Pali:
                    findInfoPali.matches = matches;
                    findInfoPali.index = nListFindIndex;
                    break;
                case ViewMenu.Plain:
                    findInfoPlain.matches = matches;
                    findInfoPlain.index = nListFindIndex;
                    break;
                case ViewMenu.Full:
                    findInfoFull.matches = matches;
                    findInfoFull.index = nListFindIndex;
                    break;
                case ViewMenu.Tabular:
                    break;
            }
        }

        private void richTextBoxFind()
        {
            richTextBoxFindClear();
            Regex regex = new Regex(textBox_Find.Text);
            matches = regex.Matches(richTextBox1.Text);
            richTextBox1.TextChanged -= richTextBox1_TextChanged;
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White; //richTextBox1.BackColor;
            foreach (Match match in matches)
            {
                richTextBox1.Select(match.Index, match.Length);
                richTextBox1.SelectionBackColor = Color.Yellow;
            }
            setMatchesAccordingToView();
            richTextBoxFindNext();
            richTextBox1.TextChanged += richTextBox1_TextChanged;
        }

        private void richTextBoxFindNext(bool flagNext = true)
        {
            getMatchesAccordingToView();
            if (matches == null) return;
            if (matches.Count == 0)
            {
                textBox_FindStatus.Text = "0/0";
                button_FindNext.Enabled = button_FindPrev.Enabled = false;
                nListFindIndex = 0;
                return;
            }

            switch (flagNext)
            {
                case true:
                    if (++nListFindIndex > matches.Count) nListFindIndex = 1;
                    break;
                case false:
                    if (--nListFindIndex < 0) nListFindIndex = 0;
                    break;
            }
            setMatchesAccordingToView();
            textBox_FindStatus.Text = nListFindIndex.ToString() + "/" + matches.Count.ToString();

            if (matches.Count <= 1)
            {
                button_FindNext.Enabled = button_FindPrev.Enabled = false;
            }
            else
            {
                button_FindPrev.Enabled = (nListFindIndex <= 1) ? false : true;
                button_FindNext.Enabled = true;
            }

            int n = 0;
            foreach (Match match in matches)
            {
                if (++n == nListFindIndex)
                {
                    //richTextBox1.SelectedText = match.Value;
                    richTextBox1.Select(match.Index, match.Length);
                    //richTextBox1.SelectionBackColor = Color.Yellow;
                    break;
                }
            }
            richTextBox1.Focus();
        }

        public void FindClear()
        {
            findInfoPlain.Clear();
            textBox_Find.Text = textBox_FindStatus.Text = string.Empty;
        }

        private void richTextBoxFindClear()
        {
            if (findInfoFull.matches != null && findInfoFull.matches.Count > 0 ||
                findInfoPali.matches != null && findInfoPali.matches.Count > 0 ||
                findInfoPlain.matches != null && findInfoPlain.matches.Count > 0)
            {
                richTextBox1.TextChanged -= richTextBox1_TextChanged;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectAll();
                richTextBox1.SelectionBackColor = richTextBox1.BackColor;
                richTextBox1.DeselectAll();
                richTextBox1.TextChanged += richTextBox1_TextChanged;
            }
            findInfoFull.Clear();
            findInfoPali.Clear();
            findInfoPlain.Clear();
        }

        private void dataGridViewFind()
        {
            dataGridViewFindClear();

            // search in the dataGridView
            int rowIndex = -1;
            int colno = -1;
            //dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;// Color.Yellow;// FromArgb(238, 238, 75);
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.DimGray;
            //dataGridView1.CurrentCell.Style.BackColor = Color.FromArgb(51, 153, 255);
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null &&
                    row.Cells[0].Value.ToString().Contains(textBox_Find.Text)) colno = 0;
                if (row.Cells[1].Value != null &&
                    row.Cells[1].Value.ToString().Contains(textBox_Find.Text)) colno = 1;
                if (row.Cells[2].Value != null &&
                    row.Cells[2].Value.ToString().Contains(textBox_Find.Text)) colno = 2;

                if (colno != -1)
                {
                    rowIndex = row.Index;
                    //dataGridView1[colno, rowIndex].Selected = true;
                    dataGridView1[colno, rowIndex].Style.BackColor = Color.Yellow;
                    //dataGridView1[colno, rowIndex];
                    listFindIndex.Add(row.Index);
                    listGridViewSelection.Add(new _GridViewSelection(rowIndex, colno));
                    colno = -1;
                    //break;
                }
            }
            nCurGridFindIndex = -1;
            if (listGridViewSelection.Count > 0) dataGridViewFindNext();
        }

        private void dataGridViewFindNext(bool flagNext = true)
        {
            switch (flagNext)
            {
                case true:
                    if (++nCurGridFindIndex >= listGridViewSelection.Count) nCurGridFindIndex = 0;
                    break;
                case false:
                    if (--nCurGridFindIndex < 0) nCurGridFindIndex = 0;
                    break;
            }
            textBox_FindStatus.Text = (nCurGridFindIndex + 1).ToString() + "/" + listGridViewSelection.Count.ToString();
            if (listGridViewSelection.Count <= 1)
            {
                button_FindNext.Enabled = button_FindPrev.Enabled = false;
            }
            else
            {
                button_FindPrev.Enabled = (nCurGridFindIndex <= 0) ? false : true;
                button_FindNext.Enabled = true;
            }
            int n = 0, row = 0, col = 0;
            //dataGridView1.CurrentCell.Style.BackColor = System.Drawing.SystemColors.Highlight;// Color.FromArgb(51, 153, 255); // ny blue
            //dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 238, 75); // yellowish
            //dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;// DimGray;
            int curRow = -1, curCol = -1;
            while (n < listGridViewSelection.Count)
            {
                row = listGridViewSelection[n].row;
                col = listGridViewSelection[n].col;
                if (n == nCurGridFindIndex)
                {
                    curRow = row; curCol = col;
                    dataGridView1[col, row].Selected = true;
                }
                else dataGridView1[col, row].Selected = false;
                //{
                //dataGridView1[col, row].Selected = false;
                //  dataGridView1[col, row].Style.BackColor = System.Drawing.SystemColors.Highlight; //Color.FromArgb(51, 153, 255); // ny blue
                //dataGridView1.CurrentCell = dataGridView1[col, row];
                //dataGridView1.CurrentCell.Style.BackColor = Color.FromArgb(51, 153, 255); // ny blue
                //}
                //else
                //    dataGridView1[col, row].Style.BackColor = Color.Yellow;
                //                   dataGridView1[col, row].Selected = true;
                ++n;
            }
            if (curRow != -1 && curCol != -1)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[curRow].Cells[0];
                dataGridView1[curCol, curRow].Selected = true;
            }
        }

        private void dataGridViewFindClear()
        {
            foreach (_GridViewSelection g in listGridViewSelection)
            {
                dataGridView1[g.col, g.row].Style.BackColor = Color.White;

            }
            nCurGridFindIndex = 0;
            dataGridView1.ClearSelection();
            listGridViewSelection.Clear();
            dataGridView1.DefaultCellStyle.SelectionBackColor = defaultDataGridViewSelectionColor;
        }
    }
}
