using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Take the average heading vector from your neighbours


[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehaviour : FlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //If there are neighbours then we maintain current alignment
        if (context.Count == 0)
            return agent.transform.up;

        //Add all heading vectors and average
        Vector2 alignmentMove = Vector2.zero;
        foreach (Transform item in context)
        {
            alignmentMove += (Vector2)item.up;
        }

        //Each item.up is normalised thus not needing to transform to local space
        alignmentMove /= context.Count;

        
        

        return alignmentMove;
    }
}
