using System;
using System.Collections;
using System.Collections.Generic;
using Planets.Profiles;
using UnityEditor;
using UnityEngine;



namespace Planets.MB.Ed
{
    [CustomEditor(typeof(PlanetChunkSwitcher))]
    public class PlanetGeneratorEditor : Editor
    {
        PlanetChunkSwitcher Target => (PlanetChunkSwitcher)this.target;


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            if (Application.isPlaying)
            {
                if (GUILayout.Button(" Generate "))
                    Target.Generate();
                if (GUILayout.Button(" Clear ", GUILayout.ExpandWidth(false)))
                    Target.Clear();
            }
            else
            {
                EditorGUILayout.HelpBox("Generation available in play mode only", MessageType.Info);
            }
            GUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }

    }
}