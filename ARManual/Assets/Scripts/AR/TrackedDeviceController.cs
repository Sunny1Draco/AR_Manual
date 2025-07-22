using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackedDeviceController : MonoBehaviour
{
    [SerializeField] private string deviceId;
    
    private ARTrackedImage trackedImage;
    private DeviceRecognitionManager deviceManager;
    private ARInstructionManager instructionManager;
    
    private void Awake()
    {
        trackedImage = GetComponentInParent<ARTrackedImage>();
        deviceManager = FindObjectOfType<DeviceRecognitionManager>();
        instructionManager = FindObjectOfType<ARInstructionManager>();
    }
    
    private void OnEnable()
    {
        // Notify the instruction manager that this device is being tracked
        if (instructionManager != null)
        {
            instructionManager.HandleDeviceRecognized(deviceId);
        }
    }
    
    public string GetDeviceId()
    {
        return deviceId;
    }
    
    public void SetDeviceId(string id)
    {
        deviceId = id;
    }
}