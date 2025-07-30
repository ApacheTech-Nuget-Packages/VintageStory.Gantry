using System.IO;

namespace Gantry.Tools.TranslationsPatcher.Extensions;

/// <summary>
///     Extension methods for merging and patching translation dictionaries for a mod project.
/// </summary>
public static class TranslationDictionaryExtensions
{
    /// <summary>
    ///     Merges the root fallback translations (_Translations/en.json) into the merged dictionary.
    /// </summary>
    public static async Task MergeRootFallbackAsync(this Dictionary<string, string> merged, string projectDir)
    {
        Log.Verbose("Merging root fallback translations from {Path}", Path.Combine(projectDir, Constants.TranslationsFolder, $"{Constants.RequiredCulture}.json"));
        var rootEn = await LoadJsonFileAsync(Path.Combine(projectDir, Constants.TranslationsFolder, $"{Constants.RequiredCulture}.json"), required: true);
        foreach (var kvp in rootEn)
        {
            merged[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    ///     Merges the root culture translations (_Translations/{culture}.json) into the merged dictionary.
    /// </summary>
    public static async Task MergeRootCultureAsync(this Dictionary<string, string> merged, string projectDir, string culture)
    {
        if (!culture.Equals(Constants.RequiredCulture, StringComparison.OrdinalIgnoreCase))
        {
            Log.Verbose("Merging root culture translations from {Path}", Path.Combine(projectDir, Constants.TranslationsFolder, $"{culture}.json"));
            var rootCulture = await LoadJsonFileAsync(Path.Combine(projectDir, Constants.TranslationsFolder, $"{culture}.json"));
            foreach (var kvp in rootCulture)
            {
                merged[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    ///     Merges feature translations from the Features directory into the merged dictionary.
    /// </summary>
    public static async Task MergeFeatureTranslationsAsync(this Dictionary<string, string> merged, string projectDir, string culture)
    {
        // 1. For each Feature in the Features directory
        //      - If the feature has a translations folder, merge the translations from {culture}.json and en.json
        //      - Folder structure: $(ProjectDir)/Features/{FeatureName}/_Translations/*.json
        //     
        var featuresRoot = Path.Combine(projectDir, Constants.FeaturesSubfolder);
        if (Directory.Exists(featuresRoot))
        {
            var featureDirectories = Directory.GetDirectories(featuresRoot, Constants.TranslationsFolder, SearchOption.AllDirectories);
            Log.Verbose("Found {Count} feature directories with translations.", featureDirectories.Length);
            foreach (var featureDir in featureDirectories)
            {
                var parentDir = Path.GetDirectoryName(featureDir);
                var featureName = Path.GetFileName(parentDir!);
                Log.Verbose("Processing feature: {FeatureName}", featureName);
                await ProcessFeatureDirectory(merged, projectDir, culture, featureDir, featureName);
            }
        }

        // 2. For each Feature in the _Translations/Features directory
        //     - If the feature has a translations folder, merge the translations from {culture}.json and en.json
        //     - Folder structure: $(ProjectDir)/_Translations/Features/{FeatureName}/*.json
        var altFeaturesRoot = Path.Combine(projectDir, Constants.TranslationsFolder, Constants.FeaturesSubfolder);
        if (Directory.Exists(altFeaturesRoot))
        {
            var altFeatureDirectories = Directory.GetDirectories(altFeaturesRoot);
            Log.Verbose("Found {Count} alternative feature directories with translations.", altFeatureDirectories.Length);
            foreach (var featureDir in altFeatureDirectories)
            {
                var featureName = Path.GetFileName(featureDir)!;
                await ProcessFeatureDirectory(merged, projectDir, culture, featureDir, featureName);
            }
        }
    }

    private static async Task ProcessFeatureDirectory(Dictionary<string, string> merged, string projectDir, string culture, string featureDir, string featureName)
    {
        var featureDirRel = featureDir.Replace(projectDir, "\\");
        var featureEnPath = Path.Combine(featureDir, $"{Constants.RequiredCulture}.json");
        var featureCulturePath = Path.Combine(featureDir, $"{culture}.json");
        var fallbackExists = File.Exists(featureEnPath);
        var cultureExists = File.Exists(featureCulturePath);
        if (!fallbackExists && !cultureExists)
        {
            Log.Verbose("No translations found for feature: {FeatureName} ({FeatureDirectory})", featureName, featureDirRel);
            return;
        }
        else
        {
            Log.Verbose("Merging feature translations from {FeatureName} ({FeatureDirectory})", featureName, featureDirRel);
        }

        if (fallbackExists)
        {
            Log.Verbose("Merging feature fallback translations from {Path}", featureEnPath);
            var featureEn = await LoadJsonFileAsync(featureEnPath, required: true);
            foreach (var kvp in featureEn)
            {
                merged[kvp.Key] = kvp.Value;
            }
        }
        if (cultureExists)
        {
            Log.Verbose("Merging feature culture translations from {Path}", featureCulturePath);
            var featureCulture = await LoadJsonFileAsync(featureCulturePath);
            foreach (var kvp in featureCulture)
            {
                merged[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    ///     Applies the root patch (_Translations/Patches/{culture}.json) to the merged dictionary.
    /// </summary>
    public static async Task ApplyRootPatchAsync(this Dictionary<string, string> merged, string projectDir, string culture)
    {
        var rootPatchPath = Path.Combine(projectDir, Constants.TranslationsFolder, Constants.PatchSubfolder, $"{culture}.json");
        if (File.Exists(rootPatchPath))
        {
            Log.Verbose("Applying root patch from {Path}", rootPatchPath);
            var patch = await LoadJsonFileAsync(rootPatchPath);
            foreach (var kvp in patch)
            {
                merged[kvp.Key] = kvp.Value;
                Log.Information("Patched (root): {Key}", kvp.Key);
            }
        }
    }

    /// <summary>
    ///     Applies feature patches from the Features directory to the merged dictionary.
    /// </summary>
    public static async Task ApplyFeaturePatchesAsync(this Dictionary<string, string> merged, string projectDir, string culture)
    {
        string[] featureDirectories =
        [
            Path.Combine(projectDir, Constants.TranslationsFolder, Constants.FeaturesSubfolder),
            Path.Combine(projectDir, Constants.FeaturesSubfolder)
        ];

        foreach (var directory in featureDirectories.Where(Directory.Exists))
            foreach (var featureDir in Directory.EnumerateDirectories(directory))
            {
                var featureName = Path.GetFileName(featureDir)!;
                var patchPath = Path.Combine(featureDir, Constants.TranslationsFolder, Constants.PatchSubfolder, $"{culture}.json");
                if (File.Exists(patchPath))
                {
                    Log.Verbose("Applying feature patch from {Path}", patchPath);
                    var patch = await LoadJsonFileAsync(patchPath);
                    foreach (var kvp in patch)
                    {
                        merged[kvp.Key] = kvp.Value;
                        Log.Information("Patched (feature: {Feature}): {Key}", featureName, kvp.Key);
                    }
                }
            }
    }

    /// <summary>
    ///     Loads a translation file into a dictionary.
    /// </summary>
    private static async Task<Dictionary<string, string>> LoadJsonFileAsync(string path, bool required = false)
    {
        if (!File.Exists(path))
        {
            if (required)
            {
                throw new FileNotFoundException($"Required translation file not found: {path}");
            }
            else
            {
                Log.Warning("Optional file not found: {Path}", path);
            }
            return [];
        }
        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse JSON in file: {path}", ex);
        }
    }
}