using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARUIManager : MonoBehaviour
{
    [SerializeField] private ARInstructionManager instructionManager;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button voiceCommandButton;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject scanDevicePrompt;
    
    [Header("Language Settings")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private string[] supportedLanguages = { "English", "Spanish", "French", "German", "Chinese" };
    
    private string currentLanguage = "English";
    
    private void Awake()
    {
        if (instructionManager == null)
            instructionManager = FindObjectOfType<ARInstructionManager>();
            
        // Setup UI
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClicked);
            
        if (prevButton != null)
            prevButton.onClick.AddListener(OnPrevButtonClicked);
            
        if (voiceCommandButton != null)
            voiceCommandButton.onClick.AddListener(OnVoiceCommandButtonClicked);
            
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new System.Collections.Generic.List<string>(supportedLanguages));
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
        
        // Initially show scan prompt and hide instruction panel
        ShowScanPrompt(true);
    }
    
    private void OnEnable()
    {
        instructionManager.OnStepChanged += HandleStepChanged;
        instructionManager.OnProgressChanged += HandleProgressChanged;
    }
    
    private void OnDisable()
    {
        instructionManager.OnStepChanged -= HandleStepChanged;
        instructionManager.OnProgressChanged -= HandleProgressChanged;
    }
    
    private void HandleStepChanged(ARInstructionManager.InstructionStep step)
    {
        // Update UI with step information
        if (titleText != null)
            titleText.text = step.title;
            
        if (descriptionText != null)
            descriptionText.text = step.description;
            
        // Show instruction panel and hide scan prompt
        ShowScanPrompt(false);
    }
    
    private void HandleProgressChanged(string deviceId, int currentStep, int totalSteps)
    {
        // Update progress UI
        if (progressSlider != null)
        {
            progressSlider.minValue = 1;
            progressSlider.maxValue = totalSteps;
            progressSlider.value = currentStep;
        }
        
        if (progressText != null)
            progressText.text = $"Step {currentStep} of {totalSteps}";
            
        // Enable/disable navigation buttons
        if (prevButton != null)
            prevButton.interactable = currentStep > 1;
            
        if (nextButton != null)
            nextButton.interactable = currentStep < totalSteps;
    }
    
    private void OnNextButtonClicked()
    {
        instructionManager.NextStep();
    }
    
    private void OnPrevButtonClicked()
    {
        instructionManager.PreviousStep();
    }
    
    private void OnVoiceCommandButtonClicked()
    {
        // Toggle voice recognition
        VoiceRecognitionManager voiceManager = FindObjectOfType<VoiceRecognitionManager>();
        if (voiceManager != null)
        {
            voiceManager.ToggleListening();
        }
    }
    
    private void OnLanguageChanged(int index)
    {
        if (index >= 0 && index < supportedLanguages.Length)
        {
            currentLanguage = supportedLanguages[index];
            
            // Notify language manager to change language
            LanguageManager languageManager = FindObjectOfType<LanguageManager>();
            if (languageManager != null)
            {
                languageManager.SetLanguage(currentLanguage);
            }
        }
    }
    
    public void ShowScanPrompt(bool show)
    {
        if (scanDevicePrompt != null)
            scanDevicePrompt.SetActive(show);
            
        if (instructionPanel != null)
            instructionPanel.SetActive(!show);
    }
    
    public void ShowLoading(bool show)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(show);
    }
}