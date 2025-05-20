using UnityEngine;

public class CanSeePlayerNode : Node
{
    private Transform guard;
    private Transform player;
    private GuardAI ai;
    private float visionRange = 8f;

    public CanSeePlayerNode(Transform guard, Transform player, GuardAI ai)
    {
        this.guard = guard;
        this.player = player;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(guard.position, player.position);
        if (distance < visionRange)
        {
            ai.isAlerted = true;
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}
