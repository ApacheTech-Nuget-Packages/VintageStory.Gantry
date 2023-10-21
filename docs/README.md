# Gantry - Vintage Story MDK

Gantry MDK ("Mod Development Kit") is a toolkit for aiding the development of third-party plugins (mods), for Vintage Story, by Anego Studios.

The MDK provides a framework that can be used to develop mods with Clean Code as a foundation. Many of the features streamline the ability to implement design and architecture patterns within your mods.

## Prerequisites:

 - Vintage Story 1.17.x
 - Visual Studio 2022, with support for .NET Standard 2.0 development.
 - `VINTAGE_STORY` Environment Variable that point to the game install directory.
 - Works well with the [Vintage Story Mod Template (.NET Framework)](https://marketplace.visualstudio.com/items?itemName=ApacheTechSolutions.vsmodtemplate) extension, for Visual Studio 2022.
 
## Recommended Tools:
 
 - **[dnSpy](https://github.com/dnSpy/dnSpy)**: Debugger and .NET assembly editor. Traverse the game's API, analyse code, convert C# to IL, and back.
 - **[RedGate Smart Assembly](https://www.red-gate.com/products/dotnet-development/smartassembly/)**: Merge Gantry MDK into your mod, resulting in a single DLL file for release. 
 - **[JetBrains ReSharper](https://www.jetbrains.com/resharper/)**: Essential plugin for Visual Studio 2022. Hugely increases your productivity, and actively helps you become a better developer.
 - **[SubMain GhostDoc](https://submain.com/ghostdoc/)**: Create XML documentation within your code, with just a keyboard shortcut.

## Libraries:

The MDK is split into a number of libraries, all available for installation through Nuget. This will allow you to customise your Gantry installation, and minimise the overall size of the resulting Mod, without packing a lot of bloat into the mod archive that isn't being used.

### Core:

This library is required, and contains the core functionality of the MDK. A lot of the core features are designed to streamline mod development, and allow for rapid prototyping of new mod features.

The Core comes with a fully-featured array of tools, such as:

 - static access to the game's core API, via the `ApiEx` static class;
 - static access to the mod's metadata, via the `ModEx` static class;
 - static access to the game's assemblies, via the `GameAssemblies` static class;
 - extended support for the translation services, via the `LangEx` static class;
 - base classes for "Gantrified", sided `ModSystem` instances;
 - extensive maths library, including multiple interpolation formulae;
 - an encyclopaedia of extension methods for all areas of the game;
 - extension methods to aid functional programming for C#;
 - extension methods to aid reflection calls, using Harmony's `AccessTools`;
 - guard clause collection, accessible via the `Guard` static class;
 - cryptographic helpers, and random number generators;
 - implementation of `JetBrains.Annotations` attribute, `UsedImplicitly`;
 - implementation of `StringEnum<>`, allowing strongly-typed filtration of strings;
 - collection of commonly used strings, translated into every language the game supports;
 - base classes for Modal dialogue windows;
 - support for visual user feedback, via `MessageBox.Show(...)`;
 - scaled image support for displaying images on GUI forms;

### Dependency Injection:

This library includes a bespoke dependency injection engine, which can be used to produce very flexible, scalable, and highly optimised mods. This allows large mods to be handled very easily, especially when being used within a vertical slice architecture, where each feature of your mod is separate from eachother, within their own folders. 

Comes with a fully-featured array of tools, such as:

 - easy integration into mods, via the `ModHost` base class;
 - static access to the mod's service provider, via the `IOC` static class;
 - interfaces that allow existing classes to register services at startup;
 - sided constructor attributes that allow classes to be instantiated differently on the client, and server;
 - automatic addition of the core game API, into the service collection;
 - automatic addition of key game features, into the service collection;

### File System Service:

The file system service allows easy access to files on the hard-drive, as well as embedded resources within the mod's assembly.

The service comes with a fully-featured array of tools, such as:

 - global, and per-world feature settings;
 - file registration, with support for JSON, Text, and Binary files;
 - parsing embedded files to JSON objects or arrays;
 - dynamic application of `INotifyPropertyChanged`, via Harmony;
 - static access to special folders, via `ModPaths`;
 - static access to mod settings, via `ModSettings`;
 - base classes for classes that consume feature settings, such as Harmony patch classes;
 - base classes for feature settings dialogue windows;

The service also comes with a Dependency Injection satellite library that makes it easy to add the service into the mod's service collection.

### Harmony Patching Service:

The harmony patching service is a simple, yet powerful solution to streamlining the inclusion of harmony patches within mods.

The service comes with a fully-featured array of tools, such as:

 - sided patch classes that will only be applied on the Server, or Client;
 - default harmony instance, using the mod assembly's full name as an ID;
 - automatic patching of all annotated patch classes within the mod's assembly;
 - manual patching of annotated classes within any assembly;
 - life-cycle management of all harmony instances, and patch classes;
 - game logs of all patched methods, giving full implementation details;

The service also comes with a Dependency Injection satellite library that makes it easy to add the service into the mod's service collection.

### Network Service:

The network service acts as a one-stop solution for IPC between the client, and server.

The service comes with a fully-featured array of tools, such as:

 - default network channel, using the ModId as a channel name;
 - automatic registration of network channels, on use;
 - extension methods to aid the publishing of network packets;
 - un-registration of message handlers for generic types;
 - signal packet to act as a ping, between app-sides;

The service also comes with a Dependency Injection satellite library that makes it easy to add the service into the mod's service collection.

## Quick Start

 1. Start a new Class Library project in Visual Studio 2022, targetting .NET Standard 2.0.
 2. Open the `.csproj` file, and add the following:  
  
```xml  
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net7.0</TargetFramework>
		<LangVersion>11</LangVersion>
		<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
	</PropertyGroup>

	<ItemGroup>
		<Reference Include="VintagestoryAPI">
			<HintPath>$(VINTAGE_STORY)\VintagestoryAPI.dll</HintPath>
			<Private>false</Private>
		</Reference>
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="VintageStory.Gantry" Version="0.6.0" />
	</ItemGroup>

</Project>  
```

 3. If you wish to use the Nuget Package Installer, you can install Gantry using the following command:

```text
    Install-Package VintageStory.Gantry  
```

 4. Once installed, you can start using the Gantrified `ModSystem` base classes, and flesh out your mod to your liking. 

```csharp
    public class QuickStartModSystem : ClientModSystem
    {
        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.RegisterCommand("hello", Lang.Get("samplegantrymod:hello-world-description"), string.Empty, OnChatCommand);
        }
        
        private void OnChatCommand(int groupId, CmdArgs args)
        {
            ApiEx.Client.ShowChatMessage(Lang.Get("samplegantrymod:hello-world-text"));
        }
    }
```
 5. Add a `ModInfoAttribute` to the assembly.
  
```csharp  
[assembly: ModInfo(
    "Sample Gantry Mod",
    "samplegantrymod",
    Description = "A sample mod, using the Gantry MDK.",
    Side = "Universal",
    Version = "0.0.1",
    Website = "https://apachetech.co.uk",
    Contributors = new[] { "ApacheTech Solutions" },
    Authors = new[] { "ApacheTech Solutions" })];
```  
 
 6. Package all required `.dll` files within your mod archive, from the output directory, when you run your mod.

## **Support the Author:**

The Gantry Mod Development Kit is developed, and maintained by me; Apache. 
I'm a professional .NET developer, a content creator, and and a long time player, and modder, of Vintage Story.
I've contributed code to the game, including a re-write of the translation engine, 
some OpenGL GUI Elements, and a number of optimisations, and bug fixes.

All of this, I've done on a voluntary basis, and as I've been more active within the community, people have requested ways to show support for the work I produce, and the help I've given. So, I have a number of options available, to that end.

Thank you to everyone that does support me, and my work.

 - **Donate via PayPal:**  *https://bit.ly/APGDonate*
 - **Buy me a Coffee:** *https://www.buymeacoffee.com/Apache*
 - **Amazon Wish List:** *http://amzn.eu/7qvKTFu*
 - **Subscribe on YouTube:** *https://youtube.com/ApacheGamingUK*
 - **Subscribe on Twitch.TV:** *https://twitch.tv/ApacheGamingUK*
 - **Donate on StreamLabs:** *https://www.streamlabs.com/ApacheGamingUK*

### **Charitable Causes**

All donations go towards making new content on a non-profit basis. Software purchases and subscriptions, hardware upgrades, training, etc. 
Any profit made will be donated to [Macmillan Cancer Support](https://www.macmillan.org.uk/), and [The British Liver Trust](https://britishlivertrust.org.uk/), in memory of my father, who died of Liver Cancer in 2015.


