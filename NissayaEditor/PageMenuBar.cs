using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Input;
using System.Reflection;

namespace NissayaEditor
{
    public partial class Form1 : Form
    {
        class PageMenuBar : DataGridView
        {
            const string newPageNo = "1";
            string[] numbMyanmar = { "၀", "၁", "၂", "၃", "၄", "၅", "၆", "၇", "၈", "၉" };
            Dictionary<char, char> numbMyanmarRoman = new Dictionary<char,char>()
            { 
                {'၀', '0'} , {'၁', '1'}, {'၂', '2'}, {'၃', '3'}, {'၄', '4'}, 
                {'၅', '5'}, {'၆', '6'}, {'၇', '7' }, {'၈', '8' }, {'၉', '9'} 
            };
            Dictionary<char, char> numbRomanMyanmar = new Dictionary<char, char>()
            { 
                {'0', '၀'} , {'1', '၁'}, {'2', '၂'}, {'3', '၃'}, {'4', '၄'}, 
                {'5', '၅'}, {'6', '၆'}, {'7', '၇'}, {'8', '၈'}, {'9', '၉'} 
            };

            int curRowIndex = 0;
            VScrollBar vsb = null;
            
            const int vsbWidth = 5;

            public PageMenuBar()
            {
                Boolean UserVSB = false;
                this.ScrollBars = ScrollBars.Vertical;
                if (UserVSB)
                {
                    vsb = new VScrollBar();
                    vsb.Width = vsbWidth;
                    vsb.AccessibleName = "Small Vertical Scroll Bar";
                    this.Controls.Add(vsb);
                    vsb.Visible = false;
                }
                initialize();
            }

            public void initialize()
            {
                //this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells | DataGridViewAutoSizeRowsMode.AllHeaders;
                this.ColumnCount = 1;

                if (vsb == null)
                {
                    this.Location = new Point(12, 70);
                    this.Size = new Size(57, 445); // 40, 445
                }
                else
                {
                    this.Location = new Point(4, 70); // 12, 70
                    this.Size = new Size(52, 445);  // 40, 445
                    vsb.Location = new Point(vsb.Location.X + 41, vsb.Location.Y + 1);
                    vsb.Size = new Size(vsbWidth, 443);
                    vsb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                    vsb.Scroll += vsb_Scroll;
                }
                // anchor menu to parent form
                this.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                // column header setup
                this.RowTemplate.Height = 24;
                //dataGridView1.RowHeadersVisible = false;
                //this.ColumnHeadersHeight = 50;
                //this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                this.RowHeadersVisible = false;
                this.RowHeadersWidth = 30;
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                this.ColumnHeadersHeight = 45;
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                
                // set col widths
                this.Columns[0].Width = 40;
                
                // set column header text and font
                this.Columns[0].HeaderText = "New\nPage";

                this.Columns[0].HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 9F);

                this.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // set column colors
                this.Columns[0].DefaultCellStyle.ForeColor = Color.Black;
                this.Columns[0].DefaultCellStyle.BackColor = Color.White;
                //this.Rows[0].Cells[0].Style.BackColor = Color.FromArgb(168, 208, 230); //Color.PaleGoldenrod;//.Linen;
                //this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                // set fonts
                this.Columns[0].DefaultCellStyle.Font = new Font("Myanmar Text", 9F); ;

                // set other attributes
                this.Columns[0].ReadOnly = true;

                this.AllowUserToResizeColumns = false;
                this.AllowUserToResizeRows = false;
                this.AllowUserToAddRows = false;
                this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                this.MultiSelect = false;

                this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

                this.EnableHeadersVisualStyles = false;
                this.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                this.RowHeadersDefaultCellStyle.BackColor = Color.LightGray;

                //this.ScrollBars = System.Windows.Forms.ScrollBars.None;
                this.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;//.DisplayedCells;

                this.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

