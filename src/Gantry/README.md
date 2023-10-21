# Gantry - Vintage Story MDK

The Gantry Mod Development Kit is a Core Framework that makes it easier to make Code Mods for Vintage Story.

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

 ## File System Service

Provides a service for interacting with the file system.

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

## Harmony Patching Service

Provides a service for patching the game with Harmony, when using Gantry MDK.

The harmony patching service is a simple, yet powerful solution to streamlining the inclusion of harmony patches within mods.

The service comes with a fully-featured array of tools, such as:

 - sided patch classes that will only be applied on the Server, or Client;
 - default harmony instance, using the mod assembly's full name as an ID;
 - automatic patching of all annotated patch classes within the mod's assembly;
 - manual patching of annotated classes within any assembly;
 - life-cycle management of all harmony instances, and patch classes;
 - game logs of all patched methods, giving full implementation details;

The service also comes with a Dependency Injection satellite library that makes it easy to add the service into the mod's service collection.

## Network Service

Provides a service for communicating between client, and server, when using Gantry MDK.

The network service acts as a one-stop solution for IPC between the client, and server.

The service comes with a fully-featured array of tools, such as:

 - default network channel, using the ModId as a channel name;
 - automatic registration of network channels, on use;
 - extension methods to aid the publishing of network packets;
 - un-registration of message handlers for generic types;
 - signal packet to act as a ping, between app-sides;

The service also comes with a Dependency Injection satellite library that makes it easy to add the service into the mod's service collection.

## **Support the Author:**

The Gantry Mod Development Kit is developed, and maintained by me; Apache. 
I'm a freelance developer, a content creator, and and a long time player, and modder, of Vintage Story.
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