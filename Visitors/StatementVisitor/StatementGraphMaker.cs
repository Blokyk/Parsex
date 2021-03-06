internal class StatementGraphMaker : IStatementVisitor<GraphNode>, IValueVisitor<GraphNode>
{

    protected readonly (string tooltip, string color) Break = ("break keyword", "");

    protected readonly (string tooltip, string color) Continue = ("continue keyword", "");

    protected readonly (string tooltip, string color) Declaration = ("DeclarationNode", "palegreen");
    protected readonly (string tooltip, string color) DeclarationName = ("variable name", "");

    protected readonly (string tooltip, string color) Else = ("else branch", "");

    protected readonly (string tooltip, string color) Foreach = ("foreach loop", "pink");

    protected readonly (string tooltip, string color) For = ("for loop", "");
    protected readonly (string tooltip, string color) ForHeader = ("for-loop header", "deepskyblue");

    protected readonly (string tooltip, string color) FuncDec = ("FunctionDeclarationNode", "indianred");
    protected readonly (string tooltip, string color) FuncDecParameters = ("parameters", "");

    protected readonly (string tooltip, string color) If = ("if statement", "");
    protected readonly (string tooltip, string color) IfCondition = ("if condition", "");

    protected readonly (string tooltip, string color) Print = ("PrintNode", "");

    protected readonly (string tooltip, string color) Return = ("return statement", "brown");

    protected readonly (string tooltip, string color) Statement = ("StatementNode", "black");

    protected readonly (string tooltip, string color) While = ("(do-)while loop", "pink");
    protected readonly (string tooltip, string color) WhileCondition = ("loop condition", "");



    protected readonly (string tooltip, string color) ArrayLiteral = ("array literal", "teal");

    protected readonly (string tooltip, string color) Bool = ("bool literal", "");

    protected readonly (string tooltip, string color) ComplexString = ("Complex string literal", "darkorange");

    protected readonly (string tooltip, string color) FuncCall = ("call to a function", "tomato");
    protected readonly (string tooltip, string color) FuncCallParam = ("argument", "");

    protected readonly (string tooltip, string color) Ident = ("identifier", "");

    protected readonly (string tooltip, string color) Number = ("number", "");

    protected readonly (string tooltip, string color) ObjCreation = ("object creation/ctor call", "indigo");
    protected readonly (string tooltip, string color) ObjTypeName = ("class name", "");

    protected readonly (string tooltip, string color) Operation = ("OperationNode", "dodgerblue");

    protected readonly (string tooltip, string color) String = ("StringNode", "orange");

    protected readonly (string tooltip, string color) Value = ("ValueNode", "lightgrey");

    protected readonly (string tooltip, string color) Tuple = ("List of values", "");

    protected readonly (string tooltip, string color) SimpleBlock = ("body", "darkviolet");


    protected GraphNode BaseDefault(StatementNode node)
        => new GraphNode(node.GetHashCode(), node.Representation)
            .SetColor(Statement.color)
            .SetTooltip(Statement.tooltip);
    protected GraphNode BaseDefault(ValueNode node)
        => new GraphNode(node.GetHashCode(), node.Representation)
            .SetColor(Value.color)
            .SetTooltip(Value.tooltip);

    // TODO: This is used by basically all nodes to be able to get the basic GraphNode
    // If you change this, be very careful ! You could end up with a stack overflow of
    // a completely different graph !
    public virtual GraphNode Default(StatementNode node)
        => BaseDefault(node);
    public virtual GraphNode Default(ValueNode node)
        => BaseDefault(node);

    public GraphNode Visit(StatementNode node)
        => Default(node);


    public GraphNode Visit(BreakNode node)
        => Default(node)
            .SetColor(Break.color)
            .SetTooltip(Break.tooltip);

    public GraphNode Visit(ContinueNode node)
        => Default(node)
            .SetColor(Continue.color)
            .SetTooltip(Continue.tooltip);

    public GraphNode Visit(DeclarationNode node)
        => new GraphNode(node.GetHashCode(), "var") {
               ASTHelper.ToGraphNode(node.Name)
                    .SetColor(DeclarationName.color)
                    .SetTooltip(DeclarationName.tooltip),
               ToGraphNode(node.Value)
           }.SetColor(Declaration.color)
            .SetTooltip(Declaration.tooltip);

    public GraphNode Visit(ElseNode node)
        => new GraphNode(node.GetHashCode(), "else") {
               node.HasIf ? ToGraphNode(node.IfNode!) : ToGraphNode(node.Body)
           }.SetColor(Else.color)
            .SetTooltip(Else.tooltip);

    public GraphNode Visit(ForeachNode node)
        => new GraphNode(node.GetHashCode(), "foreach") {
               new GraphNode(node.InToken.GetHashCode(), "in") {
                    ToGraphNode(node.ItemName),
                    ToGraphNode(node.Collection)
               }.SetTooltip("in iterator"),
               ToGraphNode(node.Body),
           }.SetColor(Foreach.color)
            .SetTooltip(Foreach.tooltip);

    public GraphNode Visit(ForNode node) {
        var root = new GraphNode(node.GetHashCode(), "for loop")
                        .SetColor(For.color)
                        .SetTooltip(For.tooltip);

        if (node.Header.Count != 0) {
            var headerNode = new GraphNode(node.Header.GetHashCode(), "header")
                .SetColor(ForHeader.color)
                .SetTooltip(ForHeader.tooltip);

            foreach (var statement in node.Header) headerNode.Add(ToGraphNode(statement));

            root.Add(headerNode);

        } else {
            root.Add(new GraphNode(node.Header.GetHashCode(), "(empty header)"));
        }

        root.Add(ToGraphNode(node.Body));

        return root;
    }

    public GraphNode Visit(FunctionDeclarationNode node) {
        var root = new GraphNode(node.GetHashCode(), "func " + node.Name.Representation)
                        .SetColor(FuncDec.color)
                        .SetTooltip(FuncDec.tooltip);

        if (node.Parameters.Count == 0) {
            root.Add(new GraphNode(node.Parameters.GetHashCode(), "(no params)"));
        } else {
            var parametersNode = new GraphNode(node.Parameters.GetHashCode(), "param")
                                        .SetColor(FuncDecParameters.color)
                                        .SetTooltip(FuncDecParameters.tooltip);

            foreach (var parameter in node.Parameters) { // FIXME: Write tooltips

                var paramNameNode = ToGraphNode(parameter.Name);

                if (parameter.Type == ValueNode.NULL) {
                    paramNameNode.Add(new GraphNode(DeterministicHashCode.Combine(ValueNode.NULL, parameter), "any"));
                } else {
                    paramNameNode.Add(ToGraphNode(parameter.Type));
                }

                if (parameter.HasDefaultValue) {
                    paramNameNode.Add(ToGraphNode(parameter.DefaultValue));
                }

                parametersNode.Add(paramNameNode);
            }

            root.Add(parametersNode);
        }

        if (node.HasReturnType) {
            root.Add(new GraphNode(DeterministicHashCode.Combine(node.ReturnType, node), "return type") { // FIXME: Color & Tooltip
                ToGraphNode(node.ReturnType)
            });
        }

        root.Add(ToGraphNode(node.Body));

        return root;
    }

    public GraphNode Visit(IfNode node) {
        var root = new GraphNode(node.GetHashCode(), "if") {
            new GraphNode(DeterministicHashCode.Combine(node, "condition"), "condition") {
                ToGraphNode(node.Condition)
            }.SetColor(IfCondition.color)
             .SetTooltip(IfCondition.tooltip),
            ToGraphNode(node.Body)
        }.SetColor(If.color)
         .SetTooltip(If.tooltip); // FIXME: Choose color

        if (node.HasElse) {
            root.Add(ToGraphNode(node.ElseNode!));
        }

        return root;
    }

    public GraphNode Visit(PrintNode node)
        => new GraphNode(node.GetHashCode(), "print") {
                ToGraphNode(node.Value)
            }.SetColor(Print.color)
             .SetTooltip(Print.tooltip); // FIXME: find color

    public GraphNode Visit(ReturnNode node) {
        var root = new GraphNode(node.GetHashCode(), "return")
            .SetColor(Return.color)
            .SetTooltip(Return.tooltip);

        if (node.IsReturningValue) {
            root.Add(ToGraphNode(node.Value));
        }

        return root;
    }

    public GraphNode Visit(StatementExpressionNode node)
        => ToGraphNode(node.Value);


    public GraphNode Visit(WhileNode node)
        => new GraphNode(node.GetHashCode(), node.IsDoLoop ? "do-while" : "while") {
                new GraphNode(DeterministicHashCode.Combine(node, "condition"), "condition") {
                    ToGraphNode(node.Condition)
                }.SetColor(WhileCondition.color)
                 .SetTooltip(WhileCondition.tooltip),
                ToGraphNode(node.Body)
            }.SetColor(While.color)
             .SetTooltip(node.IsDoLoop ? "do-while loop" : "while loop");



    public GraphNode Visit(ValueNode node)
        => new GraphNode(node.GetHashCode(), node.Representation)
            .SetColor(Value.color)
            .SetTooltip(Value.tooltip);

    public GraphNode Visit(BoolNode node)
        => Default(node)
            .SetColor(Bool.color)
            .SetTooltip(Bool.tooltip);

    public GraphNode Visit(ComplexStringNode node) {
        var root = new GraphNode(node.GetHashCode(), node.Representation)
                        .SetColor(ComplexString.color)
                        .SetTooltip(ComplexString.tooltip);

        if (node.CodeSections.Count != 0) {
            var sectionNode = new GraphNode(DeterministicHashCode.Combine(node, "sections"), "code sections");

            foreach (var section in node.CodeSections) {
                sectionNode.Add(ToGraphNode(section));
            }

            root.Add(sectionNode);
        }

        return root;
    }

    public GraphNode Visit(FunctionCallNode node) {
        GraphNode root;

        if (node.FunctionName is IdentNode name) {
            root = new GraphNode(node.GetHashCode(), name.Representation + "(...)");
        } else {
            root = new GraphNode(node.GetHashCode(), "call") {
                new GraphNode(DeterministicHashCode.Combine(node.FunctionName, "function"), "function") {
                    ToGraphNode(node.FunctionName)
                },
            };
        }

        root.SetColor(FuncCall.color)
            .SetTooltip(FuncCall.tooltip);

        if (node.ArgList.Count == 0) {
            root.Add(new GraphNode(node.ArgList.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = ToGraphNode(node.ArgList).SetTooltip("arguments");

        argsNode.Name = "args";

        root.Add(argsNode);

        return root;
    }

    public GraphNode Visit(IdentNode node)
        => Default(node)
            .SetColor(Ident.color)
            .SetTooltip(Ident.tooltip);

    public GraphNode Visit(NumberNode node)
        => Default(node)
            .SetColor(Number.color)
            .SetTooltip(Number.tooltip);

    public GraphNode Visit(ObjectCreationNode node) {
        var root = new GraphNode(node.GetHashCode(), "obj creation") {
            ToGraphNode(node.TypeName)
                .SetColor(ObjTypeName.color)
                .SetTooltip(ObjTypeName.tooltip),
        };

        root.SetColor(ObjCreation.color)
            .SetTooltip(ObjCreation.tooltip);

        if (node.InvocationNode.ArgList.Count == 0) {
            root.Add(new GraphNode(node.InvocationNode.ArgList.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = ToGraphNode(node.InvocationNode.ArgList).SetTooltip("argument");

        argsNode.Name = "args";

        root.Add(argsNode);

        return root;
    }

    public GraphNode Visit(OperationNode node) {
        GraphNode root;

        if (node.Representation == "++" || node.Representation == "--") {
            root = new GraphNode(node.GetHashCode(), (node.OperationType.ToString().StartsWith("Postfix") ? "(postfix)" : "(prefix)") + node.Representation);
        } else {
            root = new GraphNode(node.GetHashCode(), node.Representation);
        }

        root.SetColor(Operation.color)
            .SetTooltip(Operation.tooltip);

        foreach (var child in node.Operands) {
            root.Add(ToGraphNode(child));
        }

        return root;
    }

    public GraphNode Visit(ParenthesizedValueNode node)
        => ToGraphNode(node.Values);

    public GraphNode Visit(StringNode node)
        => new GraphNode(node.GetHashCode(), "'" + node.Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'")
            .SetColor(String.color)
            .SetTooltip(String.tooltip);

    public GraphNode Visit(TupleNode node) {
        var root = new GraphNode(node.GetHashCode(), node.Count == 0 ? "Empty tuple" : "Tuple with\\n" + node.Count + " elements")
                        .SetColor(Tuple.color)
                        .SetTooltip(Tuple.tooltip);

        foreach (var value in node.Values) {
            root.Add(ToGraphNode(value));
        }

        return root;
    }

    public GraphNode Visit(SimpleBlock block) {
        var root = new GraphNode(block.GetHashCode(), "block")
            .SetColor(SimpleBlock.color)
            .SetTooltip(SimpleBlock.tooltip);

        foreach (var statement in block.Content) root.Add(ToGraphNode(statement));

        return root;
    }

    public virtual GraphNode ToGraphNode(StatementNode node) => node.Accept(this);

    public virtual GraphNode ToGraphNode(ValueNode node) => node.Accept(this);

    public GraphNode ToGraphNode(SimpleBlock block) => Visit(block);
}