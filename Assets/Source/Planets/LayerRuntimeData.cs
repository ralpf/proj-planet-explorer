using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planets.Profiles.Settings;



namespace Planets.Data.Runtime
{
    public abstract class LayerRuntimeData
    {
        LayerSettings settings;

        protected T CastSettings<T>() where T : LayerSettings => (T)settings;

        protected LayerRuntimeData(LayerSettings settings)
        {
            this.settings = settings;
        }

        public abstract float Evaluate(Vector3 pointOnSphere);
        public abstract void  Sample(PlanetSample result);
    }
}
