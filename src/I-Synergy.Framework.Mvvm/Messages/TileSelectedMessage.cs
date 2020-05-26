using ISynergy.Framework.Mvvm.Messaging;

namespace ISynergy.Framework.Mvvm.Messages
{
    /// <summary>
    /// Class TileSelectedMessage.
    /// Implements the <see cref="EventMessage" />
    /// </summary>
    /// <seealso cref="EventMessage" />
    public class TileSelectedMessage : EventMessage
    {
        /// <summary>
        /// Gets the name of the tile.
        /// </summary>
        /// <value>The name of the tile.</value>
        public string TileName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileSelectedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tileName">Name of the tile.</param>
        public TileSelectedMessage(object sender, string tileName)
            : base(sender)
        {
            TileName = tileName;
        }
    }
}
