// ReSharper disable CheckNamespace
using System.Collections.Generic;
using UnityEngine;



namespace Planets.LogicalTree
{
    /// <summary>Logical element representing a subdivisible portion of planet</summary>
    public class ChunkNode
    {
        ChunkNode[] childSubdivisions;
        
        public FaceNode  ParentFace  { get; private set; }
        public ChunkNode ParentChunk { get; private set; }
        public ChunkNode[] Children => childSubdivisions; 

        public int  SubdivisionLevel { get; private set; }
        public Vector2 UVMin { get; private set; }
        public Vector2 UVMax { get; private set; }
        public Vector3 LocalCenterNormal { get; private set; }  // aka local normal of chunk from it's center
        public bool IsSubdivided { get; private set; }
        public bool IsLeafChunk => !IsSubdivided;
        public bool IsLeafParent => GetHasLeafChildren();



        public ChunkNode(int subdivisionLevel, FaceNode parentFace, ChunkNode parentChunk, Vector2 uvMin, Vector2 uvMax)
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
            if (childSubdivisions == null) childSubdivisions = new ChunkNode[4];
            // 0 = bottom-left, 1 = bottom-right, 2 = top-left, 3 = top-right
            childSubdivisions[0] = new ChunkNode(childSubLevel, ParentFace, this, new Vector2(UVMin.x, UVMin.y), new Vector2(uvCenter.x, uvCenter.y));
            childSubdivisions[1] = new ChunkNode(childSubLevel, ParentFace, this, new Vector2(uvCenter.x, UVMin.y), new Vector2(UVMax.x, uvCenter.y));
            childSubdivisions[2] = new ChunkNode(childSubLevel, ParentFace, this, new Vector2(UVMin.x, uvCenter.y), new Vector2(uvCenter.x, UVMax.y));
            childSubdivisions[3] = new ChunkNode(childSubLevel, ParentFace, this, new Vector2(uvCenter.x, uvCenter.y), new Vector2(UVMax.x, UVMax.y));
            IsSubdivided = true;
        }

        public void Collapse()
        {
            for (int i = 0; i < 4; ++i) childSubdivisions[i] = null;
            IsSubdivided = false;
        }
        
        public IEnumerable<ChunkNode> AllChunks()
        {
            yield return this;
            if (IsSubdivided)
                foreach (ChunkNode childChunk in childSubdivisions)
                    foreach (ChunkNode chunk in childChunk.AllChunks())
                        yield return chunk;
        }

        public IEnumerable<ChunkNode> LeafChunks()
        {
            if (IsSubdivided)
            {
                foreach (ChunkNode childChunk in childSubdivisions)
                    foreach (ChunkNode chunk in childChunk.LeafChunks())
                        yield return chunk;
            }
            else yield return this;
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

        private bool GetHasLeafChildren()
        {
            if (!IsSubdivided) return false;
            foreach (var x in childSubdivisions)
                if (x.IsLeafChunk == false) return false;
            return true;
        }
    }
}