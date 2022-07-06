using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for populating the flock with prefabs and
//handling the iteration and executing the behaviours on the flock agents as they are iterated through.

public class Flock : MonoBehaviour
{
    public FlockAgent agentprefab;

    //All the agents are cached in the list once they are instantiated to get iterated through.
    List<FlockAgent> agents = new List<FlockAgent>();

    //Used to put in the scriptable object
    public FlockBehaviour behaviour;

    //Allows user to specify the total number of agents in the scene
    [Range(10, 500)]
    public int startingCount = 250;

    //The circle radius of spawned agents depends on the total number of agents
    const float AGENT_DENSITY = 0.08f;

    //Drive factor. Movement calculation tend to make many counter active small corrections that results to agents moving slowly.
    //Drive factor is a multiplier
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;


    //Radiuses
    [Range(1f, 10f)]
    public float neighbourRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;
    
    //Gets the square root of respective variables
    //A lot of times we are just comparing
    //Save us from doing some excessive math when running our behaviours
    float squareMaxSpeed;
    float squareNeighbourRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareMaxSpeed; } }



    void Start()
    {
        squareMaxSpeed = squareMaxSpeed * squareMaxSpeed;
        squareNeighbourRadius = squareNeighbourRadius * squareNeighbourRadius;
        squareAvoidanceRadius = squareNeighbourRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentprefab,
                Random.insideUnitCircle * startingCount * AGENT_DENSITY,
                Quaternion.Euler(Vector3.forward * Random.Range(0, 360)),
                transform
                );

            newAgent.name = "Agent " + i;
            agents.Add(newAgent); 
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in agents)
        { 
            //What things exist around the agent in question?
            List<Transform> context = GetNearbyObjects(agent);

            //FOR DEMO ONLY
            //agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count / 6f);


            Vector2 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;

            //Have we exceeded our maximum speed that we want?
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }


    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighbourRadius);


        //Foreach collider in this array, as long it is not ourselves
        //Then take the transform and add it to the list
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }


        return context;
    }    
}
