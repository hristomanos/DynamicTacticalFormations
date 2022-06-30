using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    //Spawn four points at the corner of the screen
    [SerializeField] GameObject m_CirclePrefab;
    
    GameObject m_TopLeftPoint;
    GameObject m_BottomLeftPoint;
    GameObject m_BottomRightPoint;
    GameObject m_TopRightPoint;


    void Start()
    {
        GetScreenBorder();
    }

    // Update is called once per frame
    void Update()
    {
        StopGettingOffScreen();
    }

    #region SCREEN BORDER

    void StopGettingOffScreen()
    {
        //Top
        if (transform.position.y > m_TopLeftPoint.transform.position.y)
        {
            transform.position = new Vector3(-transform.position.x, m_BottomLeftPoint.transform.position.y, 0);
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

        m_BottomRightPoint = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.rightBottomCorner), Quaternion.identity);
        m_TopRightPoint = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.rightTopCorner), Quaternion.identity);
        m_BottomLeftPoint = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.leftBottomCorner), Quaternion.identity);
        m_TopLeftPoint = Instantiate(m_CirclePrefab, GetWorldSpaceScreenBorder(screenBorder.leftTopCorner), Quaternion.identity);

    }

    Vector3 GetWorldSpaceScreenBorder(Vector2 screenPoint)
    {
        Vector3 worldSpacePoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
        worldSpacePoint.z = 0;

        return worldSpacePoint;
    }

    #endregion

}
