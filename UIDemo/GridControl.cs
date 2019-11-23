using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIDemo
{
    public partial class GridControl : UserControl
    {
        bool selectionAllowed = false;
        bool multiSelectionAllowed = false;
        public GridControl(bool allowSelect, bool singleSelect, bool allowAdd,bool allowRemove, bool allowEdit)
        {
            InitializeComponent();
            selectionAllowed = allowSelect;
            multiSelectionAllowed = !singleSelect;
            addButton.Visible = allowAdd;
            removeButton.Visible = allowRemove;
            editButton.Visible = allowEdit;
        }

        public void loadDataSet(DataTable tbl)
        {
            DataTable dataToLoad = new DataTable();
            if (selectionAllowed)
            {
                DataColumn newColumn = new DataColumn("Select", typeof(Boolean));
                newColumn.ReadOnly = false;
                dataToLoad.Columns.Add(newColumn);
            }
            foreach(DataColumn column in tbl.Columns)
            {
                column.ReadOnly = true;
                dataToLoad.Columns.Add(column);
            }
            foreach(DataRow x in tbl.Rows)
            {
                if (selectionAllowed)
                    dataToLoad.Rows.Add(false, x.ItemArray);
                else
                    dataToLoad.Rows.Add(x.ItemArray);
            }
            controlGrid.DataSource = dataToLoad;
            fitSize();
        }

        public void hookAddNew(EventHandler hdl)
        {
            addButton.Click += hdl;
        }

        public void hookEdit(EventHandler hdl)
        {
            editButton.Click += hdl;
        }

        public void hookRemove(EventHandler hdl)
        {
            removeButton.Click += hdl;
        }

        public DataRow[] getSelectedItems()
        {
            DataTable tbl = (DataTable)controlGrid.DataSource;
            return tbl.Select("Selected=TRUE");
        }

        bool isChangingValue = false;
        private void controlGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isChangingValue || e.ColumnIndex != 1 || e.RowIndex < 0 || !selectionAllowed ||  multiSelectionAllowed)
                return;

            DataGridViewRow selectedRow = controlGrid.Rows[e.RowIndex];
            DataGridViewCell cell = selectedRow.Cells[e.ColumnIndex];
            bool val = (bool)cell.Value;
            if (!val)
                return;
            isChangingValue = true;
            foreach(DataGridViewRow row in controlGrid.Rows)
            {
                if (row == selectedRow)
                    continue;
                if ((bool)row.Cells[0].Value)
                    row.Cells[0].Value = false;
            }
            isChangingValue = false;
        }

        private void fitSize()
        {
            resizeColumns();
            resizeRows();
            int i = controlGrid.Columns.Count - 1;
            for (; i >= 0; i--)
            {
                if (!controlGrid.Columns[i].Visible)
                    continue;
                controlGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                break;
            }
        }

        private void resizeRows()
        {
            using (Graphics g = this.CreateGraphics())
            {
                foreach (DataGridViewRow row in controlGrid.Rows)
                {
                    SizeF neededHeight = SizeF.Empty;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!cell.OwningColumn.Visible)
                            continue;
                        string cellValue = "";
                        if (cell.Value == null || cell.Value.GetType() == typeof(DBNull) || cell.Value.GetType() == typeof(bool))
                            continue;
                        if (cell.ValueType == typeof(string))
                            cellValue = (string)cell.Value;
                        else if (cell.ValueType == typeof(Int32))
                            cellValue = ((int)cell.Value).ToString();
                        else if (cell.ValueType == typeof(DateTime))
                            cellValue = ((DateTime)cell.Value).ToString();
                        SizeF cellSize;
                        cellSize = g.MeasureString(cellValue, Font);
                        if (cellSize.Height > neededHeight.Height)
                            neededHeight.Height = cellSize.Height;
                    }
                    row.MinimumHeight = (int)Math.Ceiling(neededHeight.Height);
                    row.Height = row.MinimumHeight;
                }
            }
        }

        private void resizeColumns()
        {
            using (Graphics g = this.CreateGraphics())
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                foreach (DataGridViewColumn column in controlGrid.Columns)
                {
                    if (!column.Visible)
                        continue;
                    SizeF headerSize = g.MeasureString(column.HeaderText, Font);
                    SizeF sizeNeeded = SizeF.Empty;
                    foreach (DataGridViewRow row in controlGrid.Rows)
                    {
                        DataGridViewCell cell = row.Cells[column.Index];
                        if (cell.Value == null || cell.Value.GetType() == typeof(DBNull) || cell.Value.GetType() == typeof(bool))
                            continue;
                        string cellValue = "";
                        if (cell.ValueType == typeof(string))
                            cellValue = (string)cell.Value;
                        else if (cell.ValueType == typeof(Int32))
                            cellValue = ((int)cell.Value).ToString();
                        else if (cell.ValueType == typeof(DateTime))
                            cellValue = ((DateTime)cell.Value).ToString();
                        else if (cell.ValueType == typeof(float))
                            cellValue = ((float)cell.Value).ToString();
                        SizeF cellSize;
                        cellSize = g.MeasureString(cellValue, Font);
                        if (cellSize.Width > sizeNeeded.Width)
                            sizeNeeded.Width = cellSize.Width;
                    }
                    int neededHeaderWidth = (int)Math.Ceiling(headerSize.Width);
                    int neededCellWidth = (int)Math.Ceiling(sizeNeeded.Width);
                    int newWidth = (neededHeaderWidth > neededCellWidth ? neededHeaderWidth : neededCellWidth) + 25;
                    column.Width = newWidth;
                    column.MinimumWidth = newWidth;
                }
            }
        }
    }
}
