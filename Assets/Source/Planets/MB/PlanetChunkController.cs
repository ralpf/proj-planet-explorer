using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.UnityAPI;
using Planets.DataBuffers;
using Planets.LogicalTree;
using UnityEngine;



namespace Planets.MB
{
    [RequireComponent(typeof(PlanetGenerator))]
    public class PlanetChunkController : MonoBehaviour
    {
        Stack<PlanetChunk> pool = new();
        Dictionary<ChunkNode, PlanetChunk> active = new();
        int createdCount;
        float planetRadius;
        int resolution;
        Material[] mats;

        public void Init()
        {
            var gen = this.GetComponent<PlanetGenerator>();
            planetRadius = gen.Radius;
            resolution = gen.Resolution;
            mats = gen.LevelMaterials;
        }


        public void Add(ChunkNode chunk)
        {
            PlanetChunk uobj = pool.Count > 0 ? pool.Pop() : CreateNew();
            uobj.Recalculate(new PlanetChunkData(chunk, resolution, planetRadius));
            uobj.SetMaterial(mats[chunk.SubdivisionLevel]);
            uobj.Active = true;
            active.Add(chunk, uobj);
        }

        public void AddMany(IEnumerable<ChunkNode> chunks)
        {
            foreach (var x in chunks) Add(x);
        }

        public void Release(ChunkNode chunk)
        {
            if (chunk == null) return;
            if (active.Remove(chunk, out PlanetChunk uobject))
            {
                pool.Push(uobject);
                uobject.Active = false;
            }
            else Debug.LogWarning("attempt to release a chunk that had no render representation in scene");
        }

        public void ReleaseMany(IEnumerable<ChunkNode> chunks)
        {
            foreach (var x in chunks) Release(x);
        }

        public void Clear()
        {
            pool.Clear();
            active.Clear();
            createdCount = 0;
            transform.DestroyChildren();
        }

        private PlanetChunk CreateNew()
        {
            var go = new GameObject($"[{++createdCount}] Planet Chunk L");
            go.transform.SetParent(this.transform, false);
            return go.AddComponent<PlanetChunk>();
        }
    }
}