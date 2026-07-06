using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Planets.Profiles
{
    public abstract class ProfileLayer : ScriptableObject
    {
        public bool active = true;

        public abstract float Evaluate(Vector3 pointOnSphere);
        public abstract void Initialize();
    }
}