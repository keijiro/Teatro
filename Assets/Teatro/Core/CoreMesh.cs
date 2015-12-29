using UnityEngine;
using System.Collections.Generic;
using Emgen;

namespace Teatro
{
    public class CoreMesh : ScriptableObject
    {
        #region Public Properties

        [SerializeField, Range(0, 4)]
        int _subdivision = 2;

        public int subdivision {
            get { return _subdivision; }
        }

        [SerializeField, HideInInspector]
        Mesh _mesh;

        public Mesh sharedMesh {
            get { return _mesh; }
        }

        #endregion

        #region Public Methods

        public void RebuildMesh()
        {
            if (_mesh == null)
            {
                Debug.LogError("Mesh asset is missing.");
                return;
            }

            _mesh.Clear();

            // Build a icosphere and subdivide it.
            IcosphereBuilder ib = new IcosphereBuilder();
            for (var i = 0; i < _subdivision; i++) ib.Subdivide();

            // Make vertex arrays.
            var vc = ib.vertexCache;
            var vcount = 3 * vc.triangles.Count;
            var va1 = new List<Vector3>(vcount); // vertex position
            var va2 = new List<Vector3>(vcount); // previous vertex position
            var va3 = new List<Vector3>(vcount); // next vertex position

            foreach (var t in vc.triangles)
            {
                var v1 = vc.vertices[t.i1];
                var v2 = vc.vertices[t.i2];
                var v3 = vc.vertices[t.i3];

                va1.Add(v1);
                va2.Add(v2);
                va3.Add(v3);

                va1.Add(v2);
                va2.Add(v3);
                va3.Add(v1);

                va1.Add(v3);
                va2.Add(v1);
                va3.Add(v2);
            }

            // Rebuild the mesh asset.
            _mesh.SetVertices(va1);
            _mesh.SetUVs(0, va2);
            _mesh.SetUVs(1, va3);
            _mesh.SetIndices(
                vc.MakeIndexArrayForFlatMesh(), MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

            _mesh.Optimize();
            _mesh.UploadMeshData(true);
        }

        #endregion

        #region ScriptableObject Functions

        void OnEnable()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "Core";
            }
        }

        #endregion
    }
}
