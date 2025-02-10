using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace FSF.Collection.Utilities
{
    public static class JsonSaverUtility
    {
        /// <summary>
        /// Save values from path.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public static void SaveValue(object target, string path)
        {
            string directoryPath = Path.GetDirectoryName($"{Application.streamingAssetsPath}/{path}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = $"{Application.streamingAssetsPath}/{path}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string result = JsonConvert.SerializeObject(target);
            File.WriteAllText(filePath, result);
            Debug.Log($"File saved to: {filePath}");
        }

        /// <summary>
        /// Get values from path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T GetValue<T>(string path)
        {
            string filePath = $"{Application.streamingAssetsPath}/{path}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found.\ntarget path :{filePath}");
            }

            string result = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(result);
        }
    }

    public static class XmlSaverUtility
    {
        /// <summary>
        /// Save values from path.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public static void SaveValue<T>(T target, string path)
        {
            string directoryPath = Path.GetDirectoryName($"{Application.streamingAssetsPath}/{path}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = $"{Application.streamingAssetsPath}/{path}";
            using (var writer = new StreamWriter(filePath, false))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, target);
            }
            Debug.Log($"File saved to: {filePath}");
        }

        /// <summary>
        /// Get values from path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T GetValue<T>(string path)
        {
            string filePath = $"{Application.streamingAssetsPath}/{path}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found.\ntarget path :{filePath}");
            }

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    public static class BinarySaverUtility
    {
        public static void SaveValue<T>(T target, string path)
        {
            /// <summary>
            /// Save values from path.
            /// </summary>
            /// <param name="target"></param>
            /// <param name="path"></param>
            string directoryPath = Path.GetDirectoryName($"{Application.streamingAssetsPath}/{path}");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = $"{Application.streamingAssetsPath}/{path}";
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, target);
            }
            Debug.Log($"File saved to: {filePath}");
        }

        /// <summary>
        /// Get values from path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static T GetValue<T>(string path)
        {
            string filePath = $"{Application.streamingAssetsPath}/{path}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found.\ntarget path :{filePath}");
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }
    }
}
