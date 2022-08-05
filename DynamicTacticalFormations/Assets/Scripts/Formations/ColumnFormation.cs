using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a wedge formation. It also calculates the group's centre of mass

public class ColumnFormation : Formation
{
    public ColumnFormation() : base(FormationType.COLUMN)
    {

    }

    protected override void AssignPositions()
    {
        m_UnitPositions.Add(new Vector3(0f, 0f, 0f));
        m_UnitPositions.Add(new Vector3(0f, 0, -2f));
        m_UnitPositions.Add(new Vector3(0f, 0, -4f));
        m_UnitPositions.Add(new Vector3(0f, 0, -6f));

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
