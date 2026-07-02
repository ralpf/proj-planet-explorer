using System;
using UnityEngine;
using UnityEditor;



namespace Planets.Profiles.Ed
{
    [CustomEditor(typeof(DebugTectonicPlates))]
    public class DebugTectonicPlatesEditor : Editor
    {
        struct Point
        {
            public Vector3 wpos;
            public Vector3 normal;
            public int plateIdx;
            public Color color;
        }
        
        public bool showPoints;
        Point[] pointArr;
        
        DebugTectonicPlates Target => (DebugTectonicPlates)target;
        TectonicPlateLayer.Data LayerData => Target.TectonicLayer.LayerData;


        void Awake()
        {
            Target.OnChanged += OnParametersChanged;
        }

        void OnDisable()
        {
            Target.OnChanged -= OnParametersChanged;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Show Points"))
                OnParametersChanged();
        }


        public void OnSceneGUI()
        {
            if (pointArr is { Length: > 0 })
                DrawPointArray();
        }


        private void OnParametersChanged()
        {
            if (Target.TectonicLayer.LayerData == null) return;
            if (pointArr != null && pointArr.Length == Target.pointCount) return;

            Vector3 originWorldPos = Target.transform.position;
            int count = Target.pointCount;
            var data = Target.TectonicLayer.LayerData;
            pointArr = new Point[count];

            for (int i = 0; i < count; ++i)
            {
                Vector3 pointOnSphere = GetSamplePoint(i, count);
                pointArr[i].wpos = Target.transform.TransformPoint(pointOnSphere * (Target.Profile.Radius + Target.pointOffset));
                pointArr[i].normal = (pointArr[i].wpos - originWorldPos).normalized;
                var queryResult = data.Query(pointOnSphere);
                pointArr[i].plateIdx = queryResult.plateIdx;
                pointArr[i].color = GetColorFromId(pointArr[i].plateIdx);
            }
            
            Debug.Log("Rebuild points: " + pointArr.Length);
        }


        private void DrawPointArray()
        {
            if (pointArr == null || pointArr.Length == 0) return;
            Vector3 cameraWorldPos = SceneView.currentDrawingSceneView.camera.transform.position;

            for (int i = 0; i < pointArr.Length; ++i)
            {
                Vector3 toCamera    = cameraWorldPos - pointArr[i].wpos;
                if (Vector3.Dot(pointArr[i].wpos, toCamera) <= 0f) continue;

                Handles.color = pointArr[i].color;
                Handles.DotHandleCap(0, pointArr[i].wpos, Quaternion.identity, Target.pointSize, EventType.Repaint);
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