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
    BROKEN,  //The formation is not formed and is not trying to form
    FORMING, //The formation is trying to form up but has not yet reached FORMED
    FORMED   //All units have reached their desired positions
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

    //AI agent's destination
    Vector3 m_Destination;

    //A list of selected units to generate the squad
    List<SelectedComponent> m_Members;

    //A layermask used for ignoring squad units when casting rays to check for narrow paths
    [SerializeField] LayerMask m_UnitLayermask;

    //A flag for waiting all units to form the formation before being able to move
    bool m_CanMove;

    //Show path with a line renderer
    LineRenderer m_LineRenderer;

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


    //Speed adjusment
    float m_OriginalSpeed;
    float m_SpeedModifier;
    float m_MaxDrift = 8.0f;
    Vector3 centerOfMass = Vector3.zero;

    //Wheel
   public bool m_isWheeling;

    void Start()
    {
        m_Formations = new List<Formation>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Members = new List<SelectedComponent>();
        m_LineRenderer = GetComponent<LineRenderer>();

        m_CurrentFormationIndex = 2;

        m_CanMove = false;

        m_SpeedModifier = 1.0f;
        m_OriginalSpeed = m_Agent.speed;

        m_FormationState = FormationState.FORMING;

        m_isWheeling = false;

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
        MoveAgentWithMouseInput();
        Wheel();
        SwitchFormationNumInput();

       

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

            //CheckSpeed(m_Agent);

            //Debug.Log("State: " + m_FormationState);
            //Debug.Log("can Move: " + m_CanMove);
            switch (m_FormationState)
            {
                case FormationState.FORMING: //Trying to form up but has not yet reached
                    //Debug.Log("Forming!");
                    FormUp();
                    break;
                case FormationState.FORMED:
                    //StayTogether()
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

    }

    private void FixedUpdate()
    {
        // CheckSides(); 
    }

    bool m_hasSetDestination = false;

    void MoveAgentWithMouseInput()
    {
        if (Input.GetMouseButtonDown(1) && m_CanMove)
        {
            m_Destination = GetMouseWorldPosition();
            m_Agent.SetDestination(m_Destination);
            m_hasSetDestination = true;
        }

    }

    void SwitchFormationNumInput()
    {
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


        //Transforms the relative position into world space using the VL's position as the local origin
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
        //Add respective formation position to next predicted step
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
    public void CheckSpeed(NavMeshAgent agent)
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

        agent.speed = m_OriginalSpeed * m_SpeedModifier; //calculate new speed based on modifier
        
        // Debug.Log(agent.speed + " " + m_SpeedModifier);

    }

    private void OnDrawGizmos()
    {
        //Allocated positions
        Gizmos.DrawWireSphere(m_Destination, 0.5f);
        // Gizmos.DrawCube(centerOfMass,new Vector3(1f,1f,1f));
    }

    public void FormUp()
    {
        if (m_Members.Any(c => c.m_PositionReached == true))
        {
            m_CanMove = true;
            Debug.Log("Formed!");
            m_FormationState = FormationState.FORMED;
        }
    }


    void RenderLine(int positionCount, Vector3[] positions)
    {
        if (m_Agent.hasPath)
        {
            m_LineRenderer.positionCount = positionCount;
            m_LineRenderer.SetPositions(positions);
            m_LineRenderer.enabled = true;
        }
        else
        {
            m_LineRenderer.enabled = false;
        }
    }

    int m_PathIndex = 1;
    float m_WheelRadius = 5;
    Vector3[] pathCorners = new Vector3[0];

    public void Wheel()
    {
        /* Aprroach from Age of empires article
             * Check that VL is turning
             * Stop movement
             * Rotate formation
             * State: Forming
             * Wait for all units
             * Formed
             * Enable movement
             */

        if (m_Agent.path != null && m_Agent.path.corners.Length > 1)
        {

            if (m_hasSetDestination)
            {
                pathCorners = m_Agent.path.corners;
                m_hasSetDestination = false;
            }

            Debug.Log("corners.Length: " + pathCorners.Length);
            for (int i = 1; i < pathCorners.Length; i++)
            {
                NavMeshHit hit;
                bool result = NavMesh.FindClosestEdge(pathCorners[i], out hit, NavMesh.AllAreas);
                if (result && hit.distance < m_WheelRadius)
                    pathCorners[i] = hit.position + hit.normal * m_WheelRadius;
            }

            if (m_PathIndex >= pathCorners.Length - 1)
            {
                m_PathIndex = pathCorners.Length - 1;
            }


            float distance = (pathCorners[m_PathIndex] - m_Agent.transform.position).magnitude;
            Debug.Log("Distance: " + distance);
            if (distance <= m_Agent.stoppingDistance + 0.5f)
            {
                Debug.Log("Index change!");
                m_PathIndex = Mathf.Clamp(m_PathIndex + 1, 0, pathCorners.Length - 1);
            }


            RenderLine(pathCorners.Length, pathCorners);

            Debug.Log("Index: " + m_PathIndex);
            m_Agent.SetDestination(pathCorners[m_PathIndex]);
            
        }

    }
}
