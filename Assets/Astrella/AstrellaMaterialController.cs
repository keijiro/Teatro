using UnityEngine;
using System.Collections;

public class AstrellaMaterialController : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
    Color _emission = Color.white;

    [Space]
    [SerializeField]
    Vector2 _noiseFrequency = Vector2.one;

    [SerializeField]
    Vector2 _noiseMotion = Vector2.one;

    [Space]
    [SerializeField]
    float _cutoffOffset = 0.5f;

    [SerializeField]
    float _gradient = 0.5f;

    [Space]
    [SerializeField, Range(0, 1)]
    float _transition;

    public float transition {
        get { return _transition; }
        set { _transition = value; }
    }

    SkinnedMeshRenderer[] _smrs;

    void Start()
    {
        _smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (_transition > 0.0f)
        {
            var t = Time.time;

            var nparams = new Vector4(
                _noiseFrequency.x, _noiseFrequency.y,
                _noiseMotion.x * t, _noiseMotion.y * t
            );

            var rparams = new Vector2(_cutoffOffset, _gradient);

            foreach (var smr in _smrs)
            {
                foreach (var m in smr.materials)
                {
                    m.EnableKeyword("_TRANSITION");
                    m.SetVector("_NParams", nparams);
                    m.SetVector("_RParams", rparams);
                    m.SetColor("_Emission", _emission);
                    m.SetFloat("_Transition", _transition);
                }
            }
        }
        else
        {
            foreach (var smr in _smrs)
                foreach (var m in smr.materials)
                    m.DisableKeyword("_TRANSITION");
        }
    }
}
