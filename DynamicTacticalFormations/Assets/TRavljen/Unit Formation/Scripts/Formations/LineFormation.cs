using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRavljen.UnitFormation.Formations
{

    /// <summary>
    /// Formation that positions units in a straight line 
    /// with specified spacing.
    /// </summary>
    public struct LineFormation : IFormation
    {
        private float spacing;

        /// <summary>
        /// Instantiates line formation.
        /// </summary>
        /// <param name="spacing">Specifies spacing between units.</param>
        public LineFormation(float spacing)
        {
            this.spacing = spacing;
        }

        //Pass in a number of units and get a number of positions for each unit
        public List<Vector3> GetPositions(int unitCount)
        {
            //Initialise the list of positions
            List<Vector3> unitPositions = new List<Vector3>();

            //No idea on how the offset was calculated yet??????????????????????
            float offset = (unitCount-1) * spacing / 2f;

            //For each unit in the formation
            for (int index = 0; index < unitCount; index++)
            {
                //Add a new vector 3 on the list of positions
                //Each unit is multiplied by the spacing minus the offset (I am not sure of the maths here)
                unitPositions.Add(new Vector3(index * spacing - offset, 0, 0));
            }

            return unitPositions;
        }
    }

}