                this.MouseClick -= pageMenuBar_MouseClick;
                this.MouseClick += pageMenuBar_MouseClick;
                this.ColumnHeaderMouseClick += PageMenuBar_ColumnHeaderMouseClick;
                //this.SizeChanged += PageMenuBar_SizeChanged;
                //this.RowsAdded += dataGridView_RowsAdded;
                //this.RowsRemoved += dataGridView_RowsRemoved;
                //this.Rows.Add();
                //if (this.Rows.Count >= 1 && this.Rows[0].Cells[0].Value == null)
                //    this.Rows[0].Cells[0].Value = "New";
                bool flagRow = this.IsCurrentRowDirty;
                bool flagEdit = this.IsCurrentCellInEditMode;
                this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            private void vsb_Scroll(object sender, ScrollEventArgs e)
            {
                if (e.NewValue < RowCount)
                    this.FirstDisplayedScrollingRowIndex = e.NewValue;
            }
            private void PageMenuBar_SizeChanged(object sender, System.EventArgs e)
            {
                int nRows = 0;
                if (Rows.Count == 0) return;
                nRows = this.Height / this.Rows[0].Height;
                if (vsb != null)
                {
                    vsb.LargeChange = nRows;
                    vsb.Visible = (this.RowCount >= nRows);
                }
                else
                {
                    this.VerticalScrollBar.Visible = (this.RowCount >= nRows);
                }
                //if (this.vsb.Visible) //step 2
                //{
                //    this.vsb.Value = this.vsb.Height;
                //}
            }

            private void ShowLastRow()
            {
                int nRows = this.Height / this.Rows[0].Height;
                if (nRows <= RowCount)
                    this.FirstDisplayedScrollingRowIndex = RowCount - nRows + 1;
            }

            // #MENU
            public void PageMenuBar_ColumnHeaderMouseClick(object sender, MouseEventArgs e)
            {
                if (this.Rows.Count == 1 && ((Form1)this.Parent).IsNewPage())
                {
                    this.CurrentRow.Selected = false;
                    this.Rows[curRowIndex].Selected = true;
                    //this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                    MessageBox.Show(((Form1)this.Parent).SaveDataToFile);
                    return;
                }
                this.Rows.Add();
                PageMenuBar_SizeChanged(this, null);
                this.ShowLastRow();
                if (this.Rows.Count > 1) ((Form1)this.Parent).SavePageData();
                string np = newPageNum();
                this.CurrentRow.Selected = false;
                this.Rows[curRowIndex].Selected = false;
                //this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                this.Rows[this.Rows.Count - 1].Cells[0].Value = np;
                this.Rows[this.Rows.Count - 1].Selected = true;
                curRowIndex = this.Rows.Count - 1;
                ((Form1)this.Parent).NewPage(np);
            }

            public void pageMenuBar_MouseClick(object sender, MouseEventArgs e)
            {
                // if mouse was clicked outside of the page cells just return
                if (this.HitTest(e.X, e.Y).RowIndex == -1) return;
                
                //if (this.CurrentCell.Value != null && this.CurrentCell.Value.ToString() == "New")
                //{
                //    // save if there is an unsaved page
                //    if (this.Rows.Count == 2 && ((Form1)this.Parent).IsNewPage())
                //    {
                //        this.CurrentRow.Selected = false;
                //        this.Rows[curRowIndex].Selected = true;
                //        this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                //        MessageBox.Show(((Form1)this.Parent).SaveDataToFile);
                //        return;
                //    }
                //    this.Rows.Add();
                //    PageMenuBar_SizeChanged(this, null);
                //    DataGridViewRow r = Rows[0];
                //    DataGridViewCellCollection c = r.Cells;
                //    string s = c[0].Value.ToString();
                //    //Rows[0].Frozen = true;
                //    this.ShowLastRow();
                //    //if (FirstDisplayedScrollingRowIndex > 0)
                //    //    this.FirstDisplayedScrollingRowIndex--;
                //    //vsb.Value = vsb.Maximum;
                //    //ShowLastRow();
                //    if (this.Rows.Count == 2) this.Rows[0].Cells[0].Value = "New";
                //    else
                //        this.Rows[this.Rows.Count - 2].Cells[0].Value = this.Rows[this.Rows.Count - 1].Cells[0].Value;

                //    //this.Rows.Insert(this.Rows.Count - 1, 1);
                //    //this.Rows.Add();
                //    ((Form1)this.Parent).SavePageData();
                //    string np = newPageNum();
                //    this.CurrentRow.Selected = false;
                //    this.Rows[curRowIndex].Selected = false;
                //    this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                //    this.Rows[this.Rows.Count - 1].Cells[0].Value = np;
                //    this.Rows[this.Rows.Count - 1].Selected = true;
                //    curRowIndex = this.Rows.Count - 1;
                //    ((Form1)this.Parent).NewPage(np);
                //}
                //else
                {
                    // this is a same page just return
                    if (curRowIndex == this.HitTest(e.X, e.Y).RowIndex) return;
                    curRowIndex = this.CurrentRow.Index;
                    //this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                    ((Form1)this.Parent).LoadPageData(this.CurrentRow.Cells[0].Value.ToString());                   
                }
            }

