using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Css;
using AngleSharp.Dom.Svg;
using AngleSharp.Network;
using SixLabors.Svg.Dom;

namespace SixLabors.Svg
{
    public partial class SvgImage
    {

        public static Task<SvgImage> LoadFromFileAsync(string path)
        {
            var content = File.ReadAllText(path);
            return LoadFromAsync(content, false);
        }

        public static Task<SvgImage> LoadFromStringAsync(string content)
        {
            return LoadFromAsync(content, false);
        }

        public static Task<SvgImage> LoadFromUrlAsync(string url)
        {
            return LoadFromAsync(url, true);
        }

        public static async Task<SvgImage> LoadFromAsync(string content, bool isUrl)
        {
            var config = Configuration.Default.WithDefaultLoader().WithCss();
            var context = BrowsingContext.New(config);

            IDocument doc;
            if (isUrl)
            {
                doc = await context.OpenAsync(content);
            }
            else
            {
                doc = await context.OpenAsync(res =>
                {
                    res.Content(content);

                    res.Header(HeaderNames.ContentType, "image/svg+xml");
                });
            }

            var svgElement = doc as AngleSharp.Dom.Svg.ISvgDocument;

            if (svgElement == null)
            {
                throw new Exception("Failed to load document");
            }

            var dom = await SvgElement.LoadElementAsync(svgElement.DocumentElement as ISvgElement);

            return new SvgImage(dom as SvgLayer);
        }
    }
}
