using UnityEngine;
using UnityEngine.Rendering;
using Emgen;

namespace Teatro
{
    [ExecuteInEditMode]
    public class CoreRenderer : MonoBehaviour
    {
        #region Public Properties

        [SerializeField]
        float _maskFrequency = 0.5f;

        public float maskFrequency {
            get { return _maskFrequency; }
            set { _maskFrequency = value; }
        }

        [SerializeField]
        float _maskMotion = 2;

        public float maskMotion {
            get { return _maskMotion; }
            set { _maskMotion = value; }
        }

        [Space]
        [SerializeField]
        float _spikeAmplitude = 8;

        public float spikeAmplitude {
            get { return _spikeAmplitude; }
            set { _spikeAmplitude = value; }
        }

        [SerializeField]
        float _spikeExponent = 8;

        public float spikeExponent {
            get { return _spikeExponent; }
            set { _spikeExponent = value; }
        }

        [SerializeField]
        float _spikeFrequency = 2;

        public float spikeFrequency {
            get { return _spikeFrequency; }
            set { _spikeFrequency = value; }
        }

        [SerializeField]
        float _spikeMotion = 2;

        public float spikeMotion {
            get { return _spikeMotion; }
            set { _spikeMotion = value; }
        }

        [Space]
        [SerializeField, Range(0, 1)]
        float _cutoff = 0.5f;

        public float cutoff {
            get { return _cutoff; }
            set { _cutoff = value; }
        }

        [Space]
        [SerializeField, ColorUsage(false, true, 0, 16, 0.125f, 3)]
        Color _color = Color.white;

        public Color color {
            get { return _color; }
            set { _color = value; }
        }

        #endregion

        #region Protected Properties

        [SerializeField]
        CoreMesh _mesh;

        [SerializeField, HideInInspector]
        Shader _shader;

        #endregion

        #region Private Members

        Material _material;
        Vector3 _maskOffset;
        Vector3 _spikeOffset;

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

            // state update
            var maskDir = new Vector3(-0.3f, -1, -0.1f).normalized;
            var spikeDir = new Vector3(1, 0.5f, 0.2f).normalized;

            _maskOffset += maskDir * (Time.deltaTime * _maskMotion);
            _spikeOffset += spikeDir * (Time.deltaTime * _spikeMotion);

            // material setup
            _material.SetVector("_MaskOffset", _maskOffset);
            _material.SetVector("_SpikeOffset", _spikeOffset);
            _material.SetFloat("_MaskFreq", _maskFrequency);
            _material.SetVector("_SpikeParams",
                new Vector3(_spikeFrequency, _spikeAmplitude, _spikeExponent));
            _material.SetFloat("_Cutoff", _cutoff);
            _material.color = _color;

            // draw
            Graphics.DrawMesh(
                _mesh.sharedMesh, transform.localToWorldMatrix, _material, 0);
        }

        #endregion
    }
}
