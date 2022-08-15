using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehaviour : FlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //If there are no neighbours then we do not need to make any adjustments
        if (context.Count == 0)
            return Vector2.zero;

        //Add all points and average
        Vector2 avoidanceMove = Vector2.zero;

        //How many are specifically within the avoidance radius
        //Number of things to avoid
        int nAvoid = 0;
        foreach (Transform item in context)
        {
            if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                //Gives us the offset
                avoidanceMove += (Vector2)(agent.transform.position - item.position);
            }
        }

        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }

        return avoidanceMove;
    }
}
