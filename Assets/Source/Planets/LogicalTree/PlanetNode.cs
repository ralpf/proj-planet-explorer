// ReSharper disable CheckNamespace

using System.Collections.Generic;
using System.Linq;
using Planets.MB;
using UnityEngine;



namespace Planets.LogicalTree
{
    /// <summary>Logical element representing the root object of a planet</summary>
    public class PlanetNode
    {
        FaceNode[] faces;


        public FaceNode[] Faces => faces;


        public PlanetNode()
        {
            faces = new[] {
                new FaceNode(this, Vector3.right),
                new FaceNode(this, Vector3.left),
                new FaceNode(this, Vector3.up),
                new FaceNode(this, Vector3.down),
                new FaceNode(this, Vector3.forward),
                new FaceNode(this, Vector3.back),
            };
        }
    }
}