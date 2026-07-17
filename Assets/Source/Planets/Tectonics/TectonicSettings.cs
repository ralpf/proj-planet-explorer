using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Planets.Profiles.Settings
{
    [System.Serializable]
    public class TectonicSettings : LayerSettings
    {
        public int seed = 123;
        [Range(2, 32)] public int plateCount = 9;
        public MinMax plateSpeed = new MinMax(0.2f, 1f);
        public float boundaryWidthRadians = 0.05f;
        public float rollOceanicChance = 0.3f;
        public float rollContinentalChance = 0.5f;
        public MinMax oceanicAge = new MinMax(0, 1);
        public MinMax continentalAge = new MinMax(0.5f, 1);

        [Space(10), Header("Normalized thickness")]
        public MinMax oceanicThickness = new MinMax(0.05f, 0.2f);
        public MinMax continentalThickness = new MinMax(0.5f, 1);
        [Range(0.2f, 1f)] public float baseThickness = 0.7f;  // normalized crust thickness that produces an elevation equal to planet radius
        public float scaleFactor = 3f;  // converts normalized thickness to units by formula 'units = scaleFactor * planetRadius / 1000'
    }
}