using UnityEngine;

namespace Teatro
{
    [ExecuteInEditMode]
    public class WireRenderer : MonoBehaviour
    {
        #region Exposed Properties

        [SerializeField, ColorUsage(true, true, 0, 8, 0.125f, 3)]
        Color _color = Color.white;

        [SerializeField] int _verticesPerCircle = 128;
        [SerializeField] int _circleCount = 32;

        [HideInInspector, SerializeField] Shader _shader;

        #endregion

        #region Internal Objects and Variables

        Mesh _mesh;
        Material _material;

        #endregion

        #region MonoBehaviour Functions

        void Update()
        {
            if (_mesh == null) ResetResources();

            _material.SetColor("_Color", _color);

            var matrix = transform.localToWorldMatrix;
            Graphics.DrawMesh(_mesh, matrix, _material, 0); 
        }

        #endregion

        #region Resource Management

        void ResetResources()
        {
            if (_mesh) DestroyImmediate(_mesh);
            if (_material) DestroyImmediate(_material);

            _mesh = BuildMesh();

            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        Mesh BuildMesh()
        {
            var va = new Vector3[_verticesPerCircle * _circleCount];
            var ta = new Vector2[_verticesPerCircle * _circleCount];

            for (var vi = 0; vi < _verticesPerCircle; vi++)
            {
                var u = (float)vi / _verticesPerCircle;
                var r = Mathf.PI * 2 * u;

                va[vi] = new Vector3(Mathf.Cos(r), 0, Mathf.Sin(r));
                ta[vi] = new Vector2(u, 0);

                for (var ci = 1; ci < _circleCount ; ci++)
                {
                    var vi2 = vi + ci * _verticesPerCircle;
                    va[vi2] = va[vi];
                    ta[vi2]  = new Vector2(u, (float)ci / _circleCount);
                }
            }

            var ia = new int[_verticesPerCircle * _circleCount * 2];
            var ii = 0;

            for (var ci = 0; ci < _circleCount ; ci++)
            {
                var offs = ci * _verticesPerCircle;

                for (var vi = 0; vi < _verticesPerCircle - 1; vi++)
                {
                    ia[ii++] = offs + vi;
                    ia[ii++] = offs + vi + 1;
                }

                ia[ii++] = offs + _verticesPerCircle - 1;
                ia[ii++] = offs;
            }

            var mesh = new Mesh();
            mesh.vertices = va;
            mesh.uv = ta;
            mesh.SetIndices(ia, MeshTopology.Lines, 0);
            mesh.hideFlags = HideFlags.DontSave;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            return mesh;
        }

        #endregion
    }
}
