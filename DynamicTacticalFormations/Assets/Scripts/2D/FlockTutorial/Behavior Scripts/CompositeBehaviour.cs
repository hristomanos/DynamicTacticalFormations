using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    public FlockBehaviour[] behaviours;
    public float[] weights;



    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //If we have less weight than we have behaviours or vice versa then we are going to run into some issues
        if (weights.Length != behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector2.zero;
        }

        //set up move
        Vector2 move = Vector2.zero;

        //Iterate through behaviours
        for (int i = 0; i < behaviours.Length; i++)
        {
            Vector2 partialMove = behaviours[i].CalculateMove(agent, context, flock) * weights[i];


            //Partial move is limited to the extent of the weight
            if (partialMove != Vector2.zero)
            {
                //Does this overall movement exceed the weight
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    //Reset vector
                    partialMove.Normalize();

                    //Set the max of weight
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;

    }
}
