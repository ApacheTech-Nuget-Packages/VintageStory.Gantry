// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Scope = "module", 
    Justification = "Primary constructors should not be used as a first call.")]

[assembly: SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Scope = "module",
    Justification = "IDisposable pattern not properly implemented by the game.")]