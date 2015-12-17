using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    [ExecuteInEditMode]
    public class DiscRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, Range(0, 1)] float _transition = 1.0f;

        [SerializeField] int _arcCount = 12;
        [SerializeField] int _ringCount = 24;
        [SerializeField] int _pointsOnArc = 6;

        [SerializeField] float _rotationSpeed = 0.8f;
        [SerializeField] float _animationSpeed = 1.0f;
        [SerializeField] float _displacement = 0.5f;
        [SerializeField] float _blockHighlight = 2.0f;

        [SerializeField] Color _baseColor = Color.black;
        [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
        Color _emissionColor1 = Color.red;
        [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
        Color _emissionColor2 = Color.green;

        [SerializeField, Range(0, 1)] float _metallic = 0.5f;
        [SerializeField, Range(0, 1)] float _smoothness = 0.5f;

        [SerializeField] Texture2D _albedoTexture;
        [SerializeField] float _textureScale = 1;
        [SerializeField] Texture2D _normalTexture;
        [SerializeField, Range(0, 1)] float _normalScale = 1;

        [SerializeField] int _randomSeed;

        [SerializeField, HideInInspector] Shader _shader;

        public float transition {
            get { return _transition; }
            set { _transition = value; }
        }

        public float rotationSpeed {
            get { return _rotationSpeed; }
            set { _rotationSpeed = value; }
        }

        public float animationSpeed {
            get { return _animationSpeed; }
            set { _animationSpeed = value; }
        }

        public float displacement {
            get { return _displacement; }
            set { _displacement = value; }
        }

        #endregion

        #region Internal Objects and Variables

        Mesh _mesh;
        Material _material;
        bool _needsReset = true;
        float _rotationTime;
        float _animationTime;

        #endregion

        #region Public Methods

        public void RequestReset()
        {
            _needsReset = true;
        }

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            if (_needsReset)
            {
                ResetResources();
                _needsReset = false;
            }

            _rotationTime += _rotationSpeed * Time.deltaTime;
            _animationTime += _animationSpeed * Time.deltaTime;

            _material.SetColor("_BaseColor", _baseColor);
            _material.SetColor("_Emission1", _emissionColor1);
            _material.SetColor("_Emission2", _emissionColor2);
            _material.SetFloat("_Glossiness", _smoothness);
            _material.SetFloat("_Metallic", _metallic);

            _material.SetTexture("_MainTex", _albedoTexture);
            _material.SetFloat("_TexScale", _textureScale);
            _material.SetTexture("_NormalTex", _normalTexture);
            _material.SetFloat("_NormalScale", _normalScale);

            _material.SetVector("_TParams", new Vector2(
                _rotationTime, _animationTime
            ));

            _material.SetVector("_AParams", new Vector3(
                _transition, 0.05f * _displacement, _blockHighlight
            ));

            _material.SetFloat("_RandomSeed", _randomSeed);

            Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, _material, 0); 
        }

        #endregion

        #region Resource Management

        void ResetResources()
        {
            if (_mesh) DestroyImmediate(_mesh);
            if (_material) DestroyImmediate(_material);

            _mesh = BuildMesh();

            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;

            _rotationTime = Random.Range(-100.0f, 100.0f);
            _animationTime = Random.Range(-100.0f, 100.0f);
        }

        Mesh BuildMesh()
        {
            // sanitizing parameters
            _arcCount = Mathf.Clamp(_arcCount, 4, 64);
            _ringCount = Mathf.Clamp(_ringCount, 2, 64);
            _pointsOnArc = Mathf.Clamp(_pointsOnArc, 3, 32);

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
            var mesh = new Mesh();
            mesh.SetVertices(vtxList);
            mesh.SetUVs(0, uv0List);
            mesh.SetUVs(1, uv1List);
            mesh.SetUVs(2, uv2List);
            mesh.tangents = tanList;
            mesh.SetIndices(idxList.ToArray(), MeshTopology.Triangles, 0);
            mesh.hideFlags = HideFlags.DontSave;
            mesh.RecalculateNormals();
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            return mesh;
        }

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
    }
}
