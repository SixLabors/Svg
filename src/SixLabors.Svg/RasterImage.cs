using System;
using System.Collections.Generic;
using System.Text;
using ImageSharp;

namespace SixLabors.Svg
{
    internal class RasterImage : IDisposable
    {
        private readonly Image image;

        public RasterImage(int width, int height)
        {
            this.image = new ImageSharp.Image(width, height);
        }

        public void Dispose()
        {

        }

        internal RasterImage CreateChild()
        {
            throw new NotImplementedException();
        }
    }
}
