global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Threading;

global using ApacheTech.Common.BrighterSlim;
global using ApacheTech.Common.DependencyInjection.Abstractions;
global using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
global using ApacheTech.Common.Extensions.Harmony;
global using ApacheTech.Common.Extensions.System;

global using Gantry.Core.Abstractions;
global using Gantry.Core.Abstractions.ModSystems;
global using Gantry.Core.Annotation;
global using Gantry.Core.Hosting;
global using Gantry.Core.Hosting.Annotation;
global using Gantry.Core.Hosting.Registration;
global using Gantry.Core.Helpers;
global using Gantry.Core.Network.Extensions;

global using Gantry.Extensions;
global using Gantry.Extensions.Api;
global using Gantry.Extensions.DotNet;
global using Gantry.Extensions.Harmony;
global using Gantry.Extensions.Helpers;

global using Gantry.GameContent.AssetEnum;
global using Gantry.GameContent.ChatCommands;
global using Gantry.GameContent.ChatCommands.DataStructures;
global using Gantry.GameContent.ChatCommands.Parsers;
global using Gantry.GameContent.ChatCommands.Parsers.Extensions;
global using Gantry.GameContent.Extensions;
global using Gantry.GameContent.Extensions.Gui;
global using Gantry.GameContent.GUI;
global using Gantry.GameContent.GUI.Abstractions;
global using Gantry.GameContent.GUI.Helpers;
global using Gantry.GameContent.GUI.Models;

global using Gantry.Services.Brighter.Abstractions;
global using Gantry.Services.Brighter.Extensions;

global using Gantry.Services.EasyX.Abstractions;
global using Gantry.Services.EasyX.Extensions;

global using Gantry.Services.HarmonyPatches;
global using Gantry.Services.HarmonyPatches.Annotations;

global using Gantry.Services.IO.Abstractions.Contracts;
global using Gantry.Services.IO.Configuration;
global using Gantry.Services.IO.Configuration.Abstractions;
global using Gantry.Services.IO.Configuration.Consumers;
global using Gantry.Services.IO.DataStructures;
global using Gantry.Services.IO.Dialogue;
global using Gantry.Services.IO.Hosting;

global using HarmonyLib;
global using Newtonsoft.Json;
global using ProtoBuf;

global using Vintagestory.API.Client;
global using Vintagestory.API.Common;
global using Vintagestory.API.Config;
global using Vintagestory.API.MathTools;
global using Vintagestory.API.Server;
global using Vintagestory.Client.NoObf;
global using Vintagestory.GameContent;

global using AccessToolsEx = ApacheTech.Common.Extensions.Harmony.HarmonyReflectionExtensions;