// ReSharper disable CheckNamespace

using System.Collections.Generic;
using Planets.MB;
using UnityEngine;



namespace Planets.Topology
{
    /// <summary>Logical element representing the root object of a planet</summary>
    public class PlanetRootNode
    {
        PlanetFaceNode[] faces;


        public IEnumerable<PlanetFaceNode> Faces => faces;


        public void GenerateFaces()
        {
            faces = new[] {
                new PlanetFaceNode(this, Vector3.right),
                new PlanetFaceNode(this, Vector3.left),
                new PlanetFaceNode(this, Vector3.up),
                new PlanetFaceNode(this, Vector3.down),
                new PlanetFaceNode(this, Vector3.forward),
                new PlanetFaceNode(this, Vector3.back),
            };

            foreach (var face in faces) face.GenerateChunks();
        }
    }
}