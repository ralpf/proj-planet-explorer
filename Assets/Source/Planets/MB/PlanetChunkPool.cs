using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace Planets.MB
{
    public class PlanetChunkPool : MonoBehaviour
    {
        int createdCount;
        PlanetGenerator generator;
        Stack<PlanetChunk> pool = new();
        
        public PlanetChunk Rent()
        {
            return pool.Count > 0 ? pool.Pop() : CreateNew();
        }

        public void Release(PlanetChunk chunk)
        {
            if (!chunk) return;
            pool.Push(chunk);
            chunk.gameObject.SetActive(false);
        }

        public void Clear()
        {
            pool.Clear();
        }

        private PlanetChunk CreateNew()
        {
            if (!generator) generator = this.GetComponent<PlanetGenerator>();
            var go = new GameObject($"Planet Chunk {++createdCount}");
            go.transform.SetParent(this.transform, false);
            return go.AddComponent<PlanetChunk>();
        }
    }
}