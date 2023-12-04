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

var cards = lines.Select(line => line.Parse<Card>());

int sum = 0;
foreach (var card in cards)
{
    var winning = card.WinningNumbers.Intersect(card.Numbers).ToArray();
    int points = (int)Math.Pow(2,winning.Length - 1);
    sum += points;
    Console.WriteLine("Card {0}: {1} = {2}", card.ID, string.Join(" ", winning), points);
}
Console.WriteLine(sum);
