using UnityEngine;



namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "planet-profile", menuName = "Planet/Planet Profile")]
    public class PlanetProfile : ScriptableObject
    {
        [SerializeField] double radius = 6371000.0 / 1000;
        [SerializeField, Range(2, 128)] int resolution = 16;
        [SerializeField] Material[] testMats;

        [Space]
        [SerializeField] float heighMult = 1f;
        [SerializeField] float noiseScale = 2f;

        //......................................................................................PROPERTY

        public int Resolution => resolution;
        public float Radius => (float)radius;
        public Material[] LevelMaterials => testMats;

        //......................................................................................PROPERTY

        public float GetElevation(Vector3 pointOnSphere)
        {
            float x = pointOnSphere.x * noiseScale;
            float y = pointOnSphere.y * noiseScale;
            float z = pointOnSphere.z * noiseScale;

            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float zx = Mathf.PerlinNoise(z, x);

            float noise = (xy + yz + zx) / 3.0f;

            return noise * heighMult;
        }
    }
}
