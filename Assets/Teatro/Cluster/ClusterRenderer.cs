using UnityEngine;

namespace Teatro
{
    [ExecuteInEditMode]
    public class ClusterRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, Range(0, 1)] float _throttle;
        [SerializeField, Range(0, 1)] float _transition;

        [Space]
        [SerializeField] float _angularSpeed = 180.0f;
        [SerializeField] float _minRadius = 0.5f;
        [SerializeField] float _maxRadius = 1.0f;
        [SerializeField] float _height = 2.0f;

        [Space]
        [SerializeField] float _scatter = 20.0f;

        [Space]
        [SerializeField] float _noiseFrequency = 0.5f;
        [SerializeField] float _noiseMotion = 1.0f;

        [Space]
        [SerializeField] float _scale = 0.05f;
        [SerializeField] Mesh _sourceMesh;
        [SerializeField] Color _color = Color.white;
        [SerializeField, Range(0, 1)] float _metallic = 0.5f;
        [SerializeField, Range(0, 1)] float _smoothness = 0.5f;

        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Internal Objects and Variables

        Mesh _mesh;
        Material _material;
        int _instanceCount;
        bool _needsReset = true;

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            if (_needsReset && _sourceMesh)
            {
                ResetResources();
                _needsReset = false;
            }

            _material.SetFloat("_InstanceCount", _instanceCount);

            _material.SetFloat("_Throttle", _throttle);
            _material.SetFloat("_Transition", _transition);

            _material.SetVector("_TubeParams", new Vector4(
                _angularSpeed * Mathf.Deg2Rad,
                _minRadius, _maxRadius, _height
            ));

            _material.SetFloat("_Scatter", _scatter);

            _material.SetVector("_NoiseParams", new Vector2(
                _noiseFrequency, _noiseMotion
            ));

            _material.SetFloat("_Scale", _scale);
            _material.SetColor("_Color", _color);
            _material.SetFloat("_Metallic", _metallic);
            _material.SetFloat("_Glossiness", _smoothness);

            Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, _material, 0); 
        }

        #endregion

        #region Resource Management

        void ResetResources()
        {
            if (_mesh) DestroyImmediate(_mesh);
            if (_material) DestroyImmediate(_material);

            BuildBulkMesh();

            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        void BuildBulkMesh()
        {
            var va_src = _sourceMesh.vertices;
            var na_src = _sourceMesh.normals;

            // maximum count of instances for a single mesh (65k vertices)
            _instanceCount = 65000 / va_src.Length;

            var va_tmp = new Vector3[_instanceCount * va_src.Length];
            var na_tmp = new Vector3[va_tmp.Length];
            var ta_tmp = new Vector2[va_tmp.Length];

            var i = 0;
            for (var i_i = 0; i_i < _instanceCount; i_i++)
            {
                var uv = new Vector2((float)i_i / _instanceCount, 0);
                for (var i_va = 0; i_va < va_src.Length; i_va++)
                {
                    va_tmp[i] = va_src[i_va];
                    na_tmp[i] = na_src[i_va];
                    ta_tmp[i] = uv;
                    i++;
                }
            }

            var ia_src = _sourceMesh.GetIndices(0);
            var ia_tmp = new int[ia_src.Length * _instanceCount];

            i = 0;
            for (var i_i = 0; i_i < _instanceCount; i_i++)
            {
                var i0 = i_i * va_src.Length;
                for (var i_ia = 0; i_ia < ia_src.Length; i_ia++)
                    ia_tmp[i++] = ia_src[i_ia] + i0;
            }

            _mesh = new Mesh();
            _mesh.hideFlags = HideFlags.DontSave;
            _mesh.vertices = va_tmp;
            _mesh.normals = na_tmp;
            _mesh.uv = ta_tmp;
            _mesh.SetIndices(ia_tmp, MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
        }

        #endregion
    }
}
