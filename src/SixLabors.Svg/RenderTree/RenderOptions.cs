namespace SixLabors.Svg.Dom
{
    public struct RenderOptions
    {
        private const float defaultDpi = 96;
        private float? dpi;
        public float Dpi { get => dpi ?? defaultDpi; set => dpi = value; }
    }


}
