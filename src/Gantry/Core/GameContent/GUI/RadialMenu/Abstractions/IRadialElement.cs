using Gantry.Core.Maths;

namespace Gantry.Core.GameContent.GUI.RadialMenu.Abstractions;

/// <summary>
///     Represents a radial menu element with capabilities for position updates, rendering, and interaction handling.
/// </summary>
/// <remarks>
///     This interface defines the required methods and properties for implementing a radial menu element,
///     including updating position and radius, handling hover and selection interactions, and rendering.
/// </remarks>
public interface IRadialElement : IDisposable
{
    /// <summary>
    ///     Updates the position of the radial element based on the specified parameters.
    /// </summary>
    /// <param name="index">The index of the element in the radial menu.</param>
    /// <param name="xOffset">The horizontal offset for the element's position.</param>
    /// <param name="yOffset">The vertical offset for the element's position.</param>
    /// <param name="angle">The angle of the element relative to the radial menu's centre.</param>
    /// <param name="elementAngle">The angle subtended by the element.</param>
    void UpdatePosition(int index, int xOffset, int yOffset, float angle, float elementAngle);

    /// <summary>
    ///     Updates the radius and thickness of the radial element.
    /// </summary>
    /// <param name="MidRadius">The middle radius of the radial menu.</param>
    /// <param name="Thickness">The thickness of the radial element.</param>
    void UpdateRadius(int MidRadius, int Thickness);

    /// <summary>
    ///     Redraws the radial element onto its texture.
    /// </summary>
    void ReDrawElementToTexture();

    /// <summary>
    ///     Renders the radial menu element on the screen.
    /// </summary>
    void RenderMenuElement();

    /// <summary>
    ///     Updates the middle position of the radial element.
    /// </summary>
    /// <param name="x">The horizontal position of the element's centre.</param>
    /// <param name="y">The vertical position of the element's centre.</param>
    void UpdateMiddlePosition(int x, int y);

    /// <summary>
    ///     Gets the unique identifier of the radial element.
    /// </summary>
    int Id { get; }

    /// <summary>
    ///     Gets the offset of the radial element relative to the menu's centre.
    /// </summary>
    /// <returns>A <see cref="FloatXY"/> structure representing the offset.</returns>
    FloatXY GetOffset();

    /// <summary>
    ///     Called when the cursor begins hovering over the radial element.
    /// </summary>
    void OnHoverBegin();

    /// <summary>
    ///     Called when the cursor stops hovering over the radial element.
    /// </summary>
    void OnHoverEnd();

    /// <summary>
    ///     Called when the radial element is selected.
    /// </summary>
    void OnSelect();
}