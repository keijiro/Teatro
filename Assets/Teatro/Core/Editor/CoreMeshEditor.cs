using UnityEngine;
using UnityEditor;
using System.IO;

namespace Teatro
{
    [CustomEditor(typeof(CoreMesh))]
    public class CoreMeshEditor : Editor
    {
        SerializedProperty _subdivision;

        void OnEnable()
        {
            _subdivision = serializedObject.FindProperty("_subdivision");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_subdivision);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (rebuild)
                foreach (var t in targets)
                    ((CoreMesh)t).RebuildMesh();
        }

        [MenuItem("Assets/Create/CoreMesh")]
        public static void CreateCoreMeshAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/CoreMesh.asset");

            // Create an asset.
            var asset = ScriptableObject.CreateInstance<CoreMesh>();
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
