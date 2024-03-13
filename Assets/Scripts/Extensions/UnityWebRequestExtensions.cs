using UnityEngine.Networking;

public static class UnityWebRequestExtensions
{
    public static bool IsSuccessful(this UnityWebRequest webRequest)
    {
        return webRequest.result != UnityWebRequest.Result.ConnectionError
            && webRequest.result != UnityWebRequest.Result.ProtocolError
            && webRequest.result != UnityWebRequest.Result.DataProcessingError;
    }
}