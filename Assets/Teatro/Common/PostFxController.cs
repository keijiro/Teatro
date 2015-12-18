using UnityEngine;
using System.Collections;

namespace Teatro
{
    public class PostFxController : MonoBehaviour
    {
        [SerializeField] float _transitionTime = 1;

        [Space]
        [SerializeField] Kino.Bokeh _bokeh;
        [SerializeField] Kino.Isoline _isoline;
        [SerializeField] Kino.Contour _contour;
        [SerializeField] Kino.Isoline _isolineBW;
        [SerializeField] Kino.Contour _contourBW;

        float _maxBlur;
        Color _isolineColor;
        Color _contourColor;

        void Start()
        {
            _maxBlur = _bokeh.maxBlur;
            _isolineColor = _isoline.lineColor;
            _contourColor = _contour.lineColor;
        }

        public void ToggleIsoline()
        {
            StartCoroutine(SwitchIsoline(_isoline.enabled ^ true));
        }

        public void ToggleContour()
        {
            StartCoroutine(SwitchContour(_contour.enabled ^ true));
        }

        public void ToggleBlackAndWhite()
        {
            StartCoroutine(SwitchBlackAndWhite(_contourBW.enabled ^ true));
        }

        IEnumerator SwitchIsoline(bool state)
        {
            if (state) _isoline.enabled = true;

            var c = _isolineColor;

            for (var t = 0.0f; t < 1.0f;)
            {
                t = Mathf.Min(1.0f, t + Time.deltaTime / _transitionTime);

                c.a = state ? t : 1 - t;
                _isoline.lineColor = c;

                yield return null;
            }

            if (!state) _isoline.enabled = false;
        }

        IEnumerator SwitchContour(bool state)
        {
            if (state)
                _contour.enabled = true;
            else
                _bokeh.enabled = true;

            var c1 = Color.white;
            var c2 = _contourColor;

            for (var t = 0.0f; t < 1.0f;)
            {
                t = Mathf.Min(1.0f, t + Time.deltaTime / _transitionTime);

                var a = state ? t : 1 - t;
                c1.a = a;
                c2.a = a;

                _contour.backgroundColor = c1;
                _contour.lineColor = c2;
                _bokeh.maxBlur = _maxBlur * (1 - a);

                yield return null;
            }

            if (state)
                _bokeh.enabled = false;
            else
                _contour.enabled = false;
        }

        IEnumerator SwitchBlackAndWhite(bool state)
        {
            if (state)
            {
                _contourBW.enabled = true;
                _isolineBW.enabled = true;
            }
            else
            {
                _bokeh.enabled = true;
            }

            for (var t = 0.0f; t < 1.0f;)
            {
                t = Mathf.Min(1.0f, t + Time.deltaTime / _transitionTime);

                var a = state ? t : 1 - t;
                var black = new Color(0, 0, 0, a);
                var white = new Color(1, 1, 1, a);

                _contourBW.backgroundColor = black;
                _contourBW.lineColor = white;
                _isolineBW.lineColor = white;
                _bokeh.maxBlur = _maxBlur * (1 - a);

                yield return null;
            }

            if (!state)
            {
                _contourBW.enabled = false;
                _isolineBW.enabled = false;
            }
            else
            {
                _bokeh.enabled = false;
            }
        }
    }
}
