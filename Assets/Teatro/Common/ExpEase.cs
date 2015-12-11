using UnityEngine;

namespace Teatro
{
    public static class ExpEase
    {
        public static float Out(float current, float target, float coeff) {
            return target - (target - current) * Mathf.Exp(-coeff * Time.deltaTime); 
        }

        public static float OutAngle(float current, float target, float coeff) {
            return target - 
                   Mathf.DeltaAngle(current, target) * Mathf.Exp(-coeff * Time.deltaTime); 
        }

        public static Vector3 Out(Vector3 current, Vector3 target, float coeff) {
            return Vector3.Lerp(target, current, Mathf.Exp(-coeff * Time.deltaTime));
        }

        public static Quaternion Out(Quaternion current, Quaternion target, float coeff) {
            if (current == target) {
                return target;
            } else {
                return Quaternion.Lerp(target, current, Mathf.Exp(-coeff * Time.deltaTime));
            }
        }
    }
}
