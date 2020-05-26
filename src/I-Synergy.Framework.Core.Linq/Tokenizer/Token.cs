namespace ISynergy.Framework.Core.Linq.Tokenizer
{
    /// <summary>
    /// Struct Token
    /// </summary>
    internal struct Token
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public TokenId Id { get; set; }
        /// <summary>
        /// Gets or sets the original identifier.
        /// </summary>
        /// <value>The original identifier.</value>
        public TokenId OriginalId { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Pos { get; set; }
    }
}
