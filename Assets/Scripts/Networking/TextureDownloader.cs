using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UniDex
{
    public static class TextureDownloader
    {
        public static async Task<Texture> DownloadFromURL(string url)
        {
            using (var webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                if (!webRequest.IsSuccessful())
                {
                    Debug.LogError($"Failed to download texture from {url}. Error: {webRequest.error}");
                }

                return DownloadHandlerTexture.GetContent(webRequest);
            }
        }
    }
}
