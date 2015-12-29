using UnityEngine;
using UnityEditor;
using System.IO;

namespace Teatro
{
    [CustomEditor(typeof(DiscMesh))]
    public class DiscMeshEditor : Editor
    {
        SerializedProperty _arcCount;
        SerializedProperty _ringCount;
        SerializedProperty _pointsOnArc;

        void OnEnable()
        {
            _arcCount = serializedObject.FindProperty("_arcCount");
            _ringCount = serializedObject.FindProperty("_ringCount");
            _pointsOnArc = serializedObject.FindProperty("_pointsOnArc");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_arcCount);
            EditorGUILayout.PropertyField(_ringCount);
            EditorGUILayout.PropertyField(_pointsOnArc);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (rebuild)
                foreach (var t in targets)
                    ((DiscMesh)t).RebuildMesh();
        }

        [MenuItem("Assets/Create/DiscMesh")]
        public static void CreateDiscMeshAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/DiscMesh.asset");

            // Create an asset.
            var asset = ScriptableObject.CreateInstance<DiscMesh>();
            AssetDatabase.CreateAsset(asset, assetPathName);
            AssetDatabase.AddObjectToAsset(asset.sharedMesh, asset);

            // Build an initial mesh for the asset.
            asset.RebuildMesh();

            // Save the generated mesh asset.
            AssetDatabase.SaveAssets();

            // Tweak the selection.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
