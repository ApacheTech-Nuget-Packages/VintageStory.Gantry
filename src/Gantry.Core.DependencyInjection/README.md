# Gantry - Vintage Story MDK: Dependency Injection Library

Adds an IoC container for the game, using`ApacheTech.Common.DependencyInjection` as the injection engine. It manages the dependencies between classes, so that mods can stay easy to change, as they grow in size and complexity.

This library includes a bespoke dependency injection engine, which can be used to produce very flexible, scalable, and highly optimised mods. This allows large mods to be handled very easily, especially when being used within a vertical slice architecture, where each feature of your mod is separate from eachother, within their own folders. 

Comes with a fully-featured array of tools, such as:

 - easy integration into mods, via the `ModHost` base class;
 - static access to the mod's service provider, via the `IOC` static class;
 - interfaces that allow existing classes to register services at startup;
 - sided constructor attributes that allow classes to be instantiated differently on the client, and server;
 - automatic addition of the core game API, into the service collection;
 - automatic addition of key game features, into the service collection;

## Gantry MDK:

The Gantry Mod Development Kit is a Core Framework that makes it easier to make Code Mods for Vintage Story.

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
 - **YouTube:** *https://youtube.com/ApacheGamingUK*
 - **Subscribe on Twitch.TV:** *https://twitch.tv/ApacheGamingUK*
 - **Donate on StreamLabs:** *https://www.streamlabs.com/ApacheGamingUK*

### **Charitable Causes**

All donations go towards making new content on a non-profit basis. Software purchases and subscriptions, hardware upgrades, training, etc. 
Any profit made will be donated to [Macmillan Cancer Support](https://www.macmillan.org.uk/), and [The British Liver Trust](https://britishlivertrust.org.uk/), in memory of my father, who died of Liver Cancer in 2015.
