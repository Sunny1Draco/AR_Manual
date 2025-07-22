using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string translationKey;
    
    private TextMeshProUGUI textComponent;
    private LanguageManager languageManager;
    
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        languageManager = FindObjectOfType<LanguageManager>();
    }
    
    private void OnEnable()
    {
        if (languageManager != null)
        {
            languageManager.OnLanguageChanged += OnLanguageChanged;
        }
        
        UpdateText();
    }
    
    private void OnDisable()
    {
        if (languageManager != null)
        {
            languageManager.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void OnLanguageChanged(string language)
    {
        UpdateText();
    }
    
    public void UpdateText()
    {
        if (textComponent != null && languageManager != null && !string.IsNullOrEmpty(translationKey))
        {
            textComponent.text = languageManager.GetTranslation(translationKey);
        }
    }
}