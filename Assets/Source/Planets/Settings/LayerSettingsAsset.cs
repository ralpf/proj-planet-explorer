using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Planets.Profiles.Settings.Assets
{
    public abstract class LayerSettingsAsset : ScriptableObject
    {
        [SerializeField] bool active = true;

        public bool Active => active;
        public abstract LayerSettings SettingsBase { get; }
    }
}