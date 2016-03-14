using UnityEngine;

namespace Teatro
{
    public class Puppet : MonoBehaviour
    {
        #region Common Parameters

        [Space, Range(0, 1)]
        [SerializeField] float _transition;

        public float transition {
            get { return _transition; }
            set { _transition = value; }
        }

        [SerializeField] float _noise1Frequency;
        [SerializeField] float _noise2Frequency;
        [SerializeField] float _noise3Frequency;

        #endregion

        #region Private Properties

        Animator _animator;
        NoiseGenerator _noise1;
        NoiseGenerator _noise2;
        NoiseGenerator _noise3;

        float pose1Weight {
            get { return Mathf.Max(1 - _transition * 2, 0.0f); }
        }

        float pose2Weight {
            get { return 1 - Mathf.Abs(0.5f - _transition) * 2; }
        }

        float pose3Weight {
            get { return Mathf.Max(_transition * 2 - 1, 0.0f); }
        }

        float ApplyPoseWeights(float s1, float s2, float s3)
        {
            return s1 * pose1Weight + s2 * pose2Weight + s3 * pose3Weight;
        }

        Vector3 ApplyPoseWeights(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return v1 * pose1Weight + v2 * pose2Weight + v3 * pose3Weight;
        }

        Quaternion ApplyPoseWeights(Quaternion q1, Quaternion q2, Quaternion q3)
        {
            if (pose1Weight > 0)
                return Quaternion.Slerp(q1, q2, pose2Weight);
            else
                return Quaternion.Slerp(q3, q2, pose2Weight);
        }

        #endregion

        #region Body (Root Joint)

        [Header("Root Joint")]
        [SerializeField] float _noiseToBodyPosition1;
        [SerializeField] float _noiseToBodyRotation1;
        [Space]
        [SerializeField] float _noiseToBodyPosition2;
        [SerializeField] float _noiseToBodyRotation2;
        [Space]
        [SerializeField] float _noiseToBodyPosition3;
        [SerializeField] float _noiseToBodyRotation3;

        Vector3 CalculateBodyPosition()
        {
            var n1 = _noise1.Vector(0) * _noiseToBodyPosition1;
            var n2 = _noise2.Vector(1) * _noiseToBodyPosition2;
            var n3 = _noise3.Vector(2) * _noiseToBodyPosition3;
            return ApplyPoseWeights(n1, n2, n3);
        }

        Quaternion CalculateBodyRotation()
        {
            var n1 = _noise1.Rotation(3, _noiseToBodyRotation1);
            var n2 = _noise2.Rotation(4, _noiseToBodyRotation2);
            var n3 = _noise3.Rotation(5, _noiseToBodyRotation3);
            return ApplyPoseWeights(n1, n2, n3);
        }

        #endregion

        #region Spine Joints

        [Header("Spine Joints")]
        [SerializeField] float _spineBend1;
        [SerializeField] Vector3 _noiseToSpineRotation1;
        [Space]
        [SerializeField] float _spineBend2;
        [SerializeField] Vector3 _noiseToSpineRotation2;
        [Space]
        [SerializeField] float _spineBend3;
        [SerializeField] Vector3 _noiseToSpineRotation3;

        Quaternion CalculateSpineRotation()
        {
            var r1 = Vector3.Scale(_noise1.Vector(10), _noiseToSpineRotation1);
            r1.x += _spineBend1;

            var r2 = Vector3.Scale(_noise2.Vector(11), _noiseToSpineRotation2);
            r2.x += _spineBend2;

            var r3 = Vector3.Scale(_noise3.Vector(12), _noiseToSpineRotation3);
            r3.x += _spineBend3;

            return Quaternion.Euler(ApplyPoseWeights(r1, r2, r3));
        }

        #endregion

        #region Hand IK Target

