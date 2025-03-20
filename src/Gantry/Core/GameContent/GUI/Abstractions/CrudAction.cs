namespace Gantry.Core.GameContent.GUI.Abstractions;

/// <summary>
///     Specifies whether a dialogue is for added or edited an item.
/// </summary>
public enum CrudAction
{
    /// <summary>
    ///     Indicates that a new item is being added.
    /// </summary>
    Add,

    /// <summary>
    ///     Indicates that an existing item is being edited.
    /// </summary>
    Edit,

    /// <summary>
    ///     Indicates that an existing item is being deleted.
    /// </summary>
    Delete
}