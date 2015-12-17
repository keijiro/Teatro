using UnityEngine;

namespace Teatro
{
    public class Puppet : MonoBehaviour
    {
        #region Exposed Parameters

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

        [Space]
        [SerializeField] float _noiseFrequency = 0.3f;

        [Space]
        [SerializeField] float _noiseToBodyPosition = 0.15f;
        [SerializeField] float _noiseToBodyRotation = 10.0f;
        [SerializeField] float _noiseToSpineRotation = 20.0f;
        [SerializeField] float _noiseToIKTarget = 0.1f;

        #endregion

        #region Public Properties

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

        public float noiseToBodyPosition {
            get { return _noiseToBodyPosition; }
            set { _noiseToBodyPosition = value; }
        }

        public float noiseToBodyRotation {
            get { return _noiseToBodyRotation; }
            set { _noiseToBodyRotation = value; }
        }

        #endregion

        #region Private Variables

        Animator _animator;
        NoiseGenerator _noise;

        Vector3 _originalBodyPosition;
        Vector3 _bodyPosition;
        Quaternion _bodyRotation;

        Quaternion _spineRotation;

        Vector3 _leftHandPosition;
        Vector3 _leftFootPosition;

        Vector3 _rightHandPosition;
        Vector3 _rightFootPosition;

        #endregion

        #region Utility Functions

        static Vector3 Tween(Vector3 current, Vector3 target)
        {
            return Vector3.Lerp(target, current, Mathf.Exp(-6 * Time.deltaTime));
        }

        static Quaternion Tween(Quaternion current, Quaternion target)
        {
            if (current == target)
                return target;
            else
                return Quaternion.Lerp(target, current, Mathf.Exp(-6 * Time.deltaTime));
        }

        #endregion

        #region Pose Calculation Functions

        Quaternion CalculateSpineRotation()
        {
            float a1 = (_spineBend  * 2 - 1) * 45;
            float a2 = (_spineTwist * 2 - 1) * 45;
            return
                Quaternion.AngleAxis(a1, Vector3.right) *
                Quaternion.AngleAxis(a2, Vector3.up) *
                _noise.Rotation(1, _noiseToSpineRotation);
        }

        Vector3 CalculateHandPosition(bool right)
        {
            var ry = (right ? _rightArmOpen : -_leftArmOpen) * 80;
            var rx = (right ? _rightArmRaise : _leftArmRaise) * -90 + 50;
            var s = Mathf.Lerp(0.25f, 0.5f, right ? _rightArmStretch : _leftArmStretch);
            var n = _noise.Vector(right ? 2 : 3) * _noiseToIKTarget;
            return
                Quaternion.AngleAxis(ry, Vector3.up) *
                Quaternion.AngleAxis(rx, Vector3.right) *
                Vector3.forward * s + n;
        }

        Vector3 CalculateFootPosition(bool right)
        {
            var rx = (right ? _rightLegRaise : _leftLegRaise) * -110 + 45;
            var s = Mathf.Lerp(-0.3f, -0.75f, right ? _rightLegStretch : _leftLegStretch);
            var o = (right ? _rightLegStretch : -_leftLegStretch) * 0.15f;
            var n = _noise.Vector(right ? 4 : 5) * _noiseToIKTarget;
            return
                Quaternion.AngleAxis(rx, Vector3.right) *
                (Vector3.up * s + Vector3.right * o) + n;
        }

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _animator = GetComponent<Animator>();
            _noise = new NoiseGenerator(_noiseFrequency);

            var hip = _animator.GetBoneTransform(HumanBodyBones.Hips);
            _originalBodyPosition = _bodyPosition = hip.position;

            _spineRotation = CalculateSpineRotation();

            _leftHandPosition = CalculateHandPosition(false);
            _leftFootPosition = CalculateFootPosition(false);

            _rightHandPosition = CalculateHandPosition(true);
            _rightFootPosition = CalculateFootPosition(true);
        }

        void Update()
        {
            _noise.Frequency = _noiseFrequency;
            _noise.Step();

            _spineRotation = Tween(_spineRotation, CalculateSpineRotation());

            _leftHandPosition = Tween(_leftHandPosition, CalculateHandPosition(false));
            _leftFootPosition = Tween(_leftFootPosition, CalculateFootPosition(false));

            _rightHandPosition = Tween(_rightHandPosition, CalculateHandPosition(true));
            _rightFootPosition = Tween(_rightFootPosition, CalculateFootPosition(true));
        }

        void OnAnimatorMove()
        {
            var bp = _originalBodyPosition + _noise.Vector(6) * _noiseToBodyPosition;
            var br = _noise.Rotation(7, _noiseToBodyRotation);

            _bodyPosition = Tween(_bodyPosition, bp);
            _bodyRotation = Tween(_bodyRotation, br);

            _animator.bodyPosition = _bodyPosition;
            _animator.bodyRotation = _bodyRotation;
        }

        void OnAnimatorIK(int layerIndex)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

            _animator.SetBoneLocalRotation(HumanBodyBones.Spine, _spineRotation);
            _animator.SetBoneLocalRotation(HumanBodyBones.Chest, _spineRotation);

            var hip  = _animator.GetBoneTransform(HumanBodyBones.Hips);
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

        #endregion
    }
}
