using UnityEngine;

namespace Teatro
{
    public class SmoothFollow : MonoBehaviour
    {
        [Header("Interpolation Coefficients")]
        [UnityEngine.Serialization.FormerlySerializedAs("_speed")]
        [SerializeField, Range(0, 10)]
        float _positionRatio = 2;

        [SerializeField, Range(0, 10)]
        float _rotationRatio = 2;

        [Space(10)]
        [SerializeField]
        Transform _target;

        [Header("Random Jump")]
        [SerializeField]
        float _jumpDistance = 1;

        [SerializeField]
        float _jumpAngle = 60;

        public Transform target {
            get { return _target; }
            set { _target = value; }
        }

        void Update()
        {
            if (_positionRatio > 0)
            {
                var coeff = Mathf.Exp(-_positionRatio * Time.deltaTime);
                transform.position = Vector3.Lerp(target.position, transform.position, coeff);
            }

            if (_rotationRatio > 0 && transform.rotation != target.rotation)
            {
                var coeff = Mathf.Exp(-_rotationRatio * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(target.rotation, transform.rotation, coeff);
            }
        }

        public void JumpRandomly()
        {
            var r1 = Random.Range(0.5f, 1.0f);
            var r2 = Random.Range(0.5f, 1.0f);

            var dp = Random.onUnitSphere * _jumpDistance * r1;
            var dr = Quaternion.AngleAxis(_jumpAngle * r2, Random.onUnitSphere);

            transform.position = dp + target.position;
            transform.rotation = dr * target.rotation;
        }
    }
}
