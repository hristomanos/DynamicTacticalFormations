using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{

    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Tells the agent to move to a new position.
    /// </summary>
    /// <param name="velocity"> The offset vector which the agent will move to.</param>
    /// <returns></returns>
    public void Move(Vector2 velocity)
    {
        //Turn to face the direction you are going to be moving towards
        transform.up = velocity;

        //Add next position to your current position through time.
        transform.position += (Vector3) velocity * Time.deltaTime;
    }
}
