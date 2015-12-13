using UnityEngine;
using UnityEditor;

namespace Teatro
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DiscRenderer))]
    public class DiscRendererEditor : Editor
    {
        SerializedProperty _arcCount;
        SerializedProperty _ringCount;
        SerializedProperty _pointsOnArc;

        SerializedProperty _rotationSpeed;
        SerializedProperty _animationSpeed;
        SerializedProperty _displacement;
        SerializedProperty _blockIntensity;

        SerializedProperty _baseColor;
        SerializedProperty _emissionColor1;
        SerializedProperty _emissionColor2;
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

            _rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
            _animationSpeed = serializedObject.FindProperty("_animationSpeed");
            _displacement = serializedObject.FindProperty("_displacement");
            _blockIntensity = serializedObject.FindProperty("_blockIntensity");

            _baseColor = serializedObject.FindProperty("_baseColor");
            _emissionColor1 = serializedObject.FindProperty("_emissionColor1");
            _emissionColor2 = serializedObject.FindProperty("_emissionColor2");
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

            EditorGUILayout.PropertyField(_rotationSpeed);
            EditorGUILayout.PropertyField(_animationSpeed);
            EditorGUILayout.PropertyField(_displacement);
            EditorGUILayout.PropertyField(_blockIntensity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_baseColor);
            EditorGUILayout.PropertyField(_emissionColor1);
            EditorGUILayout.PropertyField(_emissionColor2);
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

