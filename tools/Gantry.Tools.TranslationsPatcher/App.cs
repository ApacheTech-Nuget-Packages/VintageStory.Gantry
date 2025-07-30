namespace Gantry.Tools.TranslationsPatcher;

/// <summary>
///     Provides translation merging and patching logic for a mod project.
/// </summary>
public static class App
{
    /// <summary>
    ///     JSON serialiser options for output files.
    /// </summary>
    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    /// <summary>
    ///     Entry point for merging translations and applying patches using parsed command line options.
    /// </summary>
    /// <param name="opts">The parsed command line options.</param>
    public static async Task PatchTranslationFilesAsync(this CommandLineOptions opts)
    {
        var projectDir = opts.ProjectDir;
        var targetDir = opts.TargetDir;
        var modId = opts.ModId;

        var sourceDir = Path.Combine(projectDir, Constants.TranslationsFolder);
        if (!Directory.Exists(sourceDir))
        {
            Log.Error("Source translations folder not found: {SourceDir}", sourceDir);
            return;
        }

        var cultures = Directory
            .EnumerateFiles(sourceDir, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!cultures.Contains(Constants.RequiredCulture, StringComparer.OrdinalIgnoreCase))
        {
            Log.Error("Required fallback language file '{0}.json' not found in root _Translations.", Constants.RequiredCulture);
            return;
        }

        foreach (var culture in cultures)
        {
            try
            {
                Log.Verbose("Merging translations for culture '{Culture}'", culture);
                if (string.IsNullOrEmpty(culture)) continue;
                var result = await BuildCultureDictionaryAsync(projectDir, culture);
                if (result.Count == 0)
                {
                    Log.Warning("No entries for culture '{Culture}' after merge.", culture);
                    continue;
                }
                await WriteMergedTranslationAsync(targetDir, modId, culture, result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to merge translations for culture '{Culture}'", culture);
            }
        }
    }

    /// <summary>
    ///     Writes the merged translation dictionary to the target directory as a JSON file.
    /// </summary>
    /// <param name="targetDir">The target directory for the mod.</param>
    /// <param name="modId">The mod identifier.</param>
    /// <param name="culture">The culture identifier (e.g., 'en', 'es').</param>
    /// <param name="result">The merged translation dictionary.</param>
    private static async Task WriteMergedTranslationAsync(string targetDir, string modId, string culture, Dictionary<string, string> result)
    {
        var outputPath = Path.Combine(targetDir, "_Includes", "assets", modId, "lang", $"{culture}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        var orderedDict = result.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var json = JsonSerializer.Serialize(orderedDict, JsonOptions);
        await File.WriteAllTextAsync(outputPath, json);
        Log.Information("Written: {OutputPath}", outputPath);
    }

    /// <summary>
    ///     Builds the merged translation dictionary for a specific culture, applying all patches and feature translations.
    /// </summary>
    /// <param name="projectDir">The root directory of the mod project.</param>
    /// <param name="culture">The target culture (e.g., 'en', 'es').</param>
    /// <returns>The merged translation dictionary for the specified culture.</returns>
    private static async Task<Dictionary<string, string>> BuildCultureDictionaryAsync(string projectDir, string culture)
    {
        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        await merged.MergeRootFallbackAsync(projectDir);
        await merged.MergeRootCultureAsync(projectDir, culture);
        await merged.MergeFeatureTranslationsAsync(projectDir, culture);
        await merged.ApplyRootPatchAsync(projectDir, culture);
        await merged.ApplyFeaturePatchesAsync(projectDir, culture);
        return merged;
    }
}