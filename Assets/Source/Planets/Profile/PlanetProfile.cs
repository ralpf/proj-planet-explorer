using UnityEngine;



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
        [SerializeField] float noiseScale = 2f;

        [SerializeField] ProfileLayer[] layers;

        //......................................................................................PROPERTY

        public int Resolution => resolution;
        public float Radius => (float)radius;
        public Material Material => material;

        //......................................................................................PROPERTY

        public float GetElevation(Vector3 pointOnSphere)
        {
            float elevation = 0;

            foreach (var layer in layers)
                elevation += layer.Evaluate(pointOnSphere);

            return elevation * heighMult + Radius;
        }

        public void Initialize()
        {
            for (int i = 0; i < layers.Length; ++i)
                if (layers[i].active)
                    layers[i].Initialize();
        }

        public T Get<T>() where T : ProfileLayer
        {
            foreach (var x in layers)
                if (x is T result) return result;
            return null;
        }

    }
}
