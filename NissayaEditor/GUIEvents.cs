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
using System.Drawing.Printing;

namespace NissayaEditor
{
    public partial class Form1 : Form
	{
        private void button_Quit_Click(object sender, EventArgs e)
        {
            //MessageForm messageForm = new MessageForm(0, 1);//, -1);
            //int resp = messageForm.ShowDialog("Confirm", "Do you to save changes to the file?", "Don't Save", "Save", "Cancel");
            this.Close();
            //Application.Exit();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // File load
            //var fileContent = string.Empty;
            //var filePath = string.Empty;

            //quitAppendMode();
            // if current page has unsaved data; prompt the user

            if (IsNewPageWithData()) return;
            button_X_Click(this, null); // clear find results if any
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = fileFilters;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //addMRUBegin(openFileDialog.FileName);
                    if (openFileList.Contains(openFileDialog.FileName)) return;
                    textBox_MN.Text = label_MNTitle.Text = string.Empty;
                    openFileList.Add(openFileDialog.FileName);
                    if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".nbk")
                    {
                        loadBook(openFileDialog.FileName, true);
                    }
                    else loadFile(openFileDialog.FileName);
                }
            }
        }
        
        // #PALI
        private void paliTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curViewCode == 1) return;
            //quitAppendMode();
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            if (!ntp.DataFileExist() && (ntp.dataGridView1_Updated || ntp.richTextBox1_Updated))
            {
                MessageBox.Show(SaveDataToFile);
                return;
            }
            button_Find.Enabled = false;
            curViewCode = 1;
            //if (ntp.UpdateNotSaved())
            //{
            //    MessageBox.Show(SaveDataToFile);
            //    return;
            //}
            RichTextBox rtb = ntp.GetRichTextBox();
            ntp.RefreshRichTextBox(ViewPali);
            //preListView(true);
            curViewCode = ViewPali;
            //refreshRichTextBoxDisplay(curViewCode);
            currentViewMenu = ViewMenu.Pali;
            //ntp.ColorFindData();
            //ntp.SelectFindText();
        }

        // #PLAIN
        private void plainTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curViewCode == 2) return;
            //quitAppendMode();
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            if (!ntp.DataFileExist() && (ntp.dataGridView1_Updated || ntp.richTextBox1_Updated))
            {
                MessageBox.Show(SaveDataToFile);
                return;
            }
            button_Find.Enabled = false;
            curViewCode = 2;
            RichTextBox rtb = ntp.GetRichTextBox();
            ntp.RefreshRichTextBox(ViewPlain);
            curViewCode = ViewPlain;
            currentViewMenu = ViewMenu.Plain;
        }

        // #FULL
        private void fullDocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curViewCode == 3) return;
            //ChangeViewNissayaTabPage();
            if (newDesign)
            {
                NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
                if (!ntp.DataFileExist() && (ntp.dataGridView1_Updated || ntp.richTextBox1_Updated))
                {
                    MessageBox.Show(SaveDataToFile);
                    return;
                }
                button_Find.Enabled = true;
                curViewCode = 3;
                ntp.RefreshRichTextBox(ViewFullDoc);
                return;
            }
        }
        /*
        private void ChangeViewNissayaTabPage()
        {
            foreach (NissayaTabPage ntp in nissayaTabPageList) //[tabControl1.SelectedIndex];
                ntp.InitView(curViewCode);
        }
        */
        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (currentViewMenu == ViewMenu.Tabular) return;
            //ChangeViewNissayaTabPage();
            if (curViewCode == 4) return;

            if (newDesign)
            {
                NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
                if (!ntp.DataFileExist() && (ntp.dataGridView1_Updated || ntp.richTextBox1_Updated))
                {
                    MessageBox.Show(SaveDataToFile);
                    return;
                }
                button_Find.Enabled = true;
                curViewCode = 4;
                ntp.TableViewToggle(sender, e);
                return;
            }
        }

        // #ABOUT
        About about = new About(progname, version);
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Control ctrl = this.ActiveControl;
            //RichTextBox rtb = nissayaTabPageList[tabControl1.SelectedIndex].GetRichTextBox();
            //var e = rtb.KeyDown;
            if (about != null)
                about.ShowDialog();
        }
        
        string stringToPrint = string.Empty;

        //**********************************************************************
        // #PRINT
        private void printCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = nissayaTabPageList[tabControl1.SelectedIndex].GetRichTextBox();
            DataGridView dgv = nissayaTabPageList[tabControl1.SelectedIndex].GetDataGridView();
            if (rtb.Text.Length == 0 && dgv.Rows.Count == 1)
            {
                MessageBox.Show("The page is empty. Nothing to print.");
                return;
            }
            if (rtb.Text.Length == 0 && dgv.Rows.Count > 1)
            {
                MessageBox.Show("Switch to textbox view to print.");
                return;
            }
            PrintDialog pDlg = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            pDlg.Document = printDocument;
            pDlg.AllowSelection = true;
            pDlg.AllowSomePages = true;
            if (pDlg.ShowDialog() == DialogResult.OK)
            {
                //RichTextBox rtb = nissayaTabPageList[tabControl1.SelectedIndex].GetRichTextBox();
                stringToPrint = rtb.Text;
                int len = stringToPrint.Length;
                //printDocument.PrintPage -= PrintDocumentOnPrintPage;
                //printDocument.PrintPage += PrintDocumentOnPrintPage;
                
                printDocument.PrintPage -= printDocument_PrintPage;
                printDocument.PrintPage += printDocument_PrintPage;
                printDocument.Print();
            }
        }

        // #PRINT
        private void PrintDocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            // Draw a picture.
            RichTextBox rtb = nissayaTabPageList[tabControl1.SelectedIndex].GetRichTextBox();
            stringToPrint = rtb.Text;
            //            e.Graphics.DrawString(this.richTextBox1.Text, this.richTextBox1.Font, Brushes.Black, 10, 25);
            e.Graphics.DrawString(stringToPrint, rtb.Font, Brushes.Black,
                new Rectangle(50, 50, (int)e.Graphics.VisibleClipBounds.Width - 80, (int)e.Graphics.VisibleClipBounds.Height - 80));
            // Indicate that this is the last page to print.
            e.HasMorePages = false;
        }

        // #PRINT
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int charactersOnPage = 0;
            int linesPerPage = 0;
            RichTextBox rtb = nissayaTabPageList[tabControl1.SelectedIndex].GetRichTextBox();
            // Sets the value of charactersOnPage to the number of characters
            // of stringToPrint that will fit within the bounds of the page.
            e.Graphics.MeasureString(stringToPrint, rtb.Font,
                e.MarginBounds.Size, 
                StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            // Draws the string within the bounds of the page
            e.Graphics.DrawString(stringToPrint, rtb.Font, Brushes.Black,
                e.MarginBounds,
                //new Rectangle(50, 50, (int)e.Graphics.VisibleClipBounds.Width - 80, (int)e.Graphics.VisibleClipBounds.Height - 80),
                StringFormat.GenericTypographic);

            // Remove the portion of the string that has been printed.
            //int len = stringToPrint.Length;
            stringToPrint = stringToPrint.Substring(charactersOnPage);
            //len = stringToPrint.Length;

            // Check to see if more pages are to be printed.
            e.HasMorePages = (stringToPrint.Length > 0);
        }
        //**********************************************************************

        //**********************************************************************
        // #SAVE
        private void button_Save_Click(object sender, EventArgs e)
        {
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            RichTextBox rtb = ntp.GetRichTextBox();
            if (rtb.Text.Length == 0 && ntp.GetDataGridView().Rows.Count == 1) return;
            if (ntp.tabPage.Text.Contains("New"))
            {
                button_SaveAs_Click(sender, e);
                return;
            }
            //quitAppendMode();
            ntp.SavePage();
            ntp.SaveFile();
            addMRUBegin(ntp.FileName);
            //dataFileList[curTabIndex].SaveFile();
            
            // no need to check for tabPage selecting
            // after the file is saved.
            // this is required for new page not saved situation
            tabControl1.Selecting -= TabControl1_Selecting;

            //timer.Interval = 1000;
            //timer.Enabled = true;
            //timer.Tick += new EventHandler(t_Elapsed);

            MessagingForm msgForm = new MessagingForm();
            msgForm.SetImage(1);
            msgForm.StartPosition = FormStartPosition.CenterParent;
            msgForm.Show(this);
            fileUpdated = false;
        }

        // #SAVE
        private void t_Elapsed(object sender, EventArgs e)
        {
            timer.Enabled = false;
            SendKeys.Send("{ESC}");
        }

        List<NissayaTabPage> FindTabList = new List<NissayaTabPage>();
        // #SAVEAS
        private void button_SaveAs_Click(object sender, EventArgs e)
        {
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            if (ntp.GetRichTextBox().Text.Length == 0 && ntp.GetDataGridView().Rows.Count == 1) return;

            string fname = string.Empty;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = fileFilters;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;
            fname = saveFileDialog1.FileName;
            quitAppendMode();

            if (fileCount == 0) 
            {
                ++fileCount;
                curTabIndex = 0;
             }
            ntp.SavePage();
            ntp.SetFile(saveFileDialog1.FileName);
            ntp.SaveFile();

            // no need to check for tabPage selecting
            // after the file is saved.
            // this is required for new page not saved situation
            tabControl1.Selecting -= TabControl1_Selecting;

            Form1_TabSelectedIndexChanged(null, null);
            addMRUBegin(ntp.FileName);

            MessagingForm msgForm = new MessagingForm();
            msgForm.SetImage(1);
            msgForm.StartPosition = FormStartPosition.CenterParent;
            msgForm.Show(this);
            fileUpdated = false;
        }
        //**********************************************************************
        
        private void newCtrlNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // it has submenu; so no code required here
            pageMenuBar.Enabled = true;
            TabControl tc = tabControl1;

            //TabControl tc = tabControl1;
            //TabPage tb = new TabPage(); //new NissayaTabPage(this, tc, textBox1)
            //tc.TabPages.Add(tb);
            //nissayaTabPageList.Add(new NissayaTabPage(this, tc, tb, textBox1)); //(NissayaTabPage)tb);
            //tc.SelectedIndex = tc.TabCount - 1;
        }

        private int CurFindTabIndex = -1;
        //private string CurFindPageNo = string.Empty;
        private string StartFindPageNo = string.Empty;
        private List<string> CurTabPageList = new List<string>();

        private int nCurTabFindIndex;
        private int nCurPageFindIndex;
        private int nCurTabStartFindIndex;
        private int nCurFindListIndex;
        private NissayaTabPage ntpFind;
        private List<NissayaTabPage> findTabPageList;

        //long start_time0;
        private void button_Find_Click(object sender, EventArgs e)
        {
            // if the current page is new with data, prompt to save 
            if (IsNewPageWithData()) return;
            // if search string is empty or text data is empty, just return
            if (textBox_Find.Text.Length == 0 ||
                (nissayaTabPageList.Count == 1 && curViewCode != ViewTabular &&
                nissayaTabPageList[0].GetRichTextBox().Text.Length == 0) ||
                (nissayaTabPageList.Count == 1 && curViewCode == ViewTabular &&
                nissayaTabPageList[0].GetDataGridView().Rows.Count <= 1)) return;

            textBox_Find.Text = MyanmarCharsCleanup(textBox_Find.Text); // clean up Myanmar text

            ClearPagesFindResults();
            CurFindTabIndex = tabControl1.SelectedIndex;
            StartFindPageNo = pageMenuBar.GetCurrentPageNo();

            Cursor = Cursors.WaitCursor;
            nCurTabFindIndex = tabControl1.SelectedIndex;
            //FindSeqNissayaTabPage(tabControl1.SelectedIndex);
            findTabPageList = new List<NissayaTabPage>(nissayaTabPageList);
            ntpFind = nissayaTabPageList[tabControl1.SelectedIndex];
            // find in the current selected tab
            bool[] flagViewMenu = new bool[] { true, true, true, true, true };
            if (curViewCode == ViewTabular)
            {
                flagViewMenu[ViewPali] = false;
                flagViewMenu[ViewPlain] = false;
                flagViewMenu[ViewFullDoc] = false;
                EnableViewMenu(flagViewMenu);
                this.ActiveControl = dataGridView1;
                // 2021-06-14
                ntpFind.curFindPgNo = ntpFind.curPageNo;
                //((NissayaDataGridView)(ntpFind.GetDataGridView())).dataGridViewFind();
                nCurTabStartFindIndex = 0;
                ((NissayaDataGridView)(ntpFind.GetDataGridView())).dataGridViewFind();
                FindNextGridAllTabPages();
                return;
            }

            flagViewMenu[ViewPali] = false;
            flagViewMenu[ViewPlain] = false;
            flagViewMenu[ViewTabular] = false;
            EnableViewMenu(flagViewMenu);
            
            //*************** find on the current active page *********************//
            ntpFind.richTextBoxFind();

            nCurPageFindIndex = ntpFind.GetPageList().IndexOf(ntpFind.GetCurrentPage());
            nCurTabStartFindIndex = ntpFind.GetPageList().IndexOf(ntpFind.GetCurrentPage());
            nCurFindListIndex = 0;

            FindNextAllTabPages(ntpFind.findResults.Count == 0);
        }

        public void logElapsedTime(string s, long milliSec)
        {
            FileStream fileStream = new FileStream("log.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine(s + " Elapsed Time = " + milliSec.ToString() + " ms");
            writer.Flush();
            writer.Close();
        }

        private void FindNextAllTabPages(bool loadFindPage)
        {
            // ntpFind = currently SelectedIndex tab's nissayaTabPage
            bool flagNewTab = false;
            bool flagComplete = false;
            //PageFindInfo pgInfoData = null;
            List<string> curPageList = ntpFind.GetPageList();   // get current tab's page list
            nCurTabStartFindIndex = curPageList.IndexOf(ntpFind.curFindPgNo);   // find index of curFindPgNo

            if (++nCurTabFindIndex >= tabControl1.TabCount) nCurTabFindIndex = 0;
            if (nCurTabFindIndex == tabControl1.SelectedIndex)
                flagComplete = true;
            else
                flagNewTab = true;

            while (!flagComplete)
            {
                //if (flagNewTab)
                //{
                    ntpFind = nissayaTabPageList[nCurTabFindIndex];
                    if (ntpFind.tabPage.Text.IndexOf("<New") != -1 || !ntpFind.ValidDataInfo())   // 
                    {
                        flagComplete = true; continue;
                    }
                    curPageList = ntpFind.GetPageList();// get current tab's page list
                    ntpFind.curFindPgNo = curPageList[0];
                    ntpFind.InitView(curViewCode);
                    //ntpFind = nissayaTabPageList[nCurTabFindIndex];
                    //pgInfoData = GetPageInfoListView(curViewCode)[0];
                    flagNewTab = false;
                    nCurTabStartFindIndex = nCurPageFindIndex = curPageList.IndexOf(ntpFind.curFindPgNo);   // find index of curFindPgNo
                    //**** do FIND ****
                    //ntpFind.richTextBoxPageFind(curPageList[nCurPageFindIndex]);
                    ntpFind.richTextBoxFind(loadFindPage);
                    //if (loadFindPage)
                    //{
                    //    loadFindPage = ntpFind.findResults.Count == 0;
                    //    if (ntpFind.findResults.Count > 0) 
                    //        tabControl1.SelectedIndex = nCurTabFindIndex;
                    //}
                //}
                //else
                //{
                //    nCurPageFindIndex = curPageList.IndexOf(ntpFind.curFindPgNo);   // find index of curFindPgNo
                //    if (++nCurPageFindIndex >= curPageList.Count) nCurPageFindIndex = 0;    // increment index, if passes limit goto the first
                //    if (nCurPageFindIndex != nCurTabStartFindIndex)             // if index is not the same as start index, continue
                //    {
                //        // continue with the next page to find text
                //        if (newThread)
                //        {
                //            ntpFind.DoFindThread();
                //            return;
                //        }
                //        else
                //        {
                //            //**** do FIND ****
                //            ntpFind.richTextBoxPageFind(curPageList[nCurPageFindIndex]);
                //        }
                //    }
                //    else
                //    {
                //        // the current tab is done
                //        // move to the next tab
                //        if (++nCurTabFindIndex >= tabControl1.TabCount) nCurTabFindIndex = 0;
                //        if (nCurTabFindIndex == tabControl1.SelectedIndex)
                //            flagComplete = true;
                //        else
                //            flagNewTab = true;
                //    }
                //}
                if (++nCurTabFindIndex >= tabControl1.TabCount) nCurTabFindIndex = 0;
                if (nCurTabFindIndex == tabControl1.SelectedIndex)
                    flagComplete = true;
                else
                    flagNewTab = true;
            }

            List<PageFindInfo> pgInfoDataList = GetPageInfoListView(curViewCode);
            if (pgInfoDataList.Count > 0)
            {
                ntpFind = nissayaTabPageList[tabControl1.SelectedIndex];
                CurActivePageFindInfo = pgInfoDataList[nCurFindListIndex];
                if (ntpFind.FileName != CurActivePageFindInfo.nissayaTabPage.FileName)
                {
                    int index = tabControl1.TabPages.IndexOf(CurActivePageFindInfo.nissayaTabPage.tabPage);
                    tabControl1.SelectedTab = tabControl1.TabPages[index];
                    //tabControl1.SelectedIndex = index;
                    LoadNextFindPage(pgInfoDataList[nCurFindListIndex], flagNewTab);
                }
                else
                {
                    CurActivePageFindInfo.nissayaTabPage.LoadPage(CurActivePageFindInfo.pageNo);
                }
                pageMenuBar.FillPageMenu(CurActivePageFindInfo.nissayaTabPage.GetPageList(), CurActivePageFindInfo.pageNo);
                nCurFindListIndex = 0;
                CurPageHighlightType = HightlightType.HighlightText;
                CurActivePageFindInfo.nissayaTabPage.richTextBoxFindNext();
                //    nCurFindListIndex = 0;      // first Find page in the list
            //    pgInfoData = pgInfoDataList[nCurFindListIndex];
            //    // if the first occurrence of search text is in the currently 
            //    // opened tab/page then just return
            //    ntpFind = nissayaTabPageList[tabControl1.SelectedIndex];
            //    string pgno = pgInfoData.nissayaTabPage.GetCurrentPage();
            //    if (pgInfoData.nissayaTabPage != ntpFind ||
            //        pgno != pageMenuBar.GetCurrentPageNo())
            //    {
            //        // the first occurrence is in another page
            //        // load that page and do highlights
            //        ntpFind.SavePage();
            //        ntpFind = pgInfoData.nissayaTabPage;
            //        ntpFind.LoadPage(pgno);
            //        //pgInfoData.nissayaTabPage.HighlightFindData();
            //        //nListFindIndex++;
            //        //UpdateTotalFindIndexView(1);
            //        pageMenuBar.FillPageMenu(ntpFind.GetPageList(), pgno);
            //        //pageMenuBar.NewPageSelected(pgno);
            //        nCurTabFindIndex = 0;   // this is the first Tab of Find results
            //        int index = tabControl1.TabPages.IndexOf(ntpFind.tabPage);
            //        tabControl1.SelectedTab = tabControl1.TabPages[index];
            //        pgInfoData.nCurPageFindIndex = 0;  // first occurrence of 
            //    }
            }
            //ntpFind.HighlightFindData();
            // load first page found
            //parentForm.CurActivePageFindInfo = pageFindInfo;
            //parentForm.CurPageHighlightType = HightlightType.HighlightText;
            //richTextBoxFindNext();
            SetTotalFindIndexView(1);
            textBox_FindStatus.Text = GetFindStatusView();
            Cursor = Cursors.Default;
            EnableFindNavButtons();
        }

        private void LoadNextFindPage(PageFindInfo pgInfoData, bool flagNewTab, bool flagFirstIndex = true)
        {
            string pageNo = pgInfoData.pageNo;
            pgInfoData.nissayaTabPage.curFindPgNo = pageNo;
            pgInfoData.nissayaTabPage.curViewCode = curViewCode;
            pgInfoData.nissayaTabPage.LoadPage(pageNo);

            //pgInfoData.flagHighlightDrawn = false;
            pgInfoData.nCurPageFindIndex = (flagFirstIndex) ? -1 : pgInfoData.matches.Count;
            //pgInfoData.nissayaTabPage.richTextBoxFindNext();
            //pgInfoData.nissayaTabPage.HighlightFindData();
            if (flagNewTab)
            {
                pageMenuBar.FillPageMenu(pgInfoData.nissayaTabPage.GetPageList(), pageNo);
                int index = tabControl1.TabPages.IndexOf(pgInfoData.nissayaTabPage.tabPage);
                tabControl1.SelectedTab = tabControl1.TabPages[index];
            }
            pageMenuBar.NewPageSelected(pageNo);
        }

        private void LoadNextFindPage(DataGridViewFindPage dataGridViewFindResults, bool flagNewTab)
        {
            string pageNo = dataGridViewFindResults.pageNo;
            NissayaTabPage ntp = dataGridViewFindResults.nissayaDataGridView.parentInstance;
            
            ntp.curViewCode = curViewCode;
            ntp.LoadPage(pageNo);

            if (flagNewTab)
            {
                pageMenuBar.FillPageMenu(ntp.GetPageList(), pageNo);
                int index = tabControl1.TabPages.IndexOf(ntp.tabPage);
                tabControl1.SelectedTab = tabControl1.TabPages[index];
            }
            pageMenuBar.NewPageSelected(pageNo);
        }

        private void FindNextGridAllTabPages()
        {
            bool flagNewTab = false;
            bool flagComplete = false;
            NissayaTabPage ntp;
            DataGridViewFindPage pgGridFindInfoData = CurActiveGridPageFindInfo;
            List<string> curPageList = ntpFind.GetPageList();   // get current tab's page list
            nCurTabStartFindIndex = curPageList.IndexOf(ntpFind.curFindPgNo);   // find index of curFindPgNo

            if (++nCurTabFindIndex >= tabControl1.TabCount) nCurTabFindIndex = 0;
            if (nCurTabFindIndex == tabControl1.SelectedIndex)
                flagComplete = true;
            else
                flagNewTab = true;


            while (!flagComplete)
            {
                if (flagNewTab)
                {
                    ntpFind = nissayaTabPageList[nCurTabFindIndex];
                    if (ntpFind.ValidDataInfo())
                    {
                        curPageList = ntpFind.GetPageList();// get current tab's page list
                        ntpFind.curFindPgNo = curPageList[0];
                        ntpFind.InitView(curViewCode);
                        //ntpFind = nissayaTabPageList[nCurTabFindIndex];
                        //pgInfoData = GetPageInfoListView(curViewCode)[0];
                        flagNewTab = false;
                        nCurTabStartFindIndex = nCurPageFindIndex = curPageList.IndexOf(ntpFind.curFindPgNo);   // find index of curFindPgNo
                        ntpFind.UseSecondaryGridContainer(true);
                        ntpFind.richTextBoxPageFind(curPageList[nCurPageFindIndex]);
                        ntpFind.UseSecondaryGridContainer(false);
                    }
                }
                else
                {
                    nCurPageFindIndex = curPageList.IndexOf(ntpFind.curFindPgNo);   // find index of curFindPgNo
                    if (++nCurPageFindIndex >= curPageList.Count) nCurPageFindIndex = 0;    // increment index, if passes limit goto the first
                    if (nCurPageFindIndex != nCurTabStartFindIndex)             // if index is not the same as start index, continue
                    {
                        //ntpFind.SetDataContainers2();
                        ntpFind.UseSecondaryGridContainer(true);
                        ntpFind.curFindPgNo = curPageList[nCurPageFindIndex];
                        ntpFind.GetPageData(ntpFind.curFindPgNo);
                        ntpFind.GetDataGridView().dataGridViewFind();
                        ntpFind.UseSecondaryGridContainer(false);
                    }
                    else
                    {
                        // the current tab is done
                        // move to the next tab
                        if (++nCurTabFindIndex >= tabControl1.TabCount) nCurTabFindIndex = 0;
                        if (nCurTabFindIndex == tabControl1.SelectedIndex)
                            flagComplete = true;
                        else
                            flagNewTab = true;
                    }
                }
                // the current tab is done
                // move to the next tab
                if (++nCurTabFindIndex >= tabControl1.TabCount) nCurTabFindIndex = 0;
                if (nCurTabFindIndex == tabControl1.SelectedIndex)
                    flagComplete = true;
                else
                    flagNewTab = true;
            }
            if (DataGridViewFindPageList.Count > 0)
            {
                pgGridFindInfoData = DataGridViewFindPageList.First().Value;
                CurActiveGridPageFindInfo = pgGridFindInfoData;
                
                //CurActiveGridPageFindInfo.nissayaTabPage.FileName
                ntp = nissayaTabPageList[nCurTabFindIndex];
                if (ntp.FileName != CurActiveGridPageFindInfo.nissayaTabPage.FileName)
                {
                    int index = tabControl1.TabPages.IndexOf(CurActiveGridPageFindInfo.nissayaTabPage.tabPage);
                    tabControl1.SelectedTab = tabControl1.TabPages[index];
                    LoadNextFindPage(CurActiveGridPageFindInfo, true);
                }
                else
                {
                    CurActiveGridPageFindInfo.nissayaTabPage.LoadPage(CurActiveGridPageFindInfo.pageNo);
                }
                //else
                //    ntp.curPageNo != CurActiveGridPageFindInfo.pageNo)
                //{
                //    LoadPageData(CurActiveGridPageFindInfo.pageNo);
                //    List<string> lsPages = ntpFind.GetPageList();
                //    pageMenuBar.FillPageMenu(lsPages, CurActiveGridPageFindInfo.pageNo);
                //}
                //else
                //    pageMenuBar.NewPageSelected(CurActiveGridPageFindInfo.pageNo);

                pageMenuBar.FillPageMenu(CurActiveGridPageFindInfo.nissayaTabPage.GetPageList(), CurActiveGridPageFindInfo.pageNo);
                CurPageHighlightType = HightlightType.HighlightSelectedText | HightlightType.HighlightText;
                ntp = CurActiveGridPageFindInfo.nissayaTabPage; // DataGridViewFindPageList.First().Value.nissayaTabPage;
                ntp.GetDataGridView().dataGridViewFindNext();
            }
            //dgvp.nissayaDataGridView.dataGridViewFindNext();
            // make the first selected cell the current cell
            //CurActiveGridPageFindInfo.nissayaDataGridView.CurrentCell =
            //    CurActiveGridPageFindInfo.nissayaDataGridView[CurActiveGridPageFindInfo.nCurSelectedColIndex, CurActiveGridPageFindInfo.nCurSelectedRowIndex];
            SetTotalFindIndexView(1);
            textBox_FindStatus.Text = GetFindStatusView();
            Cursor = Cursors.Default;
            EnableFindNavButtons();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridValue1_Updated = true;
            fileUpdated = true;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1_Updated = true;
            if (richTextBox1.SelectionStart <= indexCurCursor) return;
            string s = ((RichTextBox)sender).Text;
            string newText = s.Substring(indexCurCursor, richTextBox1.SelectionStart - indexCurCursor);
            if (newText.Length > 0)
                adjustTextColor(newText);
            fileUpdated = true;
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            indexCurCursor = richTextBox1.SelectionStart;
            caretPosition = indexCurCursor;
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            indexCurCursor = richTextBox1.SelectionStart;
        }

        public void richTextBox1_KeyDown2(object sender, KeyEventArgs e)
        {
            if (nissayaTabPageList[tabControl1.SelectedIndex].HandleMenuShortCuts(sender, e)) return;
            
            if  (e.KeyCode == Keys.F3)
            {
                if (currentViewMenu == ViewMenu.Tabular) dataGridViewFindNext();
                else richTextBoxFindNext();
            }
        }

        private void richTextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyValue == 54)
            {
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
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
       {
           if (nissayaTabPageList[tabControl1.SelectedIndex].HandleMenuShortCuts(sender, e)) return;

           if (e.KeyCode == Keys.F3)
           {
               if (currentViewMenu == ViewMenu.Tabular) dataGridViewFindNext();
               else richTextBoxFindNext();
           }
       }

        private bool handleMenuShortCuts(object sender, KeyEventArgs e)
        {
            if (dbgTrace != null)
            {
                string s = "handleMenuShortCuts() KeyData = " + e.KeyData.ToString();
                s += " KeyData = " + e.KeyData.ToString();
                s += " KeyCode= " + e.KeyCode.ToString();
                dbgTrace.WriteLine(s);
            }

            // Save-As
            if (e.Control && e.KeyCode == Keys.A)
            {
                button_SaveAs_Click(sender, e);
                return true;
            }
            // Save file
            if (e.Control && e.KeyCode == Keys.S)
            {
                button_Save_Click(sender, e);
                return true;
            }
            // Quit app
            if (e.Control && e.KeyCode == Keys.Q)
            {
                button_Quit_Click(sender, e);
                return true;
            }
            // Open file
            if (e.Control && e.KeyCode == Keys.O)
            {
                loadToolStripMenuItem_Click(sender, e);
                return true;
            }
            // new file
            if (e.Control && e.KeyCode == Keys.N)
            {
                newCtrlNToolStripMenuItem_Click(sender, e);
                return true;
            }
            // Close tab
            if (e.Control && e.KeyCode == Keys.W)
            {
                closeToolStripMenuItem_Click(sender, e);
                return true;
            }
            if (e.Control && e.KeyCode == Keys.P)
            {
                printCtrlPToolStripMenuItem_Click(sender, e);
                return true;
            }
            // Pali Text Only Ctrl-J
            if (e.Control && e.KeyCode == Keys.J)
            {
                if (currentViewMenu != ViewMenu.Pali)
                    paliTextToolStripMenuItem_Click(sender, e);
                return true;
            }
            // Plain Text Only Ctrl-K
            if (e.Control && e.KeyCode == Keys.K)
            {
                if (currentViewMenu != ViewMenu.Plain)
                    plainTextToolStripMenuItem_Click(sender, e);
                return true;
            }
            // Full Doc Ctrl-L
            if (e.Control && e.KeyCode == Keys.L)
            {
                if (currentViewMenu != ViewMenu.Full)
                    fullDocToolStripMenuItem_Click(sender, e);
                return true;
            }
            // Toggle Tabular view
            if (e.Control && e.KeyCode == Keys.T)
            {
                if (currentViewMenu != ViewMenu.Tabular)
                    tableToolStripMenuItem_Click(sender, e);
                return true;
            }
            // Data Box Ctrl-D 
            if (e.Control && e.KeyCode == Keys.D && nissayaTabPageList[tabControl1.SelectedIndex].dataGridView1_Visible())
            {
                dataBoxCtrlDToolStripMenuItem_Click(sender, e);
                return true;
            }
            //if (e.Shift && e.KeyCode == Keys.D6)
            //{
            //    e.Handled = false;
            //    KeyDownHandled = true;
            //    richTextBox1_InsertText(sender, "^");
            //    return false;
            //}
            // Ctrl-^ file
            //if (e.Control && e.KeyCode == Keys.D6)
            //{
            //    e.Handled = false;
            //    //e.KeyCode = 10;
            //    //int index = richTextBox1.GetCharIndexFromPosition(pt);
            //    // instead of Ctrl-6 insert ^
            //    //int pos = richTextBox1.SelectionStart;
            //    //richTextBox1.Text = richTextBox1.Text.Insert(pos, "^");
            //    //richTextBox1.Select(pos + 1, 0);
            //    //richTextBox1.Select(0, 0);
            //    //Clipboard.SetText("^");
            //    //richTextBox1.Paste();
            //    KeyDownHandled = true;
            //    richTextBox1_InsertText(sender, "^");
            //    return true;
            //}
            //// Ctrl-2 = @
            //if (e.Control && e.KeyCode == Keys.D2)
            //{
            //    e.Handled = true;
            //    richTextBox1_InsertText(sender, "@");
            //    return true;
            //}
            // Ctrl-3 = #
            if (e.Control && e.KeyCode == Keys.D3)
            {
                e.Handled = true;
                richTextBox1_InsertText(sender, "#");
                return true;
            }
            // Ctrl-8 = *
            if (e.Control && e.KeyCode == Keys.D8)
            {
                e.Handled = true;
                richTextBox1_InsertText(sender, "*");
                return true;
            }
            return false;
        }

        public void dataBoxCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            curNTP = nissayaTabPageList[nCurTabFindIndex];
            if (!newDesign || curViewCode != 4 || curNTP == null ||
                !curNTP.GetDataGridView().Enabled) return;
            //dataBoxCtrlDToolStripMenuItem.Checked = !dataBoxCtrlDToolStripMenuItem.Checked;
            //if (tabControl1.SelectedIndex != -1)
            //    nissayaTabPageList[tabControl1.SelectedIndex].ShowDataBox(dataBoxCtrlDToolStripMenuItem.Checked);
        }

        private void richTextBox1_InsertText(object sender, string t)
        {
            int pos;
            string s = sender.GetType().ToString();
            if (s.IndexOf("RichTextBox") != -1 || s.IndexOf("Tab") != -1)
            {
                RichTextBox rtb = nissayaTabPageList[tabControl1.SelectedIndex].GetRichTextBox();

                //rtb.TextChanged -= richTextBox1_TextChanged;
                
                //pos = rtb.SelectionStart;

                //richTextBox1.Text = richTextBox1.Text.Insert(pos, t);
                //richTextBox1.SelectionStart = pos + 1;
                //richTextBox1.Select(pos + 1, 0);

                //rtb.Select(pos, 0);
                rtb.SelectionLength = 0;
                rtb.SelectedText = t;
                //rtb.Text.Insert(pos, t);
                //Clipboard.SetText(t);
                //rtb.Paste();
                //richTextBox1.SelectedText = t;
                //richTextBox1.TextChanged += richTextBox1_TextChanged;
            }
            else
            {
                pos = textBox_Find.SelectionStart;
                richTextBox1.Select(pos, 0);
                //Clipboard.SetText(t);
                textBox_Find.SelectionLength = 0;
                textBox_Find.SelectedText = t;
                //textBox_Find.Paste();
            }
        }

        private void button_X_Click(object sender, EventArgs e)
        {
            textBox_Find.Clear();
            textBox_FindStatus.Text = string.Empty;
            // all views from 1 - 4 (0 unused) are true
            bool[] flagViewMenu = new bool[] { true, true, true, true, true };
            EnableViewMenu(flagViewMenu);

            ClearPagesFindResults();
            nissayaTabPageList[tabControl1.SelectedIndex].richTextBoxFindClear();
            ((NissayaDataGridView)nissayaTabPageList[tabControl1.SelectedIndex].GetDataGridView()).DataGridViewFindClear(true);
        }

        private void EnableViewMenu(bool[] flagMenu)
        {
            paliTextToolStripMenuItem.Enabled = flagMenu[ViewPali];
            plainTextToolStripMenuItem.Enabled = flagMenu[ViewPlain];
            fullDocToolStripMenuItem.Enabled = flagMenu[ViewFullDoc];
            tableToolStripMenuItem.Enabled = flagMenu[ViewTabular];
        }

        public void textBox_Find_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = true;
            if (nissayaTabPageList[tabControl1.SelectedIndex].KeyDownHandled)
            {
                e.Handled = true;
                nissayaTabPageList[tabControl1.SelectedIndex].KeyDownHandled = false;
            }
        }

        private void textBox_Find_KeyDown(object sender, KeyEventArgs e)
        {
            if (nissayaTabPageList[tabControl1.SelectedIndex].HandleMenuShortCuts(sender, e)) return;

            if (e.KeyValue == 13) button_Find_Click(sender, e);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                ((NissayaDataGridView)(nissayaTabPageList[tabControl1.SelectedIndex].GetDataGridView())).dataBox_EnterPressed();
            }
        }
        
        // Event-Quit
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            saveMRU();
            foreach (NissayaTabPage ntp in nissayaTabPageList)
            {
                if (ntp.UpdateNotSaved())
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to quit without saving file " + ntp.FileName + "?",
                        "Save or Quit", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dialogResult == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int MN_No;
            string bk = string.Empty;
            string MRU_Bk = string.Empty;
            Boolean MNBook = false;
            int curSelectedIndex = tabControl1.SelectedIndex;
            NissayaTabPage ntp = nissayaTabPageList[tabControl1.SelectedIndex];
            MN_No = ntp.MN_No;
            if (ntp.MNBookFile)
            {
                MNBook = true;
                bk = "MN-" + ntp.MN_No.ToString();
                MRU_Bk = "{ " + bk + " }";
                if (mnCatalog.GetFileCount(ntp.MN_No) > 1)
                {
                    DialogResult res = MessageBox.Show("This file is part of " + bk + " sutta " +
                        "and all files of " + bk + " will be closed. Do you want to continue?",
                        "Confirm", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No) return;
                }
            }
            // no need to check for tabPage selecting
            // after the file is saved.
            // this is required for new page not saved situation
            tabControl1.Selecting -= TabControl1_Selecting;

            if (ntp != null && ntp.UpdateNotSaved())
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to close without saving the data?",
                    "Save or Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.No) saveToolStripMenuItem_Click(sender, e);
            }
            if (!MNBook)
            {
                if (openFileList.Contains(ntp.FileName)) openFileList.Remove(ntp.FileName);
                resetAll();
                if (curViewCode == ViewTabular)
                {
                    ntp.GetDataGridView().Rows.Clear();
                }
            }
            if (MNBook)
            {
                curSelectedIndex = tabControl1.TabCount;
                //ntp = nissayaTabPageList[curSelectedIndex];
                //while (ntp.MNBookFile && ntp.MN_No == MN_No && openFileList.Contains(MRU_Bk))
                do
                {
                    if (tabControl1.TabCount > 0)
                    {
                        ntp = nissayaTabPageList[--curSelectedIndex];
                        tabControl1.SelectedIndex = curSelectedIndex;
                    }
                    if (dictMNTabs.ContainsKey(ntp) && dictMNTabs[ntp] == MN_No)
                    {
                        resetAll();
                        dictMNTabs.Remove(ntp);
                    }
                } while (GetNTPCount(MN_No) > 0);

                openFileList.Remove(MRU_Bk);
                if (tabControl1.TabCount == 1) curSelectedIndex = 0;
                textBox_MN.Text = label_MNTitle.Text = string.Empty;
                button_X_Click(this, null); // clear the Find results if any
                if (tabControl1.TabPages[curSelectedIndex].Text != "<New1>")
                {
                    ntp = nissayaTabPageList[curSelectedIndex];
                    if (ntp.MN_No > 0)
                    {
                        textBox_MN.Text = ntp.MN_No.ToString();
                        label_MNTitle.Text = MN_Titles[ntp.MN_No];
                    }
                }
            }
        }

        private int GetNTPCount(int MN_No)
        {
            if (dictMNTabs.Count == 0) return 0;
            int n = 0;
            foreach (KeyValuePair<NissayaTabPage, int> kv in dictMNTabs)
            {
                if (kv.Value == MN_No) ++n;
            }
            return n;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button_Save_Click(sender, e);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button_SaveAs_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button_Quit_Click(sender, e);
        }

        private void button_FindPrev_Click(object sender, EventArgs e)
        {
            if (curViewCode == ViewTabular)
            {
                // DataGridView
                FindGridNext(false); 
                return;
            }
            if (CurActivePageFindInfo == null) return;
            List<PageFindInfo> listPageFindInfo = GetPageInfoListView(curViewCode);
            if (listPageFindInfo.Count == 0) return;
            
            //nCurFindListIndex = CurActivePageFindInfo.nIndexFindPageResults;
            bool flagNextPageSuccess = false;
            int nextIncrement = 1;
            while (!flagNextPageSuccess && nCurFindListIndex >= 0 && nCurFindListIndex < listPageFindInfo.Count)
            {
                PageFindInfo pgFindInfo = listPageFindInfo[nCurFindListIndex];
                NissayaTabPage ntp = pgFindInfo.nissayaTabPage;
                ntp.curFindPgNo = ntp.GetCurrentPage();
                pgFindInfo.nextIncrement = nextIncrement;
                CurActivePageFindInfo = pgFindInfo;
                CurPageHighlightType = HightlightType.HighlightSelectedText;
                flagNextPageSuccess = this.CurActivePageFindInfo.nissayaTabPage.FindNext(false);
                if (!flagNextPageSuccess)
                {
                    if (--nCurFindListIndex < 0)
                    {
                        nCurFindListIndex = listPageFindInfo.Count - 1;
                        // reset count for the another prev 
                        SetTotalFindIndexView(GetCurViewFindStatus().TotalFindCount + 1);
                    }
                    ntp = listPageFindInfo[nCurFindListIndex].nissayaTabPage;
                    LoadFindPage(ntp, listPageFindInfo, false);
                    nextIncrement = 1;
                }
            }
            tabControl1.SelectedTab = CurActivePageFindInfo.nissayaTabPage.tabPage;
            textBox_FindStatus.Text = GetFindStatusView();
            EnableFindNavButtons();
        }

        private void button_FindNext_Click(object sender, EventArgs e)
        {
            if (curViewCode == ViewTabular)
            {
                // DataGridView
                FindGridNext(); return;
            }

            // RichTextBox
            if (CurActivePageFindInfo == null) return;
            List<PageFindInfo> listPageFindInfo = GetPageInfoListView(curViewCode);
            if (listPageFindInfo.Count == 0) return;

            bool flagNextPageSuccess = false;

            int nextIncrement = 1;
            while (!flagNextPageSuccess && 
                nCurFindListIndex >= 0 && nCurFindListIndex < listPageFindInfo.Count)
            {
                PageFindInfo pgFindInfo = listPageFindInfo[nCurFindListIndex];
                NissayaTabPage ntp = pgFindInfo.nissayaTabPage;
                ntp.curFindPgNo = ntp.GetCurrentPage();
                pgFindInfo.nextIncrement = nextIncrement;
                CurActivePageFindInfo = pgFindInfo;
                CurPageHighlightType = HightlightType.HighlightSelectedText;
                flagNextPageSuccess = this.CurActivePageFindInfo.nissayaTabPage.FindNext(true);
                if (!flagNextPageSuccess)
                {
                    if (++nCurFindListIndex >= listPageFindInfo.Count)
                    {
                        nCurFindListIndex = 0;
                        SetTotalFindIndexView(0); // reset for the next loop
                    }
                    else nextIncrement = 1;
                    ntp = listPageFindInfo[nCurFindListIndex].nissayaTabPage;
                    LoadFindPage(ntp, listPageFindInfo);
                    nextIncrement = 1;
                }
            }
            tabControl1.SelectedTab = CurActivePageFindInfo.nissayaTabPage.tabPage;
            textBox_FindStatus.Text = GetFindStatusView();
            EnableFindNavButtons();
        }

        private void LoadFindPage(NissayaTabPage ntp, List<PageFindInfo> listPageFindInfo, Boolean flagNext = true)
        {
            // reset current CurActivePageFindInfo
            CurActivePageFindInfo.nCurPageFindIndex = -1;
            CurActivePageFindInfo.nCurPageFindIndex = flagNext ? -1 : CurActivePageFindInfo.matches.Count;
            bool flagNewTab = ntp.FileName != CurActivePageFindInfo.nissayaTabPage.FileName;
            LoadNextFindPage(listPageFindInfo[nCurFindListIndex], flagNewTab);
            CurActivePageFindInfo = listPageFindInfo[nCurFindListIndex];
            CurActivePageFindInfo.nCurPageFindIndex = -1; // reset index to before 0 
            CurActivePageFindInfo.nCurPageFindIndex = flagNext ? -1 : CurActivePageFindInfo.matches.Count;
            return;
        }

        private void FindGridNext(bool flagDir = true)
        {
            if (CurActiveGridPageFindInfo == null) return;
            //CurPageHighlightType = HightlightType.HighlightSelectedText | HightlightType.HighlightText;
            //NissayaTabPage ntpFind = CurActiveGridPageFindInfo.nissayaDataGridView.parentInstance;
            //ntpFind.curFindPgNo = CurActiveGridPageFindInfo.pageNo;

            CurPageHighlightType = HightlightType.HighlightSelectedText;
            bool flagNextPageSuccess = CurActiveGridPageFindInfo.nissayaDataGridView.dataGridViewFindNext(flagDir);
            string key;
            while (!flagNextPageSuccess)
            {
                //nCurFindListIndex = CurActiveGridPageFindInfo.nIndexFindPageResults;
                key = CurActiveGridPageFindInfo.key;
                // go to next page that h                    
                bool flagNewTab = false;
                bool indexLoopAgain = false;
                nCurFindListIndex += (flagDir) ? +1 : -1;
                if (nCurFindListIndex >= DataGridViewFindPageList.Count)
                { nCurFindListIndex = 0; indexLoopAgain = true; }
                if (nCurFindListIndex < 0)
                { nCurFindListIndex = DataGridViewFindPageList.Count - 1; indexLoopAgain = true; }
                if (nCurFindListIndex < DataGridViewFindPageList.Count)
                {
                    CurActiveGridPageFindInfo = DataGridViewFindPageList.ElementAt(nCurFindListIndex).Value;
                    CurActiveGridPageFindInfo.nCurGridFindIndex = (flagDir) ? -1 : CurActiveGridPageFindInfo.listSelection.Count;
                    flagNewTab = (key != CurActiveGridPageFindInfo.key) ? true : false;
                    //SetTotalFindIndexView(nCurFindListIndex);
                    LoadNextFindPage(CurActiveGridPageFindInfo, flagNewTab);
                    CurPageHighlightType = HightlightType.HighlightSelectedText | HightlightType.HighlightText;
                    flagNextPageSuccess = CurActiveGridPageFindInfo.nissayaDataGridView.dataGridViewFindNext(flagDir);
                    if (!flagDir && indexLoopAgain) SetTotalFindIndexView(GetTotalFindCountView());
                    if (flagDir && indexLoopAgain) SetTotalFindIndexView(1);
                }
            }
            tabControl1.SelectedTab = CurActiveGridPageFindInfo.nissayaTabPage.tabPage;
            textBox_FindStatus.Text = GetFindStatusView();
            // make the selected cell the current cell
            //if (CurActiveGridPageFindInfo.nissayaTabPage.dataBox.Visible)
            //    CurActiveGridPageFindInfo.nissayaDataGridView.CurrentCell =
            //    CurActiveGridPageFindInfo.nissayaDataGridView[CurActiveGridPageFindInfo.nCurSelectedColIndex, CurActiveGridPageFindInfo.nCurSelectedRowIndex];
            // 2022-07-23
            //if (CurActiveGridPageFindInfo.nissayaTabPage.dataBox.Visible)
            //    CurActiveGridPageFindInfo.nissayaTabPage.GetDataGridView().dataGridView1_CellEnter(null, null);
            EnableFindNavButtons();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int n = 0;
            while (++n <= listGridViewSelection.Count)
            {
                if (e.RowIndex == listGridViewSelection[n - 1].row &&
                    e.ColumnIndex == listGridViewSelection[n - 1].col)
                    // this is the indexed cell
                    textBox_FindStatus.Text = n.ToString() + "/" + listGridViewSelection.Count.ToString();
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                if (currentViewMenu == ViewMenu.Tabular) dataGridViewFindNext();
                else richTextBoxFindNext();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen pen = new Pen(Color.Silver, 1);
            // Create rectangle.
            //int left = this.Size.Width - 350;
            Rectangle rect = new Rectangle(305, 33, 235, 34); //441
            rect.Location = newLocation(rect.Location);
            // Draw rectangle to screen.
            e.Graphics.FillRectangle(new SolidBrush(Color.White), rect);
            e.Graphics.DrawRectangle(pen, rect);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (nissayaTabPageList[tabControl1.SelectedIndex].HandleMenuShortCuts(sender, e)) return;
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (dataGridView_AppendMode)
            {
                dataGridView1.CurrentRow.Cells[0].Value = (dataGridView1.RowCount-1).ToString();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].HeaderText == "Plain Text")
            {
                if (e.Value != null)
                {
                    // Check for the marker in the cell.
                    string plainTextValue = (string)e.Value;
                    if (plainTextValue.Length > 0)
                        e.CellStyle.ForeColor = DataInfo.GetColor(plainTextValue[0]);
                    else
                        e.CellStyle.ForeColor = DataInfo.GetColor(' ');
                }
            }
        }

        private void dataGridView1_EditingControlShow(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].HeaderText == "Plain Text")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    string plainTextValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    if (plainTextValue.Length > 0)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor =
                            DataInfo.GetColor(plainTextValue[0]);
                        if (plainTextValue[0] == '^')
                        {
                            MessageBox.Show("No need to enter default marker '^'.");
                            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = plainTextValue.Substring(1);
                        }
                    }
                }
            }
        }
    }
}
