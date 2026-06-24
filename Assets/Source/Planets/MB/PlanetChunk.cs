using System.Collections;
using System.Collections.Generic;
using Planets.DataBuffers;
using UnityEngine;



namespace Planets.MB
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class PlanetChunk : MonoBehaviour
    {
        public bool Active {
            set => this.gameObject.SetActive(value);
        }
        
        public void Recalculate(PlanetChunkData chunkData)
        {
            var mf = this.GetComponent<MeshFilter>();
            if (mf.sharedMesh == null) mf.sharedMesh = new Mesh() { name = "Chunk Mesh" };
            var mesh = mf.sharedMesh;
            mesh.vertices = chunkData.Vertices;
            mesh.normals = chunkData.Normals;
            mesh.triangles = chunkData.Triangles;
            mesh.uv = chunkData.UVs;
            mesh.RecalculateBounds();
        }

        public void SetMaterial(Material mat)
        {
            var mr = this.GetComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
        }

    }
}