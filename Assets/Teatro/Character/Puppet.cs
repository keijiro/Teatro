using UnityEngine;

namespace Teatro
{
    public class Puppet : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float _noiseSpeed = 0;

        [Space]
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

        public float spineBend {
            get { return _spineBend; }
            set { _spineBend = value; }
        }

        public float spineTwist {
            get { return _spineTwist; }
            set { _spineTwist = value; }
        }

        public float leftArmStretch {
            get { return _leftArmStretch; }
            set { _leftArmStretch = value; }
        }

        public float leftArmRaise {
            get { return _leftArmRaise; }
            set { _leftArmRaise = value; }
        }

        public float leftArmOpen {
            get { return _leftArmOpen; }
            set { _leftArmOpen = value; }
        }

        public float rightArmStretch {
            get { return _rightArmStretch; }
            set { _rightArmStretch = value; }
        }

        public float rightArmRaise {
            get { return _rightArmRaise; }
            set { _rightArmRaise = value; }
        }

        public float rightArmOpen {
            get { return _rightArmOpen; }
            set { _rightArmOpen = value; }
        }

        public float leftLegStretch {
            get { return _leftLegStretch; }
            set { _leftLegStretch = value; }
        }

        public float leftLegRaise {
            get { return _leftLegRaise; }
            set { _leftLegRaise = value; }
        }

        public float rightLegStretch {
            get { return _rightLegStretch; }
            set { _rightLegStretch = value; }
        }

        public float rightLegRaise {
            get { return _rightLegRaise; }
            set { _rightLegRaise = value; }
        }

        Animator _animator;
        NoiseGenerator _noise;

        Vector3 _bodyPosition;
        Quaternion _spineRotation;
        Vector3 _leftHandPosition;
        Vector3 _leftFootPosition;
        Vector3 _rightHandPosition;
        Vector3 _rightFootPosition;

        Quaternion CalculateSpineRotation()
        {
            float a1 = (_spineBend  * 2 - 1) * 45;
            float a2 = (_spineTwist * 2 - 1) * 45;
            return
                Quaternion.AngleAxis(a1, Vector3.right) *
                Quaternion.AngleAxis(a2, Vector3.up) *
                _noise.Rotation(1, 20.0f);
        }

        Vector3 CalculateHandPosition(bool isRight)
        {
            var ry = (isRight ? _rightArmOpen : -_leftArmOpen) * 80;
            var rx = (isRight ? _rightArmRaise : _leftArmRaise) * -90 + 50;
            var s = Mathf.Lerp(0.25f, 0.5f, isRight ? _rightArmStretch : _leftArmStretch);
            var n = _noise.Vector(isRight ? 2 : 3) * 0.1f;
            return
                Quaternion.AngleAxis(ry, Vector3.up) *
                Quaternion.AngleAxis(rx, Vector3.right) *
                Vector3.forward * s + n;
        }

        Vector3 CalculateFootPosition(bool isRight)
        {
            var rx = (isRight ? _rightLegRaise : _leftLegRaise) * -110 + 45;
            var s = Mathf.Lerp(-0.3f, -0.75f, isRight ? _rightLegStretch : _leftLegStretch);
            var n = _noise.Vector(isRight ? 4 : 5) * 0.1f;
            return Quaternion.AngleAxis(rx, Vector3.right) * Vector3.up * s + n;
        }

        void Start()
        {
            _animator = GetComponent<Animator>();

            _bodyPosition = _animator.GetBoneTransform(HumanBodyBones.Hips).position;
            _noise = new NoiseGenerator(0.3f);

            _spineRotation = CalculateSpineRotation();
            _leftHandPosition = CalculateHandPosition(false);
            _leftFootPosition = CalculateFootPosition(false);
            _rightHandPosition = CalculateHandPosition(true);
            _rightFootPosition = CalculateFootPosition(true);
        }

        void Update()
        {
            _noise.Frequency = Mathf.Lerp(0.3f, 10.0f, _noiseSpeed);
            _noise.Step();

            var speed = Mathf.Lerp(6.0f, 20.0f, _noiseSpeed);
            _spineRotation = ExpEase.Out(_spineRotation, CalculateSpineRotation(), speed);
            _leftHandPosition = ExpEase.Out(_leftHandPosition, CalculateHandPosition(false), speed);
            _leftFootPosition = ExpEase.Out(_leftFootPosition, CalculateFootPosition(false), speed);
            _rightHandPosition = ExpEase.Out(_rightHandPosition, CalculateHandPosition(true), speed);
            _rightFootPosition = ExpEase.Out(_rightFootPosition, CalculateFootPosition(true), speed);
        }

        void OnAnimatorMove()
        {
            _animator.bodyPosition = _bodyPosition + _noise.Vector(6) * 0.15f;
            _animator.bodyRotation = _noise.Rotation(7, 10.0f);
        }

        void OnAnimatorIK(int layerIndex)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

            _animator.SetBoneLocalRotation(HumanBodyBones.Spine, _spineRotation);
            _animator.SetBoneLocalRotation(HumanBodyBones.Chest, _spineRotation);

            var hip = _animator.GetBoneTransform(HumanBodyBones.Hips);
            var larm = _animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            var rarm = _animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            var lleg = _animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
            var rleg = _animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;

            var lhp = larm.TransformPoint(_leftHandPosition);
            var rhp = rarm.TransformPoint(_rightHandPosition);
            var lfp = hip.TransformPoint(hip.InverseTransformPoint(lleg) + _leftFootPosition);
            var rfp = hip.TransformPoint(hip.InverseTransformPoint(rleg) + _rightFootPosition);

            _animator.SetIKPosition(AvatarIKGoal.LeftHand, lhp);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, rhp);
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, lfp);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, rfp);
        }
    }
}
