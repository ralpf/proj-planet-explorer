using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Planets.Profiles.Settings
{
    [System.Serializable]
    public abstract class LayerSettings
    {
        public PlanetProfile Profile { get; private set; }

        public void SetParentProfile(PlanetProfile parent) => Profile = parent;
    }
}