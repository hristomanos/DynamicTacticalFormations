using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class selection_component : MonoBehaviour
{

    NavMeshAgent m_NavMeshAgent;
    
    GameObject m_UnitSelectedIndicator;

    //Need to know about my leader of the group
    public VirtualLeader m_LeaderObject;
    VirtualLeader m_MyLeader;

    //Need to know about where to go next
    Vector3 m_TargetPos;
    
    //Need to know about my formation position
    Vector3 m_FormationPos;

    //Not sure what this is for ???????????????
    public float m_RepathDistance = 1.0f;

   


    void Start()
    {
        if (transform.childCount > 0)
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_UnitSelectedIndicator = transform.Find("UnitSelectedIndicator").gameObject;
            m_UnitSelectedIndicator.SetActive(true);

            m_LeaderObject = FindObjectOfType<VirtualLeader>();

            if (m_LeaderObject == null)
            {
                Debug.LogError("ERROR: Leader is null for " + gameObject.name);
                Debug.Break();
            }

            m_MyLeader = m_LeaderObject.GetComponent<VirtualLeader>();

            StartCoroutine(CheckSquadTargetPosition());
        }
    }

    private void Update()
    {
        
    }


    private void OnDestroy()
    {
        if (transform.childCount > 0)
        {
            m_UnitSelectedIndicator.SetActive(false);
        }

        //Remove member from the list
        m_MyLeader.DeregisterUnitFromSquad(this);
    }

    void MoveUnit(Vector3 targetPosition)
    {
        m_TargetPos = targetPosition;
        m_NavMeshAgent.SetDestination(m_TargetPos);
    }

  

    IEnumerator CheckSquadTargetPosition()
    {
        yield return null;
        m_MyLeader.RegisterUnitToSquad(this);

        while(m_MyLeader.enabled)
        {
            yield return null;

            Vector3 nextTargetPos;
            m_FormationPos = m_MyLeader.GetMemberPosition(this,out nextTargetPos);

            if (Vector3.Distance(m_TargetPos, transform.position) <= m_NavMeshAgent.stoppingDistance)
            {
                CopyRotation(m_MyLeader.gameObject);
            }

            //if then new pos is not futher than repath distance
            if ((nextTargetPos - m_TargetPos).sqrMagnitude > (m_RepathDistance * m_RepathDistance))
            {
                m_TargetPos = nextTargetPos;
                m_NavMeshAgent.SetDestination(m_TargetPos);
            }
        }

    }


    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5);
    }

    public void CopyRotation(GameObject Target)
    {
        transform.rotation = Target.transform.localRotation;
    }

}
