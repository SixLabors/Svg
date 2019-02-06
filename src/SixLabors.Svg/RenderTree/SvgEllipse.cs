using System;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgEllipse : SvgElement
    {

        public SvgUnitValue X { get; private set; }
        public SvgUnitValue Y { get; private set; }
        public SvgUnitValue RadiusX { get; private set; }
        public SvgUnitValue RadiusY { get; private set; }
        public SvgPaint Fill { get; private set; }
        public SvgPaint Stroke { get; private set; }
        public SvgUnitValue StrokeWidth { get; private set; }

        public static Task<SvgElement> LoadAsync(ISvgElement element)
        {
            var ellipse = new SvgEllipse()
            {
                Fill = SvgPaint.Parse(element.Attributes["fill"]?.Value ?? "Black", element.Attributes["fill-opacity"]?.Value ?? "1"),
                Stroke = SvgPaint.Parse(element.Attributes["stroke"]?.Value ?? "None", element.Attributes["stroke-opacity"]?.Value ?? "1"),
                StrokeWidth = SvgUnitValue.Parse(element.Attributes["stroke-width"]?.Value ?? "1"),
                X = SvgUnitValue.Parse(element.Attributes["cx"]?.Value),
                Y = SvgUnitValue.Parse(element.Attributes["cy"]?.Value)
            };
            if (element.TagName == "circle")
            {
                ellipse.RadiusY = ellipse.RadiusX = SvgUnitValue.Parse(element.Attributes["r"]?.Value ?? "0");
            }
            else
            {
                ellipse.RadiusY = SvgUnitValue.Parse(element.Attributes["ry"]?.Value ?? "0");
                ellipse.RadiusX = SvgUnitValue.Parse(element.Attributes["rx"]?.Value ?? "0");
            }

            return Task.FromResult<SvgElement>(ellipse);
        }

        internal override void RenderTo<TPixel>(Image<TPixel> image)
        {
            var rect = new SixLabors.Shapes.EllipsePolygon(X.AsPixelXAxis(image), Y.AsPixelYAxis(image), RadiusX.AsPixelXAxis(image) * 2, RadiusY.AsPixelXAxis(image) * 2);

            var fillBrush = Fill.AsBrush<TPixel>();
            var strokeBrush = Stroke.AsBrush<TPixel>();
            var strokeWidth = this.StrokeWidth.AsPixelXAxis(image);
            image.Mutate(x =>
            {
                if (fillBrush != null)
                {
                    x = x.Fill(fillBrush, rect);
                }
                if (strokeBrush != null && strokeWidth > 0)
                {
                    var outline = Outliner.GenerateOutline(rect, strokeWidth);
                    x = x.Fill(strokeBrush, outline);
                }
            });
        }


    }


}
