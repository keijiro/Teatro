using UnityEngine;

namespace Teatro
{
    public class PuppetController : MonoBehaviour
    {
        [SerializeField] float _noiseFrequency = 0.1f;
        [SerializeField, Range(0, 1)] float _closeToOpen;
        [SerializeField, Range(0, 1)] float _noiseStrength;

        public float closeToOpen {
            get { return _closeToOpen; }
            set { _closeToOpen = value; }
        }

        public float noiseStrength {
            get { return _noiseStrength; }
            set { _noiseStrength = value; }
        }

        Puppet _puppet;
        NoiseGenerator _noise;
        int _pose;

        float CalcValue(int seed, float close, float rest, float open)
        {
            var v =
                close * Mathf.Max(0.0f, 1 - _closeToOpen * 2) +
                open  * Mathf.Max(0.0f, _closeToOpen * 2 - 1) +
                rest  * (1 - Mathf.Abs(0.5f - _closeToOpen) * 2);
            return Mathf.Lerp(v, _noise.Value01(seed), _noiseStrength);
        }

        void Start()
        {
            _puppet = GetComponent<Puppet>();
            _noise = new NoiseGenerator(_noiseFrequency);
        }

        void Update()
        {
            _noise.Frequency = _noiseFrequency;
            _noise.Step();

            _puppet.spineBend       = CalcValue(0, 0.9f, 0.7f, 0.23f);
            _puppet.spineTwist      = CalcValue(1, 0.5f, 0.7f, _noise.Value01(20));

            _puppet.leftArmStretch  = CalcValue(2, 0.0f, 0.5f, 0.0f);
            _puppet.leftArmRaise    = CalcValue(3, 0.8f, 0.9f, 0.2f);
            _puppet.leftArmOpen     = CalcValue(4, 0.0f, 0.3f, 1.0f);

            _puppet.rightArmStretch = CalcValue(5, 0.0f, 0.3f, 0.7f);
            _puppet.rightArmRaise   = CalcValue(6, 0.8f, 0.8f, 0.5f);
            _puppet.rightArmOpen    = CalcValue(7, 0.0f, 0.1f, 0.7f);

            _puppet.leftLegStretch  = CalcValue(8, 0.0f, 0.7f, 0.6f);
            _puppet.leftLegRaise    = CalcValue(9, 0.5f, 0.1f, 0.1f);

            _puppet.rightLegStretch = CalcValue(10, 0.0f, 0.1f, 0.8f);
            _puppet.rightLegRaise   = CalcValue(11, 0.5f, 0.3f, 0.2f);

            _puppet.noiseToBodyPosition = Mathf.Lerp(0.15f, 0.20f, _noiseStrength);
            _puppet.noiseToBodyRotation = Mathf.Lerp(10.0f, 30.0f, _noiseStrength);
        }
    }
}
