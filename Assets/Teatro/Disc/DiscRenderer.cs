using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    [ExecuteInEditMode]
    public class DiscRenderer : MonoBehaviour
    {
        [SerializeField]
        int _arcCount = 12;

        [SerializeField]
        int _ringCount = 24;

        [SerializeField]
        int _pointsOnArc = 6;

        [SerializeField]
        Color _baseColor = Color.black;

        [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
        Color _lineColor = Color.white;

        [SerializeField, Range(0, 1)]
        float _metallic = 0.5f;

        [SerializeField, Range(0, 1)]
        float _smoothness = 0.5f;

        [SerializeField, HideInInspector] Shader _shader;

        Mesh _mesh;
        Material _material;
        bool _needsReset = true;

        public void RequestReset()
        {
            _needsReset = true;
        }

        void AppendSegment(
            float l0, float l1,
            float r0, float r1,
            float depth,
            List<Vector3> vtxList,
            List<Vector2> uv0List,
            List<Vector2> uv1List,
            List<int> idxList
        )
        {
            var vi0 = vtxList.Count;
            var down = Vector3.up * depth;
            var uv1 = new Vector2(l0, r0);

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

        Mesh BuildMesh()
        {
            // parameter sanitization
            _arcCount = Mathf.Clamp(_arcCount, 4, 64);
            _ringCount = Mathf.Clamp(_ringCount, 2, 64);
            _pointsOnArc = Mathf.Clamp(_pointsOnArc, 3, 32);

            var vtxList = new List<Vector3>();
            var uv0List = new List<Vector2>();
            var uv1List = new List<Vector2>();
            var idxList = new List<int>();

            for (var ri = 0; ri < _ringCount; ri++)
            {
                var l0 = (float)(ri + 0) / (_ringCount - 1);
                var l1 = (float)(ri + 1) / (_ringCount - 1);

                for (var ai = 0; ai < _arcCount; ai++)
                {
                    var r0 = Mathf.PI * 2 * (ai + 0) / (_arcCount - 1);
                    var r1 = Mathf.PI * 2 * (ai + 1) / (_arcCount - 1);

                    AppendSegment(
                        l0, l1, r0, r1, 0.05f,
                        vtxList, uv0List, uv1List, idxList);
                }
            }

            var mesh = new Mesh();
            mesh.SetVertices(vtxList);
            mesh.SetUVs(0, uv0List);
            mesh.SetUVs(1, uv1List);
            mesh.SetIndices(idxList.ToArray(), MeshTopology.Triangles, 0);
            mesh.hideFlags = HideFlags.DontSave;
            mesh.RecalculateNormals();
            return mesh;
        }

        void SetUpResources()
        {
            if (_mesh) DestroyImmediate(_mesh);
            if (_material) DestroyImmediate(_material);

            _mesh = BuildMesh();

            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        void Update()
        {
            if (_needsReset)
            {
                SetUpResources();
                _needsReset = false;
            }

            _material.SetColor("_BaseColor", _baseColor);
            _material.SetColor("_LineColor", _lineColor);
            _material.SetFloat("_Glossiness", _smoothness);
            _material.SetFloat("_Metallic", _metallic);

            var matrix = transform.localToWorldMatrix;
            Graphics.DrawMesh(_mesh, matrix, _material, 0); 
        }
    }
}
