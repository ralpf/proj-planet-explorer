using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Planets.Profiles.Settings.Assets
{
    public abstract class LayerSettingsAsset<TSettings> : LayerSettingsAsset where TSettings : LayerSettings
    {
        [SerializeField] TSettings settings;

        public TSettings Settings => settings;
        public override LayerSettings SettingsBase => settings;
    }
}