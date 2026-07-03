using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "tectonicPlateLayer", menuName = "Planet/Tectonic Plate Layer")]
    public class TectonicPlateLayer : ProfileLayer
    {
        [SerializeField] int seed = 123;
        [SerializeField, Range(2, 32)] int plateCount = 9;
        [SerializeField] MinMax plateSpeedMM = new MinMax(0.2f, 1f);
        [SerializeField] float boundaryWidthRadians = 0.05f;
        [SerializeField] float rollOceanicChance = 0.3f;
        [SerializeField] float rollContinentalChance = 0.5f;
        [SerializeField] MinMax oceanicAgeMM = new MinMax(0, 1);
        [SerializeField] MinMax continentalAgeMM = new MinMax(0.5f, 1);
        [SerializeField] MinMax oceanicThicknessMM = new MinMax(0.05f, 0.2f);
        [SerializeField] MinMax continentalThicknessMM = new MinMax(0.5f, 1);

        Data data;  // pre-computed data

        public Data LayerData => data;


        public override void Evaluate(ref Sample sample)
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
            data = new Data(this);
        }


        public class Data
        {
            Plate[] plates;

            TectonicPlateLayer L { get; }
            public int PlateCount => plates.Length;


            public Data(TectonicPlateLayer layer)
            {
                L = layer;
                var randState = Random.state;
                Random.InitState(L.seed);
                
                plates = new Plate[L.plateCount];
                for (int i = 0; i < L.plateCount; ++i)
                {
                    Plate plate = new();
                    plates[i] = plate;

                    plate.center = Random.onUnitSphere;
                    plate.motionDirection = GetRandomTangentDirection(plate.center);
                    plate.speed = Random.Range(L.plateSpeedMM.min, L.plateSpeedMM.max);
                    plate.rotationAxis = Vector3.Cross(plate.center, plate.motionDirection).normalized;

                    float roll = Random.value;
                    if (roll < L.rollOceanicChance)
                    {
                        // oceanic crust
                        plate.continentalAmount = new MinMax(0, 0.15f).RollRandom;
                        plate.crustThickness = L.oceanicThicknessMM.RollRandom;
                        plate.crustAge = L.oceanicAgeMM.RollRandom;
                    }
                    else if (roll < L.rollOceanicChance + L.rollContinentalChance)
                    {
                        // continental crust
                        plate.continentalAmount = new MinMax(0.85f, 1f).RollRandom;
                        plate.crustThickness = L.continentalThicknessMM.RollRandom;
                        plate.crustAge = L.continentalAgeMM.RollRandom;
                    }
                    else
                    {
                        // mixed crust
                        plate.continentalAmount = new MinMax(0.15f, 0.85f).RollRandom;
                        plate.crustThickness = Mathf.Lerp(L.oceanicThicknessMM.RollRandom, L.continentalThicknessMM.RollRandom, plate.continentalAmount);
                        plate.crustAge = Mathf.Lerp(L.oceanicAgeMM.RollRandom, L.continentalAgeMM.RollRandom, plate.continentalAmount);
                    }
                }

                Random.state = randState;
            }

            public Plate GetPlate(int idx) => plates[idx];

            public Vector3 GetPlateVelocity(int idx, Vector3 pointOnSphere) => GetPlate(idx).GetVelocity(pointOnSphere);

            public QResult Query(Vector3 pointOnSphere)
            {
                pointOnSphere.Normalize();

                int closestIdx = -1;
                int secondIdx = -1;

                float closestDot = -2f;
                float secondDot = -2f;

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

                return new QResult {
                    plateIdx = closestIdx,      plateDot = closestDot,
                    secondPlateIdx = secondIdx, secondPlateDot = secondDot,
                    boundaryType = boundary, boundaryMarginRadians = secondAngle - closestAngle
                };
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

            private Vector3 GetRandomTangentDirection(Vector3 normal)
            {
                while (true)
                {
                    Vector3 random = Random.onUnitSphere;
                    Vector3 tangent = Vector3.ProjectOnPlane(random, normal);
                    if (tangent.sqrMagnitude > 0.0001f) return tangent.normalized;
                }
            }
        }


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

            public ECrust CrustType => continentalAmount switch {
                < 0.2f => ECrust.Oceanic,
                < 0.8f => ECrust.Mixed,
                _ => ECrust.Continental
            };

            public Vector3 GetVelocity(Vector3 pointOnSphere) => Vector3.Cross(rotationAxis, pointOnSphere) * speed;
        }



        public struct QResult
        {
            public int   plateIdx;
            public int   secondPlateIdx;
            public float plateDot;
            public float secondPlateDot;
            public Plate.EBoundary boundaryType;
            public float boundaryMarginRadians;


            public float GetBoundaryStrength01(float widthRadians)
            {
                if (widthRadians <= 0f) return 0f;
                return Mathf.Clamp01(1f - boundaryMarginRadians / widthRadians);
            }
        }

    }
}