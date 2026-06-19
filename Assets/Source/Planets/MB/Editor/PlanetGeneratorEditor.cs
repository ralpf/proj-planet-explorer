using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



namespace Planets.MB.Ed
{
    [CustomEditor(typeof(PlanetGenerator))]
    public class PlanetGeneratorEditor : Editor
    {
        PlanetGenerator Target => (PlanetGenerator)this.target;


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(" Generate "))
                Target.Generate();
            if (GUILayout.Button(" Clear ", GUILayout.ExpandWidth(false)))
                Target.Clear();
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Sub")) Target.TestSubdivide();
            serializedObject.ApplyModifiedProperties();
        }
    }
}