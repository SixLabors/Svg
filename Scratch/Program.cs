using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Svg;
namespace Scratch
{
    class Program
    {

        static void Main(string[] args)
        {
            using (var img = SvgImageRenderer.LoadFromString<SixLabors.ImageSharp.PixelFormats.Rgba32>(File.ReadAllText("source.svg")))
            {
                img.Save("source.png");
            }
            //using (var img = SvgImageRenderer.LoadFromString<SixLabors.ImageSharp.PixelFormats.Rgba32>(File.ReadAllText("tiger.svg")))
            //{
            //    img.Save("tiger.png");
            //}
        }
    }
}