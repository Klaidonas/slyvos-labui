public enum NodeState { Running, Success, Failure }

public abstract class Node
{
    public NodeState state;
    public abstract NodeState Evaluate();
}
