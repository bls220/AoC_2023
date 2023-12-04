namespace part;

static class ParsableExtension
{
    public static T Parse<T>(this string input, IFormatProvider? provider = null)
        where T : IParsable<T>
        => T.Parse(input, provider);
}

