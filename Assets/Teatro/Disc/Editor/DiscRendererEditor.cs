using UnityEngine;
using UnityEditor;

namespace Teatro
{
    [CustomEditor(typeof(DiscRenderer))]
    public class DiscRendererEditor : Editor
    {
        SerializedProperty _arcCount;
        SerializedProperty _ringCount;
        SerializedProperty _pointsOnArc;

        SerializedProperty _baseColor;
        SerializedProperty _lineColor;
        SerializedProperty _metallic;
        SerializedProperty _smoothness;

        void OnEnable()
        {
            _arcCount = serializedObject.FindProperty("_arcCount");
            _ringCount = serializedObject.FindProperty("_ringCount");
            _pointsOnArc = serializedObject.FindProperty("_pointsOnArc");

            _baseColor = serializedObject.FindProperty("_baseColor");
            _lineColor = serializedObject.FindProperty("_lineColor");
            _metallic = serializedObject.FindProperty("_metallic");
            _smoothness = serializedObject.FindProperty("_smoothness");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_arcCount);
            EditorGUILayout.PropertyField(_ringCount);
            EditorGUILayout.PropertyField(_pointsOnArc);

            if (EditorGUI.EndChangeCheck())
                ((DiscRenderer)target).RequestReset();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_baseColor);
            EditorGUILayout.PropertyField(_lineColor);
            EditorGUILayout.PropertyField(_metallic);
            EditorGUILayout.PropertyField(_smoothness);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

