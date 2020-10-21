[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : StatementNode
{
    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public static new readonly ValueNode NULL = new ValueNode("", new Token('\0', TokenKind.EOF, new Location(-1, -1), false), new Location(-1, -1), false);

    public ValueNode(Token token, LocationRange range, bool isValid = true) : this(token.Representation, token, range, isValid)
    { }

    public ValueNode(string rep, Token token, LocationRange range, bool isValid = true) : base(rep, token, range, isValid)
    { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}