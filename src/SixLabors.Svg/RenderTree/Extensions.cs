using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.Shapes;
using AngleSharp.Dom;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.Svg.Dom
{
    internal static class Extensions
    {
        public static IPath GenerateStroke<TPixel>(this IPath path, Image<TPixel> img, ISvgShape shape) where TPixel : struct, IPixel<TPixel>
        {
            var strokeWidth = shape.StrokeWidth.AsPixelXAxis(img);
            if (strokeWidth == 0)
            {
                return null;
            }

            return Outliner.GenerateOutline(path, strokeWidth, shape.StrokeLineJoin.Style, shape.StrokeLineCap.Style);
        }

        public static string GetAttributeValueSelfOrGroup(this ISvgElement element, string attribute)
        {
            var val = element.Attributes[attribute]?.Value;

            if (val == null)
            {
                val = element.Ancestors<ISvgElement>().Where(x => x.TagName == "g").FirstOrDefault()?.GetAttributeValueSelfOrGroup(attribute);
            }

            return val;
        }

        public static SvgPaint GetPaint(this ISvgElement element, string attribute, string defaultPaintValue, string defaultOpacityValue)
        {
            var paint = element.GetAttributeValueSelfOrGroup(attribute) ?? defaultPaintValue;
            var opacity = element.GetAttributeValueSelfOrGroup(attribute + "-opacity") ?? defaultOpacityValue;

            return SvgPaint.Parse(paint, opacity);
        }

        public static SvgUnitValue GetUnitValue(this ISvgElement element, string attribute, string defaultValue = null)
        {
            var val = element.TryGetUnitValue(attribute, defaultValue);

            return val ?? SvgUnitValue.Unset;
        }
        public static SvgUnitValue? TryGetUnitValue(this ISvgElement element, string attribute, string defaultValue = null)
        {
            var val = element.GetAttributeValueSelfOrGroup(attribute) ?? defaultValue;

            return SvgUnitValue.Parse(val);
        }

        public static SvgLineCap GetLineCap(this ISvgElement element, string attribute, string defaultValue = null)
        {
            var val = element.TryGetLineCap(attribute, defaultValue);

            return val ?? SvgLineCap.Unset;
        }

        public static SvgLineCap? TryGetLineCap(this ISvgElement element, string attribute, string defaultValue = null)
        {
            var val = element.GetAttributeValueSelfOrGroup(attribute) ?? defaultValue;

            return SvgLineCap.Parse(val);
        }


        public static SvgLineJoin GetLineJoin(this ISvgElement element, string attribute, string defaultValue = null)
        {
            var val = element.TryGetLineJoin(attribute, defaultValue);

            return val ?? SvgLineJoin.Unset;
        }

        public static SvgLineJoin? TryGetLineJoin(this ISvgElement element, string attribute, string defaultValue = null)
        {
            var val = element.GetAttributeValueSelfOrGroup(attribute) ?? defaultValue;

            return SvgLineJoin.Parse(val);
        }

    }
}
