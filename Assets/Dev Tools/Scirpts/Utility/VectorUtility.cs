using UnityEngine;

namespace FSF.Collection.Utilities{
    public enum AxisModeV3{X,Y,Z}
    public static class VectorUtility
    {
        public static Vector3 SetSingleAxis(ref Vector3 v3, AxisModeV3 mode, float value){
            Vector3 vector3 = mode switch{
                AxisModeV3.X => new Vector3(value, v3.y, v3.z),
                AxisModeV3.Y => new Vector3(v3.x, value, v3.z),
                AxisModeV3.Z => new Vector3(v3.x, v3.y, value),
                _ => v3
            };
            v3 = vector3;
            return vector3;
        }

        public static bool OutOfRange(RectTransform scaler, RectTransform SelfRT){
            float left = scaler.position.x - SelfRT.rect.width / 2,
                right = scaler.position.x + SelfRT.rect.width / 2,
                top = scaler.position.y + SelfRT.rect.height / 2,
                bottom = scaler.position.y + SelfRT.rect.height / 2;
            return left < 0 || right > scaler.rect.width || top > scaler.rect.height || bottom < 0;
        }

        public static Vector3 MidPoint(Vector3 first, Vector3 second){
            return(first + second) / 2;
        }

        public static Vector2 MidPoint(Vector2 first, Vector2 second){
            return(first + second) / 2;
        }
    }
}
