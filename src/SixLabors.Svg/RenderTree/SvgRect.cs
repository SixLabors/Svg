using System;
using System.Numerics;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgRect : SvgElement
    {

        public SvgUnitValue X { get; private set; }
        public SvgUnitValue Y { get; private set; }
        public SvgUnitValue Width { get; private set; }
        public SvgUnitValue Height { get; private set; }
        public SvgPaint Fill { get; private set; }
        public SvgPaint Stroke { get; private set; }
        public SvgUnitValue StrokeWidth { get; private set; }
        public SvgUnitValue RadiusX { get; private set; }
        public SvgUnitValue RadiusY { get; private set; }

        public static Task<SvgElement> LoadAsync(ISvgElement element)
        {
            return Task.FromResult<SvgElement>(new SvgRect()
            {
                Fill = SvgPaint.Parse(element.Attributes["fill"]?.Value ?? "Black", element.Attributes["fill-opacity"]?.Value ?? "1"),
                Stroke = SvgPaint.Parse(element.Attributes["stroke"]?.Value ?? "None", element.Attributes["stroke-opacity"]?.Value ?? "1"),
                StrokeWidth = SvgUnitValue.Parse(element.Attributes["stroke-width"]?.Value ?? "1"),
                X = SvgUnitValue.Parse(element.Attributes["x"]?.Value),
                Y = SvgUnitValue.Parse(element.Attributes["y"]?.Value),
                RadiusX = SvgUnitValue.Parse(element.Attributes["rx"]?.Value ?? element.Attributes["ry"]?.Value ?? "0"),
                RadiusY = SvgUnitValue.Parse(element.Attributes["ry"]?.Value ?? element.Attributes["rx"]?.Value ?? "0"),
                Width = SvgUnitValue.Parse(element.Attributes["width"]?.Value),
                Height = SvgUnitValue.Parse(element.Attributes["height"]?.Value),
            });
        }

        internal override void RenderTo<TPixel>(Image<TPixel> image)
        {
            IPath rect = new SixLabors.Shapes.RectangularPolygon(X.AsPixelXAxis(image), Y.AsPixelYAxis(image), Width.AsPixelXAxis(image), Height.AsPixelXAxis(image));

            var rx = RadiusX.AsPixelXAxis(image);
            var ry = RadiusY.AsPixelXAxis(image);

            if (rx > 0 && ry > 0)
            {
                rect = MakeRounded(rect, rx, ry);
            }

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

        private IPath MakeRounded(IPath path, float rx, float ry)
        {
            return path.Clip(BuildCorners(path.Bounds.Width, path.Bounds.Height, rx, ry).Translate(path.Bounds.Location));
        }

        public static IPathCollection BuildCorners(float imageWidth, float imageHeight, float cornerRadiusX, float cornerRadiusY)
        {
            // first create a square
            var rect = new RectangularPolygon(0, 0, cornerRadiusX, cornerRadiusY);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerToptLeft = rect.Clip(new EllipsePolygon(cornerRadiusX, cornerRadiusY, cornerRadiusX * 2, cornerRadiusY * 2));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the orgional artound the center of the image
            var center = new Vector2(imageWidth / 2F, imageHeight / 2F);

            float rightPos = imageWidth - cornerToptLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerToptLeft.Bounds.Height + 1;

            // move it across the widthof the image - the width of the shape
            IPath cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos - 0.5f, 0);
            IPath cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos - 0.5f);
            IPath cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos - 0.5f, bottomPos - 0.5f);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }


    }


}
