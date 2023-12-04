
using System.Text.RegularExpressions;

internal partial class Program
{
    [GeneratedRegex(@"(\s+(?<Num>\d+))+", RegexOptions.ExplicitCapture)]
    private static partial Regex _inputRegex();

    record Race(int Time, int Distance);

    private static async Task Main(string[] args)
    {
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

        Console.WriteLine($"""args: ['{string.Join("', '", args)}']""");
        var lines = await File.ReadAllLinesAsync(args[0], cts.Token);

        // Get race data
        var times = _inputRegex().Match(lines[0]).Groups["Num"].Captures.Select(c => int.Parse(c.Value));
        var dist = _inputRegex().Match(lines[1]).Groups["Num"].Captures.Select(c => int.Parse(c.Value));
        Race[] races = times.Zip(dist).Select(d => new Race(d.First, d.Second)).ToArray();

        // (t-h)h
        // -h^2+ht
        // -h^2 + ht - d > 0
        // (t - sqrt(t^2 - 4d))/2 < h < (t + sqrt(t^2 - 4d))/2 and r < t^2/4

        double total = 1;
        foreach (var race in races)
        {
            var discreteRootRange = SolveRace(race);
#if DEBUG
            Console.Write(discreteRootRange);
            Console.Write('\t');
            Console.WriteLine(race);
#endif
            double wins = discreteRootRange.max - discreteRootRange.min + 1;
            total *= wins;
#if DEBUG
            Console.WriteLine(wins);
            Console.WriteLine();
#endif
        }
        Console.WriteLine(total);
    }

    private static (double min, double max) SolveRace(Race race)
    {
        const double epsilion = 0.1;
        double r1 = (epsilion + race.Time - Math.Sqrt(Math.Pow(race.Time, 2) - 4 * race.Distance)) / 2;
        double r2 = (-epsilion + race.Time + Math.Sqrt(Math.Pow(race.Time, 2) - 4 * race.Distance)) / 2;
        return (Math.Ceiling(r1),Math.Floor(r2));
    }
}
