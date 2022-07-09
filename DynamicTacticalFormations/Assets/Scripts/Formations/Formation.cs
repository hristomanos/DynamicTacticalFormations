using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This abstract class is responsible for defining what types of information formations need to be created.

public enum FormationType
{
    NULL = 0,
    WEDGE,
}


public abstract class Formation
{
    //Need to know about the formation type
    FormationType m_Type;

    //Need to know about the positions units will occupy
    protected List<Vector3> m_UnitPositions;

    //Need to know the maximum number of unit in the formation
    protected uint m_MaxUnits;

    //Need to know about the centre of the squad's mass
    protected Vector3 m_ExpectedCentreOfMass;

    //Accessors   

    public FormationType Type { get => m_Type; }

    public List<Vector3> UnitPositions { get => m_UnitPositions; }
    
    public uint MaxUnits { get => m_MaxUnits; }

    public Vector3 ExpectedCentreOfMass { get => m_ExpectedCentreOfMass; }
    
    public float ExpectedCentreOfMassOffset { get => m_ExpectedCentreOfMass.magnitude; }
    
    public float ExpectedCentreOfMassSqrMagnitude { get => m_ExpectedCentreOfMass.sqrMagnitude; }

    //Initilisation
    public Formation(uint maxUnits, FormationType formationType)
    {
        m_MaxUnits = maxUnits;
        m_Type = formationType;
        m_UnitPositions = new List<Vector3>();
        AssignPositions();
    }

    //Returns formation position for a unit in another list
    virtual public Vector3 GetUnitPosition(int unitIndex)
    {
        Vector3 unitPosition = Vector3.zero;

        if (unitIndex > m_UnitPositions.Count - 1)
        {
            Debug.LogError("ERROR: Index of unit position requested is too large!");
            Debug.Break();
            return unitPosition;
        }

        return m_UnitPositions[unitIndex];
    }


    protected abstract void AssignPositions();
   
}
