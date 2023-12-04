using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace part;

partial class MapEntry : IParsable<MapEntry>
{
    [GeneratedRegex(@"(?<Dest>\d+) (?<Src>\d+) (?<Len>\d+)", RegexOptions.ExplicitCapture)]
    private static partial Regex formatRegex();

    public (long start, long end) SourceRange { get; init; }
    public (long start, long end) DestinationRange { get; init; }

    private MapEntry(long destinationStart, long sourceStart, long length)
    {
        this.SourceRange = new (sourceStart, sourceStart + length);
        this.DestinationRange = new (destinationStart, destinationStart + length);
    }

    public bool TryFindDestination(long source, out long destination)
    {
        if (source > this.SourceRange.end || source < this.SourceRange.start)
        {
            // Not in range
            destination = 0;
            return false;
        }
        destination = (source - this.SourceRange.start) + this.DestinationRange.start;
        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("Dest = {0}, Src = {1}", this.DestinationRange, this.SourceRange);
        return sb.ToString();
    }

    public static MapEntry Parse(string input, IFormatProvider? provider)
    {
        var match = formatRegex().Match(input);
        if (!match.Success)
        {
            throw new FormatException($"{nameof(MapEntry)} was not in the expected format: {input}");
        }
        long dest = long.Parse(match.Groups["Dest"].Value);
        long src = long.Parse(match.Groups["Src"].Value);
        long length = long.Parse(match.Groups["Len"].Value);

        return new MapEntry(dest, src, length);
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out MapEntry result)
    {
        result = null;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        try
        {
            result = MapEntry.Parse(input, provider);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

