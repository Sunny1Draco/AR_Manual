using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CloudDataManager : MonoBehaviour
{
    [SerializeField] private string apiBaseUrl = "https://cloud.google.com/storage/docs/json_api";
    [SerializeField] private float updateCheckInterval = 3600f; // Check for updates every hour

    private string userId;
    private string appVersion;

    public event Action<Dictionary<string, object>> OnContentUpdated;

    private void Start()
    {
        // Generate or load user ID
        userId = PlayerPrefs.GetString("UserId", Guid.NewGuid().ToString());
        PlayerPrefs.SetString("UserId", userId);

        // Get app version
        appVersion = Application.version;

        // Start periodic update check
        StartCoroutine(PeriodicUpdateCheck());

        // Send initial analytics
        SendAnalyticsEvent("app_start", new Dictionary<string, object> {
            { "app_version", appVersion },
            { "device_model", SystemInfo.deviceModel },
            { "os_version", SystemInfo.operatingSystem }
        });
    }

    private IEnumerator PeriodicUpdateCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateCheckInterval);
            CheckForContentUpdates();
        }
    }

    public void CheckForContentUpdates()
    {
        StartCoroutine(FetchContentUpdates());
    }

    private IEnumerator FetchContentUpdates()
    {
        string url = $"{apiBaseUrl}content/updates?version={appVersion}&userId={userId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Dictionary<string, object> updateData = JsonUtility.FromJson<Dictionary<string, object>>(jsonResponse);

                if (updateData != null && updateData.Count > 0)
                {
                    // Notify listeners about content update
                    OnContentUpdated?.Invoke(updateData);

                    // Download any new assets if needed
                    if (updateData.TryGetValue("assets_url", out object assetsUrl))
                    {
                        StartCoroutine(DownloadAssets(assetsUrl.ToString()));
                    }
                }
            }
            else
            {
                Debug.LogError($"Error checking for updates: {request.error}");
            }
        }
    }

    private IEnumerator DownloadAssets(string assetsUrl)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(assetsUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Process and save downloaded assets
                // This would depend on your asset format and storage approach
                Debug.Log("Downloaded new assets successfully");
            }
            else
            {
                Debug.LogError($"Error downloading assets: {request.error}");
            }
        }
    }

    public void SendAnalyticsEvent(string eventName, Dictionary<string, object> eventData)
    {
        // Add standard fields
        eventData["event_name"] = eventName;
        eventData["timestamp"] = DateTime.UtcNow.ToString("o");
        eventData["user_id"] = userId;
        eventData["app_version"] = appVersion;

        StartCoroutine(SendAnalyticsRequest(eventData));
    }

    private IEnumerator SendAnalyticsRequest(Dictionary<string, object> eventData)
    {
        string url = $"{apiBaseUrl}analytics/events";
        string jsonData = JsonUtility.ToJson(eventData);

        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending analytics: {request.error}");
            }
        }
    }
}