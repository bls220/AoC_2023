using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace part;

public partial record Hand(int Red, int Green, int Blue) : IParsable<Hand>
{
    [GeneratedRegex(@"(((?<red>\d+) red|(?<green>\d+) green|(?<blue>\d+) blue)(, |$))+", RegexOptions.ExplicitCapture)]
    private static partial Regex HandRegex();

    public static Hand Parse(string input, IFormatProvider? provider)
    {
        var match = HandRegex().Match(input);
        if (!match.Success)
        {
            throw new FormatException($"Input was not in expected format for a {nameof(Hand)}");
        }

        int r = 0, g = 0, b = 0;

        if (match.Groups.ContainsKey("red"))
        {
            int.TryParse(match.Groups["red"].Value, out r);
        }

        if (match.Groups.ContainsKey("green"))
        {
            int.TryParse(match.Groups["green"].Value, out g);
        }

        if (match.Groups.ContainsKey("blue"))
        {
            int.TryParse(match.Groups["blue"].Value, out b);
        }

        return new Hand(r, g, b);
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out Hand result)
    {
        result = null;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        try
        {
            result = Hand.Parse(input, provider);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

