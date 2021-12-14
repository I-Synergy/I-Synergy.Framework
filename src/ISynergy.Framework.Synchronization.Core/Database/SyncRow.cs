using System;
using System.Data;
using System.Text;

namespace ISynergy.Framework.Synchronization.Core.Database
{
    public class SyncRow
    {
        // all the values for this line
        private object[] buffer;

        /// <summary>
        /// Gets or Sets the row's table
        /// </summary>
        public SyncTable SchemaTable { get; set; }

        /// <summary>
        /// Add a new buffer row
        /// </summary>
        public SyncRow(SyncTable schemaTable, DataRowState state = DataRowState.Unchanged)
        {
            // Buffer is +1 to store state
            buffer = new object[schemaTable.Columns.Count + 1];

            // set correct length
            Length = schemaTable.Columns.Count;

            // Get a ref
            SchemaTable = schemaTable;

            // Affect new state
            buffer[0] = (int)state;

        }


        /// <summary>
        /// Add a new buffer row. This ctor does not make a copy
        /// </summary>
        public SyncRow(SyncTable schemaTable, object[] row)
        {
            if (row.Length <= schemaTable.Columns.Count)
                throw new ArgumentException("row array must have one more item to store state");

            if (row.Length > schemaTable.Columns.Count + 1)
                throw new ArgumentException("row array has too many items");

            // Direct set of the buffer
            buffer = row;

            // set columns count as length
            Length = schemaTable.Columns.Count;

            // Get a ref
            SchemaTable = schemaTable;

        }

        /// <summary>
        /// Gets the state of the row
        /// </summary>
        public DataRowState RowState
        {
            get => (DataRowState)Convert.ToInt32(buffer[0]);
            set => buffer[0] = (int)value;
        }

        /// <summary>
        /// Gets the row Length
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Get the value in the array that correspond to the column index given
        /// </summary>
        public object this[int index]
        {
            get => buffer[index + 1];
            set => buffer[index + 1] = value;
        }

        /// <summary>
        /// Get the value in the array that correspond to the SchemaColumn instance given
        /// </summary>
        public object this[SyncColumn column] => this[column.ColumnName];

        /// <summary>
        /// Get the value in the array that correspond to the column name given
        /// </summary>
        public object this[string columnName]
        {
            get
            {
                var column = SchemaTable.Columns[columnName];

                if (column is null)
                    throw new ArgumentException("Column is null");

                var index = SchemaTable.Columns.IndexOf(column);

                return this[index];
            }
            set
            {
                var column = SchemaTable.Columns[columnName];

                if (column is null)
                    throw new ArgumentException("Column is null");

                var index = SchemaTable.Columns.IndexOf(column);

                this[index] = value;
            }
        }

        /// <summary>
        /// Get the inner copy array
        /// </summary>
        /// <returns></returns>
        public object[] ToArray() => buffer;


        /// <summary>
        /// Clear the data in the buffer
        /// </summary>
        public void Clear() => Array.Clear(buffer, 0, buffer.Length);


        /// <summary>
        /// ToString()
        /// </summary>
        public override string ToString()
        {
            if (buffer is null || buffer.Length == 0)
                return "empty row";

            if (SchemaTable is null)
                return buffer.ToString();

            var sb = new StringBuilder();

            sb.Append($"[Sync state]:{RowState}");

            var columns = RowState == DataRowState.Deleted ? SchemaTable.GetPrimaryKeysColumns() : SchemaTable.Columns;

            foreach (var c in columns)
            {
                var o = this[c.ColumnName];
                var os = o is null ? "<NULL />" : o.ToString();

                sb.Append($", [{c.ColumnName}]:{os}");
            }

            return sb.ToString();
        }
    }
}
