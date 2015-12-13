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

        SerializedProperty _albedoTexture;
        SerializedProperty _textureScale;
        SerializedProperty _normalTexture;
        SerializedProperty _normalScale;

        void OnEnable()
        {
            _arcCount = serializedObject.FindProperty("_arcCount");
            _ringCount = serializedObject.FindProperty("_ringCount");
            _pointsOnArc = serializedObject.FindProperty("_pointsOnArc");

            _baseColor = serializedObject.FindProperty("_baseColor");
            _lineColor = serializedObject.FindProperty("_lineColor");
            _metallic = serializedObject.FindProperty("_metallic");
            _smoothness = serializedObject.FindProperty("_smoothness");

            _albedoTexture = serializedObject.FindProperty("_albedoTexture");
            _textureScale = serializedObject.FindProperty("_textureScale");
            _normalTexture = serializedObject.FindProperty("_normalTexture");
            _normalScale = serializedObject.FindProperty("_normalScale");
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

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_metallic);
            EditorGUILayout.PropertyField(_smoothness);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_albedoTexture);
            EditorGUILayout.PropertyField(_textureScale);
            EditorGUILayout.PropertyField(_normalTexture);
            EditorGUILayout.PropertyField(_normalScale);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

