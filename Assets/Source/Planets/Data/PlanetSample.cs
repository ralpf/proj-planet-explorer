using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Planets.Data.Runtime
{
    public class PlanetSample
    {
        public Vector3 pointOnSphere;
        public Tectonics tectonics;


        public PlanetSample(Vector3 pointOnSphere)
        {
            this.pointOnSphere = pointOnSphere.normalized;
        }
        
        //...................................................................INNER-CLASS

        public struct Tectonics
        {
            public int plateIdx;
            public float boundaryMarginRadians;
            public float crustThickness;
            public Plate.EBoundary boundaryType;
            public Plate.ECrust crustType;
        }
    }
}