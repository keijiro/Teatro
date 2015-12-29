using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    public class RingMesh : ScriptableObject
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

            var instanceCount = 65000 / 8;

            var vtx_tmp = new Vector3[instanceCount * 8];
            var nrm_tmp = new Vector3[vtx_tmp.Length];
            var uv0_tmp = new Vector2[vtx_tmp.Length];

            var i_tmp = 0;
            for (var instance = 0; instance < instanceCount; instance++)
            {
                var uv0 = new Vector2(
                    (float)(instance % 40) / 40,
                    (float)(instance / 40) * 40 / instanceCount
                );

                vtx_tmp[i_tmp + 0] = vtx_tmp[i_tmp + 4] = new Vector3(-1, 0, -1);
                vtx_tmp[i_tmp + 1] = vtx_tmp[i_tmp + 5] = new Vector3(+1, 0, -1);
                vtx_tmp[i_tmp + 2] = vtx_tmp[i_tmp + 6] = new Vector3(-1, 0, +1);
                vtx_tmp[i_tmp + 3] = vtx_tmp[i_tmp + 7] = new Vector3(+1, 0, +1);

                nrm_tmp[i_tmp + 0] = nrm_tmp[i_tmp + 1] =
                nrm_tmp[i_tmp + 2] = nrm_tmp[i_tmp + 3] = Vector3.up;
                nrm_tmp[i_tmp + 4] = nrm_tmp[i_tmp + 5] =
                nrm_tmp[i_tmp + 6] = nrm_tmp[i_tmp + 7] = -Vector3.up;

                uv0_tmp[i_tmp + 0] = uv0_tmp[i_tmp + 1] =
                uv0_tmp[i_tmp + 2] = uv0_tmp[i_tmp + 3] =
                uv0_tmp[i_tmp + 4] = uv0_tmp[i_tmp + 5] =
                uv0_tmp[i_tmp + 6] = uv0_tmp[i_tmp + 7] = uv0;

                i_tmp += 8;
            }

            var idx_tmp = new int[12 * instanceCount];

            i_tmp = 0;
            var i_0 = 0;
            for (var instance = 0; instance < instanceCount; instance++)
            {
                idx_tmp[i_tmp++] = i_0 + 0;
                idx_tmp[i_tmp++] = i_0 + 2;
                idx_tmp[i_tmp++] = i_0 + 1;

                idx_tmp[i_tmp++] = i_0 + 3;
                idx_tmp[i_tmp++] = i_0 + 1;
                idx_tmp[i_tmp++] = i_0 + 2;

                i_0 += 4;

                idx_tmp[i_tmp++] = i_0 + 0;
                idx_tmp[i_tmp++] = i_0 + 1;
                idx_tmp[i_tmp++] = i_0 + 2;

                idx_tmp[i_tmp++] = i_0 + 3;
                idx_tmp[i_tmp++] = i_0 + 2;
                idx_tmp[i_tmp++] = i_0 + 1;

                i_0 += 4;
            }

            _mesh.vertices = vtx_tmp;
            _mesh.normals = nrm_tmp;
            _mesh.uv = uv0_tmp;
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
                _mesh.name = "Ring";
            }
        }

        #endregion
    }
}
