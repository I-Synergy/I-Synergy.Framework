using MessagePack;
using System;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.MessageBus.Performance.Models
{
    /// <summary>
    /// Class TestModel.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class TestModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number.</value>
        public int Number { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; set; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public object Exception { get; set; }
    }
}
