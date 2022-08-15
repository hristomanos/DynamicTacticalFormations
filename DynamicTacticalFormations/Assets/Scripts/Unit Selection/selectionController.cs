using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///This script is responsible for receiving mouse input and selecting units by adding to the dictionary
///or rubber band them by generating a 2D rectangular mesh with a mouse drag.
///
/// it can:
///     * Receive mouse input when clicked, dragged and released.
///     * Cast a ray and add found gameObject to the dictionary
///     * Generate and draw a 2D rectangular mesh.
///     * Cast four rays from each corner of the 2D rect to the ground.
///     * Generate 3D rect collider on the ground from those four rays.
///     * Add gameObjects that overlap with the 3D collider to the dictionary.
///     * make inclusive and exlusive selection using the left shift key.
/// </summary>

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance { get; private set; }
    
    [SerializeField] LayerMask m_LayerMask;
    
    SelectedDictionary m_SelectedTable;
    RaycastHit m_Hit;
    bool m_DragSelect;
    float m_MouseDragThreshold;

    public SelectedDictionary SelectedTable { get; }

    //================= Collider variables =========================//

    MeshCollider m_SelectionBox;
    Mesh m_SelectionMesh;

    Vector3 m_MousePoint1;
    Vector3 m_MousePoint2;

    //The corners of our 2d selection box
    Vector2[] m_Corners;

    //The vertices of our meshcollider
    Vector3[] m_Verts;
    Vector3[] m_Vecs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {        
        m_SelectedTable = GetComponent<SelectedDictionary>();
        m_DragSelect = false;
        m_MouseDragThreshold = 40;
      
    }

    public event Action<int> onUnitSelectionComplete;
    public void UnitSelectectionComplete(int unitAmount)
    {
        if (onUnitSelectionComplete != null)
        {
            onUnitSelectionComplete(unitAmount);
        }
    }

    public event Action onClearFormations;
    public void ClearFormations()
    {
        if (onClearFormations != null)
        {
            onClearFormations();
        }
    }

    void Update()
    {
        //If left mouse button clicked (but not released)
        if (Input.GetMouseButtonDown(0))
        {
            m_MousePoint1 = Input.mousePosition;
        }

        //While left mouse button is held
        if (Input.GetMouseButton(0))
        {
            if((m_MousePoint1 - Input.mousePosition).magnitude > m_MouseDragThreshold)
            {
                m_DragSelect = true;
            }
        }

        //When left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            //Single select
            if (m_DragSelect == false) 
            {
                Ray ray = Camera.main.ScreenPointToRay(m_MousePoint1);

                if(Physics.Raycast(ray,out m_Hit, 50000.0f, m_LayerMask))
                {
                    //Inclusive select - Select multiple units with each click
                    if (Input.GetKey(KeyCode.LeftShift)) 
                    {
                        m_SelectedTable.AddSelected(m_Hit.transform.gameObject);
                    }
                    else //exclusive select - Select one unit
                    {
                       // ClearFormations();
                        m_SelectedTable.DeselectAll();
                        m_SelectedTable.AddSelected(m_Hit.transform.gameObject);
                    }
                }
                else //if we didnt hit something
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //do nothing because other units might already be selected. Prevents inconvenience.
                    }
                    else
                    {
                        m_SelectedTable.DeselectAll();
                      //  ClearFormations();
                    }
                }
            }
            else //marquee select
            {
                m_Verts = new Vector3[4];
                m_Vecs = new Vector3[4];
                int i = 0;
                m_MousePoint2 = Input.mousePosition;
                m_Corners = getBoundingBox(m_MousePoint1, m_MousePoint2);

                foreach (Vector2 corner in m_Corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner);

                    if (Physics.Raycast(ray, out m_Hit, 50000.0f, (1 << 8)))
                    {
                        m_Verts[i] = new Vector3(m_Hit.point.x, m_Hit.point.y, m_Hit.point.z);
                        m_Vecs[i] = ray.origin - m_Hit.point;
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), m_Hit.point, Color.red, 1.0f);
                    }
                    i++;
                }

                //generate the mesh
                m_SelectionMesh = generateSelectionMesh(m_Verts,m_Vecs);

                m_SelectionBox = gameObject.AddComponent<MeshCollider>();
                m_SelectionBox.sharedMesh = m_SelectionMesh;
                m_SelectionBox.convex = true;
                m_SelectionBox.isTrigger = true;

                StartCoroutine(CreateFormations());

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    ClearFormations();
                    m_SelectedTable.DeselectAll();
                }

                Destroy(m_SelectionBox, 0.02f);

                
            }//end marquee select

            m_DragSelect = false;

        }


      
    }

    private void OnGUI()
    {
        //Draw rectangle on game screen
        if(m_DragSelect == true)
        {
            var rect = Utils.GetScreenRect(m_MousePoint1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }


    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] getBoundingBox(Vector2 p1,Vector2 p2)
    {
        // Min and Max to get 2 corners of rectangle regardless of drag direction.
        var bottomLeft = Vector3.Min(p1, p2);
        var topRight = Vector3.Max(p1, p2);

        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right;
        Vector2[] corners =
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y),
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };
        return corners;

    }

    //generate a mesh from the 4 bottom points
    Mesh generateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

        for(int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for(int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }


    private void OnTriggerEnter(Collider other)
    {
        m_SelectedTable.AddSelected(other.gameObject);
    }

    IEnumerator CreateFormations()
    {
        yield return new WaitForFixedUpdate();
        Debug.Log("Before UnitSelectionComplete: " + m_SelectedTable.g_SelectedTable.Count);
        UnitSelectectionComplete(m_SelectedTable.g_SelectedTable.Count);
    }

}
