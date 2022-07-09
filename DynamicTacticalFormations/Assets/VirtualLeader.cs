using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This script is responsible for calculating the positions for each unit in the formation

public class VirtualLeader : MonoBehaviour
{
    //Need to know about what a formation is
    List<Formation> m_Formations;

    //Need to know about what types of formations we got
   public FormationType[] m_FormationTypes;
    
    //Need to generate paths and move along with the squad
    NavMeshAgent m_Agent;

    //Need to know about squad members
    List<selection_component> m_Members;

    //Need to know what is the max number of members
    uint m_MaxMemberCapacity;

    //???????????
    //Current formation index

    // Start is called before the first frame update
    void Start()
    {
        m_Formations = new List<Formation>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Members = new List<selection_component>();
        
        //Initilise formations list with formations
        CreateFormations();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Populate list of formations by checking that all formation are in the list once
    //And storing the maximum number of units for each formation
    void CreateFormations()
    {
        //We check that for each formation type in the list
        foreach (FormationType type in m_FormationTypes)
        {
            //If that type of formation does not exist in the formation list
            if (!m_Formations.Exists(f => type == f.Type))
            {
                //Then evaluate it and add it to the list
                switch (type)
                {
                    case FormationType.WEDGE:
                        m_Formations.Add(new WedgeFormation());
                        break;
                    default:
                        Debug.LogError("Formation requested for " + gameObject.name + " is not implemented! ");
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Duplicate type requested for " + gameObject.name + "!");
            }
        }

        //Get Maximum number of members in a formation
        m_MaxMemberCapacity = m_Formations[0].MaxUnits;

        foreach (Formation formation in m_Formations)
        {
            if (formation.MaxUnits < m_MaxMemberCapacity)
            {
                m_MaxMemberCapacity = formation.MaxUnits;
            }
        }
    }

    
    public bool RegisterUnitToSquad(selection_component unit)
    {
        //We reached maximum squad capacity
        if (m_Members.Count == m_MaxMemberCapacity)
        {
            Debug.LogWarning("WARNING: Squad has reached max capacity!");
            return false;
        }

        //Make sure no units are added twice!
        if (!m_Members.Exists(u => u.GetInstanceID() == unit.GetInstanceID()))
        {
            m_Members.Add(unit);
            return true;
        }
        else
        {
            Debug.LogWarning("WARNING: " + unit.name + " is already in the squad!");
            return false;
        }
    }

    public Vector3 GetMemberPosition(selection_component member, out Vector3 targetPos)
    {
        targetPos = Vector3.zero;
        Vector3 unitPos = Vector3.zero;
        Vector3 respectiveUnitPos = Vector3.zero; //position in the formation
        int unitIndex = -1;

        //Find the index in members list for that unit
        unitIndex = m_Members.FindIndex(u => u.GetInstanceID() == member.GetInstanceID());

        //if unit is a member then get formation position
        if (unitIndex >= 0)
        {

            respectiveUnitPos = m_Formations[0].GetUnitPosition(unitIndex);
        }
        else
        {
            Debug.LogError(member.name + " requested a formation position but is not a member of " + gameObject.name);
            return unitPos;
        }


        //Tranform Local formation pos to world space
        unitPos = transform.TransformPoint(respectiveUnitPos);

        //Get predicted pos ?????????????????????????????????????????????????
        float maxDistance = m_Agent.speed * 0.5f;
        NavMeshHit prediction;
        m_Agent.SamplePathPosition(1, maxDistance, out prediction);
        targetPos = transform.TransformPoint(transform.InverseTransformPoint(prediction.position) + respectiveUnitPos);

        return unitPos;
    }
}
