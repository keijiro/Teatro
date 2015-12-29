using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    public class WireMesh : ScriptableObject
    {
        #region Public Properties

        [SerializeField, HideInInspector]
        Mesh _mesh;

        public Mesh sharedMesh {
            get { return _mesh; }
        }

        #endregion

        #region Public Methods

        public void RebuildMesh()
        {
            if (_mesh == null)
            {
                Debug.LogError("Mesh asset is missing.");
                return;
            }

            _mesh.Clear();

            const int vcount = 65000;

            var va = new Vector3[vcount];
            var ta = new Vector2[vcount];

            for (var i = 0; i < vcount; i++)
                ta[i] = new Vector2((float)i / vcount, 0);

            var ia = new int[vcount];

            for (var i = 0; i < vcount; i++)
                ia[i] = i;

            _mesh.vertices = va;
            _mesh.uv = ta;
            _mesh.SetIndices(ia, MeshTopology.LineStrip, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

            _mesh.Optimize();
            _mesh.UploadMeshData(true);
        }

        #endregion

        #region ScriptableObject Functions

        void OnEnable()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "Wire";
            }
        }

        #endregion
    }
}
