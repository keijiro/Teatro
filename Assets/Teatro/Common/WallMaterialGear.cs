using UnityEngine;
using System.Collections;
using Reaktion;

namespace Teatro
{
    public class WallMaterialGear : MonoBehaviour
    {
        public ReaktorLink reaktor;
        public Modifier emission;

        Color originalColor;
        Material material;

        void Start()
        {
            reaktor.Initialize(this);

            material = GetComponent<Kvant.Wall>().material;
            originalColor = material.GetColor("_Emission");

            UpdateEmission(0);
        }

        void Update()
        {
            UpdateEmission(reaktor.Output);
        }

        void UpdateEmission(float param)
        {
            if (emission.enabled)
                material.SetColor("_Emission", originalColor * param);
        }
    }
}
