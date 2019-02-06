using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgLayer : SvgElement
    {
        private List<SvgElement> children = new List<SvgElement>();
        public IReadOnlyList<SvgElement> Children => children;

        public void Add(SvgElement elm)
        {
            children.Add(elm);
            elm.SetParent(this);
        }

        internal override void RenderTo<TPixel>(Image<TPixel> img)
        {
            // note to self each lay can be draw on the previous layer with an image brush for masking and compositing
            foreach (var c in Children)
            {
                c.RenderTo(img);
            }
        }

        public static async Task<SvgElement> LoadLayerAsync(ISvgElement element)
        {
            var layer = new SvgLayer();
            var children = element.Children.OfType<ISvgElement>();
            foreach (var c in children)
            {
                var elm = await SvgElement.LoadElementAsync(c);
                if (elm != null)
                {
                    layer.Add(elm);
                }
            }

            return layer;
        }
    }
}
