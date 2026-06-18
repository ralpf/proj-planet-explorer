// ReSharper disable CheckNamespace

using System.Collections.Generic;
using Planets.MB;
using UnityEngine;



namespace Planets.Topology
{
    public class PlanetRootNode
    {
        PlanetFaceNode[] faces;
        PlanetGenerator generator;

        public IEnumerable<PlanetFaceNode> Faces => faces;
        public PlanetGenerator Generator => generator;


        public PlanetRootNode()
        {
            faces = new[] {
                new PlanetFaceNode(this, Vector3.right),
                new PlanetFaceNode(this, Vector3.left),
                new PlanetFaceNode(this, Vector3.up),
                new PlanetFaceNode(this, Vector3.down),
                new PlanetFaceNode(this, Vector3.forward),
                new PlanetFaceNode(this, Vector3.back),
            };

            foreach (var face in faces)
                face.Generate();
        }
    }
}