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

        public Color emissionColor {
            get { return _emissionColor; }
            set { _emissionColor = value; }
        }

        [Space]
        [SerializeField] float _randomSeed;

        [SerializeField, HideInInspector] RingMesh _mesh;
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

        Material _material;
        float _rotation;
        Vector2 _noiseOffset;

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _rotation = 60.0f + _randomSeed * 30;
            _noiseOffset = Vector3.one * _randomSeed * 11.1f;
        }

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

            Graphics.DrawMesh(_mesh.sharedMesh, transform.localToWorldMatrix, _material, 0); 
        }

        #endregion
    }
}
