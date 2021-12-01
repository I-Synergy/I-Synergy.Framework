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
        public SyncTable Table { get; set; }

        /// <summary>
        /// Creates an instance, in which data can be written to,
        /// with the default initial capacity.
        /// </summary>
        public SyncRow(int bufferSize)
        {
            buffer = new object[bufferSize];
            Length = bufferSize;
        }

        /// <summary>
        /// Add a new buffer row
        /// </summary>
        public SyncRow(SyncTable table, DataRowState state = DataRowState.Unchanged)
        {
            buffer = new object[table.Columns.Count];

            // set correct length
            Length = table.Columns.Count;

            // Get a ref
            Table = table;

            // Affect new state
            RowState = state;
        }


        /// <summary>
        /// Add a new buffer row. This ctor does not make a copy
        /// </summary>
        public SyncRow(SyncTable table, object[] row, DataRowState state = DataRowState.Unchanged)
        {
            // Direct set of the buffer
            buffer = row;

            // set correct length
            Length = row.Length;

            // Get a ref
            Table = table;

            // Affect new state
            RowState = state;

        }

        /// <summary>
        /// Gets or Sets the state of the row
        /// </summary>
        public DataRowState RowState { get; set; }

        /// <summary>
        /// Gets the row Length
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Get the value in the array that correspond to the column index given
        /// </summary>
        public object this[int index]
        {
            get => buffer[index];
            set => buffer[index] = value;
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
                var column = Table.Columns[columnName];

                if (column == null)
                    throw new ArgumentException("Column is null");

                var index = Table.Columns.IndexOf(column);

                return this[index];
            }
            set
            {
                var column = Table.Columns[columnName];

                if (column == null)
                    throw new ArgumentException("Column is null");

                var index = Table.Columns.IndexOf(column);

                this[index] = value;
            }
        }

        /// <summary>
        /// Get the inner array with state on Index 0. Need to replace with ReadOnlySpan{object} !!!!
        /// </summary>
        public object[] ToArray()
        {
            var array = new object[Length + 1];
            Array.Copy(buffer, 0, array, 1, Length);

            // set row state on index 0 of my buffer
            array[0] = (int)RowState;

            return array;
        }

        /// <summary>
        /// Import a raw array, containing state on Index 0
        /// </summary>
        public void FromArray(object[] row)
        {
            var length = Table.Columns.Count;

            if (row.Length != length + 1)
                throw new Exception("row must contains State on position 0 and UpdateScopeId on position 1");

            Array.Copy(row, 1, buffer, 0, length);
            RowState = (DataRowState)Convert.ToInt32(row[0]);
        }

        /// <summary>
        /// Clear the data in the buffer
        /// </summary>
        public void Clear()
        {
            Array.Clear(buffer, 0, buffer.Length);
            Table = null;
        }


        /// <summary>
        /// ToString()
        /// </summary>
        public override string ToString()
        {
            if (buffer == null || buffer.Length == 0)
                return "empty row";

            if (Table == null)
                return buffer.ToString();

            var sb = new StringBuilder();

            sb.Append($"[Sync state]:{RowState}");

            var columns = RowState == DataRowState.Deleted ? Table.GetPrimaryKeysColumns() : Table.Columns;

            foreach (var c in columns)
            {
                var o = this[c.ColumnName];
                var os = o == null ? "<NULL />" : o.ToString();

                sb.Append($", [{c.ColumnName}]:{os}");
            }

            return sb.ToString();
        }
    }
}
