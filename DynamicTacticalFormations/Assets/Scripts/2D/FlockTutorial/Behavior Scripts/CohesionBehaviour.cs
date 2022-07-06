using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cohesion behavior tries to find the middle point of all our neighbours and tries to move there

[CreateAssetMenu(menuName ="Flock/Behavior/Cohesion")]
public class CohesionBehaviour : FlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //If there are neighbours then we do not need to make any adjustments
        if (context.Count == 0)
               return Vector2.zero;

        //Add all points and average
        Vector2 cohesionMove = Vector2.zero;
        foreach (Transform item in context)
        {
            cohesionMove += (Vector2) item.position;
        }

        cohesionMove /= context.Count;

        //Because the result is in global space, Create offset from agent position ?????????????
        cohesionMove -= (Vector2)agent.transform.position;

        return cohesionMove;
    }
}
