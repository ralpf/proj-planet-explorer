using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "tectonicPlateLayer", menuName = "Planet/Tectonic Plate Layer")]
    public class TectonicPlateLayer : ProfileLayer
    {
        [SerializeField] int seed = 123;
        [SerializeField] int plateCount = 9;
        [SerializeField] int ownershipMapResolution = 64;
        [SerializeField] Cubemap debugCubemap;

        ComputedData computedData;

        public Cubemap DebugCubemap => debugCubemap;


        public override void Evaluate(ref Sample sample)
        {
            throw new System.NotImplementedException();
        }

        [ContextMenu("Run Precompute")]
        public override void Precompute()
        {
            computedData = new ComputedData(seed, plateCount, ownershipMapResolution);
            debugCubemap = computedData.CreateDebugCubemap();
        }


        public class ComputedData
        {
            Plate[] plates;
            OwnershipMap map;


            public ComputedData(int seed, int plateCount, int mapResolution)
            {
                var randState = Random.state;
                Random.InitState(seed);
                
                plates = new Plate[plateCount];
                for (int i = 0; i < plateCount; ++i)
                {
                    plates[i] = new Plate();
                    plates[i].id = i;
                    plates[i].center = Random.onUnitSphere;
                }

                Random.state = randState;
                map = new OwnershipMap(mapResolution, plates);
            }

            public Cubemap CreateDebugCubemap() => map.CreateDebugCubemap(plates.Length);
        }


        public class Plate
        {
            public int id;
            public Vector3 center;
        }

        public class OwnershipMap
        {
            int resolution;
            int[] ids;
            
            public OwnershipMap(int resolution, Plate[] plates)
            {
                this.resolution = resolution;
                ids = new int[6 * resolution * resolution]; // 6 faces for cube sphere

                for (int face = 0; face < 6; ++face)        // build map
                    for (int y = 0; y < resolution; ++y)
                        for (int x = 0; x < resolution; ++x)
                        {
                            Vector3 direction = GetDirection(face, x, y);
                            ids[ GetIndex(face, x, y) ] = FindClosestPlate(direction, plates); 
                        }
            }

            public Cubemap CreateDebugCubemap(int plateCount)
            {
                Cubemap cm = new Cubemap(resolution, TextureFormat.RGBA32, false);
                cm.filterMode = FilterMode.Point;
                cm.wrapMode = TextureWrapMode.Clamp;
                FillFace(CubemapFace.PositiveX, 0);
                FillFace(CubemapFace.NegativeX, 1);
                FillFace(CubemapFace.PositiveY, 2);
                FillFace(CubemapFace.NegativeY, 3);
                FillFace(CubemapFace.PositiveZ, 4);
                FillFace(CubemapFace.NegativeZ, 5);
                cm.Apply();
                return cm;

                void FillFace(CubemapFace cubemapFace, int mapFace)
                {
                    var colors = new Color[resolution * resolution];
                    for (int y = 0; y < resolution; ++y)
                        for (int x = 0; x < resolution; ++x)
                        {
                            int id = ids[ GetIndex(mapFace, x, y) ];
                            colors[y * resolution + x] = PlateColor(id);
                        }
                    cm.SetPixels(colors, cubemapFace);
                }

                Color PlateColor(int plateId)
                {
                    float hue = plateId / (float)plateCount;
                    return Color.HSVToRGB(hue, 0.75f, 1f);
                }
            }

            private Vector3 GetDirection(int face, int x, int y)
            {
                float u = ((x + 0.5f) / resolution) * 2f - 1f;
                float v = ((y + 0.5f) / resolution) * 2f - 1f;
                return face switch
                {
                    0 => new Vector3(1f, -v, -u).normalized,   // Positive X
                    1 => new Vector3(-1f, -v, u).normalized,   // Negative X
                    2 => new Vector3(u, 1f, v).normalized,     // Positive Y
                    3 => new Vector3(u, -1f, -v).normalized,   // Negative Y
                    4 => new Vector3(u, -v, 1f).normalized,    // Positive Z
                    5 => new Vector3(-u, -v, -1f).normalized,  // Negative Z
                    _ => throw new ArgumentException(face.ToString())
                };
            }
            
            private int FindClosestPlate(Vector3 direction, Plate[] plates)
            {
                int closestID = -1;
                float closestDot = -2f;

                for (int i = 0; i < plates.Length; ++i)
                {
                    float dot = Vector3.Dot(direction, plates[i].center);
                    if (dot > closestDot)
                    {
                        closestDot = dot;
                        closestID = plates[i].id;
                    }
                }
                return closestID;
            }

            private int GetIndex(int face, int x, int y) => face * resolution * resolution + y * resolution + x;

        }
    }
}