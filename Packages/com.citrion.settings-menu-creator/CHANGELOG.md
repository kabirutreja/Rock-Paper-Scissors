# Changelog

## [1.6.7] - July 10th 2026
- Add Unity 6.5 compatibility

## [1.6.6] - March 26th 2026
- Added more [Serializable] attributes to remove warnings in Unity 6.3

## [1.6.5] - November 30th 2025
- Fixed game view window stealing focus when exiting play mode.

## [1.6.4] - November 28th 2025
- Added [Serializable] attributes to remove warnings in Unity 6.3

## [1.6.3] - November 16th 2025
- NEW Link.xml files for WebGL & IL2CPP compatibility. Available as integrations
- NEW GetCurrentSettingsValue(s) methods in SettingsCollection
- Fixed PreviousNextSelector interfering with navigation
- Extended OnSettingValueChangeListener to support updating when enabled if a SettingsCollection is referenced
- Removed obsolete (edit mode) layout prefab presets
- Updated Documentation & added WebGL/IL2CPP documentation

## [1.6.2] - September 1st 2025
- Fixed issue with LocalizationBehaviour

## [1.6.1] - August 26th 2025
- Adjusted ScrollRect scroll speed for Unity 6
- Improved internal event handling
- Added support to properly handle settings of type integer with localization
- Replaced null propagation checks on Unity objects

## [1.6.0] - August 22nd 2025
- NEW UI Toolkit localization support
- NEW Switch UI element (based on a Toggle)
- NEW Game view resolution reverts back to original after leaving play mode
- Updated various prefabs to support outlines with a StyleProfile
- Updated TND upscaler asset integrations to work with their latest framework
- Updated third party localization integrations to contain UI Toolkit demo scenes
- Updated Documentation

## [1.5.3] - June 17th 2025
- Added automatic CanvasGroup handling to prevent unintended menu navigation when panel/menu is closed

## [1.5.2] - June 6th 2025
- Added panel closing on application quit

## [1.5.1] - June 2nd 2025
- Improved handling of default values loading

## [1.5.0] - June 1st 2025
- NEW Added Unity Localization Integration
- NEW Added Beautify 3 Integration for URP
- NEW Added Beautify 3 Integration for BIRP
- NEW Added Beautify HDRP Integration
- NEW names to UI Toolkit PreviousNextSelector hierarchy to allow style customization

## [1.4.2] - April 28th 2025
- Fixed issue with runtime UI element creation in Unity 6
- A few minor fixes and improvements

## [1.4.1] - February 5th 2025
- Fixed incorrect link to I2 Localization asset in documentation

## [1.4.0] - February 4th 2025
- NEW Generic localization integration support
- NEW I2 localization integration
- NEW Lean localization integration
- NEW 'Save On Close' option to settings menu script
- Updated documentation
- Fixed input element removal via SettingsCollection not saving properly in an open prefab stage

## [1.3.4] - November 9th 2024
- Updated DLSS integration to be compatible with the latest DLSS asset version

## [1.3.3] - November 5th 2024
- Fixed 'Input Element Provider' fields not showing

## [1.3.2] - November 3rd 2024
- Updated FSR 3 integration to make it work again with the latest version
- NEW Integration for FMOD asset by FMOD

## [1.3.1] - September 23rd 2024
- Fixed build errors (thanks to -NiallMc for reporting)

## [1.3.0] - September 16th 2024
Overview Video: https://www.youtube.com/watch?v=A-XuDnAFcR0
- NEW Added edit mode prefab creation and modification tools to the SettingsCollection. 
  This now enables easy creation and updating of a layout prefab used for the settings menu. 
  Using this it is possible to easily customize the settings menu without ever leaving the 
  prefab workflow or relying on the runtime generation of UI elements for your settings. 
- Improved tab menu & its inspector 
- Improved settings initialization 
- Improved runtime UI element ordering

## [1.2.0] - August 28th 2024
- NEW Integrations tab in manager window (Tools > CitrioN > Settings Menu Creator > Integrations
- NEW Integration for the FSR 3 asset from The Naked Dev
- NEW Integration for DLSS asset from The Naked Dev
- NEW Integration for Master Audio from Dark Tonic Inc. 
- NEW Stepslider now has an option to offset the display value.
- NEW Setting options variable for stepslider value visuals offset: VALUE_VISUALS_OFFSET
- Updated default options for many settings to display decimals from 0 - 1 as 0 - 100 % on a step slider with input field.

## [1.1.3] - August 19th 2024
- Added UGUI tab menu edit mode initialization
- Disabled UI element adding if parent is not found
- Improved prefab creation of the MenuWithInputElementsPrefabCreator
- NEW Added OnPrefabCreationActionInvoker script to be invoked when the MenuWithInputElementsPrefabCreator adds content to a prefab
- NEW Added FAQ
- NEW UI Components documentation
- NEW Added delete save setting
- NEW Added SettingsCollectionUtility script to allow adding & removing of settings with code.
- Updated existing documentation
- Updated button base prefab to have the circle image by default.

## [1.1.2] - August 6th 2024
- NEW Added asset comparison image to welcome tab
- Fixed visible UI element resizing when first opening a settings menu

## [1.1.1] - August 1st 2024
- NEW Added setting options variable support for stepslider value visuals multiplier and value suffix (VALUE_VISUALS_MULTIPLIER & VALUE_SUFFIX)
- Added 'Delete Save' context menu to settings menu scripts
- Fixed minor issue with main menu demo scene
- NEW Disable regular console logs of the asset with the ConsoleLoggerDisabler script

## [1.1.0] - July 24th 2024
Overview Video: https://youtu.be/74dX4WS7WEM
- NEW Edit mode support for style profile system
- NEW MonoBehaviours and ScriptableObjects now have descriptions that can be toggled to learn more about the script and its fields
- NEW Uninstall tab for easier removal of the packages the asset consists of
- NEW Stepslider now has an option to display the value with an offset and suffix. Change 0.5 for example to 5 %
- Added required asset reimport check
- Stepslider input field now applies its value when deselected
- Improved all manager window tabs
- Improved automatic navigation
- Fixed float rounding issue
- Fixed 2023.1 UxmlElement & UxmlAttribute usage (Thanks to Caden for reporting it)
- Changed settings savers to not append data by default

## [1.0.2] - June 17th 2024
- Updated welcome tab
- NEW Documentation tab
- NEW Support Tab
- NEW UI Toolkit demo scene
- Updated base text prefab to use correct text mesh pro material
- Updated how dynamic text prefabs listen to text size changes
- Fixed incorrect version check (Thanks to Chnappi for reporting it)
- Fixed incorrect method call if new input system is in project (Thanks to Chnappi for reporting it)
- Fixed menu refresh causing manually placed elements to be removed too

## [1.0.1] - May 21st 2024
- NEW Unity 6 Support
- Fixed several minor bugs & typos
- Made several small quality of life improvements

## [1.0.0] - May 2024
First release.