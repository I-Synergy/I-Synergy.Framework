#if !NET9_0_OR_GREATER
using ISynergy.Framework.Core.Collections;
#endif

using System.Data;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// DataColumn collection extensions.
/// </summary>
public static class DataColumnCollectionExtensions
{
    /// <summary>
    ///   Creates and adds multiple <see cref="System.Data.DataColumn"/>
    ///   objects with the given names at once.
    /// </summary>
    /// 
    /// <param name="collection">The <see cref="System.Data.DataColumnCollection"/>
    /// to add in.</param>
    /// <param name="columnNames">The names of the <see cref="System.Data.DataColumn"/> to
    /// be created and added.</param>
    /// 
    /// <example>
    ///   <code>
    ///   DataTable table = new DataTable();
    ///   
    ///   // Add multiple columns at once:
    ///   table.Columns.Add("columnName1", "columnName2");
    ///   </code>
    /// </example>
    /// 
    public static void Add(this DataColumnCollection collection, params string[] columnNames)
    {
        for (int i = 0; i < columnNames.Length; i++)
            collection.Add(columnNames[i]);
    }

    /// <summary>
    ///   Creates and adds multiple <see cref="System.Data.DataColumn"/>
    ///   objects with the given names at once.
    /// </summary>
    /// 
    /// <param name="collection">The <see cref="System.Data.DataColumnCollection"/>
    ///   to add in.</param>
    /// <param name="columns">The names of the <see cref="System.Data.DataColumn"/>s to
    ///   be created and added, alongside with their types.</param>
    /// 
    /// <example>
    ///   <code>
    ///   DataTable table = new DataTable();
    ///   
    ///   // Add multiple columns at once:
    ///   table.Columns.Add(new OrderedDictionary&gt;String, Type&lt;()
    ///   {
    ///       { "columnName1", typeof(int)    },
    ///       { "columnName2", typeof(double) },
    ///   });
    ///   </code>
    /// </example>
    /// 
    public static void Add(this DataColumnCollection collection, OrderedDictionary<string, Type> columns)
    {
        foreach (var pair in columns.EnsureNotNull())
            collection.Add(pair.Key, pair.Value);
    }
}
