using UnityEngine;
using UnityEngine.AI;

public class LadderClimbSystem : MonoBehaviour
{
    public Transform ladderTop; // Assign the top of the ladder in the Inspector
    public Transform ladderBottom; // Assign the bottom of the ladder in the Inspector
    public NavMeshAgent agent; // Assign the character's NavMeshAgent in the Inspector
    public Animator animator; // Assign the character's Animator (optional)

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object has an AtticArea component
                AtticArea atticArea = hit.collider.GetComponent<AtticArea>();
                if (atticArea != null)
                {
                    // Move the agent to the bottom of the ladder
                    agent.SetDestination(ladderBottom.position);

                    // Wait for the agent to reach the ladder, then climb
                    StartCoroutine(ClimbLadder());
                }
            }
        }
    }

    System.Collections.IEnumerator ClimbLadder()
    {
        // Wait for the agent to reach the bottom of the ladder
        while (Vector3.Distance(agent.transform.position, ladderBottom.position) > 0.1f)
        {
            yield return null;
        }

        // Play climbing animation (optional)
        if (animator != null)
        {
            animator.SetTrigger("Climb");
            yield return new WaitForSeconds(2.0f); // Adjust time to match animation length
        }

        // Teleport the agent to the top of the ladder
        agent.Warp(ladderTop.position);
        agent.SetDestination(ladderTop.position); // Continue moving to the attic
    }
}