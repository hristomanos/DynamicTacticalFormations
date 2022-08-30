using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This abstract class is responsible for defining
//what kind of data formations need to be created.

public enum FormationType
{
    NULL = 0,
    WEDGE,
    LINE,
    COLUMN,
    SQUARE,
    INVERTEDWEDGE,
}


public abstract class Formation
{
    FormationType m_Type;

    //Positions units will occupy in the formation
    protected List<Vector3> m_UnitPositions;

    //Centre of the squad's mass
    protected Vector3 m_ExpectedCentreOfMass;

    //Accessors   

    public FormationType Type { get => m_Type; }

    public List<Vector3> UnitPositions { get => m_UnitPositions; }

    public Vector3 ExpectedCentreOfMass { get => m_ExpectedCentreOfMass; }
    
    public float ExpectedCentreOfMassOffset { get => m_ExpectedCentreOfMass.magnitude; }
    
    public float ExpectedCentreOfMassSqrMagnitude { get => m_ExpectedCentreOfMass.sqrMagnitude; }

    protected int m_UnitAmount;

    //Initilisation
    public Formation(int unitAmount, FormationType formationType)
    {
       // m_MaxUnits = maxUnits;
        m_Type = formationType;
        m_UnitAmount = unitAmount;
        m_UnitPositions = new List<Vector3>();
        AssignPositions();
    }

    //Returns formation position for a unit in another list
    virtual public Vector3 GetUnitPosition(int unitIndex)
    {
        Vector3 unitPosition = Vector3.zero;

        if (unitIndex > m_UnitPositions.Count - 1)
        {
            Debug.Log("Error: index: " + unitIndex + " ||  Count: " + m_UnitPositions.Count);
            Debug.LogError("ERROR: Index of unit position requested is too large!");
            Debug.Break();
            return unitPosition;
        }

        return m_UnitPositions[unitIndex];
    }

    virtual public List<float> GetLeftSide()
    {
        List<float> leftSide = new List<float>();
        
        foreach (Vector3 position in m_UnitPositions)
        {
            if (position.x < 0)
            {
                leftSide.Add(position.x);
                Debug.Log(position.x);
            }
        }

        return leftSide;
    }

    virtual public List<float> GetRightSide()
    {
        List<float> rightSide = new List<float>();

        foreach (Vector3 position in m_UnitPositions)
        {
            if (position.x > 0)
            {
                rightSide.Add(position.x);
            }
        }

        return rightSide;
    }

    protected abstract void AssignPositions();
   
}
