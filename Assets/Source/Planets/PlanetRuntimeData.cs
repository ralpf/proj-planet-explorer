using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Planets.Profiles;
using Planets.Profiles.Settings;
using Planets.Profiles.Settings.Assets;
using UnityEngine;



namespace Planets.Data.Runtime
{
    public class PlanetRuntimeData
    {
        PlanetProfile profile;
        LayerRuntimeData[] dataLayers;



        public PlanetRuntimeData(PlanetProfile profile)
        {
            this.profile = profile;
            dataLayers = profile.Layers
                .Where(x => x != null && x.Active)
                .Select(x => InstantiateLayerData(x.SettingsBase))
                .ToArray();
        }

        public T GetRuntimeData<T>() where T : LayerRuntimeData
        {
            foreach (LayerRuntimeData data in dataLayers)
                if (data is T tdata) return tdata;
        
            throw new System.InvalidOperationException($"unexpected type {typeof(T).Name}");
        }

        public float GetElevation(Vector3 pointOnSphere)
        {
            float elevation = 0;
            foreach (LayerRuntimeData layer in dataLayers)
                elevation += layer.Evaluate(pointOnSphere);
            return elevation * profile.HeightMult;
        }

        public PlanetSample GetFullSample(Vector3 pointOnSphere)
        {
            PlanetSample sample = new(pointOnSphere);
            foreach (LayerRuntimeData layer in dataLayers)
                layer.Sample(sample);
            return sample;
        }

        private LayerRuntimeData InstantiateLayerData(LayerSettings layer)
        {
            return layer switch
            {
                TectonicSettings s => new TectonicRuntimeData(s),
                null => throw new System.ArgumentNullException(),
                _    => throw new System.NotSupportedException($"unexpected type {layer.GetType().Name}")
            };
        }
    }
}