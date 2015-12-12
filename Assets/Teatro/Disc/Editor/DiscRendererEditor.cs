using UnityEngine;
using UnityEditor;

namespace Teatro
{
    [CustomEditor(typeof(DiscRenderer))]
    public class DiscRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var instance = (DiscRenderer)target;

            if (!EditorApplication.isPlaying)
                if (GUILayout.Button("Rebuild"))
                    instance.RequestRebuild();
        }
    }
}

