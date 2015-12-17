using UnityEngine;

namespace Teatro
{
    [ExecuteInEditMode]
    public class RingRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, Range(0, 1)] float _throttle = 1.0f;

        [Space]
        [SerializeField] float _width = 0.02f;
        [SerializeField] float _minLength = 0.01f;
        [SerializeField] float _maxLength = 0.05f;

        [Space]
        [SerializeField] float _minRadius = 0.45f;
        [SerializeField] float _maxRadius = 3.0f;
        [SerializeField] float _slide = 0.2f;

        [Space]
        [SerializeField] float _arcAngle = 60.0f;
        [SerializeField] float _rotationSpeed = 90.0f;

        [Space]
        [SerializeField] float _noiseFrequency = 4.0f;
        [SerializeField] float _noiseMotion = 0.5f;

        [Space]
        [SerializeField] Color _albedoColor = Color.gray;
        [SerializeField, Range(0, 1)] float _metallic = 0.5f;
        [SerializeField, Range(0, 1)] float _smoothness = 0.5f;
        [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
        Color _emissionColor = Color.white;

        [Space]
        [SerializeField] float _randomSeed;

        [SerializeField, HideInInspector] Shader _shader;

        public float width {
            get { return _width; }
            set { _width = value; }
        }

        public float throttle {
            get { return _throttle; }
            set { _throttle = value; }
        }

        #endregion

        #region Internal Objects and Variables

        Mesh _mesh;
        Material _material;

        bool _needsReset = true;

        float _rotation;
        Vector2 _noiseOffset;

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            if (_needsReset)
            {
                ResetResources();
                _needsReset = false;
            }

            _rotation += _rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
            _noiseOffset.y += _noiseMotion * Time.deltaTime;

            _material.SetVector("_Config", new Vector2(_throttle, _randomSeed));
            _material.SetFloat("_Width", _width);
            _material.SetVector("_Length", new Vector2(_minLength, _maxLength));
            _material.SetVector("_Radius", new Vector2(_minRadius, _maxRadius));
            _material.SetFloat("_Slide", _slide);
            _material.SetVector("_Angles", new Vector2(_arcAngle * Mathf.Deg2Rad, _rotation));
            _material.SetVector("_Noise", new Vector3(_noiseFrequency, _noiseOffset.x, _noiseOffset.y));
            _material.SetColor("_Color", _albedoColor);
            _material.SetFloat("_Metallic", _metallic);
            _material.SetFloat("_Glossiness", _smoothness);
            _material.SetColor("_Emission", _emissionColor);

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

            _rotation = 60.0f + _randomSeed * 30;
            _noiseOffset = Vector3.one * _randomSeed * 11.1f;
        }

        void BuildBulkMesh()
        {
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

            _mesh = new Mesh();
            _mesh.hideFlags = HideFlags.DontSave;
            _mesh.vertices = vtx_tmp;
            _mesh.normals = nrm_tmp;
            _mesh.uv = uv0_tmp;
            _mesh.SetIndices(idx_tmp, MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
        }

        #endregion
    }
}
