using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.Svg.Dom
{
    internal abstract class SvgElement
    {
        public SvgLayer Parent { get; private set; }

        public static async Task<SvgElement> LoadElementAsync(ISvgElement element)
        {
            switch (element.TagName)
            {
                case "svg":
                case "g":
                    return await SvgLayer.LoadLayerAsync(element);
                case "rect":
                    return await SvgRect.LoadAsync(element);
                case "circle":
                case "ellipse":
                    return await SvgEllipse.LoadAsync(element);
                default:
                    return null;
            }
        }

        internal void SetParent(SvgLayer layer)
        {
            Parent = layer;
        }

        internal abstract void RenderTo<TPixel>(Image<TPixel> image) where TPixel : struct, IPixel<TPixel>;
    }


}
