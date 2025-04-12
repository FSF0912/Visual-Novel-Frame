using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;

namespace FSF.VNG
{
    public static class ProfileManager
    {
        public static EpisodeSymbol CurrentSymbol = new (1, 1);
        public static string DialoguesFolder = "DialogueProfiles";

        public static async UniTask<DialogueProfile> LoadProfileAsync(EpisodeSymbol episode)
        {    
            string path = Path.Combine(Application.streamingAssetsPath, DialoguesFolder, $"Episode_{episode[0]}_{episode[1]}.json");
            
            #if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
                if (!path.StartsWith("file://")) 
                    path = "file://" + path;
            #elif UNITY_ANDROID
            #endif

            try
            {
                string json;
                
                #if UNITY_EDITOR || UNITY_STANDALONE
                    json = await File.ReadAllTextAsync(path);
                #else
                    using (var request = UnityWebRequest.Get(path))
                    {
                        await request.SendWebRequest();
                        if (request.result != UnityWebRequest.Result.Success)
                        {
                            throw new FileLoadException($"Load failed: {request.error}\nPath: {path}");
                        }
                        json = request.downloadHandler.text;
                    }
                #endif

                return await UniTask.RunOnThreadPool(() => 
                    JsonConvert.DeserializeObject<DialogueProfile>(json)
                );
            }   
            catch (Exception e)
            {
                Debug.LogError($"Profile loaded failed.\n {e.Message}\nStackTrace: {e.StackTrace}");
                return null;
            }
        }
    }
}