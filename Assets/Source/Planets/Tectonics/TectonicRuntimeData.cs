using System.Collections;
using System.Collections.Generic;
using Planets.Profiles.Settings;
using UnityEngine;



namespace Planets.Data.Runtime
{
    public class TectonicRuntimeData : LayerRuntimeData
    {
        Plate[] plates;
        
        TectonicSettings S => CastSettings<TectonicSettings>();


        public TectonicRuntimeData(TectonicSettings settings) : base(settings)
        {
            var randState = Random.state;
            Random.InitState(S.seed);

            plates = new Plate[S.plateCount];
            for (int i = 0; i < S.plateCount; ++i)
            {
                Plate plate = new();
                plates[i] = plate;

                plate.center = Random.onUnitSphere;
                plate.speed = S.plateSpeed.RollRandom;
                plate.motionDirection = GetRandomTangentDirection(plate.center);
                plate.rotationAxis = Vector3.Cross(plate.center, plate.motionDirection).normalized;

                float roll = Random.value;
                if (roll < S.rollOceanicChance)
                {
                    // oceanic crust
                    plate.continentalAmount = new MinMax(0, 0.15f).RollRandom;
                    plate.crustThickness = S.oceanicThickness.RollRandom;
                    plate.crustAge = S.oceanicAge.RollRandom;
                }
                else if (roll < S.rollOceanicChance + S.rollContinentalChance)
                {
                    // continental crust
                    plate.continentalAmount = new MinMax(0.85f, 1f).RollRandom;
                    plate.crustThickness = S.continentalThickness.RollRandom;
                    plate.crustAge = S.continentalAge.RollRandom;
                }
                else
                {
                    // mixed crust
                    plate.continentalAmount = new MinMax(0.15f, 0.85f).RollRandom;
                    plate.crustThickness = Mathf.Lerp(S.oceanicThickness.RollRandom, S.continentalThickness.RollRandom, plate.continentalAmount);
                    plate.crustAge = Mathf.Lerp(S.oceanicAge.RollRandom, S.continentalAge.RollRandom, plate.continentalAmount);
                }
            }

            Random.state = randState;
        }
        
        public Vector3 GetPlateVelocity(int idx, Vector3 pointOnSphere) => plates[idx].GetVelocity(pointOnSphere);
        
        public QResult Query(Vector3 pointOnSphere)
        {
            pointOnSphere.Normalize();

            int closestIdx = -1, secondIdx = -1;
            float closestDot = -2f, secondDot = -2f;

            for (int i = 0; i < plates.Length; ++i)
            {
                float dot = Vector3.Dot(pointOnSphere, plates[i].center);
                if (dot > closestDot)
                {
                    secondIdx = closestIdx;
                    secondDot = closestDot;
                    closestIdx = i;
                    closestDot = dot;
                }
                else if (dot > secondDot)
                {
                    secondIdx = i;
                    secondDot = dot;
                }
            }

            float closestAngle = Mathf.Acos(Mathf.Clamp(closestDot, -1f, 1f));
            float secondAngle = Mathf.Acos(Mathf.Clamp(secondDot, -1f, 1f));

            Plate.EBoundary boundary = ClassifyBoundary(pointOnSphere, plates[closestIdx], plates[secondIdx]);

            return new QResult
            {
                plateIdx = closestIdx, plateDot = closestDot,
                secondPlateIdx = secondIdx, secondPlateDot = secondDot,
                boundaryType = boundary, boundaryMarginRadians = secondAngle - closestAngle,
                crushtThickness = plates[closestIdx].crustThickness
            };
        }

        public override float Evaluate(Vector3 pointOnSphere)
        {
            var q = Query(pointOnSphere);
            return q.crushtThickness;
        }

        private Vector3 GetRandomTangentDirection(Vector3 normal)
        {
            while (true)
            {
                Vector3 random = Random.onUnitSphere;
                Vector3 tangent = Vector3.ProjectOnPlane(random, normal);
                if (tangent.sqrMagnitude > 0.0001f) return tangent.normalized;
            }
        }
        
        private Plate.EBoundary ClassifyBoundary(Vector3 direction, Plate owner, Plate neighbor)
        {
            Vector3 ownerVelocity = owner.GetVelocity(direction);
            Vector3 neighborVelocity = neighbor.GetVelocity(direction);
            Vector3 relativeVelocity = neighborVelocity - ownerVelocity;

            Vector3 separationDirection = Vector3.ProjectOnPlane(neighbor.center - owner.center, direction);
            if (separationDirection.sqrMagnitude < 0.0001f) return Plate.EBoundary.None;

            separationDirection.Normalize();
            float boundaryMotion = Vector3.Dot(relativeVelocity, separationDirection);

            if (Mathf.Abs(boundaryMotion) < 0.05f) return Plate.EBoundary.Slide;
            if (boundaryMotion > 0f) return Plate.EBoundary.Divergent;
            return Plate.EBoundary.Convergent;
        }

    }
    
    
        
    //.................................................................................CLASS
    
    public class Plate
    {
        public enum EBoundary { None, Divergent, Convergent, Slide }
        public enum ECrust    { Oceanic, Continental, Mixed }

        public float   speed;
        public Vector3 center;
        public Vector3 motionDirection;
        public Vector3 rotationAxis;
        public float   continentalAmount;       // [0,1]
        public float   crustAge;
        public float   crustThickness;

        public ECrust CrustType => GetCrustType(); 
        public Vector3 GetVelocity(Vector3 pointOnSphere) => Vector3.Cross(rotationAxis, pointOnSphere) * speed;


        private ECrust GetCrustType()
        {
            return continentalAmount switch {
                < 0.2f => ECrust.Oceanic,
                < 0.8f => ECrust.Mixed,
                _ => ECrust.Continental
            };
        }
    }


    //.................................................................................CLASS

    public struct QResult
    {
        public int   plateIdx;
        public int   secondPlateIdx;
        public float plateDot;
        public float secondPlateDot;
        public float crushtThickness;
        public Plate.EBoundary boundaryType;
        public float boundaryMarginRadians;


        public float GetBoundaryStrength01(float widthRadians)
        {
            if (widthRadians <= 0f) return 0f;
            return Mathf.Clamp01(1f - boundaryMarginRadians / widthRadians);
        }
    }
}