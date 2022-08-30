using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is responsible for assigning positions for a square formation.
//It also calculates the group's centre of mass

public class SquareFormation : Formation
{

    // Represents the max number of units in a single row.
    public int ColumnCount { get; private set; }

    float m_Spacing = 2;
    float m_Noise = 0.3f;

    bool m_CentreUnits = true;
    bool m_Hollow = false;

    public SquareFormation(int unitAmount) : base(unitAmount,FormationType.SQUARE)
    {

    }

    protected override void AssignPositions()
    {
        //Hard coded representation
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
        //Declare a list of 3D vectors
        List<Vector3> unitPositions = new List<Vector3>();

        //If the number of units is less than the column count.
        int unitsPerRow = Mathf.Min(ColumnCount, unitCount);
        
        //Calculate offset to have an even distribution across the X axis
        float offset = (unitsPerRow - 1) * m_Spacing / 2f;

        float x, z, column;

        for (int row = 0; unitPositions.Count < unitCount; row++)
        {
            
            //Take the first index in row by multiplying the current index by the column count
            int firstIndexInRow = row * ColumnCount;
            
            //Check if the total amount of available slots is larger than the amount of units
            int totalAmountOfSlots = firstIndexInRow + ColumnCount;
            if (m_CentreUnits && row != 0 && totalAmountOfSlots > unitCount)
            {
                // Alter the offset to center the units that do not fill the row
                int emptySlots = totalAmountOfSlots - unitCount;

                offset -= emptySlots / 2f * m_Spacing;
            }

            for (column = 0; column < ColumnCount; column++)
            {
                if (firstIndexInRow + column < unitCount)
                {
                    if (m_Hollow && (row != 0 && row != unitsPerRow - 1) && column != 0 && column != ColumnCount - 1) 
                    {
                        continue;
                    }
                    x = column * m_Spacing - offset;
                    z = row * m_Spacing;

                    Vector3 newPosition = new Vector3(x, 0, -z);

                    //newPosition += GetNoise(newPosition);

                    unitPositions.Add(newPosition);
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



    Vector3 GetNoise(Vector3 pos)
    {
        var noise = Mathf.PerlinNoise(pos.x * m_Noise, pos.z * m_Noise);

        return new Vector3(noise,0,noise);
    }
}
