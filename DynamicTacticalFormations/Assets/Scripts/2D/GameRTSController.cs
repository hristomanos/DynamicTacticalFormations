using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRTSController : MonoBehaviour
{

    Vector3 m_StartPosition;
    [SerializeField] LayerMask m_LayerMask;

    
    void Start()
    {
        
    }

    
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            m_StartPosition = GetMouseWorldPosition();
        }

        // Unit Selection by dragging the mouse
        if (Input.GetMouseButtonUp(0))
        {
            
            //Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(m_StartPosition,GetMouseWorldPosition());
            
        }

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
