using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        class DataInfo
        {
            Form1 form;
            TabControl tabControl;
            TabPage tabPage;
            FileManager fMgr;
            public string MN = string.Empty;
            int countTotal;
            string paliText = string.Empty;
            
            public const int dvTextBox = 1;
            public const int dvDataGrid = 2;
            public const int dvBoth = 3;

            public int ThreadAction = 0;
            public int ViewCode = Form1.ViewFullDoc;
            public bool UpdateFileContent = false;
            public string PageToLoad = string.Empty;

            public bool flagPali;
            public bool flagPlain;
            public bool UseSecondaryGridContainer = false;
            public RichTextBox richTextBox, richTextBox2;
            public DataGridView dataGridView, dataGridView2;
            public string fileContent;
            public Dictionary<string, string> Pages = new Dictionary<string, string>();
            public struct KeyMap
            {
                public string curKey;
                public string orgKey;

                public KeyMap(string key, string org) { curKey = key; orgKey = org; }
            }
            public List<KeyMap> KeyMapper = new List<KeyMap>();

            public List<string> GetPageList()
            {
                List<string> l = new List<string>();
                foreach (KeyMap k in KeyMapper)
                    l.Add(k.curKey);
                return l;
            }
            private List<_NissayaDataRecord> NissayaRecordList = new List<_NissayaDataRecord>();
            public List<_NissayaDataRecord> GetDataList() { return NissayaRecordList; }

            private string curPage = string.Empty;
            string paraNewLine = "\n\n";
            private delegate void SafeCallDelegate(string s, int start, int end);
            private delegate void SafeDeleteRowsDelegate();
            private delegate void safeCallAddDataGridView(string srno, char marker, string pali, string plain, string footnote);
            private delegate void safeCallRichTextAppend(int codeView, string s);
            private delegate void safeCallRichTextUpdate(Color color, string s);
            private delegate void safeCallGetRichText();
            private delegate void safeTextBox2Clear();
            public string GetCurrentPage() { return curPage; }

            public DataInfo(Form1 f, TabControl tc, TabPage tb, RichTextBox rtb, DataGridView dgv, string fName)
            {
                form = f;
                tabControl = tc;
                tabPage = tb;
                richTextBox = rtb;
                dataGridView = dgv;
                string fn = Path.GetFileNameWithoutExtension(fName);
                if (fn.Length > 15) fn = fn.Substring(0, 5) + " ... " + fn.Substring(fn.Length - 5, 5);
                tb.Text = fn;

                fMgr = new FileManager(fName, richTextBox);
            }

            public DataInfo(RichTextBox rtb, DataGridView dgv, string fName)
            {
                richTextBox = rtb;
                dataGridView = dgv;
                string fn = Path.GetFileNameWithoutExtension(fName);
                if (fn.Length > 15) fn = fn.Substring(0, 5) + " ... " + fn.Substring(fn.Length - 5, 5);
                fMgr = new FileManager(fName, richTextBox);
            }
            
            public void SetDataContainers2(RichTextBox rtb2) //, DataGridView dgv2)
            {
                richTextBox2 = rtb2;
                //dataGridView2 = dgv2;
            }
            public void SetFileName(string fname)
            {
                fMgr.fName = fname;
                string fn = Path.GetFileNameWithoutExtension(fname);
                if (fn.Length > 15) fn = fn.Substring(0, 5) + " ... " + fn.Substring(fn.Length - 5, 5);
                tabPage.Text = fn;
            }
            
            public void ClearMemory()
            {
                tabPage.Text = "<New>";
                clearDataGridView();
            }
            //public void logElapsedTime(string s, long milliSec)
            //{
            //    FileStream fileStream = new FileStream("log.txt", FileMode.Append);
            //    StreamWriter writer = new StreamWriter(fileStream);
            //    writer.WriteLine(s + " Elapsed Time = " + milliSec.ToString() + " ms");
            //    writer.Flush();
            //    writer.Close();
            //}
            public void ReadFileLoadData(int viewCode)
            {
                fileContent = fMgr.ReadFile().Trim();
                if (fileContent.Length == 0 || !ValidateData())
                {
                    string msg;
                    if (fileContent.Length == 0)
                    {
                        msg = fMgr.fName + " is empty.";
                        MessageBox.Show(msg);
                        return;
                    }
                    msg = fMgr.fName + " is not a valid file. ";
                    msg += "Number of (*) and (^) records mismatched.\n\n";
                    msg += "Do you want to fix it by combining consecutive same type records into one (y/n)?";
                    DialogResult res = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No)
                    {
                        fileContent = string.Empty;
                        return;
                    }
                    CombineRecords();
                }
                // clean up the space after markers
                fileContent = fileContent.Replace("* ", "*");
                fileContent = fileContent.Replace("^ ", "^");
                paraNewLine = getCR2();
                // check for header info
                ExtractHeader();
                parseIntoPages();
                //logElapsedTime("DataInfo.ReadFileLoadData.ParseIntoPages():",
                //    (DateTime.Now.Ticks - start_time) / 10000);
                string s = Pages.Keys.First<string>();
                PageToLoad = s;
                LoadPageData(viewCode);
            }

            private bool ValidateData()
            {
                if (fileContent.Length == 0) return false;
                string pattern = "\\*";
                MatchCollection matches1 = Regex.Matches(fileContent, pattern);
                pattern = "\\^";
                MatchCollection matches2 = Regex.Matches(fileContent, pattern);
                return (matches1.Count == matches2.Count);
            }
            private void CombineRecords()
            {
                Dictionary<int, string> updatedRecs = new Dictionary<int, string>();
                Dictionary<int, string> updatedPage = new Dictionary<int, string>();
                HashSet<int> rec2Delete = new HashSet<int>();
                string compoundPaliText = string.Empty;
                string[] pages = fileContent.Split('#');
                int pageNo = -1;
                string pg;
                foreach (string page in pages)
                {
                    ++pageNo;
                    pg = page.Trim();
                    if (pg.Length == 0) continue;
                    string pattern = "\\*";
                    MatchCollection matches1 = Regex.Matches(page, pattern);
                    pattern = "\\^";
                    MatchCollection matches2 = Regex.Matches(page, pattern);
                    if (matches1.Count != matches2.Count)
                    {
                        //}
                        //}
                        ////string page = pages[2];
                        //int pageNo = -1;
                        //foreach (string page in pages)
                        //{
                        string[] nisRecords = page.Split('*');
                        int n1 = -1;
                        foreach (string nisRec in nisRecords)
                        {
                            if (++n1 == 0) continue; // this is to skip the page no
                            string[] trans = nisRec.Split('^');
                            switch (trans.Length)
                            {
                                case 0:
                                case 1:
                                    // found Pali text only
                                    if (compoundPaliText.Length > 0)
                                    {
                                        compoundPaliText = compoundPaliText.Trim();
                                        compoundPaliText += "၊ ";
                                    }
                                    compoundPaliText += trans[0];
                                    rec2Delete.Add(n1);
                                    break;
                                case 2:
                                    // normal NIS record
                                    // if there is a compoundRec previously, save it
                                    if (compoundPaliText.Length > 0)
                                    {
                                        compoundPaliText = compoundPaliText.Trim() + "၊ ";
                                        compoundPaliText += trans[0] + "^" + trans[1];
                                        updatedRecs.Add(n1, compoundPaliText);
                                        compoundPaliText = string.Empty;
                                    }
                                    break;
                                default:
                                    int n2 = -1; string s2 = string.Empty;
                                    foreach (string t in trans)
                                    {
                                        if (++n2 == 0) continue;
                                        if (t.Trim().Length > 0)
                                        {
                                            if (s2.Length > 0) s2 += "၊ ";
                                            s2 += t;
                                        }
                                    }
                                    updatedRecs.Add(n1, trans[0] + "^" + s2);
                                    break;
                            }
                        }
                        // update NIS records
                        if (updatedRecs.Count > 0 || rec2Delete.Count > 0)
                        {
                            foreach (KeyValuePair<int, string> kv in updatedRecs)
                            {
                                nisRecords[kv.Key] = kv.Value;
                            }
                            List<string> nisRecords2 = new List<string>();
                            string updatedPage2 = "#";
                            int n3 = -1;
                            foreach (string s in nisRecords)
                            {
                                ++n3;
                                if (n3 == 0) updatedPage2 += s;
                                if (!rec2Delete.Contains(n3) && n3 > 0) updatedPage2 += "*" + s;
                            }
                            updatedRecs.Clear();
                            rec2Delete.Clear();
                            updatedPage.Add(pageNo, updatedPage2);
                            updatedPage2 = string.Empty;
                        }
                    }
                }
                
                UpdateFileContent = true;
                pageNo = -1;
                fileContent = string.Empty;
                foreach (string p in pages)
                {
                    if (fileContent.Length > 0) fileContent += Environment.NewLine;
                    if (updatedPage.ContainsKey(++pageNo))
                    {
                        fileContent += updatedPage[pageNo];
                    }
                    else
                    {
                        fileContent += (p.Length > 0) ? "#" + p : p;
                    }
                }
            }

            private void ExtractHeader()
            {
                int pos = fileContent.IndexOf("}\r");
                if (pos == -1) pos = fileContent.IndexOf("}\n");
                if (pos == -1) return;
                string s = fileContent.Substring(0, pos + 1).Trim();
                if (s[0] == '{' && s[s.Length - 1] == '}')
                {
                    s = s.Substring(1, s.Length - 2);
                    string[] f = s.Split(':');
                    if (f[0] == "MN") MN = f[1];
                    fileContent = fileContent.Substring(pos + 3);
                }
            }

            private void parseIntoPages()
            {
                //string paraNewLine = "\n\n";
                string key;
                List<string> paraPages = new List<string>();
                int start = 0;
                int end = fileContent.IndexOf(paraNewLine);
                while (end != -1)
                {
                    paraPages.Add(fileContent.Substring(start, end - start + 1));
                    start = end + paraNewLine.Length;
                    end = fileContent.IndexOf(paraNewLine, start);
                }
                paraPages.Add(fileContent.Substring(start));

                List<string> listParaPages = new List<string>();
                // cleanup
                foreach (string s in paraPages)
                {
                    string s1 = s.Trim();
                    if (s1.Length > 0)
                    {
                        listParaPages.Add(s1);
                        end = s1.IndexOf('*');
                        // 2022-05-29 v1.0.3
                        if (end == -1) key = s1;
                        else
                        {
                            key = s1.Substring(0, end);
                            // if there is no space between page# and *, add one
                            if (s1[end - 1] != ' ') 
                            {
                                s1 = key + " " + s1.Substring(end);
                            }
                        }
                        if (key[0] == '#') key = key.Substring(1);
                        key = key.Trim();
                        // key cannot have spaces, so use the first non-space contiguous chars
                        int n = key.IndexOf(' ');
                        if (n != -1) key = key.Substring(0, n);
                        KeyMapper.Add(new KeyMap(key, key));
                        Pages.Add(key, s1);
                    }
                }
            }

            public void UpdatePageKey(string newKey, string oldKey)
            {
                int i = 0;
                while (i < KeyMapper.Count)
                {
                    if (KeyMapper[i].curKey == oldKey)
                    {
                        KeyMap km = new KeyMap(newKey, KeyMapper[i].orgKey);
                        KeyMapper[i] = km;
                        break;
                    }
                    ++i;
                }
            }

            public string GetOrigKey(string curKey)
            {
                int i = 0;
                while (i < KeyMapper.Count)
                {
                    if (KeyMapper[i].curKey == curKey)
                        return KeyMapper[i].orgKey;
                    ++i;
                }
                return string.Empty;
            }

            public void LoadPageData(int viewCode = ViewFullDoc)
            {
                if (curPage.Length > 0 && UpdateFileContent) Pages[curPage] = fileContent;
                string orgKey = GetOrigKey(PageToLoad);
                if (orgKey.Length > 0)
                {
                    fileContent = Pages[orgKey];
                    curPage = PageToLoad;
                    loadData(viewCode);
                }
            }

            public void LoadPageData2(int viewCode = ViewFullDoc)
            {
                string orgKey = GetOrigKey(PageToLoad);
                if (orgKey.Length > 0)
                {
                    fileContent = Pages[orgKey];
                    curPage = PageToLoad;
                    loadData(viewCode);
                }
            }

            public void loadData(int viewCode = ViewFullDoc)
            {
                if (fileContent != null && fileContent.Length == 0) return;

                flagPali = flagPlain = true;
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //updateDataView(Form1.ViewFullDoc, dvBoth);
                //updateDataView(viewCode, dvBoth);
                updateDataView(viewCode); //, viewCode == 4 ? dvDataGrid : dvTextBox);
                //stopwatch.Stop();
                //TimeSpan ts = stopwatch.Elapsed;
                //if (dbgTrace != null) dbgTrace.OutElpasedTime(ts);
            }

            // codeView
            // 1 = Pali text only
            // 2 = Plain text only
            // 3 = Full doc
            public void RefreshData(int codeView)
            {
                if (fileContent != null && fileContent.Length == 0) return;

                updateDataView(codeView); //, dvTextBox);
            }

            private string getCR2()
            {
                string WinCR2 = Environment.NewLine + Environment.NewLine;
                string UnixCR2 = "\n\n";
                if (fileContent.Length == 0) return UnixCR2;
                if (fileContent.IndexOf(WinCR2) != -1) return WinCR2;
                if (fileContent.IndexOf(UnixCR2) != -1) return UnixCR2;
                return UnixCR2;
            }

            //static string KeyPageNo = string.Empty;
            public string GetNextPageNo(string curPgNo)
            {
                string nextPageNo = curPgNo;
                string orgKey = GetOrigKey(curPgNo);
                if (Pages.ContainsKey(orgKey))
                {
                    List<string> list = Pages.Keys.ToList();
                    int n = list.IndexOf(orgKey);
                    //KeyPageNo = orgKey;
                    //Predicate<string> predicate = SearchKey;
                    //n = list.FindIndex(predicate);
                    if (n < list.Count - 1) nextPageNo = list[++n];
                }
                return nextPageNo;
            }
            // 2022-05-29 v1.0.3
            public Dictionary<string, MatchCollection> SearchText(string s)
            {
                Dictionary<string, MatchCollection> findResults = new Dictionary<string, MatchCollection>();
                MatchCollection matches;
                string fname = fMgr.fName; 
                foreach (KeyValuePair<string, string> kv in Pages)
                {
                    matches = Regex.Matches(kv.Value, s);
                    if (matches.Count > 0) findResults.Add(fname + "-P" + kv.Key, matches);
                }
                return findResults;
            }

            public void updateDataView(int codeView) //, int dview)
            {
                countTotal = 0;
                string s;
                char marker;
                int start, end;
                start = 0;
                
                if (fileContent.Length == 0) return;

                int dview = codeView == ViewTabular ? dvDataGrid : dvTextBox;
                // clear view
                //if ((dview & dvTextBox) == dview) richTextBox.Text = string.Empty;
                if ((dview & dvDataGrid) == dvDataGrid) clearDataGridView();

                //nissayaData.Clear();
                end = fileContent.IndexOfAny(recMarkers);
                if (end == 0) end = fileContent.IndexOfAny(recMarkers, 1);

                while (end != -1)
                {
                    s = fileContent.Substring(start, end - start).Trim(' ');
                    marker = fileContent[start];
                    populateData(codeView, dview, marker, s);
                    start = end;
                    end = fileContent.IndexOfAny(recMarkers, start + 1);
                }

                s = fileContent.Substring(start, fileContent.Length - start).Trim();
                //string t = richTextBox.Text.Substring(100);
                marker = fileContent[start];
                populateData(codeView, dview, marker, s);
                return;
            }

            private void TextBox2Clear() { richTextBox2.Text = string.Empty; }

            public string GetPageContent(string pgno, int CodeView)
            {
                string content = string.Empty;
                return Pages[GetOrigKey(pgno)];
            }

            public List<_GridViewSelection> FindGridPageMatches(string pgno, string s)
            {
                List<_GridViewSelection> listMatches = new List<_GridViewSelection>();
                if (!Pages.ContainsKey(pgno)) return listMatches;
                string data = Pages[pgno];
                if (data.Length == 0) return listMatches;
                int pos1 = data.IndexOf('*');
                int pos2 = data.IndexOf('^');
                string p;
                int row = 0; // first row is page number
                // col0 = row number; col1 = *; col2 = pali; col3 = translation; col4 = footnote
                while (pos1 != -1 && pos2 != -1)
                {
                    row++;
                    // find in Pali text
                    p = data.Substring(pos1, pos2 - pos1 - 1);
                    if (p.IndexOf(s) != -1)
                    {
                        // search string found in Pali
                        listMatches.Add(new _GridViewSelection(row, 2));
                    }
                    // find in translation text
                    pos1 = data.IndexOf('*', pos2 + 1);
                    p = pos1 == -1 ? data.Substring(pos2 + 1) : data.Substring(pos2 + 1, pos1 - pos2 - 1);
                    string[] f = p.Split('@');
                    if (f[0].IndexOf(s) != -1)
                    {
                        // search string found in translation
                        listMatches.Add(new _GridViewSelection(row, 3));
                    }
                    // check in footnote if it exists
                    if (f.Length == 2 && f[1].IndexOf(s) != -1)
                    {
                        // search string found in footnote
                        listMatches.Add(new _GridViewSelection(row, 4));
                    }
                    pos1 = data.IndexOf('*', pos2 + 1);
                    pos2 = pos1 == -1 ? -1 : data.IndexOf('^', pos1 + 1);
                }
                return listMatches;
            }

            public void GetPageData(string pgno, int codeView)
            {
                countTotal = 0;
                string s, pageContent;
                char marker;
                int start, end;
                start = 0;

                int dview = codeView == ViewTabular ? dvDataGrid : dvTextBox;
                if (dview == dvTextBox)
                {
                    if (richTextBox2.InvokeRequired)
                    {
                        var d = new safeTextBox2Clear(TextBox2Clear);
                        richTextBox2.Invoke(d, new object[] { });
                    }
                    else richTextBox2.Text = string.Empty;
                }
                // clear view
                // if the data will be retrieved in 
                if (codeView == ViewTabular)
                {
                    if (UseSecondaryGridContainer) NissayaRecordList.Clear(); // removeDataGridViewRows2();
                    else clearDataGridView();
                }

                pageContent = Pages[GetOrigKey(pgno)];

                end = pageContent.IndexOfAny(recMarkers);
                if (end == 0) end = pageContent.IndexOfAny(recMarkers, 1);

                while (end != -1)
                {
                    s = pageContent.Substring(start, end - start).Trim(' ');
                    marker = pageContent[start];
                    GetRecord(codeView, dview, marker, s);
                    start = end;
                    end = pageContent.IndexOfAny(recMarkers, start + 1);
                }

                s = pageContent.Substring(start, pageContent.Length - start).Trim();
                marker = pageContent[start];
                GetRecord(codeView, dview, marker, s);
            }

            private void GetRecord(int codeView, int dview, char marker, string s)
            {
                string dgvPali, dgvPlain, dgvFootnote;
                string rtbPali, rtbPlain, rtbFootnote;
                dgvPali = dgvPlain = dgvFootnote = string.Empty;
                rtbPali = rtbPlain = rtbFootnote = string.Empty;
                s = s.Trim(' ');
                //richTextBox.Text = "AZM";
                switch (marker)
                {
                    case '*':
                        parseNissayaDataRecord(s, out dgvPali, out dgvPlain, out dgvFootnote);
                        rtbPali = (dgvPali.Length > 0) ? "*" + dgvPali : string.Empty;
                        rtbPlain = (dgvPlain.Length > 0) ? "^" + dgvPlain : string.Empty;
                        rtbFootnote = (dgvFootnote.Length > 0) ? "@" + dgvFootnote : string.Empty;
                        break;
                    case '#':
                        dgvPali = s.Substring(1);
                        rtbPali = s;
                        break;
                    default:
                        // assume this is a comment line
                        marker = '#';
                        rtbPali = marker + s;
                        dgvPali = s;
                        dgvPlain = dgvFootnote = rtbPali = rtbFootnote = string.Empty;
                        break;
                }
                if ((dview & dvDataGrid) == dvDataGrid)
                    populateDataGridView(marker, dgvPali, dgvPlain, dgvFootnote);
                if ((dview & dvTextBox) == dvTextBox)
                    AddViewText(codeView, rtbPali, rtbPlain, rtbFootnote);
            }

            void AddViewText(int codeView, string rtbPali, string rtbPlain, string rtbFootnote)
            {
                if (codeView == ViewTabular)
                {
                }
                else
                {
                    if ((codeView == ViewFullDoc || codeView == ViewPali) && rtbPali.Length > 0)
                    {
                        if (richTextBox2.Text.Length > 0) richTextBox2.AppendText(" ");
                        richTextBox2.AppendText(rtbPali);
                    }
                    if ((codeView == ViewFullDoc || codeView == ViewPlain) && rtbPlain.Length > 0)
                    {
                        if (richTextBox2.Text.Length > 0) richTextBox2.AppendText(" ");
                        richTextBox2.AppendText(rtbPlain);
                    }
                    if ((codeView == ViewFullDoc) && rtbFootnote.Length > 0)
                    {
                        if (richTextBox2.Text.Length > 0) richTextBox2.AppendText(" ");
                        richTextBox2.AppendText(rtbFootnote);
                    }
                }
            }

            private void populateData(int codeView, int dview, char marker, string s)
            {
                string dgvPali, dgvPlain, dgvFootnote;
                string rtbPali, rtbPlain, rtbFootnote;
                dgvPali = dgvPlain = dgvFootnote = string.Empty;
                rtbPali = rtbPlain = rtbFootnote = string.Empty;
                s = s.Trim(' ');
                //richTextBox.Text = "AZM";
                switch (marker)
                {
                    case '*':
                        parseNissayaDataRecord(s, out dgvPali, out dgvPlain, out dgvFootnote);
                        rtbPali = (dgvPali.Length > 0) ? "*" + dgvPali : string.Empty;
                        rtbPlain = (dgvPlain.Length > 0) ? "^" + dgvPlain : string.Empty;
                        rtbFootnote = (dgvFootnote.Length > 0) ? "@" + dgvFootnote : string.Empty;
                        break;
                    case '#':
                        dgvPali = s.Substring(1);
                        rtbPali = s;
                        break;
                    default:
                        // assume this is a comment line
                        marker = '#';
                        rtbPali = marker + s;
                        dgvPali = s;
                        dgvPlain = dgvFootnote = rtbPali = rtbFootnote = string.Empty;
                        break;
                }
                if ((dview & dvDataGrid) == dvDataGrid) 
                    populateDataGridView(marker, dgvPali, dgvPlain, dgvFootnote);
                if ((dview & dvTextBox) == dvTextBox)
                {
                    populateRichTextBoxView(codeView, rtbPali);
                    if (rtbPlain.Length > 0) populateRichTextBoxView(codeView, rtbPlain);
                    if (rtbFootnote.Length > 0) populateRichTextBoxView(codeView, rtbFootnote);
                }
            }

            private void populateDataGridView(char marker, string pali, string plain, string footnote)
            {
                string srno;

                srno = (++countTotal).ToString();
                if (dataGridView.InvokeRequired)
                {
                    var d = new safeCallAddDataGridView(safeCallDataGridViewAddRow);
                    richTextBox.Invoke(d, new object[] { srno, marker, pali, plain, footnote });
                }
                else
                    safeCallDataGridViewAddRow(srno, marker, pali, plain, footnote);
            }

            private void populateRichTextBoxView(int codeView, string s)
            {
                if (s.Length <= 1) return;

                // check for Pali text
                if (displayOn(s[0], codeView))
                {
                    Color color = GetColor(s[0]);
                    if (codeView < Form1.ViewFullDoc) s = s.Substring(1);

                    //safeCallRichTextAppend
                    if (richTextBox.InvokeRequired)
                    {
                        var d = new safeCallRichTextUpdate(appendRichTextBox);
                        richTextBox.Invoke(d, new object[] { color, s });
                    }
                    else
                        appendRichTextBox(color, s);
                }
            }

            private void appendRichTextBox(Color color, string s)
            {
                int start = richTextBox.Text.Length;
                // add a trailing blank to the existing text
                //if (start > 0 && richTextBox.Text[start - 1] != ' ') richTextBox.Text += " ";
                s += " ";
                int end = start + s.Length;
                richTextBox.Select(start, end);
                richTextBox.SelectionColor = color;
                richTextBox.AppendText(s);
            }

            public void clearDataGridView()
            {
                if (dataGridView.InvokeRequired)
                {
                    var d = new SafeDeleteRowsDelegate(removeDataGridViewRows);
                    richTextBox.Invoke(d, new object[] { });
                }
                else
                    removeDataGridViewRows();
            }

            private void removeDataGridViewRows()
            {
                try
                {
                    while (dataGridView.Rows.Count > 1)
                        dataGridView.Rows.RemoveAt(0);

                    if (dataGridView.Rows.Count == 1 && dataGridView.Rows[0].IsNewRow)
                    {
                        dataGridView.Rows[0].Cells[0].Value = null;// string.Empty;
                        dataGridView.Rows[0].Cells[1].Value = "*";
                        dataGridView.Rows[0].Cells[2].Value = string.Empty;
                        if (dataGridView.Columns[4].Visible) dataGridView.Rows[0].Cells[4].Value = string.Empty;
                    }

                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            private void removeDataGridViewRows2()
            {
                try
                {
                    while (dataGridView2.Rows.Count > 1)
                        dataGridView2.Rows.RemoveAt(0);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            public void RefreshDataGridView()
            {
                //updateDataView(ViewFullDoc, dvDataGrid);
                updateDataView(ViewTabular); //, dvDataGrid);
                return;
            }

            public void RefreshRichTextBox(int viewCode)
            {
                updateDataView(viewCode); //, dvTextBox);
                return;
            }

            private void appendRichTextAppend(int code, string s)
            {
                //richTextBoxdataGridView.Rows.Add(row);
                switch (code)
                {
                    case 0:
                        richTextBox.Clear();
                        break;
                    case 1:
                        richTextBox.Select(richTextBox.Text.Length, s.Length);
                        richTextBox.SelectionColor = GetColor(s[0]);
                        richTextBox.AppendText(s);
                        break;
                }
            }

            public void DataGridViewToFileContent()
            {
                string s = string.Empty;
                fileContent = string.Empty;
                dataGridView.EndEdit();
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    // for newly added row, assign empty strings to null fields
                    if (row.Cells[dgvColRecordType].Value == null) row.Cells[dgvColRecordType].Value = "*";
                    if (row.Cells[dgvColPali].Value == null) row.Cells[dgvColPali].Value = string.Empty;
                    if (row.Cells[2].Value == null) row.Cells[2].Value = string.Empty;
                    if (row.Cells[3].Value == null) row.Cells[3].Value = string.Empty;
                    if (row.Cells[4].Value == null) row.Cells[4].Value = string.Empty;
                    if (!row.IsNewRow)// && row.Cells[dgvColPali].Value != null && row.Cells[dgvColPali].Value.ToString().Length > 0)
                    {
                        switch (row.Cells[dgvColRecordType].FormattedValue.ToString())
                        {
                            case "#": // comment record
                                s = "#" + row.Cells[2].FormattedValue.ToString();
                                break;
                            case "*": // nissaya data record
                                s = "*" + row.Cells[2].FormattedValue.ToString();
                                s += " ^" + row.Cells[3].FormattedValue.ToString();
                                if (row.Cells[4].FormattedValue != null && row.Cells[4].FormattedValue.ToString().Length > 0)
                                    s += " @" + row.Cells[4].FormattedValue.ToString();
                                break;
                        }
                        fileContent += (fileContent.Length == 0) ? s : " " + s;
                    }
                }
            }

            public void RefreshRichTextBox()
            {
                string s = string.Empty;
                int code = 0;
                fileContent = string.Empty;

                if (richTextBox.InvokeRequired)
                {
                    var d = new safeCallRichTextAppend(appendRichTextAppend);
                    richTextBox.Invoke(d, new object[] { code, s });
                }
                else
                    appendRichTextAppend(code, s);

                code = 1;
                char c;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        switch(i)
                        {
                            case 1:
                                s = "*";
                                break;
                            case 2:
                                c = row.Cells[i].Value.ToString()[0];
                                if (c != '#' && c != '@') s = "^";
                                break;
                        }
                        s += row.Cells[i].Value.ToString();
                        if (richTextBox.InvokeRequired)
                        {
                            var d = new safeCallRichTextAppend(appendRichTextAppend);
                            richTextBox.Invoke(d, new object[] { code, s });
                        }
                        else
                            appendRichTextAppend(code, s);
                    }
                    fileContent += s;
                }
            }

            public static Color GetColor(char marker)
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

            public RichTextBox GetRichTextBox()
            {
                return richTextBox;
            }

            private bool displayOn(char marker, int codeView)
            {
                if (marker == '*' && codeView == 1 ||
                    marker == '^' && codeView == 2) return true;
                return codeView == 3 ? true : false;
            }

            private void safeCallDataGridViewAddRow(string srno, char recType, string pali, string plain, string footnote)
            {
                string[] row = { srno, recType.ToString(), pali, plain, footnote};
                if (UseSecondaryGridContainer)
                {
                    _NissayaDataRecord nisDataRec = new _NissayaDataRecord();
                    nisDataRec.srno = srno;
                    nisDataRec.marker = recType.ToString();
                    nisDataRec.pali = pali;
                    nisDataRec.plain = plain;
                    nisDataRec.footnote = footnote;
                    NissayaRecordList.Add(nisDataRec);
                    //dataGridView2.Rows.Add(row);
                }
                else dataGridView.Rows.Add(row);
            }

            private void parseNissayaDataRecord(string s, out string pali, out string plain, out string footnote)
            {
                // all parsed strings are returned with a trailing blank
                pali = plain = footnote = string.Empty;
                if (s[0] == '#')
                {
                    // comment record. no need to parse anything
                    pali = s.Substring(1).Trim(' ');
                    return;
                }

                int n = s.IndexOf('^');
                if (n != -1)
                {
                    pali = s.Substring(1, n - 1).Trim(' ');
                    int n1 = s.IndexOf('@', n + 1);
                    if (n1 == -1) plain = s.Substring(n + 1).Trim(' ');
                    else
                    {
                        plain = s.Substring(n + 1, n1 - n - 1).Trim(' ');
                        footnote = s.Substring(n1 + 1).Trim(' ');
                    }
                }
                else pali = s.Substring(1);
                return;
            }

            public void SaveFile(string outFile = "")
            {
                int n = 0;
                if (Pages.Count == 0 && fileContent.Length > 0)
                {
                    // this is a new file and has no pages parsed
                    // parse the pages into memory
                    parseIntoPages();
                }
                foreach(KeyValuePair<string, string> item in Pages)
                {
                    if (n == 0) fileContent = item.Value;
                    else
                        fileContent += paraNewLine + item.Value;
                    ++n;
                }
                try
                {
                    fMgr.SaveFile(GetHeader() + fileContent, outFile);
                    UpdateFileContent = false;
                }
                catch (UnauthorizedAccessException)
                {
                    FileAttributes attr = (new FileInfo(outFile)).Attributes;
                    string msg = "UnAuthorizedAccessException: Unable to access file ";
                    msg += outFile;
                    MessageBox.Show(msg);
                    //if ((attr & FileAttributes.ReadOnly) > 0)
                    //    Console.Write("The file is read-only.");
                }
            }
            private string GetHeader()
            {
                string header = string.Empty;
                if (MN.Length > 0) header = "{MN:" + MN + "}" + System.Environment.NewLine; 
                return header;
            }
            public string SaveCurrentPage(string pgno = "")
            {
                // save the fileContent to the current page
                string orgKey = (pgno.Length > 0) ? GetOrigKey(pgno) : curPage;
                if (orgKey.Length > 0 && Pages.ContainsKey(orgKey))
                    Pages[orgKey] = fileContent;
                else
                {
                    Pages.Add(pgno, fileContent);
                    orgKey = pgno;
                    KeyMapper.Add(new KeyMap(pgno, orgKey));
                }
                // check the page no in fileContent
                char[] pageNoDelimiters = {' ', '\n', '\r'};
                int pos = fileContent.IndexOfAny(pageNoDelimiters);
                string newKey = (pos != -1) ? fileContent.Substring(0, pos) : fileContent;
                if (newKey[0] == '#') newKey = newKey.Substring(1);
                if (newKey.Length > 3) newKey = newKey.Substring(0, 3); // shorten new key
                if (newKey != orgKey) UpdatePageKey(newKey, orgKey);
                // save current page to file
                //SaveFile();
                return newKey;
            }
            public void RichTextBoxToFileContent() 
            {
                if (richTextBox.InvokeRequired)
                {
                    var d = new safeCallGetRichText(getRichTextValue);
                        richTextBox.Invoke(d, new object[] { });
                }
                else
                    fMgr.fileContent = fileContent = richTextBox.Text;
            }

            private void getRichTextValue()
            {
                fMgr.fileContent = fileContent = richTextBox.Text;
            }
        }

        public class NissayaBook
        {
            string title;
            string author;
            string bookFileName;
            int ChapCount = 0;
            public class Chapter
            {
                public bool checkBox;
                public string chapName;
                public string fileName;

                public Chapter(bool chk, string cName, string fName) { checkBox = chk; chapName = cName; fileName = fName; }
            }
            public List<Chapter> chapters = new List<Chapter>();
            public string GetTitle() { return title; }
            public string GetAuthor() { return author; }
            public NissayaBook(string bfname, string tle, string auth) { title = tle; author = auth; bookFileName = bfname; }
            public NissayaBook(string bfname)
            {
                JSON.Json nbk = new JSON.Json(bfname);
                bookFileName = bfname;
                title = nbk.GetData("TITLE");
                author = nbk.GetData("AUTHOR");
                ChapCount = Convert.ToInt32(nbk.GetData("CHCOUNT"));
                string chName, chFile;
                for (int i = 0; i < ChapCount; i++)
                {
                    chName = nbk.GetData("CHAP" + (i + 1).ToString());
                    chFile = nbk.GetData("FILE" + (i + 1).ToString());
                    chapters.Add(new Chapter(true, chName, chFile));
                }
            }
            public NissayaBook()
            {
            }
            public void AddChapter(bool chkbox, string cName, string fName)
            {
                chapters.Add(new Chapter(chkbox, cName, fName));
            }

            public string[] NissayaBookFileData()
            {
                List<string> info = new List<string>();
                info.Add("AUTHOR|" + author.Trim());
                info.Add("TITLE|" + title.Trim());
                info.Add("CHCOUNT|" + chapters.Count.ToString());
                int n = 0;
                foreach (Chapter ch in chapters)
                {
                    ++n;
                    info.Add("CHAP" + n.ToString() + "|" + ch.chapName.Trim());
                    info.Add("FILE" + n.ToString() + "|" + ch.fileName.Trim());
                }

                return info.ToArray();
            }
        }
    }
}