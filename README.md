# Note: This repo is now archived. It is still available READ ONLY for forking or historical interest.

# UnityUWPBTLE
A unity project that uses a UWP BTLE plug in winmd to access windows BTLE functionality.

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
The following sections serve as guide posts to help navigate the code by
explaining some of the larger systems, both how they work and how they
interact.

# Sample Device Note
This project is a framework for BTLE communication.  As such it does not actually connect to any particular BTLE device.  You will need to replace the 
code in the UnityUWPBTLE\Build\Scripts\SampleDevice directory with code relevant to your particular BTLE device.

## Requirements
Minimum MSVS 2017
Minimum Unity 2017.3.x

## Related
The plugin used in this project is can be found at [Microsoft/UnityuWPBTLEPlugin](https://github.com/Microsoft/UnityUWPBTLEPlugin/)

# Running in Unity
The Unity sample project can be found in the UnityUWPBTLE directory.  Open Unity 2017 and open the project "On Disk"

## UnityUWPBTLE

The unity sample project.

## UnityUWPBTLE\Build
The unity project assets 

## UnityUWPBTLE\Build\_Scenes
The main unity scene

## UnityUWPBTLE\Build\Plugins
This is where the UnityUWPBTLEPlugin.winmd plugin is placed for the project use.

## UnityUWPBTLE\Build\Prefabs
A helper scrolling panel for program output

## UnityUWPBTLE\Build\Scripts
Where the unity scripts are for the project

## UnityUWPBTLE\Build\Scripts\SampleDevice
The location of a dummy BTLE device.  Use as a guideline for your BTLE device communications. 

## UnityUWPBTLE\Build
This directory contains some files which will be overlapped with a Unity generated XAML project.  It contains a working visual studio project solution that can be explored.

# Building
From Unity, choose File->Build Settings to bring up the Build Settings
window. All of the scenes in the Scenes to Build section should be checked.
Choose Universal Windows Platform as the Platform. On the right side, choose
"any device" as the Target device, XAML as the UWP Build Type, 10.0.16299.0
as the SDK, check "Unity C# Projects" and then click Build. Select the folder
called 'UWP' and choose this folder.

After the build completes successfully, an explorer window will pop up.
Navigate into the UWP folder and double-click `UnityUWPBTLE.sln` to launch
Visual Studio. From Visual Studio, set the Configuration to **Release**
for faster builds (doesn't use .NET Native) or **Master** to build the
type of package the Store needs (uses .NET Native).





