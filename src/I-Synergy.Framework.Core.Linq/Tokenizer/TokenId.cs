namespace ISynergy.Framework.Core.Linq.Tokenizer
{
    /// <summary>
    /// Enum TokenId
    /// </summary>
    internal enum TokenId
    {
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// The end
        /// </summary>
        End,
        /// <summary>
        /// The identifier
        /// </summary>
        Identifier,
        /// <summary>
        /// The string literal
        /// </summary>
        StringLiteral,
        /// <summary>
        /// The integer literal
        /// </summary>
        IntegerLiteral,
        /// <summary>
        /// The real literal
        /// </summary>
        RealLiteral,
        /// <summary>
        /// The exclamation
        /// </summary>
        Exclamation,
        /// <summary>
        /// The percent
        /// </summary>
        Percent,
        /// <summary>
        /// The amphersand
        /// </summary>
        Amphersand,
        /// <summary>
        /// The open paren
        /// </summary>
        OpenParen,
        /// <summary>
        /// The close paren
        /// </summary>
        CloseParen,
        /// <summary>
        /// The open curly paren
        /// </summary>
        OpenCurlyParen,
        /// <summary>
        /// The close curly paren
        /// </summary>
        CloseCurlyParen,
        /// <summary>
        /// The asterisk
        /// </summary>
        Asterisk,
        /// <summary>
        /// The plus
        /// </summary>
        Plus,
        /// <summary>
        /// The comma
        /// </summary>
        Comma,
        /// <summary>
        /// The minus
        /// </summary>
        Minus,
        /// <summary>
        /// The dot
        /// </summary>
        Dot,
        /// <summary>
        /// The slash
        /// </summary>
        Slash,
        /// <summary>
        /// The colon
        /// </summary>
        Colon,
        /// <summary>
        /// The less than
        /// </summary>
        LessThan,
        /// <summary>
        /// The equal
        /// </summary>
        Equal,
        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan,
        /// <summary>
        /// The question
        /// </summary>
        Question,
        /// <summary>
        /// The open bracket
        /// </summary>
        OpenBracket,
        /// <summary>
        /// The close bracket
        /// </summary>
        CloseBracket,
        /// <summary>
        /// The bar
        /// </summary>
        Bar,
        /// <summary>
        /// The exclamation equal
        /// </summary>
        ExclamationEqual,
        /// <summary>
        /// The double amphersand
        /// </summary>
        DoubleAmphersand,
        /// <summary>
        /// The less than equal
        /// </summary>
        LessThanEqual,
        /// <summary>
        /// The less greater
        /// </summary>
        LessGreater,
        /// <summary>
        /// The double equal
        /// </summary>
        DoubleEqual,
        /// <summary>
        /// The greater than equal
        /// </summary>
        GreaterThanEqual,
        /// <summary>
        /// The double bar
        /// </summary>
        DoubleBar,
        /// <summary>
        /// The double greater than
        /// </summary>
        DoubleGreaterThan,
        /// <summary>
        /// The double less than
        /// </summary>
        DoubleLessThan,
        /// <summary>
        /// The null coalescing
        /// </summary>
        NullCoalescing,
        /// <summary>
        /// The lambda
        /// </summary>
        Lambda,
        /// <summary>
        /// The null propagation
        /// </summary>
        NullPropagation
    }
}
