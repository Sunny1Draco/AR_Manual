using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AppSettings
{
    public string defaultLanguage = "English";
    public List<string> supportedLanguages = new List<string> { "English", "Spanish", "French", "German", "Chinese" };
    public bool checkForUpdatesOnStartup = true;
    public bool analyticsEnabled = true;
    public bool voiceCommandsEnabled = true;
    public float uiScale = 1.0f;
}

[Serializable]
public class ARSettings
{
    public string imageTrackingMode = "Quality";
    public int maxTrackedImages = 3;
    public float minConfidenceThreshold = 0.75f;
    public bool useCloudAnchors = false;
}

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SettingsManager");
                _instance = go.AddComponent<SettingsManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    public AppSettings appSettings = new AppSettings();
    public ARSettings arSettings = new ARSettings();
    
    private const string SETTINGS_KEY = "ARManualSettings";
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadSettings();
    }
    
    public void LoadSettings()
    {
        // Try to load from PlayerPrefs
        if (PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            JsonUtility.FromJsonOverwrite(json, this);
        }
        else
        {
            // Try to load from config file
            TextAsset configAsset = Resources.Load<TextAsset>("SceneConfig");
            if (configAsset != null)
            {
                try
                {
                    // Parse JSON manually since we have a complex structure
                    Dictionary<string, object> config = JsonUtility.FromJson<Dictionary<string, object>>(configAsset.text);
                    
                    if (config.TryGetValue("appSettings", out object appSettingsObj))
                    {
                        string appSettingsJson = JsonUtility.ToJson(appSettingsObj);
                        JsonUtility.FromJsonOverwrite(appSettingsJson, appSettings);
                    }
                    
                    if (config.TryGetValue("arSettings", out object arSettingsObj))
                    {
                        string arSettingsJson = JsonUtility.ToJson(arSettingsObj);
                        JsonUtility.FromJsonOverwrite(arSettingsJson, arSettings);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing config file: {e.Message}");
                }
            }
        }
    }
    
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();
    }
    
    public void SetLanguage(string language)
    {
        if (appSettings.supportedLanguages.Contains(language))
        {
            appSettings.defaultLanguage = language;
            SaveSettings();
            
            // Notify language manager
            LanguageManager languageManager = FindObjectOfType<LanguageManager>();
            if (languageManager != null)
            {
                languageManager.SetLanguage(language);
            }
        }
    }
    
    public void SetAnalyticsEnabled(bool enabled)
    {
        appSettings.analyticsEnabled = enabled;
        SaveSettings();
    }
    
    public void SetVoiceCommandsEnabled(bool enabled)
    {
        appSettings.voiceCommandsEnabled = enabled;
        SaveSettings();
        
        // Update voice recognition manager
        VoiceRecognitionManager voiceManager = FindObjectOfType<VoiceRecognitionManager>();
        if (voiceManager != null)
        {
            // Implementation would depend on how voice recognition is toggled
        }
    }
    
    public void SetUIScale(float scale)
    {
        appSettings.uiScale = Mathf.Clamp(scale, 0.5f, 2.0f);
        SaveSettings();
        
        // Update UI scale
        // This would depend on your UI scaling implementation
    }
}