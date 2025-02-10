using UnityEngine;

    public static class m_PlayerPrefs
    {
        public static void SetString(string valueName, string s)
        {
            PlayerPrefs.SetString(valueName, s);
        }

        public static string GetString(string valueName)
        {
            return PlayerPrefs.GetString(valueName);
        }

        public static void SetFloat(string valueName, float i)
        {
            PlayerPrefs.SetFloat(valueName, i);
        }

        public static float GetFloat(string valueName)
        {
            return PlayerPrefs.GetFloat(valueName);
        }

        public static void SetInt(string valueName, int i)
        {
            PlayerPrefs.SetInt(valueName, i);
        }

        public static int GetInt(string valueName)
        {
            return PlayerPrefs.GetInt(valueName);
        }

        public static void SetBool(string valueName, bool b)
        {
            SetInt(valueName, b ? 1 : 0);
        }

        public static bool GetBool(string valueName)
        {
            return GetInt(valueName) != 0;
        }

        public static void SetVector2(string valueName, Vector2 v3)
        {
            SetFloat(valueName + "_v2_x", v3.x);
            SetFloat(valueName + "_v2_y", v3.y);
        }

        public static Vector2 GetVector2(string valueName)
        {
            return new Vector2(
                GetFloat(valueName + "_v2_x"),
                GetFloat(valueName + "_v2_y")
            );
        }

        public static void SetVector3(string valueName, Vector3 v3)
        {
            SetFloat(valueName + "_v3_x", v3.x);
            SetFloat(valueName + "_v3_y", v3.y);
            SetFloat(valueName + "_v3_z", v3.z);
        }

        public static Vector3 GetVector3(string valueName)
        {
            return new Vector3(
                GetFloat(valueName + "_v3_x"),
                GetFloat(valueName + "_v3_y"),
                GetFloat(valueName + "_v3_z")
            );
        }

        public static void SetQuaternion(string valueName, Quaternion q4)
        {
            SetFloat(valueName + "_qua_x", q4.x);
            SetFloat(valueName + "_qua_y", q4.y);
            SetFloat(valueName + "_qua_z", q4.z);
            SetFloat(valueName + "_qua_w", q4.w);
        }

        public static Quaternion GetQuaternion(string valueName)
        {
            return new Quaternion(
                GetFloat(valueName + "_qua_x"),
                GetFloat(valueName + "_qua_y"),
                GetFloat(valueName + "_qua_z"),
                GetFloat(valueName + "_qua_w")
            );
        }

        public static void SetColor(string valueName, Color color){
            SetFloat($"{valueName}_color_r", color.r);
            SetFloat($"{valueName}_color_g", color.g);
            SetFloat($"{valueName}_color_b", color.b);
            SetFloat($"{valueName}_color_a", color.a);
        }

        public static Color GetColor(string valueName){
            return new Color(
                GetFloat($"{valueName}_color_r"),
                GetFloat($"{valueName}_color_g"),
                GetFloat($"{valueName}_color_b"),
                GetFloat($"{valueName}_color_a")
            );
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void DeleteKey(string n)
        {
            PlayerPrefs.DeleteKey(n);
        }
    }