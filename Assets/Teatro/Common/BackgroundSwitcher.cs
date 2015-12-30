using UnityEngine;
using System.Collections;

namespace Teatro
{
    public class BackgroundSwitcher : MonoBehaviour
    {
        [SerializeField] float _transitionTime = 1;

        [Space]
        [SerializeField] DiscRenderer _disc1;
        [SerializeField] DiscRenderer _disc2;

        bool _disc1Switch = true;
        bool _disc2Switch = true;

        public void ToggleDisc(int index)
        {
            if (index == 0)
                _disc1Switch = !_disc1Switch;
            else
                _disc2Switch = !_disc2Switch;
        }

        float DeltaTime {
            get { return Time.deltaTime / _transitionTime; }
        }

        void Update()
        {
            var disc1Delta = (_disc1Switch ? 1 : -1) * DeltaTime;
            _disc1.transition = Mathf.Clamp01(_disc1.transition + disc1Delta);

            var disc2Delta = (_disc2Switch ? 1 : -1) * DeltaTime;
            _disc2.transition = Mathf.Clamp01(_disc2.transition + disc2Delta);
        }
    }
}
