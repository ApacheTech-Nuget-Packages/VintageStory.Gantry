using Gantry.Core.GameContent.GUI.RadialMenu.Abstractions;
using Gantry.Core.GameContent.GUI.RadialMenu.Extensions;
using Gantry.Core.Maths;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.GUI.RadialMenu;

/// <summary>
///     Represents a radial menu with multiple interactive elements and support for mouse input.
/// </summary>
public class RadialMenu : IDisposable
{
    private static readonly float VectorLengthThreshold = 0.5f;
    private readonly List<IRadialElement> _elements = [];
    private readonly ICoreClientAPI _capi;
    private float _elementAngle;
    private int _middleScreenX;
    private int _middleScreenY;
    private readonly int _innerCircleRadius;
    private readonly int _outerCircleRadius;
    private float _vectorSensitivity;
    private FloatXY _mouseDirection = FloatXY.Zero;
    private int _lastSelectedElement = -1;
    private IInnerCircleRenderer _innerCircle;
    private bool _opened;
    private bool _disposed;

    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialMenu"/> class.
    /// </summary>
    /// <param name="capi">The core client API instance for managing the menu.</param>
    /// <param name="innerCircleRadius">The radius of the inner circle.</param>
    /// <param name="outerCircleRadius">The radius of the outer circle.</param>
    public RadialMenu(ICoreClientAPI capi, int innerCircleRadius, int outerCircleRadius)
    {
        _capi = capi;
        _innerCircleRadius = innerCircleRadius;
        _outerCircleRadius = outerCircleRadius;
        UpdateScreenMidPoint();
    }

    /// <summary>
    ///     Gets or sets the gape between the inner circle and the outer circle.
    /// </summary>
    public int Gape { get; set; } = 5;

    /// <summary>
    ///     Updates the screen's midpoint and adjusts the menu elements accordingly.
    /// </summary>
    protected virtual void UpdateScreenMidPoint()
    {
        var x = 0;
        var y = 0;
        _capi.GetScreenResolution(ref x, ref y);

        _vectorSensitivity = y / 9f;
        _middleScreenX = x / 2;
        _middleScreenY = y / 2;

        foreach (var element in _elements)
        {
            element.UpdateMiddlePosition(_middleScreenX, _middleScreenY);
        }
    }

    /// <summary>
    ///     Renders the radial menu and its elements.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last frame.</param>
    public virtual void OnRender(float deltaTime)
    {
        foreach (var element in _elements)
        {
            element.RenderMenuElement();
        }

        _innerCircle?.Render(_middleScreenX, _middleScreenY);
    }

    /// <summary>
    ///     Gets or sets the inner circle renderer for the menu.
    /// </summary>
    public IInnerCircleRenderer InnerRenderer
    {
        get => _innerCircle;
        set
        {
            _innerCircle?.Dispose();
            _innerCircle = value;

            if (_innerCircle != null)
            {
                _innerCircle.Radius = _innerCircleRadius;
                if (_innerCircle.Gape < 0)
                {
                    _innerCircle.Gape = Gape;
                }

                _innerCircle.Rebuild();
            }
        }
    }

    /// <summary>
    ///     Updates the mouse direction vector and selects the closest element.
    /// </summary>
    /// <param name="x">The horizontal mouse movement.</param>
    /// <param name="y">The vertical mouse movement.</param>
    public virtual void MouseDeltaMove(int x, int y)
    {
        _mouseDirection += new FloatXY(x, y);

        var magnitude = _mouseDirection.Magnitude;
        if (magnitude > _vectorSensitivity)
        {
            _mouseDirection = _mouseDirection / magnitude * _vectorSensitivity;
        }

        var closest = SimpleFindClosest(_mouseDirection);
        if (closest == null || closest.Id == _lastSelectedElement)
        {
            return;
        }

        if (_lastSelectedElement >= 0)
        {
            _elements[_lastSelectedElement].OnHoverEnd();
        }

        closest.OnHoverBegin();
        _lastSelectedElement = closest.Id;
    }

