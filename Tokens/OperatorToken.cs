[System.Diagnostics.DebuggerDisplay("{Location} {Precedence}({(int)Precedence}) : {Representation}")]
public class OperatorToken : Token
{
    public new static readonly OperatorToken NULL = new OperatorToken("", Precedence.Comma, false, LocationRange.NULL, false);
    public Precedence Precedence { get; protected set; }

    public bool IsLeftAssociative { get; protected set; }


    public OperatorToken(string representation, Precedence precedence, bool isLeftAssociative, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.@operator, location, isValid, leading, trailing)
    {
        Precedence = precedence;
        IsLeftAssociative = isLeftAssociative;
    }

    public OperatorToken(string representation, Precedence precedence, string associativity, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation, precedence, associativity == "left", location, isValid, leading, trailing)
    { }

    public OperatorToken(char representation, Precedence precedence, string associativity, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), precedence, associativity == "left", location, isValid, leading, trailing)
    { }

    public OperatorToken(char representation, Precedence precedence, bool isLeftAssociative, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), precedence, isLeftAssociative, location, isValid, leading, trailing)
    { }


    public OperatorToken(Token token, Precedence precedence, string associativity, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(token, precedence, associativity == "left", location, isValid, leading, trailing)
    {
        Kind = TokenKind.@operator;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}