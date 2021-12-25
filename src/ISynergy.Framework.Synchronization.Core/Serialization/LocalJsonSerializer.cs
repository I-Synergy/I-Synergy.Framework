using ISynergy.Framework.Synchronization.Core.Set;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Serialization
{

    public class LocalJsonSerializerFactory : ILocalSerializerFactory
    {
        public string Key => "json";
        public ILocalSerializer GetLocalSerializer() => new LocalJsonSerializer();
    }
    public class LocalJsonSerializer : ILocalSerializer
    {
        private StreamWriter sw;
        private JsonTextWriter writer;

        public string Extension => "json";

        public async Task CloseFileAsync(string path, SyncTable shemaTable)
        {
            // Close file
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
            await writer.CloseAsync();
            sw.Close();
        }
        public async Task OpenFileAsync(string path, SyncTable shemaTable)
        {
            if (writer is not null)
            {
                await writer.CloseAsync();
                writer = null;
            }

            sw = new StreamWriter(path);
            writer = new JsonTextWriter(sw) { CloseOutput = true };

            writer.WriteStartObject();
            writer.WritePropertyName("t");
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WritePropertyName("n");
            writer.WriteValue(shemaTable.TableName);
            writer.WritePropertyName("s");
            writer.WriteValue(shemaTable.SchemaName);
            writer.WritePropertyName("r");
            writer.WriteStartArray();
            writer.WriteWhitespace(Environment.NewLine);

        }
        public Task WriteRowToFileAsync(SyncRow row, SyncTable shemaTable)
        {
            writer.WriteStartArray();
            var innerRow = row.ToArray();
            for (var i = 0; i < innerRow.Length; i++)
                writer.WriteValue(innerRow[i]);
            writer.WriteEndArray();
            writer.WriteWhitespace(Environment.NewLine);
            writer.Flush();

            return Task.CompletedTask;
        }
        public Task<long> GetCurrentFileSizeAsync()
            => sw is not null && sw.BaseStream is not null ?
                Task.FromResult(sw.BaseStream.Position / 1024L) :
                Task.FromResult(0L);

        public IEnumerable<SyncRow> ReadRowsFromFile(string path, SyncTable shemaTable)
        {
            if (!File.Exists(path))
                yield break;

            JsonSerializer serializer = new JsonSerializer();
            using var reader = new JsonTextReader(new StreamReader(path));
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.ValueType == typeof(string) && reader.Value is not null && (string)reader.Value == "t")
                {
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.PropertyName && reader.ValueType == typeof(string) && reader.Value is not null && (string)reader.Value == "n")
                        {
                            reader.Read();

                            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                            {
                                if (reader.TokenType == JsonToken.PropertyName && reader.ValueType == typeof(string) && reader.Value is not null && (string)reader.Value == "r")
                                {
                                    // Go to children of the array
                                    reader.Read();
                                    // read all array
                                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                                    {
                                        if (reader.TokenType == JsonToken.StartArray)
                                        {
                                            var array = serializer.Deserialize<object[]>(reader);
                                            yield return new SyncRow(shemaTable, array);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }

        }
    }
}
