using UnityEngine;
using UnityEditor;
using System.IO;

namespace Teatro
{
    [CustomEditor(typeof(RingMesh))]
    public class RingMeshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // there is nothing to show!
        }

        [MenuItem("Assets/Create/RingMesh")]
        public static void CreateRingMeshAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/RingMesh.asset");

            // Create an asset.
            var asset = ScriptableObject.CreateInstance<RingMesh>();
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
