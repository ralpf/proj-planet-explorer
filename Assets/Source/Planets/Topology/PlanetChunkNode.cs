// ReSharper disable CheckNamespace
using System.Collections.Generic;
using UnityEngine;



namespace Planets.Topology
{
    /// <summary>Logical element representing a subdivisible portion of planet</summary>
    public class PlanetChunkNode
    {
        PlanetChunkNode[] childSubdivisions;
        
        public PlanetFaceNode  ParentFace  { get; private set; }
        public PlanetChunkNode ParentChunk { get; private set; }
        public IEnumerable<PlanetChunkNode> Children => childSubdivisions; 

        public bool IsSubdivided => childSubdivisions != null;
        public int  SubdivisionLevel { get; private set; }
        public Vector2 UVMin { get; private set; }
        public Vector2 UVMax { get; private set; }
        public Vector3 LocalCenterNormal { get; private set; }  // aka local normal of chunk from it's center



        public PlanetChunkNode(int subdivisionLevel, PlanetFaceNode parentFace, PlanetChunkNode parentChunk, Vector2 uvMin, Vector2 uvMax)
        {
            SubdivisionLevel = subdivisionLevel;
            ParentFace = parentFace;
            ParentChunk = parentChunk;
            UVMin = uvMin;
            UVMax = uvMax;
            LocalCenterNormal = CalculateLocalCenterNormal();
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

        public void Collapse()
        {
            //throw null;
        }
        
        public IEnumerable<PlanetChunkNode> AllChunks()
        {
            yield return this;
            if (IsSubdivided)
                foreach (PlanetChunkNode childChunk in childSubdivisions)
                    foreach (PlanetChunkNode chunk in childChunk.AllChunks())
                        yield return chunk;
        }

        public IEnumerable<PlanetChunkNode> LeafChunks()
        {
            if (IsSubdivided)
            {
                foreach (PlanetChunkNode childChunk in childSubdivisions)
                    foreach (PlanetChunkNode chunk in childChunk.LeafChunks())
                        yield return chunk;
            }
            else
            {
                yield return this;
            }
        }

        private Vector3 CalculateLocalCenterNormal()
        {
            Vector3 localUp = ParentFace.LocalUp;
            Vector2 uvCenter = (UVMin + UVMax) * 0.5f;
            Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            Vector3 axisB = Vector3.Cross(localUp, axisA);

            Vector3 pointOnCube =
                localUp
                + (uvCenter.x - 0.5f) * 2f * axisA
                + (uvCenter.y - 0.5f) * 2f * axisB;

            return pointOnCube.normalized;
        }
    }
}