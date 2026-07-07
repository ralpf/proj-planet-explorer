using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Planets.Profiles.Settings.Assets
{
    [CreateAssetMenu(fileName = "tectonic-plate-layer", menuName = "Planet/Tectonic Plate Layer")]
    public class TectonicSettingsAsset : LayerSettingsAsset<TectonicSettings>
    {
    }
}