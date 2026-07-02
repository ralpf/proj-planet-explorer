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
        bool showPlatePoints = false;


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
            
            GUILayout.BeginVertical("Box");
            EditorGUI.BeginChangeCheck();
            showPlatePoints = GUILayout.Toggle(showPlatePoints, "Plate Points");
            if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (showPlatePoints) ShowPlatePoints();
        }

        private void ShowPlatePoints()
        {
            int count = Target.debugInfo.tectonicPointCount;
            for (int i = 0; i < count; ++i)
            {
                Vector3 pointOnSphere = GetSamplePoint(i, count);

                var data = ( TectonicPlateLayer.QResult)Target.QueryTectonicLayer(pointOnSphere);
                Vector3 wPos = Target.transform.TransformPoint(pointOnSphere * (Target.Profile.Radius + Target.debugInfo.tectonicPointOffset));
                
                Vector3 worldNormal = Target.transform.TransformDirection(pointOnSphere);
                Vector3 toCamera = SceneView.currentDrawingSceneView.camera.transform.position - wPos;
                if (Vector3.Dot(worldNormal, toCamera) <= 0f) continue;
                
                Handles.color = GetColorFromId(data.plateIdx);
                Handles.SphereHandleCap(0, wPos, Quaternion.identity, Target.debugInfo.tectonicPointSize, EventType.Repaint);
            }
        }

        private static Vector3 GetSamplePoint(int idx, int count)
        {
            float t = (idx + 0.5f) / count;
            float y = 1f - 2f * t;
            float r = Mathf.Sqrt(1f - y * y);

            float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));
            float angle = idx * goldenAngle;

            float x = Mathf.Cos(angle) * r;
            float z = Mathf.Sin(angle) * r;

            return new Vector3(x, y, z).normalized;
        }
        
        private static Color GetColorFromId(int id)
        {
            float hue = Mathf.Repeat(id * 0.61803398875f, 1f);
            return Color.HSVToRGB(hue, 0.75f, 1f);
        }
    }
}