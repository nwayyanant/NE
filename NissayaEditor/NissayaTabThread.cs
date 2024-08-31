using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        public partial class NissayaTabPage
        {
            public const int thdFileLoad = 1;
            public const int thdPageLoad = 2;
            public const int thdRefreshDataGridView = 3;
            public const int thdRefreshRichTextBox = 4;
            public const int thdSavePage = 5;
            public const int thdFindText = 6;

            BackgroundWorker backgroundWorker2 = new BackgroundWorker();

            private void init_FileLoad_BackgroundWorker(BackgroundWorker backgroundWorker)
            {
                backgroundWorker.DoWork -= new DoWorkEventHandler(backgroundWorker1_DoWork);
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
                backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                backgroundWorker.WorkerSupportsCancellation = true;
            }

            private void init_FileLoad_BackgroundWorker()
            {
                backgroundWorker1.DoWork -= new DoWorkEventHandler(backgroundWorker1_DoWork);
                backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
                backgroundWorker1.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                backgroundWorker1.WorkerSupportsCancellation = true;
            }

            // This event handler is where the actual loading data from file is done.
            private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
            {
                // Get the BackgroundWorker that raised this event.
                BackgroundWorker worker = sender as BackgroundWorker;

                // RunWorkerCompleted eventhandler.
                doFileLoad_BackgroundWork((DataInfo)e.Argument, worker, e);
            }

            private void doFileLoad_BackgroundWork(DataInfo dInfo, BackgroundWorker worker, DoWorkEventArgs e)
            {
                if (dataInfo != null)
                {
                    if (dataGridView1_Updated) dataInfo.DataGridViewToFileContent();
                    if (richTextBox1_Updated) dataInfo.RichTextBoxToFileContent();
                    SavePage();

                    switch (dataInfo.ThreadAction)
                    {
                        case thdFileLoad:
                            dataInfo.ReadFileLoadData(curViewCode);
                            e.Result = thdFileLoad;
                            break;
                        case thdPageLoad:
                            //if (dataGridView1_Updated) dataInfo.DataGridViewToFileContent();
                            //if (richTextBox1_Updated) dataInfo.RichTextBoxToFileContent();
                            //int viewCode = PageLastView[curPageNo];
                            //dataInfo.LoadPageData(viewCode);
                            dataInfo.LoadPageData(curViewCode);
                            e.Result = thdPageLoad;
                            break;
                        case thdRefreshRichTextBox:
                            //if (dataInfo.UpdateFileContent) dataInfo.DataGridViewToFileContent();
                            dataInfo.RefreshRichTextBox(dataInfo.ViewCode);
                            e.Result = thdRefreshRichTextBox;
                            break;
                        case thdRefreshDataGridView:
                            //if (dataInfo.UpdateFileContent) dataInfo.RichTextBoxToFileContent();
                            dataInfo.RefreshDataGridView();
                            e.Result = thdRefreshDataGridView;
                            break;
                        case thdSavePage:
                            //if (dataGridView1_Updated) dataInfo.DataGridViewToFileContent();
                            //if (richTextBox1_Updated) dataInfo.RichTextBoxToFileContent();
                            //if (dataGridView1_Updated || richTextBox1_Updated) dataInfo.SaveCurrentPage();
                            break;
                    }
                }
            }

            // This event handler deals with the results of the FileLoad background operation.
            private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                    return;
                }

                bool fileSaveCheckMark = false;
                bool flagPageLoad = false;

                switch ((int)e.Result)
                {
                    // Completion of file load.
                    case thdFileLoad:
                        curPageNo = dataInfo.GetCurrentPage();
                        int n;
                        var isNumeric = int.TryParse(dataInfo.MN, out n);
                        if (isNumeric)
                        {
                            parentForm.textBox_MN.Text = dataInfo.MN;
                            MN_No = n;
                            parentForm.label_MNTitle.Text = parentForm.MN_Titles[MN_No];
                        }
                        break;
                    case thdPageLoad:
                        flagPageLoad = true;
                        break;
                    // Completion of dataGridView refresh
                    case thdRefreshRichTextBox:
                        if (this.UpdateNotSaved()) this.dataInfo.SaveFile();
                        fileSaveCheckMark = false;
                        break;
                    case thdRefreshDataGridView:
                        if (this.UpdateNotSaved()) this.dataInfo.SaveFile();
                        fileSaveCheckMark = false;
                        break;
                    case thdSavePage:
                        break;
                    case thdFindText:
                        parentForm.FindNextAllTabPages(true);
                        break;
                }
                if (fileSaveCheckMark)
                {
                    MessagingForm msgForm = new MessagingForm();
                    msgForm.SetImage(1);
                    msgForm.StartPosition = FormStartPosition.CenterParent;
                    msgForm.Show(parentForm);
                }

                if (richTextBox1.Visible)
                {
                    richTextBox1.SelectionStart = 0;
                    parentForm.ActiveControl = richTextBox1;
                }
                else
                    parentForm.ActiveControl = dataGridView1;


                richTextBox1.Enabled = true;
                dataGridView1.Enabled = true;
                richTextBox1_Updated = false;
                dataGridView1_Updated = false;

                richTextBox1_registerEvents(rtbAllEventsOn);
                dataGridView1.RegisterEvents(dgvAllEventsOn);
                parentForm.Cursor = Cursors.Default;
                richTextBox1.Cursor = Cursors.Default;
                dataGridView1.Cursor = Cursors.Default;
                parentForm.dataStatus();
                if (!flagPageLoad) parentForm.loadBookFile();
            }

            private void normalStatus()
            {
                richTextBox1.Enabled = true;
                dataGridView1.Enabled = true;
                richTextBox1_Updated = false;
                dataGridView1_Updated = false;

                richTextBox1_registerEvents(rtbAllEventsOn);
                dataGridView1.RegisterEvents(dgvAllEventsOn);
                parentForm.Cursor = Cursors.Default;
                richTextBox1.Cursor = Cursors.Default;
                dataGridView1.Cursor = Cursors.Default;
                parentForm.dataStatus();
            }

            // ***********************************************************************
            // Find thread
            // ***********************************************************************
            private void init_FindTab_BackgroundWorker(BackgroundWorker backgroundWorker)
            {
                backgroundWorker.DoWork -= new DoWorkEventHandler(backgroundWorker_FindDoWork);
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_FindDoWork);
                backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                backgroundWorker.WorkerSupportsCancellation = true;
            }

            // This event handler is where the actual loading data from file is done.
            private void backgroundWorker_FindDoWork(object sender, DoWorkEventArgs e)
            {
                // Get the BackgroundWorker that raised this event.
                BackgroundWorker worker = sender as BackgroundWorker;

                // RunWorkerCompleted eventhandler.
                doFind_BackgroundWork((NissayaTabPage)e.Argument, worker, e);
            }

            private void doFind_BackgroundWork(NissayaTabPage nissayaTabPage, BackgroundWorker worker, DoWorkEventArgs e)
            {
                nissayaTabPage.richTextBoxPageFind(curFindPgNo);
                e.Result = thdFindText;
            }

            private bool flagTabFindFirst = false;
            public void TabFindText()
            {
                flagTabFindFirst = true;
                init_FindTab_BackgroundWorker(backgroundWorker2);
                backgroundWorker2.RunWorkerAsync((NissayaTabPage)this);
            }
        }
    }
}
