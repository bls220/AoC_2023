using part;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Canceling...");
    cts.Cancel();
    e.Cancel = true;
};

Console.WriteLine($"""args: ['{String.Join("', '", args)}']""");
var lines = await File.ReadAllLinesAsync(args[0], cts.Token);


int maxRed = 12, maxGreen = 13, maxBlue = 14;

// Load Games
var games = lines.Select(line => line.Parse<Game>());

int sum = 0;
foreach (var game in games)
{
    // Get min Hand of game
    var minHand = new Hand(
            game.Hands.Select(h => h.Red).Max(),
            game.Hands.Select(h => h.Green).Max(),
            game.Hands.Select(h => h.Blue).Max());
    sum += minHand.Power;
    Console.WriteLine($"{game.ID}: {minHand.Power}");
}
Console.WriteLine(sum);


