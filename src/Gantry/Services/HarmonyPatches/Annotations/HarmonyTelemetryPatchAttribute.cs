namespace Gantry.Services.HarmonyPatches.Annotations;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
sealed class HarmonyTelemetryPatchAttribute : HarmonyAttribute;