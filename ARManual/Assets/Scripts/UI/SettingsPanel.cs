using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Toggle analyticsToggle;
    [SerializeField] private Toggle voiceCommandsToggle;
    [SerializeField] private Slider uiScaleSlider;
    
    private SettingsManager settingsManager;
    
    private void Awake()
    {
        settingsManager = SettingsManager.Instance;
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
            
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            
        if (analyticsToggle != null)
            analyticsToggle.onValueChanged.AddListener(OnAnalyticsToggled);
            
        if (voiceCommandsToggle != null)
            voiceCommandsToggle.onValueChanged.AddListener(OnVoiceCommandsToggled);
            
        if (uiScaleSlider != null)
            uiScaleSlider.onValueChanged.AddListener(OnUIScaleChanged);
    }
    
    private void OnEnable()
    {
        UpdateUI();
    }
    
    public void ShowSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            UpdateUI();
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    private void UpdateUI()
    {
        if (settingsManager == null)
            return;
            
        // Update language dropdown
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new List<string>(settingsManager.appSettings.supportedLanguages));
            languageDropdown.value = settingsManager.appSettings.supportedLanguages.IndexOf(settingsManager.appSettings.defaultLanguage);
        }
        
        // Update toggles
        if (analyticsToggle != null)
            analyticsToggle.isOn = settingsManager.appSettings.analyticsEnabled;
            
        if (voiceCommandsToggle != null)
            voiceCommandsToggle.isOn = settingsManager.appSettings.voiceCommandsEnabled;
            
        // Update slider
        if (uiScaleSlider != null)
            uiScaleSlider.value = settingsManager.appSettings.uiScale;
    }
    
    private void OnLanguageChanged(int index)
    {
        if (settingsManager != null && index >= 0 && index < settingsManager.appSettings.supportedLanguages.Count)
        {
            string language = settingsManager.appSettings.supportedLanguages[index];
            settingsManager.SetLanguage(language);
        }
    }
    
    private void OnAnalyticsToggled(bool isOn)
    {
        if (settingsManager != null)
        {
            settingsManager.SetAnalyticsEnabled(isOn);
        }
    }
    
    private void OnVoiceCommandsToggled(bool isOn)
    {
        if (settingsManager != null)
        {
            settingsManager.SetVoiceCommandsEnabled(isOn);
        }
    }
    
    private void OnUIScaleChanged(float value)
    {
        if (settingsManager != null)
        {
            settingsManager.SetUIScale(value);
        }
    }
}