using UnityEngine;

namespace Teatro
{
    public class Configurator : MonoBehaviour
    {
        float _ambientLightIntensity = 0.0f;

        public float ambientLightIntensity {
            get { return _ambientLightIntensity; }
            set { _ambientLightIntensity = value; }
        }

        void Start()
        {
            if (!Application.isEditor) Cursor.visible = false;
        }

        void Update()
        {
            RenderSettings.ambientIntensity = _ambientLightIntensity;
            RenderSettings.reflectionIntensity = _ambientLightIntensity;
        }
    }
}
