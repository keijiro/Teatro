using UnityEngine;

[ExecuteInEditMode]
public class DiscRenderer : MonoBehaviour
{
    [SerializeField] int _arcSegments = 8;
    [SerializeField] int _ringSegments = 10;
    [SerializeField] int _pointCount = 10;

    [SerializeField] Material _material;

    Mesh[] _meshes;
    bool _needsRebuild = true;

    public void RequestRebuild()
    {
        _needsRebuild = true;
    }

    Mesh BuildMesh(float l0, float l1)
    {
        var va = new Vector3[_pointCount * 6 + 8];

        var r0 = 0.0f;
        var r1 = Mathf.PI * 2 / _arcSegments;

        for (var i = 0; i < _pointCount; i++)
        {
            var r = Mathf.Lerp(r0, r1, (float)i / (_pointCount - 1));
            var v = new Vector3(Mathf.Cos(r), 0, Mathf.Sin(r));
            va[i + _pointCount * 0] = v * l0 - Vector3.up;
            va[i + _pointCount * 1] = v * l0;
            va[i + _pointCount * 2] = v * l0;
            va[i + _pointCount * 3] = v * l1;
            va[i + _pointCount * 4] = v * l1;
            va[i + _pointCount * 5] = v * l1 - Vector3.up;
        }

        va[_pointCount * 6 + 0] = va[_pointCount * 0];
        va[_pointCount * 6 + 1] = va[_pointCount * 1];
        va[_pointCount * 6 + 2] = va[_pointCount * 4];
        va[_pointCount * 6 + 3] = va[_pointCount * 5];

        va[_pointCount * 6 + 4] = va[_pointCount * 0 + _pointCount - 1];
        va[_pointCount * 6 + 5] = va[_pointCount * 1 + _pointCount - 1];
        va[_pointCount * 6 + 6] = va[_pointCount * 4 + _pointCount - 1];
        va[_pointCount * 6 + 7] = va[_pointCount * 5 + _pointCount - 1];

        var ta = new Vector2[va.Length];
        for (var i = 0; i < ta.Length; i++)
        {
            ta[i] = new Vector2(va[i].x, va[i].z);
        }

        var ia = new int[(_pointCount - 1) * 6 * 3 + 12];
        var ii = 0;

        for (var i = 0; i < _pointCount - 1; i++)
        {
            ia[ii++] = _pointCount * 0 + i;
            ia[ii++] = _pointCount * 0 + i + 1;
            ia[ii++] = _pointCount * 1 + i;

            ia[ii++] = _pointCount * 1 + i;
            ia[ii++] = _pointCount * 0 + i + 1;
            ia[ii++] = _pointCount * 1 + i + 1;

            ia[ii++] = _pointCount * 2 + i;
            ia[ii++] = _pointCount * 2 + i + 1;
            ia[ii++] = _pointCount * 3 + i;

            ia[ii++] = _pointCount * 3 + i;
            ia[ii++] = _pointCount * 2 + i + 1;
            ia[ii++] = _pointCount * 3 + i + 1;

            ia[ii++] = _pointCount * 4 + i;
            ia[ii++] = _pointCount * 4 + i + 1;
            ia[ii++] = _pointCount * 5 + i;

            ia[ii++] = _pointCount * 5 + i;
            ia[ii++] = _pointCount * 4 + i + 1;
            ia[ii++] = _pointCount * 5 + i + 1;
        }

        ia[ii++] = _pointCount * 6 + 0;
        ia[ii++] = _pointCount * 6 + 1;
        ia[ii++] = _pointCount * 6 + 2;

        ia[ii++] = _pointCount * 6 + 2;
        ia[ii++] = _pointCount * 6 + 3;
        ia[ii++] = _pointCount * 6 + 0;

        ia[ii++] = _pointCount * 6 + 4;
        ia[ii++] = _pointCount * 6 + 6;
        ia[ii++] = _pointCount * 6 + 5;

        ia[ii++] = _pointCount * 6 + 6;
        ia[ii++] = _pointCount * 6 + 4;
        ia[ii++] = _pointCount * 6 + 7;

        var mesh = new Mesh();
        mesh.vertices = va;
        mesh.uv = ta;
        mesh.SetIndices(ia, MeshTopology.Triangles, 0);
        mesh.hideFlags = HideFlags.DontSave;
        mesh.RecalculateNormals();
        return mesh;
    }

    void SetUpResources()
    {
        if (_meshes != null) foreach (var m in _meshes) DestroyImmediate(m);

        _meshes = new Mesh[_ringSegments];
        for (var ri = 0; ri < _ringSegments; ri++)
        {
            _meshes[ri] = BuildMesh(0.05f + 0.95f * ri / _ringSegments, 0.05f + 0.95f * (ri + 1) / _ringSegments);
        }
    }

    void Update()
    {
        if (_needsRebuild)
        {
            SetUpResources();
            _needsRebuild = false;
        }

        var matrix = transform.localToWorldMatrix;
        var x = Time.time * 0.357f;
        for (var ri = 0; ri < _ringSegments; ri++)
        {
            for (var ai = 0; ai < _arcSegments; ai++)
            {
                var y = Perlin.Noise(x);
                Graphics.DrawMesh(_meshes[ri], Matrix4x4.TRS(Vector3.up * y * 0.2f, Quaternion.AngleAxis(360.0f / _arcSegments * ai, Vector3.up), Vector3.one) * matrix, _material, 0); 
                x += 0.091f;
            }
        }
    }
}
