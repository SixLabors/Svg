using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Css;
using SixLabors.Svg.Dom;
using AngleSharp.Io;
using AngleSharp.Svg.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Configuration = AngleSharp.Configuration;

namespace SixLabors.Svg
{
    public static class SvgImage
    {
        public static Task<Image<TPixel>> LoadFromFileAsync<TPixel>(string path)
            where TPixel : struct, IPixel<TPixel>
        {
            var content = File.ReadAllText(path);
            return LoadFromAsync<TPixel>(content, false);
        }

        public static Task<Image<TPixel>> LoadFromStringAsync<TPixel>(string content)
            where TPixel : struct, IPixel<TPixel>
        {
            return LoadFromAsync<TPixel>(content, false);
        }

        public static Task<Image<TPixel>> LoadFromUrlAsync<TPixel>(Uri url)
            where TPixel : struct, IPixel<TPixel>
        {
            return LoadFromAsync<TPixel>(url.ToString(), true);
        }

        public static async Task<Image<TPixel>> LoadFromAsync<TPixel>(string content, bool isUrl)
            where TPixel : struct, IPixel<TPixel>
        {
            var config = Configuration.Default.WithDefaultLoader();
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

            var svgElement = doc as ISvgDocument;

            if (svgElement == null)
            {
                throw new Exception("Failed to load document");
            }

            var dom = await SvgDocument.LoadAsync(svgElement.DocumentElement as ISvgElement);

            return dom.Generate<TPixel>(new RenderOptions
            {
                Dpi = 96
            });
        }
    }
}
