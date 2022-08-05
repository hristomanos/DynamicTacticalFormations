using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a wedge formation.
//It also calculates the group's centre of mass

public class LineFormation : Formation
{

    float horizontalSpacing = 2;
    float verticalSpacing;

    int m_UnitAmount = 15;

    public LineFormation() : base(FormationType.LINE)
    {
        //m_UnitAmount = 15;
       
    }

    protected override void AssignPositions()
    {
        //m_UnitPositions.Add(new Vector3(0.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(2.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(-2.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(4.0f, 0.0f, 0.0f));

        
        float offset = (m_UnitAmount - 1) * horizontalSpacing / 2f;

        //For each unit in the formation
        for (int index = 0; index < m_UnitAmount; index++)
        {
            //Add a new vector 3 on the list of positions
            //Each unit is multiplied by the spacing minus the offset (I am not sure of the maths here)
            m_UnitPositions.Add(new Vector3(index * horizontalSpacing - offset, 0, 0));
        }

        Debug.Log("LineFormation: " + m_UnitPositions.Count);

        CalculateCentreOfMass();
    }

    //The formation's centre of mass is calculated by taking the average formation position. The sum of formation position devided by the number of positions 
    void CalculateCentreOfMass()
    {
        m_ExpectedCentreOfMass = Vector3.zero;

        foreach (Vector3 pos in m_UnitPositions)
        {
            m_ExpectedCentreOfMass += pos;
        }

        m_ExpectedCentreOfMass /= m_UnitPositions.Count;
    }
}
