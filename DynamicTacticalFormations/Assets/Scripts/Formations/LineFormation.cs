using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a single line formation.
//It also calculates the group's centre of mass

public class LineFormation : Formation
{

    float m_HorizontalSpacing = 2;
    
    public LineFormation(int unitAmount) : base(unitAmount,FormationType.LINE)
    {
       
    }

    protected override void AssignPositions()
    {
        //Hard coded representation
        //m_UnitPositions.Add(new Vector3(0.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(2.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(-2.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(4.0f, 0.0f, 0.0f));

        
        //The offset scatters the positions uniformly across the line
        float offset = (m_UnitAmount - 1) * m_HorizontalSpacing / 2f;
       

        //For each unit in the formation
        for (int index = 0; index < m_UnitAmount; index++)
        {
            //Add a new 3D vector on the list of positions
            //Each unit is multiplied by the horizontal spacing minus the offset
            m_UnitPositions.Add(new Vector3(index * m_HorizontalSpacing - offset, 0, 0));
        }

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
