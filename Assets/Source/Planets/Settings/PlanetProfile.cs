using System.Linq;
using UnityEngine;
using Planets.Profiles.Settings;
using Planets.Profiles.Settings.Assets;


namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "planet-profile", menuName = "Planet/Planet Profile")]
    public class PlanetProfile : ScriptableObject
    {
        [SerializeField] double radius = 6371000.0 / 1000;
        [SerializeField, Range(2, 128)] int resolution = 16;
        [SerializeField] Material material;

        [Space]
        [SerializeField] float heighMult = 1f;
        [SerializeField] LayerSettingsAsset[] layers;

        //......................................................................................PROPERTY

        public int Resolution => resolution;
        public float Radius => (float)radius;
        public float HeightMult => heighMult;
        public Material Material => material;
        public LayerSettingsAsset[] Layers => layers;

        //......................................................................................PROPERTY

        // public float GetElevation(Vector3 pointOnSphere)
        // {
        //     float elevation = 0;
        //
        //     foreach (var layer in settings)
        //         elevation += layer.Evaluate(pointOnSphere);
        //
        //     return elevation * heighMult + Radius;
        // }

        // public void Initialize()
        // {
        //     for (int i = 0; i < settings.Length; ++i)
        //         if (settings[i].active)
        //             settings[i].Initialize();
        // }
        //
        // public T Get<T>() where T : LayerSettings
        // {
        //     foreach (var x in settings)
        //         if (x is T result) return result;
        //     return null;
        // }

    }
}
