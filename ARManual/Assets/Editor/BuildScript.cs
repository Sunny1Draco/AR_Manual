using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class BuildScript
{
    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        string[] scenes = { "Assets/Scenes/Main.unity" };
        string buildPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ARManual.apk");
        
        PlayerSettings.Android.keystorePass = "YOUR_KEYSTORE_PASSWORD";
        PlayerSettings.Android.keyaliasPass = "YOUR_KEY_PASSWORD";
        
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.None);
        
        Debug.Log("Android build completed: " + buildPath);
    }
    
    [MenuItem("Build/Build iOS")]
    public static void BuildIOS()
    {
        string[] scenes = { "Assets/Scenes/Main.unity" };
        string buildPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ARManual_iOS");
        
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.iOS, BuildOptions.None);
        
        Debug.Log("iOS build completed: " + buildPath);
    }
    
    [MenuItem("Build/Setup Project")]
    public static void SetupProject()
    {
        // Set company name and product name
        PlayerSettings.companyName = "YourCompanyName";
        PlayerSettings.productName = "AR Manual";
        
        // Set version
        PlayerSettings.bundleVersion = "1.0.0";
        
        // Set bundle identifier
        PlayerSettings.applicationIdentifier = "com.yourcompany.armanual";
        
        // Set minimum API level for Android
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        
        // Set minimum iOS version
        PlayerSettings.iOS.targetOSVersionString = "13.0";
        
        // Set scripting backend
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        
        // Set architecture
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        
        Debug.Log("Project setup completed");
    }
}