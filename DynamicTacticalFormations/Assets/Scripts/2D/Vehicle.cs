using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds all the necessary data and behaviour required for a moving entity


public struct ScreenBorder
{
    public Vector2 rightBottomCorner;
    public Vector2 leftBottomCorner;
    public Vector2 rightTopCorner;
    public Vector2 leftTopCorner;


    public ScreenBorder(int width, int height)
    {
       rightBottomCorner = new Vector2(width, 0);
       leftBottomCorner = new Vector2(0, 0);
       rightTopCorner = new Vector2(width, height);
       leftTopCorner = new Vector2(0, height);
    }
}


public class Vehicle : MonoBehaviour
{

    Vector3 m_Velocity = Vector3.zero;

    //A normalised vector pointing at the direction the entity is heading.
    Vector2 m_Heading;

    //A vector perpendicular to the heading vector
    Vector2 m_Side;

    //Data updated every frame
    float m_Mass;

    //Maximum speed at which the entity may travel
    [Range(10,500)]
    public float m_MaxSpeed;

    //Maximum force this entity can produce to power itself
    //(Think rockets and thrust)
    float m_MaxForce;

    //The maximum rate (radians per second) at which this entity can rotate
    float m_MaxTurnRate;

    // ##################################################################################

    Rigidbody2D m_Rigidbody2D;

    

    Vector3 m_TargetPos;

    [SerializeField] GameObject TargetAgent;
    
    void Start()
    {
       TryGetComponent<Rigidbody2D>(out m_Rigidbody2D);
       
       
      m_Rigidbody2D.AddForce(transform.up * 150);
    }

    
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //   m_TargetPos = GetMouseWorldPosition();
        //}
        //transform.position = Vector3.SmoothDamp(transform.position,m_TargetPos,ref m_Velocity, m_MaxSpeed * Time.deltaTime);
    }
    private void FixedUpdate()
    {
       
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

    void Pursuit()
    {
        //Vector3 ToEvader =
    }


}
