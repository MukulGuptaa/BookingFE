using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class APIManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:5002/api"; // Replace with your actual API URL

    private static APIManager _instance;
    public static APIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Find existing instance in scene
                _instance = FindFirstObjectByType<APIManager>();
                
                // If not found, create new GameObject with APIManager
                if (_instance == null)
                {
                    GameObject go = new GameObject("APIManager");
                    _instance = go.AddComponent<APIManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PostRequest<T>(string endpoint, object data, Action<T> onSuccess, Action<string> onError)
    {
        StartCoroutine(PostRequestCoroutine(endpoint, data, onSuccess, onError));
    }

    private IEnumerator PostRequestCoroutine<T>(string endpoint, object data, Action<T> onSuccess, Action<string> onError)
    {
        string url = $"{BASE_URL}{endpoint}";
        string jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (UserData.Instance.IsLoggedIn)
            {
                request.SetRequestHeader("Authorization", $"Bearer {UserData.Instance.Token}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                try
                {
                    T result = JsonConvert.DeserializeObject<T>(jsonResponse);
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON Parse Error: {e.Message}");
                    onError?.Invoke($"JSON Parse Error: {e.Message}");
                }
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorMsg)) errorMsg = request.error;
                Debug.LogError($"Error: {request.error} - {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }

    public void GetRequest<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetRequestCoroutine(endpoint, onSuccess, onError));
    }

    private IEnumerator GetRequestCoroutine<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
    {
        string url = $"{BASE_URL}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            if (UserData.Instance.IsLoggedIn)
            {
                request.SetRequestHeader("Authorization", $"Bearer {UserData.Instance.Token}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                try
                {
                    T result = JsonConvert.DeserializeObject<T>(jsonResponse);
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON Parse Error: {e.Message}");
                    onError?.Invoke($"JSON Parse Error: {e.Message}");
                }
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorMsg)) errorMsg = request.error;
                Debug.LogError($"Error: {request.error} - {errorMsg}");
                onError?.Invoke(errorMsg);
            }
        }
    }
}
