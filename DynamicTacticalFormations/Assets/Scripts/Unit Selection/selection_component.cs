using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class selection_component : MonoBehaviour
{

    NavMeshAgent m_NavMeshAgent;
    Animator m_Animator;
    GameObject m_UnitSelectedIndicator;

    //Need to know about my leader of the group
    public VirtualLeader m_LeaderObject;
    VirtualLeader m_MyLeader;

    //Need to know about where to go next
    Vector3 m_TargetPos;
    
    //Need to know about my formation position
    Vector3 m_FormationPos;

    //Not sure what this is for
    public float m_RepathDistance = 1.0f;

    
    void Start()
    {
        if (transform.childCount > 0)
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
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
        if (transform.childCount > 0)
        {
            m_Animator.SetFloat("Speed", m_NavMeshAgent.velocity.magnitude);
        }
    }


    private void OnDestroy()
    {
        if (transform.childCount > 0)
        {
            m_UnitSelectedIndicator.SetActive(false);
        }
    }

    void MoveUnit()
    {
        if (Input.GetMouseButtonDown(1))
        {
            m_NavMeshAgent.SetDestination(GetMouseWorldPosition());
        }
    }

    void MoveUnit(Vector3 targetPosition)
    {
        m_TargetPos = targetPosition;
        m_NavMeshAgent.SetDestination(m_TargetPos);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        Vector3 mouseWorldPosition;

        if (Physics.Raycast(ray, out hit))
        {
            mouseWorldPosition = hit.point;
            return mouseWorldPosition;
        }
        else
        {
            return Vector3.zero;
        }
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

            if ((nextTargetPos - m_TargetPos).sqrMagnitude > (m_RepathDistance * m_RepathDistance))
            {
                m_TargetPos = nextTargetPos;
                m_NavMeshAgent.SetDestination(m_TargetPos);
            }
        }

    }

}