        [Header("Hand IK Target")]
        [SerializeField] Vector3 _handTarget1;
        [SerializeField] float _noiseToHandTarget1;
        [Space]
        [SerializeField] Vector3 _handTarget2;
        [SerializeField] Vector3 _noiseToHandTarget2;
        [Space]
        [SerializeField] Vector3 _leftHandTarget3;
        [SerializeField] Vector3 _rightHandTarget3;
        [SerializeField] float _noiseToHandTarget3;
        [Space]
        [SerializeField] float _noiseToHandRotation;

        Vector3 _leftHandPosition;
        Vector3 _rightHandPosition;

        Vector3 CalculateHandPosition(bool right)
        {
            var hip = _animator.GetBoneTransform(HumanBodyBones.Hips);
            var neck = _animator.GetBoneTransform(HumanBodyBones.Neck);

            // pose 1
            var p1 = _handTarget1;
            p1 += _noise1.Vector(right ? 20 : 21) * _noiseToHandTarget1;
            if (!right) p1.x *= -1;
            p1 = hip.TransformPoint(p1);

            // pose 2
            var p2 = _handTarget2;
            var n2 = _noise2.Vector(right ? 20 : 21);
            p2 += Vector3.Scale(n2, _noiseToHandTarget2);
            if (!right) p2.x *= -1;
            p2 = neck.TransformPoint(p2);

            // pose 3
            var p3 = right ? _rightHandTarget3 : _leftHandTarget3;
            p3 += _noise3.Vector(right ? 22 : 23) * _noiseToHandTarget3;
            p3 = hip.TransformPoint(p3);

            return ApplyPoseWeights(p1, p2, p3);
        }

        Quaternion CalculateHandRotation(bool right)
        {
            return _noise1.Rotation(right ? 24 : 25, _noiseToHandRotation);
        }

        #endregion

        #region Foot IK Target

        [Header("Foot IK Target")]
        [SerializeField] Vector3 _FootTarget1;
        [SerializeField] float _noiseToFootTarget1;
        [Space]
        [SerializeField] Vector3 _FootTarget2;
        [SerializeField] Vector3 _noiseToFootTarget2;
        [Space]
        [SerializeField] Vector3 _leftFootTarget3;
        [SerializeField] Vector3 _rightFootTarget3;
        [SerializeField] float _noiseToFootTarget3;
        [Space]
        [SerializeField] float _noiseToFootRotation;

        Vector3 _leftFootPosition;
        Vector3 _rightFootPosition;

        Vector3 CalculateFootPosition(bool right)
        {
            var hip = _animator.GetBoneTransform(HumanBodyBones.Hips);

            // pose 1
            var p1 = _FootTarget1;
            p1 += _noise1.Vector(right ? 30 : 31) * _noiseToFootTarget1;
            if (!right) p1.x *= -1;

            // pose 2
            var p2 = _FootTarget2;
            var n2 = _noise2.Vector(right ? 30 : 31);
            p2 += Vector3.Scale(n2, _noiseToFootTarget2);
            if (!right) p2.x *= -1;

            // pose 3
            var p3 = right ? _rightFootTarget3 : _leftFootTarget3;
            p3 += _noise3.Vector(right ? 32 : 33) * _noiseToFootTarget3;

            return hip.TransformPoint(ApplyPoseWeights(p1, p2, p3));
        }

        Quaternion CalculateFootRotation(bool right)
        {
            var r = _noise2.Value01(right ? 34 : 35) * _noiseToFootRotation;
            return Quaternion.AngleAxis(r, Vector3.right);
        }

        #endregion

        #region Finger Joints

        [Header("Fingers")]
        [SerializeField] float _noiseToThumbRotation;
        [SerializeField] float _noiseToFingerRotation;

        void UpdateFinger(
            int noiseSeed, bool thumb, bool right,
            HumanBodyBones bone1, HumanBodyBones bone2, HumanBodyBones bone3)
        {
            var a = thumb ? Vector3.up : Vector3.forward;
            var r = thumb ? _noiseToThumbRotation : _noiseToFingerRotation;
            var n = _noise2.Value01(noiseSeed) * (right ? -1 : 1);
            var q = Quaternion.AngleAxis(r * n, a);
            _animator.SetBoneLocalRotation(bone1, q);
            _animator.SetBoneLocalRotation(bone2, q);
            _animator.SetBoneLocalRotation(bone3, q);
        }

