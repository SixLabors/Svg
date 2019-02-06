using System;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace Scratch
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            using (var img = await SixLabors.Svg.SvgImage.LoadFromFileAsync<SixLabors.ImageSharp.PixelFormats.Rgba32>("source.svg"))
            {
                img.Save("source.png");
            }
        }
    }
}