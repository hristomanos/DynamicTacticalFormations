using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

//This script is responsible for handling formations and the squad.
//It can:
//      * Register a unit to the squad
//      * Create formations
//      * Select a formation from input
//      * Calculate relative positions in the chosen formation for each member of the squad every frame.
//      * Switch formations depending on how narrow a path is

public enum FormationState
{
    NULL,
    BROKEN, // The formation is not formed and is not trying to form
    FORMING, // The formation is trying to form up but has not yet reached FORMED
    FORMED //All units have reached their desired positions
}


public class VirtualLeader : MonoBehaviour
{
    //A list of generated formations based on how big the squad is
    List<Formation> m_Formations;

    //An array of formation types to ensure that all types have been generated at least once
    public FormationType[] m_FormationTypes;

    //An index used to hold the current employed formation
    int m_CurrentFormationIndex;

    FormationState m_FormationState;
    
    //A nav mesh agent component to enable movement within the environment
    NavMeshAgent m_Agent;

    //A list of selected units to generate the squad
    List<SelectedComponent> m_Members;

    //A layermask used for ignoring squad units when casting rays to check for narrow paths
    [SerializeField] LayerMask m_UnitLayermask;

    bool m_CanMove;

    //Number input
    KeyCode[] keyCodes = {
            KeyCode.Alpha0,
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


    //Speed moderation
    float m_OriginalSpeed;
    float m_SpeedModifier;
    float m_MaxDrift = 1.0f;
    Vector3 centerOfMass = Vector3.zero;


    void Start()
    {
        m_Formations = new List<Formation>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Members = new List<SelectedComponent>();

        m_CurrentFormationIndex = 0;

        m_CanMove = false;

        m_SpeedModifier = 1.0f;
        m_OriginalSpeed = m_Agent.speed;

        m_FormationState = FormationState.FORMING;

        //Subscribe create and clear formations to the event. Invoke event after a squad has been selected or deselected.
        SelectionController.Instance.onUnitSelectionComplete += CreateFormations;
        SelectionController.Instance.onClearFormations += ClearFormations;

    }


    private void OnDestroy()
    {
        //Unscubscribe functions from the event if gameobject gets destroyed
        SelectionController.Instance.onUnitSelectionComplete -= CreateFormations;
        SelectionController.Instance.onClearFormations -= ClearFormations;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && m_CanMove)
        {
            m_Agent.SetDestination(GetMouseWorldPosition());
        }

        //Get number from keyboard and switch formation
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                int numberPressed = i + 1;
                if (numberPressed <= m_Formations.Count - 1)
                {
                    m_CurrentFormationIndex = numberPressed;
                }
                else if (Input.GetKey(keyCodes[4]))
                {
                    m_CurrentFormationIndex = 0;
                }
            }
        }

        

        if (m_Members.Count > 0)
        {
            //Update UI
            UIManager.Instance.UpdateCurrentFormationUI(m_Formations[m_CurrentFormationIndex].Type);
            UIManager.Instance.UpdateUnitsInFormation(m_Members.Count);
            UIManager.Instance.UpdateCurrentFormationState(m_FormationState);

            //TODO: Need to change state from FORMED TO BROKEN TO FORMING

            
            if (m_FormationState == FormationState.BROKEN)
            {
                m_FormationState = FormationState.FORMING;
            }

            //CheckSpeed();

          //  Debug.Log("State: " + m_FormationState);
            //Debug.Log("can Move: " + m_CanMove);
            switch (m_FormationState)
            {
                case FormationState.FORMING: //Trying to form up but has not yet reached
                    Debug.Log("Forming!");
                    FormUp();
                    break;
                case FormationState.FORMED:
                   
                    break;
                default:
                    break;
            }
        }
        else
        {
            m_CanMove = false;
            m_FormationState = FormationState.BROKEN;
            UIManager.Instance.UpdateCurrentFormationUI(FormationType.NULL);
            UIManager.Instance.UpdateUnitsInFormation(0);
            UIManager.Instance.UpdateCurrentFormationState(FormationState.NULL);
        }


        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    m_CanMove = false;
        //    m_Agent.transform.LookAt(m_Agent.destination);
        //    m_Agent.isStopped = true;
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    m_CanMove = true;
        //    m_Agent.transform.LookAt(m_Agent.destination);
        //    m_Agent.isStopped = false;
        //}

        
    }

    private void FixedUpdate()
    {
        CheckSides(); 
    }

    //Add unit to squad by checking its instance id first
    public bool RegisterUnitToSquad(SelectedComponent unit)
    {
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

    //Remove unit from squad
    public bool DeregisterUnitFromSquad(SelectedComponent unit)
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


    //Populate list of formations by checking that all formation are in the list once
    //And storing the maximum number of units for each formation
    void CreateFormations(int unitAmount)
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
                        m_Formations.Add(new WedgeFormation(unitAmount));
                        break;
                    case FormationType.LINE:
                        m_Formations.Add(new LineFormation(unitAmount));
                        break;
                    case FormationType.SQUARE:
                        m_Formations.Add(new SquareFormation(unitAmount));
                        break;
                    case FormationType.COLUMN:
                        m_Formations.Add(new ColumnFormation(unitAmount));
                        break;
                    case FormationType.INVERTEDWEDGE:
                        m_Formations.Add(new InvertedWedgeFormation(unitAmount));
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

    }

    void ClearFormations()
    {
        m_Formations.Clear();
        Debug.Log("Formations cleared! " + m_Formations.Count);
    }

    //Gets relative position in the formation for each squad unit at each frame.
    public Vector3 GetMemberPosition(SelectedComponent member, out Vector3 targetPos)
    {
        Vector3 unitPos = Vector3.zero;
        targetPos = Vector3.zero;
        Vector3 respectiveUnitPos = Vector3.zero; //position in the formation
        int unitIndex = -1;

        //Find the index in members list for that unit 
        unitIndex = m_Members.FindIndex(u => u.GetInstanceID() == member.GetInstanceID());

        

        //if passed unit is a member then get formation position
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

    //Check if you are traversing a narrow path
    void CheckSides()
    {
        RaycastHit rightHit;
        RaycastHit leftHit;
      
        int maxDistance = 5;

        bool rightSide = false;
        bool leftSide = false;

        //Right
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out rightHit, maxDistance, m_UnitLayermask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * rightHit.distance, Color.yellow, m_UnitLayermask);
            rightSide = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * maxDistance, Color.white, m_UnitLayermask);
            rightSide = false;
        }

        //Left
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), out leftHit, maxDistance, m_UnitLayermask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * leftHit.distance, Color.yellow, m_UnitLayermask);
            leftSide = true;
           
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * maxDistance, Color.white, m_UnitLayermask);
            leftSide = false;
        }

        if (leftSide && rightSide)
        {
            float averageDistance = leftHit.distance + rightHit.distance / 2;

            if (averageDistance <= 2)
            {
                m_CurrentFormationIndex = 3; //Column
            }
            else if (averageDistance >= 2)
            {
                m_CurrentFormationIndex = 2; //Square
            }
        }


    }

    /// <summary>
    /// Modifies the leader's speed to keep it from getting ahead of the formation
    /// </summary>
    /// 
    private void CheckSpeed()
    {
        centerOfMass = Vector3.zero;
        //calculate squad's center of mass
        foreach (SelectedComponent member in m_Members)
        {
            centerOfMass += member.transform.position;
        }
        centerOfMass /= m_Members.Count;



        float distFromCM = (centerOfMass - transform.position).sqrMagnitude - m_Formations[m_CurrentFormationIndex].ExpectedCentreOfMassSqrMagnitude; //distance from center of mass

        m_SpeedModifier = (m_MaxDrift * m_MaxDrift - distFromCM) / (m_MaxDrift * m_MaxDrift);

        m_Agent.speed = m_OriginalSpeed * m_SpeedModifier; //calculate new speed based on modifier
        Debug.Log(m_Agent.speed + " " + m_SpeedModifier);
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(centerOfMass,new Vector3(0.5f,0.5f,0.5f));
    }

    public void FormUp()
    {
        ////Make sure no units are added twice!
        //if (m_Members.Exists(u => u.GetInstanceID() == unit.GetInstanceID()))
        //{
        //    // unit.m_PositionReached;
        //    //return true;
        //}
        //else
        //{
        //    Debug.LogWarning("WARNING: " + unit.name + " is not in the squad!");
        //    //return false;
        //}


        //Debug.Log(m_Members.Any(c => c.m_PositionReached == true));

        if (m_Members.Any(c => c.m_PositionReached == true))
        {
            m_CanMove = true;
            Debug.Log("Formed!");
            m_FormationState = FormationState.FORMED;
            //Enable movement
        }

    }    

    void Wheel()
    {
        /* TODO:
             * Check that VL is turning
             * Stop movement
             * Rotate formation
             * State: Forming
             * Wait for all units
             * Formed
             * Enable movement
             */

        //Stop moving
        m_Agent.isStopped = true;
        m_Agent.transform.LookAt(m_Agent.destination);


        
    }

}
