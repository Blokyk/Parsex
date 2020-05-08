using System.Collections.Generic;
using System.Collections.ObjectModel;

public class OperationNode : ValueNode
{
    public new OperatorToken Token { get => Token; }

    /// <summary>
    /// The operands of this operation
    /// </summary>
    /// <value>An array of ValueNode</value>
    public ReadOnlyCollection<ValueNode> Operands { get; }

    public OperationType OperationType { get; }

    public OperationKind OperationKind {
        get => Operands.Count switch {
            1 => OperationKind.Unary,
            2 => OperationKind.Binary,
            3 => OperationKind.Trinary,
            _ => OperationKind.Unknown
        };
    }

    public OperationNode(OperatorToken token, IList<ValueNode> operands, OperationType opType) : base(token)
    {
        Operands = operands.AsReadOnly();
        OperationType = opType;
    }

    public override GraphNode ToGraphNode() {

        var root = new GraphNode(GetHashCode(), Representation);

        root.AddProperty("color", "dodgerblue");
        root.AddProperty("tooltip", nameof(OperationNode));

        if (Representation == "++" || Representation == "--") {
            root = new GraphNode(GetHashCode(), (OperationType.ToString().StartsWith("Postfix") ? "(postfix)" : "(prefix)") + Representation);
        }

        foreach (var child in Operands)
        {
            root.AddNode(child.ToGraphNode());
        }

        return root;
    }
}