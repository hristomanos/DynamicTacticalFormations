using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class selection_component : MonoBehaviour
{

    NavMeshAgent m_NavMeshAgent;
    Animator m_Animator;
    GameObject m_UnitSelectedIndicator;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
        {
            //GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.red;
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_UnitSelectedIndicator = transform.Find("UnitSelectedIndicator").gameObject;
            m_UnitSelectedIndicator.SetActive(true);
        }
    }

    private void Update()
    {
        MoveUnit();
        if (transform.childCount > 0)
        {
            m_Animator.SetFloat("Speed", m_NavMeshAgent.velocity.magnitude);
        }
    }


    private void OnDestroy()
    {
        if (transform.childCount > 0)
        {
           // GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.white;
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


}
