using UnityEngine;

namespace Teatro
{
    [RequireComponent(typeof(Kvant.Spray))]
    public class SprayController : MonoBehaviour
    {
        [SerializeField]
        Transform _target;

        [SerializeField]
        bool _enableRotation;

        Kvant.Spray _spray;
        float _initialSpeed;

        void Start()
        {
            _spray = GetComponent<Kvant.Spray>();
            _initialSpeed = _spray.initialVelocity.magnitude;
        }

        void Update()
        {
            _spray.emitterCenter = _target.position;

            if (_enableRotation)
                _spray.initialVelocity = _target.forward * _initialSpeed;
        }
    }
}

