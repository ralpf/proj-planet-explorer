// ReSharper disable CheckNamespace
using UnityEngine;



namespace Planets.Topology
{
    /// <summary>Logical element representing a subdivisible portion of planet</summary>
    public class PlanetChunkNode
    {
        PlanetChunkNode[] childSubdivisions;
        
        public PlanetFaceNode  ParentFace { get; private set; }
        public PlanetChunkNode ParentChunk { get; private set; }

        public bool IsSubdivided => childSubdivisions != null;
        public int SubdivisionLevel { get; private set; }
        public Vector2 UVMin { get; private set; }
        public Vector2 UVMax { get; private set; }



        public PlanetChunkNode(int subdivisionLevel, PlanetFaceNode parentFace, PlanetChunkNode parentChunk, Vector2 uvMin, Vector2 uvMax)
        {
            SubdivisionLevel = subdivisionLevel;
            ParentFace = parentFace;
            ParentChunk = parentChunk;
            UVMin = uvMin;
            UVMax = uvMax;
        }

        public void Subdivide()
        {
            if (IsSubdivided) return;

            int childSubLevel = SubdivisionLevel + 1;
            Vector2 uvCenter = (UVMin + UVMax) * 0.5f;
            childSubdivisions = new PlanetChunkNode[4];
            // 0 = bottom-left, 1 = bottom-right, 2 = top-left, 3 = top-right
            childSubdivisions[0] = new PlanetChunkNode(childSubLevel, ParentFace, this, new Vector2(UVMin.x, UVMin.y), new Vector2(uvCenter.x, uvCenter.y));
            childSubdivisions[1] = new PlanetChunkNode(childSubLevel, ParentFace, this, new Vector2(uvCenter.x, UVMin.y), new Vector2(UVMax.x, uvCenter.y));
            childSubdivisions[2] = new PlanetChunkNode(childSubLevel, ParentFace, this, new Vector2(UVMin.x, uvCenter.y), new Vector2(uvCenter.x, UVMax.y));
            childSubdivisions[3] = new PlanetChunkNode(childSubLevel, ParentFace, this, new Vector2(uvCenter.x, uvCenter.y), new Vector2(UVMax.x, UVMax.y));
        }
    }
}