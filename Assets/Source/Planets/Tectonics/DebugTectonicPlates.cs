using System;
using UnityEngine;
using Planets.MB;




namespace Planets.Profiles
{
    public class DebugTectonicPlates : MonoBehaviour
    {
        public int   pointCount = 1024;
        public float pointOffset = 10;
        public float pointSize = 3;
        public float boundaryWidthRadians = 0.05f;
        public float motionDirectionScale = 5f;
        public bool enableMotionDirection = false;

        public PlanetProfile Profile => this.GetComponent<PlanetChunkSwitcher>().Profile;
        //public TectonicPlateLayer TectonicLayer => Profile.Get<TectonicPlateLayer>();

        public event Action OnChanged;

        void OnValidate() => OnChanged?.Invoke();

    }
}