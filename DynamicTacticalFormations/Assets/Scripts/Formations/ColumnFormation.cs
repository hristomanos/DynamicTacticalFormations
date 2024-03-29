using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a wedge formation. It also calculates the group's centre of mass

public class ColumnFormation : Formation
{

    float verticalSpacing = 2;

    

    public ColumnFormation(int unitAmount) : base(unitAmount,FormationType.COLUMN)
    {

    }

    protected override void AssignPositions()
    {
        //m_UnitPositions.Add(new Vector3(0f, 0f, 0f));
        //m_UnitPositions.Add(new Vector3(0f, 0, -2f));
        //m_UnitPositions.Add(new Vector3(0f, 0, -4f));
        //m_UnitPositions.Add(new Vector3(0f, 0, -6f));

        float offset = (m_UnitAmount - 1) * verticalSpacing / 2f;

        //For each unit in the formation
        for (int index = 0; index < m_UnitAmount; index++)
        {
            //Add a new vector 3 on the list of positions
            //Each unit is multiplied by the spacing minus the offset (I am not sure of the maths here)
            m_UnitPositions.Add(new Vector3(0, 0, index * -verticalSpacing));
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
