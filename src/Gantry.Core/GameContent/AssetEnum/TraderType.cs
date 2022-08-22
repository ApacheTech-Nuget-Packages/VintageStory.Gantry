using Gantry.Core.Abstractions;
using JetBrains.Annotations;

// ReSharper disable StringLiteralTypo

#pragma warning disable CS1591

namespace Gantry.Core.GameContent.AssetEnum
{
    /// <summary>
    ///     The different types of trader that are available in the vanilla game.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class TraderType : StringEnum<TraderType>
    {
        public static string Artisan { get; } = Create("artisan");
        public static string BuildingSupplies { get; } = Create("buildmaterials");
        public static string Clothing { get; } = Create("clothing");
        public static string Commodities { get; } = Create("commodities");
        public static string Foods { get; } = Create("foods");
        public static string Furniture { get; } = Create("furniture");
        public static string Luxuries { get; } = Create("luxuries");
        public static string SurvivalGoods { get; } = Create("survivalgoods");
        public static string TreasureHunter { get; } = Create("treasurehunter");
    }
}