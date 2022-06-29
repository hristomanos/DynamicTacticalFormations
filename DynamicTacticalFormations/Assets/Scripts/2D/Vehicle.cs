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

    [SerializeField] GameObject m_CirclePrefab;
    GameObject m_TopLeftPoint;
    GameObject m_BottomLeftPoint;
    GameObject m_BottomRightPoint;
    GameObject m_TopRightPoint;

    // Start is called before the first frame update
    void Start()
    {
       TryGetComponent<Rigidbody2D>(out m_Rigidbody2D);
       GetScreenBorder();
    }

    // Update is called once per frame
    void Update()
    {
        StopGettingOffScreen();
       // transform.position = Vector3.SmoothDamp(transform.position,m_TopRightPoint.transform.position,ref m_Velocity, m_MaxSpeed * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        m_Rigidbody2D.AddForce(transform.up * 4); 
    }

    void StopGettingOffScreen()
    {
        //Top
        if (transform.position.y > m_TopLeftPoint.transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, m_BottomLeftPoint.transform.position.y, 0);
        }

        //Bottom
        if (transform.position.y < m_BottomLeftPoint.transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, m_TopLeftPoint.transform.position.y, 0);
        }

        //Left
        if (transform.position.x < m_BottomLeftPoint.transform.position.x)
        {
            transform.position = new Vector3(m_BottomRightPoint.transform.position.x, transform.position.y, 0);
        }

        //Right
        if (transform.position.x > m_BottomRightPoint.transform.position.x)
        {
            transform.position = new Vector3(m_BottomLeftPoint.transform.position.x, transform.position.y, 0);
        }
    }

    void GetScreenBorder()
    {
        ScreenBorder screenBorder = new ScreenBorder(Screen.width, Screen.height);

        m_BottomRightPoint = Instantiate(m_CirclePrefab,GetWorldSpaceScreenBorder(screenBorder.rightBottomCorner),Quaternion.identity);
        m_TopRightPoint    = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.rightTopCorner), Quaternion.identity);
        m_BottomLeftPoint  = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.leftBottomCorner), Quaternion.identity);
        m_TopLeftPoint     = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.leftTopCorner), Quaternion.identity);

    }

    Vector3 GetWorldSpaceScreenBorder(Vector2 screenPoint)
    {
        Vector3 worldSpacePoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
        worldSpacePoint.z = 0;


        return worldSpacePoint;
    }

    Vector3 Arrive(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;

        float dist = toTarget.magnitude;

        if (dist > 0)
        {

        }

        return toTarget;
    }
}
