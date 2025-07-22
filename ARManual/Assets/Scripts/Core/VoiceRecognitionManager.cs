using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class VoiceRecognitionManager : MonoBehaviour
{
    [SerializeField] private ARInstructionManager instructionManager;
    [SerializeField] private GameObject microphoneIcon;
    [SerializeField] private GameObject listeningIndicator;
    
    private bool isListening = false;
    
    // Keywords for voice commands
    private readonly string[] nextStepKeywords = { "next", "continue", "forward" };
    private readonly string[] prevStepKeywords = { "back", "previous", "return" };
    private readonly string[] helpKeywords = { "help", "assistance", "explain" };
    
    public event Action<string> OnSpeechRecognized;
    
    private void Awake()
    {
        if (instructionManager == null)
            instructionManager = FindObjectOfType<ARInstructionManager>();
            
        // Initially hide listening indicator
        if (listeningIndicator != null)
            listeningIndicator.SetActive(false);
    }
    
    private void Start()
    {
        // Request microphone permission
        RequestMicrophonePermission();
    }
    
    private void RequestMicrophonePermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#elif UNITY_IOS
        // iOS permission handling
#endif
    }
    
    public void ToggleListening()
    {
        isListening = !isListening;
        
        if (listeningIndicator != null)
            listeningIndicator.SetActive(isListening);
            
        if (isListening)
        {
            StartListening();
        }
        else
        {
            StopListening();
        }
    }
    
    private void StartListening()
    {
        // In a real implementation, this would connect to the platform's speech recognition API
        // For this example, we'll simulate speech recognition with a coroutine
        StartCoroutine(SimulateSpeechRecognition());
    }
    
    private void StopListening()
    {
        StopAllCoroutines();
    }
    
    private IEnumerator SimulateSpeechRecognition()
    {
        // In a real implementation, this would be replaced with actual speech recognition
        Debug.Log("Started listening for voice commands...");
        
        // Simulate processing time
        yield return new WaitForSeconds(3f);
        
        // Simulate a recognized command (in a real app, this would come from the speech API)
        string[] possibleCommands = { "next", "back", "help" };
        string recognizedText = possibleCommands[UnityEngine.Random.Range(0, possibleCommands.Length)];
        
        ProcessSpeechCommand(recognizedText);
        
        // Turn off listening after processing a command
        isListening = false;
        if (listeningIndicator != null)
            listeningIndicator.SetActive(false);
    }
    
    private void ProcessSpeechCommand(string command)
    {
        Debug.Log($"Recognized speech command: {command}");
        
        // Notify listeners
        OnSpeechRecognized?.Invoke(command);
        
        // Check for navigation commands
        if (ContainsAny(command, nextStepKeywords))
        {
            instructionManager.NextStep();
        }
        else if (ContainsAny(command, prevStepKeywords))
        {
            instructionManager.PreviousStep();
        }
        else if (ContainsAny(command, helpKeywords))
        {
            // Show additional help for current step
            // This would be implemented based on your specific help system
            Debug.Log("Showing help for current step");
        }
    }
    
    private bool ContainsAny(string text, string[] keywords)
    {
        text = text.ToLower();
        
        foreach (string keyword in keywords)
        {
            if (text.Contains(keyword.ToLower()))
                return true;
        }
        
        return false;
    }
}