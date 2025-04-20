using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace FSF.VNG
{
    public static class ProfileManager
    {
        public static string DialoguesFolder = "DialogueProfiles";
        public static string VoicesFolder = "Voices";

        public static async UniTask<DialogueProfile> LoadProfileAsync(EpisodeSymbol episode)
        {    
            try
            {
                string path = Path.Combine(Application.streamingAssetsPath, DialoguesFolder, $"Episode_{episode[0]}_{episode[1]}.json");
                string json;

                #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                if (!File.Exists(path)) 
                    throw new FileNotFoundException($"File not found at {path}");
                json = await File.ReadAllTextAsync(path);
                #else
                    #if UNITY_STANDALONE_OSX || UNITY_IOS
                    path = "file://" + path;
                    #elif UNITY_ANDROID
                    #endif

                    using (var request = UnityWebRequest.Get(path))
                    {
                        await request.SendWebRequest();
                        if (request.result != UnityWebRequest.Result.Success)
                            throw new FileLoadException(
                                $"Failed to load {path}\n" +
                                $"Error: {request.error}\n" +
                                $"HTTP Code: {request.responseCode}\n" +
                                $"Response: {request.downloadHandler.text}");

                        json = request.downloadHandler.text;
                    }
                #endif

                return JsonConvert.DeserializeObject<DialogueProfile>(json);
            }   
            catch (Exception e)
            {
                Debug.LogError($"Profile load failed for Episode {episode[0]}-{episode[1]}\n" +
                    $"Error Type: {e.GetType().Name}\n" +
                    $"Message: {e.Message}\n" +
                    $"Stack Trace: {e.StackTrace}");
                return null;
            }
        }

        public static DialogueProfile LoadProfile(EpisodeSymbol episode)
        {
            try
            {
                string path = Path.Combine(Application.streamingAssetsPath, DialoguesFolder, $"Episode_{episode[0]}_{episode[1]}.json");
                string json;

                #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                if (!File.Exists(path))
                    throw new FileNotFoundException($"File not found at {path}");
                json = File.ReadAllText(path);
                #else
                    #if UNITY_STANDALONE_OSX || UNITY_IOS
                    path = "file://" + path;
                    #elif UNITY_ANDROID
                    // Android 不需要添加 file:// 前綴
                    #endif

                    using (var request = UnityWebRequest.Get(path))
                    {
                        // 同步等待請求完成
                        request.SendWebRequest();
                        while (!request.isDone) { } // 阻塞當前線程直到完成

                        if (request.result != UnityWebRequest.Result.Success)
                            throw new FileLoadException(
                                $"Failed to load {path}\n" +
                                $"Error: {request.error}\n" +
                                $"HTTP Code: {request.responseCode}\n" +
                                $"Response: {request.downloadHandler.text}");

                        json = request.downloadHandler.text;
                    }
                #endif

                return JsonConvert.DeserializeObject<DialogueProfile>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Profile load failed for Episode {episode[0]}-{episode[1]}\n" +
                    $"Error Type: {e.GetType().Name}\n" +
                    $"Message: {e.Message}\n" +
                    $"Stack Trace: {e.StackTrace}");
                return null;
            }
        }
    }
}