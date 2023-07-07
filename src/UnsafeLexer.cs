using System.Collections.Immutable;
using System.Runtime.CompilerServices;

ref struct UnsafeLexer
{
    private ImmutableArray<char> _content;
    private ReadOnlySpan<char> _span;

    private int _tokenStart;
    private int _tokenEnd;

    public UnsafeLexer(string content)
    {
        _content = Unsafe.As<string, ImmutableArray<char>>(ref content);
        _span = _content.AsSpan();
        _tokenStart = 0;
        _tokenEnd = 0;
    }

    public List<string> CollectTokens()
    {
        List<string> tokens = new List<string>();
        while (_tokenStart < _content.Length)
        {
            tokens.Add(GetNextToken());
        }

        return tokens;
    }

    void TrimLeft()
    {
        int n = _tokenStart;

        while (_content.Length > 0 && Char.IsWhiteSpace(_content[n]))
        {
            n += 1;
        }

        _tokenStart = n;
    }

    string Chop()
    {
        var slice = _span.Slice(_tokenStart, _tokenEnd - _tokenStart);

        _tokenStart = _tokenEnd + 1;

        return new String(slice);
    }

    string ChopWhile(Func<char, bool> predicate)
    {
        int n = _tokenStart;
        while (n < _content.Length && predicate(_content[n]))
        {
            n++;
        }
        _tokenEnd = n;

        return Chop();
    }

    string GetNextToken()
    {
        if (_tokenStart >= _content.Length)
            return string.Empty;

        TrimLeft();

        if (_content.Length == 0)
            return string.Empty;

        if (char.IsDigit(_content[_tokenStart]))
            return ChopWhile(char.IsDigit);

        if (char.IsLetter(_content[_tokenStart]))
            return ChopWhile(char.IsLetterOrDigit);

        _tokenEnd = _tokenStart + 1;

        return Chop();
    }
}
