using System.Text.RegularExpressions;
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

var filteredGames = games
    .Where(game =>
        game.Hands.All(hand => 
            hand.Red <= maxRed && hand.Green <= maxGreen && hand.Blue <= maxBlue
        )
    );

int sum = 0;
foreach (var game in filteredGames){
    sum += game.ID;
    Console.WriteLine(game.ID);
}

Console.WriteLine(sum);


