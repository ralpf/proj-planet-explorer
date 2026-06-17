using UnityEngine;



namespace Planets.Profiles
{
    [CreateAssetMenu(fileName = "planet-profile", menuName = "Planet/Planet Profile")]
    public class PlanetProfile : ScriptableObject
    {
        public double radius = 6371000.0 / 1000;
    }       
}
