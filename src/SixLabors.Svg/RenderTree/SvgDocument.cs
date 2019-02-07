using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgDocument
    {
        private readonly SvgLayer root;

        public SvgUnitValue X { get; private set; }
        public SvgUnitValue Y { get; private set; }
        public SvgUnitValue Width { get; private set; }
        public SvgUnitValue Height { get; private set; }

        public SvgDocument()
        {
            root = new SvgLayer();
        }

        public void Add(SvgElement elm)
        {
            root.Add(elm);
            elm.SetParent(root);
        }

        public static async Task<SvgDocument> LoadAsync(ISvgElement element)
        {
            var document = new SvgDocument()
            {
                X = element.GetUnitValue("x", "0"),
                Y = element.GetUnitValue("y", "0"),
                Width = element.GetUnitValue("width"),
                Height = element.GetUnitValue("height")
            };

            var children = element.Children.OfType<ISvgElement>();
            foreach (var c in children)
            {
                var elm = await SvgElement.LoadElementAsync(c);
                if (elm != null)
                {
                    document.Add(elm);
                }
            }

            return document;
        }

        internal Image<TPixel> Generate<TPixel>(RenderOptions options) where TPixel : struct, IPixel<TPixel>
        {
            // todo pass along the ImageSharp configuration
            var img = new Image<TPixel>((int)this.Width.AsPixel(options.Dpi), (int)this.Height.AsPixel(options.Dpi));

            this.root.RenderTo(img);

            return img;
        }
    }


}
