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
            public Color color;
        }

        public bool showPoints;
        Point[] pointArr;
        int mode;
        
        DebugTectonicPlates Target => (DebugTectonicPlates)target;
        TectonicPlateLayer.Data LayerData => Target.TectonicLayer.LayerData;


        void OnEnable()
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
            {
                mode = 0;
                OnParametersChanged();
            }
            if (GUILayout.Button("Show Borders"))
            {
                mode = 1;
                OnParametersChanged();
            }
        }


        public void OnSceneGUI()
        {
            if (pointArr is { Length: > 0 })
                DrawPointArray();
            if (Target.enableMotionDirection)
                DrawMotionArrows();
        }


        private void OnParametersChanged()
        {
            if (Target.TectonicLayer.LayerData == null) return;

            Vector3 originWorldPos = Target.transform.position;
            var data = Target.TectonicLayer.LayerData;
            int count = Target.pointCount;
            pointArr = new Point[count];

            if (mode == 0) PreparePlatesData();
            else if (mode == 1) PrepareBorderData();
            else throw new System.NotImplementedException();

            void PrepareBorderData()
            {
                for (int i = 0; i < count; ++i)
                {
                    Vector3 pointOnSphere = GetSamplePoint(i, count);
                    pointArr[i].wpos = Target.transform.TransformPoint(pointOnSphere * (Target.Profile.Radius + Target.pointOffset));
                    pointArr[i].normal = pointArr[i].wpos.normalized;
                    var queryResult = data.Query(pointOnSphere);
                    pointArr[i].color = GetColorFromBorder(queryResult.boundaryType);
                    if (queryResult.boundaryMarginRadians > Target.boundaryWidthRadians) pointArr[i].color = Color.black;
                }
            }
            
            void PreparePlatesData()
            {
                for (int i = 0; i < count; ++i)
                {
                    Vector3 pointOnSphere = GetSamplePoint(i, count);
                    pointArr[i].wpos = Target.transform.TransformPoint(pointOnSphere * (Target.Profile.Radius + Target.pointOffset));
                    pointArr[i].normal = pointArr[i].wpos.normalized;
                    var queryResult = data.Query(pointOnSphere);
                    pointArr[i].color = GetColorFromIdx(queryResult.plateIdx);
                }
            }
        }


        private void DrawPointArray()
        {
            if (pointArr == null || pointArr.Length == 0) return;
            Vector3 cameraWorldPos = SceneView.currentDrawingSceneView.camera.transform.position;

            for (int i = 0; i < pointArr.Length; ++i)
            {
                Vector3 wpos = Target.transform.TransformPoint(pointArr[i].wpos);
                Vector3 norm = Target.transform.TransformPoint(pointArr[i].normal);
                Vector3 toCamera    = cameraWorldPos - wpos;
                if (Vector3.Dot(norm, toCamera) <= 0f) continue;

                Handles.color = pointArr[i].color;
                Handles.DotHandleCap(0, wpos, Quaternion.identity, Target.pointSize, EventType.Repaint);
            }
        }

        private void DrawMotionArrows()
        {
            var data = Target.TectonicLayer.LayerData;
            if (data == null) return;
            Vector3 cameraWorldPos = SceneView.currentDrawingSceneView.camera.transform.position;

            for (int i = 0; i < data.PlateCount; ++i)
            {
                var plate = data.GetPlate(i);
                Vector3 lpos = plate.center * (Target.Profile.Radius + Target.pointOffset);
                Vector3 wpos = Target.transform.TransformPoint(lpos);
                Vector3 norm = Target.transform.TransformPoint(lpos.normalized);
                Vector3 wposEnd = Target.transform.TransformPoint(lpos + plate.motionDirection * Target.motionDirectionScale);
                Vector3 toCamera    = cameraWorldPos - wpos;
                if (Vector3.Dot(norm, toCamera) <= 0f) continue;

                var queryResult = data.Query(plate.center);
                Handles.color = GetColorFromIdx(queryResult.plateIdx);
                Handles.DrawWireCube(wpos, Vector3.one * 10);
                Handles.DrawLine(wpos, wposEnd);
                Handles.ArrowHandleCap(0, wposEnd, Quaternion.LookRotation((wposEnd - wpos).normalized), 60, EventType.Repaint);
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

        private static Color GetColorFromIdx(int id)
        {
            float hue = Mathf.Repeat(id * 0.61803398875f, 1f);
            return Color.HSVToRGB(hue, 0.75f, 1f);
        }

        private static Color GetColorFromBorder(TectonicPlateLayer.Plate.EBoundary border)
        {
            return border switch {
                TectonicPlateLayer.Plate.EBoundary.None => Color.black,
                TectonicPlateLayer.Plate.EBoundary.Convergent => Color.red,
                TectonicPlateLayer.Plate.EBoundary.Divergent => Color.green,
                TectonicPlateLayer.Plate.EBoundary.Slide => Color.yellow,
            };
        }
    }
}