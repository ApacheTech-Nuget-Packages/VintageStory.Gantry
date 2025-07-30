using Gantry.Tools.ModInfoFileGenerator.Builders;
using Gantry.Tools.ModInfoFileGenerator.DataStructures;

namespace Gantry.Tools.ModInfoFileGenerator.Extensions;

/// <summary>
///     Provides extension methods for generating modinfo.json files from command line arguments.
/// </summary>
public static class ModInfoFileGenerator
{
    /// <summary>
    ///     Generates a modinfo.json file using the specified <see cref="CommandLineArgs"/>.
    ///     This method builds the mod info file by resolving the assembly, extracting mod details,
    ///     converting to a JSON object, validating the schema, and writing the output file.
    /// </summary>
    /// <param name="args">The command line arguments containing configuration for mod info generation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<ModDetails> GenerateModInfoFileAsync(this CommandLineArgs args)
    {
        var builder = await ModInfoFileBuilder
        .WithArguments(args)
        .ResolveAssemblyFile()
        .ExtractModDetails(out var modDetails)
        .ConvertToModInfoJsonObject()
        .ValidateSchemaAsync();

        await builder
            .WithOutputDirectory(args.TargetDir)
            .WriteModInfoFileAsync();

        return modDetails;
    }
}