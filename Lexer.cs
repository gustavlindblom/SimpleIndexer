public class Lexer
{
    private string _content;

    public Lexer(string content)
    {
        _content = content;
    }

    public List<string> CollectTokens()
    {
        List<string> tokens = new List<string>();
        while (_content.Length > 0)
        {
            tokens.Add(GetNextToken());
        }

        return tokens;
    }

    string GetNextToken()
    {
        if (_content.Length == 0)
            return string.Empty;

        TrimLeft();

        if (_content.Length == 0)
            return string.Empty;

        if (char.IsDigit(_content[0]))
            return ChopWhile(char.IsDigit);

        if (char.IsLetter(_content[0]))
            return ChopWhile(char.IsLetterOrDigit);


        return Chop(1);
    }

    void TrimLeft()
    {
        int n = 0;

        while (n < _content.Length && Char.IsWhiteSpace(_content[n]))
        {
            n += 1;
        }

        _content = _content[n..];
    }

    string ChopWhile(Func<char, bool> predicate)
    {
        int n = 0;
        while (n < _content.Length && predicate(_content[n]))
        {
            n++;
        }

        return Chop(n);
    }

    string Chop(int length)
    {
        string result = _content.Substring(0, length);
        _content = _content.Substring(length);
        return result;
    }
}
