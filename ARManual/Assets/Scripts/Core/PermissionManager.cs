using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class PermissionManager : MonoBehaviour
{
    public enum PermissionType
    {
        Camera,
        Microphone
    }
    
    public delegate void PermissionCallback(PermissionType permission, bool granted);
    public event PermissionCallback OnPermissionResult;
    
    private static PermissionManager _instance;
    public static PermissionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("PermissionManager");
                _instance = go.AddComponent<PermissionManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public bool HasPermission(PermissionType permission)
    {
        switch (permission)
        {
            case PermissionType.Camera:
                #if UNITY_ANDROID
                return Permission.HasUserAuthorizedPermission(Permission.Camera);
                #elif UNITY_IOS
                return Application.HasUserAuthorization(UserAuthorization.WebCam);
                #else
                return true;
                #endif
                
            case PermissionType.Microphone:
                #if UNITY_ANDROID
                return Permission.HasUserAuthorizedPermission(Permission.Microphone);
                #elif UNITY_IOS
                return Application.HasUserAuthorization(UserAuthorization.Microphone);
                #else
                return true;
                #endif
                
            default:
                return false;
        }
    }
    
    public void RequestPermission(PermissionType permission)
    {
        switch (permission)
        {
            case PermissionType.Camera:
                #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Permission.RequestUserPermission(Permission.Camera);
                    StartCoroutine(CheckPermissionAfterRequest(permission));
                }
                else
                {
                    OnPermissionResult?.Invoke(permission, true);
                }
                #elif UNITY_IOS
                StartCoroutine(RequestIOSPermission(UserAuthorization.WebCam, permission));
                #else
                OnPermissionResult?.Invoke(permission, true);
                #endif
                break;
                
            case PermissionType.Microphone:
                #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                {
                    Permission.RequestUserPermission(Permission.Microphone);
                    StartCoroutine(CheckPermissionAfterRequest(permission));
                }
                else
                {
                    OnPermissionResult?.Invoke(permission, true);
                }
                #elif UNITY_IOS
                StartCoroutine(RequestIOSPermission(UserAuthorization.Microphone, permission));
                #else
                OnPermissionResult?.Invoke(permission, true);
                #endif
                break;
        }
    }
    
    private IEnumerator CheckPermissionAfterRequest(PermissionType permission)
    {
        // Wait a moment for the permission dialog to be answered
        yield return new WaitForSeconds(0.5f);
        
        bool granted = HasPermission(permission);
        OnPermissionResult?.Invoke(permission, granted);
    }
    
    #if UNITY_IOS
    private IEnumerator RequestIOSPermission(UserAuthorization authorizationType, PermissionType permission)
    {
        yield return Application.RequestUserAuthorization(authorizationType);
        bool granted = Application.HasUserAuthorization(authorizationType);
        OnPermissionResult?.Invoke(permission, granted);
    }
    #endif
    
    public void RequestAllRequiredPermissions()
    {
        RequestPermission(PermissionType.Camera);
        
        // Only request microphone if voice commands are enabled
        if (SettingsManager.Instance.appSettings.voiceCommandsEnabled)
        {
            RequestPermission(PermissionType.Microphone);
        }
    }
}