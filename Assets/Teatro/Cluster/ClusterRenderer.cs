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

        [Space]
        [SerializeField] Color _color = Color.white;
        [SerializeField, Range(0, 1)] float _metallic = 0.5f;
        [SerializeField, Range(0, 1)] float _smoothness = 0.5f;

        [Space]
        [SerializeField] Texture2D _albedoTexture;
        [SerializeField] float _textureScale = 1.0f;

        [Space]
        [SerializeField] Texture2D _normalTexture;
        [SerializeField, Range(0, 2)] float _normalScale = 1.0f;

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
            _material.SetTexture("_MainTex", _albedoTexture);
            _material.SetFloat("_TexScale", _textureScale);

            _material.SetFloat("_Metallic", _metallic);
            _material.SetFloat("_Glossiness", _smoothness);

            _material.SetTexture("_NormalTex", _normalTexture);
            _material.SetFloat("_NormalScale", _normalScale);

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

            _mesh = new Mesh();
            _mesh.hideFlags = HideFlags.DontSave;
            _mesh.vertices = vtx_tmp;
            _mesh.normals = nrm_tmp;
            _mesh.tangents = tan_tmp;
            _mesh.uv = uv0_tmp;
            _mesh.uv2 = uv1_tmp;
            _mesh.SetIndices(idx_tmp, MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
        }

        #endregion
    }
}
