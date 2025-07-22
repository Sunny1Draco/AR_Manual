using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManualAppController : MonoBehaviour
{
    [SerializeField] private ARSession arSession;
    [SerializeField] private DeviceRecognitionManager deviceRecognitionManager;
    [SerializeField] private ARInstructionManager instructionManager;
    [SerializeField] private ARUIManager uiManager;
    [SerializeField] private VoiceRecognitionManager voiceManager;
    [SerializeField] private CloudDataManager cloudManager;
    
    [SerializeField] private float initialLoadDelay = 2f;
    
    private void Awake()
    {
        // Find components if not assigned
        if (arSession == null)
            arSession = FindObjectOfType<ARSession>();
            
        if (deviceRecognitionManager == null)
            deviceRecognitionManager = FindObjectOfType<DeviceRecognitionManager>();
            
        if (instructionManager == null)
            instructionManager = FindObjectOfType<ARInstructionManager>();
            
        if (uiManager == null)
            uiManager = FindObjectOfType<ARUIManager>();
            
        if (voiceManager == null)
            voiceManager = FindObjectOfType<VoiceRecognitionManager>();
            
        if (cloudManager == null)
            cloudManager = FindObjectOfType<CloudDataManager>();
    }
    
    private void Start()
    {
        // Show loading indicator
        if (uiManager != null)
            uiManager.ShowLoading(true);
            
        // Initialize AR session
        StartCoroutine(InitializeARSession());
    }
    
    private IEnumerator InitializeARSession()
    {
        // Wait for AR to initialize
        yield return new WaitForSeconds(initialLoadDelay);
        
        // Check AR availability
        if (ARSession.state == ARSessionState.None || 
            ARSession.state == ARSessionState.CheckingAvailability)
        {
            yield return ARSession.CheckAvailability();
        }
        
        // If AR is not supported, show error
        if (ARSession.state == ARSessionState.Unsupported)
        {
            Debug.LogError("AR is not supported on this device");
            // Show error UI
            yield break;
        }
        
        // Initialize device recognition
        if (deviceRecognitionManager != null)
        {
            deviceRecognitionManager.SetupReferenceLibrary();
        }
        
        // Check for content updates
        if (cloudManager != null)
        {
            cloudManager.CheckForContentUpdates();
        }
        
        // Hide loading indicator and show scan prompt
        if (uiManager != null)
        {
            uiManager.ShowLoading(false);
            uiManager.ShowScanPrompt(true);
        }
        
        // Log app start analytics
        if (cloudManager != null)
        {
            cloudManager.SendAnalyticsEvent("ar_session_ready", new System.Collections.Generic.Dictionary<string, object>());
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Handle app pausing/resuming
        if (pauseStatus)
        {
            // App is pausing
            if (cloudManager != null)
            {
                cloudManager.SendAnalyticsEvent("app_pause", new System.Collections.Generic.Dictionary<string, object>());
            }
        }
        else
        {
            // App is resuming
            if (cloudManager != null)
            {
                cloudManager.SendAnalyticsEvent("app_resume", new System.Collections.Generic.Dictionary<string, object>());
            }
        }
    }
}