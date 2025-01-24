namespace Gantry.Core.Maths;

/// <summary>
///     Represents a 2D vector with integer components.
/// </summary>
public readonly struct IntXY
{
    private readonly int _x;
    private readonly int _y;

    /// <summary>
    ///     Initialises a new instance of the <see cref="IntXY"/> struct with specified x and y values.
    /// </summary>
    /// <param name="x">The x-component of the vector.</param>
    /// <param name="y">The y-component of the vector.</param>
    public IntXY(int x, int y)
    {
        _x = x;
        _y = y;
    }

    /// <summary>
    ///     Gets the x-component of the vector.
    /// </summary>
    public readonly int X => _x;

    /// <summary>
    ///     Gets the y-component of the vector.
    /// </summary>
    public readonly int Y => _y;

    /// <summary>
    ///     Gets the magnitude (length) of the vector as a floating-point value.
    /// </summary>
    public readonly float Magnitude => MathF.Sqrt(_x * _x + _y * _y);

    /// <summary>
    ///     Gets the maximum value between the x and y components.
    /// </summary>
    public readonly int Max => Math.Max(_x, _y);

    /// <summary>
    ///     Gets the minimum value between the x and y components.
    /// </summary>
    public readonly int Min => Math.Min(_x, _y);

    /// <summary>
    ///     Computes the distance to another <see cref="IntXY"/> instance.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The distance between the two vectors as a floating-point value.</returns>
    public readonly float Distance(IntXY other)
    {
        var dx = other._x - _x;
        var dy = other._y - _y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    /// <inheritdoc />
    public override readonly string ToString() => $"{{x: {_x}, y: {_y}}}";

    /// <summary>
    ///     Adds two <see cref="IntXY"/> instances component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static IntXY operator +(IntXY a, IntXY b) => new(a._x + b._x, a._y + b._y);

    /// <summary>
    ///     Subtracts the second <see cref="IntXY"/> instance from the first component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static IntXY operator -(IntXY a, IntXY b) => new(a._x - b._x, a._y - b._y);

    /// <summary>
    ///     Divides a <see cref="IntXY"/> instance by a scalar.
    /// </summary>
    /// <param name="a">The vector.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The result of dividing the vector by the scalar.</returns>
    public static IntXY operator /(IntXY a, int scalar) => new(a._x / scalar, a._y / scalar);

    /// <summary>
    ///     Multiplies a <see cref="IntXY"/> instance by a scalar.
    /// </summary>
    /// <param name="a">The vector.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The result of multiplying the vector by the scalar.</returns>
    public static IntXY operator *(IntXY a, int scalar) => new(a._x * scalar, a._y * scalar);

    /// <summary>
    ///     Represents a <see cref="IntXY"/> with zero components.
    /// </summary>
    public static IntXY Zero => new(0, 0);
}
