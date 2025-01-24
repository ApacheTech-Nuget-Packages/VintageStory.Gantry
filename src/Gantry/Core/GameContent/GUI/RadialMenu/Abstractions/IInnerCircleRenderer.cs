namespace Gantry.Core.GameContent.GUI.RadialMenu.Abstractions;

/// <summary>
///     Defines the interface for an inner circle renderer used in a radial menu.
/// </summary>
public interface IInnerCircleRenderer : IDisposable
{
    /// <summary>
    ///     Gets or sets the radius of the inner circle.
    /// </summary>
    int Radius { get; set; }

    /// <summary>
    ///     Gets or sets the gape between the inner circle and the outer circle.
    /// </summary>
    int Gape { get; set; }

    /// <summary>
    ///     Rebuilds the inner circle, updating its appearance and structure.
    /// </summary>
    void Rebuild();

    /// <summary>
    ///     Renders the inner circle at the specified screen coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the centre of the circle.</param>
    /// <param name="y">The Y-coordinate of the centre of the circle.</param>
    void Render(int x, int y);
}