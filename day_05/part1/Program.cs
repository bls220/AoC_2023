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
var seeds = Regex.Match(lines[0], @"(\s(?<Seed>\d+))+", RegexOptions.ExplicitCapture)
    .Groups["Seed"].Captures
    .Select(c => long.Parse(c.Value));
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

var locations = new List<long>();
foreach (long seed in seeds)
{
    long location = LookupSeedLocation(seed);
    locations.Add(location);
    Console.WriteLine("{0}: {1}", seed, location);
}
Console.WriteLine(locations.Min());

long LookupSeedLocation(long seed)
{
    long dest = seed;
    foreach (var map in maps)
    {
        long? newDest = map
            .Select(me => me.TryFindDestination(dest, out long temp) ? temp : (long?)null)
            .FirstOrDefault(d => d is not null);
        dest = newDest ?? dest;
    }
    return dest;
}
