using UnityEngine;



namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "planet-profile", menuName = "Planet/Planet Profile")]
    public class PlanetProfile : ScriptableObject
    {
        [SerializeField] double radius = 6371000.0 / 1000;

        public float Radius => (float)radius;
    }       
}
