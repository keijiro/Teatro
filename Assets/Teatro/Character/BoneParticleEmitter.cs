using UnityEngine;
using System.Collections.Generic;

namespace Teatro
{
    public class BoneParticleEmitter : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particleSystem;
        [SerializeField] float _emissionRate = 1.0f;
        [SerializeField] float _emissionRadius = 0.1f;

        public float emissionRate {
            get { return _emissionRate; }
            set { _emissionRate = value; }
        }

        public float emissionRadius {
            get { return _emissionRadius; }
            set { _emissionRadius = value; }
        }

        Animator _animator;

        class Segment
        {
            Transform _joint1;
            Transform _joint2;
            float _length;
            float _energy;

            public Segment(Animator animator, HumanBodyBones bone1, HumanBodyBones bone2)
            {
                _joint1 = animator.GetBoneTransform(bone1);
                _joint2 = animator.GetBoneTransform(bone2);
                _length = (_joint1.position - _joint2.position).magnitude;
            }

            public int Step(float density)
            {
                _energy += density * _length;
                var emit = Mathf.FloorToInt(_energy);
                _energy -= emit;
                return emit;
            }

            public Vector3 ChooseRandomPosition()
            {
                return Vector3.Lerp(_joint1.position, _joint2.position, Random.value);
            }
        }

        List<Segment> _segments;

        void Start()
        {
            var animator = GetComponent<Animator>();
            _segments = new List<Segment>();

            _segments.Add(new Segment(animator, HumanBodyBones.Hips, HumanBodyBones.Chest));
            _segments.Add(new Segment(animator, HumanBodyBones.Chest, HumanBodyBones.Neck));
            _segments.Add(new Segment(animator, HumanBodyBones.Neck, HumanBodyBones.Head));

            _segments.Add(new Segment(animator, HumanBodyBones.Chest, HumanBodyBones.LeftShoulder));
            _segments.Add(new Segment(animator, HumanBodyBones.LeftShoulder, HumanBodyBones.LeftLowerArm));
            _segments.Add(new Segment(animator, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand));

            _segments.Add(new Segment(animator, HumanBodyBones.Chest, HumanBodyBones.RightShoulder));
            _segments.Add(new Segment(animator, HumanBodyBones.RightShoulder, HumanBodyBones.RightLowerArm));
            _segments.Add(new Segment(animator, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand));

            _segments.Add(new Segment(animator, HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg));
            _segments.Add(new Segment(animator, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg));
            _segments.Add(new Segment(animator, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot));

            _segments.Add(new Segment(animator, HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg));
            _segments.Add(new Segment(animator, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg));
            _segments.Add(new Segment(animator, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot));
        }

        void Update()
        {
            var energy = _emissionRate * Time.deltaTime;
            foreach (var segment in _segments)
            {
                var emit = segment.Step(energy);
                for (var i = 0; i < emit; i++)
                {
                    var p = segment.ChooseRandomPosition() + Random.insideUnitSphere * _emissionRadius;
                    _particleSystem.transform.position = p;
                    _particleSystem.Emit(1);
                }
            }
        }
    }
}
