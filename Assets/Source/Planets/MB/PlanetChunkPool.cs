using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.UnityAPI;
using Planets.DataBuffers;
using Planets.LogicalTree;
using Planets.Profiles;
using UnityEngine;



namespace Planets.MB
{
    [RequireComponent(typeof(PlanetChunkSwitcher))]
    public class PlanetChunkPool : MonoBehaviour
    {
        PlanetProfile profile;
        Stack<PlanetChunk> pool = new();
        Dictionary<ChunkNode, PlanetChunk> active = new();


        void Awake()
        {
            profile = this.GetComponentAsserted<PlanetChunkSwitcher>().Profile;
        }

        public void Add(ChunkNode chunk)
        {
            PlanetChunk uobj = pool.Count > 0 ? pool.Pop() : CreateNew();
            uobj.Recalculate(new PlanetChunkData(chunk, profile));
            uobj.SetMaterial(profile.LevelMaterials[chunk.SubdivisionLevel]);
            uobj.Active = true;
            uobj.name = $"Planet Chunk L{chunk.SubdivisionLevel}";
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
            transform.DestroyChildren();
        }

        private PlanetChunk CreateNew()
        {
            var go = new GameObject();
            go.transform.SetParentAndReset(transform);
            return go.AddComponent<PlanetChunk>();
        }
    }
}