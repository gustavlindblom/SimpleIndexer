class Document
{
    private readonly Dictionary<string, int> _termFrequencyMap;
    private readonly int _count;
    private readonly string _name;

    public string Name => _name;
    public int Count => _count;
    public Dictionary<string, int> TermFrequencyMap => _termFrequencyMap;

    public Document(string name, Dictionary<string, int> termFrequencyMap)
    {
        _termFrequencyMap = termFrequencyMap;
        _count = termFrequencyMap.Keys.Count;
        _name = name;
    }
}

class Index
{
    private readonly Dictionary<string, Document> _documents;
    private readonly Dictionary<string, int> _documentFrequencyMap;

    public Index()
    {
        _documents = new Dictionary<string, Document>();
        _documentFrequencyMap = new Dictionary<string, int>();
    }

    public void InsertDocument(string name, string contents)
    {
        var tokens = new UnsafeLexer(contents).CollectTokens();
        var termFrequencyMap = new Dictionary<string, int>();
        foreach (var token in tokens)
        {
            if (!termFrequencyMap.TryAdd(token, 1))
            {
                termFrequencyMap[token] += 1;
            }
        }
        _documents.Add(name, new Document(name, termFrequencyMap));
        foreach (string token in termFrequencyMap.Keys)
        {
            if (!_documentFrequencyMap.TryAdd(token, 1))
            {
                _documentFrequencyMap[token] += 1;
            }
        }
    }

    public List<(string documentName, double score)> Search(string query)
    {
        var result = new List<(string documentName, double score)>();

        var tokens = new Lexer(query).CollectTokens();

        foreach ((string name, Document document) in _documents)
        {
            float score = 0f;
            foreach (string token in tokens)
            {
                score += ComputeTermFrequencyScore(token, document) * ComputeInverseDocumentFrequencyScore(token, _documents.Keys.Count, _documentFrequencyMap);
            }

            result.Add((name, score));
        }

        return result.OrderByDescending(s => s.score).ToList();
    }

    private static float ComputeTermFrequencyScore(string term, Document document)
    {
        float termCount = document.Count;
        float termFrequency = document.TermFrequencyMap.TryGetValue(term, out int frequency) ? frequency : 0f;

        return termFrequency / termCount;
    }

    private static float ComputeInverseDocumentFrequencyScore(string term, float documentsCount, Dictionary<string, int> documentFrequencyMap)
    {
        float documentTermFrequency = documentFrequencyMap.TryGetValue(term, out int frequency) ? frequency : 1f;

        return float.Log10(documentsCount / documentTermFrequency);
    }
}