using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    public class DiscMesh : ScriptableObject
    {
        #region Public Properties

        [SerializeField, Range(4, 64)]
        int _arcCount = 12;

        public int arcCount {
            get { return _arcCount; }
        }

        [SerializeField, Range(2, 64)]
        int _ringCount = 38;

        public int ringCount {
            get { return _ringCount; }
        }

        [SerializeField, Range(3, 32)]
        int _pointsOnArc = 5;

        public int pointsOnArc {
            get { return _pointsOnArc; }
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

            _mesh.Clear();

            // vertex and index arrays
            var vtxList = new List<Vector3>();
            var uv0List = new List<Vector2>();
            var uv1List = new List<Vector2>();
            var uv2List = new List<Vector3>();
            var idxList = new List<int>();

            // append segments to the arrays
            for (var ri = 0; ri < _ringCount; ri++)
            {
                var l0 = (ri + 0.0f) / _ringCount;
                var l1 = (ri + 1.0f) / _ringCount;

                for (var ai = 0; ai < _arcCount; ai++)
                {
                    AppendSegmentToVertexArray(
                        l0, l1,
                        Mathf.PI * 2 * (ai + 0) / _arcCount,
                        Mathf.PI * 2 * (ai + 1) / _arcCount,
                        vtxList, uv0List, uv1List, uv2List, idxList);
                }
            }

            // make tangent array
            var tan = new Vector4(1, 0, 0, 1);
            var tanList = new Vector4[vtxList.Count];
            for (var i = 0; i < tanList.Length; i++)
                tanList[i] = tan;

            // create mesh object
            _mesh.SetVertices(vtxList);
            _mesh.SetUVs(0, uv0List);
            _mesh.SetUVs(1, uv1List);
            _mesh.SetUVs(2, uv2List);
            _mesh.tangents = tanList;
            _mesh.SetIndices(idxList.ToArray(), MeshTopology.Triangles, 0);

            _mesh.RecalculateNormals();
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

            _mesh.Optimize();
            _mesh.UploadMeshData(true);
        }

        #endregion

        #region Private Methods

        void AppendSegmentToVertexArray(
            float l0, float l1,
            float r0, float r1,
            List<Vector3> vtxList,
            List<Vector2> uv0List,
            List<Vector2> uv1List,
            List<Vector3> uv2List,
            List<int> idxList
        )
        {
            var vi0 = vtxList.Count;
            var down = Vector3.up * (2.0f / _ringCount);
            var uv1 = new Vector2(r0, l0);

            // calculate the centroid
            var lm = (l0 + l1) * 0.5f;
            var rm = (r0 + r1) * 0.5f;
            var uv2 = new Vector3(Mathf.Cos(rm) * lm, 0, Mathf.Sin(rm) * lm);

            for (var i = 0; i < _pointsOnArc; i++)
            {
                var u = (float)i / (_pointsOnArc - 1);
                var r = Mathf.Lerp(r0, r1, u);
                var p = new Vector3(Mathf.Cos(r), 0, Mathf.Sin(r));

                var p0 = p * l0;
                var p1 = p * l1;

                vtxList.Add(p0 - down);
                vtxList.Add(p0);
                vtxList.Add(p0);
                vtxList.Add(p1);
                vtxList.Add(p1);
                vtxList.Add(p1 - down);

                var u0 = new Vector2(u, 0);
                var u1 = new Vector2(u, 1);

                uv0List.Add(u1);
                uv0List.Add(u0);
                uv0List.Add(u0);
                uv0List.Add(u1);
                uv0List.Add(u1);
                uv0List.Add(u0);

                uv1List.Add(uv1);
                uv1List.Add(uv1);
                uv1List.Add(uv1);
                uv1List.Add(uv1);
                uv1List.Add(uv1);
                uv1List.Add(uv1);

                uv2List.Add(uv2);
                uv2List.Add(uv2);
                uv2List.Add(uv2);
                uv2List.Add(uv2);
                uv2List.Add(uv2);
                uv2List.Add(uv2);
            }

            var vi2 = vtxList.Count;

            vtxList.Add(vtxList[vi0]);
            vtxList.Add(vtxList[vi0 + 1]);
            vtxList.Add(vtxList[vi0 + 3]);
            vtxList.Add(vtxList[vi0 + 5]);

            uv0List.Add(new Vector2(0, 0));
            uv0List.Add(new Vector2(0, 1));
            uv0List.Add(new Vector2(1, 1));
            uv0List.Add(new Vector2(1, 0));

            uv1List.Add(uv1);
            uv1List.Add(uv1);
            uv1List.Add(uv1);
            uv1List.Add(uv1);

            uv2List.Add(uv2);
            uv2List.Add(uv2);
            uv2List.Add(uv2);
            uv2List.Add(uv2);

            vtxList.Add(vtxList[vi2 - 6]);
            vtxList.Add(vtxList[vi2 - 6 + 1]);
            vtxList.Add(vtxList[vi2 - 6 + 3]);
            vtxList.Add(vtxList[vi2 - 6 + 5]);

            uv0List.Add(new Vector2(0, 0));
            uv0List.Add(new Vector2(0, 1));
            uv0List.Add(new Vector2(1, 1));
            uv0List.Add(new Vector2(1, 0));

            uv1List.Add(uv1);
            uv1List.Add(uv1);
            uv1List.Add(uv1);
            uv1List.Add(uv1);

            uv2List.Add(uv2);
            uv2List.Add(uv2);
            uv2List.Add(uv2);
            uv2List.Add(uv2);

            var vi = vi0;

            for (var i = 0; i < _pointsOnArc - 1; i++)
            {
                idxList.Add(vi + 0);
                idxList.Add(vi + 6);
                idxList.Add(vi + 1);

                idxList.Add(vi + 6);
                idxList.Add(vi + 7);
                idxList.Add(vi + 1);

                idxList.Add(vi + 0 + 2);
                idxList.Add(vi + 6 + 2);
                idxList.Add(vi + 1 + 2);

                idxList.Add(vi + 6 + 2);
                idxList.Add(vi + 7 + 2);
                idxList.Add(vi + 1 + 2);

                idxList.Add(vi + 0 + 4);
                idxList.Add(vi + 6 + 4);
                idxList.Add(vi + 1 + 4);

                idxList.Add(vi + 6 + 4);
                idxList.Add(vi + 7 + 4);
                idxList.Add(vi + 1 + 4);

                vi += 6;
            }

            idxList.Add(vi2 + 0);
            idxList.Add(vi2 + 1);
            idxList.Add(vi2 + 2);

            idxList.Add(vi2 + 0);
            idxList.Add(vi2 + 2);
            idxList.Add(vi2 + 3);

            idxList.Add(vi2 + 4);
            idxList.Add(vi2 + 6);
            idxList.Add(vi2 + 5);

            idxList.Add(vi2 + 4);
            idxList.Add(vi2 + 7);
            idxList.Add(vi2 + 6);
        }

        #endregion

        #region ScriptableObject Functions

        void OnEnable()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "Disc";
            }
        }

        #endregion
    }
}
