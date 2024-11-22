using System.ComponentModel.Composition;
using Gantry.Services.MefLab.Abstractions;

namespace Gantry.Services.MefLab.Attributes;

/// <summary>
///     Designates the class as a composable MefLab contract.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MefLabContractAttribute() : ExportAttribute(IMefLabContract.ContractId, typeof(IMefLabContract));