            public void pageMenuBar_CellEvent(object sender, DataGridViewCellMouseEventArgs e)
            {
                // if mouse was clicked outside of the page cells just return
                if (this.HitTest(e.X, e.Y).RowIndex == -1) return;

                //if (this.CurrentCell.Value != null && this.CurrentCell.Value.ToString() == "New")
                //{
                //    // save if there is an unsaved page
                //    if (this.Rows.Count == 2 && ((Form1)this.Parent).IsNewPage())
                //    {
                //        this.CurrentRow.Selected = false;
                //        this.Rows[curRowIndex].Selected = true;
                //        this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                //        MessageBox.Show(((Form1)this.Parent).SaveDataToFile);
                //        return;
                //    }
                //    this.Rows.Add();
                //    PageMenuBar_SizeChanged(this, null);
                //    if (this.Rows.Count == 2) this.Rows[0].Cells[0].Value = "New";
                //    else
                //        this.Rows[this.Rows.Count - 2].Cells[0].Value = this.Rows[this.Rows.Count - 1].Cells[0].Value;

                //    //this.Rows.Insert(this.Rows.Count - 1, 1);
                //    //this.Rows.Add();
                //    ((Form1)this.Parent).SavePageData();
                //    string np = newPageNum();
                //    this.CurrentRow.Selected = false;
                //    this.Rows[curRowIndex].Selected = false;
                //    this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                //    this.Rows[this.Rows.Count - 1].Cells[0].Value = np;
                //    this.Rows[this.Rows.Count - 1].Selected = true;
                //    curRowIndex = this.Rows.Count - 1;
                //    ((Form1)this.Parent).NewPage(np);
                //}
                //else
                {
                    // this is a same page just return
                    if (curRowIndex == this.HitTest(e.X, e.Y).RowIndex) return;
                    curRowIndex = this.CurrentRow.Index;
                    //this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                    ((Form1)this.Parent).LoadPageData(this.CurrentRow.Cells[0].Value.ToString());
                }
            }

            public void NewPageSelected(string pgno)
            {
                if (curRowIndex == GetRowIndex(pgno)) return;
                curRowIndex = this.CurrentRow.Index;
                this.Rows[curRowIndex].Selected = false;
                curRowIndex = GetRowIndex(pgno);
                this.Rows[curRowIndex].Selected = true;
                //this.Rows[0].Cells[0].Style.BackColor = Color.Linen;
                this.CurrentCell = this.Rows[curRowIndex].Cells[0];
            }

            private string newPageNum()
            {
                if (this.Rows.Count == 1) return newPageNo;
                // int prevRowIndex = this.Rows.Count - 2;
                string s = this.Rows[this.Rows.Count - 2].Cells[0].Value.ToString();
                return incrementPageNo(s);
            }

            private string incrementPageNo(string s)
            {
                string decStr = string.Empty;
                if (s.Length == 0) return newPageNo;
                bool flagMyanmarNumbers = false;
                if (numbMyanmar.Contains(s[0].ToString()))
                {
                    flagMyanmarNumbers = true;
                    foreach (char c in s)
                    {
                        if (numbMyanmarRoman.ContainsKey(c))
                            decStr += numbMyanmarRoman[c];
                    }
                }
                else decStr = s;
                s = (Convert.ToInt32(decStr) + 1).ToString();
                decStr = s;
                if (flagMyanmarNumbers)
                {
                    decStr = string.Empty;
                    foreach (char c in s)
                    {
                        if (numbRomanMyanmar.ContainsKey(c))
                            decStr += numbRomanMyanmar[c];
                    }
                }
                return decStr;
            }

