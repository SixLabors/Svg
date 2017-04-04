using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Svg;

namespace SixLabors.Svg.Dom
{
    internal sealed class SvgLayer : SvgElement
    {
        private List<SvgElement> children = new List<SvgElement>();
        public IReadOnlyList<SvgElement> Children => children;

        public void Add(SvgElement elm)
        {
            children.Add(elm);
            elm.SetParent(this);
        }

        internal override void RenderTo(RasterImage img)
        {
            // note to self each lay can be draw on the previous layer with an image brush for masking and compositing
            foreach (var c in Children)
            {
                c.RenderTo(img);
            }
        }

        public static async Task<SvgElement> LoadLayerAsync(AngleSharp.Dom.Svg.ISvgElement element)
        {
            var layer = new SvgLayer();
            var children = element.Children.OfType<ISvgElement>();
            foreach (var c in children)
            {
                var elm = await SvgElement.LoadElementAsync(c);
                if (elm != null)
                {
                    layer.Add(elm);
                }
            }

            return layer;
        }
    }

    internal abstract class SvgElement
    {
        public SvgLayer Parent { get; private set; }


        public static async Task<SvgElement> LoadElementAsync(AngleSharp.Dom.Svg.ISvgElement element)
        {
            switch (element.TagName)
            {
                case "svg":
                case "g":
                    return await SvgLayer.LoadLayerAsync(element);
                case "rect":
                    return await SvgRect.LoadAsync(element);
                default:
                    return null;
            }
        }

        internal void SetParent(SvgLayer layer)
        {
            Parent = layer;
        }

        internal abstract void RenderTo(RasterImage writer);
    }

    internal sealed class SvgRect : SvgElement
    {
        internal override void RenderTo(RasterImage writer)
        {
            throw new NotImplementedException();
        }

        public SvgUnitValue X { get; private set; }
        public SvgUnitValue Y { get; private set; }
        public SvgUnitValue Width { get; private set; }
        public SvgUnitValue Height { get; private set; }

        public static Task<SvgElement> LoadAsync(AngleSharp.Dom.Svg.ISvgElement element)
        {
            return Task.FromResult<SvgElement>(new SvgRect()
            {
                X = SvgUnitValue.Parse(element.Attributes["x"]?.Value),
                Y = SvgUnitValue.Parse(element.Attributes["y"]?.Value),
                Width = SvgUnitValue.Parse(element.Attributes["width"]?.Value),
                Height = SvgUnitValue.Parse(element.Attributes["height"]?.Value),
            });
        }
    }

    internal struct SvgUnitValue
    {
        public static readonly SvgUnitValue Unset = default(SvgUnitValue);

        public bool IsSet { get; }
        public float Value { get; }
        public Units Unit { get; }

        public SvgUnitValue(float value, Units unit)
        {
            IsSet = true;
            Value = value;
            Unit = unit;
        }

        public static SvgUnitValue Parse(string val)
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
                        if(unit == "in")
                        {
                            unitType = Units.inches;
                        }
                        else
                        if (!Enum.TryParse(unit, true, out unitType))
                        {
                            unitType = Units.undefined;
                        }

                        if(unitType != Units.undefined)
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

            return Unset;
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

    
}
