using System.Collections.Generic;

public class SelectorNode : Node
{
    private List<Node> children;

    public SelectorNode(params Node[] nodes)
    {
        children = new List<Node>(nodes);
    }

    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.Success || result == NodeState.Running)
            {
                return result;
            }
        }
        return NodeState.Failure;
    }
}
