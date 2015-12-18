using UnityEngine;
using System.Collections;

namespace Teatro
{
    public class PostFxController : MonoBehaviour
    {
        public enum Mode {
            None, WhiteContour, BlackContour
        }

        [SerializeField] float _transitionTime = 1;

        [Space]
        [SerializeField] Kino.Bokeh _bokeh;
        [SerializeField] Kino.Isoline _isoline;
        [SerializeField] Kino.Contour _contour;
        [SerializeField] Kino.Isoline _isolineBlack;
        [SerializeField] Kino.Contour _contourBlack;

        Mode _mode;
        float _maxBlur;
        Color _isolineColor;
        Color _contourColor;

        float DeltaTime {
            get { return Time.deltaTime / _transitionTime; }
        }

        void Start()
        {
            _maxBlur = _bokeh.maxBlur;
            _isolineColor = _isoline.lineColor;
            _contourColor = _contour.lineColor;
            StartModeCoroutine();
        }

        public void ToggleIsoline()
        {
            StartCoroutine(SwitchIsoline(_isoline.enabled ^ true));
        }

        public void ToggleWhiteContour()
        {
            if (_mode == Mode.WhiteContour)
                _mode = Mode.None;
            else
                _mode = Mode.WhiteContour;
        }

        public void ToggleBlackContour()
        {
            if (_mode == Mode.BlackContour)
                _mode = Mode.None;
            else
                _mode = Mode.BlackContour;
        }

        void StartModeCoroutine()
        {
            if (_mode == Mode.WhiteContour)
                StartCoroutine(WhiteContourCoroutine());
            else if (_mode == Mode.BlackContour)
                StartCoroutine(BlackContourCoroutine());
            else
                StartCoroutine(ReadyCoroutine());
        }

        IEnumerator SwitchIsoline(bool state)
        {
            if (state) _isoline.enabled = true;

            var c = _isolineColor;

            for (var t = 0.0f; t < 1.0f;)
            {
                t = Mathf.Min(1.0f, t + DeltaTime);

                c.a = state ? t : 1 - t;
                _isoline.lineColor = c;

                yield return null;
            }

            if (!state) _isoline.enabled = false;
        }

        IEnumerator ReadyCoroutine()
        {
            while (_mode == Mode.None) yield return null;
            StartModeCoroutine();
        }

        IEnumerator WhiteContourCoroutine()
        {
            _contour.enabled = true;

            var c1 = Color.white;
            var c2 = _contourColor;

            for (var t = 0.0f; t < 1.0f;)
            {
                t = Mathf.Min(1.0f, t + DeltaTime);

                c1.a = t;
                c2.a = t;

                _contour.backgroundColor = c1;
                _contour.lineColor = c2;
                _bokeh.maxBlur = _maxBlur * (1 - t);

                yield return null;
            }

            _bokeh.enabled = false;

            while (_mode == Mode.WhiteContour) yield return null;

            _bokeh.enabled = true;

            for (var t = 1.0f; t > 0.0f;)
            {
                t = Mathf.Max(0.0f, t - DeltaTime);

                c1.a = t;
                c2.a = t;

                _contour.backgroundColor = c1;
                _contour.lineColor = c2;
                _bokeh.maxBlur = _maxBlur * (1 - t);

                yield return null;
            }

            _contour.enabled = false;

            StartModeCoroutine();
        }

        IEnumerator BlackContourCoroutine()
        {
            _contourBlack.enabled = true;
            _isolineBlack.enabled = true;

            for (var t = 0.0f; t < 1.0f;)
            {
                t = Mathf.Min(1.0f, t + DeltaTime);

                var black = new Color(0, 0, 0, t);
                var white = new Color(1, 1, 1, t);

                _contourBlack.backgroundColor = black;
                _contourBlack.lineColor = white;
                _isolineBlack.lineColor = white;
                _bokeh.maxBlur = _maxBlur * (1 - t);

                yield return null;
            }

            _bokeh.enabled = false;

            while (_mode == Mode.BlackContour) yield return null;

            _bokeh.enabled = true;

            for (var t = 1.0f; t > 0.0f;)
            {
                t = Mathf.Max(0.0f, t - DeltaTime);

                var black = new Color(0, 0, 0, t);
                var white = new Color(1, 1, 1, t);

                _contourBlack.backgroundColor = black;
                _contourBlack.lineColor = white;
                _isolineBlack.lineColor = white;
                _bokeh.maxBlur = _maxBlur * (1 - t);

                yield return null;
            }

            _contourBlack.enabled = false;
            _isolineBlack.enabled = false;

            StartModeCoroutine();
        }
    }
}
