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

    int m_CurrentFormationIndex;

    //TEST
    KeyCode[] keyCodes = {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
    };

    // Start is called before the first frame update
    void Start()
    {
        m_Formations = new List<Formation>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Members = new List<selection_component>();

        m_CurrentFormationIndex = 0;

        //Initilise formations list with formations
        CreateFormations();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            m_Agent.SetDestination(GetMouseWorldPosition());
        }

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                int numberPressed = i + 1;
                if (numberPressed <= m_Formations.Count - 1)
                {
                    m_CurrentFormationIndex = numberPressed;
                }
            }
        }
        
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
                    case FormationType.LINE:
                        m_Formations.Add(new LineFormation());
                        break;
                    case FormationType.SQUARE:
                        m_Formations.Add(new SquareFormation());
                        break;
                    case FormationType.COLUMN:
                        m_Formations.Add(new ColumnFormation());
                        break;
                    case FormationType.INVERTEDWEDGE:
                        m_Formations.Add(new InvertedWedgeFormation());
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

    public bool DeregisterUnitFromSquad(selection_component unit)
    {
        //We reached maximum squad capacity
        if (m_Members.Count <= 0)
        {
            Debug.LogWarning("WARNING: Squad is already empty!");
            return false;
        }

        //Make sure no units are added twice!
        if (m_Members.Exists(u => u.GetInstanceID() == unit.GetInstanceID()))
        {
            m_Members.Remove(unit);
            return true;
        }
        else
        {
            Debug.LogWarning("WARNING: " + unit.name + " is not in the squad!");
            return false;
        }
    }

    public Vector3 GetMemberPosition(selection_component member, out Vector3 targetPos)
    {
        Vector3 unitPos = Vector3.zero;
        targetPos = Vector3.zero;
        Vector3 respectiveUnitPos = Vector3.zero; //position in the formation
        int unitIndex = -1;

        //Find the index in members list for that unit
        unitIndex = m_Members.FindIndex(u => u.GetInstanceID() == member.GetInstanceID());

        //if unit is a member then get formation position
        if (unitIndex >= 0)
        {
            //get unit's position in the formation relative to the origin
            respectiveUnitPos = m_Formations[m_CurrentFormationIndex].GetUnitPosition(unitIndex);
        }
        else
        {
            Debug.LogError(member.name + " requested a formation position but is not a member of " + gameObject.name);
            return unitPos;
        }


        //Transforms the relative position into world space using teh VL's position as the local origin
        //Tranform Local formation pos to world space
        unitPos = transform.TransformPoint(respectiveUnitPos);


        //Use the unit's speed to project ahead on its current path and get an estimate of where the unit will soon be
        
        //Predict your optimised position relative to the formation while moving

        //Terminate scanning the path at this distance.
        float maxDistance = m_Agent.speed * 0.5f;

        NavMeshHit prediction;
        //Look ahead a specified distance
        m_Agent.SamplePathPosition(1, maxDistance, out prediction);

        //Calculate a new target position by adding the predicted position in the VL's local space to the unit's position within the formation.
        //Add respective formation position to next predicted step to 
        targetPos = transform.TransformPoint(transform.InverseTransformPoint(prediction.position) + respectiveUnitPos);

       // Debug.Log("Remaining : " + m_Agent.remainingDistance);


        return unitPos;
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