            public void FillPageMenu(List<string> pgno, string curPage = "")
            {
                //if (pgno.Count == 0) return;
                if (pgno.Count > 0)
                {
                    if (pgno.Count > 0 && pgno.Count > this.Rows.Count)
                    {
                        this.Rows.Add(pgno.Count - this.Rows.Count);
                    }
                    else
                    {
                        if (pgno.Count < this.Rows.Count)
                        {
                            for (int i = this.Rows.Count; i > pgno.Count; --i)
                            {
                                this.Rows.RemoveAt(1);
                            }
                        }
                    }
                }
                else
                {
                    this.Rows.Clear();
                    //MessageBox.Show("FillePageMenu with 0 entry");
                }
                // "New" has been removed
                curRowIndex = 0;
                int n = -1;
                //this.Rows[n].Cells[0].Value = "New";
                //this.Rows[n].Cells[0].Style.BackColor = Color.Linen;
                foreach (string s in pgno)
                {
                    this.Rows[++n].Cells[0].Value = s;
                    this.Rows[n].Cells[0].Style.BackColor = Color.White;
                    if (s == curPage) curRowIndex = n;
                }
                // first page is the current selected page
                this.ClearSelection();
                if (pgno.Count > 0)
                    this.Rows[curRowIndex].Selected = true;    // first page is selected
                if (vsb != null)
                {
                    vsb.Maximum = this.Height;
                    vsb.SmallChange = 1;
                    PageMenuBar_SizeChanged(this, new System.EventArgs());
                    //int nRows = this.Height / this.Rows[0].Height;
                    //vsb.LargeChange = nRows;
                    //vsb.Visible = (this.RowCount > nRows);
                }
            }
            private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
            {
                int height = 0;
                for (int i = e.RowIndex; i < (e.RowIndex + e.RowCount); i++)
                    height += this.Rows[i].Height;

                this.Height += height;
            }
            private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
            {
                int height = 0;
                for (int i = e.RowIndex; i < (e.RowIndex + e.RowCount); i++)
                    height += this.Rows[i].Height;

                this.Height -= height;
            }
            public void UpdateKey(string newKey, string oldKey)
            {
                if (this.Rows[curRowIndex].Cells[0].Value.ToString() == oldKey)
                    this.Rows[curRowIndex].Cells[0].Value = newKey;

                int i = 0;
                while (i < this.Rows.Count)
                {
                    if (this.Rows[i].Cells[0].Value.ToString() == oldKey)
                    {
                        this.Rows[i].Cells[0].Value = newKey;
                        break;
                    } 
                    ++i;
                }
            }

            public string GetCurrentPageNo()
            {
                if (this.Rows.Count == 0) return string.Empty;
                if (curRowIndex < 0 || curRowIndex >= this.Rows.Count)
                {
                    //MessageBox.Show("curRowIndex = " + curRowIndex.ToString() + " Rows.Count = " + this.Rows.Count.ToString()
                    //    + " Rows[0].Cells[0] = " + this.Rows[0].Cells[0].Value.ToString());
                    curRowIndex = 0;
                }
                return this.Rows[curRowIndex].Cells[0].Value.ToString();
            }

            public int GetRowIndex(string pgno)
            {
                int n = 0;
                foreach (DataGridViewRow r in this.Rows)
                {
                    if (r.Cells[0].Value.ToString() == pgno) return n;
                    ++n;
                }
                return -1;
            }
            /*
            public string NextPageNumber(string pageNo)
            {
                // returns the next page number of the current page
                // if pageNo is at the end, it loops back from the beginning.
                if (this.Rows.Count <= 2) return pageNo;
                bool found = false;
                string pg = string.Empty;
                foreach(DataGridViewRow row in this.Rows)
                {
                    pg = this.Rows[this.Rows.Count - 1].Cells[0].Value.ToString();
                    if (found) return pg;
                    if (pg == pageNo) found = true;
                }
                if (found) return this.Rows[0].Cells[0].Value.ToString();
                return pg;
            }
            */
            /*
            protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
            {
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
             * */
        }
    }
}
