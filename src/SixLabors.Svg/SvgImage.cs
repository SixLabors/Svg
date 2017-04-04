using System;
using AngleSharp.Dom.Svg;
using SixLabors.Svg.Dom;

namespace SixLabors.Svg
{
    public sealed partial class SvgImage
    {
        internal SvgLayer root;

        private SvgImage(SvgLayer root)
        {
            this.root = root;
        }
    }
}
