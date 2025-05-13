public class BehaviourTree
{
    public Node root;

    public void Tick()
    {
        if (root != null)
            root.Evaluate();
    }
}
