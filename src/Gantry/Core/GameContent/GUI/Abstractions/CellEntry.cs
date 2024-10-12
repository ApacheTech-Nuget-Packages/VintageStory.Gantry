using Vintagestory.API.Client;

namespace Gantry.Core.GameContent.GUI.Abstractions;

/// <summary>
///     A cell, used to display a model of a specific type, within a scrollable list, in a GUI.
/// </summary>
/// <typeparam name="T">The type of model to display.</typeparam>
public class CellEntry<T> : SavegameCellEntry
{
    /// <summary>
    ///     The model to display.
    /// </summary>
    public T Model { get; set; }
}