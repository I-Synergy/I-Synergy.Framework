using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Linq;

namespace ISynergy.Framework.Synchronization.Core.Tests.Converters
{
    public class DateConverter : IConverter
    {
        public string Key => "cuscom";

        public void BeforeSerialize(object[] row, SyncTable schemaTable)
        {
            // Convert all DateTime columns to ticks
            foreach (var col in schemaTable.Columns.Where(c => c.GetDataType() == typeof(DateTime)))
            {
                var index = schemaTable.Columns.IndexOf(col);

                if (row[index] != null)
                    row[index] = ((DateTime)row[index]).Ticks;
            }
        }

        public void AfterDeserialized(object[] row, SyncTable schemaTable)
        {
            // Convert all DateTime back from ticks
            foreach (var col in schemaTable.Columns.Where(c => c.GetDataType() == typeof(DateTime)))
            {
                var index = schemaTable.Columns.IndexOf(col);
                if (row[index] != null)
                    row[index] = new DateTime(Convert.ToInt64(row[index]));
            }
        }
    }
}
