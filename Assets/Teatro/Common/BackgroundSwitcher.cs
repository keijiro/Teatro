using UnityEngine;
using System.Collections;

namespace Teatro
{
    public class BackgroundSwitcher : MonoBehaviour
    {
        #region Exposed Properties

        public enum Mode { Disc, Wall, Tunnel }

        [SerializeField] float _transitionTime = 1;
        [SerializeField] float _scrollSpeed = 2;

        public float scrollSpeed {
            get { return _scrollSpeed; }
            set { _scrollSpeed = value; }
        }

        [Space]
        [SerializeField] DiscRenderer _disc1;
        [SerializeField] DiscRenderer _disc2;
        [SerializeField] Kvant.Wall[] _walls;
        [SerializeField] Kvant.Tunnel _tunnel;
        [SerializeField] Kino.Fog _fog;

        [Space]
        [SerializeField] Kvant.Spray[] _sprays;
        [SerializeField] Kvant.WallScroller[] _wallScrollers;
        [SerializeField] Kvant.TunnelScroller _tunnelScroller;

        #endregion

        #region Private Variables And Properties

        Mode _mode;

        bool _disc1Switch = true;
        bool _disc2Switch = true;

        Vector3 _wallBaseScale;
        float _wallTransition;

        float _tunnelRadius;
        float _tunnelTransition;

        float DeltaTime {
            get { return Time.deltaTime / _transitionTime; }
        }

        #endregion

        #region Public Methods

        public void ToggleDisc(int index)
        {
            if (_mode == Mode.Disc)
            {
                if (index == 0)
                    _disc1Switch ^= true;
                else
                    _disc2Switch ^= true;
            }
            else
            {
                _mode = Mode.Disc;
                _disc1Switch = (index == 0);
                _disc2Switch = (index == 1);
            }
        }

        public void ToggleWallMode()
        {
            _mode = (_mode == Mode.Wall) ? Mode.Disc : Mode.Wall;
            _disc1Switch = _disc2Switch = false;
        }

        public void ToggleTunnelMode()
        {
            _mode = (_mode == Mode.Tunnel) ? Mode.Disc : Mode.Tunnel;
            _disc1Switch = _disc2Switch = false;
        }

        #endregion

        #region MonoBehaviour Functions

        void Start()
        {
            _wallBaseScale = _walls[0].baseScale;
            _tunnelRadius = _tunnel.radius;
        }

        void Update()
        {
            // disc (floor)
            var disc1Delta = (_disc1Switch ? 1 : -1) * DeltaTime;
            _disc1.transition = Mathf.Clamp01(_disc1.transition + disc1Delta);
            _disc1.enabled = (_disc1.transition > 0);

            // disc (ceiling)
            var disc2Delta = (_disc2Switch ? 1 : -1) * DeltaTime;
            _disc2.transition = Mathf.Clamp01(_disc2.transition + disc2Delta);
            _disc2.enabled = (_disc2.transition > 0);

            // walls
            var wallDelta = (_mode == Mode.Wall ? 1 : -1) * DeltaTime;
            _wallTransition = Mathf.Clamp01(_wallTransition + wallDelta);
            foreach (var wall in _walls)
            {
                wall.baseScale = _wallBaseScale * _wallTransition;
                wall.enabled = (_wallTransition > 0);
            }

            // tunnel
            var tunnelDelta = (_mode == Mode.Tunnel ? 1 : -1) * DeltaTime;
            _tunnelTransition = Mathf.Clamp01(_tunnelTransition + tunnelDelta);
            var easing = -Mathf.Cos(Mathf.PI *_tunnelTransition);
            _tunnel.radius = Mathf.Lerp(3, 1, easing) * _tunnelRadius;
            _tunnel.enabled = (_tunnelTransition > 0.5);

            // fog opacity
            var fog = Mathf.Max(_wallTransition, _tunnelTransition);
            _fog.bgOpacity = Mathf.Clamp01(fog * 2);

            // spray velocity
            var speed = Mathf.Max(_wallTransition, _tunnelTransition) * 3;
            speed = Mathf.Max(speed, _scrollSpeed);
            foreach (var spray in _sprays)
                spray.initialVelocity = Vector3.forward * speed;

            // scrollers
            foreach (var scroller in _wallScrollers)
                scroller.speed = speed;

            _tunnelScroller.speed = speed;
        }

        #endregion
    }
}
