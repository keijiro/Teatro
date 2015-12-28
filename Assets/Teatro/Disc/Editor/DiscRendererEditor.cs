using UnityEngine;
using UnityEditor;

namespace Teatro
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DiscRenderer))]
    public class DiscRendererEditor : Editor
    {
        SerializedProperty _transition;

        SerializedProperty _rotationSpeed;
        SerializedProperty _animationSpeed;
        SerializedProperty _displacement;
        SerializedProperty _blockHighlight;

        SerializedProperty _baseColor;
        SerializedProperty _emissionColor1;
        SerializedProperty _emissionColor2;
        SerializedProperty _metallic;
        SerializedProperty _smoothness;

        SerializedProperty _albedoTexture;
        SerializedProperty _textureScale;
        SerializedProperty _normalTexture;
        SerializedProperty _normalScale;

        SerializedProperty _mesh;
        SerializedProperty _randomSeed;

        void OnEnable()
        {
            _transition = serializedObject.FindProperty("_transition");

            _rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
            _animationSpeed = serializedObject.FindProperty("_animationSpeed");
            _displacement = serializedObject.FindProperty("_displacement");
            _blockHighlight = serializedObject.FindProperty("_blockHighlight");

            _baseColor = serializedObject.FindProperty("_baseColor");
            _emissionColor1 = serializedObject.FindProperty("_emissionColor1");
            _emissionColor2 = serializedObject.FindProperty("_emissionColor2");
            _metallic = serializedObject.FindProperty("_metallic");
            _smoothness = serializedObject.FindProperty("_smoothness");

            _albedoTexture = serializedObject.FindProperty("_albedoTexture");
            _textureScale = serializedObject.FindProperty("_textureScale");
            _normalTexture = serializedObject.FindProperty("_normalTexture");
            _normalScale = serializedObject.FindProperty("_normalScale");

            _mesh = serializedObject.FindProperty("_mesh");
            _randomSeed = serializedObject.FindProperty("_randomSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_transition);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_rotationSpeed);
            EditorGUILayout.PropertyField(_animationSpeed);
            EditorGUILayout.PropertyField(_displacement);
            EditorGUILayout.PropertyField(_blockHighlight);

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

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_mesh);
            EditorGUILayout.PropertyField(_randomSeed);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
