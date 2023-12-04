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

foreach (var line in lines)
{

}
