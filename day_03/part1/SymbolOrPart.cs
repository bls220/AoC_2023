using RBush;

namespace part;

record SymbolOrPart : ISpatialData
{
    private readonly Envelope _envelope;
    private readonly string _value;

    public ref readonly Envelope Envelope => ref _envelope;

    public ref readonly String Value => ref _value;

    public bool IsSymbol => !Char.IsAsciiDigit(this.Value[0]);

    public SymbolOrPart(int x, int y, String val) {
        this._envelope = new Envelope(x, y, x + val.Length - 1, y);
        this._value = val;
    }
}

