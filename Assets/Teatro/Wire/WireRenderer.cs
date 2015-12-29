using UnityEngine;

namespace Teatro
{
    [ExecuteInEditMode]
    public class WireRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
        Color _color = Color.white;

        [Space]
        [SerializeField] float _minRadius = 1;
        [SerializeField] float _maxRadius = 4;
        [SerializeField] float _windingNumber = 32;

        [Space]
        [SerializeField] float _waveAmplitude = 0.2f;
        [SerializeField] float _waveFrequency = 0.4f;
        [SerializeField] float _waveSpeed = 4;

        [Space]
        [SerializeField] float _noiseAmplitude = 0.3f;
        [SerializeField] float _noiseFrequency = 0.6f;
        [SerializeField] float _noiseSpeed = 0.6f;
        [SerializeField] float _noiseThreshold = 0;

        [HideInInspector, SerializeField] WireMesh _mesh;
        [HideInInspector, SerializeField] Shader _shader;

        public float minRadius {
            get { return _minRadius; }
            set { _minRadius = value; }
        }

        public float maxRadius {
            get { return _maxRadius; }
            set { _maxRadius = value; }
        }

        public float windingNumber {
            get { return _windingNumber; }
            set { _windingNumber = value; }
        }

        public float noiseAmplitude {
            get { return _noiseAmplitude; }
            set { _noiseAmplitude = value; }
        }

        public float noiseFrequency {
            get { return _noiseFrequency; }
            set { _noiseFrequency = value; }
        }

        public float noiseSpeed {
            get { return _noiseSpeed; }
            set { _noiseSpeed = value; }
        }

        public float noiseThreshold {
            get { return _noiseThreshold; }
            set { _noiseThreshold = value; }
        }

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

            _material.SetVector("_Radius", new Vector2(_minRadius, _maxRadius));

            _material.SetVector("_RingParams", new Vector4(
                Mathf.PI * 2 * _windingNumber,
                _waveAmplitude, _waveFrequency, _waveSpeed
            ));

            _material.SetVector("_NoiseParams", new Vector4(
                _noiseAmplitude, _noiseFrequency, _noiseSpeed, _noiseThreshold
            ));

            _material.SetColor("_Color", _color);

            var matrix = transform.localToWorldMatrix;
            Graphics.DrawMesh(_mesh.sharedMesh, matrix, _material, 0); 
        }

        #endregion
    }
}
