using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// This scripts is responsible for finding a virtual leader, registering to it, and receive orders from the leader as to where to stand in the formation.
/// 
/// It can:
///     * Find a virtual leader and register itself to it
///     * Get relative formation position from the leader
///     * Look at the same direction as the leader
/// </summary>


public class SelectedComponent : MonoBehaviour
{
    //AI movement
    NavMeshAgent  m_NavMeshAgent;
    
    //Invisible entity that calculates where each unit will stand in the formation
    VirtualLeader m_MyVirtualLeader;

    //Green circle indicating that this object has been selected
    GameObject m_UnitSelectedIndicator;

    Vector3 m_TargetPos;

    Vector3 m_FormationPos;

    public bool m_PositionReached = false;

    CapsuleCollider m_CapsuleCollider;

    List<Transform> m_NeighboursTranform = new List<Transform>();

    float m_MaxDrift = 1.0f;
    float m_SpeedModifier;
    float m_OriginalSpeed;
    public float m_MaxSpeedModifier = 2.0f;
    void Start()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();


        m_OriginalSpeed = m_NavMeshAgent.speed;

        //Because the script is added to the selected object, I couldn't find a way to reference the indicator child game object.
        m_UnitSelectedIndicator = transform.Find("UnitSelectedIndicator").gameObject;
        m_UnitSelectedIndicator.SetActive(true);
            
        //Because I cannot reference the virtual leader through the inspector,
        //We make a reference by finding it on the scene. What if there are more than one? Will need to find a different way to make a reference.
        //What if I instantiate a virtual leader (or have an object pool) and I just place it at the average centre position?
        m_MyVirtualLeader = FindObjectOfType<VirtualLeader>();

        m_FormationPos = Vector3.zero;

        m_CapsuleCollider = GetComponent<CapsuleCollider>();

        //If virtual leader was not found
        if (m_MyVirtualLeader == null)
        {
            Debug.LogError("ERROR: Leader is null for " + gameObject.name);
            Debug.Break();
        }
        else
        {
            //Register to squad
            m_MyVirtualLeader.RegisterUnitToSquad(this);
        }

        StartCoroutine(UpdateTargetPosition());
        
    }

    private void Update()
    {
        CheckSpeed();
    }

    private void OnDestroy()
    {
       //Turn indicator off
       m_UnitSelectedIndicator.SetActive(false);
       
       //Remove member from the list
       m_MyVirtualLeader.DeregisterUnitFromSquad(this);
    }

    /// <summary>
    /// Gets new target position, sets destination and copies rotation
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateTargetPosition()
    {
        while(m_MyVirtualLeader.enabled)
        {
            //Wait until next frame. Since it is a while loop everything will be run on a single frame
            yield return null;

            //Get next position
            m_FormationPos = m_MyVirtualLeader.GetMemberPosition(this,out m_TargetPos);

            //If you reach the target position and stop look at the same direction as the leader. All soldiers look at the same direction.
            //This solution is a bit snappy but does the job.
            if (Vector3.Distance(m_FormationPos, transform.position) <= m_NavMeshAgent.stoppingDistance)
            {
                CopyRotationFrom(m_MyVirtualLeader.gameObject);
                m_PositionReached = true;
            }
            else
                m_PositionReached = false;


            //Go to the target position
            //m_NavMeshAgent.SetDestination(m_TargetPos);
            m_NavMeshAgent.SetDestination(m_FormationPos);
            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_FormationPos, 0.1f);
    }


    public void CopyRotationFrom(GameObject Target)
    {
        transform.rotation = Target.transform.localRotation;
    }


    List<Transform> GetNearbyObjects()
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(transform.position, 1);


        //Foreach collider in this array, as long it is not ourselves
        //Then take the transform and add it to the list
        foreach (Collider c in contextColliders)
        {
            if (c != m_CapsuleCollider)
            {
                context.Add(c.transform);
            }
        }


        return context;
    }

    private void CheckSpeed()
    {
        float distToFormationPosZ = transform.InverseTransformPoint(m_FormationPos).z;
        float distToFormationPosX = transform.InverseTransformPoint(m_FormationPos).x;

        if (distToFormationPosZ > 0 && distToFormationPosZ > m_MaxDrift)
        {
            m_SpeedModifier = distToFormationPosZ / m_MaxDrift;
        }
        else if (distToFormationPosZ < 0 && Mathf.Abs(distToFormationPosZ) > m_MaxDrift && m_NavMeshAgent.remainingDistance < m_MaxDrift * 4.0f)
        {
            m_SpeedModifier = m_MaxDrift / Mathf.Abs(distToFormationPosZ);
        }
        else
            m_SpeedModifier = 1.0f;

        if (distToFormationPosX > m_MaxDrift)
            m_SpeedModifier = 1.0f;

        if (m_SpeedModifier > m_MaxSpeedModifier)
            m_SpeedModifier = m_MaxSpeedModifier;

        m_NavMeshAgent.speed = m_OriginalSpeed * m_SpeedModifier;
    }

    public Vector3 AvoidNeighbours()
    {
        //If there are no neighbours then we do not need to make any adjustments
        if (m_NeighboursTranform.Count == 0)
            return Vector2.zero;

        //Add all points and average
        Vector2 avoidanceMove = Vector2.zero;

        //How many are specifically within the avoidance radius
        //Number of things to avoid
        int nAvoid = 0;
        foreach (Transform neighbour in m_NeighboursTranform)
        {
            if (Vector2.SqrMagnitude(neighbour.position - transform.position) < 2)//flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                //Gives us the offset
                avoidanceMove += (Vector2)(transform.position - neighbour.position);
            }
        }

        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }

        return avoidanceMove;
    }

}
