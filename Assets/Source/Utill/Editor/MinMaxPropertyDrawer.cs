using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxPropertyDrawer : PropertyDrawer
{
    const float Spacing = 4f;
    const float SmallLabelWidth = 24f;
    const int SmallLabelFontSize = 8;

    static GUIStyle _smallLabelStyle;

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        InitStyles();
        EditorGUI.BeginProperty(rect, label, property);

        SerializedProperty minProperty = property.FindPropertyRelative("_min");
        SerializedProperty maxProperty = property.FindPropertyRelative("_max");

        rect.height = EditorGUIUtility.singleLineHeight;
        rect = EditorGUI.PrefixLabel(rect, label);

        int oldIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float sectionWidth = (rect.width - Spacing) * 0.5f;
        Rect minRect = new Rect(rect.x, rect.y, sectionWidth, rect.height);
        Rect maxRect = new Rect(rect.x + sectionWidth + Spacing, rect.y, sectionWidth, rect.height);

        DrawSmallFloatField(minRect, "min", minProperty);
        DrawSmallFloatField(maxRect, "max", maxProperty);

        EditorGUI.indentLevel = oldIndent;
        EditorGUI.EndProperty();
    }

    static void DrawSmallFloatField(Rect rect, string label, SerializedProperty property)
    {
        Rect labelRect = new Rect(rect.x, rect.y, SmallLabelWidth, rect.height);
        Rect fieldRect = new Rect(rect.x + SmallLabelWidth, rect.y, rect.width - SmallLabelWidth, rect.height);
        EditorGUI.LabelField(labelRect, label, _smallLabelStyle);
        EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
    }

    static void InitStyles()
    {
        if (_smallLabelStyle != null) return;
        _smallLabelStyle = new GUIStyle(EditorStyles.miniLabel);
        _smallLabelStyle.fontSize = SmallLabelFontSize;
        _smallLabelStyle.alignment = TextAnchor.MiddleLeft;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
