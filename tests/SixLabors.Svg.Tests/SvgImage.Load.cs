using System;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.Svg.Dom;
using Xunit;

namespace SixLabors.Svg.Tests
{
    public class UnitTest1
    {

        [Fact]
        public async Task Test1()
        {
            var path = Utils.GetPath("TestCases", "styling-css-01-b", "source.svg");
            var img = await SixLabors.Svg.SvgImage.LoadFromFileAsync(path);
        }

        [Fact]
        public async Task FromString()
        {
            var img = await SixLabors.Svg.SvgImage.LoadFromStringAsync(@"<svg></svg>");

            Assert.NotNull(img.root);
            Assert.Equal(0, img.root.Children.Count);
        }

        [Fact]
        public async Task FromStringRect()
        {
            var img = await SixLabors.Svg.SvgImage.LoadFromStringAsync(@"<svg><rect x='1' y='2mm' width='3in' height='4%'/></svg>");

            Assert.NotNull(img.root);
            Assert.Equal(1, img.root.Children.Count);
            var rect =Assert.IsType<SvgRect>(img.root.Children.First());
            Assert.Equal(1, rect.X.Value);
            Assert.Equal(SvgUnitValue.Units.undefined, rect.X.Unit);
            Assert.Equal(2, rect.Y.Value);
            Assert.Equal(SvgUnitValue.Units.mm, rect.Y.Unit);
            Assert.Equal(3, rect.Width.Value);
            Assert.Equal(SvgUnitValue.Units.inches, rect.Width.Unit);
            Assert.Equal(4, rect.Height.Value);
            Assert.Equal(SvgUnitValue.Units.percent, rect.Height.Unit);
        }
    }
}
