using System;
using System.Collections;
using System.Collections.Generic;
using Extensions.UnityAPI;
using Planets.Data.Runtime;
using Planets.DataBuffers;
using UnityEngine;
using Planets.Profiles;
using Planets.LogicalTree;



namespace Planets.MB
{
    [RequireComponent(typeof(PlanetChunkPool))]
    public class PlanetChunkSwitcher : MonoBehaviour
    {
        [SerializeField] PlanetProfile profile;

        [SerializeField, Space(20)] Transform observerXf;
        [SerializeField] int maxSubdivisions = 4;
        [SerializeField] float subdivisionDistance = 100f;
        [SerializeField] float subdivisionHysteresis = 20f;

        PlanetNode      planetNode;
        PlanetChunkPool chunkPool;


        public PlanetProfile Profile => profile;
        public PlanetRuntimeData RuntimeData { get; private set; }



        void Awake()
        {
            chunkPool = this.GetComponentAsserted<PlanetChunkPool>();
        }

        void Update()
        {
            UpdateSubdivisions();
        }

        void UpdateSubdivisions()
        {
            if (planetNode == null) return;

            foreach (FaceNode faceNode in planetNode.Faces)
                EvaluateChunkSubdivision(faceNode.RootChunk);

            void EvaluateChunkSubdivision(ChunkNode chunk)	// => local method
            {
                if (ShouldSplit(chunk))
                {
                    chunk.Subdivide();
                    chunkPool.Release(chunk);
                    chunkPool.AddMany(chunk.Children);
                }
                else if (ShouldCollapse(chunk))
                {
                    chunkPool.ReleaseMany(chunk.Children);
                    chunk.Collapse();
                    chunkPool.Add(chunk);
                }
                else if (chunk.IsSubdivided)
                {
                    // pass thru node or leaf parent that did not collapse
                    foreach (ChunkNode child in chunk.Children)
                        EvaluateChunkSubdivision(child);
                }
            }

            bool ShouldSplit(ChunkNode chunk)
            {
                if (chunk.IsLeafChunk == false || chunk.SubdivisionLevel >= maxSubdivisions) return false;
                float splitDistance = (maxSubdivisions - chunk.SubdivisionLevel) * subdivisionDistance;
                return DistanceFromChunkToObserver(chunk) < splitDistance;
            }

            bool ShouldCollapse(ChunkNode chunk)
            {
                if (chunk.IsLeafParent == false) return false;
                float collapseDistance = (maxSubdivisions - chunk.SubdivisionLevel) * subdivisionDistance + subdivisionHysteresis;
                return DistanceFromChunkToObserver(chunk) > collapseDistance;
            }

            float DistanceFromChunkToObserver(ChunkNode chunk)
            {
                Vector3 localCenter = chunk.LocalCenterNormal * profile.Radius;
                Vector3 worldCenter = transform.TransformPoint(localCenter);
                return Vector3.Distance(observerXf.position, worldCenter);
            }

        }


        [ContextMenu("GENERATE")]
        public void Generate()
        {
            this.Clear();
            RuntimeData = new PlanetRuntimeData(profile);
            // make logical nodes
            planetNode = new PlanetNode();
            
            // make unity objects
            foreach (FaceNode faceNode in planetNode.Faces)
                chunkPool.Add(faceNode.RootChunk);
        }

        [ContextMenu("CLEAR")]
        public void Clear()
        {
            chunkPool.Clear();
            planetNode = null;
        }

    }
}
