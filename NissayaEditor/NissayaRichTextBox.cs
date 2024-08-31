using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        public const int rtbTextChanged = 1;
        public const int rtbKeyDown = 2;
        public const int rtbKeyPress = 4;
        public const int rtbMouseClick = 8;
        public const int rtbAllEventsOn = 0xffff;
        public const int rtbAllEventsOff = 0;

        public partial class NissayaTabPage
        {
            MatchCollection matches = null;
            public Dictionary<string, MatchCollection> findResults;
            public string GetThisTabPageKey() { return this.FileName + "-P" + this.curFindPgNo; }
            public string curFindPgNo = string.Empty;
            int nListFindIndex = 0;
            int prevListFindIndex = 0;
            //int nCurGridFindIndex = 0;
/*
            // do be removed later
            class PageFindInfo1
            {
                public string pageNo;
                //public FindInfo pageFindData;
                public int viewCode;

                public string searchText;
                public string findStatus;
                public MatchCollection matches;
                public int index;
                public int nListFindIndex;
                public int prevListFindIndex;
                public void Clear()
                {
                    findStatus = searchText = string.Empty;
                    matches = null;
                    index = prevListFindIndex = -1;
                }
                public void init(MatchCollection m) { matches = m; index = 0; }

                public PageFindInfo1(string pNo, int vCode)
                {
                    pageNo = pNo; viewCode = vCode;
                    nListFindIndex = 0; prevListFindIndex = -1;
                    findStatus = searchText = string.Empty; matches = null;
                }

                public void SetFindIndex(int n) { nListFindIndex = n; }
                public void SetPrevFindIndex(int n) { prevListFindIndex = n; }
                public int UpdateFindIndex(int n) { nListFindIndex += n; return nListFindIndex;  }
                public int UpdatePrevFindIndex(int n) { prevListFindIndex += n; return prevListFindIndex; }
                public void SetFindIndices(int n) { nListFindIndex = prevListFindIndex = n; }
            }
*/
            public void ClearFindIndices() { nListFindIndex = prevListFindIndex = -1; }
            
            void richTextBox1_initialize()
            {
                richTextBox1.Font = parentForm.fontMYA;
                richTextBox1.SelectionStart = 0;
                richTextBox1.SelectionLength = 0;
                richTextBox1.ScrollToCaret();
                richTextBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                //richTextBox1_registerEvents();
            }

            private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
            {
                indexCurCursor = richTextBox1.SelectionStart;
                caretPosition = indexCurCursor;
                parentForm.flagFindTextBoxClicked = false;
            }

            public void richTextBox1_TextChanged(object sender, EventArgs e)
            {
                this.richTextBox1_Updated = true;
                if (richTextBox1.SelectionStart <= indexCurCursor) return;
                //this.richTextBox1_registerEvents(rtbAllEventsOff);
                string s = ((RichTextBox)sender).Text;
                string newText = s.Substring(indexCurCursor, richTextBox1.SelectionStart - indexCurCursor);
                if (newText.Length > 0)
                    adjustTextColor(newText);
                this.fileUpdated = true;
                //this.richTextBox1_registerEvents(rtbAllEventsOn);
            }

            public void richTextBox1_KeyDown(object sender, KeyEventArgs e)
            {
                //if (e.Control && e.KeyCode == Keys.R)
                //    return;
                e.Handled = HandleMenuShortCuts(sender, e);
                if (e.Handled) return;

                if (e.KeyCode == Keys.F3)
                {
                    if (parentForm.currentViewMenu == ViewMenu.Tabular) parentForm.dataGridViewFindNext();
                    else parentForm.richTextBoxFindNext();
                }
            }
            public bool KeyDownHandled = false;
            public void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (KeyDownHandled)
                {
                    e.Handled = true;
                    KeyDownHandled = false;
                }
            }
            public bool HandleMenuShortCuts(object sender, KeyEventArgs e)
            {
                if (dbgTrace != null)
                {
                    string s = "HandleMenuShortCuts() KeyData = " + e.KeyData.ToString();
                    s += " KeyData = " + e.KeyData.ToString();
                    s += " KeyCode= " + e.KeyCode.ToString();
                    dbgTrace.WriteLine(s);
                }
                if (e.Control && e.KeyCode == Keys.R)
                {
                    //return true;
                }
                // Save-As
                if (e.Control && e.KeyCode == Keys.A)
                {
                    parentForm.button_SaveAs_Click(sender, e);
                    return true;
                }
                // Save file
                if (e.Control && e.KeyCode == Keys.S)
                {
                    //SaveFile();
                    parentForm.button_Save_Click(sender, e);
                    return true;
                }
                // Quit app
                if (e.Control && e.KeyCode == Keys.Q)
                {
                    parentForm.button_Quit_Click(sender, e);
                    return true;
                }
                // load new file
                if (e.Control && e.KeyCode == Keys.O)
                {
                    parentForm.loadToolStripMenuItem_Click(sender, e);
                    return true;
                }
                // new file
                if (e.Control && e.KeyCode == Keys.N)
                {
                    parentForm.newCtrlNToolStripMenuItem_Click(sender, e);
                    return true;
                }
                if (e.Control && e.KeyCode == Keys.P)
                {
                    parentForm.printCtrlPToolStripMenuItem_Click(sender, e);
                    return true;
                }
                // Pali Text Only Ctrl-W
                if (e.Control && e.KeyCode == Keys.W)
                {
                    parentForm.closeToolStripMenuItem_Click(sender, e);
                    e.Handled = true;
                    return true;
                }
                // Plain Text Only Ctrl-E
                if (e.Control && e.KeyCode == Keys.E)
                {
                    //parentForm.plainTextToolStripMenuItem_Click(sender, e);
                    e.Handled = true;
                    return true;
                }
                // Plain Text Only Ctrl-R
                if (e.Control && e.KeyCode == Keys.R)
                {
                    //parentForm.fullDocToolStripMenuItem_Click(sender, e);
                    e.Handled = true;
                    return true;
                }

                // Toggle Tabular view
                if (e.Control && e.KeyCode == Keys.T)
                {
                    TableViewToggle(sender, e);
                    return true;
                }
                // Data Box Ctrl-D 
                //if (e.Control && e.KeyCode == Keys.D && nissayaTabPageList[tabControl1.SelectedIndex].dataGridView1_Visible())
                //{
                //    dataBoxCtrlDToolStripMenuItem_Click(sender, e);
                //    return true;
                //}
                // Ctrl-^ file
                if ((e.Control || e.Shift) && e.KeyCode == Keys.D6)
                {
                    e.Handled = true;
                    KeyDownHandled = true;
                    richTextBox1_InsertText(sender, "^");
                    //this.richTextBox1_Updated = true;
                    //adjustTextColor("^");
                    return true;
                }
                // Ctrl-2 = @
                if (e.Control && e.KeyCode == Keys.D2)
                {
                    e.Handled = true;
                    richTextBox1_InsertText(sender, "@");
                    //this.richTextBox1_Updated = true;
                    //adjustTextColor("@");
                    return true;
                }
                // Ctrl-3 = #
                if (e.Control && e.KeyCode == Keys.D3)
                {
                    e.Handled = true;
                    richTextBox1_InsertText(sender, "#");
                    //this.richTextBox1_Updated = true;
                    //adjustTextColor("#");
                    return true;
                }
                // Ctrl-8 = *
                if (e.Control && e.KeyCode == Keys.D8)
                {
                    e.Handled = true;
                    richTextBox1_InsertText(sender, "*");
                    //this.richTextBox1_Updated = true;
                    //adjustTextColor("*");
                    return true;
                }
                return false;
            }

            public void richTextBox1_InsertText(object sender, string t)
            {
                //int pos = 0;
                //var v = sender.GetType().ToString();
                string s = sender.GetType().ToString();
                if (sender.GetType().ToString().IndexOf(".RichTextBox") >= 0)
                {
                    //pos = richTextBox1.SelectionStart;
                    //richTextBox1.Select(pos, 0);

                    richTextBox1.SelectionLength = 0;
                    richTextBox1.SelectedText = t;
                    //Clipboard.SetText(t);
                    //richTextBox1.Paste();
                    this.richTextBox1_Updated = true;
                }
                if (sender.GetType().ToString().IndexOf(".TextBox") >= 0)
                {
                    // the object is FindTextBox
                    //
                    //pos = parentForm.textBox_Find.SelectionStart;
                    //parentForm.textBox_Find.Select(pos, 0);

                    parentForm.textBox_Find.SelectionLength = 0;
                    parentForm.textBox_Find.SelectedText = t;
                    //Clipboard.SetText(t);
                    //parentForm.textBox_Find.Paste();
                }
            }

            private void adjustTextColor(string newText)
            {
                int pos = newText.IndexOfAny(textMarkers);
                if (pos == -1) return;

                while (pos != -1)
                {
                    richTextBox1.SelectionStart = indexCurCursor + pos;
                    richTextBox1.SelectionLength = newText.Length - pos;
                    richTextBox1.SelectionColor = GetNTPColor(newText[pos]);// DataInfo.GetColor(newText[pos]);
                    pos = newText.IndexOfAny(textMarkers, pos + 1);
                }
                richTextBox1.SelectionStart = indexCurCursor + newText.Length;
                richTextBox1.SelectionLength = 0;
                return;
            }

            private string HandleRegExpControlChars(string s)
            {
                if (s.IndexOf("*") != -1)
                {
                    s = s.Replace("*", "\\*");
                }
                if (s.IndexOf("^") != -1)
                {
                    s = s.Replace("^", "\\^");
                } return s;
            }
            // #FIND_RTB
            public void richTextBoxFind(Boolean loadFirstPageFound = true)
            {
                // this the start of Find operation
                richTextBoxFindClear();
                //parentForm.ClearPagesFindResults(); //false);    // false = clear all view results
                findText = parentForm.textBox_Find.Text;
                curFindPgNo = curPageNo;
                if (curViewCode == ViewTabular)
                {
                    // if Tabular View call this routine
                    dataGridView1.dataGridViewFind();
                    return;
                }
                // the view is not Pali, Plain or Full Doc
                // v1.0.1 2022-05-17
                string s = HandleRegExpControlChars(parentForm.textBox_Find.Text);
               
                //Regex regex = new Regex(s, RegexOptions.IgnoreCase);
                //matches = regex.Matches(richTextBox1.Text);
                //matches = regex.Matches(":");
                // find the text in all the pages of the current file
                findResults = dataInfo.SearchText(s);
                
                if (findResults.Count > 0)
                {
                    // save all the results in 
                    string TabPageKey = GetThisTabPageKey();
                    // find the next page that has the find result 
                    var keys = findResults.Keys.ToList();
                    if (!keys.Contains(TabPageKey))
                    {
                        var result = keys
                           .Where(item => StringComparer.OrdinalIgnoreCase.Compare(item, TabPageKey) > 0)
                           //.OrderBy(item => item)
                           //.First();
                           .ToList();
                        if (result.Count > 0) TabPageKey = result.First();
                        else TabPageKey = findResults.Keys.First();
                    }
                    //string pgNo;
                    //int n = TabPageKey.LastIndexOf("P");
                    //if (n != -1) pgNo = TabPageKey.Substring(n + 1);
                    //else pgNo = TabPageKey;

                    // add find pages to Add2ViewFindMatchesList
                    int nn = 0;
                    PageFindInfo pageFindInfo;
                    Boolean found = false;
                    foreach(string k in keys)
                    {
                        if (!found && k == TabPageKey)
                            // page with result found
                            found = true;

                        if (found)
                        {
                            TabPageKey = k;
                            pageFindInfo = new PageFindInfo(TabPageKey, curViewCode);
                            nListFindIndex = -1;
                            pageFindInfo.nissayaTabPage = this;
                            pageFindInfo.pageNo = GetKeyPageNo(TabPageKey);
                            pageFindInfo.searchText = findText;
                            pageFindInfo.matches = findResults[k];
                            parentForm.Add2ViewFindMatchesList(pageFindInfo);
                            parentForm.UpdateTotalFindCountView(pageFindInfo.matches.Count);
                        }
                        if (!found) ++nn;
                    }
                    if (nn > 0)
                    {
                        for (int i = 0; i < nn; i++)
                        {
                            TabPageKey = keys[i];
                            pageFindInfo = new PageFindInfo(TabPageKey, curViewCode);
                            nListFindIndex = -1;
                            pageFindInfo.nissayaTabPage = this;
                            pageFindInfo.pageNo = GetKeyPageNo(TabPageKey);
                            pageFindInfo.searchText = findText;
                            pageFindInfo.matches = findResults[TabPageKey];
                            parentForm.Add2ViewFindMatchesList(pageFindInfo);
                            parentForm.UpdateTotalFindCountView(pageFindInfo.matches.Count);
                        }
                    }
                    //if (loadFirstPageFound)
                    //{
                    //    LoadPage(pgNo);
                    //    List<string> lsPages = GetPageNo();
                    //    lsPages = GetPageList();
                    //    //parentForm.pageMenuBar.P
                    //    parentForm.pageMenuBar.FillPageMenu(lsPages, pgNo);
                    //    //parentForm.pageMenuBar.NewPageSelected(pgNo);
                    //}
                    //pageFindInfo = new PageFindInfo(TabPageKey, curViewCode);
                    //curFindPgNo = curPageNo = pgNo;
                    //nListFindIndex = -1;
                    //pageFindInfo.nissayaTabPage = this;
                    //pageFindInfo.pageNo = GetKeyPageNo(TabPageKey);
                    //pageFindInfo.searchText = findText;
                    ////pageFindInfo.nIndexFindPageResults = 0;     // zero-based index in FindPageResults
                    //pageFindInfo.matches = findResults[TabPageKey];
                    //parentForm.Add2ViewFindMatchesList(pageFindInfo);
                    //// store info in the FindMatchingList
                    //parentForm.UpdateTotalFindCountView(pageFindInfo.matches.Count);
                    //parentForm.CurActivePageFindInfo = pageFindInfo;
                    //parentForm.CurPageHighlightType = HightlightType.HighlightText;
                    //richTextBoxFindNext();
                }
            }

            private string GetKeyPageNo(string key)
            {
                string pgNo = string.Empty;
                int n = key.LastIndexOf("P");
                if (n != -1) pgNo = key.Substring(n + 1);
                else pgNo = key;
                return pgNo;
            }
            // #FIND_RTB
            //private int prevListFindIndex = -1;

            public bool richTextBoxFindNext(bool flagNext = true)
            {
                bool newPage = false;   // = false the next occurrence is within the page
                // = true the next occurrence is in the next page

                PageFindInfo pgFindInfo = parentForm.CurActivePageFindInfo;
                if (pgFindInfo == null) return true;
                matches = pgFindInfo.matches;
                // save the current nListFindIndex in prevListFindIndex
                // as prevListFindIndex will turn to yellow background
                // and nListFindIndex will turn to blue background
                //prevListFindIndex = nListFindIndex;
                prevListFindIndex = pgFindInfo.nCurPageFindIndex;

                // if the next item is 
                if (matches != null && pgFindInfo.nCurPageFindIndex <= matches.Count)
                {
                    switch (flagNext)
                    {
                        case true:
                            if (pgFindInfo.nCurPageFindIndex + 1 >= matches.Count) 
                                newPage = true;
                            else
                            {
                                //nListFindIndex++; 
                                pgFindInfo.nCurPageFindIndex++;
                                parentForm.UpdateTotalFindIndexView(1);
                            }
                            break;
                        case false:
                            if (pgFindInfo.nCurPageFindIndex - 1 < 0) newPage = true;
                            else
                            {
                                //nListFindIndex--; 
                                pgFindInfo.nCurPageFindIndex--;
                                parentForm.UpdateTotalFindIndexView(-1);
                            }
                            break;
                    }
                }
                if (newPage) return false;      // next occurence is in another page
                parentForm.CurPageHighlightType |= HightlightType.HighlightSelectedText;
                parentForm.CurActivePageFindInfo = pgFindInfo;
                HighlightFindData();
                return true;                    // next occurence is within this page
            }

            public void DoFindThread()
            {
                init_FindTab_BackgroundWorker(backgroundWorker2);
                backgroundWorker2.RunWorkerAsync((NissayaTabPage)this);
            }

            public void richTextBoxPageFind(string pgno)
            {
                if (curViewCode == ViewTabular)
                {
                    dataGridView1.dataGridViewFind();
                    return;
                } 
                
                // this routine find text in next page 
                curFindPgNo = pgno;
                // set view containers
                //***dataInfo.SetDataContainers2(richTextBox2); //, dataGridView2);
                // fill richTextBox2.Text with text
                //***dataInfo.GetPageData(curFindPgNo, curViewCode);
                //string content = dataInfo.GetPageContent(curFindPgNo, curViewCode);

                //parentForm.ClearPagesFindResults(true, pgno);    // true = current page data only
                //string findText = parentForm.textBox_Find.Text;

                // v1.0.1 2022-05-17
                //string s = HandleRegExpControlChars(parentForm.textBox_Find.Text);
                //Regex regex = new Regex(s, RegexOptions.IgnoreCase);
                //***matches = regex.Matches(richTextBox2.Text);
                //matches = regex.Matches(content);

                string key = GetThisTabPageKey();
                // 2022-07-07
                // when findResults is null the program crashes
                // checking for findResults if it is null added
                if (findResults == null || !findResults.ContainsKey(key)) return;
                matches = findResults[key];
                //if (matches.Count == 0) return;

                PageFindInfo pageFindInfo = new PageFindInfo(key, curViewCode);
                pageFindInfo.matches = matches;
                pageFindInfo.searchText = findText;
                nListFindIndex = 0;
                //pageFindInfo.SetCurFindIndex(0);
                pageFindInfo.nissayaTabPage = this;
                pageFindInfo.pageNo = curFindPgNo;
                //pageFindInfo.nIndexFindPageResults = parentForm.GetPageFindResultsViewCount();
                //pageFindInfo.flagHighlightDrawn = false;
                parentForm.CurPageHighlightType = HightlightType.DoNothing;
                parentForm.UpdateTotalFindCountView(matches.Count);
                parentForm.Add2ViewFindMatchesList(pageFindInfo);
            }

            public void HighlightFindData(bool flagCurHighlight = true)
            {
                if (!flagCurHighlight) nListFindIndex = -1;
                // color selectiontext
                ColorFindData();
                if (flagCurHighlight)
                {
                    //richTextBox1.DeselectAll();
                    //v1.0.2
                    //richTextBox1.SelectionStart = 0;
                }
                richTextBox1.Focus();
            }

            // #FIND_RTB
            public void richTextBoxFindClear()
            {
                if (matches != null && matches.Count > 0)
                //if (findInfoFull.matches != null && findInfoFull.matches.Count > 0 ||
                //    findInfoPali.matches != null && findInfoPali.matches.Count > 0 ||
                //    findInfoPlain.matches != null && findInfoPlain.matches.Count > 0)
                {
                    richTextBox1.TextChanged -= richTextBox1_TextChanged;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.SelectAll();
                    richTextBox1.SelectionBackColor = richTextBox1.BackColor;
                    richTextBox1.DeselectAll();
                    richTextBox1.TextChanged += richTextBox1_TextChanged;
                }
                //findInfoFull.Clear();
                //findInfoPali.Clear();
                //findInfoPlain.Clear();
                //ClearPagesFindResults();
                //parentForm.textBox_FindStatus.Text = parentForm.textBox_Find.Text = string.Empty;
                matches = null;
                nListFindIndex = 0;
            }
           
            // #FIND_DATA
            MatchCollection prevPageMatches = null;
            public void ColorFindData()
            {
                if (parentForm.currentViewMenu == ViewMenu.Tabular)
                {
                    setDataGridViewFindData();
                    return;
                }
                // check if the current page Find results are currentt active Find results
                PageFindInfo pgFindInfo = GetPageFindInfo(GetThisTabPageKey(), curViewCode);
                if (pgFindInfo == null) return; // if current page has no Find results, return
                matches = pgFindInfo.matches;
                if (parentForm.CurPageHighlightType == HightlightType.DoNothing ||
                    matches == null || matches.Count == 0) return;

                richTextBox1.TextChanged -= richTextBox1_TextChanged;
                // highlight all Find results
                if ((parentForm.CurPageHighlightType & HightlightType.HighlightText) == HightlightType.HighlightText)
                {
                    //if (prevPageMatches != null)
                    //{
                    //    Match m = prevPageMatches[prevPageMatches.Count - 1];
                    //    richTextBox1.Select(m.Index, m.Length);
                    //    richTextBox1.SelectionBackColor = Color.Yellow;
                    //}
                    foreach (Match match in matches)
                    {
                        richTextBox1.Select(match.Index, match.Length);
                        richTextBox1.SelectionBackColor = Color.Yellow;
                    }
                    //pgFindInfo.flagHighlightDrawn = true;
                }
                else
                {
                    if (//prevPageMatches == matches &&
                        prevListFindIndex >= 0 && prevListFindIndex < matches.Count)
                    {
                        Match prevMatch = matches[prevListFindIndex];
                        richTextBox1.Select(prevMatch.Index, prevMatch.Length);
                        richTextBox1.SelectionBackColor = Color.Yellow;
                    }
                    prevPageMatches = matches;
                }
                // highlight current selection
                if (parentForm.CurActivePageFindInfo != null)
                {
                    int n = parentForm.CurActivePageFindInfo.nCurPageFindIndex;
                    if (((parentForm.CurPageHighlightType & HightlightType.HighlightSelectedText) == HightlightType.HighlightSelectedText)
                        && n >= 0 && n < matches.Count)
                    //&& (pgFindInfo.nCurPageFindIndex >= 0 && pgFindInfo.nCurPageFindIndex < matches.Count))
                    {
                        Match curMatch = matches[n];
                        richTextBox1.Select(curMatch.Index, curMatch.Length);

                        richTextBox1.SelectionBackColor = SystemColors.Highlight;
                        //richTextBox1.SelectionColor = SystemColors.HighlightText;

                        richTextBox1.SelectionStart = curMatch.Index;
                        richTextBox1.ScrollToCaret();
                        //prevListFindIndex = pgFindIndo.nCurPageFindIndex;
                    }
                }
                richTextBox1.TextChanged += richTextBox1_TextChanged;
                richTextBox1.Focus();
            }

            /*
            // #FIND SETDATA
            private void refreshFindInfo()
            {
                PageFindInfo pgFindInfo = GetPageFindInfo(GetThisTabPageKey(), curViewCode);
                if (matches == null)
                {
                    parentForm.textBox_Find.Text = parentForm.textBox_FindStatus.Text = string.Empty;
                    parentForm.button_FindNext.Enabled = parentForm.button_FindPrev.Enabled = false;
                    return;
                }
                if (matches.Count == 0)
                {
                    findStatus = parentForm.textBox_FindStatus.Text = "0/0";
                    parentForm.button_FindNext.Enabled = parentForm.button_FindPrev.Enabled = false;
                    nListFindIndex = 0;
                    return;
                }

                //findStatus = parentForm.textBox_FindStatus.Text = nListFindIndex.ToString() + "/" + matches.Count.ToString();
                findStatus = parentForm.textBox_FindStatus.Text = nListFindIndex.ToString() + "/" + matches.Count.ToString();

                if (matches.Count <= 1)
                    parentForm.button_FindNext.Enabled = parentForm.button_FindPrev.Enabled = false;
                else
                {
                    parentForm.button_FindPrev.Enabled = (nListFindIndex <= 1) ? false : true;
                    parentForm.button_FindNext.Enabled = true;
                }
                pgFindInfo.findStatus = findStatus;
            }

            public void setViewFindText()
            {
                if (curViewCode == ViewTabular) return;
                parentForm.textBox_Find.Text = findText;
                if (matches == null) parentForm.textBox_FindStatus.Text = string.Empty;
                else
                {
                    PageFindInfo pgFindInfo = GetPageFindInfo(GetThisTabPageKey(), curViewCode);
                    parentForm.textBox_FindStatus.Text = nListFindIndex.ToString() + "/" + matches.Count.ToString();
                }
            }
            */
            private void setDataGridViewFindData()
            {
                parentForm.textBox_Find.Text = dataGridView1.SearchText;
                parentForm.textBox_FindStatus.Text = dataGridView1.FindStatus;
            }

            public void CheckFindData()
            {
                this.curFindPgNo = curPageNo;
                string key = GetThisTabPageKey();
                if (curViewCode == ViewTabular)
                {
                    // Tabular view
                    DataGridViewFindPage dgvPage = null;
                    if (DataGridViewFindPageList.ContainsKey(key))
                        dgvPage = DataGridViewFindPageList[key];
                    if (dgvPage != null)
                    {
                        parentForm.CurPageHighlightType = HightlightType.HighlightText;
                        if (dgvPage.key == CurActiveGridPageFindInfo.key)
                            parentForm.CurPageHighlightType |= HightlightType.HighlightCurrentSelectedText;
                        dataGridView1.dataGridViewFindNext();
                    }
                    else dataGridView1.Rows[0].Selected = true;
                    return;
                }

                PageFindInfo pgFindInfo = null;
                if (PageFindResults_Full.ContainsKey(key))
                    pgFindInfo = PageFindResults_Full[key];
                if (pgFindInfo != null)
                {
                    parentForm.CurPageHighlightType = HightlightType.HighlightText;
                    if (pgFindInfo.key == parentForm.CurActivePageFindInfo.key)
                        parentForm.CurPageHighlightType |= HightlightType.HighlightSelectedText;
                    HighlightFindData();
                }
                else 
                {
                    richTextBox1.SelectionStart = 0;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.ScrollToCaret();
                }
            }
        }
    }
}
