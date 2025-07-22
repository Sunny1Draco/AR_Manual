using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARSessionManager : MonoBehaviour
{
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private ARCameraManager cameraManager;
    
    [SerializeField] private float initializationTimeout = 10f;
    
    public enum ARSessionState
    {
        Initializing,
        Ready,
        Failed,
        Unsupported
    }
    
    public ARSessionState CurrentState { get; private set; } = ARSessionState.Initializing;
    
    public delegate void ARSessionStateChangedHandler(ARSessionState newState);
    public event ARSessionStateChangedHandler OnARSessionStateChanged;
    
    private void Awake()
    {
        if (arSession == null)
            arSession = FindObjectOfType<ARSession>();
            
        if (trackedImageManager == null)
            trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
            
        if (cameraManager == null)
            cameraManager = FindObjectOfType<ARCameraManager>();
    }
    
    private void Start()
    {
        StartCoroutine(InitializeARSession());
    }
    
    private IEnumerator InitializeARSession()
    {
        // Wait for AR to initialize
        float timeElapsed = 0f;
        
        while (ARSession.state == UnityEngine.XR.ARSubsystems.ARSessionState.None || 
               ARSession.state == UnityEngine.XR.ARSubsystems.ARSessionState.CheckingAvailability)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
            
            if (timeElapsed > initializationTimeout)
            {
                SetState(ARSessionState.Failed);
                yield break;
            }
        }
        
        // Check if AR is supported
        if (ARSession.state == UnityEngine.XR.ARSubsystems.ARSessionState.Unsupported)
        {
            SetState(ARSessionState.Unsupported);
            yield break;
        }
        
        // Initialize tracked image library if needed
        if (trackedImageManager != null && trackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary)
        {
            yield return StartCoroutine(LoadReferenceImages());
        }
        
        // AR session is ready
        SetState(ARSessionState.Ready);
    }
    
    private IEnumerator LoadReferenceImages()
    {
        // This would load reference images from Resources or other sources
        // For now, we'll just yield to simulate loading time
        yield return new WaitForSeconds(0.5f);
    }
    
    private void SetState(ARSessionState newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            OnARSessionStateChanged?.Invoke(newState);
        }
    }
    
    public void RestartSession()
    {
        if (arSession != null)
        {
            arSession.Reset();
            StartCoroutine(InitializeARSession());
        }
    }
    
    public void ToggleTracking(bool enable)
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = enable;
        }
    }
}