        void UpdateAllFingers()
        {
            UpdateFinger(40, true, false,
                HumanBodyBones.LeftThumbProximal,
                HumanBodyBones.LeftThumbIntermediate,
                HumanBodyBones.LeftThumbDistal);

            UpdateFinger(41, false, false,
                HumanBodyBones.LeftIndexProximal,
                HumanBodyBones.LeftIndexIntermediate,
                HumanBodyBones.LeftIndexDistal);

            UpdateFinger(41, false, false,
                HumanBodyBones.LeftMiddleProximal,
                HumanBodyBones.LeftMiddleIntermediate,
                HumanBodyBones.LeftMiddleDistal);

            UpdateFinger(42, false, false,
                HumanBodyBones.LeftRingProximal,
                HumanBodyBones.LeftRingIntermediate,
                HumanBodyBones.LeftRingDistal);

            UpdateFinger(42, false, false,
                HumanBodyBones.LeftLittleProximal,
                HumanBodyBones.LeftLittleIntermediate,
                HumanBodyBones.LeftLittleDistal);

            UpdateFinger(45, true, true,
                HumanBodyBones.RightThumbProximal,
                HumanBodyBones.RightThumbIntermediate,
                HumanBodyBones.RightThumbDistal);

            UpdateFinger(46, false, true,
                HumanBodyBones.RightIndexProximal,
                HumanBodyBones.RightIndexIntermediate,
                HumanBodyBones.RightIndexDistal);

            UpdateFinger(46, false, true,
                HumanBodyBones.RightMiddleProximal,
                HumanBodyBones.RightMiddleIntermediate,
                HumanBodyBones.RightMiddleDistal);

            UpdateFinger(47, false, true,
                HumanBodyBones.RightRingProximal,
                HumanBodyBones.RightRingIntermediate,
                HumanBodyBones.RightRingDistal);

            UpdateFinger(47, false, true,
                HumanBodyBones.RightLittleProximal,
                HumanBodyBones.RightLittleIntermediate,
                HumanBodyBones.RightLittleDistal);
        }

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _animator = GetComponent<Animator>();

            _noise1 = new NoiseGenerator(_noise1Frequency);
            _noise2 = new NoiseGenerator(_noise2Frequency);
            _noise3 = new NoiseGenerator(_noise3Frequency);
        }

        void Update()
        {
            _noise1.Frequency = _noise1Frequency;
            _noise2.Frequency = _noise2Frequency;
            _noise3.Frequency = _noise3Frequency;

            _noise1.Step();
            _noise2.Step();
            _noise3.Step();

            _leftHandPosition = CalculateHandPosition(false);
            _rightHandPosition = CalculateHandPosition(true);
            _leftFootPosition = CalculateFootPosition(false);
            _rightFootPosition = CalculateFootPosition(true);
        }

        void OnAnimatorMove()
        {
            _animator.bodyPosition = CalculateBodyPosition();
            _animator.bodyRotation = CalculateBodyRotation();
        }

        void OnAnimatorIK(int layerIndex)
        {
            var spine = CalculateSpineRotation();
            _animator.SetBoneLocalRotation(HumanBodyBones.Spine, spine);
            _animator.SetBoneLocalRotation(HumanBodyBones.Chest, spine);

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandPosition);
            _animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, CalculateHandRotation(false));

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandPosition);
            _animator.SetBoneLocalRotation(HumanBodyBones.RightHand, CalculateHandRotation(true));

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootPosition);
            _animator.SetBoneLocalRotation(HumanBodyBones.LeftFoot, CalculateFootRotation(false));

            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootPosition);
            _animator.SetBoneLocalRotation(HumanBodyBones.RightFoot, CalculateFootRotation(true));

            UpdateAllFingers();
        }

        #endregion
    }
}
