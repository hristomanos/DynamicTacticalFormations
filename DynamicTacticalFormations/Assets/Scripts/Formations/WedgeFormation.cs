using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a wedge formation. It also calculates the group's centre of mass

public class WedgeFormation : Formation
{
    private float m_Spacing = 2;
    private bool m_CenterUnits = true;

    

    public WedgeFormation(int unitAmount) : base(unitAmount,FormationType.WEDGE)
    {
       
    }

    protected override void AssignPositions()
    {
        //m_UnitPositions.Add(new Vector3(0.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(-2.0f, 0.0f, -1.0f));
        //m_UnitPositions.Add(new Vector3(2.0f, 0.0f, -1.0f));
        //m_UnitPositions.Add(new Vector3(-4.0f, 0.0f, -2.0f));

       // Debug.Log("Inside wedge formation: " + m_UnitAmount);

        m_UnitPositions = GetPositions(m_UnitAmount);

        CalculateCentreOfMass();
    }


    public List<Vector3> GetPositions(int unitCount)
    {
        List<Vector3> unitPositions = new List<Vector3>();

        // Offset starts at 0, then each row is applied change for half of spacing
        float currentRowOffset = 0f;
        float x, z;

        //For each unit in the formation
        for (int row = 0; unitPositions.Count < unitCount; row++)
        {
            // Current unit positions are the index of first unit in row
            int columnsInRow = row + 1;
            int firstIndexInRow = unitPositions.Count;

            for (int column = 0; column < columnsInRow; column++)
            {
                x = column * m_Spacing + currentRowOffset;
                z = row * m_Spacing;

                // Check if centering is enabled and if row has less than maximum
                // allowed units within the row.
                if (m_CenterUnits &&
                    row != 0 &&
                    firstIndexInRow + columnsInRow > unitCount)
                {
                    // Alter the offset to center the units that do not fill the row
                    int emptySlots = firstIndexInRow + columnsInRow - unitCount;
                    x += emptySlots / 2f * m_Spacing;
                }

                unitPositions.Add(new Vector3(x, 0, -z));

                //If the positions exceed the number of units stop
                if (unitPositions.Count >= unitCount) break;
            }

            //Make them come closer by half since there is an extra unit at each row
            currentRowOffset -= m_Spacing / 2;
        }

        return unitPositions;
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
