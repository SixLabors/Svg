using System;
using System.Numerics;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using AngleSharp.Css.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;

namespace SixLabors.Svg.Dom
{

    internal sealed class SvgRect : SvgElement, ISvgShape
    {
        public SvgUnitValue X { get; private set; }
        public SvgUnitValue Y { get; private set; }
        public SvgUnitValue Width { get; private set; }
        public SvgUnitValue Height { get; private set; }
        public SvgPaint Fill { get; private set; }
        public SvgPaint Stroke { get; private set; }
        public SvgLineCap StrokeLineCap { get; private set; }
        public SvgLineJoin StrokeLineJoin { get; private set; }
        public SvgUnitValue StrokeWidth { get; private set; }
        public SvgUnitValue RadiusX { get; private set; }
        public SvgUnitValue RadiusY { get; private set; }

        public static Task<SvgElement> LoadAsync(ISvgElement element)
        {
            var rx = element.TryGetUnitValue("rx");
            var ry = element.TryGetUnitValue("ry");
            return Task.FromResult<SvgElement>(new SvgRect()
            {
                Fill = element.GetPaint("fill", "Black", "1"),
                Stroke = element.GetPaint("stroke", "None", "1"),
                StrokeWidth = element.GetUnitValue("stroke-width", "1"),
                StrokeLineCap = element.GetLineCap("stroke-linecap", "butt"),
                StrokeLineJoin = element.GetLineJoin("stroke-linejoin", "miter"),
                X = element.GetUnitValue("x"),
                Y = element.GetUnitValue("y"),
                RadiusX = rx ?? ry ?? SvgUnitValue.Zero,
                RadiusY = ry ?? rx ?? SvgUnitValue.Zero,
                Width = element.GetUnitValue("width"),
                Height = element.GetUnitValue("height")
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

            float rightPos = imageWidth - cornerToptLeft.Bounds.Width ;
            float bottomPos = imageHeight - cornerToptLeft.Bounds.Height ;

            // move it across the widthof the image - the width of the shape
            IPath cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
