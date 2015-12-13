//
// Deforming shell like thing
//
using UnityEngine;
using UnityEngine.Rendering;
using Emgen;

[ExecuteInEditMode]
public class ShellRenderer : MonoBehaviour
{
    #region Public Properties

    // Subdivision level
    [SerializeField, Range(0, 4)]
    int _subdivision = 2;

    public int subdivision {
        get { return _subdivision; }
        set { _subdivision = Mathf.Clamp(value, 0, 4); }
    }

    // Wave speed
    [SerializeField, Header("Wave Parameters")]
    float _waveSpeed = 8;

    public float waveSpeed {
        get { return _waveSpeed; }
        set { _waveSpeed = value; }
    }

    // Wave parameter alpha
    [SerializeField]
    float _waveAlpha = 1;

    public float waveAlpha {
        get { return _waveAlpha; }
        set { _waveAlpha = value; }
    }

    // Wave parameter beta
    [SerializeField]
    float _waveBeta = 1;

    public float waveBeta {
        get { return _waveBeta; }
        set { _waveBeta = value; }
    }

    // Cutoff level
    [SerializeField, Range(0, 1)]
    float _cutoff = 0.5f;

    public float cutoff {
        get { return _cutoff; }
        set { _cutoff = value; }
    }

    // Noise amplitude
    [SerializeField, Header("Noise Parameters")]
    float _noiseAmplitude = 5;

    public float noiseAmplitude {
        get { return _noiseAmplitude; }
        set { _noiseAmplitude = value; }
    }

    // Exponent of noise amplitude
    [SerializeField]
    float _noiseExponent = 4.5f;

    public float noiseExponent {
        get { return _noiseExponent; }
        set { _noiseExponent = value; }
    }

    // Noise frequency
    [SerializeField]
    float _noiseFrequency = 3;

    public float noiseFrequency {
        get { return _noiseFrequency; }
        set { _noiseFrequency = value; }
    }

    // Noise speed
    [SerializeField]
    float _noiseSpeed = 3;

    public float noiseSpeed {
        get { return _noiseSpeed; }
        set { _noiseSpeed = value; }
    }

    // Rendering settings
    [SerializeField, Header("Rendering")]
    Material _material;
    bool _owningMaterial; // whether owning the material

    public Material sharedMaterial {
        get { return _material; }
        set { _material = value; }
    }

    public Material material {
        get {
            if (!_owningMaterial) {
                _material = Instantiate<Material>(_material);
                _owningMaterial = true;
            }
            return _material;
        }
        set {
            if (_owningMaterial) Destroy(_material, 0.1f);
            _material = value;
            _owningMaterial = false;
        }
    }

    [SerializeField]
    bool _receiveShadows;

    [SerializeField]
    ShadowCastingMode _shadowCastingMode;

    #endregion

    #region Private Members

    Mesh _mesh;
    int _subdivided = -1;
    float _waveTime;
    Vector3 _noiseOffset;

    #endregion

    #region MonoBehaviour Functions

    void Update()
    {
        if (_subdivided != _subdivision) RebuildMesh();

        var noiseDir = new Vector3(1, 0.5f, 0.2f).normalized;

        _waveTime += Time.deltaTime * _waveSpeed;
        _noiseOffset += noiseDir * (Time.deltaTime * _noiseSpeed);

        var props = new MaterialPropertyBlock();

        props.SetFloat("_Cutoff", _cutoff);

        Vector3 wparam1 = new Vector3(3.1f, 2.3f, 6.3f);
        Vector3 wparam2 = new Vector3(0.031f, 0.022f, 0.039f);
        Vector3 wparam3 = new Vector3(1.21f, 0.93f, 1.73f);

        props.SetFloat("_WTime", _waveTime);
        props.SetVector("_WParams1", wparam1 * _waveAlpha);
        props.SetVector("_WParams2", wparam2);
        props.SetVector("_WParams3", wparam3 * _waveBeta);

        props.SetVector("_NOffset", _noiseOffset);

        var np = new Vector3(_noiseFrequency, _noiseAmplitude, _noiseExponent);
        props.SetVector("_NParams", np);

        Graphics.DrawMesh(
            _mesh, transform.localToWorldMatrix,
            _material, 0, null, 0, props,
            _shadowCastingMode, _receiveShadows);
    }

    #endregion

    #region Mesh Builder

    void RebuildMesh()
    {
        if (_mesh) DestroyImmediate(_mesh);

        // The Shell vertex shader needs positions of three vertices in a triangle
        // to calculate the normal vector. To provide these information, it uses
        // not only the position attribute but also the normal and tangent attributes
        // to store the 2nd and 3rd vertex position.

        IcosphereBuilder ib = new IcosphereBuilder();

        for (var i = 0; i < _subdivision; i++) ib.Subdivide();

        var vc = ib.vertexCache;

        var vcount = 3 * vc.triangles.Count;
        var va1 = new Vector3[vcount];
        var va2 = new Vector3[vcount];
        var va3 = new Vector4[vcount];

        var vi = 0;
        foreach (var t in vc.triangles)
        {
            var v1 = vc.vertices[t.i1];
            var v2 = vc.vertices[t.i2];
            var v3 = vc.vertices[t.i3];

            va1[vi + 0] = v1;
            va2[vi + 0] = v2;
            va3[vi + 0] = v3;

            va1[vi + 1] = v2;
            va2[vi + 1] = v3;
            va3[vi + 1] = v1;

            va1[vi + 2] = v3;
            va2[vi + 2] = v1;
            va3[vi + 2] = v2;

            vi += 3;
        }

        _mesh = new Mesh();
        _mesh.hideFlags = HideFlags.DontSave;
        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10);

        _mesh.vertices = va1;
        _mesh.normals  = va2;
        _mesh.tangents = va3;

        _mesh.SetIndices(vc.MakeIndexArrayForFlatMesh(), MeshTopology.Triangles, 0);

        _subdivided = _subdivision;
    }

    #endregion
}
