using System;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SixLabors.Shapes.Text;
using SixLabors.Svg.Shapes;
using SVGSharpie;

namespace SixLabors.Svg.Dom
{
    internal sealed partial class SvgDocumentRenderer<TPixel> : SvgElementWalker
        where TPixel : struct, IPixel<TPixel>
    {
        public static string DefaultFont { get; set; } = "Times New Roman";
        public static string DefaultSansSerifFont { get; set; } = "Arial";
        public static string DefaultSerifFont { get; set; } = "Times New Roman";
        public override void VisitTextElement(SvgTextElement element)
        {
            base.VisitTextElement(element);

            var fonts = SystemFonts.Collection;
            FontFamily family = null;
            if (element.PresentationStyleData.FontFamily.HasValue)
            {
                foreach (var f in element.PresentationStyleData.FontFamily.Value.Value)
                {
                    var fontName = f;
                    if (fontName.Equals("sans-serif"))
                    {
                        fontName = DefaultSansSerifFont;
                    }
                    else if (fontName.Equals("serif"))
                    {
                        fontName = DefaultSerifFont;
                    }

                    if (fonts.TryFind(fontName, out family))
                    {
                        break;
                    }
                }
            }

            if (family == null)
            {
                family = fonts.Find(DefaultFont);
            }

            var fontSize = element.PresentationStyleData.FontSize?.Value.Value ?? 12;
            var origin = new PointF(element.X?.Value ?? 0, element.Y?.Value ?? 0);
            var font = family.CreateFont(fontSize);
            var render = new RendererOptions(font, 72);
            var text = element.Text;


            var lineHeight = ((font.LineHeight * font.Size) / (font.EmSize * 72)) * 72;
            var ascender = ((font.Ascender * font.Size) / (font.EmSize * 72)) * 72;

            var glyphs = TextBuilder.GenerateGlyphs(text, new RendererOptions(font, 72, origin - new PointF(0, ascender)));
            foreach (var p in glyphs)
            {
                this.RenderShapeToCanvas(element, p);
            }
        }

    }
}
