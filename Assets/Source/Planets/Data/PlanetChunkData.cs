// ReSharper disable CheckNamespace
using System.Collections;
using System.Collections.Generic;
using Planets.LogicalTree;
using UnityEngine;


namespace Planets.DataBuffers
{
    public class PlanetChunkData
    {
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] UVs { get; private set; }
        public int[] Triangles { get; private set; }



        public PlanetChunkData(ChunkNode chunkNode, int resolution, float planetRadius)
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

                    vertices[vertexIndex] = pointOnSphere * planetRadius;
                    normals[vertexIndex]  = pointOnSphere;
                    uvs[vertexIndex] = faceUv;
                }
            }

            Vertices = vertices;
            Normals = normals;
            UVs = uvs;
            Triangles = triangles;
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
