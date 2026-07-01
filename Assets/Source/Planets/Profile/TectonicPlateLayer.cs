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
        [SerializeField] Vector2 plateSpeedMinMax = new Vector2(0.2f, 1f);

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


            public Data(TectonicPlateLayer layer)
            {
                L = layer;
                var randState = Random.state;
                Random.InitState(L.seed);
                
                plates = new Plate[L.plateCount];
                for (int i = 0; i < L.plateCount; ++i)
                {
                    Plate plate = new();
                    plate.id = i;
                    plate.center = Random.onUnitSphere;
                    plate.motionDirection = GetRandomTangentDirection(plate.center);
                    plate.speed = Random.Range(L.plateSpeedMinMax.x, L.plateSpeedMinMax.y);
                    plate.rotationAxis = Vector3.Cross(plate.center, plate.motionDirection).normalized;
                    plates[i] = plate;
                }

                Random.state = randState;
            }

            public QResult Querry(Vector3 pointOnSphere)
            {
                pointOnSphere.Normalize();

                int closestId = -1;
                int secondId = -1;

                float closestDot = -2f;
                float secondDot = -2f;

                for (int i = 0; i < plates.Length; ++i)
                {
                    float dot = Vector3.Dot(pointOnSphere, plates[i].center);
                    if (dot > closestDot)
                    {
                        secondId = closestId;
                        secondDot = closestDot;
                        closestId = plates[i].id;
                        closestDot = dot;
                    }
                    else if (dot > secondDot)
                    {
                        secondId = plates[i].id;
                        secondDot = dot;
                    }
                }

                float closestAngle = Mathf.Acos(Mathf.Clamp(closestDot, -1f, 1f));
                float secondAngle = Mathf.Acos(Mathf.Clamp(secondDot, -1f, 1f));

                Plate.EBoundary boundary = ClassifyBoundary(pointOnSphere, plates[closestId], plates[secondId]);

                return new QResult {
                    plateId = closestId,      plateDot = closestDot,
                    secondPlateId = secondId, secondPlateDot = secondDot,
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
            
            public int id;
            public Vector3 center;
            public Vector3 motionDirection;
            public Vector3 rotationAxis;
            public float   speed;

            public Vector3 GetVelocity(Vector3 pointDirection) => Vector3.Cross(rotationAxis, pointDirection) * speed;
        }

        public struct QResult
        {
            public int   plateId;
            public int   secondPlateId;
            public float plateDot;
            public float secondPlateDot;
            public Plate.EBoundary boundaryType;
            public float boundaryMarginRadians;
        }

    }
}