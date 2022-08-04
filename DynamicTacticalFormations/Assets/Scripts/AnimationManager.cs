using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{

    UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
    Animator m_Animator;

    // Start is called before the first frame update
    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            m_Animator.SetFloat("Speed", m_NavMeshAgent.velocity.magnitude);
        }
    }
}
