namespace Fonet.Fo.Expr
{
    internal class PropertyTokenizer
    {
        protected const int TOK_EOF = 0;
        protected const int TOK_NCNAME = TOK_EOF + 1;
        protected const int TOK_MULTIPLY = TOK_NCNAME + 1;
        protected const int TOK_LPAR = TOK_MULTIPLY + 1;
        protected const int TOK_RPAR = TOK_LPAR + 1;
        protected const int TOK_LITERAL = TOK_RPAR + 1;
        protected const int TOK_NUMBER = TOK_LITERAL + 1;
        protected const int TOK_FUNCTION_LPAR = TOK_NUMBER + 1;
        protected const int TOK_PLUS = TOK_FUNCTION_LPAR + 1;
        protected const int TOK_MINUS = TOK_PLUS + 1;
        protected const int TOK_MOD = TOK_MINUS + 1;
        protected const int TOK_DIV = TOK_MOD + 1;
        protected const int TOK_NUMERIC = TOK_DIV + 1;
        protected const int TOK_COMMA = TOK_NUMERIC + 1;
        protected const int TOK_PERCENT = TOK_COMMA + 1;
        protected const int TOK_COLORSPEC = TOK_PERCENT + 1;
        protected const int TOK_FLOAT = TOK_COLORSPEC + 1;
        protected const int TOK_INTEGER = TOK_FLOAT + 1;

        protected int currentToken = TOK_EOF;
        protected string currentTokenValue = null;
        protected int currentUnitLength = 0;

        private int _currentTokenStartIndex = 0;
        private readonly string _expr;
        private int _exprIndex = 0;
        private readonly int _exprLength;
        private bool _recognizeOperator = false;

        protected PropertyTokenizer(string s)
        {
            this._expr = s;
            this._exprLength = s.Length;
        }

