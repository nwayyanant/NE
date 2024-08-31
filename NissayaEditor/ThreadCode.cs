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
        private void init_FileLoad_BackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void init_RefreshGridView_BackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWorkRefreshGridView);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void init_RichTextView_BackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWorkRefreshRichTextView);
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

        // This event handler is where refreshing of dataGridView is done.
        private void backgroundWorker1_DoWorkRefreshGridView(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // RunWorkerCompleted eventhandler.
            doRefreshGridView_BackgroundWork((DataInfo)e.Argument, worker, e);
        }

        // This event handler is where refreshing of dataGridView is done.
        private void backgroundWorker1_DoWorkRefreshRichTextView(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // RunWorkerCompleted eventhandler.
            doRefreshRichTextView_BackgroundWork((DataInfo)e.Argument, worker, e);
        }

        private void doFileLoad_BackgroundWork(DataInfo dInfo, BackgroundWorker worker, DoWorkEventArgs e)
        {
            dInfo.ReadFileLoadData(curViewCode);
            e.Result = 1;
        }

        private void doRefreshGridView_BackgroundWork(DataInfo dInfo, BackgroundWorker worker, DoWorkEventArgs e)
        {
            dInfo.RefreshDataGridView();
            e.Result = 2;
        }

        private void doRefreshRichTextView_BackgroundWork(DataInfo dInfo, BackgroundWorker worker, DoWorkEventArgs e)
        {
            dInfo.RefreshRichTextBox();
            e.Result = 3;
        }
        
        // This event handler deals with the results of the FileLoad background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
            switch ((int)e.Result)
            {
                // Completion of file load.
                case 1:
                    dataGridValue1_Updated = false;
                    dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                    richTextBox1.TextChanged += richTextBox1_TextChanged;
                    richTextBox1_Updated = false;
                    backgroundWorker1.DoWork -= new DoWorkEventHandler(backgroundWorker1_DoWork);
                    backgroundWorker1.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    if (richTextBox1.Visible)
                    {
                        this.ActiveControl = richTextBox1;
                        richTextBox1.SelectionStart = 0;
                    }
                    if (dataGridView1.Visible)
                        this.ActiveControl = dataGridView1;
                    break;
                // Completion of dataGridView refresh
                case 2:
                    backgroundWorker1.DoWork -= new DoWorkEventHandler(backgroundWorker1_DoWorkRefreshGridView);
                    backgroundWorker1.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    dataGridValue1_Updated = false;
                    dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                    richTextBox1_Updated = false;
                    this.ActiveControl = dataGridView1;
                    break;
                case 3:
                    backgroundWorker1.DoWork -= new DoWorkEventHandler(backgroundWorker1_DoWorkRefreshRichTextView);
                    backgroundWorker1.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
                    richTextBox1.TextChanged += richTextBox1_TextChanged;
                    SelectFindText();
                    break;
            }
            Cursor.Current = Cursors.Default;
            dataStatus();
        }
    }
}