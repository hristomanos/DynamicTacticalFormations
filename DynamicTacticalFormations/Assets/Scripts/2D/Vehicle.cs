using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds all the necessary data and behaviour required for a moving entity

public class Vehicle : MonoBehaviour
{
    Vector3 m_Velocity = Vector3.zero;

    [Range(10,500)]
    public float m_Force;

    Rigidbody2D m_Rigidbody2D;

    Vector3 m_TargetPos;
    
    void Start()
    {
       TryGetComponent<Rigidbody2D>(out m_Rigidbody2D);

       m_Rigidbody2D.AddForce(transform.up * m_Force);

    }

    void Update()
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

    void RotateToFaceTarget(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;
        float angle = Mathf.Atan2(toTarget.y,toTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 20);
       // transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.deltaTime * 1000);
    }


    void MoveToSpecifiedPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_TargetPos = GetMouseWorldPosition();
        }

        transform.position = Vector3.SmoothDamp(transform.position, m_TargetPos, ref m_Velocity, m_Force * Time.deltaTime);
        RotateToFaceTarget(m_TargetPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 force;

         force.y = collision.bounds.size.magnitude - collision.transform.position.y;
    }


}

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