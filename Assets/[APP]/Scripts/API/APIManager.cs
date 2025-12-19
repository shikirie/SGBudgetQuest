using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using VContainer;

public class APIManager : MonoBehaviour
{
    private string apiUrl;

    [Inject]
    public APIManager(GameplaySettings gameplaySettings)
    {
        apiUrl = gameplaySettings.ApiUrl;
    }

    public IEnumerator PostRequest(string jsonBody)
    {
        if (string.IsNullOrEmpty(apiUrl))
        {
            Debug.LogError("API URL is not set.");
            yield break;
        }

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending Data: " + jsonBody);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Succeed! Post Data AWS: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Failed: " + request.error);
        }
    }
}