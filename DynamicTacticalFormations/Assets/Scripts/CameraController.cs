using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for handling the camera system for a strategy game.

public class CameraController : MonoBehaviour
{

    public float m_MovementSpeed;
    public float m_MovementTime;

    public Vector3 m_NewPosition;


    public float m_NormalSpeed;
    public float m_FastSpeed;

    public float m_RotationAmount;
    public Quaternion m_NewRotation;


    public Transform m_CameraTransform;
    Vector3 m_NewZoom;
    public Vector3 m_ZoomAmount;

    public Vector3 m_DragStartPos;
    public Vector3 m_DragCurrentPos;

    // Start is called before the first frame update
    void Start()
    {
        m_NewPosition = transform.position;
        m_NewRotation = transform.rotation;

        m_NewZoom = m_CameraTransform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        HandleMovementInput();
        HandleMouseInput();
    }

    void HandleMouseInput()
    {

        if (Input.mouseScrollDelta.y != 0)
        {
            m_NewZoom += Input.mouseScrollDelta.y * m_ZoomAmount;
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Plane plane = new Plane(Vector3.up, Vector3.zero);


        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    float entry;

        //    if ( plane.Raycast(ray, out entry))
        //    {
        //        m_DragStartPos = ray.GetPoint(entry);
        //    }


        //}

        //if (Input.GetMouseButton(0))
        //{
        //    Plane plane = new Plane(Vector3.up, Vector3.zero);


        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    float entry;

        //    if (plane.Raycast(ray, out entry))
        //    {
        //        m_DragCurrentPos = ray.GetPoint(entry);

        //        m_NewPosition = transform.position + m_DragStartPos - m_DragCurrentPos;
        //    }


        //}


    }


    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_MovementSpeed = m_FastSpeed;
        }
        else
        {
            m_MovementSpeed = m_NormalSpeed;
        }


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            m_NewPosition += transform.forward * m_MovementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            m_NewPosition += transform.right * m_MovementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_NewPosition += transform.right * -m_MovementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_NewPosition += transform.forward * -m_MovementSpeed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            m_NewRotation *= Quaternion.Euler(Vector3.up * m_RotationAmount); 
        }

        if (Input.GetKey(KeyCode.E))
        {
            m_NewRotation *= Quaternion.Euler(Vector3.up * -m_RotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            m_NewZoom += m_ZoomAmount;
        }

        if (Input.GetKey(KeyCode.F))
        {
            m_NewZoom -= m_ZoomAmount;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, m_NewRotation, Time.deltaTime * m_MovementTime);
        transform.position = Vector3.Lerp(transform.position, m_NewPosition, Time.deltaTime * m_MovementTime);
        m_CameraTransform.localPosition = Vector3.Lerp(m_CameraTransform.localPosition, m_NewZoom,Time.deltaTime * m_MovementTime);
    }
}
