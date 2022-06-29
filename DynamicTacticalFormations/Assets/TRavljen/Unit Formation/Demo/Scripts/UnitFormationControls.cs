using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRavljen.UnitFormation.Formations;
using TRavljen.UnitFormation;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitFormationControls : MonoBehaviour
{

    #region Public Properties

    /// <summary>
    /// List of units in the scene
    /// </summary>
    public List<GameObject> units = new List<GameObject>();

    /// <summary>
    /// Specifies the layer mask used for mouse point raycasts in order to
    /// find the drag positions in world/scene.
    /// </summary>
    public LayerMask groundLayerMask;

    /// <summary>
    /// Specifies the line renderer used for rendering the mouse drag line
    /// that indicates the unit facing direction.
    /// </summary>
    public LineRenderer LineRenderer;

    /// <summary>
    /// Specifies the unit count that will be generated for the scene.
    /// May be adjusted in realtime.
    /// </summary>
    public Slider UnitCountSlider;

    /// <summary>
    /// Specifies the unit spacing that will be used to generate formation
    /// positions.
    /// </summary>
    public Slider UnitSpacingSlider;

    /// <summary>
    /// Specifies the <see cref="Text"/> used to represent the unit count
    /// selected by <see cref="UnitCountSlider"/>.
    /// </summary>
    public Text UnitCountText;

    /// <summary>
    /// Specifies the <see cref="Text"/> used to represent the unit spacing
    /// selected by <see cref="UnitSpacingSlider"/>.
    /// </summary>
    public Text UnitSpacingText;

    public GameObject UnitPrefab = null;

    #endregion

    #region Private Properties

    private IFormation currentFormation;

    private bool isDragging = false;

    private int unitCount => (int)UnitCountSlider.value;
    private int unitSpacing => (int)UnitSpacingSlider.value;

    #endregion

    private void Start()
    {
        LineRenderer.enabled = false;

        //Starts off with a line formation
        SetUnitFormation(new LineFormation(unitSpacing));

        // Initial UI update
        UpdateUnitCountText();
        UpdateUnitSpacing();
    }

    private void Update()
    {
        //If the number of game objects is less than the desired unit count
        if (units.Count < unitCount)
        {
            //Setting the index one after the last element in the list and if it is less than the desired unit count
            for (int index = units.Count; index < unitCount; index++)
            {
                //Instantiate a new game object
                var gameObject = Instantiate(UnitPrefab, transform.position, Quaternion.identity);

                //And insert it in the list
                units.Insert(index, gameObject);
            }

            ApplyCurrentUnitFormation();
        }
        //Else if the number of spawned units is greater than the desired unit count
        else if (units.Count > unitCount)
        {
            //Set the index one the last element on the list and if it is greater or equal to the desired unit count
            for (int index = units.Count - 1; index >= unitCount; index--)
            {
                //Get the reduntant unit 
                var gameObject = units[index];

                //Destroy its game object
                Destroy(gameObject);

                //Remove it from the list
                units.RemoveAt(index);
            }

            ApplyCurrentUnitFormation();
        }

        //If there are units in the world, handle unit movement input
        if (units.Count > 0)
        {
            HandleMouseDrag();
        }
    }

    private void HandleMouseDrag()
    {
        //If the right mouse button is down but not dragging
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //Cast a screen point to ray from current mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //If the raycast hits the ground
            if (Physics.Raycast(ray, out RaycastHit hit, 100, groundLayerMask))
            {

                LineRenderer.enabled = true;
                isDragging = true;

                //Set positions
                LineRenderer.SetPosition(0, hit.point);
                LineRenderer.SetPosition(1, hit.point); //Set both vertices to be hit point 
            }
        }
        //If the right mouse button is held down and dragging
        else if (Input.GetKey(KeyCode.Mouse1) & isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, groundLayerMask))
            {

                //If it is dragging then set the second vertex to be at the hit point?
                //I am not sure why he does that???????????????????????????????????????
                LineRenderer.SetPosition(1, hit.point);

            }
        }
        //If the right mouse button is released and was dragging
        if (Input.GetKeyUp(KeyCode.Mouse1) && isDragging)
        {
            //Reset flags
            isDragging = false;
            LineRenderer.enabled = false;

            //Apply formation
            ApplyCurrentUnitFormation();
        }
    }


    private void ApplyCurrentUnitFormation()
    {
        //Store the direction vector in which the formation is facing
        var direction = LineRenderer.GetPosition(1) - LineRenderer.GetPosition(0);

        //A data structure that represents new formation positions and angles
        UnitsFormationPositions formationPos;

        // Check if mouse drag was NOT minor, then we can calculate angle
        // (direction) for the mouse drag.
        if (direction.magnitude > 0.8f)
        {
            //Calculate the direction's angle to help with formation's orientation
            var angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            //Debug.Log("1: " + LineRenderer.GetPosition(1));
            //Debug.Log("0: " + LineRenderer.GetPosition(0));
            //Debug.Log("Direction: " + direction);
            //Debug.Log("Angle: " + angle);


            //Get new positions by entering the current unit count,
            //currentFormation initilised at Start(),
            //first mouse point and the direction's angle
            var newPositions = FormationPositioner.GetAlignedPositions(
                units.Count, currentFormation, LineRenderer.GetPosition(0), angle);

            //Instantiate a new UnitFormationPositions object by passing in the constructor the new positions and angle
            formationPos = new UnitsFormationPositions(newPositions, angle);
        }
        else
        {
            //If the mouse is not dragged then convert the game object list to a vector 3 by passsing the each game object's position
            var currentPositions = units.ConvertAll(obj => obj.transform.position);

            //Get new positions by passing the current position of each object,
            //is that to make them move?
            formationPos = FormationPositioner.GetPositions(
                currentPositions, currentFormation, LineRenderer.GetPosition(0));
        }

        //For all spawned units
        for (int index = 0; index < units.Count; index++)
        {
            //Try to get navmeshagent component (If it does not exist, don't return an error)
            //Set the destination of each agent to its allocated position.
            if (units[index].TryGetComponent(out NavMeshAgent agent))
                agent.destination = formationPos.UnitPositions[index];
        }
    }

    private void SetUnitFormation(IFormation formation)
    {
        currentFormation = formation;
        ApplyCurrentUnitFormation();
    }

    #region User Interactions

    public void LineFormationSelected() =>
        SetUnitFormation(new LineFormation(unitSpacing));

    public void CircleFormationSelected() =>
        SetUnitFormation(new CircleFormation(unitSpacing));

    public void TriangleFormationSelected() =>
        SetUnitFormation(new TriangleFormation(unitSpacing));

    public void RectangleFormationFirstConfigSelected() =>
        SetUnitFormation(new RectangleFormation(5, unitSpacing));

    public void RectangleFormationSecondConfigSelected() =>
        SetUnitFormation(new RectangleFormation(2, unitSpacing));

    public void UpdateUnitCountText()
    {
        UnitCountText.text = "Unit Count: " + unitCount;
    }

    public void UpdateUnitSpacing()
    {
        UnitSpacingText.text = "Unit Spacing: " + unitSpacing;

        if (currentFormation is LineFormation)
        {
            currentFormation = new LineFormation(unitSpacing);
        }
        else if (currentFormation is RectangleFormation rectangleFormation)
        {
            currentFormation = new RectangleFormation(
                rectangleFormation.ColumnCount, unitSpacing);
        }
        else if (currentFormation is CircleFormation)
        {
            currentFormation = new CircleFormation(unitSpacing);
        }
        else if (currentFormation is TriangleFormation)
        {
            currentFormation = new TriangleFormation(unitSpacing);
        }

        ApplyCurrentUnitFormation();
    }

    #endregion

}
