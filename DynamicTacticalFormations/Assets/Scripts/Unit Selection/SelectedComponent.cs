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

    void Start()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>(); 

        //Because the script is added to the selected object, I couldn't find a way to reference the indicator child game object.
        m_UnitSelectedIndicator = transform.Find("UnitSelectedIndicator").gameObject;
        m_UnitSelectedIndicator.SetActive(true);
            
        //Because I cannot reference the virtual leader through the inspector,
        //We make a reference by finding it on the scene. What if there are more than one? Will need to find a different way to make a reference.
        //What if I instantiate a virtual leader (or have an object pool) and I just place it at the average centre position?
        m_MyVirtualLeader = FindObjectOfType<VirtualLeader>();

        m_FormationPos = Vector3.zero;

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
            if (Vector3.Distance(m_TargetPos, transform.position) <= m_NavMeshAgent.stoppingDistance)
            {
                CopyRotationFrom(m_MyVirtualLeader.gameObject);
            }

            //Go to the target position
            m_NavMeshAgent.SetDestination(m_TargetPos);
            
        }
    }

    public void CopyRotationFrom(GameObject Target)
    {
        transform.rotation = Target.transform.localRotation;
    }

}
