using Gantry.Services.FileSystem.v2.DataStructures;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.Abstractions;

/// <summary>
///     A File Container, which holds references to Added files, and their <see cref="FileDescriptor"/> representations.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IFileCollection : IList<FileDescriptor>
{
}