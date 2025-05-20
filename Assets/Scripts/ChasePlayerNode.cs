using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerNode : Node
{
    private NavMeshAgent agent;
    private Transform player;

    public ChasePlayerNode(NavMeshAgent agent, Transform player)
    {
        this.agent = agent;
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        agent.SetDestination(player.position);
        return NodeState.Running;
    }
}
