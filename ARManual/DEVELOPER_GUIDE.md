# AR Manual Developer Guide

This guide provides detailed information for developers working on the AR Manual project.

## Architecture Overview

The AR Manual application follows a modular architecture with the following key components:

### Core Components

- **ARManualAppController**: Central controller that initializes and coordinates all other components.
- **SettingsManager**: Manages application settings and preferences.
- **PermissionManager**: Handles permission requests for camera and microphone.
- **DeviceDataLoader**: Loads device data from JSON files.
- **LanguageManager**: Manages multilingual support and translations.
- **VoiceRecognitionManager**: Handles voice commands for hands-free navigation.

### AR Components

- **ARSessionManager**: Manages AR session initialization and state.
- **DeviceRecognitionManager**: Handles device recognition using AR image tracking.
- **ARInstructionManager**: Manages step-by-step instructions for recognized devices.
- **AROverlayElement**: Visual elements that are overlaid on the physical device.
- **TrackedDeviceController**: Connects recognized devices with their instructions.

### UI Components

- **ARUIManager**: Manages the user interface elements.
- **ARStatusDisplay**: Displays AR session status messages.
- **SettingsPanel**: UI for changing application settings.
- **LocalizedText**: Text component that supports multiple languages.

### Cloud Components

- **CloudDataManager**: Handles cloud connectivity for content updates and analytics.

## Development Workflow

### Adding a New Device

1. Create a reference image for the device and place it in `Assets/Resources/ReferenceImages/`.
2. Create a JSON data file in `Assets/Resources/DeviceData/` with the device information and instructions.
3. Create a prefab in `Assets/Prefabs/Devices/` with visual overlays for each instruction step.
4. Add the device to the `SceneConfig.json` file in `Assets/Resources/`.

### Adding Translations

1. Update the device data JSON file with translations for each language.
2. Add translated strings to the `LanguageManager` for UI elements.

### Adding New Features

1. Create a new script in the appropriate folder (Core, AR, UI, or Cloud).
2. Add the component to the appropriate GameObject in the scene.
3. Update the `ARManualAppController` to initialize and coordinate the new component.

## Code Style Guidelines

- Use PascalCase for class names and public methods.
- Use camelCase for private fields and local variables.
- Add XML documentation comments to public methods and classes.
- Use [SerializeField] for inspector-exposed private fields.
- Group related fields and methods together.
- Keep methods short and focused on a single task.

## Testing

### Device Testing

- Test on a variety of Android and iOS devices.
- Test in different lighting conditions.
- Test with different device orientations.

### AR Testing

- Test with different reference images.
- Test with different distances from the camera to the device.
- Test with partial occlusion of the device.

### UI Testing

- Test with different screen sizes and resolutions.
- Test with different language settings.
- Test with accessibility features enabled.

## Performance Considerations

- Optimize reference images for fast recognition.
- Use low-poly models for AR overlays.
- Minimize draw calls by batching similar materials.
- Use object pooling for frequently created/destroyed objects.
- Minimize garbage collection by avoiding allocations in update loops.

## Troubleshooting

### Common Issues

- **AR Session Initialization Failure**: Check that the device supports ARCore/ARKit and has the required permissions.
- **Device Recognition Issues**: Ensure reference images have good contrast and distinct features.
- **Performance Issues**: Check for memory leaks, excessive draw calls, or high poly count models.

### Debugging Tools

- Use Unity Profiler to identify performance bottlenecks.
- Use AR Foundation's debug visualizations to troubleshoot tracking issues.
- Enable verbose logging in development builds for detailed error information.

## Release Process

1. Update version number in `ProjectSettings.asset`.
2. Run all tests on target devices.
3. Build release versions for Android and iOS.
4. Submit to app stores with appropriate metadata.
5. Update cloud content if needed.

## Resources

- [Unity AR Foundation Documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html)
- [ARCore Documentation](https://developers.google.com/ar)
- [ARKit Documentation](https://developer.apple.com/documentation/arkit)
- [TextMeshPro Documentation](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html)