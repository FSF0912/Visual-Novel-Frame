using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Buffers;

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
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string result = JsonConvert.SerializeObject(target, Formatting.Indented);
            File.WriteAllText(path, result);
            Debug.Log($"JSON saved to: {path}");
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
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"JSON file not found: {path}");
            }

            string result = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// [Async] Get values from path.
        /// </summary>
        public static async UniTask<T> GetValueAsync<T>(string path, CancellationToken token = default)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"JSON file not found: {path}");
            }

            string result = await File.ReadAllTextAsync(path, token);
            return await UniTask.RunOnThreadPool(() => JsonConvert.DeserializeObject<T>(result));
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
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var writer = new StreamWriter(path, false))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, target);
            }
            Debug.Log($"XML saved to: {path}");
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
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"XML file not found: {path}");
            }

            using (var reader = new StreamReader(path))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// [Async] Get values from path.
        /// </summary>
        public static async UniTask<T> GetValueAsync<T>(string path, CancellationToken token = default)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"XML file not found: {path}");
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            using var reader = new StreamReader(stream);
            string xmlContent = await reader.ReadToEndAsync();
            return await UniTask.RunOnThreadPool(() => 
            {
                var serializer = new XmlSerializer(typeof(T));
                using var textReader = new StringReader(xmlContent);
                return (T)serializer.Deserialize(textReader);
            });
        }
    }

    public static class BinarySaverUtility
    {
        /// <summary>
        /// Save values from path.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public static void SaveValue<T>(T target, string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, target);
            }
            Debug.Log($"Binary saved to: {path}");
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
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Binary file not found: {path}");
            }

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }

        /// <summary>
        /// [Async] Get values from path.
        /// </summary>
        public static async UniTask<T> GetValueAsync<T>(string path, CancellationToken token = default)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Binary file not found: {path}");
            }

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)fs.Length);
            try
            {
                int bytesRead = 0;
                while (bytesRead < fs.Length)
                {
                    int read = await fs.ReadAsync(buffer, bytesRead, (int)fs.Length - bytesRead, token);
                    if (read == 0) break;
                    bytesRead += read;
                }

                return await UniTask.RunOnThreadPool(() =>
                {
                    using var ms = new MemoryStream(buffer, 0, bytesRead);
                    var formatter = new BinaryFormatter();
                    return (T)formatter.Deserialize(ms);
                });
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}