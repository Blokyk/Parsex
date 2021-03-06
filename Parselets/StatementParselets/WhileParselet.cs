public sealed class WhileParslet : IStatementParslet<WhileNode>
{
    public WhileNode Parse(StatementParser parser, Token whileToken) {
        if (!(whileToken is ComplexToken whileKeyword && whileKeyword == "while")) {
            throw Logger.Fatal(new InvalidCallException(whileToken.Location));
        }

        var isValid = true;

        var conditionNode = parser.ExpressionParser.ConsumeValue();

        if (!(conditionNode is ParenthesizedValueNode condition)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: conditionNode,
                context: "as a while-loop condition",
                expected: "a condition between parenthesis (e.g. `(a == b)`)"
            ));

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                condition = new ParenthesizedValueNode(
                    tuple.Count == 0 ? ValueNode.NULL : tuple.Values[0],
                    tuple.OpeningToken,
                    tuple.ClosingToken
                );
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL);
            }
        }

        var body = parser.ConsumeSimpleBlock();

        return new WhileNode(condition, body, whileKeyword, isValid);
    }
}