using System.Collections.Generic;

public class SequenceNode : Node
{
    private List<Node> children;

    public SequenceNode(params Node[] nodes)
    {
        children = new List<Node>(nodes);
    }

    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.Failure)
                return NodeState.Failure;
            if (result == NodeState.Running)
                return NodeState.Running;
        }
        return NodeState.Success;
    }
}
