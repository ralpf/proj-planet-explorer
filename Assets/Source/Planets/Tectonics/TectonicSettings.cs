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
        public MinMax oceanicThickness = new MinMax(0.05f, 0.2f);
        public MinMax continentalThickness = new MinMax(0.5f, 1);
    }
}