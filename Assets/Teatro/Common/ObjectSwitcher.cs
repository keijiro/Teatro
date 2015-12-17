using UnityEngine;

namespace Teatro
{
    public class ObjectSwitcher : MonoBehaviour
    {
        [SerializeField]
        Reaktion.ReaktorLink _reaktor;

        [SerializeField]
        GameObject _target;

        [SerializeField]
        float _offDelay = 5;

        const float _threshold = 0.01f;

        float _delay;

        void Start()
        {
            _reaktor.Initialize(this);
        }

        void Update()
        {
            var o = _reaktor.Output;

            if (!_target.activeInHierarchy)
            {
                if (o > _threshold)
                {
                    _target.SetActive(true);
                    _delay = 0;
                }
            }
            else
            {
                if (o < _threshold)
                {
                    _delay += Time.deltaTime;
                    if (_delay >= _offDelay)
                        _target.SetActive(false);
                }
                else
                {
                    _delay = 0;
                }
            }
        }
    }
}
