// ReSharper disable CheckNamespace
using UnityEngine;



namespace Planets.Topology
{
    /// <summary>Logical element representing a subdivisible portion of planet</summary>
    public class PlanetChunkNode
    {
        public PlanetFaceNode ParentFace { get; private set; }
        public Vector2 UVMin { get; private set; }
        public Vector2 UVMax { get; private set; }


        public PlanetChunkNode(PlanetFaceNode parentFace, Vector2 uvMin, Vector2 uvMax)
        {
            ParentFace = parentFace;
            UVMin = uvMin;
            UVMax = uvMax;
        }
    }
}