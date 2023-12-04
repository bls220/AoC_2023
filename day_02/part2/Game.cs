using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace part;

partial class Game : IParsable<Game>
{
    [GeneratedRegex(@"Game (?<GameID>\d+)((:|;) (?<Hand>.*?(?=;|$)))+", RegexOptions.ExplicitCapture)]
    private static partial Regex GameRegex();

    private List<Hand> _hands = new();

    public int ID { get; }
    public ReadOnlyCollection<Hand> Hands => this._hands.AsReadOnly();

    public Game(int id)
    {
        ID = id;
    }

    public void AddHand(Hand hand)
    {
        this._hands.Add(hand);
    }

    public static Game Parse(string input, IFormatProvider? provider)
    {
        var match = GameRegex().Match(input);
        if (!match.Success)
        {
            throw new FormatException($"Input was not in expected format for a {nameof(Game)}");
        }

        if (!match.Groups.ContainsKey("GameID") || !int.TryParse(match.Groups["GameID"].Value, out int id))
        {
            throw new FormatException("Input did not contain valid game ID");
        }

        Game game = new(id);

        if (match.Groups.ContainsKey("Hand"))
        {
            var handGroup = match.Groups["Hand"];
            foreach (var handCapture in handGroup.Captures.ToArray())
            {
                var hand = handCapture.Value.Parse<Hand>();
                game.AddHand(hand);
            }
        }

        return game;
    }

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, [MaybeNullWhen(false)] out Game result)
    {
        result = null;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        try
        {
            result = Game.Parse(input, provider);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
