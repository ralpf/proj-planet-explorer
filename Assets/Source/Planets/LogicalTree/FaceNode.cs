// ReSharper disable CheckNamespace
using System.Collections.Generic;
using UnityEngine;



namespace Planets.LogicalTree
{
    /// <summary>Logical element representing a face on a cube-sphere surface</summary>
    public class FaceNode
    {
        PlanetNode planetNode;
        ChunkNode  rootChunk;  // subdivision level 0 chunk

        public PlanetNode PlanetNode => planetNode;
        public ChunkNode  RootChunk  => rootChunk;
        public Vector3    LocalUp { get; private set; }


        public FaceNode(PlanetNode planetNode, Vector3 localUp)
        {
            this.planetNode = planetNode;
            LocalUp = localUp;
            rootChunk = new ChunkNode(0,this, null, Vector2.zero, Vector2.one);
        }
    }
}