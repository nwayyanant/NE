using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Input;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        public const int dgvCellValueChanged = 1;
        public const int dgvEditingControlShowing = 2;
        public const int dgvKeyPress = 4;
        public const int dgvKeyDown = 8;
        public const int dgvCellStyleChanged = 16;
        public const int dgvRowEnter = 32;
        public const int dgvCellEnter = 64;
        public const int dgvCellFormatting = 128;
        public const int dgvMouseClick = 256;
        public const int dgvAllEventsOn = 0xffff;
        public const int dgvAllEventsOff = 0;

        public const int dgvColSrNo = 0;
        public const int dgvColRecordType = 1;
        public const int dgvColPali = 2;
        public const int dgvColTranslation = 3;
        public const int dgvColFootnote = 4;

        static int gDataGridViewEventFlags = 0;

        public class DataGridViewFindPage
        {
            public string key;
            public int nIndexFindPageResults;
            public string pageNo;

            public int nCurGridFindIndex = -1;
            public int nCurSelectedRowIndex;
            public int nCurSelectedColIndex;
            public string findStatus = string.Empty;
            public string searchText = string.Empty;
            public List<_GridViewSelection> listSelection;
            public DataGridViewRow currentRow;
            public NissayaDataGridView nissayaDataGridView;
            public NissayaTabPage nissayaTabPage;
            public List<_NissayaDataRecord> NissayaGridDataList;

            public void SetValues(int nCurIndex, string status, string text, List<_GridViewSelection> l, DataGridViewRow curRow)
            {
                //nCurFindIndex = nCurIndex;
                findStatus = status;
                searchText = text;
                //listSelection = l;
                currentRow = curRow;
            }

            public void Clear()
            {
                //nCurFindIndex = -1;
                findStatus = string.Empty;
                searchText = string.Empty;
                //listSelection = null;
                currentRow = null;
            }
        }

        public struct _NissayaDataRecord
        {
            public string srno, marker, pali, plain, footnote;
        }

        static DataGridViewFindPage CurActiveGridPageFindInfo = null;

        public class NissayaDataGridView : DataGridView
        {
            Form1 parentForm;
            //int nCurFindIndex = -1;
            //int nListFindIndex = 0;
            //int nCurGridFindIndex = 0;
            public string FindStatus = string.Empty;
            public string SearchText = string.Empty;

            string curPageNo = string.Empty;

            public NissayaTabPage parentInstance;
            public int ID;
            Button button1 = new Button();
            int prevCellRow = -1;
            int prevCellCol = -1;
            //int nCurSelectedRowIndex;
            //int nCurSelectedColIndex;

            const int cellBlockedRGB = 220;
            Color gridCellBlockedColor = Color.FromArgb(cellBlockedRGB, cellBlockedRGB, cellBlockedRGB);
            Color defaultDataGridViewSelectionColor = System.Drawing.SystemColors.Highlight;

            TextBox dataBox = null;

            int dataGridView1_MouseClickRowIndex = -1;
            const string menuItem_InsertRowText = "Insert row(s)";
            const string menuItem_DeleteRowText = "Delete row(s)";
            const string menuItem_AppendRowText = "Append rows";
            
            Dictionary<string, DataGridViewFindPage> dictTableFindResults = new Dictionary<string,DataGridViewFindPage>();

            public void SetCurrentPageNo(string pgNo) { curPageNo = pgNo; }

            public NissayaDataGridView(NissayaTabPage pInstance)
            {
                parentInstance = pInstance;
                parentForm = parentInstance.parentForm;
                dataBox = pInstance.dataBox;
            }

            public void initialize()
            {
                this.Enabled = false;
                this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells | DataGridViewAutoSizeRowsMode.AllHeaders;
                this.ColumnCount = 5;

                // column header setup
                this.RowTemplate.Height = 35;
                //dataGridView1.RowHeadersVisible = false;
                //this.ColumnHeadersHeight = 50;
                //this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                this.RowHeadersWidth = 35;
                this.ColumnHeadersHeight = 50;
                
                // more row, columns settings

                // set column header text and font
                this.Columns[dgvColSrNo].HeaderText = "Sr No";
                this.Columns[dgvColRecordType].HeaderText = "*" + Environment.NewLine + "#";
                this.Columns[dgvColPali].HeaderText = "Pāḷi";
                this.Columns[dgvColTranslation].HeaderText = "Translation";
                this.Columns[dgvColFootnote].HeaderText = "Footnote";

                this.Columns[dgvColSrNo].HeaderCell.Style.Font = fontROM;
                this.Columns[dgvColRecordType].HeaderCell.Style.Font = fontROM;
                this.Columns[dgvColPali].HeaderCell.Style.Font = fontROM;
                this.Columns[dgvColTranslation].HeaderCell.Style.Font = fontROM;
                this.Columns[dgvColFootnote].HeaderCell.Style.Font = fontROM;

                // set col widths
                this.Columns[dgvColSrNo].Width = 60;
                this.Columns[dgvColRecordType].Width = 30;
                this.Columns[dgvColFootnote].Width = 150;
                int nWidth = this.Columns[dgvColSrNo].Width + this.Columns[dgvColRecordType].Width + this.Columns[dgvColFootnote].Width;
                this.Columns[dgvColPali].Width = (this.Width - nWidth) / 2; ;
                this.Columns[dgvColTranslation].Width = this.Columns[dgvColPali].Width;

                // set column colors
                this.Columns[dgvColRecordType].DefaultCellStyle.ForeColor = DataInfo.GetColor('*');
                this.Columns[dgvColRecordType].DefaultCellStyle.BackColor = Color.White;
                this.Columns[dgvColPali].DefaultCellStyle.ForeColor = DataInfo.GetColor('*');
                this.Columns[dgvColPali].DefaultCellStyle.BackColor = Color.White;
                this.Columns[dgvColTranslation].DefaultCellStyle.ForeColor = DataInfo.GetColor('^');
                this.Columns[dgvColTranslation].DefaultCellStyle.BackColor = Color.White;
                this.Columns[dgvColFootnote].DefaultCellStyle.ForeColor = DataInfo.GetColor('@');
                this.Columns[dgvColFootnote].DefaultCellStyle.BackColor = Color.White;

                // set fonts
                this.Columns[dgvColSrNo].DefaultCellStyle.Font = fontROM;
                this.Columns[dgvColRecordType].DefaultCellStyle.Font = fontROM;
                this.Columns[dgvColPali].DefaultCellStyle.Font = parentInstance.fontMYA;
                this.Columns[dgvColTranslation].DefaultCellStyle.Font = parentInstance.fontMYA;
                this.Columns[dgvColFootnote].DefaultCellStyle.Font = parentInstance.fontMYA;

                // set other attributes
                this.Columns[dgvColSrNo].ReadOnly = true;
                this.Columns[dgvColRecordType].ReadOnly = true;
                this.Columns[dgvColRecordType].DefaultCellStyle.NullValue = "*";
                ((DataGridViewTextBoxColumn)this.Columns[dgvColRecordType]).MaxInputLength = 1;

                this.AllowUserToResizeColumns = true;
                this.AllowUserToResizeRows = true;
                this.AllowUserToOrderColumns = false;
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                //this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //this.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; //A            

                this.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Set the background color for all rows and for alternating rows. 
                // The value for alternating rows overrides the value for all rows.
                this.RowsDefaultCellStyle.BackColor = Color.White;
                this.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 255, 236);

                this.EnableHeadersVisualStyles = false;
                //this.ColumnHeadersDefaultCellStyle.BackColor = DataInfo.getColor(' ');
                //this.RowHeadersDefaultCellStyle.BackColor = Color.OldLace;
                //this.Columns[0].HeaderCell.Style.BackColor = Color.OldLace;
                this.EnableHeadersVisualStyles = false;
                this.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                this.RowHeadersDefaultCellStyle.BackColor = Color.LightGray;

                //this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                this.Columns[dgvColSrNo].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;//.DisplayedCells;
                this.Columns[dgvColRecordType].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;//.DisplayedCells;
                this.Columns[dgvColPali].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.Columns[dgvColTranslation].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.Columns[dgvColFootnote].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // disable sorting
                foreach (DataGridViewColumn column in this.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                // button ">>"
                Controls.Add(button1);
                button1.Font = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                button1.Size = new Size(40, 28);
                button1.Text = "<<";
                button1.Click += button1_Click;
                button1.FlatStyle = FlatStyle.Flat;
                int X = this.Location.X + this.Width - 65;
                button1.Location = new Point(X, this.Location.Y + 3);
                button1.Visible = true;
                button1.Enabled = true;
                button1.AutoSize = false;
                button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                button1.BackColor = Color.LightGray;
                button1.FlatAppearance.BorderColor = Color.LightGray;

                RegisterEvents(dgvAllEventsOn);
            }

            private void button1_Click(object sender, EventArgs e)
            {
                switch (button1.Text)
                {
                    case "<<":
                        button1.Text = ">>";
                        this.Columns[4].Visible = false;
                        break;
                    case ">>":
                        button1.Text = "<<";
                        this.Columns[4].Visible = true;
                        break;
                }
            }

            public void RegisterEvents(int eventFlags)
            {
                gDataGridViewEventFlags = eventFlags;
                this.CellValueChanged -= dataGridView1_CellValueChanged;
                if ((eventFlags & dgvCellValueChanged) == dgvCellValueChanged) this.CellValueChanged += dataGridView1_CellValueChanged;

                this.EditingControlShowing -= dataGridView1_EditingControlShowing;
                if ((eventFlags & dgvEditingControlShowing) == dgvEditingControlShowing) this.EditingControlShowing += dataGridView1_EditingControlShowing;

                this.KeyPress -= dataGridView1_KeyPress;
                if ((eventFlags & dgvKeyPress) == dgvKeyPress) this.KeyPress += dataGridView1_KeyPress;

                this.KeyDown -= dataGridView1_KeyDown;
                if ((eventFlags & dgvKeyDown) == dgvKeyDown) this.KeyDown += dataGridView1_KeyDown;

                this.CellStyleChanged -= DataGridView1_CellStyleChanged;
                if ((eventFlags & dgvCellStyleChanged) == dgvCellStyleChanged) this.CellStyleChanged += DataGridView1_CellStyleChanged;
                
                this.RowEnter -= dataGridView1_RowEnter;
                if ((eventFlags & dgvRowEnter) == dgvRowEnter) this.RowEnter += dataGridView1_RowEnter;

                this.CellEnter -= dataGridView1_CellEnter;
                if ((eventFlags & dgvCellEnter) == dgvCellEnter) this.CellEnter += dataGridView1_CellEnter;

                this.CellFormatting -= dataGridView1_CellFormatting;
                if ((eventFlags & dgvCellFormatting) == dgvCellFormatting) this.CellFormatting += dataGridView1_CellFormatting;

                this.MouseClick -= dataGridView1_MouseClick;
                if ((eventFlags & dgvMouseClick) == dgvMouseClick) this.MouseClick += dataGridView1_MouseClick;
            }

            public void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
            {
                parentInstance.dataGridView1_Updated = true;
                parentInstance.fileUpdated = true;
            }

            public void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
            {
                RegisterEvents(dgvAllEventsOff);
                if (this.CurrentRow != null && this.CurrentRow.Cells[dgvColSrNo].Value == null &&
                    this.CurrentCell.ColumnIndex > 1)
                    this.CurrentRow.Cells[dgvColSrNo].Value = (this.Rows.Count).ToString();

                if (this.CurrentRow != null && this.CurrentCell.ColumnIndex > 1 && parentInstance.dataBox.Visible == true)
                {
                    // save previous
                    if (prevCellRow != -1 && prevCellCol != -1)
                    {
                        if (this.Rows[prevCellRow].Cells[prevCellCol].EditedFormattedValue.ToString() != parentInstance.dataBox.Text.Trim())
                        {
                            this.Rows[prevCellRow].Cells[prevCellCol].Value = parentInstance.dataBox.Text.Trim();
                            parentInstance.dataGridView1_Updated = parentInstance.fileUpdated = true;
                        }
                        this.Rows[prevCellRow].Cells[prevCellCol].Selected = false;
                    }
                    switch (this.CurrentCell.ColumnIndex)
                    {
                        case 2:
                            dataBox.ForeColor = DataInfo.GetColor(this.CurrentRow.Cells[1].EditedFormattedValue.ToString()[0]);
                            break;
                        case 3:
                            dataBox.ForeColor = DataInfo.GetColor('^');
                            break;
                        case 4:
                            dataBox.ForeColor = DataInfo.GetColor('@');
                            break;
                    }
                    //if (this.CurrentCell.ColumnIndex > 1) dataBox.Text = this.CurrentCell.EditedFormattedValue.ToString();
                    //else dataBox.Text = string.Empty;
                    prevCellRow = this.CurrentRow.Index;
                    prevCellCol = this.CurrentCell.ColumnIndex;
                }
                if (this.CurrentCell.ColumnIndex > 1) dataBox.Text = this.CurrentCell.EditedFormattedValue.ToString();
                else dataBox.Text = string.Empty;

                // store dataBoxInfo info
                //parentInstance.dataBoxInfo.SetValues(dataBox.Text, this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex);

                this.Rows[this.CurrentRow.Index].Cells[this.CurrentCell.ColumnIndex].Selected = true;
                dataBox.ReadOnly = (dataBox.Visible == true && this.CurrentCell.ColumnIndex <= 1);
                if (dataBox.Visible)
                {
                    //parentForm.ActiveControl = dataBox;
                    dataBox.DeselectAll();
                    dataBox.SelectionStart = dataBox.Text.Length;
                }
                RegisterEvents(dgvAllEventsOn);
            }

            private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
            {
                if (e.ColumnIndex == dgvColRecordType && e.Value != null) //(string)e.Value == "#")
                {
                    //int curEventFlags = gDataGridViewEventFlags;
                    //RegisterEvents(dgvAllEventsOff);
                    Color cHash = DataInfo.GetColor('#');
                    Color cAsterisk = DataInfo.GetColor('*');

                    switch (((string)e.Value)[0])
                    {
                        case '#':
                            this.Rows[e.RowIndex].Cells[dgvColRecordType].Style.ForeColor = cHash;
                            this.Rows[e.RowIndex].Cells[dgvColPali].Style.ForeColor = cHash;
                            this.Rows[e.RowIndex].Cells[dgvColTranslation].Value = string.Empty;
                            //this.Rows[e.RowIndex].Cells[dgvColTranslation].Style.BackColor = gridCellBlockedColor;
                            this.Rows[e.RowIndex].Cells[dgvColTranslation].ReadOnly = true;
                            //this.Rows[e.RowIndex].Cells[dgvColTranslation].Selected = false;
                            this.Rows[e.RowIndex].Cells[dgvColFootnote].Value = string.Empty;
                            //this.Rows[e.RowIndex].Cells[dgvColFootnote].Style.BackColor = gridCellBlockedColor;
                            this.Rows[e.RowIndex].Cells[dgvColFootnote].ReadOnly = true;
                            //this.Rows[e.RowIndex].Cells[dgvColFootnote].Selected = false;
                            break;
                        case 'A':
                            this.Rows[e.RowIndex].Cells[dgvColRecordType].Style.ForeColor = cAsterisk;
                            this.Rows[e.RowIndex].Cells[dgvColPali].Style.ForeColor = cAsterisk;
                            //this.Rows[e.RowIndex].Cells[dgvColTranslation].Value = string.Empty;
                            this.Rows[e.RowIndex].Cells[dgvColTranslation].Style.ForeColor = DataInfo.GetColor('^');
                            //this.Rows[e.RowIndex].Cells[dgvColTranslation].Style.BackColor = Color.White;
                            this.Rows[e.RowIndex].Cells[dgvColTranslation].ReadOnly = false;
                            //this.Rows[e.RowIndex].Cells[dgvColTranslation].Selected = false;
                            //this.Rows[e.RowIndex].Cells[dgvColFootnote].Value = string.Empty;
                            this.Rows[e.RowIndex].Cells[dgvColFootnote].Style.ForeColor = DataInfo.GetColor('@');
                            //this.Rows[e.RowIndex].Cells[dgvColFootnote].Style.BackColor = Color.White;
                            this.Rows[e.RowIndex].Cells[dgvColFootnote].ReadOnly = false;
                            // change remove gridCellBlocked color and restore the base color depending upon the line
                            break;
                    }
                    //RegisterEvents(curEventFlags);// dgvAllEventsOn);
                }
            }

            private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
            {
            }

            private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
            {
                //this.EditingPanel
                //dataGridView1_RegisterCellValueChanged(false);
                //if (this.CurrentRow.Cells[0].Value == null)
                //    this.CurrentRow.Cells[0].Value = (this.CurrentRow.Index+1).ToString();
                //this.CurrentCell.Style.BackColor = Color.White;
                //dataGridView1_RegisterCellValueChanged(true);
                e.CellStyle.BackColor = this.CurrentRow.Index % 2 == 1 ? Color.FromArgb(250, 255, 236)
                    : Color.White;
            }

            private void DataGridView1_CellStyleChanged(Object sender, DataGridViewCellEventArgs e)
            {
            }

            private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
            {

            }

            private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyData == (Keys.Control | Keys.D))
                {
                    parentInstance.ShowDataBox();
                }
                else parentInstance.HandleMenuShortCuts(sender, e);
            }

            public void dataBox_EnterPressed()
            {
                int maxColIndex = (this.CurrentRow.Cells[4].Visible) ? 4 : 3;
                this.CurrentCell.Value = dataBox.Text.Trim();
                int rowIndex = this.CurrentRow.Index;
                int colIndex = this.CurrentCell.ColumnIndex + 1;
                this.ReadOnly = false;
                if (colIndex > maxColIndex)
                {
                    colIndex = 2;
                    CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
                    this.EndEdit();
                    if (rowIndex == this.Rows.Count - 1)
                    {
                        if (maxColIndex + 1 == 5)
                        {
                            object[] fields5 = new object[5] {
                                    this.Rows[rowIndex].Cells[0].Value.ToString(),
                                    this.Rows[rowIndex].Cells[1].FormattedValue.ToString(),
                                    this.Rows[rowIndex].Cells[2].Value.ToString(),
                                    this.Rows[rowIndex].Cells[3].Value.ToString(),
                                    this.Rows[rowIndex].Cells[4].Value.ToString()
                                };
                            this.Rows.Add(fields5);
                        }
                        else
                        {
                            object[] fields4 = new object[4] {
                                    this.Rows[rowIndex].Cells[0].Value.ToString(),
                                    this.Rows[rowIndex].Cells[1].FormattedValue.ToString(),
                                    this.Rows[rowIndex].Cells[2].Value.ToString(),
                                    this.Rows[rowIndex].Cells[3].Value.ToString()
                                };
                            this.Rows.Add(fields4);
                        }
                        resetNewRowValues(++rowIndex);
                    }
                    else 
                        ++rowIndex;
                }
                this.CurrentCell = this.Rows[rowIndex].Cells[colIndex];
                dataBox.Text = this.CurrentCell.FormattedValue.ToString().Trim();
                dataBox.SelectionStart = dataBox.Text.Length;
                this.ReadOnly = true;
            }

            private void resetNewRowValues(int curRow)
            {
                this.Rows[this.Rows.Count - 1].Cells[0].Value = null;
                //this.Rows[this.Rows.Count - 1].Cells[1].Value = "*";
                this.Rows[this.Rows.Count - 1].Cells[2].Value = string.Empty;
                this.Rows[this.Rows.Count - 1].Cells[3].Value = string.Empty;
                this.Rows[this.Rows.Count - 1].Cells[4].Value = string.Empty;
                dataBox.Text = string.Empty;
            }

            private void addAppendRow()
            {
                int rowIndex = this.Rows.Count - 1;
                this.FirstDisplayedScrollingRowIndex = rowIndex;
                //this.Rows[rowIndex].Cells[0].Value = (rowIndex + 1).ToString();
                this.CurrentCell = this.Rows[rowIndex].Cells[1];
                this.BeginEdit(true);
            }

            // #MENU
            private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
            {
                // flagFindTextBoxClicked is set when textBox_Find is the focus
                // this is to show where hot keys are to be inserted
                parentForm.flagFindTextBoxClicked = false;
                if (e.Button == MouseButtons.Right)
                {
                    ContextMenuStrip cms = new ContextMenuStrip();
                    cms.Items.Add(menuItem_InsertRowText);
                    cms.Items.Add(menuItem_DeleteRowText);
                    cms.Items.Add(menuItem_AppendRowText);
                    dataGridView1_MouseClickRowIndex = this.HitTest(e.X, e.Y).RowIndex;
                    if (dataGridView1_MouseClickRowIndex >= 0 && dataGridView1_MouseClickRowIndex < this.Rows.Count - 1)
                    {
                        cms.Show(PointToScreen(e.Location));
                        cms.ItemClicked += new ToolStripItemClickedEventHandler(contexMenu_ItemClicked);
                    }
                    else dataGridView1_MouseClickRowIndex = -1;
                }
            }

            // #MENU
            private void contexMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
            {
                if (dataGridView1_MouseClickRowIndex == -1) return;

                this.CellValueChanged -= dataGridView1_CellValueChanged;
                ToolStripItem item = e.ClickedItem;
                switch (item.Text)
                {
                    case menuItem_InsertRowText:
                        insertDataRow(dataGridView1_MouseClickRowIndex, this.SelectedRows.Count);
                        break;
                    case menuItem_DeleteRowText:
                        deleteDataRow(dataGridView1_MouseClickRowIndex);
                        break;
                    case menuItem_AppendRowText:
                        //this.RowsAdded += dataGridView1_RowsAdded;
                        //dataGridView_AppendMode = true;
                        addAppendRow();
                        break;
                }
                this.CellValueChanged += dataGridView1_CellValueChanged;
                //parentInstance.dataGridValue1_Updated = true;
                dataGridView1_MouseClickRowIndex = -1;
            }

            // #MENU
            private void insertDataRow(int index, int rowCount)
            {
                if (rowCount == 0) rowCount = 1;
                while (rowCount-- > 0)
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.Height = 35;
                    this.Rows.Insert(dataGridView1_MouseClickRowIndex, newRow);
                    this.Rows[dataGridView1_MouseClickRowIndex].Cells[0].ReadOnly = true;
                    //nissayaData.Insert(index, new NissayaData(dataGridView1_MouseClickRowIndex + 1));
                }
                // adjust row numbers for all rows below the new added row
                while (index < this.Rows.Count)
                    this.Rows[index].Cells[0].Value = (++index).ToString();
            }

            // #MENU
            private void deleteDataRow(int index)
            {
                if (this.SelectedRows.Count > 0)
                {
                    index = -1;
                    //int srno = -1;
                    foreach (DataGridViewRow row in this.SelectedRows)
                    {
                        index = row.Index; // Int16.Parse(row.Cells[0].Value.ToString());
                        this.Rows.RemoveAt(row.Index);
                    }
                }
                else
                    this.Rows.RemoveAt(index);

                // adjust row numbers for all rows below the new added row
                while (index < this.Rows.Count)
                    this.Rows[index].Cells[0].Value = (++index).ToString();
            }

            //***********************************************************************************
            // #FIND
            public void dataGridViewFind()
            {
                if (parentInstance.tabPage.Text.IndexOf("<New") != -1) return;
                if (!parentInstance.UsingSecondaryContainer())
                    DataGridViewFindClear();
                if (parentForm.textBox_Find.Text.Length == 0) return;

                string pgno = string.Empty;// ntpFind.curFindPgNo 
                List<string> curPageList = parentInstance.GetPageList();   // get current tab's page list
                int nn = 0;
                Boolean found = false;
                List<_GridViewSelection> listGridViewSelection;
                //DataGridViewFindPage dataGridViewFindResults;
                foreach (string pno in curPageList)
                {
                    if (!found && pno == parentInstance.curFindPgNo) found = true;
                    if (found)
                    {
                        listGridViewSelection = parentInstance.GetGridDataMatches(pno, parentForm.textBox_Find.Text);
                        if (listGridViewSelection.Count > 0) FillFindGridMatches(pno, listGridViewSelection);
                    }
                    else ++nn;
                }
                if (nn > 0)
                {
                    string pno;
                    for (int i = 0; i < nn; ++i)
                    {
                        pno = curPageList[i];
                        listGridViewSelection = parentInstance.GetGridDataMatches(pno, parentForm.textBox_Find.Text);
                        if (listGridViewSelection.Count > 0) FillFindGridMatches(pno, listGridViewSelection);
                    }
                }
                //listGridViewSelection = parentInstance.GetGridDataMatches(parentInstance.curFindPgNo, parentForm.textBox_Find.Text);

                //if (listGridViewSelection.Count > 0)
                //{
                //    dataGridViewFindResults = new DataGridViewFindPage();
                //    curPageNo = parentInstance.curFindPgNo;
                //    string TabPageKey = parentInstance.GetThisTabPageKey();

                //    dataGridViewFindResults.key = TabPageKey;
                //    dataGridViewFindResults.nCurSelectedRowIndex = listGridViewSelection[0].row;
                //    dataGridViewFindResults.nCurSelectedColIndex = listGridViewSelection[0].col;
                //    dataGridViewFindResults.nCurGridFindIndex = -1;
                //    dataGridViewFindResults.listSelection = listGridViewSelection;
                //    dataGridViewFindResults.pageNo = curPageNo;
                //    dataGridViewFindResults.searchText = parentForm.textBox_Find.Text;
                //    dataGridViewFindResults.nissayaDataGridView = this;
                //    dataGridViewFindResults.nissayaTabPage = parentInstance;
                //    dataGridViewFindResults.nIndexFindPageResults = DataGridViewFindPageList.Count;
                //    if (DataGridViewFindPageList.Count == 0) CurActiveGridPageFindInfo = dataGridViewFindResults;
                //    DataGridViewFindPageList.Add(TabPageKey, dataGridViewFindResults);
                //    parentForm.UpdateTotalFindCountView(listGridViewSelection.Count);
                //}
            }

            private void FillFindGridMatches(string pgno, List<_GridViewSelection> listGridViewSelection)
            {
                DataGridViewFindPage dataGridViewFindResults = new DataGridViewFindPage();
                parentInstance.curFindPgNo = pgno;
                string TabPageKey = parentInstance.GetThisTabPageKey();
                dataGridViewFindResults.key = TabPageKey;
                dataGridViewFindResults.nCurSelectedRowIndex = listGridViewSelection[0].row;
                dataGridViewFindResults.nCurSelectedColIndex = listGridViewSelection[0].col;
                dataGridViewFindResults.nCurGridFindIndex = -1;
                dataGridViewFindResults.listSelection = listGridViewSelection;
                dataGridViewFindResults.pageNo = pgno;
                dataGridViewFindResults.searchText = parentForm.textBox_Find.Text;
                dataGridViewFindResults.nissayaDataGridView = this;
                dataGridViewFindResults.nissayaTabPage = parentInstance;
                dataGridViewFindResults.nIndexFindPageResults = DataGridViewFindPageList.Count;

                DataGridViewFindPageList.Add(TabPageKey, dataGridViewFindResults);
                parentForm.UpdateTotalFindCountView(listGridViewSelection.Count);
            }
            // not used
            List<_GridViewSelection> FindGridMatches(string SearchText)
            {
                List<_GridViewSelection> listGridViewMatches = new List<_GridViewSelection>();
                // 2022-05-30
                List<int> colFound = new List<int>();

                int rowIndex = -1;
                int colno = -1;
                // this is for the currently opened DataGridView()
                if (!parentInstance.UsingSecondaryContainer())
                {
                    DataGridView dgv = (DataGridView)this;

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.Cells[2].Value != null &&
                            row.Cells[2].Value.ToString().Contains(parentForm.textBox_Find.Text)) colFound.Add(2);
                        if (row.Cells[3].Value != null &&
                            row.Cells[3].Value.ToString().Contains(parentForm.textBox_Find.Text)) colFound.Add(3);
                        if (row.Cells[4].Value != null &&
                            row.Cells[4].Value.ToString().Contains(parentForm.textBox_Find.Text)) colFound.Add(4);

                        if (colFound.Count > 0)
                        {
                            rowIndex = row.Index;
                            foreach (int col in colFound)
                            {
                                this[col, rowIndex].Style.BackColor = Color.Yellow;
                                listGridViewMatches.Add(new _GridViewSelection(rowIndex, col));
                            }
                            colFound.Clear();
                        }
                    }
                }
                else
                {
                    List<_NissayaDataRecord> NissayaGridDataList = parentInstance.GetDataList();
                    if (NissayaGridDataList.Count > 0)
                    {
                        colno = -1; rowIndex = -1;
                        foreach (_NissayaDataRecord rec in NissayaGridDataList)
                        {
                            ++rowIndex;
                            if (rec.pali.Contains(parentForm.textBox_Find.Text)) colno = 2;
                            if (colno == -1 && rec.plain.Contains(parentForm.textBox_Find.Text)) colno = 3;
                            if (colno == -1 && rec.footnote.Contains(parentForm.textBox_Find.Text)) colno = 4;
                            if (colno != -1)
                            {
                                listGridViewMatches.Add(new _GridViewSelection(rowIndex, colno));
                                colno = -1;
                            }
                        }
                    }
                }

                return listGridViewMatches;
            }

            public bool dataGridViewFindNext(bool flagNext = true)
            {
                if (CurActiveGridPageFindInfo == null) return false;
                parentInstance.curFindPgNo = parentInstance.curPageNo = CurActiveGridPageFindInfo.pageNo;
                //parentInstance.curFindPgNo = CurActiveGridPageFindInfo.pageNo;
                string TabPageKey = parentInstance.GetThisTabPageKey();
                DataGridViewFindPage dataGridViewFindResults = null;
                if (DataGridViewFindPageList.ContainsKey(TabPageKey))
                    dataGridViewFindResults = DataGridViewFindPageList[TabPageKey];
                if (dataGridViewFindResults == null) return false;

                List<_GridViewSelection> listGridViewSelection = dataGridViewFindResults.listSelection;

                if (dataGridViewFindResults.listSelection == null ||
                    dataGridViewFindResults.listSelection.Count == 0) return true;

                // highlight data
                if ((parentForm.CurPageHighlightType & HightlightType.HighlightText) == HightlightType.HighlightText)
                {
                    //this.ClearSelection();
                    foreach (_GridViewSelection sel in dataGridViewFindResults.listSelection)
                    {
                        this[sel.col, sel.row].Style.BackColor = Color.Yellow;
                    }
                }

                if ((parentForm.CurPageHighlightType & HightlightType.HighlightSelectedText) == HightlightType.HighlightSelectedText)
                {
                    switch (flagNext)
                    {
                        case true:
                            if (dataGridViewFindResults.nCurGridFindIndex + 1 >= listGridViewSelection.Count) return false;
                            ++dataGridViewFindResults.nCurGridFindIndex;
                            parentForm.UpdateTotalFindIndexView(1);
                            break;
                        case false:
                            if (dataGridViewFindResults.nCurGridFindIndex - 1 < 0) return false;
                            --dataGridViewFindResults.nCurGridFindIndex;
                            parentForm.UpdateTotalFindIndexView(-1);
                            break;
                    }
                }
                if ((parentForm.CurPageHighlightType & HightlightType.HighlightSelectedText) == HightlightType.HighlightSelectedText ||
                    (parentForm.CurPageHighlightType & HightlightType.HighlightCurrentSelectedText) == HightlightType.HighlightCurrentSelectedText)
                {
                    int n = 0, row = 0, col = 0;
                    n = dataGridViewFindResults.nCurGridFindIndex;
                    if (n == -1) n = 0;
                    if (n >= listGridViewSelection.Count) n = listGridViewSelection.Count - 1;
                    row = listGridViewSelection[n].row;
                    col = listGridViewSelection[n].col;

                    this[dataGridViewFindResults.nCurSelectedColIndex, dataGridViewFindResults.nCurSelectedRowIndex].Selected = false;
                    //this[col, row].Selected = true;
                    dataGridViewFindResults.nCurSelectedColIndex = col;
                    dataGridViewFindResults.nCurSelectedRowIndex = row;
                    this.CurrentCell = this.Rows[row].Cells[0];
                    //this[0, row].Selected = true;
                    this[col, row].Selected = true;
                }
                PerformLayout();   // adjust scrollbar position
                return true;    // sucessful
            }

            public void DataGridViewFindClear(bool clearAllResults = false)
            {
                // clear textBox_Find 
                //parentForm.FindClear();
                DataGridViewFindPageList.Clear();
                /*
                if (listGridViewSelection != null && listGridViewSelection.Count > 0)
                {
                    foreach (_GridViewSelection g in listGridViewSelection)
                        this[g.col, g.row].Style.BackColor = Color.White;
                    listGridViewSelection.Clear();
                }
                nCurGridFindIndex = 0;
                */
                this.ClearSelection();
                this.DefaultCellStyle.SelectionBackColor = defaultDataGridViewSelectionColor;
                //if (clearAllResults) dictTableFindResults.Clear();
                //else dictTableFindResults.Remove(curPageNo);
            }

            public void DataGridViewFindPageClear()
            {
                //nCurGridFindIndex = 0;
                FindStatus = string.Empty;
                SearchText = string.Empty;
                //listGridViewSelection.Clear();
            }

            public void InsertText()
            {
                int n = dataBox.SelectionStart + Clipboard.GetText().Length;
                dataBox.Paste();
                dataBox.SelectionStart = n;
                dataBox.Focus();
                parentInstance.dataGridView1_Updated = true;
            }

            //***********************************************************************************

            protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
            {
                //if (keyData == (Keys.Control | Keys.W))
                //{
                //    //parentInstance.ShowDataBox();
                //    return true;
                //}
                if (keyData == (Keys.Control | Keys.D))
                {
                    parentInstance.ShowDataBox();
                    return true;
                }

                if (keyData == (Keys.Control | Keys.S))
                {
                    this.EndEdit();
                    parentForm.button_Save_Click(null, null);
                    return true;
                }

                int index = this.CurrentCell.ColumnIndex;
                if (index == dgvColRecordType)
                {
                    bool processed = false;
                    //TextBox editingBox = (TextBox)this.EditingControl;
                    switch (keyData)
                    {
                        case (Keys.Control | Keys.D3):  // #
                            this.CurrentCell.Value = "#";
                            //this.EditingControl.Text = "#";
                            this.CurrentCell.Style.ForeColor = DataInfo.GetColor('#');
                            //this.EditingControl.ForeColor = DataInfo.GetColor('#');
                            //editingBox.Select(0, 0);
                            //Clipboard.SetText("#");
                            //editingBox.Paste();
                            processed = true;
                            break;
                        case (Keys.Control | Keys.D8):  // *
                            this.CurrentCell.Value = "*";
                            this.CurrentCell.Style.ForeColor = DataInfo.GetColor('*');
                            //this.EditingControl.Text = "*";
                            //this.EditingControl.ForeColor = DataInfo.GetColor('*');
                            //editingBox.Select(0, 0);
                            //Clipboard.SetText("*");
                            //editingBox.Paste();
                            processed = true;
                            break;
                        default:
                            break;
                    }
                    if (processed) return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}
