using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    public class TileSelectedMessage : EventMessage
    {
        public string TileName { get; }

        public TileSelectedMessage(object sender, string tileName)
            : base(sender)
        {
            TileName = tileName;
        }
    }
}
