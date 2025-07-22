using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    [Serializable]
    public class TranslationEntry
    {
        public string key;
        public string englishText;
        public string spanishText;
        public string frenchText;
        public string germanText;
        public string chineseText;
    }
    
    [SerializeField] private List<TranslationEntry> translations = new List<TranslationEntry>();
    
    private Dictionary<string, Dictionary<string, string>> translationDict = new Dictionary<string, Dictionary<string, string>>();
    private string currentLanguage = "English";
    
    public event Action<string> OnLanguageChanged;
    
    private void Awake()
    {
        // Initialize translation dictionary
        InitializeTranslations();
    }
    
    private void InitializeTranslations()
    {
        // Create language dictionaries
        translationDict["English"] = new Dictionary<string, string>();
        translationDict["Spanish"] = new Dictionary<string, string>();
        translationDict["French"] = new Dictionary<string, string>();
        translationDict["German"] = new Dictionary<string, string>();
        translationDict["Chinese"] = new Dictionary<string, string>();
        
        // Populate dictionaries
        foreach (var entry in translations)
        {
            translationDict["English"][entry.key] = entry.englishText;
            translationDict["Spanish"][entry.key] = entry.spanishText;
            translationDict["French"][entry.key] = entry.frenchText;
            translationDict["German"][entry.key] = entry.germanText;
            translationDict["Chinese"][entry.key] = entry.chineseText;
        }
    }
    
    public void SetLanguage(string language)
    {
        if (translationDict.ContainsKey(language))
        {
            currentLanguage = language;
            OnLanguageChanged?.Invoke(language);
            
            // Update all UI text elements
            UpdateAllTextElements();
        }
    }
    
    public string GetTranslation(string key)
    {
        if (translationDict.TryGetValue(currentLanguage, out Dictionary<string, string> langDict))
        {
            if (langDict.TryGetValue(key, out string translation))
            {
                return translation;
            }
        }
        
        // Fallback to English or key itself
        if (currentLanguage != "English" && 
            translationDict.TryGetValue("English", out Dictionary<string, string> englishDict) &&
            englishDict.TryGetValue(key, out string englishTranslation))
        {
            return englishTranslation;
        }
        
        return key;
    }
    
    private void UpdateAllTextElements()
    {
        // Find all LocalizedText components and update them
        LocalizedText[] textElements = FindObjectsOfType<LocalizedText>();
        foreach (var element in textElements)
        {
            element.UpdateText();
        }
    }
}