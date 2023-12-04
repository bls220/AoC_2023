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

var cards = lines.Select(line => line.Parse<Card>()).ToArray();

int[] copies = new int[cards.Length];
Array.Fill(copies, 1);

foreach (var card in cards)
{
    int copiesWon = card.WinningNumbers.Count();
    for (int i = 1; i <= copiesWon; i++)
    {
        copies[card.ID - 1 + i] += copies[card.ID - 1];
    }
}

Console.WriteLine(string.Join(Environment.NewLine, copies.Select((copy, i) => $"[{i}]: {copy}")));

Console.WriteLine(copies.Sum());
