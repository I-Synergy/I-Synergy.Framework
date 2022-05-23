using ISynergy.Framework.Core.Extensions;
using System.Text.Json;

namespace ISynergy.Framework.Core.Policies
{
    public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public static JsonSnakeCaseNamingPolicy Instance { get; } = new JsonSnakeCaseNamingPolicy();

        public override string ConvertName(string name) =>
            name.ToSnakeCase();
    }
}
