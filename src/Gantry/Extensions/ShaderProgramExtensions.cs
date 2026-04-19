using OpenTK.Graphics.OpenGL;
using System.Globalization;

namespace Gantry.Extensions;

/// <summary>
///     Provides extension methods for ShaderPrograms.
/// </summary>
public static class ShaderProgramExtensions
{
    /// <summary>
    ///     Tries to get the current value of a uniform variable from a shader program. 
    ///     This is primarily for debugging purposes, as reading back uniform values can be slow 
    ///     and may not always succeed (e.g., if the uniform is optimised out or in a block).
    /// </summary>
    /// <param name="shaderProgram">The shader program to query.</param>
    /// <param name="uniformName">The name of the uniform variable.</param>
    /// <param name="value">The current value of the uniform variable, if available.</param>
    /// <returns>True if the value was successfully retrieved; otherwise, false.</returns>
    public static bool TryGetUniformValue(this ShaderProgram shaderProgram, string uniformName, out string value)
    {
        if (shaderProgram is null) { value = "Null shaderProgram"; return false; }
        if (shaderProgram.ProgramId == 0) { value = "Shader program not compiled (ProgramId == 0)"; return false; }

        int programId = shaderProgram.ProgramId;

        // Resolve location from name. arrays are sometimes represented as "name" vs "name[0]"
        int location = GL.GetUniformLocation(programId, uniformName);
        if (location < 0 && !uniformName.EndsWith("[0]", StringComparison.Ordinal))
        {
            location = GL.GetUniformLocation(programId, uniformName + "[0]");
        }
        if (location < 0)
        {
            // Location -1 means optimized-out / in a block / or not active — don't call GL.GetUniform
            value = $"Invalid uniform location ({location})";
            return false;
        }

        string normRequested = NormalizeUniformName(uniformName);

        try
        {
            GL.GetProgram(programId, GetProgramParameterName.ActiveUniforms, out int activeUniforms);

            for (int idx = 0; idx < activeUniforms; idx++)
            {
                GL.GetActiveUniform(programId, idx, 256, out int length, out int size, out ActiveUniformType type, out string name);
                var normName = NormalizeUniformName(name);

                if (normName != normRequested) continue;

                int count = Math.Max(1, size);
                int components = ComponentsForType(type);

                // Samplers / bool / int-like types
                if (IsSamplerOrIntLike(type))
                {
                    if (count > 1)
                    {
                        var iarr = new int[count];
                        GL.GetUniform(programId, location, iarr);
                        value = FormatIntArray(iarr);
                        return true;
                    }
                    GL.GetUniform(programId, location, out int ival);
                    value = ival.ToString(CultureInfo.InvariantCulture);
                    return true;
                }

                // Double scalar/arrays
                if (type == ActiveUniformType.Double)
                {
                    if (count > 1)
                    {
                        var darr = new double[count];
                        GL.GetUniform(programId, location, darr);
                        value = FormatDoubleArray(darr);
                        return true;
                    }
                    GL.GetUniform(programId, location, out double dval);
                    value = dval.ToString("G", CultureInfo.InvariantCulture);
                    return true;
                }

                // Float scalar / vectors / matrices
                if (components >= 1)
                {
                    int total = count * components;
                    if (total > 1)
                    {
                        var arr = new float[total];
                        GL.GetUniform(programId, location, arr);
                        value = FormatFloatArray(arr, components);
                        return true;
                    }
                    GL.GetUniform(programId, location, out float fval);
                    value = fval.ToString("G", CultureInfo.InvariantCulture);
                    return true;
                }

                value = $"Unsupported ActiveUniformType: {type}";
                return false;
            }

            value = "Uniform not found in active uniforms";
            return false;
        }
        catch (Exception ex)
        {
            value = $"Error reading uniform: {ex.GetType().Name}: {ex.Message}";
            return false;
        }

        static string NormalizeUniformName(string n) =>
            (n ?? string.Empty).EndsWith("[0]", StringComparison.Ordinal) ? n!.Substring(0, n!.Length - 3) : (n ?? string.Empty);

        static int ComponentsForType(ActiveUniformType t) =>
            t switch
            {
                ActiveUniformType.Float => 1,
                ActiveUniformType.FloatVec2 => 2,
                ActiveUniformType.FloatVec3 => 3,
                ActiveUniformType.FloatVec4 => 4,
                ActiveUniformType.FloatMat2 => 4,
                ActiveUniformType.FloatMat3 => 9,
                ActiveUniformType.FloatMat4 => 16,
                _ => 0
            };

        static bool IsSamplerOrIntLike(ActiveUniformType t) =>
            t switch
            {
                ActiveUniformType.Int or ActiveUniformType.Bool
                or ActiveUniformType.Sampler1D or ActiveUniformType.Sampler2D or ActiveUniformType.Sampler3D
                or ActiveUniformType.SamplerCube or ActiveUniformType.Sampler2DShadow or ActiveUniformType.Sampler2DArray => true,
                _ => false
            };

        static string FormatFloatArray(float[] arr, int components)
        {
            if (components <= 1)
            {
                return "[" + string.Join(", ", Array.ConvertAll(arr, f => f.ToString("G", CultureInfo.InvariantCulture))) + "]";
            }

            var groups = new List<string>(arr.Length / components);
            for (int i = 0; i < arr.Length; i += components)
            {
                var slice = new string[components];
                for (int j = 0; j < components; j++)
                {
                    slice[j] = arr[i + j].ToString("G", CultureInfo.InvariantCulture);
                }
                groups.Add("(" + string.Join(", ", slice) + ")");
            }
            return "[" + string.Join(", ", groups) + "]";
        }

        static string FormatIntArray(int[] arr) =>
            "[" + string.Join(", ", Array.ConvertAll(arr, i => i.ToString(CultureInfo.InvariantCulture))) + "]";

        static string FormatDoubleArray(double[] arr) =>
            "[" + string.Join(", ", Array.ConvertAll(arr, d => d.ToString("G", CultureInfo.InvariantCulture))) + "]";
    }
}