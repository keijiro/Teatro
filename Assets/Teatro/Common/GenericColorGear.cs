using UnityEngine;
using UnityEngine.Events;

namespace Reaktion
{
    public class GenericColorGear : MonoBehaviour
    {
        [System.Serializable]
        public class ColorEvent : UnityEvent<Color> {}

        public ReaktorLink reaktor;

        [ColorUsage(true, true, 0, 8, 0.125f, 3)]
        public Color color1 = Color.black;

        [ColorUsage(true, true, 0, 8, 0.125f, 3)]
        public Color color2 = Color.white;

        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        public ColorEvent target;

        void Awake()
        {
            reaktor.Initialize(this);
            UpdateTarget(0);
        }

        void Update()
        {
            UpdateTarget(reaktor.Output);
        }

        void UpdateTarget(float param)
        {
            target.Invoke(Color.Lerp(color1, color2, curve.Evaluate(param)));
        }
    }
}
