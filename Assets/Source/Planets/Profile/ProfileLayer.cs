using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Planets.Profiles
{
    public abstract class ProfileLayer : ScriptableObject
    {
        public bool active = true;

        public abstract void Evaluate(ref Sample sample);
        public abstract void Precompute();
    }
}