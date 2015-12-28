using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    public class ClusterMesh : ScriptableObject
    {
        #region Public Properties

        [SerializeField]
        Mesh _sourceMesh;

        [SerializeField]
        int _instanceCount;

        public int instanceCount {
            get { return _instanceCount; }
        }

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

            if (_sourceMesh == null)
            {
                Debug.LogError("Source mesh is not specified.");
                return;
            }

            _mesh.Clear();

            var vtx_src = _sourceMesh.vertices;
            var nrm_src = _sourceMesh.normals;
            var tan_src = _sourceMesh.tangents;
            var uv0_src = _sourceMesh.uv;

            // maximum count of instances for a single mesh (65k vertices)
            _instanceCount = 65000 / vtx_src.Length;

            var vtx_tmp = new Vector3[_instanceCount * vtx_src.Length];
            var nrm_tmp = new Vector3[vtx_tmp.Length];
            var tan_tmp = new Vector4[vtx_tmp.Length];
            var uv0_tmp = new Vector2[vtx_tmp.Length];
            var uv1_tmp = new Vector2[vtx_tmp.Length];

            var i_tmp = 0;
            for (var instance = 0; instance < _instanceCount; instance++)
            {
                var uv1 = new Vector2((float)instance / _instanceCount, 0);
                for (var i_src = 0; i_src < vtx_src.Length; i_src++)
                {
                    vtx_tmp[i_tmp] = vtx_src[i_src];
                    nrm_tmp[i_tmp] = nrm_src[i_src];
                    tan_tmp[i_tmp] = tan_src[i_src];
                    uv0_tmp[i_tmp] = uv0_src[i_src];
                    uv1_tmp[i_tmp] = uv1;
                    i_tmp ++;
                }
            }

            var idx_src = _sourceMesh.GetIndices(0);
            var idx_tmp = new int[idx_src.Length * _instanceCount];

            i_tmp = 0;
            for (var instance = 0; instance < _instanceCount; instance++)
            {
                var i_0 = instance * vtx_src.Length;
                for (var i_src = 0; i_src < idx_src.Length; i_src++)
                    idx_tmp[i_tmp++] = idx_src[i_src] + i_0;
            }

            _mesh.vertices = vtx_tmp;
            _mesh.normals = nrm_tmp;
            _mesh.tangents = tan_tmp;
            _mesh.uv = uv0_tmp;
            _mesh.uv2 = uv1_tmp;
            _mesh.SetIndices(idx_tmp, MeshTopology.Triangles, 0);
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
                _mesh.name = "Cluster";
            }
        }

        #endregion
    }
}
