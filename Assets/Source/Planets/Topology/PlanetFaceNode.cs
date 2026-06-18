// ReSharper disable CheckNamespace
using System.Collections.Generic;
using UnityEngine;



namespace Planets.Topology
{
    public class PlanetFaceNode
    {
        List<PlanetChunkNode> chunks = new();
        PlanetRootNode planetNode;

        public PlanetRootNode PlanetNode => planetNode; 
        public IEnumerable<PlanetChunkNode> Chunks => chunks;
        public Vector3 LocalUp { get; private set; }


        public PlanetFaceNode(PlanetRootNode planetNode, Vector3 localUp)
        {
            this.planetNode = planetNode;
            LocalUp = localUp;
        }

        public void Generate()
        {
            chunks.Clear();
            chunks.Add(new PlanetChunkNode(this, Vector2.zero, Vector2.up));
        }
    }
}