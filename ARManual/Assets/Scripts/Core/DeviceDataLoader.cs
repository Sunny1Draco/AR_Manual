using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeviceData
{
    public string deviceId;
    public string deviceName;
    public string manufacturer;
    public string modelNumber;
    public List<InstructionStepData> steps;
    public Dictionary<string, TranslationData> translations;
    
    [Serializable]
    public class InstructionStepData
    {
        public string stepId;
        public string title;
        public string description;
        public string voiceOverFile;
        public float autoAdvanceDelay;
    }
    
    [Serializable]
    public class TranslationData
    {
        public string deviceName;
        public List<TranslatedStep> steps;
        
        [Serializable]
        public class TranslatedStep
        {
            public string title;
            public string description;
        }
    }
}

public class DeviceDataLoader : MonoBehaviour
{
    private Dictionary<string, DeviceData> deviceDataCache = new Dictionary<string, DeviceData>();
    
    public DeviceData LoadDeviceData(string deviceId)
    {
        // Check if data is already cached
        if (deviceDataCache.TryGetValue(deviceId, out DeviceData cachedData))
        {
            return cachedData;
        }
        
        // Try to load from Resources
        string path = $"DeviceData/{deviceId}";
        TextAsset jsonAsset = Resources.Load<TextAsset>(path);
        
        if (jsonAsset == null)
        {
            Debug.LogError($"Failed to load device data for {deviceId}");
            return null;
        }
        
        // Parse JSON
        DeviceData deviceData = JsonUtility.FromJson<DeviceData>(jsonAsset.text);
        
        // Cache for future use
        deviceDataCache[deviceId] = deviceData;
        
        return deviceData;
    }
    
    public string GetLocalizedTitle(DeviceData deviceData, string stepId, string language)
    {
        if (deviceData == null) return string.Empty;
        
        // Find the step
        DeviceData.InstructionStepData step = deviceData.steps.Find(s => s.stepId == stepId);
        if (step == null) return string.Empty;
        
        // If requesting English or translation not available, return original
        if (language == "English" || !deviceData.translations.ContainsKey(language.ToLower()))
        {
            return step.title;
        }
        
        // Find the step index
        int stepIndex = deviceData.steps.FindIndex(s => s.stepId == stepId);
        if (stepIndex < 0 || stepIndex >= deviceData.translations[language.ToLower()].steps.Count)
        {
            return step.title;
        }
        
        return deviceData.translations[language.ToLower()].steps[stepIndex].title;
    }
    
    public string GetLocalizedDescription(DeviceData deviceData, string stepId, string language)
    {
        if (deviceData == null) return string.Empty;
        
        // Find the step
        DeviceData.InstructionStepData step = deviceData.steps.Find(s => s.stepId == stepId);
        if (step == null) return string.Empty;
        
        // If requesting English or translation not available, return original
        if (language == "English" || !deviceData.translations.ContainsKey(language.ToLower()))
        {
            return step.description;
        }
        
        // Find the step index
        int stepIndex = deviceData.steps.FindIndex(s => s.stepId == stepId);
        if (stepIndex < 0 || stepIndex >= deviceData.translations[language.ToLower()].steps.Count)
        {
            return step.description;
        }
        
        return deviceData.translations[language.ToLower()].steps[stepIndex].description;
    }
}