        protected void Next()
        {
            currentTokenValue = null;
            _currentTokenStartIndex = _exprIndex;
            bool bSawDecimal;
            _recognizeOperator = true;
            while (true)
            {
                if (_exprIndex >= _exprLength)
                {
                    currentToken = TOK_EOF;
                    return;
                }
                char c = _expr[_exprIndex++];
                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        _currentTokenStartIndex = _exprIndex;
                        break;
                    case ',':
                        _recognizeOperator = false;
                        currentToken = TOK_COMMA;
                        return;
                    case '+':
                        _recognizeOperator = false;
                        currentToken = TOK_PLUS;
                        return;
                    case '-':
                        _recognizeOperator = false;
                        currentToken = TOK_MINUS;
                        return;
                    case '(':
                        currentToken = TOK_LPAR;
                        _recognizeOperator = false;
                        return;
                    case ')':
                        currentToken = TOK_RPAR;
                        return;
                    case '"':
                    case '\'':
                        _exprIndex = _expr.IndexOf(c, _exprIndex);
                        if (_exprIndex < 0)
                        {
                            _exprIndex = _currentTokenStartIndex + 1;
                            throw new PropertyException("missing quote");
                        }
                        currentTokenValue = _expr.Substring(
                            _currentTokenStartIndex + 1,
                            _exprIndex++ - (_currentTokenStartIndex + 1));
                        currentToken = TOK_LITERAL;
                        return;
                    case '*':
                        currentToken = TOK_MULTIPLY;
                        return;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        ScanDigits();
                        if (_exprIndex < _exprLength && _expr[_exprIndex] == '.')
                        {
                            _exprIndex++;
                            bSawDecimal = true;
                            if (_exprIndex < _exprLength
                                && IsDigit(_expr[_exprIndex]))
                            {
                                _exprIndex++;
                                ScanDigits();
                            }
                        }
                        else
                        {
                            bSawDecimal = false;
                        }
                        if (_exprIndex < _exprLength && _expr[_exprIndex] == '%')
                        {
                            _exprIndex++;
                            currentToken = TOK_PERCENT;
                        }
                        else
                        {
                            currentUnitLength = _exprIndex;
                            ScanName();
                            currentUnitLength = _exprIndex - currentUnitLength;
                            if (currentUnitLength > 0)
                                currentToken = TOK_NUMERIC;
                            else if (bSawDecimal)
                                currentToken = TOK_FLOAT;
                            else
                                currentToken = TOK_INTEGER;
                        }
                        currentTokenValue = _expr.Substring(_currentTokenStartIndex,
                                                           _exprIndex - _currentTokenStartIndex);
                        return;

                    case '.':
                        if (_exprIndex < _exprLength
                            && IsDigit(_expr[_exprIndex]))
                        {
                            ++_exprIndex;
                            ScanDigits();
                            if (_exprIndex < _exprLength
                                && _expr[_exprIndex] == '%')
                            {
                                _exprIndex++;
                                currentToken = TOK_PERCENT;
                            }
                            else
                            {
                                currentUnitLength = _exprIndex;
                                ScanName();
                                currentUnitLength = _exprIndex - currentUnitLength;
                                currentToken = (currentUnitLength > 0) ? TOK_NUMERIC
                                    : TOK_FLOAT;
                            }
                            currentTokenValue = _expr.Substring(_currentTokenStartIndex,
                                                               _exprIndex - _currentTokenStartIndex);
                            return;
                        }
                        throw new PropertyException("illegal character '.'");

                    case '#':
                        if (_exprIndex < _exprLength && IsHexDigit(_expr[_exprIndex]))
                        {
                            ++_exprIndex;
                            ScanHexDigits();
                            currentToken = TOK_COLORSPEC;
                            currentTokenValue = _expr.Substring(_currentTokenStartIndex,
                                                               _exprIndex - _currentTokenStartIndex);
                            return;
                        }
                        else
                        {
                            throw new PropertyException("illegal character '#'");
                        }

                    default:
                        --_exprIndex;
                        ScanName();
                        if (_exprIndex == _currentTokenStartIndex)
                        {
                            throw new PropertyException("illegal character");
                        }
                        currentTokenValue = _expr.Substring(
                            _currentTokenStartIndex, _exprIndex - _currentTokenStartIndex);
                        if (currentTokenValue.Equals("mod"))
                        {
                            currentToken = TOK_MOD;
                            return;
                        }
                        else if (currentTokenValue.Equals("div"))
                        {
                            currentToken = TOK_DIV;
                            return;
                        }
                        if (FollowingParen())
                        {
                            currentToken = TOK_FUNCTION_LPAR;
                            _recognizeOperator = false;
                        }
                        else
                        {
                            currentToken = TOK_NCNAME;
                            _recognizeOperator = false;
                        }
                        return;
                }
            }
        }

        private void ScanName()
        {
            if (_exprIndex < _exprLength && IsNameStartChar(_expr[_exprIndex]))
            {
                while (++_exprIndex < _exprLength && IsNameChar(_expr[_exprIndex]))
                {
                    // Do nothing
                }
            }
        }

        private void ScanDigits()
        {
            while (_exprIndex < _exprLength && IsDigit(_expr[_exprIndex]))
            {
                _exprIndex++;
            }
        }

        private void ScanHexDigits()
        {
            while (_exprIndex < _exprLength && IsHexDigit(_expr[_exprIndex]))
            {
                _exprIndex++;
            }
        }

        private bool FollowingParen()
        {
            for (int i = _exprIndex; i < _exprLength; i++)
            {
                switch (_expr[i])
                {
                    case '(':
                        _exprIndex = i + 1;
                        return true;
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }

        private const string _nameStartChars = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _nameChars = ".-0123456789";
        private const string _digits = "0123456789";
        private const string _hexchars = _digits + "abcdefABCDEF";

        private static bool IsDigit(char c)
        {
            return _digits.IndexOf(c) >= 0;
        }

        private static bool IsHexDigit(char c)
        {
            return _hexchars.IndexOf(c) >= 0;
        }

        private static bool IsSpace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    return true;
            }
            return false;
        }

        private static bool IsNameStartChar(char c)
        {
            return _nameStartChars.IndexOf(c) >= 0 || c >= 0x80;
        }

        private static bool IsNameChar(char c)
        {
            return _nameStartChars.IndexOf(c) >= 0 || _nameChars.IndexOf(c) >= 0 || c >= 0x80;
        }
    }
}