using UnityEngine;
using UnityEditor;
using System.IO;

namespace Teatro
{
    [CustomEditor(typeof(ClusterMesh))]
    public class ClusterMeshEditor : Editor
    {
        SerializedProperty _sourceMesh;

        void OnEnable()
        {
            _sourceMesh = serializedObject.FindProperty("_sourceMesh");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_sourceMesh);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (rebuild)
                foreach (var t in targets)
                    ((ClusterMesh)t).RebuildMesh();
        }

        [MenuItem("Assets/Create/ClusterMesh")]
        public static void CreateClusterMeshAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/ClusterMesh.asset");

            // Create an asset.
            var asset = ScriptableObject.CreateInstance<ClusterMesh>();
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
