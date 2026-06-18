// ReSharper disable CheckNamespace
using System.Collections;
using System.Collections.Generic;
using Planets.Topology;
using UnityEngine;


namespace Planets.DataBuffers
{
    public class PlanetChunkData
    {
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] UVs { get; private set; }
        public int[] Triangles { get; private set; }


        public PlanetChunkData(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles)
        {
            Vertices = vertices;
            Normals = normals;
            UVs = uvs;
            Triangles = triangles;
        }
    }
}
