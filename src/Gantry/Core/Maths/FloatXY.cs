namespace Gantry.Core.Maths;

/// <summary>
///     Represents a 2D vector with single-precision floating-point components.
/// </summary>
public readonly struct FloatXY
{
    private readonly float _x;
    private readonly float _y;

    /// <summary>
    ///     Initialises a new instance of the <see cref="FloatXY"/> struct with specified x and y values.
    /// </summary>
    /// <param name="x">The x-component of the vector.</param>
    /// <param name="y">The y-component of the vector.</param>
    public FloatXY(float x, float y) => (_x, _y) = (x, y);

    /// <summary>
    ///     Gets the x-component of the vector.
    /// </summary>
    public readonly float X => _x;

    /// <summary>
    ///     Gets the y-component of the vector.
    /// </summary>
    public readonly float Y => _y;

    /// <summary>
    ///     Gets the magnitude (length) of the vector.
    /// </summary>
    public readonly float Magnitude => MathF.Sqrt(_x * _x + _y * _y);

    /// <summary>
    ///     Gets the maximum value between the x and y components.
    /// </summary>
    public readonly float Max => MathF.Max(_x, _y);

    /// <summary>
    ///     Gets the minimum value between the x and y components.
    /// </summary>
    public readonly float Min => MathF.Min(_x, _y);

    /// <summary>
    ///     Computes the distance to another <see cref="FloatXY"/> instance.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The distance between the two vectors.</returns>
    public readonly float Distance(FloatXY other)
    {
        var dx = other._x - _x;
        var dy = other._y - _y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    /// <inheritdoc />
    public override readonly string ToString() => $"{{x: {_x}, y: {_y}}}";

    /// <summary>
    ///     Adds two <see cref="FloatXY"/> instances component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static FloatXY operator +(FloatXY a, FloatXY b) => new(a._x + b._x, a._y + b._y);

    /// <summary>
    ///     Subtracts the second <see cref="FloatXY"/> instance from the first component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static FloatXY operator -(FloatXY a, FloatXY b) => new(a._x - b._x, a._y - b._y);

    /// <summary>
    ///     Divides a <see cref="FloatXY"/> instance by a scalar.
    /// </summary>
    /// <param name="a">The vector.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The result of dividing the vector by the scalar.</returns>
    public static FloatXY operator /(FloatXY a, float scalar) => new(a._x / scalar, a._y / scalar);

    /// <summary>
    ///     Multiplies a <see cref="FloatXY"/> instance by a scalar.
    /// </summary>
    /// <param name="a">The vector.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The result of multiplying the vector by the scalar.</returns>
    public static FloatXY operator *(FloatXY a, float scalar) => new(a._x * scalar, a._y * scalar);

    /// <summary>
    ///     Explicitly converts a <see cref="FloatXY"/> instance to an <see cref="IntXY"/>.
    /// </summary>
    /// <param name="f">The vector to convert.</param>
    public static explicit operator IntXY(FloatXY f) => new((int)f._x, (int)f._y);

    /// <summary>
    ///     Represents a <see cref="FloatXY"/> with zero components.
    /// </summary>
    public static FloatXY Zero => new(0f, 0f);
}
