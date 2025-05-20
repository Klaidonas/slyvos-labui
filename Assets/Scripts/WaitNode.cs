using UnityEngine;

public class WaitNode : Node
{
    private float waitTime;
    private float timer;

    public WaitNode(float seconds)
    {
        waitTime = seconds;
        timer = 0f;
    }

    public override NodeState Evaluate()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            timer = 0f;
            return NodeState.Success;
        }
        return NodeState.Running;
    }
}
