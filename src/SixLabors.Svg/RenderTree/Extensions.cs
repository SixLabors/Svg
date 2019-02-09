using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Shapes;
using SVGSharpie;
using System.Numerics;

namespace SixLabors.Svg.Dom
{
    internal static class Extensions
    {
        public static Matrix3x2 AsMatrix3x2(this SvgMatrix matrix)
        {
            return new Matrix3x2(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);
        }

        public static TPixel As<TPixel>(this SvgColor value, float opactiy) where TPixel : struct, IPixel<TPixel>
        {
            var colorRgb = new Rgba32(value.R, value.G, value.B, (byte)(opactiy * value.A));

            var color = default(TPixel);
            color.FromRgba32(colorRgb);

            return color;
        }

        public static JointStyle AsJointStyle(this StyleProperty<SvgStrokeLineJoin> join) => join.Value.AsJointStyle();

        public static JointStyle AsJointStyle(this SvgStrokeLineJoin join)
        {
            switch (join)
            {
                case SvgStrokeLineJoin.Miter:
                    return JointStyle.Miter;
                case SvgStrokeLineJoin.Round:
                    return JointStyle.Round;
                case SvgStrokeLineJoin.Bevel:
                    return JointStyle.Square;
                case SvgStrokeLineJoin.Inherit:
                default:
                    return JointStyle.Miter;
            }
        }
        public static EndCapStyle AsEndCapStyle(this StyleProperty<SvgStrokeLineCap> join) => join.Value.AsEndCapStyle();

        public static EndCapStyle AsEndCapStyle(this SvgStrokeLineCap join)
        {
            switch (join)
            {
                case SvgStrokeLineCap.Butt:
                    return EndCapStyle.Butt;
                case SvgStrokeLineCap.Round:
                    return EndCapStyle.Round;
                case SvgStrokeLineCap.Square:
                    return EndCapStyle.Square;
                case SvgStrokeLineCap.Inherit:
                default:
                    return EndCapStyle.Butt;
            }
        }
    }
}
