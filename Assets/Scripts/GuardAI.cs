using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GuardAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;
    public bool isAlerted = false;

    private Node topNode;
    private NavMeshAgent agent;
    private PatrolNode patrolNode;

    void Start()
    {
        // Check for multiple GuardAI components
        if (GetComponents<GuardAI>().Length > 1)
        {
            Debug.LogError($"Multiple GuardAI components on {gameObject.name}! Disabling this instance.");
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found!");
            return;
        }

        Debug.Log($"GuardAI Start: PatrolPoints count: {patrolPoints.Length}, Player: {player != null}, Agent: {agent != null}");

        patrolNode = new PatrolNode(agent, patrolPoints, this);
        SequenceNode patrolSequence = new SequenceNode(patrolNode);

        CanSeePlayerNode canSeePlayer = new CanSeePlayerNode(transform, player, this);
        ChasePlayerNode chase = new ChasePlayerNode(agent, player);

        SequenceNode chaseSequence = new SequenceNode(canSeePlayer, chase);

        topNode = new SelectorNode(chaseSequence, patrolSequence);
        Debug.Log("Behavior tree initialized");
    }

    void Update()
    {
        if (topNode == null)
        {
            Debug.LogWarning("topNode is null!");
            return;
        }
        Debug.Log($"GuardAI Update: Evaluating topNode, PatrolNode: {patrolNode != null}, isAlerted: {isAlerted}");
        topNode.Evaluate();
    }

    void OnDrawGizmos()
    {
        if (patrolNode != null)
        {
            patrolNode.OnDrawGizmos();
        }
    }
}