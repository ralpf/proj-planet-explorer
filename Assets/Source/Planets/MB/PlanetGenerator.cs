using System;
using System.Collections;
using System.Collections.Generic;
using Planets.DataBuffers;
using UnityEngine;
using Planets.Profiles;
using Planets.Topology;
using Extensions.UnityAPI;



namespace Planets.MB
{
    [RequireComponent(typeof(PlanetChunkPool))]
    public class PlanetGenerator : MonoBehaviour
    {
        [SerializeField] PlanetChunkPool chunkPool;
        [SerializeField] PlanetProfile profile;
        [SerializeField, Range(2, 128)] int resolution = 16;
        [SerializeField] Material material;
        [SerializeField] Material[] testMats;

        [SerializeField, Space(20)] Transform observerXf;
        [SerializeField] int maxSubdivisions = 4;
        [SerializeField] float subdivisionDistance = 100f;
        [SerializeField] float subdivisionHysteresis = 20f;
        
        
        
        public PlanetRootNode PlanetNode { get; private set; }


        void OnValidate()
        {
            Debug.Assert(chunkPool, "Planet chunk pool not set in inspector");
        }

        void Update()
        {
            UpdateSubdivisions();
        }

        void UpdateSubdivisions()
        {
            if (PlanetNode == null) Generate();
            bool dirty = false;

            foreach (PlanetFaceNode faceNode in PlanetNode.Faces)
            {
                dirty |= EvaluateChunk(faceNode.RootChunk);
            }

            if (dirty) RebuildRenderChunkObjects();

            bool EvaluateChunk(PlanetChunkNode chunk)	// => local method
            {
                float distance = DistanceFromChunkToObserver(chunk);
                if (chunk.IsSubdivided)
                {
                    if (ShouldCollapse(chunk, distance))
                    {
                        chunk.Collapse();
                        return true;
                    }

                    bool dirty = false;

                    foreach (PlanetChunkNode childChunk in chunk.Children)
                        dirty |= EvaluateChunk(childChunk);      // recursive call

                    return dirty;
                }
                // not subdivided chunk
                if (ShouldSubdivide(chunk, distance))
                {
                    chunk.Subdivide();
                    return true;
                }

                return false;
            }

            float DistanceFromChunkToObserver(PlanetChunkNode chunk)
            {
                Vector3 localCenter = chunk.LocalCenterNormal * profile.Radius;
                Vector3 worldCenter = transform.TransformPoint(localCenter);
                return Vector3.Distance(observerXf.position, worldCenter);
            }

            bool ShouldSubdivide(PlanetChunkNode chunk, float distance)
            {
                if (chunk.SubdivisionLevel >= maxSubdivisions) return false;
                return distance < SplitDistance(chunk.SubdivisionLevel);
            }

            bool ShouldCollapse(PlanetChunkNode chunk, float distance)
            {
                if (chunk.IsSubdivided == false) return false;
                return distance > CollapseDistance(chunk.SubdivisionLevel);
            }

            float SplitDistance(int subdivisionLevel)
            {
                return (maxSubdivisions - subdivisionLevel) * subdivisionDistance;
            }

            float CollapseDistance(int subdivisionLevel)
            {
                return SplitDistance(subdivisionLevel) + subdivisionHysteresis;
            }
        }

        void RebuildRenderChunkObjects()
        {
            foreach (Transform xf in this.transform)
            {
                if (xf.TryGetComponent<PlanetChunk>(out var cmp))
                    chunkPool.Release(cmp);
            }

            foreach (PlanetFaceNode faceNode in PlanetNode.Faces)
            {
                foreach (PlanetChunkNode chunk in faceNode.RootChunk.LeafChunks())
                {
                    var data = this.CreateChunkData(chunk);
                    var uobj = chunkPool.Rent();
                    uobj.Recalculate(data);
                    uobj.SetMaterial(testMats[chunk.SubdivisionLevel]);
                }
            }
        }
        


        [ContextMenu("GENERATE")]
        public void Generate()
        {
            this.Clear();
            // make logical nodes
            PlanetNode = new PlanetRootNode();
            PlanetNode.GenerateFaces();
            // make unity objects
            foreach (PlanetFaceNode faceNode in PlanetNode.Faces)
            {
                faceNode.GenerateRootChunk();
                var chunkData = this.CreateChunkData(faceNode.RootChunk);

                PlanetChunk chunkObject = chunkPool.Rent();
                chunkObject.Recalculate(chunkData);
                chunkObject.SetMaterial(material);
            }
        }

        [ContextMenu("CLEAR")]
        public void Clear()
        {
            this.transform.DestroyChildren();
            chunkPool.Clear();
        }

        // public void TestSubdivide()
        // {
        //     Clear();
        //     foreach (PlanetFaceNode faceNode in PlanetNode.Faces)
        //         faceNode.TestSubdivide();
        //
        //     foreach (PlanetFaceNode faceNode in PlanetNode.Faces)
        //     {
        //         foreach (PlanetChunkNode chunk in faceNode.RootChunk.LeafChunks())
        //         {
        //             var data = this.CreateChunkData(chunk);
        //             var uobj = chunkPool.Rent();
        //             uobj.Recalculate(data);
        //             uobj.SetMaterial(material);
        //         }
        //     }
        // }

        private PlanetChunkData CreateChunkData(PlanetChunkNode chunkNode)
        {
            int vertexCount = resolution * resolution;

            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = GenerateTriangles(resolution);

            Vector3 localUp = chunkNode.ParentFace.LocalUp;
            Vector2 uvMin = chunkNode.UVMin;
            Vector2 uvMax = chunkNode.UVMax;

            Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            Vector3 axisB = Vector3.Cross(localUp, axisA);

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int vertexIndex = x + y * resolution;

                    Vector2 percent = new Vector2(x, y) / (resolution - 1);

                    Vector2 faceUv = new Vector2(
                        Mathf.Lerp(uvMin.x, uvMax.x, percent.x),
                        Mathf.Lerp(uvMin.y, uvMax.y, percent.y)
                    );

                    Vector3 pointOnCube =
                        localUp
                        + (faceUv.x - 0.5f) * 2.0f * axisA
                        + (faceUv.y - 0.5f) * 2.0f * axisB;

                    Vector3 pointOnSphere = pointOnCube.normalized;

                    vertices[vertexIndex] =
                        pointOnSphere * (float)profile.Radius;
                    normals[vertexIndex] = pointOnSphere;
                    uvs[vertexIndex] = faceUv;
                }
            }

            return new PlanetChunkData(vertices, normals, uvs, triangles);
        }
        
        private int[] GenerateTriangles(int resolution)
        {
            int quadCount = (resolution - 1) * (resolution - 1);
            int[] triangles = new int[quadCount * 6];

            int triangleIndex = 0;

            for (int y = 0; y < resolution - 1; y++)
            {
                for (int x = 0; x < resolution - 1; x++)
                {
                    int vertexIndex = x + y * resolution;

                    int a = vertexIndex;
                    int b = vertexIndex + resolution + 1;
                    int c = vertexIndex + resolution;
                    int d = vertexIndex + 1;

                    triangles[triangleIndex + 0] = a;
                    triangles[triangleIndex + 1] = b;
                    triangles[triangleIndex + 2] = c;

                    triangles[triangleIndex + 3] = a;
                    triangles[triangleIndex + 4] = d;
                    triangles[triangleIndex + 5] = b;

                    triangleIndex += 6;
                }
            }

            return triangles;
        }
    }
}
