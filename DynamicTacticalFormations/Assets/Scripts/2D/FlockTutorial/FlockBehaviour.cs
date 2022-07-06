using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This abstract class is used for keeping the main methods that flocking behaviours use

public abstract class FlockBehaviour : ScriptableObject
{

    /// <summary>
    /// This method combines all the behaviours such as allignment, cohesion and separation and calculates the way an agent will move.
    /// </summary>
    /// <param name="agent"> The actual agent object.</param> 
    /// <param name="context"> Agent's neighbours' positions. They could be other agents or obstacles.</param>
    /// <param name="flock"> The flock itself. There may be sittuations with certain behaviours that we need some information about the flock. </param>
    /// <returns>Vector2 point to move to</returns>
    public abstract Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock);
}
