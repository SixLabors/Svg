using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Shapes;
using System;
using System.Linq;
using System.Reflection;

namespace SixLabors.Svg.Dom
{

    internal struct SvgUnitValue
    {
        public static readonly SvgUnitValue Unset = default(SvgUnitValue);
        public static readonly SvgUnitValue Zero = new SvgUnitValue(0, Units.undefined);
        public static readonly SvgUnitValue One = new SvgUnitValue(1, Units.undefined);

        public bool IsSet { get; }
        public float Value { get; }
        public Units Unit { get; }

        public SvgUnitValue(float value, Units unit)
        {
            IsSet = true;
            Value = value;
            Unit = unit;
        }
        public float AsPixelXAxis<TPixel>(Image<TPixel> img) where TPixel : struct, IPixel<TPixel>
        {
            return AsPixel((float)img.MetaData.HorizontalResolution, img.Width);
        }

        public float AsPixelYAxis<TPixel>(Image<TPixel> img) where TPixel : struct, IPixel<TPixel>
        {
            return AsPixel((float)img.MetaData.VerticalResolution, img.Height);
        }

        public float AsPixel(float dpi, float maxAxis = 0)
        {
            switch (Unit)
            {
                case Units.undefined:
                    return Value;
                    break;
                case Units.px:
                    return Value;
                case Units.cm:
                    return (float)(dpi / 2.54) * Value;
                case Units.mm:
                    return (float)(dpi / 2.54) * Value / 10;
                case Units.pt:
                    return dpi / 72 * Value;
                case Units.pc:
                    return dpi / 6 * Value;
                    break;
                case Units.inches:
                    return Value * dpi;

                case Units.percent:
                    return (Value / 100) * maxAxis;
                case Units.em:
                case Units.ex:
                default:
                    return Value;
            }
        }

        public static SvgUnitValue? Parse(string val)
        {
            val = val?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(val))
            {
                var valNum = val;
                Units unitType = Units.undefined;

                if (val.EndsWith("%"))
                {
                    unitType = Units.percent;
                    valNum = val.TrimEnd('%');
                }
                else
                {
                    if (val.Length > 2)
                    {
                        var unit = val.Substring(val.Length - 2);
                        if (unit == "in")
                        {
                            unitType = Units.inches;
                        }
                        else
                        if (!Enum.TryParse(unit, true, out unitType) || !Enum.IsDefined(typeof(Units), unitType))
                        {
                            unitType = Units.undefined;
                        }

                        if (unitType != Units.undefined)
                        {
                            valNum = valNum.Substring(0, val.Length - 2);
                        }
                    }
                }

                float finalVal = 0;
                if (float.TryParse(valNum, out finalVal))
                {
                    return new SvgUnitValue(finalVal, unitType);
                }
            }

            return null;
        }

        public enum Units
        {
            undefined,
            percent,
            px,
            cm,
            mm,
            em,
            ex,
            pt,
            pc,
            inches, // in
        }
    }


    internal struct SvgLineCap
    {
        public static readonly SvgLineCap Unset = default(SvgLineCap);
        public static readonly SvgLineCap Butt = new SvgLineCap(EndCapStyle.Butt);
        public static readonly SvgLineCap Round = new SvgLineCap(EndCapStyle.Round);
        public static readonly SvgLineCap Square = new SvgLineCap(EndCapStyle.Square);

        public bool IsSet { get; }

        public EndCapStyle Style { get; private set; }

        public SvgLineCap(EndCapStyle style)
        {
            this.IsSet = true;
            this.Style = style;
        }

        public static SvgLineCap? Parse(string val)
        {
            val = (val?.Trim().ToLower() ?? "");
            switch (val)
            {
                case "butt":
                    return SvgLineCap.Butt;
                case "round":
                    return SvgLineCap.Round;
                case "square":
                    return SvgLineCap.Square;
                default:
                    return null;
            }
        }
    }
    internal struct SvgLineJoin
    {
        public static readonly SvgLineJoin Unset = default(SvgLineJoin);
        public static readonly SvgLineJoin Miter = new SvgLineJoin(JointStyle.Miter);
        public static readonly SvgLineJoin Round = new SvgLineJoin(JointStyle.Round);
        public static readonly SvgLineJoin Square = new SvgLineJoin(JointStyle.Square);

        public bool IsSet { get; }

        public JointStyle Style { get; private set; }

        public SvgLineJoin(JointStyle style)
        {
            this.IsSet = true;
            this.Style = style;
        }

        public static SvgLineJoin? Parse(string val)
        {
            val = (val?.Trim().ToLower() ?? "");
            switch (val)
            {
                case "miter":
                    return SvgLineJoin.Miter;
                case "round":
                    return SvgLineJoin.Round;
                case "bevel":
                    return SvgLineJoin.Square;
                default:
                    return null;
            }
        }
    }

}
