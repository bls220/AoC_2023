using System.Buffers;
using part;
using RBush;

bool isWatch = Environment.GetEnvironmentVariable("DOTNET_WATCH") == "1";
if (isWatch)
{
    Console.Clear();
}

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Canceling...");
    cts.Cancel();
    e.Cancel = true;
};

Console.WriteLine($"""args: ['{String.Join("', '", args)}']""");
var lines = await File.ReadAllLinesAsync(args[0], cts.Token);


RBush<SymbolOrPart> tree = ParseSchematic(lines);

var potentialGears = tree.Search().Where(i => i.Value == "*").ToArray();
int sum = 0;
foreach (var gear in potentialGears)
{
    var found = tree.Search(new Envelope(
                gear.Envelope.MinX-1,
                gear.Envelope.MinY-1,
                gear.Envelope.MaxX+1,
                gear.Envelope.MaxY+1
                )).Where(i => !i.IsSymbol);
    if (found.Count() == 2) {
        sum += found.Select(g => int.Parse(g.Value)).Aggregate((a,b) => a * b);
    }
}

Console.WriteLine(sum);


static RBush<SymbolOrPart> ParseSchematic(string[] lines)
{
    var digitSearch = SearchValues.Create("1234567890");
    var dotSearch = SearchValues.Create(".");

    var tree = new RBush<SymbolOrPart>();

    List<SymbolOrPart> rects = new();

    for (int y = 0; y < lines.Length; y++)
    {
        var line = lines[y].AsSpan();
        int x = 0;
        while (line.Length > 0)
        {
            // Find first non-dot
            int toSkip = line.IndexOfAnyExcept(dotSearch);
            if (toSkip == -1)
            {
                break;
            }
            line = line[toSkip..];
            x += toSkip;

            // Are we getting a number or symbol?
            bool isNum = Char.IsAsciiDigit(line[0]);
            // Find end of number. Symbols are only single chars
            int toKeep = isNum ? line.IndexOfAnyExcept(digitSearch) : 1;
            //Console.WriteLine($"Skip {toSkip}, Keep {toKeep}");
            if (toKeep == -1)
            {
                toKeep = line.Length;
            }

            var rect = new SymbolOrPart(x, y, line[..toKeep].ToString());
            rects.Add(rect);

            // Prepare for next loop
            line = line[toKeep..];
            x += toKeep;
        }
    }

    tree.BulkLoad(rects);
    return tree;
}
