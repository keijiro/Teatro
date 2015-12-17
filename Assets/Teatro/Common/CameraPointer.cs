using UnityEngine;

namespace Teatro
{
    public class CameraPointer : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        float _distanceRandomness = 0.2f;

        [SerializeField, Range(0, 180)]
        float _yawRange = 75;

        [SerializeField, Range(0, 180)]
        float _pitchRange = 30;

        [SerializeField, Range(0, 180)]
        float _bankRange = 5;

        Transform _mount;
        float _initialDistance;
        Quaternion _initialRotation;

        void Start()
        {
            _mount = transform.GetChild(0);
            _initialDistance = _mount.transform.localPosition.z;
            _initialRotation = transform.localRotation;
        }

        public void ChangePoint()
        {
            var dist = _initialDistance *
                Random.Range(1.0f - _distanceRandomness,
                             1.0f + _distanceRandomness);

            var yaw = Random.Range(-_yawRange, _yawRange);
            var pitch = Random.Range(-_pitchRange, _pitchRange);
            var bank = Random.Range(-_bankRange, _bankRange);

            var rot = Quaternion.Euler(pitch, yaw, bank);

            _mount.transform.localPosition = Vector3.forward * dist;
            transform.localRotation = rot * _initialRotation;
        }

        public void ResetPoint()
        {
            _mount.transform.localPosition = Vector3.forward * _initialDistance;
            transform.localRotation = _initialRotation;
        }
    }
}
