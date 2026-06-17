using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Planets.Generation
{
    public sealed class CubeSphereFace
    {
        readonly Material material;
        readonly Transform parent;
        readonly Vector3 localUp;
        readonly float radius;
        readonly int resolution;

        public CubeSphereFace(Transform parent, Vector3 localUp, float radius, int resolution, Material material)
        {
            this.parent = parent;
            this.localUp = localUp;
            this.radius = radius;
            this.resolution = resolution;
            this.material = material;
        }

        public void Generate()
        {
            GameObject faceObject = new GameObject($"Face {localUp}");
            faceObject.transform.SetParent(parent, false);

            MeshFilter meshFilter = faceObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = faceObject.AddComponent<MeshRenderer>();

            meshRenderer.sharedMaterial = material;
            meshFilter.sharedMesh = CreateMesh();
        }

        private Mesh CreateMesh()
        {
            int vertexCount = resolution * resolution;

            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];

            int quadCount = (resolution - 1) * (resolution - 1);
            int[] triangles = new int[quadCount * 6];

            Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            Vector3 axisB = Vector3.Cross(localUp, axisA);

            int triangleIndex = 0;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int vertexIndex = x + y * resolution;
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointOnCube = localUp
                                          + (percent.x - 0.5f) * 2.0f * axisA
                                          + (percent.y - 0.5f) * 2.0f * axisB;

                    Vector3 pointOnSphere = pointOnCube.normalized;

                    vertices[vertexIndex] = pointOnSphere * radius;
                    normals[vertexIndex] = pointOnSphere;
                    uvs[vertexIndex] = percent;

                    if (x == resolution - 1 || y == resolution - 1) continue;

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

            Mesh mesh = new Mesh();
            mesh.name = $"Cube Sphere Face ({localUp})";
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}