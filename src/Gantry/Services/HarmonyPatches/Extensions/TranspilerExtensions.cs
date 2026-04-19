using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Gantry.Services.HarmonyPatches.Extensions;

/// <summary>
///     Provides extension methods for Harmony Transpilers, to make them easier to write and read.
/// </summary>
public static class TranspilerExtensions
{
    /// <summary>
    ///     Transforms a sequence of shader instructions by multiplying the value assigned to a specified uniform variable
    /// with the result of a provided lambda expression.
    /// </summary>
    /// <remarks>Only instructions that assign to the specified uniform variable are modified; all other
    /// instructions are returned unchanged. The lambda expression should be compatible with the type of the uniform
    /// variable.</remarks>
    /// <typeparam name="T">The type of the uniform variable to be multiplied.</typeparam>
    /// <param name="instructions">The sequence of shader instructions to process.</param>
    /// <param name="uniformName">The name of the uniform variable whose assignment will be modified.</param>
    /// <param name="fullyQualifiedMethodName">The fully qualified name for the method to call to get the current multiplier. FullTypeName:MethodName</param>
    /// <returns>An enumerable collection of shader instructions with the specified uniform assignment multiplied by the lambda
    /// expression result.</returns>
    public static IEnumerable<CodeInstruction> MultiplyUniform<T>(
        this IEnumerable<CodeInstruction> instructions, 
        string uniformName, 
        string fullyQualifiedMethodName
        )
    {
        for (var i = 0; i < instructions.Count(); i++)
        {
            var codeInstruction = instructions.ElementAt(i);
            if (!codeInstruction.Calls(AccessTools.Method(typeof(IShaderProgram), "Uniform", parameters: [typeof(string), typeof(T)])))
            {
                yield return codeInstruction;
                continue;
            }
            var currentUniformName = instructions.ElementAt(i - 2).operand as string;
            if (currentUniformName != uniformName)
            {
                yield return codeInstruction;
                continue;
            }

            yield return CodeInstruction.Call(fullyQualifiedMethodName);
            yield return new CodeInstruction(OpCodes.Mul);
            yield return codeInstruction;
        }
    }
}