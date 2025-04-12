using a = System.Diagnostics;
using System.IO;
using System.Linq;
using FSF.Collection.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FSF.Collection;

public class test : MonoBehaviour
{
    public string action;
    public string path = Path.Combine(Application.streamingAssetsPath, "test.a");
    public string[] strings = Enumerable.Range(0, 300000).Select(i => i.ToString()).Select(i => i = "1").ToArray();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)){
            PopupMenu.Instance.Panel_Top("aa");
        }
    }

}

[CustomEditor(typeof(test))]
[CanEditMultipleObjects]
public class ed : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        test t = (test)target;
        using (var hor = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (GUILayout.Button("Save-Json"))
            {
                a.Stopwatch stopwatch = new a.Stopwatch();
                stopwatch.Start();
                JsonSaverUtility.SaveValue(t.action, t.path);
                Debug.Log($"JSON saved to: {t.path}");
                Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();

            }

            if (GUILayout.Button("Get-Json"))
            {
                a.Stopwatch stopwatch = new a.Stopwatch();
                stopwatch.Start();
                var result = JsonSaverUtility.GetValue<string>(t.path);
                Debug.Log($"JSON loaded from: {t.path}");
                Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                Debug.Log(result);
                stopwatch.Stop();
            }
        }

        using (var hor = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (GUILayout.Button("Save-XML"))
            {
                a.Stopwatch stopwatch = new a.Stopwatch();
                stopwatch.Start();
                XmlSaverUtility.SaveValue(t.action, t.path);
                Debug.Log($"XML saved to: {t.path}");
                Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();
            }

            if (GUILayout.Button("Get-XML"))
            {
                a.Stopwatch stopwatch = new a.Stopwatch();
                stopwatch.Start();
                var result = XmlSaverUtility.GetValue<string>(t.path);
                Debug.Log($"XML loaded from: {t.path}");
                Debug.Log(result);
                Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();
            }
        }

        using (var hor = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (GUILayout.Button("Save-Binary"))
            {
                a.Stopwatch stopwatch = new a.Stopwatch();
                stopwatch.Start();
                BinarySaverUtility.SaveValue(t.action, t.path);
                Debug.Log($"Binary saved to: {t.path}");
                Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();
            }

            if (GUILayout.Button("Get-Binary"))
            {
                a.Stopwatch stopwatch = new a.Stopwatch();
                stopwatch.Start();
                var result = BinarySaverUtility.GetValue<string>(t.path);
                Debug.Log($"Binary loaded from: {t.path}");
                Debug.Log(result);
                Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();
            }
        }
    }
        
}
