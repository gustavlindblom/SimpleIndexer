using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;

using var httpClient = new HttpClient();

string[] urls = new string[]
{
    "https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-7.0",
    "https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching",
    "https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references",
    "https://learn.microsoft.com/en-us/dotnet/csharp/nullable-migration-strategies",
    "https://learn.microsoft.com/en-us/dotnet/csharp/methods",
};

Index index = new Index();
var instant = Stopwatch.StartNew();
foreach (var url in urls)
{
    Console.WriteLine($"Fetching html of {url}");
    var htmlDocument = await GetHtmlContents(url, httpClient);
    Console.WriteLine($"\tIndexing contents of {url}");
    index.InsertDocument(htmlDocument.Name, htmlDocument.Contents);
}
Console.WriteLine($"Fetching html and collecting tokens took {instant.ElapsedMilliseconds}ms");
Console.WriteLine("----------------------------------\n");

while (true)
{
    Console.WriteLine("Type query (q to quit):");
    string query = Console.ReadLine() ?? string.Empty;
    if (query == "q")
        return;

    instant = Stopwatch.StartNew();
    var results = index.Search($"   {query}   ");
    Console.WriteLine($"Searching for \"{query}\" took {instant.ElapsedMilliseconds}ms\n");
    instant.Stop();
    foreach (var result in results)
    {
        Console.WriteLine(result);
    }
}

static async Task<(string Name, string Contents)> GetHtmlContents(string url, HttpClient client)
{
    var response = await client.GetAsync(url);

    if (!response.IsSuccessStatusCode)
        return (url, "error");

    StringBuilder sb = new StringBuilder();
    var document = new HtmlDocument();
    document.Load(response.Content.ReadAsStream());

    string name = document.DocumentNode.Element("html").Element("head").Element("title").InnerText;

    var body = document.DocumentNode.SelectSingleNode("//body");

    foreach (var node in body.DescendantsAndSelf())
    {
        sb.AppendLine(node.InnerText);
    }

    return (name, sb.ToString());
}