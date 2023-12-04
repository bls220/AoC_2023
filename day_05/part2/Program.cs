using System.Text.RegularExpressions;
using part;

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

// Get Seeds
var match = Regex.Match(lines[0], @"(\s(?<Start>\d+)\s(?<Length>\d+))+", RegexOptions.ExplicitCapture);
var seeds = match.Groups["Start"].Captures.Select(c => long.Parse(c.Value)).Zip(match.Groups["Length"].Captures.Select(c => long.Parse(c.Value)))
.SelectMany(s => GetSeedsInRange(s.First, s.Second));
//Console.WriteLine("seeds: " + string.Join(" ", seeds));

// Get Maps
var maps = new List<List<MapEntry>>();
foreach (var line in lines.Skip(2))
{
    if (line.Length == 0)
    {
        continue;
    }
    if (!char.IsAsciiDigit(line[0]))
    {
        maps.Add(new());
        continue;
    }
    var me = line.Parse<MapEntry>();
    maps.Last().Add(me);
}

var tasks = seeds.Chunk(1000).Select(
        s => Task.Run(() => s.Select(x => LookupSeedLocation(x)), cts.Token)
);

await Task.WhenAll<IEnumerable<long>>(tasks);
long minLocation = tasks.SelectMany(t => t.Result).Min();

//foreach (long seed in seeds)
//{
//cts.Token.ThrowIfCancellationRequested();
//long location = LookupSeedLocation(seed);
//if (location < minLocation)
//{
//minLocation = location;
//}
////Console.WriteLine("{0}: {1}", seed, location);
//}
Console.WriteLine(minLocation);
return;

IEnumerable<long> GetSeedsInRange(long start, long length)
{
    for (long seed = start; seed < start + length; seed++)
    {
        yield return seed;
    }
}

long LookupSeedLocation(long seed)
{
    long dest = seed;
    foreach (var map in maps)
    {
        foreach (var me in map)
        {
            if (me.TryFindDestination(dest, out long newDest))
            {
                dest = newDest;
                break;
            }
        }
    }
    return dest;
}
