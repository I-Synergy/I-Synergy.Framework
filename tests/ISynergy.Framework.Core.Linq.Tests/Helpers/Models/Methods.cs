namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    public class Methods
    {
        public bool Method1(int value)
        {
            return value == 1;
        }

        public bool Method2(object value)
        {
            return value != null && (int)value == 1;
        }

        public bool Method3(object value)
        {
            return value is Item item && item.Value == 1;
        }

        public class Item
        {
            public int Value { get; set; }
        }
    }
}
