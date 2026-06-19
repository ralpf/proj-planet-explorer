// ReSharper disable CheckNamespace
using System.Collections.Generic;
using UnityEngine;



namespace Planets.Topology
{
    /// <summary>Logical element representing a face on a cube-sphere surface</summary>
    public class PlanetFaceNode
    {
        PlanetRootNode planetNode;
        PlanetChunkNode rootChunk;  // subdivision level 0 chunk

        public PlanetRootNode PlanetNode => planetNode;
        public PlanetChunkNode RootChunk => rootChunk;
        public Vector3 LocalUp { get; private set; }


        public PlanetFaceNode(PlanetRootNode planetNode, Vector3 localUp)
        {
            this.planetNode = planetNode;
            LocalUp = localUp;
        }

        public void GenerateRootChunk()
        {
            rootChunk = new PlanetChunkNode(0,this, null, Vector2.zero, Vector2.one);
        }

        public void TestSubdivide()
        {
            RootChunk.Subdivide();
        }
    }
}