namespace Gantry.Core.GameContent.GUI.Abstractions;

/// <summary>
///     Specifies the type of CRUD (Create, Read, Update, Delete) action being performed in a dialogue window.
/// </summary>
public enum CrudAction
{
    /// <summary>
    ///     Indicates that a new item is being added.
    ///     Use this action when creating a new entry.
    /// </summary>
    Add,

    /// <summary>
    ///     Indicates that an existing item is being edited.
    ///     Use this action when modifying an existing entry.
    /// </summary>
    Edit,

    /// <summary>
    ///     Indicates that an existing item is being deleted.
    ///     Use this action when removing an entry.
    /// </summary>
    Delete
}