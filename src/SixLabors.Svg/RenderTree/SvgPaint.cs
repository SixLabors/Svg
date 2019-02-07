using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace SixLabors.Svg.Dom
{
    internal struct SvgPaint
    {
        public static readonly SvgPaint Unset = default(SvgPaint);

        internal SvgPaint(string value, float opacity)
        {
            Value = value;
            Opacity = opacity;
        }

        public string Value { get; }

        public float Opacity { get; }

        public static SvgPaint Parse(string val, string opacity)
        {
            val = val?.Trim() ?? "";
            opacity = opacity?.Trim() ?? "";

            float opacityVal = 1;
            if (float.TryParse(opacity, out float op))
            {
                opacityVal = op;
                if (opacityVal < 0)
                {
                    opacityVal = 0;
                }
                if (opacityVal > 1)
                {
                    opacityVal = 1;
                }
            }

            return new SvgPaint(val, opacityVal);

        }

        private TPixel? GetNamedColor<TPixel>(string name) where TPixel : struct, IPixel<TPixel>
        {
            var dict = new Dictionary<string, TPixel>(StringComparer.OrdinalIgnoreCase)
            {
                ["AliceBlue"] = NamedColors<TPixel>.AliceBlue,
                ["MistyRose"] = NamedColors<TPixel>.MistyRose,
                ["Moccasin"] = NamedColors<TPixel>.Moccasin,
                ["NavajoWhite"] = NamedColors<TPixel>.NavajoWhite,
                ["Navy"] = NamedColors<TPixel>.Navy,
                ["OldLace"] = NamedColors<TPixel>.OldLace,
                ["Olive"] = NamedColors<TPixel>.Olive,
                ["MintCream"] = NamedColors<TPixel>.MintCream,
                ["OliveDrab"] = NamedColors<TPixel>.OliveDrab,
                ["OrangeRed"] = NamedColors<TPixel>.OrangeRed,
                ["Orchid"] = NamedColors<TPixel>.Orchid,
                ["PaleGoldenrod"] = NamedColors<TPixel>.PaleGoldenrod,
                ["PaleGreen"] = NamedColors<TPixel>.PaleGreen,
                ["PaleTurquoise"] = NamedColors<TPixel>.PaleTurquoise,
                ["PaleVioletRed"] = NamedColors<TPixel>.PaleVioletRed,
                ["Orange"] = NamedColors<TPixel>.Orange,
                ["PapayaWhip"] = NamedColors<TPixel>.PapayaWhip,
                ["MidnightBlue"] = NamedColors<TPixel>.MidnightBlue,
                ["MediumTurquoise"] = NamedColors<TPixel>.MediumTurquoise,
                ["LightSteelBlue"] = NamedColors<TPixel>.LightSteelBlue,
                ["LightYellow"] = NamedColors<TPixel>.LightYellow,
                ["Lime"] = NamedColors<TPixel>.Lime,
                ["LimeGreen"] = NamedColors<TPixel>.LimeGreen,
                ["Linen"] = NamedColors<TPixel>.Linen,
                ["Magenta"] = NamedColors<TPixel>.Magenta,
                ["MediumVioletRed"] = NamedColors<TPixel>.MediumVioletRed,
                ["Maroon"] = NamedColors<TPixel>.Maroon,
                ["MediumBlue"] = NamedColors<TPixel>.MediumBlue,
                ["MediumOrchid"] = NamedColors<TPixel>.MediumOrchid,
                ["MediumPurple"] = NamedColors<TPixel>.MediumPurple,
                ["MediumSeaGreen"] = NamedColors<TPixel>.MediumSeaGreen,
                ["MediumSlateBlue"] = NamedColors<TPixel>.MediumSlateBlue,
                ["MediumSpringGreen"] = NamedColors<TPixel>.MediumSpringGreen,
                ["MediumAquamarine"] = NamedColors<TPixel>.MediumAquamarine,
                ["LightSlateGray"] = NamedColors<TPixel>.LightSlateGray,
                ["PeachPuff"] = NamedColors<TPixel>.PeachPuff,
                ["Pink"] = NamedColors<TPixel>.Pink,
                ["SpringGreen"] = NamedColors<TPixel>.SpringGreen,
                ["SteelBlue"] = NamedColors<TPixel>.SteelBlue,
                ["Tan"] = NamedColors<TPixel>.Tan,
                ["Teal"] = NamedColors<TPixel>.Teal,
                ["Thistle"] = NamedColors<TPixel>.Thistle,
                ["Tomato"] = NamedColors<TPixel>.Tomato,
                ["Snow"] = NamedColors<TPixel>.Snow,
                ["Transparent"] = NamedColors<TPixel>.Transparent,
                ["Violet"] = NamedColors<TPixel>.Violet,
                ["Wheat"] = NamedColors<TPixel>.Wheat,
                ["White"] = NamedColors<TPixel>.White,
                ["WhiteSmoke"] = NamedColors<TPixel>.WhiteSmoke,
                ["Yellow"] = NamedColors<TPixel>.Yellow,
                ["YellowGreen"] = NamedColors<TPixel>.YellowGreen,
                ["Turquoise"] = NamedColors<TPixel>.Turquoise,
                ["Peru"] = NamedColors<TPixel>.Peru,
                ["SlateGray"] = NamedColors<TPixel>.SlateGray,
                ["SkyBlue"] = NamedColors<TPixel>.SkyBlue,
                ["Plum"] = NamedColors<TPixel>.Plum,
                ["PowderBlue"] = NamedColors<TPixel>.PowderBlue,
                ["Purple"] = NamedColors<TPixel>.Purple,
                ["RebeccaPurple"] = NamedColors<TPixel>.RebeccaPurple,
                ["Red"] = NamedColors<TPixel>.Red,
                ["RosyBrown"] = NamedColors<TPixel>.RosyBrown,
                ["SlateBlue"] = NamedColors<TPixel>.SlateBlue,
                ["RoyalBlue"] = NamedColors<TPixel>.RoyalBlue,
                ["Salmon"] = NamedColors<TPixel>.Salmon,
                ["SandyBrown"] = NamedColors<TPixel>.SandyBrown,
                ["SeaGreen"] = NamedColors<TPixel>.SeaGreen,
                ["SeaShell"] = NamedColors<TPixel>.SeaShell,
                ["Sienna"] = NamedColors<TPixel>.Sienna,
                ["Silver"] = NamedColors<TPixel>.Silver,
                ["SaddleBrown"] = NamedColors<TPixel>.SaddleBrown,
                ["LightSkyBlue"] = NamedColors<TPixel>.LightSkyBlue,
                ["LightSeaGreen"] = NamedColors<TPixel>.LightSeaGreen,
                ["LightSalmon"] = NamedColors<TPixel>.LightSalmon,
                ["Crimson"] = NamedColors<TPixel>.Crimson,
                ["Cyan"] = NamedColors<TPixel>.Cyan,
                ["DarkBlue"] = NamedColors<TPixel>.DarkBlue,
                ["DarkCyan"] = NamedColors<TPixel>.DarkCyan,
                ["DarkGoldenrod"] = NamedColors<TPixel>.DarkGoldenrod,
                ["DarkGray"] = NamedColors<TPixel>.DarkGray,
                ["Cornsilk"] = NamedColors<TPixel>.Cornsilk,
                ["DarkGreen"] = NamedColors<TPixel>.DarkGreen,
                ["DarkMagenta"] = NamedColors<TPixel>.DarkMagenta,
                ["DarkOliveGreen"] = NamedColors<TPixel>.DarkOliveGreen,
                ["DarkOrange"] = NamedColors<TPixel>.DarkOrange,
                ["DarkOrchid"] = NamedColors<TPixel>.DarkOrchid,
                ["DarkRed"] = NamedColors<TPixel>.DarkRed,
                ["DarkSalmon"] = NamedColors<TPixel>.DarkSalmon,
                ["DarkKhaki"] = NamedColors<TPixel>.DarkKhaki,
                ["DarkSeaGreen"] = NamedColors<TPixel>.DarkSeaGreen,
                ["CornflowerBlue"] = NamedColors<TPixel>.CornflowerBlue,
                ["Chocolate"] = NamedColors<TPixel>.Chocolate,
                ["AntiqueWhite"] = NamedColors<TPixel>.AntiqueWhite,
                ["Aqua"] = NamedColors<TPixel>.Aqua,
                ["Aquamarine"] = NamedColors<TPixel>.Aquamarine,
                ["Azure"] = NamedColors<TPixel>.Azure,
                ["Beige"] = NamedColors<TPixel>.Beige,
                ["Bisque"] = NamedColors<TPixel>.Bisque,
                ["Coral"] = NamedColors<TPixel>.Coral,
                ["Black"] = NamedColors<TPixel>.Black,
                ["Blue"] = NamedColors<TPixel>.Blue,
                ["BlueViolet"] = NamedColors<TPixel>.BlueViolet,
                ["Brown"] = NamedColors<TPixel>.Brown,
                ["BurlyWood"] = NamedColors<TPixel>.BurlyWood,
                ["CadetBlue"] = NamedColors<TPixel>.CadetBlue,
                ["Chartreuse"] = NamedColors<TPixel>.Chartreuse,
                ["BlanchedAlmond"] = NamedColors<TPixel>.BlanchedAlmond,
                ["DarkSlateBlue"] = NamedColors<TPixel>.DarkSlateBlue,
                ["DarkSlateGray"] = NamedColors<TPixel>.DarkSlateGray,
                ["DarkTurquoise"] = NamedColors<TPixel>.DarkTurquoise,
                ["Indigo"] = NamedColors<TPixel>.Indigo,
                ["Ivory"] = NamedColors<TPixel>.Ivory,
                ["Khaki"] = NamedColors<TPixel>.Khaki,
                ["Lavender"] = NamedColors<TPixel>.Lavender,
                ["LavenderBlush"] = NamedColors<TPixel>.LavenderBlush,
                ["LawnGreen"] = NamedColors<TPixel>.LawnGreen,
                ["IndianRed"] = NamedColors<TPixel>.IndianRed,
                ["LemonChiffon"] = NamedColors<TPixel>.LemonChiffon,
                ["LightCoral"] = NamedColors<TPixel>.LightCoral,
                ["LightCyan"] = NamedColors<TPixel>.LightCyan,
                ["LightGoldenrodYellow"] = NamedColors<TPixel>.LightGoldenrodYellow,
                ["LightGray"] = NamedColors<TPixel>.LightGray,
                ["LightGreen"] = NamedColors<TPixel>.LightGreen,
                ["LightPink"] = NamedColors<TPixel>.LightPink,
                ["LightBlue"] = NamedColors<TPixel>.LightBlue,
                ["HotPink"] = NamedColors<TPixel>.HotPink,
                ["Honeydew"] = NamedColors<TPixel>.Honeydew,
                ["GreenYellow"] = NamedColors<TPixel>.GreenYellow,
                ["DarkViolet"] = NamedColors<TPixel>.DarkViolet,
                ["DeepPink"] = NamedColors<TPixel>.DeepPink,
                ["DeepSkyBlue"] = NamedColors<TPixel>.DeepSkyBlue,
                ["DimGray"] = NamedColors<TPixel>.DimGray,
                ["DodgerBlue"] = NamedColors<TPixel>.DodgerBlue,
                ["Firebrick"] = NamedColors<TPixel>.Firebrick,
                ["FloralWhite"] = NamedColors<TPixel>.FloralWhite,
                ["ForestGreen"] = NamedColors<TPixel>.ForestGreen,
                ["Fuchsia"] = NamedColors<TPixel>.Fuchsia,
                ["Gainsboro"] = NamedColors<TPixel>.Gainsboro,
                ["GhostWhite"] = NamedColors<TPixel>.GhostWhite,
                ["Gold"] = NamedColors<TPixel>.Gold,
                ["Goldenrod"] = NamedColors<TPixel>.Goldenrod,
                ["Gray"] = NamedColors<TPixel>.Gray,
                ["Green"] = NamedColors<TPixel>.Green,
            };

            if (dict.TryGetValue(name, out var pixel))
            {
                return pixel;
            }
            return null;
        }
        public IBrush<TPixel> AsBrush<TPixel>() where TPixel : struct, IPixel<TPixel>
        {
            // lets asume the brush is a color for now
            // TODO update ColourBuilder to expose named colors
            var value = this.Value;
            if (string.IsNullOrWhiteSpace(value) || value.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                // if a null prush is returned we should skip
                return null;
            }


            var color = GetNamedColor<TPixel>(Value) ?? ColorBuilder<TPixel>.FromHex(value);


            if(Opacity == 0)
            {
                // no-op push here to efficienty sake
                color = NamedColors<TPixel>.Transparent;
            } else if (Opacity != 1)
            {
                var vector = color.ToScaledVector4();
                vector.W *= Opacity;
                color.FromScaledVector4(vector);
            }

            return new SolidBrush<TPixel>(color);

        }
    }


}
