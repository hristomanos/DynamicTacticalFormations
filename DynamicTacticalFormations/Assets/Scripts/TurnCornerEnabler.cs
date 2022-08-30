using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCornerEnabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Leader"))
        {


            VirtualLeader leader = GetComponent<VirtualLeader>();

            leader.m_isWheeling = true;
            Debug.Log(leader.m_isWheeling);
        }
    }
}
