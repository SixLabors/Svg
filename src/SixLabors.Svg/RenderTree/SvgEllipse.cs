using System;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgEllipse : SvgElement, ISvgShape
    {

        public SvgUnitValue X { get; private set; }
        public SvgUnitValue Y { get; private set; }
        public SvgUnitValue RadiusX { get; private set; }
        public SvgUnitValue RadiusY { get; private set; }
        public SvgPaint Fill { get; private set; }
        public SvgPaint Stroke { get; private set; }
        public SvgUnitValue StrokeWidth { get; private set; }
        public SvgLineCap StrokeLineCap { get; private set; }
        public SvgLineJoin StrokeLineJoin { get; private set; }

        public static Task<SvgElement> LoadAsync(ISvgElement element)
        {
            var ellipse = new SvgEllipse()
            {
                Fill = element.GetPaint("fill", "Black", "1"),
                Stroke = element.GetPaint("stroke", "None", "1"),
                StrokeWidth = element.GetUnitValue("stroke-width", "1"),
                StrokeLineCap = element.GetLineCap("stroke-linecap", "butt"),
                StrokeLineJoin = element.GetLineJoin("stroke-linejoin", "miter"),
                X = element.GetUnitValue("cx"),
                Y = element.GetUnitValue("cy"),
            };

            if (element.TagName == "circle")
            {
                ellipse.RadiusY = ellipse.RadiusX = element.GetUnitValue("r", "0");
            }
            else
            {
                ellipse.RadiusX = element.GetUnitValue("rx", "0");
                ellipse.RadiusY = element.GetUnitValue("ry", "0");
            }

            return Task.FromResult<SvgElement>(ellipse);
        }

        internal override void RenderTo<TPixel>(Image<TPixel> image)
        {
            var rect = new SixLabors.Shapes.EllipsePolygon(X.AsPixelXAxis(image), Y.AsPixelYAxis(image), RadiusX.AsPixelXAxis(image) * 2, RadiusY.AsPixelXAxis(image) * 2);

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
