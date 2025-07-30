# Mod Packager

These are the steps to create a mod package with the Mod Packager.

## Command Line Arguments
- `TargetPath`: The path to the mod assembly file.
- `TargetDir`: The directory where the mod files are located.
- `DependenciesDir`: The directory where the dependencies are located.
- `VersioningStyle`: The versioning style to use (e.g., ModInfo, Assembly, or Custom).
- `Version`: The version of the mod to use, if using Custom versioning style
- `LogLevel`: The minimum log level to use (e.g. Information, Debug, Verbose).

- `ProjectDir`: The directory where the project files are located.
- `ModId`: The unique identifier for the mod.

- `SolutionDir`: The root directory of the solution.
- `Configuration`: The configuration to use (Debug, Release, or Package).

## Shared Prerequisites

### Preparation
 - [x] Generate a modinfo.json file for the mod in `TargetDir`.
 - [x] Copy all files from `SolutionDir/.gantry` into `TargetDir`.
 - [x] Recursively copy all `_Includes` folders from the mod into `TargetDir/_Includes`.
 - [x] Patch and merge translations into `TargetDir/_Includes`.

## Debug Configuration
	
### Copy Files
 - [x] Copy `TargetDir` to `SolutionDir/.debug/ProjectName`.

### Smart Assembly
 - [x] Copy `SolutionDir/.debug/ProjectName/ProjectName.dll` to `SolutionDir/.debug/ProjectName/unmerged/ProjectName.dll`. 
 - [x] Generate saproj file to merge Gantry into `SolutionDir/.debug/ProjectName/ProjectName.dll`.
 - [x] Save the saproj file in `SolutionDir/.debug/ProjectName/ProjectName.saproj`. 
 - [x] Run the saproj file to merge Gantry into `SolutionDir/.debug/ProjectName/ProjectName.dll`.

### Cleanup
 - [x] Move all files from `SolutionDir/.debug/ProjectName/_Includes` to `SolutionDir/.debug/ProjectName`.
 - [x] Delete `SolutionDir/.debug/ProjectName/_Includes` directory.

## Package Configuration
	
### Smart Assembly
 - [ ] Copy all files from `TargetDir` into `SolutionDir/.tmp` directory.
 - [ ] Generate a saproj file to merge the mod to `SolutionDir/.tmp/ProjectName.dll`.
 - [ ] Save the saproj file in `SolutionDir/.tmp/ProjectName.saproj`.
 - [ ] Run the saproj file to merge the mod to `SolutionDir/.tmp/ProjectName.dll`.

### Smart Assembly Cleanup
 - [ ] Remove files handled by Smart Assembly
 - [ ] Remove compile-time only reference files
 - [ ] Remove junk files
 
### Create Mod Package
 - [ ] Zip the contents of `SolutionDir/.tmp` into `SolutionDir/.releases/ProjectName_vVersion.zip`.
 - [ ] Copy `SolutionDir/.releases/ProjectName_vVersion.zip` to `env:VsModArtifactsDir`.

### Cleanup
 - [ ] Delete `SolutionDir/.tmp` directory.