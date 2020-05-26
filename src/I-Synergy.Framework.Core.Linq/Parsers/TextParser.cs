using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ISynergy.Framework.Core.Linq.Constants;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Tokenizer;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// Class TextParser.
    /// </summary>
    internal class TextParser
    {
        /// <summary>
        /// The default number decimal separator
        /// </summary>
        private const char DefaultNumberDecimalSeparator = '.';

        /// <summary>
        /// The escape characters
        /// </summary>
        private static readonly char[] EscapeCharacters = new[] { '\\', 'a', 'b', 'f', 'n', 'r', 't', 'v' };

        // These aliases are supposed to simply the where clause and make it more human readable
        /// <summary>
        /// The predefined operator aliases
        /// </summary>
        private static readonly Dictionary<string, TokenId> PredefinedOperatorAliases = new Dictionary<string, TokenId>(StringComparer.OrdinalIgnoreCase)
        {
            { "eq", TokenId.Equal },
            { "equal", TokenId.Equal },
            { "ne", TokenId.ExclamationEqual },
            { "notequal", TokenId.ExclamationEqual },
            { "neq", TokenId.ExclamationEqual },
            { "lt", TokenId.LessThan },
            { "LessThan", TokenId.LessThan },
            { "le", TokenId.LessThanEqual },
            { "LessThanEqual", TokenId.LessThanEqual },
            { "gt", TokenId.GreaterThan },
            { "GreaterThan", TokenId.GreaterThan },
            { "ge", TokenId.GreaterThanEqual },
            { "GreaterThanEqual", TokenId.GreaterThanEqual },
            { "and", TokenId.DoubleAmphersand },
            { "AndAlso", TokenId.DoubleAmphersand },
            { "or", TokenId.DoubleBar },
            { "OrElse", TokenId.DoubleBar },
            { "not", TokenId.Exclamation },
            { "mod", TokenId.Percent }
        };

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly ParsingConfig _config;
        /// <summary>
        /// The number decimal separator
        /// </summary>
        private readonly char _numberDecimalSeparator;
        /// <summary>
        /// The text
        /// </summary>
        private readonly string _text;
        /// <summary>
        /// The text length
        /// </summary>
        private readonly int _textLen;

        /// <summary>
        /// The text position
        /// </summary>
        private int _textPos;
        /// <summary>
        /// The ch
        /// </summary>
        private char _ch;
        /// <summary>
        /// The current token
        /// </summary>
        public Token CurrentToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextParser"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="text">The text.</param>
        public TextParser(ParsingConfig config, string text)
        {
            _config = config;
            _numberDecimalSeparator = config.NumberParseCulture?.NumberFormat.NumberDecimalSeparator[0] ?? DefaultNumberDecimalSeparator;

            _text = text;
            _textLen = _text.Length;

            SetTextPos(0);
            NextToken();
        }

        /// <summary>
        /// Sets the text position.
        /// </summary>
        /// <param name="pos">The position.</param>
        private void SetTextPos(int pos)
        {
            _textPos = pos;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        /// <summary>
        /// Nexts the character.
        /// </summary>
        private void NextChar()
        {
            if (_textPos < _textLen)
            {
                _textPos++;
            }
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        /// <summary>
        /// Peeks the next character.
        /// </summary>
        /// <returns>System.Char.</returns>
        public char PeekNextChar()
        {
            if (_textPos + 1 < _textLen)
            {
                return _text[_textPos + 1];
            }

            return '\0';
        }

        /// <summary>
        /// Nexts the token.
        /// </summary>
        public void NextToken()
        {
            while (char.IsWhiteSpace(_ch))
            {
                NextChar();
            }

            var tokenId = TokenId.Unknown;
            var tokenPos = _textPos;

            switch (_ch)
            {
                case '!':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        tokenId = TokenId.Exclamation;
                    }
                    break;

                case '%':
                    NextChar();
                    tokenId = TokenId.Percent;
                    break;

                case '&':
                    NextChar();
                    if (_ch == '&')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        tokenId = TokenId.Amphersand;
                    }
                    break;

                case '(':
                    NextChar();
                    tokenId = TokenId.OpenParen;
                    break;

                case ')':
                    NextChar();
                    tokenId = TokenId.CloseParen;
                    break;

                case '{':
                    NextChar();
                    tokenId = TokenId.OpenCurlyParen;
                    break;

                case '}':
                    NextChar();
                    tokenId = TokenId.CloseCurlyParen;
                    break;

                case '*':
                    NextChar();
                    tokenId = TokenId.Asterisk;
                    break;

                case '+':
                    NextChar();
                    tokenId = TokenId.Plus;
                    break;

                case ',':
                    NextChar();
                    tokenId = TokenId.Comma;
                    break;

                case '-':
                    NextChar();
                    tokenId = TokenId.Minus;
                    break;

                case '.':
                    NextChar();
                    tokenId = TokenId.Dot;
                    break;

                case '/':
                    NextChar();
                    tokenId = TokenId.Slash;
                    break;

                case ':':
                    NextChar();
                    tokenId = TokenId.Colon;
                    break;

                case '<':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.LessThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        tokenId = TokenId.LessGreater;
                    }
                    else if (_ch == '<')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleLessThan;
                    }
                    else
                    {
                        tokenId = TokenId.LessThan;
                    }
                    break;

                case '=':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        tokenId = TokenId.Lambda;
                    }
                    else
                    {
                        tokenId = TokenId.Equal;
                    }
                    break;

                case '>':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        tokenId = TokenId.GreaterThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleGreaterThan;
                    }
                    else
                    {
                        tokenId = TokenId.GreaterThan;
                    }
                    break;

                case '?':
                    NextChar();
                    if (_ch == '?')
                    {
                        NextChar();
                        tokenId = TokenId.NullCoalescing;
                    }
                    else if (_ch == '.')
                    {
                        NextChar();
                        tokenId = TokenId.NullPropagation;
                    }
                    else
                    {
                        tokenId = TokenId.Question;
                    }
                    break;

                case '[':
                    NextChar();
                    tokenId = TokenId.OpenBracket;
                    break;

                case ']':
                    NextChar();
                    tokenId = TokenId.CloseBracket;
                    break;

                case '|':
                    NextChar();
                    if (_ch == '|')
                    {
                        NextChar();
                        tokenId = TokenId.DoubleBar;
                    }
                    else
                    {
                        tokenId = TokenId.Bar;
                    }
                    break;

                case '"':
                case '\'':
                    var balanced = false;
                    var quote = _ch;

                    NextChar();

                    while (_textPos < _textLen && _ch != quote)
                    {
                        var next = PeekNextChar();

                        if (_ch == '\\')
                        {
                            if (EscapeCharacters.Contains(next))
                            {
                                NextChar();
                            }

                            if (next == '"')
                            {
                                NextChar();
                            }
                        }

                        NextChar();

                        if (_ch == quote)
                        {
                            balanced = !balanced;
                        }
                    }

                    if (_textPos == _textLen && !balanced)
                    {
                        throw ParseError(_textPos, Res.UnterminatedStringLiteral);
                    }

                    NextChar();

                    tokenId = TokenId.StringLiteral;
                    break;

                default:
                    if (char.IsLetter(_ch) || _ch == '@' || _ch == '_' || _ch == '$' || _ch == '^' || _ch == '~')
                    {
                        do
                        {
                            NextChar();
                        } while (char.IsLetterOrDigit(_ch) || _ch == '_');
                        tokenId = TokenId.Identifier;
                        break;
                    }

                    if (char.IsDigit(_ch))
                    {
                        tokenId = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (char.IsDigit(_ch));

                        var hexInteger = false;
                        if (_ch == 'X' || _ch == 'x')
                        {
                            NextChar();
                            ValidateHexChar();
                            do
                            {
                                NextChar();
                            } while (IsHexChar(_ch));

                            hexInteger = true;
                        }

                        if (_ch == 'U' || _ch == 'L')
                        {
                            NextChar();
                            if (_ch == 'L')
                            {
                                if (_text[_textPos - 1] == 'U') NextChar();
                                else throw ParseError(_textPos, Res.InvalidIntegerQualifier, _text.Substring(_textPos - 1, 2));
                            }
                            ValidateExpression();
                            break;
                        }

                        if (hexInteger)
                        {
                            break;
                        }

                        if (_ch == _numberDecimalSeparator)
                        {
                            tokenId = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'E' || _ch == 'e')
                        {
                            tokenId = TokenId.RealLiteral;
                            NextChar();
                            if (_ch == '+' || _ch == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(_ch));
                        }

                        if (_ch == 'F' || _ch == 'f') NextChar();
                        if (_ch == 'D' || _ch == 'd') NextChar();
                        if (_ch == 'M' || _ch == 'm') NextChar();
                        break;
                    }

                    if (_textPos == _textLen)
                    {
                        tokenId = TokenId.End;
                        break;
                    }

                    throw ParseError(_textPos, Res.InvalidCharacter, _ch);
            }

            CurrentToken.Pos = tokenPos;
            CurrentToken.Text = _text.Substring(tokenPos, _textPos - tokenPos);
            CurrentToken.OriginalId = tokenId;
            CurrentToken.Id = GetAliasedTokenId(tokenId, CurrentToken.Text);
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="errorMessage">The error message.</param>
        public void ValidateToken(TokenId t, string errorMessage)
        {
            if (CurrentToken.Id != t)
            {
                throw ParseError(errorMessage);
            }
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="t">The t.</param>
        public void ValidateToken(TokenId t)
        {
            if (CurrentToken.Id != t)
            {
                throw ParseError(Res.SyntaxError);
            }
        }

        /// <summary>
        /// Validates the expression.
        /// </summary>
        private void ValidateExpression()
        {
            if (char.IsLetterOrDigit(_ch))
            {
                throw ParseError(_textPos, Res.ExpressionExpected);
            }
        }

        /// <summary>
        /// Validates the digit.
        /// </summary>
        private void ValidateDigit()
        {
            if (!char.IsDigit(_ch))
            {
                throw ParseError(_textPos, Res.DigitExpected);
            }
        }

        /// <summary>
        /// Validates the hexadecimal character.
        /// </summary>
        private void ValidateHexChar()
        {
            if (!IsHexChar(_ch))
            {
                throw ParseError(_textPos, Res.HexCharExpected);
            }
        }

        /// <summary>
        /// Parses the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Exception.</returns>
        private Exception ParseError(string format, params object[] args)
        {
            return ParseError(CurrentToken.Pos, format, args);
        }

        /// <summary>
        /// Parses the error.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Exception.</returns>
        private static Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }

        /// <summary>
        /// Gets the aliased token identifier.
        /// </summary>
        /// <param name="tokenId">The token identifier.</param>
        /// <param name="alias">The alias.</param>
        /// <returns>TokenId.</returns>
        private static TokenId GetAliasedTokenId(TokenId tokenId, string alias)
        {
            return tokenId == TokenId.Identifier && PredefinedOperatorAliases.TryGetValue(alias, out var id) ? id : tokenId;
        }

        /// <summary>
        /// Determines whether [is hexadecimal character] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is hexadecimal character] [the specified c]; otherwise, <c>false</c>.</returns>
        private static bool IsHexChar(char c)
        {
            if (char.IsDigit(c))
            {
                return true;
            }

            if (c <= '\x007f')
            {
                c |= (char)0x20;
                return c >= 'a' && c <= 'f';
            }

            return false;
        }
    }
}
