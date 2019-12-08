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
        bool isChangingValue = false;       //Is a grid cell value changing programmatically (disables reentry of event handler)
        string selectedColumnName = "Select";

        EventHandler addDelegate;
        EventHandler removeDelegate;
        EventHandler editDelegate;

        public GridControl()
        {
            InitializeComponent();
            addButton.Click += AddButton_Click;
            editButton.Click += EditButton_Click;
            removeButton.Click += RemoveButton_Click;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            removeDelegate?.Invoke(sender, e);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            DataRow[] found = getSelectedItems();
            if (found == null || found.Length == 0)
                return;
            if (found.Length > 1)
            {
                MessageBox.Show(this, "Selected one item at a time", "Cannot Select Multiple Items", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            editDelegate?.Invoke(sender, e);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            addDelegate?.Invoke(sender, e);
        }

        public GridControl(bool allowSelect, bool singleSelect, bool allowAdd,bool allowRemove, bool allowEdit,string selectColumnName = "Select")
        {
            InitializeComponent();
            init(allowSelect, singleSelect, allowAdd, allowRemove, allowEdit, selectColumnName);
        }
        public void init(bool allowSelect, bool singleSelect, bool allowAdd, bool allowRemove, bool allowEdit, string selectColumnName = "Select")
        {
            removeDelegate = null;
            editDelegate = null;
            addDelegate = null;
            selectionAllowed = allowSelect;
            multiSelectionAllowed = !singleSelect;
            addButton.Visible = allowAdd;
            removeButton.Visible = allowRemove;
            editButton.Visible = allowEdit;
            removeButton.Enabled = false;
            editButton.Enabled = false;
            selectedColumnName = selectColumnName;
        }
        public void setEditButtonName(string editButtonName)
        {
            editButton.Text = editButtonName;
        }

        public DataTable load(DataTable tbl, EventHandler addNewHandler, EventHandler editHandler, EventHandler removeHandler)
        {
            DataTable dataToLoad = new DataTable();
            if (selectionAllowed)
            {
                DataColumn newColumn = new DataColumn(selectedColumnName, typeof(Boolean));
                newColumn.ReadOnly = false;
                dataToLoad.Columns.Add(newColumn);
            }
            foreach (DataColumn column in tbl.Columns)
            {
                DataColumn newColumn = new DataColumn(column.ColumnName, column.DataType, column.Expression, column.ColumnMapping);
                newColumn.ReadOnly = true;
                dataToLoad.Columns.Add(newColumn);
            }
            foreach (DataRow x in tbl.Rows)
            {
                int numberOfItems = selectionAllowed ? x.ItemArray.Length + 1 : x.ItemArray.Length;
                object[] items = new object[numberOfItems];
                int offset = 0;
                if(selectionAllowed)
                {
                    items[offset] = false;
                    offset++;
                }
                foreach(var item in x.ItemArray)
                {
                    items[offset] = item;
                    offset++;
                }
                if (selectionAllowed)
                    dataToLoad.Rows.Add(items);
                else
                    dataToLoad.Rows.Add(items);
            }
            dataToLoad.DefaultView.RowFilter = tbl.DefaultView.RowFilter;
            controlGrid.DataSource = dataToLoad;
            fitSize();

            if (addNewHandler != null)
                addDelegate  = addNewHandler;
            if (editHandler != null)
                editDelegate = editHandler;
            if (removeHandler != null)
                removeDelegate = removeHandler;
            return dataToLoad;
        }

        public void addRow(params object [] args)
        {
            DataTable tbl = (DataTable)controlGrid.DataSource;
            object[] vars = new object[selectionAllowed ? args.Length + 1 : args.Length];
            if (selectionAllowed)
                vars[0] = false;
            for (int i = 0; i < args.Length; i++)
                vars[selectionAllowed ? i + 1 : i] = args[i];
            tbl.Rows.Add(vars);
        }

        public void removeRow(DataRow row)
        {
            DataTable tbl = (DataTable)controlGrid.DataSource;
            tbl.Rows.Remove(row);
        }

        public DataTable GridDataSource
        {
            get
            {
                return this.controlGrid.DataSource as DataTable;
            }
        }

        public DataRow[] getSelectedItems()
        {
            DataTable tbl = (DataTable)controlGrid.DataSource;
            return tbl.Select(selectedColumnName + "=TRUE");
        }

        private void controlGrid_CellValueChanging(object sender, DataGridViewCellEventArgs e)
        {
            if (isChangingValue || e.ColumnIndex != 0|| e.RowIndex < 0 || !selectionAllowed)
                return;

            DataGridViewRow selectedRow = controlGrid.Rows[e.RowIndex];
            DataGridViewCell cell = selectedRow.Cells[e.ColumnIndex];
            bool val = (bool)cell.Value;
            int totalSelected = val ? 1 : 0;

            isChangingValue = true;
            foreach (DataGridViewRow row in controlGrid.Rows)
            {
                if (row == selectedRow)
                    continue;
                bool currentCellVal = (bool)row.Cells[0].Value;
                if (!multiSelectionAllowed && currentCellVal)
                    row.Cells[0].Value = false;
                else if (currentCellVal)
                    totalSelected++;
            }
            if (totalSelected > 0)
            {
                editButton.Enabled = true;
                removeButton.Enabled = true;
            }
            else
            {
                editButton.Enabled = false;
                removeButton.Enabled = false;
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
                    row.MinimumHeight = (int)Math.Ceiling(neededHeight.Height) + 5;
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

        private void editButton_Click(object sender, EventArgs e)
        {
           
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            
        }

        private void controlGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (controlGrid.IsCurrentCellDirty)
            {
                controlGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        
        public void selectRows(List<DataRow> rows)
        {
            DataTable tbl = (DataTable)controlGrid.DataSource;
            string expression = "";
            int argIndex = 0;
            foreach(DataColumn column in tbl.Columns)
            {
                if (column.ColumnName == selectedColumnName)
                    continue;
                expression += '[' + column.ColumnName + "]='{" + argIndex + "}' AND ";
                argIndex++;
            }
            expression = expression.Substring(0, expression.Length - 5);
            foreach(DataRow row in rows)
            {
                string selectExpression = String.Format(expression, row.ItemArray);
                DataRow[] foundRows = tbl.Select(selectExpression);
                if (foundRows == null || foundRows.Length == 0)
                    continue;
                foundRows[0][selectedColumnName] = true;
            }
        }

        private void controlGrid_Sorted(object sender, EventArgs e)
        {
            this.fitSize();
        }
    }
}
