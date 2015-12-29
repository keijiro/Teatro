using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    [ExecuteInEditMode]
    public class DiscRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, Range(0, 1)] float _transition = 1.0f;

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

        [SerializeField] DiscMesh _mesh;
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

        public Color emissionColor1 {
            get { return _emissionColor1; }
            set { _emissionColor1 = value; }
        }

        public Color emissionColor2 {
            get { return _emissionColor2; }
            set { _emissionColor2 = value; }
        }

        #endregion

        #region Internal Objects and Variables

        Material _material;
        float _rotationTime;
        float _animationTime;

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _rotationTime = Random.Range(-100.0f, 100.0f);
            _animationTime = Random.Range(-100.0f, 100.0f);
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

            Graphics.DrawMesh(
                _mesh.sharedMesh, transform.localToWorldMatrix, _material, 0); 
        }

        #endregion
    }
}
