using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WireRenderer : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
    Color _color = Color.white;

    [SerializeField]
    int _vertexCount = 128;

    [HideInInspector]
    [SerializeField]
    Shader _shader;

    Mesh _mesh;
    Material _material;
    float _offset;

    Mesh BuildMesh()
    {
        var va = new Vector3[_vertexCount];
        var ta = new Vector2[_vertexCount];

        for (var i = 0; i < _vertexCount; i++)
        {
            var r = Mathf.PI * 2 * i / (_vertexCount - 1);
            va[i] = new Vector3(Mathf.Cos(r), 0, Mathf.Sin(r));
            ta[i] = new Vector2((float)i / (_vertexCount - 1), 0);
        }

        var ia = new int[_vertexCount + 1];

        for (var i = 0; i < _vertexCount; i++) ia[i] = i;
        ia[_vertexCount] = 0;

        var mesh = new Mesh();
        mesh.vertices = va;
        mesh.uv = ta;
        mesh.SetIndices(ia, MeshTopology.LineStrip, 0);
        mesh.hideFlags = HideFlags.DontSave;
        return mesh;
    }

    void SetUpResources()
    {
        if (_mesh) DestroyImmediate(_mesh);
        if (_material) DestroyImmediate(_material);

        _mesh = BuildMesh();

        _material = new Material(_shader);
        _material.hideFlags = HideFlags.DontSave;

        _offset = Random.Range(-100.0f, 100.0f);
    }

    void Update()
    {
        if (_mesh == null) SetUpResources();

        _material.SetColor("_Color", _color);
        _material.SetFloat("_Offset", _offset);

        var matrix = transform.localToWorldMatrix;
        Graphics.DrawMesh(_mesh, matrix, _material, 0); 
    }
}
