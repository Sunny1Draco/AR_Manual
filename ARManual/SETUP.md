# AR Manual Setup Guide

This guide will help you set up the AR Manual project in Unity.

## Prerequisites

- Unity 2022.3 LTS or newer
- Android SDK (for Android builds)
- Xcode 14+ (for iOS builds)
- Git

## Installation Steps

1. **Clone the repository**:
   ```
   git clone https://github.com/yourusername/ar-manual.git
   ```

2. **Open the project in Unity**:
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Open the project with Unity 2022.3 LTS or newer

3. **Install required packages**:
   - Open the Package Manager (Window > Package Manager)
   - Install the following packages:
     - AR Foundation (com.unity.xr.arfoundation)
     - ARCore XR Plugin (com.unity.xr.arcore)
     - ARKit XR Plugin (com.unity.xr.arkit)
     - TextMeshPro (com.unity.textmeshpro)
     - Universal Render Pipeline (com.unity.render-pipelines.universal)

4. **Configure XR Plugin Management**:
   - Go to Edit > Project Settings > XR Plugin Management
   - Click "Install XR Plugin Management" if not already installed
   - Enable ARCore on Android tab
   - Enable ARKit on iOS tab

5. **Set up the project**:
   - Go to Build > Setup Project in the menu
   - This will configure all the necessary project settings

6. **Open the main scene**:
   - Navigate to Assets/Scenes in the Project window
   - Double-click on Main.unity to open it

7. **Configure device reference images**:
   - Place reference images in Assets/Resources/ReferenceImages
   - Each image should be named after the device it represents (e.g., SmartThermostat.jpg)

8. **Add device data**:
   - Edit the JSON files in Assets/Resources/DeviceData to add your device information
   - Follow the format in the existing sample files

9. **Create device prefabs**:
   - Create prefabs for each device in Assets/Prefabs/Devices
   - Each prefab should contain the AR overlay elements for the device

10. **Build the project**:
    - For Android: Go to Build > Build Android
    - For iOS: Go to Build > Build iOS

## Project Structure

- **Assets/Scripts/Core**: Core application scripts
- **Assets/Scripts/AR**: AR-related functionality
- **Assets/Scripts/UI**: User interface scripts
- **Assets/Scripts/Cloud**: Cloud connectivity scripts
- **Assets/Prefabs**: Reusable prefabs
- **Assets/Resources**: Data files and resources
- **Assets/Scenes**: Unity scenes

## Adding New Devices

1. Add a reference image to Assets/Resources/ReferenceImages
2. Create a device data JSON file in Assets/Resources/DeviceData
3. Create a device prefab in Assets/Prefabs/Devices
4. Add the device to the SceneConfig.json file in Assets/Resources

## Troubleshooting

- **AR not working on device**: Ensure camera permissions are granted and the device supports ARCore/ARKit
- **Reference images not recognized**: Make sure images are high contrast and have distinct features
- **Build errors**: Check that all required packages are installed and compatible

## Contact

For support, contact: your.email@example.com