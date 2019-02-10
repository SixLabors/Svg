using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Svg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Tests.TestUtilities.ImageComparison;
using SixLabors.Svg.Dom;
using Xunit;

namespace SixLabors.Svg.Tests
{
    public class UnitTest1
    {
        public static string rootFolder = Utils.GetPath("external", "svgwg");
        public static string rootFolderOutput = Utils.GetPath("output", "svgwg");
        public static TheoryData<string, string, string> CustomPaths => Utils.SampleImages(@"tests\SixLabors.Svg.Tests\Tests Cases\Simple Text");

        [Theory]
        [MemberData(nameof(CustomPaths))]
        public async Task Test1(string svgFileName, string pngFileName, string folder)
        {
            var svgFullPath = Path.Combine(folder, svgFileName);
            var pngFullPath = Path.Combine(folder, pngFileName);
            using (var svgImg = SvgImageRenderer.LoadFromString<Rgba32>(File.ReadAllText(svgFullPath)))
            using (var pngImg = SixLabors.ImageSharp.Image.Load(pngFullPath))
            {
                var outputPath = pngFullPath.Replace(rootFolder, rootFolderOutput);
                var dir = Path.GetDirectoryName(outputPath);
                var fn = Path.GetFileNameWithoutExtension(outputPath);
                Directory.CreateDirectory(dir);
                svgImg.Save(Path.Combine(dir, fn + ".gen.png"));

                ImageComparer.Tolerant(perPixelManhattanThreshold: 500).VerifySimilarity(svgImg, pngImg);
            }
        }
    }
}
