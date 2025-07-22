using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DeviceRecognitionManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private ARSession arSession;
    
    [Serializable]
    public class DeviceMapping
    {
        public string deviceId;
        public Texture2D referenceImage;
        public GameObject instructionPrefab;
    }
    
    [SerializeField] private List<DeviceMapping> deviceMappings = new List<DeviceMapping>();
    
    private Dictionary<string, GameObject> activeInstructions = new Dictionary<string, GameObject>();
    private Dictionary<string, DeviceMapping> deviceMappingDict = new Dictionary<string, DeviceMapping>();
    
    public event Action<string> OnDeviceRecognized;
    
    private void Awake()
    {
        if (trackedImageManager == null)
            trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
            
        if (arSession == null)
            arSession = FindObjectOfType<ARSession>();
            
        // Create lookup dictionary for faster access
        foreach (var mapping in deviceMappings)
        {
            deviceMappingDict[mapping.deviceId] = mapping;
        }
    }
    
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle added images
        foreach (var trackedImage in eventArgs.added)
        {
            HandleTrackedImage(trackedImage);
        }
        
        // Handle updated images
        foreach (var trackedImage in eventArgs.updated)
        {
            HandleTrackedImage(trackedImage);
        }
        
        // Handle removed images
        foreach (var trackedImage in eventArgs.removed)
        {
            // Remove the instruction overlay when the image is no longer tracked
            string deviceId = trackedImage.referenceImage.name;
            if (activeInstructions.TryGetValue(deviceId, out GameObject instructionObj))
            {
                instructionObj.SetActive(false);
                activeInstructions.Remove(deviceId);
            }
        }
    }
    
    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string deviceId = trackedImage.referenceImage.name;
        
        // Check if we have instructions for this device
        if (deviceMappingDict.TryGetValue(deviceId, out DeviceMapping mapping))
        {
            // If the instruction object doesn't exist yet, create it
            if (!activeInstructions.TryGetValue(deviceId, out GameObject instructionObj))
            {
                instructionObj = Instantiate(mapping.instructionPrefab, trackedImage.transform);
                activeInstructions[deviceId] = instructionObj;
                
                // Notify listeners that a device was recognized
                OnDeviceRecognized?.Invoke(deviceId);
            }
            
            // Update position and rotation to match the tracked image
            instructionObj.transform.position = trackedImage.transform.position;
            instructionObj.transform.rotation = trackedImage.transform.rotation;
            
            // Only show the instruction if the tracking state is good
            bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
            instructionObj.SetActive(isTracking);
        }
    }
    
    public void SetupReferenceLibrary()
    {
        // Create a mutable reference library
        var mutableLibrary = trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        
        if (mutableLibrary != null)
        {
            // Add each reference image to the library
            foreach (var mapping in deviceMappings)
            {
                if (mapping.referenceImage != null)
                {
                    // Add the reference image to the library
                    mutableLibrary.ScheduleAddImageWithValidationJob(
                        mapping.referenceImage,
                        mapping.deviceId,
                        0.1f); // Physical size in meters (adjust as needed)
                }
            }
        }
    }
}