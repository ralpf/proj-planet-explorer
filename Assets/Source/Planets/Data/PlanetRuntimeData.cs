using System.Linq;
using UnityEngine;
using Planets.Profiles;
using Planets.Profiles.Settings;



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

            return elevation * profile.HeightMult + profile.Radius;
        }

        public PlanetSample GetFullSample(Vector3 pointOnSphere)
        {
            var sample = new PlanetSample(pointOnSphere);
            foreach (LayerRuntimeData layer in dataLayers)
                layer.Sample(sample);
            return sample;
        }

        private LayerRuntimeData InstantiateLayerData(LayerSettings layer)
        {
            layer.SetParentProfile(profile);
            return layer switch
            {
                TectonicSettings s => new TectonicRuntimeData(s),
                null => throw new System.ArgumentNullException(),
                _    => throw new System.NotSupportedException($"unexpected type {layer.GetType().Name}")
            };
        }
    }
}