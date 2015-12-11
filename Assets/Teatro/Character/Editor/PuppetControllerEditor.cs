using UnityEngine;
using UnityEditor;

namespace Teatro
{
    [CustomEditor(typeof(PuppetController))]
    public class PuppetControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var instance = (PuppetController)target;

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Rehash")) instance.Rehash();
            }
        }
    }
}

