using System;
using System.Collections;
using System.Collections.Generic;
using Planets.DataBuffers;
using UnityEngine;
using Planets.Profiles;
using Planets.LogicalTree;
using Extensions.UnityAPI;



namespace Planets.MB
{
    [RequireComponent(typeof(PlanetChunkController))]
    public class PlanetGenerator : MonoBehaviour
    {
        [SerializeField] PlanetChunkController chunkController;
        [SerializeField] PlanetProfile profile;
        [SerializeField, Range(2, 128)] int resolution = 16;
        [SerializeField] Material material;
        [SerializeField] Material[] testMats;

        [SerializeField, Space(20)] Transform observerXf;
        [SerializeField] int maxSubdivisions = 4;
        [SerializeField] float subdivisionDistance = 100f;
        [SerializeField] float subdivisionHysteresis = 20f;

        
        
        public PlanetNode PlanetNode { get; private set; }
        public float Radius => profile.Radius;
        public int Resolution => resolution;
        public Material[] LevelMaterials => testMats;


        void OnValidate()
        {
            Debug.Assert(chunkController, "Planet chunk pool not set in inspector");
        }

        void Update()
        {
            UpdateSubdivisions();
        }

        void UpdateSubdivisions()
        {
            if (PlanetNode == null) Generate();

            foreach (FaceNode faceNode in PlanetNode.Faces)
                EvaluateChunkSubdivision(faceNode.RootChunk);

            void EvaluateChunkSubdivision(ChunkNode chunk)	// => local method
            {
                if (ShouldSplit(chunk))
                {
                    chunk.Subdivide();
                    chunkController.Release(chunk);
                    chunkController.AddMany(chunk.Children);
                }
                else if (ShouldCollapse(chunk))
                {
                    chunkController.ReleaseMany(chunk.Children);
                    chunk.Collapse();
                    chunkController.Add(chunk);
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
            // make logical nodes
            PlanetNode = new PlanetNode();
            
            // make unity objects
            foreach (FaceNode faceNode in PlanetNode.Faces)
                chunkController.Add(faceNode.RootChunk);
        }

        [ContextMenu("CLEAR")]
        public void Clear()
        {
            chunkController.Clear();
            PlanetNode = null;
        }
        
    }
}
