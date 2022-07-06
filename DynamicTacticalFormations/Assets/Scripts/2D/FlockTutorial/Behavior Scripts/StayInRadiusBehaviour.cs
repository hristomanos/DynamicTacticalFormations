using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/StatInRadius")]
public class StayInRadiusBehaviour : FlockBehaviour
{

    public Vector2 centre;
    public float radius = 15f;
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        Vector2 centreOffset = centre - (Vector2)agent.transform.position;

        //If t = 0 => right on centre.
        //If t = 1 => at the radius.
        //if t > 1 => beyond radius.
        float t = centreOffset.magnitude / radius;

        if (t < 0.9f)
        {
            return Vector2.zero;
        }

        return centreOffset * t * t;

    }
}
