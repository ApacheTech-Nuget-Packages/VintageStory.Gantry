using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using ApacheTech.Common.Extensions.System;
using Gantry.Core.Abstractions;
using JetBrains.Annotations;

#pragma warning disable CS1591

namespace Gantry.Core.GameContent.AssetEnum
{
    /// <summary>
    ///     The colours that can be used when adding waypoints to the map.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class NamedColour : StringEnum<NamedColour>
    {
        //
        // A
        //
        public static string AliceBlue { get; } = Create(Color.AliceBlue.Name.ToLowerInvariant());
        public static string AntiqueWhite { get; } = Create(Color.AntiqueWhite.Name.ToLowerInvariant());
        public static string Aqua { get; } = Create(Color.Aqua.Name.ToLowerInvariant());
        public static string Aquamarine { get; } = Create(Color.Aquamarine.Name.ToLowerInvariant());
        public static string Azure { get; } = Create(Color.Azure.Name.ToLowerInvariant());

        //
        // B
        //
        public static string Beige { get; } = Create(Color.Beige.Name.ToLowerInvariant());
        public static string Bisque { get; } = Create(Color.Bisque.Name.ToLowerInvariant());
        public static string Black { get; } = Create(Color.Black.Name.ToLowerInvariant());
        public static string BlanchedAlmond { get; } = Create(Color.BlanchedAlmond.Name.ToLowerInvariant());
        public static string Blue { get; } = Create(Color.Blue.Name.ToLowerInvariant());
        public static string BlueViolet { get; } = Create(Color.BlueViolet.Name.ToLowerInvariant());
        public static string Brown { get; } = Create(Color.Brown.Name.ToLowerInvariant());
        public static string BurlyWood { get; } = Create(Color.BurlyWood.Name.ToLowerInvariant());

        //
        // C
        //
        public static string CadetBlue { get; } = Create(Color.CadetBlue.Name.ToLowerInvariant());
        public static string Chartreuse { get; } = Create(Color.Chartreuse.Name.ToLowerInvariant());
        public static string Chocolate { get; } = Create(Color.Chocolate.Name.ToLowerInvariant());
        public static string Coral { get; } = Create(Color.Coral.Name.ToLowerInvariant());
        public static string CornflowerBlue { get; } = Create(Color.CornflowerBlue.Name.ToLowerInvariant());
        public static string CornSilk { get; } = Create(Color.Cornsilk.Name.ToLowerInvariant());
        public static string Crimson { get; } = Create(Color.Crimson.Name.ToLowerInvariant());
        public static string Cyan { get; } = Create(Color.Cyan.Name.ToLowerInvariant());

        //
        // D
        //
        public static string DarkBlue { get; } = Create(Color.DarkBlue.Name.ToLowerInvariant());
        public static string DarkCyan { get; } = Create(Color.DarkCyan.Name.ToLowerInvariant());
        public static string DarkGoldenRod { get; } = Create(Color.DarkGoldenrod.Name.ToLowerInvariant());
        public static string DarkGrey { get; } = Create(Color.DarkGray.Name.ToLowerInvariant());
        public static string DarkGreen { get; } = Create(Color.DarkGreen.Name.ToLowerInvariant());
        public static string DarkKhaki { get; } = Create(Color.DarkKhaki.Name.ToLowerInvariant());
        public static string DarkMagenta { get; } = Create(Color.DarkMagenta.Name.ToLowerInvariant());
        public static string DarkOliveGreen { get; } = Create(Color.DarkOliveGreen.Name.ToLowerInvariant());
        public static string DarkOrange { get; } = Create(Color.DarkOrange.Name.ToLowerInvariant());
        public static string DarkOrchid { get; } = Create(Color.DarkOrchid.Name.ToLowerInvariant());
        public static string DarkRed { get; } = Create(Color.DarkRed.Name.ToLowerInvariant());
        public static string DarkSalmon { get; } = Create(Color.DarkSalmon.Name.ToLowerInvariant());
        public static string DarkSeaGreen { get; } = Create(Color.DarkSeaGreen.Name.ToLowerInvariant());
        public static string DarkSlateBlue { get; } = Create(Color.DarkSlateBlue.Name.ToLowerInvariant());
        public static string DarkSlateGrey { get; } = Create(Color.DarkSlateGray.Name.ToLowerInvariant());
        public static string DarkTurquoise { get; } = Create(Color.DarkTurquoise.Name.ToLowerInvariant());
        public static string DarkViolet { get; } = Create(Color.DarkViolet.Name.ToLowerInvariant());
        public static string DeepPink { get; } = Create(Color.DeepPink.Name.ToLowerInvariant());
        public static string DeepSkyBlue { get; } = Create(Color.DeepSkyBlue.Name.ToLowerInvariant());
        public static string DimGrey { get; } = Create(Color.DimGray.Name.ToLowerInvariant());
        public static string DodgerBlue { get; } = Create(Color.DodgerBlue.Name.ToLowerInvariant());

        //
        // F
        //
        public static string Firebrick { get; } = Create(Color.Firebrick.Name.ToLowerInvariant());
        public static string FloralWhite { get; } = Create(Color.FloralWhite.Name.ToLowerInvariant());
        public static string ForestGreen { get; } = Create(Color.ForestGreen.Name.ToLowerInvariant());
        public static string Fuchsia { get; } = Create(Color.Fuchsia.Name.ToLowerInvariant());

        //
        // G
        //
        public static string Gainsborough { get; } = Create(Color.Gainsboro.Name.ToLowerInvariant());
        public static string GhostWhite { get; } = Create(Color.GhostWhite.Name.ToLowerInvariant());
        public static string Gold { get; } = Create(Color.Gold.Name.ToLowerInvariant());
        public static string GoldenRod { get; } = Create(Color.Goldenrod.Name.ToLowerInvariant());
        public static string Grey { get; } = Create(Color.Gray.Name.ToLowerInvariant());
        public static string Green { get; } = Create(Color.Green.Name.ToLowerInvariant());
        public static string GreenYellow { get; } = Create(Color.GreenYellow.Name.ToLowerInvariant());

        //
        // H
        //
        public static string Honeydew { get; } = Create(Color.Honeydew.Name.ToLowerInvariant());
        public static string HotPink { get; } = Create(Color.HotPink.Name.ToLowerInvariant());

        //
        // I
        //
        public static string IndianRed { get; } = Create(Color.IndianRed.Name.ToLowerInvariant());
        public static string Indigo { get; } = Create(Color.Indigo.Name.ToLowerInvariant());
        public static string Ivory { get; } = Create(Color.Ivory.Name.ToLowerInvariant());

        //
        // K
        //
        public static string Khaki { get; } = Create(Color.Khaki.Name.ToLowerInvariant());

        //
        // L
        //
        public static string Lavender { get; } = Create(Color.Lavender.Name.ToLowerInvariant());
        public static string LavenderBlush { get; } = Create(Color.LavenderBlush.Name.ToLowerInvariant());
        public static string LawnGreen { get; } = Create(Color.LawnGreen.Name.ToLowerInvariant());
        public static string LemonChiffon { get; } = Create(Color.LemonChiffon.Name.ToLowerInvariant());
        public static string LightBlue { get; } = Create(Color.LightBlue.Name.ToLowerInvariant());
        public static string LightCoral { get; } = Create(Color.LightCoral.Name.ToLowerInvariant());
        public static string LightCyan { get; } = Create(Color.LightCyan.Name.ToLowerInvariant());
        public static string LightGoldenRodYellow { get; } = Create(Color.LightGoldenrodYellow.Name.ToLowerInvariant());
        public static string LightGrey { get; } = Create(Color.LightGray.Name.ToLowerInvariant());
        public static string LightGreen { get; } = Create(Color.LightGreen.Name.ToLowerInvariant());
        public static string LightPink { get; } = Create(Color.LightPink.Name.ToLowerInvariant());
        public static string LightSalmon { get; } = Create(Color.LightSalmon.Name.ToLowerInvariant());
        public static string LightSeaGreen { get; } = Create(Color.LightSeaGreen.Name.ToLowerInvariant());
        public static string LightSkyBlue { get; } = Create(Color.LightSkyBlue.Name.ToLowerInvariant());
        public static string LightSlateGray { get; } = Create(Color.LightSlateGray.Name.ToLowerInvariant());
        public static string LightSteelBlue { get; } = Create(Color.LightSteelBlue.Name.ToLowerInvariant());
        public static string LightYellow { get; } = Create(Color.LightYellow.Name.ToLowerInvariant());
        public static string Lime { get; } = Create(Color.Lime.Name.ToLowerInvariant());
        public static string LimeGreen { get; } = Create(Color.LimeGreen.Name.ToLowerInvariant());
        public static string Linen { get; } = Create(Color.Linen.Name.ToLowerInvariant());

        //
        // M
        //
        public static string Magenta { get; } = Create(Color.Magenta.Name.ToLowerInvariant());
        public static string Maroon { get; } = Create(Color.Maroon.Name.ToLowerInvariant());
        public static string MediumAquamarine { get; } = Create(Color.MediumAquamarine.Name.ToLowerInvariant());
        public static string MediumBlue { get; } = Create(Color.MediumBlue.Name.ToLowerInvariant());
        public static string MediumOrchid { get; } = Create(Color.MediumOrchid.Name.ToLowerInvariant());
        public static string MediumPurple { get; } = Create(Color.MediumPurple.Name.ToLowerInvariant());
        public static string MediumSeaGreen { get; } = Create(Color.MediumSeaGreen.Name.ToLowerInvariant());
        public static string MediumSlateBlue { get; } = Create(Color.MediumSlateBlue.Name.ToLowerInvariant());
        public static string MediumSpringGreen { get; } = Create(Color.MediumSpringGreen.Name.ToLowerInvariant());
        public static string MediumTurquoise { get; } = Create(Color.MediumTurquoise.Name.ToLowerInvariant());
        public static string MediumVioletRed { get; } = Create(Color.MediumVioletRed.Name.ToLowerInvariant());
        public static string MidnightBlue { get; } = Create(Color.MidnightBlue.Name.ToLowerInvariant());
        public static string MintCream { get; } = Create(Color.MintCream.Name.ToLowerInvariant());
        public static string MistyRose { get; } = Create(Color.MistyRose.Name.ToLowerInvariant());
        public static string Moccasin { get; } = Create(Color.Moccasin.Name.ToLowerInvariant());

        //
        // N
        //
        public static string NavajoWhite { get; } = Create(Color.NavajoWhite.Name.ToLowerInvariant());
        public static string Navy { get; } = Create(Color.Navy.Name.ToLowerInvariant());

        //
        // O
        //
        public static string OldLace { get; } = Create(Color.OldLace.Name.ToLowerInvariant());
        public static string Olive { get; } = Create(Color.Olive.Name.ToLowerInvariant());
        public static string OliveDrab { get; } = Create(Color.OliveDrab.Name.ToLowerInvariant());
        public static string Orange { get; } = Create(Color.Orange.Name.ToLowerInvariant());
        public static string OrangeRed { get; } = Create(Color.OrangeRed.Name.ToLowerInvariant());
        public static string Orchid { get; } = Create(Color.Orchid.Name.ToLowerInvariant());

        //
        // P
        //
        public static string PaleGoldenRod { get; } = Create(Color.PaleGoldenrod.Name.ToLowerInvariant());
        public static string PaleGreen { get; } = Create(Color.PaleGreen.Name.ToLowerInvariant());
        public static string PaleVioletRed { get; } = Create(Color.PaleVioletRed.Name.ToLowerInvariant());
        public static string PapayaWhip { get; } = Create(Color.PapayaWhip.Name.ToLowerInvariant());
        public static string PeachPuff { get; } = Create(Color.PeachPuff.Name.ToLowerInvariant());
        public static string Peru { get; } = Create(Color.Peru.Name.ToLowerInvariant());
        public static string Pink { get; } = Create(Color.Pink.Name.ToLowerInvariant());
        public static string Plum { get; } = Create(Color.Plum.Name.ToLowerInvariant());
        public static string PowderBlue { get; } = Create(Color.PowderBlue.Name.ToLowerInvariant());
        public static string Purple { get; } = Create(Color.Purple.Name.ToLowerInvariant());

        //
        // R
        //
        public static string Red { get; } = Create(Color.Red.Name.ToLowerInvariant());
        public static string RosyBrown { get; } = Create(Color.RosyBrown.Name.ToLowerInvariant());
        public static string RoyalBlue { get; } = Create(Color.RoyalBlue.Name.ToLowerInvariant());

        //
        // S
        //
        public static string SaddleBrown { get; } = Create(Color.SaddleBrown.Name.ToLowerInvariant());
        public static string Salmon { get; } = Create(Color.Salmon.Name.ToLowerInvariant());
        public static string SandyBrown { get; } = Create(Color.SandyBrown.Name.ToLowerInvariant());
        public static string SeaGreen { get; } = Create(Color.SeaGreen.Name.ToLowerInvariant());
        public static string SeaShell { get; } = Create(Color.SeaShell.Name.ToLowerInvariant());
        public static string Silver { get; } = Create(Color.Silver.Name.ToLowerInvariant());
        public static string Sienna { get; } = Create(Color.Sienna.Name.ToLowerInvariant());
        public static string SkyBlue { get; } = Create(Color.SkyBlue.Name.ToLowerInvariant());
        public static string SlateBlue { get; } = Create(Color.SlateBlue.Name.ToLowerInvariant());
        public static string SlateGrey { get; } = Create(Color.SlateGray.Name.ToLowerInvariant());
        public static string Snow { get; } = Create(Color.Snow.Name.ToLowerInvariant());
        public static string SpringGreen { get; } = Create(Color.SpringGreen.Name.ToLowerInvariant());
        public static string SteelBlue { get; } = Create(Color.SteelBlue.Name.ToLowerInvariant());

        //
        // T
        //
        public static string Tan { get; } = Create(Color.Tan.Name.ToLowerInvariant());
        public static string Teal { get; } = Create(Color.Teal.Name.ToLowerInvariant());
        public static string Thistle { get; } = Create(Color.Thistle.Name.ToLowerInvariant());
        public static string Tomato { get; } = Create(Color.Tomato.Name.ToLowerInvariant());
        public static string Transparent { get; } = Create(Color.Transparent.Name.ToLowerInvariant());
        public static string Turquoise { get; } = Create(Color.Turquoise.Name.ToLowerInvariant());

        //
        // V
        //
        public static string Violet { get; } = Create(Color.Violet.Name.ToLowerInvariant());

        //
        // W
        //
        public static string Wheat { get; } = Create(Color.Wheat.Name.ToLowerInvariant());
        public static string White { get; } = Create(Color.White.Name.ToLowerInvariant());
        public static string WhiteSmoke { get; } = Create(Color.WhiteSmoke.Name.ToLowerInvariant());

        //
        // Y
        //
        public static string Yellow { get; } = Create(Color.Yellow.Name.ToLowerInvariant());
        public static string YellowGreen { get; } = Create(Color.YellowGreen.Name.ToLowerInvariant());
        
        public static string[] ValuesList()
        {
            var list = AllColours.Select(p => p.GetValue(null).ToString()).ToArray();
            return list;
        }

        public static string[] NamesList()
        {
            var list = AllColours.Select(p => p.Name.SplitPascalCase()).ToArray();
            return list;
        }

        public static Dictionary<string, string> ColoursByName()
        {
            var dict = new Dictionary<string, string>();
            var keys = ValuesList();
            var values = NamesList();
            for (var i = 0; i < ValuesList().Length; i++)
            {
                dict.Add(keys[i], values[i]);
            }
            return dict;
        }

        public static bool Validate(string colour)
        {
            return NamesList().Contains(colour.ToLowerInvariant());
        }

        public static string GetFullName(string colour)
        {
            return !Validate(colour) 
                ? string.Empty 
                : ColoursByName()[colour];
        }

        public static string FromArgb(int argb)
        {
            return ValuesList()
                .FirstOrDefault(p => Color.FromName(p).ToArgb() == argb) ?? Black;
        }

        static NamedColour()
        {
            AllColours = typeof(NamedColour)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string));
        }

        private static IEnumerable<PropertyInfo> AllColours { get; }
    }
}
