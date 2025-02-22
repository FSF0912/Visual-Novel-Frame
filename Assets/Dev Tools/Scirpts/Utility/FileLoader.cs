using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public static class FileLoader
{
    public static async Task<AudioClip> LoadAudioWAV(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerAudioClip.GetContent(uwr);
            }
            return null;
        }
    }

    public static async Task<AudioClip> LoadAudioOGG(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS))
        {
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerAudioClip.GetContent(uwr);
            }
            return null;
        }
    }

    public static async Task<AudioClip> LoadAudioMPEG(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerAudioClip.GetContent(uwr);
            }
            return null;
        }
    }

    public static async Task<Texture2D> LoadImage(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerTexture.GetContent(uwr);
            }
            return null;
        }
    }
}