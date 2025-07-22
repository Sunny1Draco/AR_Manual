using System;
using System.Collections.Generic;
using UnityEngine;

public class ARInstructionManager : MonoBehaviour
{
    [SerializeField] private DeviceRecognitionManager deviceRecognitionManager;
    
    [Serializable]
    public class InstructionStep
    {
        public string stepId;
        public string title;
        public string description;
        public AudioClip voiceOver;
        public GameObject visualElements;
        public float autoAdvanceDelay = 0f; // 0 means manual advance
    }
    
    [Serializable]
    public class DeviceInstructions
    {
        public string deviceId;
        public List<InstructionStep> steps = new List<InstructionStep>();
    }
    
    [SerializeField] private List<DeviceInstructions> deviceInstructionsList = new List<DeviceInstructions>();
    
    private Dictionary<string, DeviceInstructions> deviceInstructionsDict = new Dictionary<string, DeviceInstructions>();
    private string currentDeviceId;
    private int currentStepIndex = -1;
    
    public event Action<InstructionStep> OnStepChanged;
    public event Action<string, int, int> OnProgressChanged; // deviceId, currentStep, totalSteps
    
    private void Awake()
    {
        if (deviceRecognitionManager == null)
            deviceRecognitionManager = FindObjectOfType<DeviceRecognitionManager>();
            
        // Create lookup dictionary for faster access
        foreach (var instructions in deviceInstructionsList)
        {
            deviceInstructionsDict[instructions.deviceId] = instructions;
        }
    }
    
    private void OnEnable()
    {
        deviceRecognitionManager.OnDeviceRecognized += HandleDeviceRecognized;
    }
    
    private void OnDisable()
    {
        deviceRecognitionManager.OnDeviceRecognized -= HandleDeviceRecognized;
    }
    
    private void HandleDeviceRecognized(string deviceId)
    {
        // If this is a new device or we're restarting instructions for the same device
        if (currentDeviceId != deviceId || currentStepIndex < 0)
        {
            currentDeviceId = deviceId;
            currentStepIndex = -1;
            
            // Start the instruction sequence
            NextStep();
        }
    }
    
    public void NextStep()
    {
        if (string.IsNullOrEmpty(currentDeviceId))
            return;
            
        if (!deviceInstructionsDict.TryGetValue(currentDeviceId, out DeviceInstructions instructions))
            return;
            
        // Deactivate current step if any
        if (currentStepIndex >= 0 && currentStepIndex < instructions.steps.Count)
        {
            var currentStep = instructions.steps[currentStepIndex];
            if (currentStep.visualElements != null)
                currentStep.visualElements.SetActive(false);
        }
        
        // Move to next step
        currentStepIndex++;
        
        // Check if we've reached the end
        if (currentStepIndex >= instructions.steps.Count)
        {
            currentStepIndex = instructions.steps.Count - 1;
            return;
        }
        
        // Activate new step
        var newStep = instructions.steps[currentStepIndex];
        if (newStep.visualElements != null)
            newStep.visualElements.SetActive(true);
            
        // Play voice over if available
        if (newStep.voiceOver != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = newStep.voiceOver;
                audioSource.Play();
            }
        }
        
        // Notify listeners
        OnStepChanged?.Invoke(newStep);
        OnProgressChanged?.Invoke(currentDeviceId, currentStepIndex + 1, instructions.steps.Count);
        
        // Auto advance if specified
        if (newStep.autoAdvanceDelay > 0)
        {
            Invoke(nameof(NextStep), newStep.autoAdvanceDelay);
        }
    }
    
    public void PreviousStep()
    {
        if (string.IsNullOrEmpty(currentDeviceId))
            return;
            
        if (!deviceInstructionsDict.TryGetValue(currentDeviceId, out DeviceInstructions instructions))
            return;
            
        // Deactivate current step if any
        if (currentStepIndex >= 0 && currentStepIndex < instructions.steps.Count)
        {
            var currentStep = instructions.steps[currentStepIndex];
            if (currentStep.visualElements != null)
                currentStep.visualElements.SetActive(false);
                
            // Cancel any pending auto-advance
            CancelInvoke(nameof(NextStep));
        }
        
        // Move to previous step
        currentStepIndex--;
        
        // Check if we've reached the beginning
        if (currentStepIndex < 0)
        {
            currentStepIndex = 0;
            return;
        }
        
        // Activate new step
        var newStep = instructions.steps[currentStepIndex];
        if (newStep.visualElements != null)
            newStep.visualElements.SetActive(true);
            
        // Play voice over if available
        if (newStep.voiceOver != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = newStep.voiceOver;
                audioSource.Play();
            }
        }
        
        // Notify listeners
        OnStepChanged?.Invoke(newStep);
        OnProgressChanged?.Invoke(currentDeviceId, currentStepIndex + 1, instructions.steps.Count);
    }
    
    public InstructionStep GetCurrentStep()
    {
        if (string.IsNullOrEmpty(currentDeviceId))
            return null;
            
        if (!deviceInstructionsDict.TryGetValue(currentDeviceId, out DeviceInstructions instructions))
            return null;
            
        if (currentStepIndex < 0 || currentStepIndex >= instructions.steps.Count)
            return null;
            
        return instructions.steps[currentStepIndex];
    }
}