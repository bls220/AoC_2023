using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace part;

partial class Card : IParsable<Card>
{
    [GeneratedRegex(@"Card\s+(?<ID>\d+):(((\s+)(?<Win>\d+))+) \|(((\s+)(?<Num>\d+))+)", RegexOptions.ExplicitCapture)]
    private static partial Regex CardRegex();

    public int ID { get; }
    public required ReadOnlyCollection<int> TargetNumbers { get; init; }
    public required ReadOnlyCollection<int> Numbers { get; init; }

    public IEnumerable<int> WinningNumbers => this.TargetNumbers.Intersect(this.Numbers);

    public int Score => (int)Math.Pow(2, this.WinningNumbers.Count() - 1);

    private Card(int id)
    {
        ID = id;
    }

    public static Card Parse(string input, IFormatProvider? provider)
    {
        var match = CardRegex().Match(input);
        if (!match.Success)
        {
            throw new FormatException($"Input was not in expected format for a {nameof(Card)}");
        }

        if (!match.Groups.ContainsKey("ID") || !int.TryParse(match.Groups["ID"].Value, out int id))
        {
            throw new FormatException("Input did not contain valid ID");
        }


        List<int> winningNumbers = new();
        if (match.Groups.ContainsKey("Win"))
        {
            var group = match.Groups["Win"];
            var values = group.Captures.Select(c => int.Parse(c.Value));
            winningNumbers.AddRange(values);
        }

        List<int> numbers = new();
        if (match.Groups.ContainsKey("Num"))
        {
            var group = match.Groups["Num"];
            var values = group.Captures.Select(c => int.Parse(c.Value));
            numbers.AddRange(values);
        }

        return new Card(id)
        {
            TargetNumbers = winningNumbers.AsReadOnly(),
            Numbers = numbers.AsReadOnly()
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out Card result)
    {
        result = null;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        try
        {
            result = Card.Parse(input, provider);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
