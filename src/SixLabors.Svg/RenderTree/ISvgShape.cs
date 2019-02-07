namespace SixLabors.Svg.Dom
{
    internal interface ISvgShape
    {
        SvgPaint Fill { get; }
        SvgPaint Stroke { get; }
        SvgUnitValue StrokeWidth { get; }
        SvgLineCap StrokeLineCap { get; }
        SvgLineJoin StrokeLineJoin { get; }
    }
}
