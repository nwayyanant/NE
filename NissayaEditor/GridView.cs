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
        bool dataGridView_AppendMode = true;
        int dataGridView1_MouseClickRowIndex = -1;
        const string menuItem_InsertRowText = "Insert row(s)";
        const string menuItem_DeleteRowText = "Delete row(s)";
        const string menuItem_AppendRowText = "Append rows";
        const string menuItem_AppEndRowText = "Append ends";

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip cms = new ContextMenuStrip();
                cms.Items.Add(menuItem_InsertRowText);
                cms.Items.Add(menuItem_DeleteRowText);
                cms.Items.Add(menuItem_AppendRowText);
                dataGridView1_MouseClickRowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                cms.Show(PointToScreen(e.Location));
                cms.ItemClicked += new ToolStripItemClickedEventHandler(contexMenu_ItemClicked);
            }
        }

        private void contexMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (dataGridView1_MouseClickRowIndex == -1) return;
            
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            ToolStripItem item = e.ClickedItem;
            quitAppendMode();
            switch (item.Text)
            {
                case menuItem_InsertRowText:
                    insertDataRow(dataGridView1_MouseClickRowIndex, dataGridView1.SelectedRows.Count);
                    break;
                case menuItem_DeleteRowText:
                    deleteDataRow(dataGridView1_MouseClickRowIndex);
                    break;
                case menuItem_AppendRowText:
                    dataGridView1.RowsAdded += dataGridView1_RowsAdded;
                    dataGridView_AppendMode = true;
                    addAppendRow();
                    break;
            }
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridValue1_Updated = true;
            dataGridView1_MouseClickRowIndex = -1;
        }

        private void insertDataRow(int index, int rowCount)
        {
            if (rowCount == 0) rowCount = 1;
            while (rowCount-- > 0)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Height = 35;
                dataGridView1.Rows.Insert(dataGridView1_MouseClickRowIndex, newRow);
                dataGridView1.Rows[dataGridView1_MouseClickRowIndex].Cells[0].ReadOnly = true;
                //nissayaData.Insert(index, new NissayaData(dataGridView1_MouseClickRowIndex + 1));
            }
            // adjust row numbers for all rows below the new added row
            while (index < dataGridView1.Rows.Count)
                dataGridView1.Rows[index].Cells[0].Value = (++index).ToString();
        }

        private void deleteDataRow(int index)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                index = -1;
                //int srno = -1;
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    index = row.Index; // Int16.Parse(row.Cells[0].Value.ToString());
                    dataGridView1.Rows.RemoveAt(row.Index);
                }
            }
            else
                dataGridView1.Rows.RemoveAt(index);

            // adjust row numbers for all rows below the new added row
            while (index < dataGridView1.Rows.Count)
                dataGridView1.Rows[index].Cells[0].Value = (++index).ToString();
        }

        private void addAppendRow()
        {
            int rowIndex = dataGridView1.Rows.Count - 1;
            dataGridView1.FirstDisplayedScrollingRowIndex = rowIndex;
            dataGridView1.Rows[rowIndex].Cells[0].Value = (rowIndex + 1).ToString();
            dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[1];
            dataGridView1.BeginEdit(true);
        }

        private void quitAppendMode()
        {
            if (dataGridView_AppendMode)
            {
                dataGridView1.RowsAdded -= dataGridView1_RowsAdded;
                dataGridView_AppendMode = false;
            }
        }

        private void colorDataGridViewCell()
        {
            int n = dataGridView1.Rows.Count;
            for (int i = 0; i < n; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    switch (dataGridView1.Rows[i].Cells[2].Value.ToString()[0])
                    {
                        case '#':
                            dataGridView1.Rows[i].Cells[2].Style =
                                new DataGridViewCellStyle { ForeColor = Color.RoyalBlue };
                            break;
                        case '@':
                            dataGridView1.Rows[i].Cells[2].Style.ForeColor = Color.LightSalmon;
                            break;
                        default:
                            dataGridView1.Rows[i].Cells[2].Style.ForeColor = Color.DimGray;
                            break;
                    }
                }
            }
        }
	}
}
