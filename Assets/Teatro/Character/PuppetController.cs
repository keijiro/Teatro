using UnityEngine;

namespace Teatro
{
    public class PuppetController : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float _closeToOpen;
        [SerializeField, Range(0, 1)] float _noiseStrength;

        Puppet _puppet;
        XXHash _hash;
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

        public void Rehash()
        {
            _hash = new XXHash(Random.Range(0, 0x7fffffff));
        }

        void Start()
        {
            _puppet = GetComponent<Puppet>();
            _hash = new XXHash(Random.Range(0, 0x7fffffff));
            _noise = new NoiseGenerator(0.1f);
        }

        void Update()
        {
            _noise.Step();

            _puppet.spineBend = CalcValue(0, 0.9f, _hash.Value(0), 0.25f);
            _puppet.spineTwist = CalcValue(1, 0.5f, _hash.Value(1), _hash.Value(1));

            _puppet.leftArmStretch = CalcValue(2, 0, _hash.Value(2), _hash.Range(0.0f, 1.0f, 2));
            _puppet.leftArmRaise = CalcValue(3, 0.8f, _hash.Value(3), _hash.Range(0.0f, 1.0f, 2));
            _puppet.leftArmOpen = CalcValue(4, 0, _hash.Value(4), _hash.Range(1.0f, 0.0f, 2) * _hash.Value(4));

            _puppet.rightArmStretch = CalcValue(5, 0, _hash.Value(5), _hash.Range(1.0f, 0.0f, 2));
            _puppet.rightArmRaise = CalcValue(6, 0.8f, _hash.Value(6), _hash.Range(1.0f, 0.0f, 2));
            _puppet.rightArmOpen = CalcValue(7, 0, _hash.Value(7), _hash.Range(0.0f, 1.0f, 2) * _hash.Value(7));

            _puppet.leftLegStretch = CalcValue(8, 0, _hash.Value(8), 1);
            _puppet.leftLegRaise = CalcValue(9, 0.5f, _hash.Value(9), _hash.Range(0.2f, 0.8f, 9));

            _puppet.rightLegStretch = CalcValue(10, 0, _hash.Value(10), 1);
            _puppet.rightLegRaise = CalcValue(11, 0.5f, _hash.Value(11), _hash.Range(0.2f, 0.8f, 11));
        }
    }
}
