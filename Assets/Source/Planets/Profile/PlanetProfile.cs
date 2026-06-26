using UnityEngine;



namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "planet-profile", menuName = "Planet/Planet Profile")]
    public class PlanetProfile : ScriptableObject
    {
        [SerializeField] double radius = 6371000.0 / 1000;
        [SerializeField, Range(2, 128)] int resolution = 16;
        [SerializeField] Material[] testMats;
        

        public int Resolution => resolution;
        public float Radius => (float)radius;
        public Material[] LevelMaterials => testMats;
        
    }       
}
