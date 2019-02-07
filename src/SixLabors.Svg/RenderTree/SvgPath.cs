using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgPath : SvgElement, ISvgShape
    {
        public IPath Path { get; private set; }

        public SvgPaint Fill { get; private set; }
        public SvgPaint Stroke { get; private set; }
        public SvgUnitValue StrokeWidth { get; private set; }
        public SvgLineCap StrokeLineCap { get; private set; }
        public SvgLineJoin StrokeLineJoin { get; private set; }

        public IEnumerable<SvgPathOperation> PathOperations { get; private set; }
        public static Task<SvgElement> LoadAsync(ISvgElement element)
        {
            var path = new SvgPath()
            {
                Fill = element.GetPaint("fill", "Black", "1"),
                Stroke = element.GetPaint("stroke", "None", "1"),
                StrokeWidth = element.GetUnitValue("stroke-width", "1"),
                StrokeLineCap = element.GetLineCap("stroke-linecap", "butt"),
                StrokeLineJoin = element.GetLineJoin("stroke-linejoin", "miter"),
            };

            if (element.TagName == "line")
            {
                path.PathOperations = new[] {
                    SvgPathOperation.MoveTo(element.GetUnitValue("x1", "0"), element.GetUnitValue("y1", "0"), false),
                    SvgPathOperation.LineTo(element.GetUnitValue("x2", "0"), element.GetUnitValue("y2", "0"), false),
                };
            }
            else if (element.TagName == "polyline" || element.TagName == "polygon")
            {
                var pathData = element.GetAttributeValueSelfOrGroup("points");
                path.PathOperations = ParsePoints(pathData, element.TagName == "polygon");
            }
            else if (element.TagName == "path")
            {
                var pathData = element.GetAttributeValueSelfOrGroup("d");
                path.PathOperations = SvgPathOperation.Parse(pathData);
            }

            if (path.PathOperations == null)
            {
                return Task.FromResult<SvgElement>(null);
            }

            return Task.FromResult<SvgElement>(path);
        }

        private static IEnumerable<SvgPathOperation> ParsePoints(string pathData, bool closePath)
        {
            if (pathData != null && !pathData.Equals("none", System.StringComparison.OrdinalIgnoreCase))
            {
                var parts = pathData.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
                {
                    if (int.TryParse(x, out var i))
                    {
                        return i;
                    }
                    else
                    { return (int?)null; }
                }).Where(x => x != null).Select(x => x.Value).ToArray();
                if (parts.Length > 4)
                {
                    var len = parts.Length / 2;
                    if (closePath)
                    {
                        len++;
                    }

                    var ops = new SvgPathOperation[len];
                    ops[0] = SvgPathOperation.MoveTo(new SvgUnitValue(parts[0], SvgUnitValue.Units.undefined), new SvgUnitValue(parts[1], SvgUnitValue.Units.undefined), false);

                    for (var i = 2; i < parts.Length; i += 2)
                    {
                        ops[i / 2] = SvgPathOperation.LineTo(new SvgUnitValue(parts[i], SvgUnitValue.Units.undefined), new SvgUnitValue(parts[i + 1], SvgUnitValue.Units.undefined), false);
                    }

                    if (closePath)
                    {
                        ops[len - 1] = SvgPathOperation.ClosePath();
                    }
                    return ops;
                }
            }

            return null;
        }

        internal override void RenderTo<TPixel>(Image<TPixel> image)
        {
            var rect = this.PathOperations.GeneratePath(image);

            var fillBrush = Fill.AsBrush<TPixel>();
            var strokeBrush = Stroke.AsBrush<TPixel>();
            image.Mutate(x =>
            {
                if (fillBrush != null)
                {
                    x = x.Fill(fillBrush, rect);
                }
                if (strokeBrush != null)
                {
                    var outline = rect.GenerateStroke(image, this);
                    if (outline != null)
                    {
                        x = x.Fill(strokeBrush, outline);
                    }
                }
            });
        }


    }


}
