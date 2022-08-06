using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a wedge formation. It also calculates the group's centre of mass

public class SquareFormation : Formation
{

    /// <summary>
    /// Returns the column count which represents the max
    /// unit number in a single row.
    /// </summary>
    public int ColumnCount { get; private set; }

    private float spacing = 2;
    private bool centerUnits = true;

    public SquareFormation(int unitAmount) : base(unitAmount,FormationType.SQUARE)
    {

    }

    protected override void AssignPositions()
    {
        //m_UnitPositions.Add(new Vector3(0.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(2.0f, 0.0f, 0.0f));
        //m_UnitPositions.Add(new Vector3(0.0f, 0.0f, -2.0f));
        //m_UnitPositions.Add(new Vector3(2.0f, 0.0f, -2.0f));

        ColumnCount = 3;

        //Hold a list of positions
        m_UnitPositions = GetPositions(m_UnitAmount);


        CalculateCentreOfMass();
    }

    List<Vector3> GetPositions(int unitCount)
    {
        List<Vector3> unitPositions = new List<Vector3>();
        var unitsPerRow = Mathf.Min(ColumnCount, unitCount);
        float offset = (unitsPerRow - 1) * spacing / 2f;
        float x, y, column;

        for (int row = 0; unitPositions.Count < unitCount; row++)
        {
            // Check if centering is enabled and if row has less than maximum
            // allowed units within the row.
            var firstIndexInRow = row * ColumnCount;
            if (centerUnits &&
                row != 0 &&
                firstIndexInRow + ColumnCount > unitCount)
            {
                // Alter the offset to center the units that do not fill the row
                var emptySlots = firstIndexInRow + ColumnCount - unitCount;
                offset -= emptySlots / 2f * spacing;
            }

            for (column = 0; column < ColumnCount; column++)
            {
                if (firstIndexInRow + column < unitCount)
                {
                    x = column * spacing - offset;
                    y = row * spacing;
                    unitPositions.Add(new Vector3(x, 0, -y));
                }
                else
                {
                    return unitPositions;
                }
            }
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
