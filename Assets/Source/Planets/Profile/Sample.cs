using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Planets.Profiles
{
    public struct Sample
    {
        public Vector3 pointOnSphere;
        public Vector3 normal;
        public float   radius;

        public TectonicPlateBlock tectonicPlate;
        
        public Sample(Vector3 pointOnSphere, float radius)
        {
            this.pointOnSphere = pointOnSphere;
            this.radius = radius;
            this.normal = pointOnSphere.normalized;
            this.tectonicPlate = default;
        }


        public struct TectonicPlateBlock
        {
            public enum EBoundary { None, Divergent, Convergent, Slide }
            
            public bool hasData;
            public int plateID;
            public int secondPlateID;
            public float plateDistance;
            public float secondPlateDistance;
            public float boundaryStrength;
            public EBoundary boundaryType;
        }
    }
}