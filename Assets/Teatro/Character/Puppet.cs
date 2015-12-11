using UnityEngine;

namespace Teatro
{
    public class Puppet : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float _spineBend = 0.5f;
        [SerializeField, Range(0, 1)] float _spineTwist = 0.5f;

        [Space]
        [SerializeField, Range(0, 1)] float _leftArmStretch = 0;
        [SerializeField, Range(0, 1)] float _leftArmRaise = 0.5f;
        [SerializeField, Range(0, 1)] float _leftArmOpen = 0;

        [Space]
        [SerializeField, Range(0, 1)] float _rightArmStretch = 0;
        [SerializeField, Range(0, 1)] float _rightArmRaise = 0.5f;
        [SerializeField, Range(0, 1)] float _rightArmOpen = 0;

        [Space]
        [SerializeField, Range(0, 1)] float _leftLegStretch = 0;
        [SerializeField, Range(0, 1)] float _leftLegRaise = 0.5f;

        [Space]
        [SerializeField, Range(0, 1)] float _rightLegStretch = 0;
        [SerializeField, Range(0, 1)] float _rightLegRaise = 0.5f;

        Vector3 _bodyPosition;

        class Jitter
        {
            float _position;

            public float NormalizedValue {
                get {
                    return Perlin.Fbm(_position, 2) * (1 / 0.75f);
                }
            }

            public Jitter()
            {
                _position = Random.Range(-100.0f, 100.0f);
            }

            public void Step()
            {
                _position += Time.deltaTime * 0.2f;
            }
        }

        class Jitter3D
        {
            Jitter _jitter1, _jitter2, _jitter3;

            public float NormalizedValue1 { get { return _jitter1.NormalizedValue; } }
            public float NormalizedValue2 { get { return _jitter2.NormalizedValue; } }
            public float NormalizedValue3 { get { return _jitter3.NormalizedValue; } }

            public Vector3 NormalizedVector {
                get {
                    return new Vector3(
                        NormalizedValue1,
                        NormalizedValue2,
                        NormalizedValue3
                    );
                }
            }

            public Jitter3D()
            {
                _jitter1 = new Jitter();
                _jitter2 = new Jitter();
                _jitter3 = new Jitter();
            }

            public void Step()
            {
                _jitter1.Step();
                _jitter2.Step();
                _jitter3.Step();
            }
        }

        Jitter3D _bodyJitter;
        Jitter3D _leftHandJitter;
        Jitter3D _rightHandJitter;
        Jitter3D _leftFootJitter;
        Jitter3D _rightFootJitter;

        void Start()
        {
            _bodyJitter = new Jitter3D();
            _leftHandJitter = new Jitter3D();
            _rightHandJitter = new Jitter3D();
            _leftFootJitter = new Jitter3D();
            _rightFootJitter = new Jitter3D();

            var animator = GetComponent<Animator>();
            _bodyPosition = animator.GetBoneTransform(HumanBodyBones.Hips).position;
        }

        void Update()
        {
            _bodyJitter.Step();
            _leftHandJitter.Step();
            _rightHandJitter.Step();
            _leftFootJitter.Step();
            _rightFootJitter.Step();
        }

        void OnAnimatorMove()
        {
            var animator = GetComponent<Animator>();
            animator.bodyPosition = _bodyPosition + _bodyJitter.NormalizedVector * 0.2f;
        }

        void OnAnimatorIK(int layerIndex)
        {
            var animator = GetComponent<Animator>();

            var spine =
                Quaternion.AngleAxis((_spineBend  * 2 - 1) * 45, Vector3.right) *
                Quaternion.AngleAxis((_spineTwist * 2 - 1) * 45, Vector3.up);
            animator.SetBoneLocalRotation(HumanBodyBones.Spine, spine);
            animator.SetBoneLocalRotation(HumanBodyBones.Chest, spine);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

            var larm = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            var rarm = animator.GetBoneTransform(HumanBodyBones.RightShoulder);

            var lhp = larm.TransformPoint(
                Quaternion.AngleAxis(-_leftArmOpen * 80, Vector3.up) *
                Quaternion.AngleAxis(-_leftArmRaise * 90 + 50, Vector3.right) *
                Vector3.forward * Mathf.Lerp(0.25f, 0.5f, _leftArmStretch));
            
            var rhp = rarm.TransformPoint(
                Quaternion.AngleAxis(+_rightArmOpen * 80, Vector3.up) *
                Quaternion.AngleAxis(-_rightArmRaise * 90 + 50, Vector3.right) *
                Vector3.forward * Mathf.Lerp(0.25f, 0.5f, _rightArmStretch));

            var hip = animator.GetBoneTransform(HumanBodyBones.Hips);
            var lul = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
            var rul = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;

            var lfp = hip.TransformPoint(
                hip.InverseTransformPoint(lul) +
                Quaternion.AngleAxis(-_leftLegRaise * 110 + 45, Vector3.right) *
                -Vector3.up * Mathf.Lerp(0.3f, 0.75f, _leftLegStretch));

            var rfp = hip.TransformPoint(
                hip.InverseTransformPoint(rul) +
                Quaternion.AngleAxis(-_rightLegRaise * 110 + 45, Vector3.right) *
                -Vector3.up * Mathf.Lerp(0.3f, 0.75f, _rightLegStretch));

            animator.SetIKPosition(AvatarIKGoal.LeftHand, lhp + _leftHandJitter.NormalizedVector * 0.05f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rhp + _rightHandJitter.NormalizedVector * 0.05f);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, lfp + _leftFootJitter.NormalizedVector * 0.05f);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rfp + _rightFootJitter.NormalizedVector * 0.05f);
        }
    }
}
