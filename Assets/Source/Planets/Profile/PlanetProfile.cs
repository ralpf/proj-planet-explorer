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
            float elevation = 0f;

            elevation += Mathf.Sin(Vector3.Dot(pointOnSphere, new Vector3(1.0f, 0.4f, 0.2f)) * noiseScale);
            elevation += Mathf.Sin(Vector3.Dot(pointOnSphere, new Vector3(0.3f, 1.0f, 0.7f)) * noiseScale * 1.7f) * 0.5f;
            elevation += Mathf.Sin(Vector3.Dot(pointOnSphere, new Vector3(0.6f, 0.2f, 1.0f)) * noiseScale * 2.3f) * 0.25f;

            return elevation * heighMult;
        }

        public void Precompute()
        {
            for (int i = 0; i < layers.Length; ++i)
                if (layers[i].active)
                    layers[i].Precompute();
        }

        public Cubemap GetDebugCubemap()
        {
            foreach (var x in layers)
                if (x is TectonicPlateLayer tectonic)
                    return tectonic.DebugCubemap;
            throw new System.InvalidProgramException();
        }
    }
}
