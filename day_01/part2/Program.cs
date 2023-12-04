Console.Clear();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Canceling...");
    cts.Cancel();
    e.Cancel = true;
};

Dictionary<string, int> numNames = new()
{
    ["one"] = 1,
    ["two"] = 2,
    ["three"] = 3,
    ["four"] = 4,
    ["five"] = 5,
    ["six"] = 6,
    ["seven"] = 7,
    ["eight"] = 8,
    ["nine"] = 9,
};

Console.WriteLine($"""args: ['{String.Join("', '", args)}']""");
var lines = await File.ReadAllLinesAsync(args[0], cts.Token);

int sum = 0;
foreach (var rawLine in lines)
{
    sum += ExtractNumberFromLine(rawLine);
}
Console.WriteLine(sum);

int FindNumber(ReadOnlySpan<Char> span)
{
    if (Char.IsDigit(span[0]))
    {
        return int.Parse(span[0].ToString());
    }
    foreach (var (word, value) in numNames)
    {
        if (span.StartsWith(word))
        {
            return value;
        }
    }
    return 0;
}

int ExtractNumberFromLine(string line)
{
    int num = 0;
    var fullSpan = line.AsSpan();
    var span = fullSpan;
    // Find first number
    while (span.Length > 0)
    {
        int first = FindNumber(span);
        if (first != 0) {
            num = 10 * first;
            break;
        }
        span = span[1..];
    }

    // Find second number
    for (int i = 1; i <= fullSpan.Length; i++)
    {
        span = fullSpan[^i..];
        int second = FindNumber(span);
        if (second != 0) {
            num += second;
            break;
        }
    }
    return num;
}
