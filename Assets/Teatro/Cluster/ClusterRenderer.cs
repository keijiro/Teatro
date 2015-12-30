using UnityEngine;

namespace Teatro
{
    [ExecuteInEditMode]
    public class ClusterRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, Range(0, 1)] float _throttle;
        [SerializeField, Range(0, 1)] float _transition;

        public float throttle {
            get { return _throttle; }
            set { _throttle = value; }
        }

        public float transition {
            get { return _transition; }
            set { _transition = value; }
        }

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
        [SerializeField] ClusterMesh _mesh;

        public float scale {
            get { return _scale; }
            set { _scale = value; }
        }

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

        Material _material;

        #endregion

        #region MonoBehaviour Functions

        void OnDestroy()
        {
            if (_material) DestroyImmediate(_material);
        }

        void Update()
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetFloat("_InstanceCount", _mesh.instanceCount);

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

            Graphics.DrawMesh(
                _mesh.sharedMesh, transform.localToWorldMatrix, _material, 0); 
        }

        #endregion
    }
}
