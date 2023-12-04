internal class Program
{

    public static readonly string Cards = "J23456789TQKA";

    private static void Main(string[] args)
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


        new Program().Run(args[0]);
    }

    public void Run(string filePath)
    {
        Console.WriteLine();
        var entries = this.ParseEntries(filePath).OrderBy(e => e.HandType).ThenBy(e => e.Hand, new HandComparer()).ToArray();
        Console.WriteLine();
        Console.WriteLine(string.Join(Environment.NewLine, entries));
        decimal winnings = entries.Select((e, i) => (i + 1) * (decimal)e.Bid).Sum();
        Console.WriteLine();
        Console.WriteLine("Target: 5905");
        Console.WriteLine(winnings);
    }

    IEnumerable<Entry> ParseEntries(string filePath)
    {
        using var streamReader = new StreamReader(filePath);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine().AsSpan();
            string hand = line[..5].ToString();
            yield return new(hand, ushort.Parse(line[6..]), FindKind(hand));
        }
    }

    HandType FindKind(string hand)
    {
        Dictionary<char, int> matches = new();

        var wilds = hand.Count(c => c == 'J');
        var sansWilds = hand.Replace('J', ' ');
        foreach (var card in Cards)
        {
            matches[card] = sansWilds.Count(c => c == card);
        }

        var sortedMatches = matches.OrderByDescending(kvp => kvp.Value).ToArray();
        Console.WriteLine("{0}: {1} + {2}", hand, string.Join("", sortedMatches.Select(kvp => kvp.Value.ToString())), wilds);

        return (sortedMatches[0].Value + wilds) switch
        {
            5 => HandType.FiveKind,
            4 => HandType.FourKind,
            3 when sortedMatches[1].Value is 2 => HandType.FullHouse,
            3 when sortedMatches[1].Value is 1 => HandType.ThreeKind,
            2 when sortedMatches[1].Value is 2 => HandType.TwoPair,
            2 when sortedMatches[1].Value is 1 => HandType.OnePair,
            1 => HandType.HighCard,
            int val => throw new ArgumentOutOfRangeException(nameof(val), val.ToString())
        };
    }
}

class HandComparer : Comparer<String>
{
    public override int Compare(string? x, string? y)
    {
        if (x is null || y is null)
        {
            throw new ArgumentOutOfRangeException(x is null ? nameof(x) : nameof(y));
        }

        for (int i = 0; i < x.Length; i++)
        {
            int indexA = Program.Cards.IndexOf(x[i]);
            int indexB = Program.Cards.IndexOf(y[i]);
            if (indexA is -1 || indexB is -1)
            {
                throw new InvalidDataException(indexA is -1 ? nameof(indexA) : nameof(indexB));
            }
            if (indexA == indexB)
            {
                continue;
            }
            return indexA.CompareTo(indexB);
        }
        return 0;
    }
}

enum HandType : byte
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeKind,
    FullHouse,
    FourKind,
    FiveKind
}

record struct Entry(string Hand, ushort Bid, HandType HandType);
