using System.Reflection.Emit;
using System.Text;

namespace Gantry.Services.HarmonyPatches.Extensions;

/// <summary>
///    Provides extension methods for working with Harmony's <see cref="CodeInstruction"/> objects, including formatting and manipulation utilities for transpilers.
/// </summary>
public static class CodeInstructionExtensions
{
    /// <summary>
    ///     Converts a collection of <see cref="CodeInstruction"/> objects into a readable string representation with each
    /// entry prefixed by its index and returns the result in a <see cref="StringBuilder"/>.
    /// Useful for debugging transpiler output.
    /// </summary>
    /// <param name="instructions">The collection of <see cref="CodeInstruction"/> objects.</param>
    /// <returns>A <see cref="StringBuilder"/> containing the formatted instructions.</returns>
    public static StringBuilder ToStringBuilder(this IEnumerable<CodeInstruction> instructions)
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        var index = 0;

        foreach (var instruction in instructions)
        {
            var result = instruction.opcode.ToString();

            if (instruction.operand != null)
            {
                result += " " + instruction.operand switch
                {
                    MethodBase method => $"{method.DeclaringType?.FullName}::{method.Name}",
                    FieldInfo field => $"{field.DeclaringType?.FullName}::{field.Name}",
                    LocalBuilder local => $"Local_{local.LocalIndex} ({local.LocalType.Name})",
                    Label label => $"Label_{label.GetHashCode()}",
                    string str => $"\"{str}\"",
                    _ => instruction.operand.ToString()
                };
            }

            if (instruction.labels?.Count > 0)
            {
                var labels = string.Join(", ", instruction.labels.Select(l => $"Label_{l.GetHashCode()}"));
                result = $"[{labels}] {result}";
            }

            if (instruction.blocks?.Count > 0)
            {
                var blocks = string.Join(", ", instruction.blocks.Select(b => b.ToString()));
                result += $" // Blocks: {blocks}";
            }

            builder.AppendLine($"{index}: {result}");
            index++;
        }

        return builder;
    }

    /// <summary>
    ///     Finds and replaces operands of the specified type in a collection of <see cref="CodeInstruction"/> objects.
    ///     This is useful for modifying IL instructions in Harmony transpilers.
    /// </summary>
    /// <typeparam name="T">The type of the operand to find and replace.</typeparam>
    /// <param name="instructions">The collection of <see cref="CodeInstruction"/> objects to process.</param>
    /// <param name="from">The operand value to find.</param>
    /// <param name="to">The operand value to replace it with.</param>
    /// <returns>A collection of <see cref="CodeInstruction"/> objects with the specified operand replaced.</returns>
    public static IEnumerable<CodeInstruction> Replace<T>(this IEnumerable<CodeInstruction> instructions, T from, T to)
        where T : IEquatable<T>
    {
        foreach (var instruction in instructions)
        {
            if (instruction.operand is T operand && operand.Equals(from))
            {
                instruction.operand = to;
            }
            yield return instruction;
        }
    }

    /// <summary>
    ///     Logs the output of the transpiler to the debug file using the Gantry logger.
    ///     This is useful for debugging transpiler modifications.
    /// </summary>
    /// <param name="instructions">The collection of <see cref="CodeInstruction"/> objects to process.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    /// <returns>The original collection of <see cref="CodeInstruction"/> objects, unmodified.</returns>
    public static IEnumerable<CodeInstruction> LogOutput(this IEnumerable<CodeInstruction> instructions, ILogger logger)
    {
        logger.Debug(instructions.ToStringBuilder().ToString());
        return instructions;
    }
}