    /// <summary>
    ///     Finds the closest menu element to the given vector.
    /// </summary>
    /// <param name="val">The vector direction to compare against.</param>
    /// <returns>The closest radial element.</returns>
    protected IRadialElement SimpleFindClosest(FloatXY val)
    {
        IRadialElement closest = null;
        var minDistance = float.MaxValue;

        foreach (var element in _elements)
        {
            var offset = element.GetOffset();
            var magnitude = (offset / offset.Magnitude * _vectorSensitivity - val).Magnitude;

            if (magnitude < _vectorSensitivity * VectorLengthThreshold && magnitude <= minDistance)
            {
                closest = element;
                minDistance = magnitude;
            }
        }

        return closest;
    }

    /// <summary>
    ///     Adds a new element to the radial menu.
    /// </summary>
    /// <param name="element">The element to add.</param>
    /// <param name="rebuild">Indicates whether the menu should be rebuilt after adding the element.</param>
    /// <returns><c>true</c> if the element was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddElement(IRadialElement element, bool rebuild = false)
    {
        if (_opened)
        {
            return false;
        }

        var thickness = _outerCircleRadius - _innerCircleRadius;
        var midRadius = _innerCircleRadius + thickness / 2;

        element.UpdateRadius(midRadius, thickness);
        element.UpdateMiddlePosition(_middleScreenX, _middleScreenY);

        _elements.Add(element);

        if (rebuild)
        {
            Rebuild();
        }

        return true;
    }

    /// <summary>
    ///     Disposes of the radial menu, releasing any unmanaged resources.
    /// </summary>
    /// <remarks>
    ///     Ensures that the menu and all its elements are properly cleaned up, preventing memory leaks.
    /// </remarks>
    public void Dispose()
    {
        if (_opened)
        {
            Close(false);
        }

        foreach (var element in _elements)
        {
            (element as IDisposable)?.Dispose();
        }

        _innerCircle?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Removes an element from the radial menu by its ID.
    /// </summary>
    /// <param name="id">The ID of the element to remove.</param>
    /// <param name="rebuild">Indicates whether the menu should be rebuilt after removing the element.</param>
    /// <returns><c>true</c> if the element was removed successfully; otherwise, <c>false</c>.</returns>
    public bool RemoveElement(int id, bool rebuild = false)
    {
        if (id < 0 || id >= ElementsCount())
        {
            return false;
        }

        _elements.RemoveAt(id);

        if (rebuild)
        {
            Rebuild();
        }

        return true;
    }

    /// <summary>
    ///     Rebuilds the radial menu, updating element positions and visuals.
    /// </summary>
    /// <returns><c>true</c> if the menu was rebuilt successfully; otherwise, <c>false</c>.</returns>
    public virtual bool Rebuild()
    {
        if (_disposed)
        {
            return false;
        }

        _elementAngle = 2 * MathF.PI / _elements.Count;
        var midRadius = _innerCircleRadius + (_outerCircleRadius - _innerCircleRadius) / 2;

        for (var i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            if (element == null)
            {
                return false;
            }

            var angle = i * _elementAngle;
            var xOffset = (int)(midRadius * GameMath.Sin(angle));
            var yOffset = (int)(-midRadius * GameMath.Cos(angle));

            element.UpdatePosition(i, xOffset, yOffset, angle, _elementAngle);
            element.ReDrawElementToTexture();
        }

        _innerCircle?.Rebuild();
        return true;
    }

    /// <summary>
    ///     Gets the number of elements in the radial menu.
    /// </summary>
    /// <returns>The number of elements, or -1 if the element list is null.</returns>
    public int ElementsCount() => _elements?.Count ?? -1;

    /// <summary>
    ///     Gets a value indicating whether the menu is currently open.
    /// </summary>
    public bool Opened => _opened;

    /// <summary>
    ///     Opens the radial menu, updating its state and resetting element selection.
    /// </summary>
    public virtual void Open()
    {
        if (_disposed || _opened)
        {
            return;
        }

        UpdateScreenMidPoint();
        _lastSelectedElement = -1;
        _opened = true;
    }

    /// <summary>
    ///     Closes the radial menu and optionally selects the currently hovered element.
    /// </summary>
    /// <param name="select">Indicates whether to select the currently hovered element upon closing.</param>
    public virtual void Close(bool select = true)
    {
        _opened = false;

        if (_lastSelectedElement <= -1)
        {
            return;
        }

        var element = _elements[_lastSelectedElement];
        element.OnHoverEnd();

        if (select)
        {
            element.OnSelect();
        }
    }
}
