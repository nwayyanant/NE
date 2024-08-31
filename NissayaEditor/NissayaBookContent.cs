using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NissayaEditor
{
    public partial class NissayaBookContent : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        MessagingForm msgForm = null;
        Font fontROM = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        string fName;
        public bool FlagUpdate = false;
        public bool FlagOpenBook = false;
        string nbkFilters = "nbk files (*.nbk)|*.nbk";
        Form1.NissayaBook nbook = null;
        const string _newFile = "<New>";

        public NissayaBookContent(string fname = "", Form1.NissayaBook nbk = null)
        {
            InitializeComponent();
            fName = fname;
            if (fname.Length > 0)
            {
                this.Text += fname;
                this.button_Done.Text = "Quit";
            }
            else
                this.Text += _newFile;

            initDataGridView();
            setEvents(true);
            if (nbk != null) LoadNissayaBook(nbk);
            //else nbk = new Form1.NissayaBook();
        }

        private void setEvents(bool flag)
        {
            switch (flag)
            {
                case false:
                    textBox_Author.TextChanged -= control_ValueChanged;
                    textBox_Title.TextChanged -= control_ValueChanged;
                    dataGridView1.CellValueChanged -= control_ValueChanged;
                    break;
                case true:
                    textBox_Author.TextChanged += control_ValueChanged;
                    textBox_Title.TextChanged += control_ValueChanged;
                    dataGridView1.CellValueChanged += control_ValueChanged;
                    break;
            }
        }
        private void initDataGridView()
        {
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.ColumnCount = 2;

            // add checkbox column
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "";
            checkBoxColumn.ThreeState = false;
            checkBoxColumn.FalseValue = false;
            checkBoxColumn.TrueValue = true;
            dataGridView1.Columns.Insert(0, checkBoxColumn);

            // Column headers
            dataGridView1.Columns[1].HeaderText = "Chapter";
            dataGridView1.Columns[2].HeaderText = "File";
            dataGridView1.Columns[1].HeaderCell.Style.Font = fontROM;
            dataGridView1.Columns[2].HeaderCell.Style.Font = fontROM;
            // Cell font
            dataGridView1.Columns[1].DefaultCellStyle.Font = fontROM;
            dataGridView1.Columns[2].DefaultCellStyle.Font = fontROM;
            // Column widths
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = dataGridView1.Width - 195;
            // default value for checkbox
            dataGridView1.Columns[0].DefaultCellStyle.NullValue = true;

            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
       
            // disable sorting
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void button_Done_Click(object sender, EventArgs e)
        {
            if (FlagUpdate)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to leave without saving the Nissaya book?", 
                    "Nissaya Book Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.No) return;
            }
            //FlagUpdate = false;
            this.Close();
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            if (nbook != null)
            {
                if (FlagUpdate)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to open files without saving the Nissaya book?",
                        "Nissaya Book Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dialogResult == DialogResult.No) return;
                }
                FlagOpenBook = true;
                int n = 0;
                foreach (Form1.NissayaBook.Chapter ch in nbook.chapters)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dataGridView1.Rows[n].Cells[0];
                    //if (dataGridView1.Rows[n].Cells[0].ValueType == "Boolean")
                    ch.checkBox = (bool)chk.EditedFormattedValue;
                    ++n;
                }
            }
            this.Close();
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Form1.fileFilters;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.CurrentRow.Cells[2].Value = openFileDialog.FileName;
                    //if (nbook != null && nbook.chapters.Count > dataGridView1.CurrentRow.Index)
                    //    nbook.chapters[dataGridView1.CurrentRow.Index].fileName = openFileDialog.FileName;
                    //else
                    //    addNewChapter();
                }
            }
        }

        private void addNewChapter()
        {
            string chName, fName;
            bool chkbox = false;

            chName = (dataGridView1.CurrentRow.Cells[1].Value == null) ? "" : dataGridView1.CurrentRow.Cells[1].Value.ToString();
            fName = (dataGridView1.CurrentRow.Cells[2].Value == null) ? "" : dataGridView1.CurrentRow.Cells[2].Value.ToString();
            Form1.NissayaBook.Chapter ch = new Form1.NissayaBook.Chapter(chkbox, chName, fName);
            if (nbook == null) nbook = new Form1.NissayaBook();
            nbook.chapters.Add(ch);
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = nbkFilters;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (fName.Length == 0)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;
                fName = saveFileDialog1.FileName;
            }
            this.Text = this.Text.Replace(_newFile, Path.GetFileName(fName));
            if (nbook == null) nbook = new Form1.NissayaBook(fName, textBox_Title.Text, textBox_Author.Text);
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)r.Cells[0];
                //string s = chk.TrueValue.ToString();
                //bool f = Convert.ToBoolean(chk.FormattedValue);
                if (r.Cells[1].Value != null && r.Cells[2].Value != null)
                {
                    nbook.AddChapter(Convert.ToBoolean(chk.FormattedValue), r.Cells[1].Value.ToString(), r.Cells[2].Value.ToString());
                }
            }
            JSON.Json nissayaBook = new JSON.Json();
            nissayaBook.CreateFile(fName);
            nissayaBook.UpdateData(nbook.NissayaBookFileData());
            FlagUpdate = false;

            msgForm = new MessagingForm();
            msgForm.SetImage(1);
            msgForm.StartPosition = FormStartPosition.CenterParent;
            msgForm.Show(this);
        }
        public Form1.NissayaBook GetNissayaBook() { return nbook; }
        public string GetFileName() { return fName; }

        private void LoadNissayaBook(Form1.NissayaBook nbk)
        {
            setEvents(false);
            nbook = nbk;
            textBox_Author.Text = nbk.GetAuthor();
            textBox_Title.Text = nbk.GetTitle();
            List<Form1.NissayaBook.Chapter> chapters = nbk.chapters;
            int n = 0;
            foreach (Form1.NissayaBook.Chapter ch in chapters)
            {
                this.dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = ch.checkBox;
                dataGridView1.Rows[n].Cells[1].Value = ch.chapName;
                dataGridView1.Rows[n].Cells[2].Value = ch.fileName;
                ++n;
            }
            setEvents(true);
        }
        private void control_ValueChanged(object sender, EventArgs e)
        {
            FlagUpdate = true;
            setEvents(false);
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            // 2022-05-16
            // v1.0.2
            if (dataGridView1.CurrentRow.Index < nbook.chapters.Count)
            {
                nbook.chapters.RemoveAt(dataGridView1.CurrentRow.Index);
            }
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
        }
    }
}
