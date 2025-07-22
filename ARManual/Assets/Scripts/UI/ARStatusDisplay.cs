using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARStatusDisplay : MonoBehaviour
{
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusIcon;
    [SerializeField] private Button retryButton;
    
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color initializingColor = Color.yellow;
    [SerializeField] private Color errorColor = Color.red;
    
    [SerializeField] private ARSessionManager arSessionManager;
    
    private void Awake()
    {
        if (arSessionManager == null)
            arSessionManager = FindObjectOfType<ARSessionManager>();
            
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryButtonClicked);
    }
    
    private void OnEnable()
    {
        if (arSessionManager != null)
            arSessionManager.OnARSessionStateChanged += HandleARSessionStateChanged;
    }
    
    private void OnDisable()
    {
        if (arSessionManager != null)
            arSessionManager.OnARSessionStateChanged -= HandleARSessionStateChanged;
    }
    
    private void HandleARSessionStateChanged(ARSessionManager.ARSessionState newState)
    {
        UpdateStatusDisplay(newState);
    }
    
    private void UpdateStatusDisplay(ARSessionManager.ARSessionState state)
    {
        if (statusPanel == null || statusText == null || statusIcon == null)
            return;
            
        switch (state)
        {
            case ARSessionManager.ARSessionState.Initializing:
                statusPanel.SetActive(true);
                statusText.text = "Initializing AR...";
                statusIcon.color = initializingColor;
                if (retryButton != null)
                    retryButton.gameObject.SetActive(false);
                break;
                
            case ARSessionManager.ARSessionState.Ready:
                statusPanel.SetActive(false);
                break;
                
            case ARSessionManager.ARSessionState.Failed:
                statusPanel.SetActive(true);
                statusText.text = "AR initialization failed. Please try again.";
                statusIcon.color = errorColor;
                if (retryButton != null)
                    retryButton.gameObject.SetActive(true);
                break;
                
            case ARSessionManager.ARSessionState.Unsupported:
                statusPanel.SetActive(true);
                statusText.text = "AR is not supported on this device.";
                statusIcon.color = errorColor;
                if (retryButton != null)
                    retryButton.gameObject.SetActive(false);
                break;
        }
    }
    
    private void OnRetryButtonClicked()
    {
        if (arSessionManager != null)
            arSessionManager.RestartSession();
    }
}