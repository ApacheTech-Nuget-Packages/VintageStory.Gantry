using System.Reflection.Emit;

namespace Gantry.Extensions.Harmony;

/// <summary>
///     Provides extension methods for working with Harmony transpiler instructions.
/// </summary>
public static class CodeInstructionExtensions
{
    /// <summary>
    ///     Patches a sequence of Harmony <see cref="CodeInstruction" /> objects by replacing a specified range with the given patch.
    /// </summary>
    /// <param name="instructions">The original list of <see cref="CodeInstruction" /> objects to patch.</param>
    /// <param name="takeIndex">The zero-based index up to which instructions will be included from the start of the original list.</param>
    /// <param name="skipIndex">The zero-based index at which instructions will resume from the original list after the patch.</param>
    /// <param name="patch">The patch to insert between the specified indices.</param>
    /// <returns>A new <see cref="IEnumerable{T}" /> containing the patched sequence of instructions.</returns>
    /// <remarks>
    ///     This method creates a new sequence by taking instructions up to the specified <paramref name="takeIndex" />,
    ///     appending the <paramref name="patch" />, and skipping instructions starting at <paramref name="skipIndex" />.
    ///     It uses list slicing to achieve this efficiently.
    /// </remarks>
    public static IEnumerable<CodeInstruction> Inject(
        this List<CodeInstruction> instructions,
        int takeIndex,
        int skipIndex,
        IEnumerable<CodeInstruction> patch)
        => [.. instructions.Take(takeIndex + 1), .. patch, .. instructions.Skip(skipIndex)];

    /// <summary>
    ///     Finds the label in the instructions starting from the given index.
    /// </summary>
    /// <param name="instructions">The list of instructions to search through.</param>
    /// <param name="index">The index at which to start searching for the label.</param>
    /// <returns>The index of the instruction containing the label, or -1 if not found.</returns>
    public static int FindLabel(this List<CodeInstruction> instructions, int index)
    {
        var jumpLabel = instructions[index].operand.To<Label>();
        return instructions.FindIndex(index, p => p.labels.Contains(jumpLabel));
    }
}