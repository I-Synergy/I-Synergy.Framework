using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Serialization;
using System;
using System.Linq;

namespace ISynergy.Framework.Synchronization.Core.Tests.Converters
{
    public class DateConverter : IConverter
    {
        public string Key => "cuscom";

        public void BeforeSerialize(SyncRow row)
        {
            // Convert all DateTime columns to ticks
            foreach (var col in row.Table.Columns.Where(c => c.GetDataType() == typeof(DateTime)))
                if (row[col.ColumnName] != null)
                    row[col.ColumnName] = ((DateTime)row[col.ColumnName]).Ticks;
        }

        public void AfterDeserialized(SyncRow row)
        {
            // Convert all DateTime back from ticks
            foreach (var col in row.Table.Columns.Where(c => c.GetDataType() == typeof(DateTime)))
                if (row[col.ColumnName] != null)
                    row[col.ColumnName] = new DateTime(Convert.ToInt64(row[col.ColumnName]));
        }
    }
}
